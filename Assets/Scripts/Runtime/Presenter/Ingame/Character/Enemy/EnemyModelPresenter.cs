using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Enemy
{
    public class EnemyModelPresenter : MonoBehaviour
    {
        public void Init(Transform target)
        {
            if (target == null)
            {
                Debug.LogError("Target transform is not assigned.");
                return;
            }

            _target = target;
        }

        public void Dead()
        {
            Destroy(gameObject);
        }

        private Transform _target;

        private void Update()
        {
            if (_target == null) return;

            transform.rotation = Quaternion.LookRotation(_target.position - transform.position);
        }
    }
}
