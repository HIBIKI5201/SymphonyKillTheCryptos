using Cryptos.Runtime.Entity.Ingame.Card;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Cryptos.Editor.Ingame
{
    [CustomEditor(typeof(CardData))]
    public class CardDataDrawer : UnityEditor.Editor
    {
        // EditorPrefsに保存するためのキー
        public const string SKILL_ANIMATION_GUID_MAP_KEY = "skill-animation-guid-map";

        // 解析結果を保持する辞書
        private Dictionary<int, AnimationClip> _skillAnimationClips = new Dictionary<int, AnimationClip>();

        public void OnEnable()
        {
            LoadData();
        }


        public override void OnInspectorGUI()
        {
            // デフォルトのインスペクターを表示
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Animation Skill Viewer", EditorStyles.boldLabel);

            // アニメーション解析ウィンドウを開くボタン
            if (GUILayout.Button("Open Animator Analyzer"))
            {
                CardDataWindow window = CardDataWindow.ShowWindow();
                window.OnDataAnalyzed += LoadData;
            }

            // 解析データが存在する場合、読み取り専用で表示
            if (_skillAnimationClips.Count > 0)
            {
                foreach (var item in _skillAnimationClips)
                {
                    EditorGUILayout.LabelField(item.ToString());
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No animation data found. Open the analyzer to generate it.", MessageType.Info);
            }
        }

        public void LoadData()
        {
            // EditorPrefsからGUIDのマップを読み込み、AnimationClipを復元する
            var json = EditorPrefs.GetString(SKILL_ANIMATION_GUID_MAP_KEY, string.Empty);
            if (string.IsNullOrEmpty(json)) return;

            var guidMap = JsonConvert.DeserializeObject<Dictionary<int, string>>(json);
            if (guidMap == null) return;

            _skillAnimationClips.Clear();
            foreach (var pair in guidMap)
            {
                var path = AssetDatabase.GUIDToAssetPath(pair.Value);
                if (!string.IsNullOrEmpty(path))
                {
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                    if (clip != null)
                    {
                        _skillAnimationClips[pair.Key] = clip;
                    }
                }
            }
        }

    }

    public class CardDataWindow : EditorWindow
    {
        /// <summary>
        ///     Animator Analyzerウィンドウを表示します。
        ///     CardDataのインスペクターから起動可能。
        /// </summary>
        public static CardDataWindow ShowWindow()
        {
            return GetWindow<CardDataWindow>("Animator Analyzer");
        }

        public event Action OnDataAnalyzed;

        [SerializeField] private AnimatorController _animatorController;
        private string _targetStateName = "Skill/Entry";
        private string _intParameterName = "Skill";

        private void OnGUI()
        {
            _animatorController = (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", _animatorController, typeof(AnimatorController), false);
            _targetStateName = EditorGUILayout.TextField("Target State Name", _targetStateName);
            _intParameterName = EditorGUILayout.TextField("Int Parameter Name", _intParameterName);

            if (GUILayout.Button("Analyze and Save to EditorPrefs"))
            {
                Dictionary<int, AnimationClip> animationDict = AnalyzeAnimator();
                if (animationDict == null) return;

                SaveAnalyzeData(animationDict);

                // ウィンドウを閉じて、インスペクターの更新を促す

                OnDataAnalyzed?.Invoke();
                this.Close();
            }
        }

        private Dictionary<int, AnimationClip> AnalyzeAnimator()
        {
            if (_animatorController == null)
            {
                Debug.LogWarning("Animator Controller を設定してください。");
                return null;
            }

            var resultDict = new Dictionary<int, AnimationClip>();
            var pathParts = _targetStateName.Split('/');

            foreach (var layer in _animatorController.layers)
            {
                TraverseStateMachine(layer.stateMachine, pathParts, 0, _intParameterName, resultDict);
            }

            var resultLog = string.Join("\n", resultDict.Select(kvp => $"  {kvp.Key}: {kvp.Value.name}"));
            Debug.Log($"✅ 解析完了 \nstate name : {_targetStateName} 結果数: {resultDict.Count}件\n{resultLog}");

            return resultDict;
        }

        private void TraverseStateMachine(
            AnimatorStateMachine currentSM,
            string[] pathParts,
            int index,
            string paramName,
            Dictionary<int, AnimationClip> resultDict)
        {
            if (index >= pathParts.Length) return;

            var currentPart = pathParts[index];

            if (index < pathParts.Length - 1)
            {
                var childSM = currentSM.stateMachines.FirstOrDefault(sm => sm.stateMachine.name == currentPart);
                if (childSM.stateMachine != null)
                {
                    TraverseStateMachine(childSM.stateMachine, pathParts, index + 1, paramName, resultDict);
                }
                else
                {
                    Debug.LogWarning($"❌ サブステートマシン '{currentPart}' が見つかりません。");
                }
                return;
            }

            FindAndAnalyzeStateTransitions(currentSM, currentPart, paramName, resultDict);
        }

        private void FindAndAnalyzeStateTransitions(AnimatorStateMachine stateMachine, string targetName, string paramName, Dictionary<int, AnimationClip> resultDict)
        {
            AnimatorTransitionBase[] transitions;

            switch (targetName)
            {
                case "Entry":
                    transitions = stateMachine.entryTransitions;
                    break;
                case "AnyState":
                    transitions = stateMachine.anyStateTransitions;
                    break;
                default:
                    var state = stateMachine.states.FirstOrDefault(s => s.state.name == targetName).state;
                    if (state == null)
                    {
                        Debug.LogWarning($"❌ ステート '{targetName}' が見つかりません。");
                        return;
                    }
                    transitions = state.transitions;
                    break;
            }

            if (transitions != null)
            {
                ExtractClipsFromTransitions(transitions, paramName, resultDict);
            }
        }

        private void ExtractClipsFromTransitions(AnimatorTransitionBase[] transitions, string paramName, Dictionary<int, AnimationClip> resultDict)
        {
            foreach (var transition in transitions)
            {
                foreach (var condition in transition.conditions)
                {
                    if (condition.parameter == paramName && condition.mode == AnimatorConditionMode.Equals)
                    {
                        var value = (int)condition.threshold;
                        if (transition.destinationState?.motion is AnimationClip clip && !resultDict.ContainsKey(value))
                        {
                            resultDict.Add(value, clip);
                        }
                    }
                }
            }
        }

        private void SaveAnalyzeData(Dictionary<int, AnimationClip> animationDict)
        {
            // アニメーションのGUIDを取得してマップを作成
            var guidMap = new Dictionary<int, string>();
            foreach (var pair in animationDict)
            {
                var path = AssetDatabase.GetAssetPath(pair.Value);
                if (!string.IsNullOrEmpty(path))
                {
                    guidMap[pair.Key] = AssetDatabase.AssetPathToGUID(path);
                }
            }

            // JSONにシリアライズしてEditorPrefsに保存
            var json = JsonConvert.SerializeObject(guidMap);
            EditorPrefs.SetString(CardDataDrawer.SKILL_ANIMATION_GUID_MAP_KEY, json);
        }
    }
}
