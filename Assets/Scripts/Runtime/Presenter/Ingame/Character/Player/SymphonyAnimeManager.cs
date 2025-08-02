using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Player
{
    [RequireComponent(typeof(Animator))]
    public class SymphonyAnimeManager : MonoBehaviour
    {
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
        private AnimatorOverrideController _overrideController;

        private void Awake()
        {
            if (!TryGetComponent(out _animator))
            {
                Debug.LogError("Animator component is not found on the GameObject.");
                return;
            }
        }

        private void Skill(int number)
        {
            Debug.Log($"Skill animation event{number} triggered");
        }
    }
}
