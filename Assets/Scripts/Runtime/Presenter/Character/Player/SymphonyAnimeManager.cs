using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Player
{
    [RequireComponent(typeof(Animator))]
    public class SymphonyAnimeManager : MonoBehaviour
    {
        private readonly int _dirXHash = Animator.StringToHash("DirX");
        private readonly int _dirYHash = Animator.StringToHash("DirY");
        private readonly int _velocityHash = Animator.StringToHash("Velocity");
        private readonly int _sprintHash = Animator.StringToHash("IsSprint");

        private Animator _animator;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetDirX(float value) => _animator.SetFloat(_dirXHash, value);
        public void SetDirY(float value) => _animator.SetFloat(_dirYHash, value);
        public void SetVelocity(float value) => _animator.SetFloat(_velocityHash, value);
        public void SetSprint(bool value) => _animator.SetBool(_sprintHash, value);
    }
}
