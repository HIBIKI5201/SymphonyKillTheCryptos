using System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Player
{
    [RequireComponent(typeof(Animator), typeof(SkillEndReceiver))]
    public class SymphonyAnimeManager : MonoBehaviour
    {
        public event Action<int> OnSkillTriggered;
        public event Action OnSkillEnded;

        public void SetDirX(float value) => _animator.SetFloat(_dirXHash, value);
        public void SetDirY(float value) => _animator.SetFloat(_dirYHash, value);
        public void SetVelocity(float value) => _animator.SetFloat(_velocityHash, value);
        public void SetSprint(bool value) => _animator.SetBool(_sprintHash, value);

        public void ActiveSkill(int index)
        {
            _animator.SetInteger(_SkillIndexHash, index);
            _animator.SetTrigger(_skillTriggerHash);
        }

        private readonly int _dirXHash = Animator.StringToHash("DirX");
        private readonly int _dirYHash = Animator.StringToHash("DirY");
        private readonly int _velocityHash = Animator.StringToHash("Velocity");
        private readonly int _sprintHash = Animator.StringToHash("IsSprint");
        private readonly int _SkillIndexHash = Animator.StringToHash("SkillIndex");
        private readonly int _skillTriggerHash = Animator.StringToHash("SkillTrigger");

        private Animator _animator;

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
    }
}
