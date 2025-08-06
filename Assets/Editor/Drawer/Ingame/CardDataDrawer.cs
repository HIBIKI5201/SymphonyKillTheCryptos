using Cryptos.Runtime.Entity.Ingame.Word;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Cryptos.Editor.Ingame
{
    [CustomEditor(typeof(WordDataBase))]
    public class CardDataDrawer : UnityEditor.Editor
    {
        public class CardDataWindow : EditorWindow
        {
            private AnimatorController _animatorController;
            private string _targetStateName = "Skill/Entry"; // 調査対象のステート名
            private string _intParameterName = "Skill"; // 条件に使うintパラメータ名

            [MenuItem("Tools/Analyze Animator Transitions")]
            private static void ShowWindow()
            {
                GetWindow<CardDataWindow>("Animator Analyzer");
            }

            private void OnGUI()
            {
                _animatorController = (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", _animatorController, typeof(AnimatorController), false);
                _targetStateName = EditorGUILayout.TextField("Target State Name", _targetStateName);
                _intParameterName = EditorGUILayout.TextField("Int Parameter Name", _intParameterName);

                if (GUILayout.Button("Analyze Transitions"))
                {
                    if (_animatorController != null)
                    {
                        {
                            Dictionary<int, AnimationClip> resultDict = new Dictionary<int, AnimationClip>();
                            string[] pathParts = _targetStateName.Split('/');

                            foreach (var layer in _animatorController.layers)
                            {
                                AnimatorStateMachine rootSM = layer.stateMachine;

                                TraverseStateMachine(rootSM, pathParts, 0, _intParameterName, resultDict);
                                Debug.Log($"✅ 解析完了 \nstate name : {_targetStateName} 結果数: {resultDict.Count}件");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Animator Controller を設定してください。");
                    }
                }
            }

            private void TraverseStateMachine(
                AnimatorStateMachine currentSM,
                string[] pathParts,
                int index,
                string paramName,
                Dictionary<int, AnimationClip> resultDict)
            {
                if (index >= pathParts.Length)
                    return;

                string currentPart = pathParts[index];

                if (index == pathParts.Length - 1)
                {
                    // 最終パート：Entry または ステート
                    if (currentPart == "Entry")
                    {
                        foreach (var entryTransition in currentSM.entryTransitions)
                        {
                            foreach (var condition in entryTransition.conditions)
                            {
                                if (condition.parameter == paramName && condition.mode == AnimatorConditionMode.Equals)
                                {
                                    int value = (int)condition.threshold;
                                    if (entryTransition.destinationState?.motion is AnimationClip clip && !resultDict.ContainsKey(value))
                                    {
                                        resultDict.Add(value, clip);
                                        Debug.Log($"[Entry] 条件: {paramName} == {value} → {entryTransition.destinationState.name}, Clip: {clip.name}");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var state in currentSM.states)
                        {
                            if (state.state.name != currentPart)
                                continue;

                            foreach (var transition in state.state.transitions)
                            {
                                foreach (var condition in transition.conditions)
                                {
                                    if (condition.parameter == paramName && condition.mode == AnimatorConditionMode.Equals)
                                    {
                                        int value = (int)condition.threshold;
                                        if (transition.destinationState?.motion is AnimationClip clip && !resultDict.ContainsKey(value))
                                        {
                                            resultDict.Add(value, clip);
                                            Debug.Log($"[State] 条件: {paramName} == {value} → {transition.destinationState.name}, Clip: {clip.name}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // サブステートマシンを探して再帰
                    foreach (var child in currentSM.stateMachines)
                    {
                        if (child.stateMachine.name == currentPart)
                        {
                            TraverseStateMachine(child.stateMachine, pathParts, index + 1, paramName, resultDict);
                            return;
                        }
                    }

                    Debug.LogWarning($"❌ サブステートマシン '{currentPart}' が見つかりません。");
                }
            }

        }
    }
}