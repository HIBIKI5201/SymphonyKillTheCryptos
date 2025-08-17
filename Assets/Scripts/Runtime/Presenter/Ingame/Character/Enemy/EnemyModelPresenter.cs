using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Enemy
{
    /// <summary>
    ///   敵キャラクターのモデルを操作するクラスです。
    /// </summary>
    public class EnemyModelPresenter : MonoBehaviour
    {
        public void Init(CharacterEntity<EnemyData> self, Transform target)
        {
            if (self == null)
            {
                Debug.LogError("self entity is null");
                return;
            }

            if (target == null)
            {
                Debug.LogError("Target transform is not assigned.");
                return;
            }

            _self = self;
            _target = target;

            self.OnDead += Dead;
            self.OnTakeDamage += HandleTakeDamage;

            destroyCancellationToken.Register(() =>
            {
                if (_self == null) return;
                _self.OnDead -= Dead;
                _self.OnTakeDamage -= HandleTakeDamage;
            });
        }

        public void Dead()
        {
            Destroy(gameObject);
        }

        private CharacterEntity<EnemyData> _self;
        private Transform _target;

        private void Update()
        {
            if (_target == null) return;

            transform.rotation = Quaternion.LookRotation(_target.position - transform.position);
        }

        private void HandleTakeDamage()
        {

        }
    }
}
