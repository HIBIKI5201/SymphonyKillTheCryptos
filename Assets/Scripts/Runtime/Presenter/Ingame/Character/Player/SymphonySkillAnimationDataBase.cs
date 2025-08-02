using System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Player
{
    [CreateAssetMenu(fileName = "SymphonySkillAnimationDataBase", menuName = "Cryptos/SymphonySkillAnimationDataBase", order = 1)]
    public class SymphonySkillAnimationDataBase : ScriptableObject
    {
        public int this[AnimationClip clip]
        {
            get
            {
                for (int i = 0; i < SkillAnimations.Length; i++)
                {
                    if (SkillAnimations[i].AnimationClip == clip)
                    {
                        return SkillAnimations[i].SkillIndex;
                    }
                }
                return -1; // Clip not found
            }
        }

        [SerializeField]
        private SymphonySkillAnimationData[] SkillAnimations = default;

        private void OnValidate()
        {
            if (SkillAnimations == null || SkillAnimations.Length == 0) return;

            //インデックスに重複がないか確認する
            Span<int> indexs = stackalloc int[SkillAnimations.Length];
            for (int i = 0; i < SkillAnimations.Length; i++)
            {
                for(int j = 0; j < i; j++)
                {
                    if (SkillAnimations[i].SkillIndex == SkillAnimations[j].SkillIndex)
                    {
                        Debug.LogError($"Duplicate SkillIndex found: {SkillAnimations[i].SkillIndex} at index {i} and {j}");
                        return;
                    }
                }

                indexs[i] = SkillAnimations[i].SkillIndex;
            }
        }

        [Serializable]
        private struct SymphonySkillAnimationData
        {
            public int SkillIndex;
            public AnimationClip AnimationClip;
        }
    }
}
