using Cryptos.Runtime.Entity.Ingame.Card;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Cryptos.Editor.Ingame
{
    [CustomEditor(typeof(CardData))]
    public class CardDataDrawer : UnityEditor.Editor
    {
        public const string SKILL_ANIMATION_GUID_MAP_KEY = "skill-animation-guid-map"; // EditorUserSettingsに保存するためのキー

        private const string VALIABLE_ANIMATION_CLIP_ID = "_animationClipID"; //アニメーションIDの変数名
        private const string VALIABLE_CONTENTS_ARRAY = "_contentsArray";

        private const string SKILL_EVENT_NAME = "TriggeredSkill"; // スキルイベントの名前

        // 解析結果を保持する辞書
        private Dictionary<int, AnimationClip> _skillAnimationClips = new();
        private KeyValuePair<int, AnimationClip>[] _skillAnimationClipsArray;

        private string[] _animationClipOptions;
        private SerializedProperty _animationClipProperty;
        private SerializedProperty _contentsArrayProperty;

        private void OnEnable()
        {
            LoadData();

            // プロパティ取得
            _animationClipProperty = serializedObject.FindProperty(VALIABLE_ANIMATION_CLIP_ID);
            _contentsArrayProperty = serializedObject.FindProperty(VALIABLE_CONTENTS_ARRAY);
        }

        public override void OnInspectorGUI()
        {
            // 対象のデータ
            serializedObject.Update();

            // デフォルトのインスペクタを描画（_animationClipを除く）
            DrawPropertiesExcluding(serializedObject, VALIABLE_ANIMATION_CLIP_ID);

            DrawAnimationClipID();
            // 変更を適用
            serializedObject.ApplyModifiedProperties();

            DrawAnalyzer();
        }

        private void DrawAnimationClipID()
        {
            if (_animationClipProperty == null) return;

            // _animationClip用のカスタム描画（プルダウン）
            EditorGUILayout.LabelField("演出設定", EditorStyles.boldLabel);

            #region クリップインデックスのパラメータ
            if (_skillAnimationClipsArray == null || _skillAnimationClipsArray.Length == 0)
            {
                EditorGUILayout.HelpBox("No animation clips found. Please analyze the animator first.", MessageType.Warning);
                return;
            }

            int currentValue = _animationClipProperty.intValue;
            int selectedIndex = Array.FindIndex(_skillAnimationClipsArray, x => x.Key == currentValue);

            selectedIndex = EditorGUILayout.Popup("アニメーションID", selectedIndex, _animationClipOptions);

            if (selectedIndex < 1 || _skillAnimationClipsArray.Length <= selectedIndex)
            {
                // 現在の値が見つからない場合、最初の要素を選択
                selectedIndex = 0;
            }

            _animationClipProperty.intValue = _skillAnimationClipsArray[selectedIndex].Key;

            #endregion

            #region アニメーションクリップのイベントプレビュー
            int length = _contentsArrayProperty.arraySize;

            //選択中のアニメーションのイベントを表示
            AnimationClip selectedClip = _skillAnimationClipsArray[selectedIndex].Value;
            AnimationEvent[] events = selectedClip.events;
            Span<int> indexs = stackalloc int[events.Length];

            EditorGUILayout.LabelField($"Event of {selectedClip.name}", EditorStyles.boldLabel);

            for (int i = 0; i < events.Length; i++)
            {
                AnimationEvent evt = events[i];
                if (evt.functionName != SKILL_EVENT_NAME)
                    continue;

                indexs[i] = evt.intParameter;
                EditorGUILayout.LabelField($"index {evt.intParameter} of {evt.functionName} at {evt.time}");
            }

            for (int i = 0; i < indexs.Length; i++)
            {
               int index = indexs[i];
                if (index < 0 || index >= length)
                {
                    EditorGUILayout.HelpBox($"Invalid index {index} for contents array.", MessageType.Error);
                    continue;
                }
            }
#endregion
        }

        private void DrawAnalyzer()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Animation Skill Viewer", EditorStyles.boldLabel);

            // アニメーション解析ウィンドウを開くボタン
            if (GUILayout.Button("Open Animator Analyzer"))
            {
                CardDataWindow window = CardDataWindow.ShowWindow();
                window.OnDataAnalyzed += LoadData;
            }
        }

        /// <summary>
        ///     EditorUserSettingsからアニメーションデータを読み込み、アニメーションの辞書を初期化します。
        /// </summary>
        private void LoadData()
        {
            // EditorUserSettingsからGUIDのマップを読み込み、AnimationClipを復元する
            var json = EditorUserSettings.GetConfigValue(SKILL_ANIMATION_GUID_MAP_KEY) ?? string.Empty;
            if (string.IsNullOrEmpty(json)) return;

            var data = JsonConvert.DeserializeObject<CardDataDrawerData>(json);

            if (data.Data == null || data.Data.Count == 0)
            {
                Debug.LogWarning("No animation data found in EditorUserSettings.");
                return;
            }

            _skillAnimationClips.Clear();
            foreach (var pair in data.Data)
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
            _skillAnimationClipsArray = _skillAnimationClips.ToArray();

            // プルダウンに表示する選択肢を生成
            _animationClipOptions = new string[_skillAnimationClips.Count];
            for (int i = 0; i < _skillAnimationClips.Count; i++)
            {
                _animationClipOptions[i] = _skillAnimationClipsArray[i].Value.ToString();
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
        private string _intParameterName = "SkillIndex";

        private IEnumerable<KeyValuePair<int, AnimationClip>> _skillAnimationClips;
        private void OnEnable()
        {
            var json = EditorUserSettings.GetConfigValue(CardDataDrawer.SKILL_ANIMATION_GUID_MAP_KEY) ?? string.Empty;
            if (string.IsNullOrEmpty(json)) return;

            var data = JsonConvert.DeserializeObject<CardDataDrawerData>(json);

            string path = AssetDatabase.GUIDToAssetPath(data.AnimationControllerGUID);
            _animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

            _skillAnimationClips = data.Data
                .Select(kvp =>
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(kvp.Value);
                    AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                    return new KeyValuePair<int, AnimationClip>(kvp.Key, clip);
                });
        }

        private void OnGUI()
        {
            _animatorController = (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", _animatorController, typeof(AnimatorController), false);
            _targetStateName = EditorGUILayout.TextField("Target State Name", _targetStateName);
            _intParameterName = EditorGUILayout.TextField("Int Parameter Name", _intParameterName);

            if (GUILayout.Button("Analyze and Save to EditorUserSettings"))
            {
                Dictionary<int, AnimationClip> animationDict = AnalyzeAnimator();
                if (animationDict == null) return;

                SaveAnalyzeData(animationDict);

                // ウィンドウを閉じて、インスペクターの更新を促す

                OnDataAnalyzed?.Invoke();
                this.Close();
            }

            // 解析データが存在する場合、読み取り専用で表示
            if (_skillAnimationClips == null || _skillAnimationClips.Count() > 0)
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

            // JSONにシリアライズしてEditorUserSettingsに保存
            string controllerGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_animatorController));
            CardDataDrawerData data = new(guidMap, controllerGUID);
            var json = JsonConvert.SerializeObject(data);
            EditorUserSettings.SetConfigValue(CardDataDrawer.SKILL_ANIMATION_GUID_MAP_KEY, json);
        }
    }

    [Serializable]
    public struct CardDataDrawerData
    {
        public CardDataDrawerData(Dictionary<int, string> data, string animationControllerGUID)
        {
            _data = data;
            _animationControllerGUID = animationControllerGUID;
        }
        public Dictionary<int, string> Data => _data;
        public string AnimationControllerGUID => _animationControllerGUID;

        private Dictionary<int, string> _data;
        private string _animationControllerGUID;
    }
}