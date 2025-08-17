using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Enemy
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimeManager : MonoBehaviour
    {
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
