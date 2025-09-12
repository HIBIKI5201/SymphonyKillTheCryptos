using System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Character.Player
{
    /// <summary>
    ///     プレイヤーのアニメーションを管理するクラス
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(SkillEndReceiver))]
    public class SymphonyAnimeManager : MonoBehaviour, ISymphonyAnimeManager
    {
        public event Action<int> OnSkillTriggered;
        public event Action OnSkillEnded;

        public void SetDirX(float value) => _animator.SetFloat(_animatorHash.DirXHash, value);
        public void SetDirY(float value) => _animator.SetFloat(_animatorHash.DirYHash, value);
        public void SetVelocity(float value) => _animator.SetFloat(_animatorHash.VelocityHash, value);
        public void SetSprint(bool value) => _animator.SetBool(_animatorHash.SprintHash, value);

        public void ActiveSkill(int index)
        {
            _animator.SetInteger(_animatorHash.SkillIndexHash, index);
            _animator.SetTrigger(_animatorHash.SkillTriggerHash);
        }

        [SerializeField]
        private string _dirXParam = "DirX";
        [SerializeField]
        private string _dirYParam = "DirY";
        [SerializeField]
        private string _velocityParam = "Velocity";
        [SerializeField]
        private string _sprintParam = "Sprint";
        [SerializeField]
        private string _skillIndexParam = "SkillIndex";
        [SerializeField]
        private string _skillTriggerParam = "SkillTrigger";

        private Animator _animator;
        private SymphonyAnimatorHash _animatorHash;

        private void OnValidate()
        {
            _animatorHash = new SymphonyAnimatorHash(
                _dirXParam, _dirYParam,
                _velocityParam, _sprintParam,
                _skillIndexParam, _skillTriggerParam
            );
        }

        private void Awake()
        {
            if (!TryGetComponent(out _animator))
            {
                Debug.LogError("Animator component is not found on the GameObject.");
                return;
            }

            if (TryGetComponent(out SkillEndReceiver skillEndReceiver))
            {
                skillEndReceiver.OnSkillEnd += EndSkill;

                destroyCancellationToken.Register(() =>
                {
                    if (skillEndReceiver != null)
                    {
                        Debug.Log("Unsubscribing from OnSkillEnd event in SymphonyAnimeManager.");
                        skillEndReceiver.OnSkillEnd -= EndSkill;
                    }
                });
            }
            else
            {
                Debug.LogError("SkillEndReceiver component is not found on the GameObject.");
                return;
            }
        }

        private void TriggeredSkill(int number)
        {
            Debug.Log($"Skill animation event{number} triggered");
            OnSkillTriggered?.Invoke(number);
        }

        private void EndSkill()
        {
            Debug.Log("Skill animation ended");
            OnSkillEnded?.Invoke();
        }

        /// <summary>
        ///     ハッシュ値を保存する構造体。
        /// </summary>
        private readonly struct SymphonyAnimatorHash
        {
            public SymphonyAnimatorHash(string dirX, string dirY,
                string velocity, string sprint,
                string skillIndex, string skillTrigger)
            {
                _dirXHash = Animator.StringToHash(dirX);
                _dirYHash = Animator.StringToHash(dirY);
                _velocityHash = Animator.StringToHash(velocity);
                _sprintHash = Animator.StringToHash(sprint);
                _SkillIndexHash = Animator.StringToHash(skillIndex);
                _skillTriggerHash = Animator.StringToHash(skillTrigger);
            }

            public int DirXHash => _dirXHash;
            public int DirYHash => _dirYHash;
            public int VelocityHash => _velocityHash;
            public int SprintHash => _sprintHash;
            public int SkillIndexHash => _SkillIndexHash;
            public int SkillTriggerHash => _skillTriggerHash;

            private readonly int _dirXHash;
            private readonly int _dirYHash;
            private readonly int _velocityHash;
            private readonly int _sprintHash;
            private readonly int _SkillIndexHash;
            private readonly int _skillTriggerHash;
        }
    }
}
