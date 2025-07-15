using Cryptos.Runtime.Entity;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Player
{
    /// <summary>
    ///     プレイヤーのマネージャークラス
    /// </summary>
    public class SymphonyManager : MonoBehaviour, IAttackable, IHitable
    {
        public IHitableData HitableData => _symphonyData;
        public IAttackableData AttackableData => _symphonyData;

        /// <summary>
        ///     ダメージを計算する
        /// </summary>
        /// <returns>ダメージ量</returns>
        public float GetAttackPower()
        {
            float power = _symphonyData.AttackPower;

            //クリティカル時に倍率を掛ける
            if (Random.Range(0, 1) < _symphonyData.CriticalChance)
            {
                power *= _symphonyData.CriticalDamage;
            }

            return power;
        }

        /// <summary>
        ///     ヒットした際にダメージを受ける
        /// </summary>
        /// <param name="damage"></param>
        public void AddHealthDamage(float damage)
        {
            _health -= damage;

            if (_health <= 0) //体力が無くなったら死亡
            {
                Dead();
            }
        }

        /// <summary>
        ///     ヒールする
        /// </summary>
        /// <param name="amount"></param>
        public void AddHealthHeal(float amount)
        {
            _health += amount;

            if (_health > _symphonyData.MaxHealth) //体力が最大値を超えないようにする
            {
                _health = _symphonyData.MaxHealth;
            }
        }

        /// <summary>
        ///     死亡する
        /// </summary>
        public void Dead()
        {
            Debug.Log("Player is dead.");
        }

        [SerializeField]
        private SymphonyData _symphonyData;

        private SymphonyAnimeManager _animeManager;

        private float _health;

        private void Awake()
        {
            _health = _symphonyData.MaxHealth;

            _animeManager = GetComponentInChildren<SymphonyAnimeManager>();
        }
    }
}
