using Cryptos.Runtime.UI;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Character.Enemy
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimeManager : MonoBehaviour, IEnemyAnimeManager
    {
        public void Hit()
        {
            _animator?.SetTrigger(_hitHash);
        }

        public async ValueTask Dead()
        {
            _animator.SetTrigger(_deadHash);
            await Awaitable.WaitForSecondsAsync(_deadAnimation.length);
        }

        private readonly int _hitHash = Animator.StringToHash("Hit");
        private readonly int _deadHash = Animator.StringToHash("Dead");

        [SerializeField]
        private AnimationClip _deadAnimation;
        private Animator _animator;

        private void Awake()
        {
            if (!TryGetComponent(out _animator))
            {
                Debug.LogError("Animator component is not found on the GameObject.");
                return;
            }
        }
    }
}
