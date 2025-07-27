using SymphonyFrameWork.Attribute;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    ///     体力のクラス
    /// </summary>
    [Serializable]
    public class HealthEntity
    {
        public HealthEntity(float maxHealth)
        {
            _maxHealth = maxHealth;
            _health = maxHealth;
        }

        public event Action<float, float> OnHealthChanged; //第一引数は現在値、第二引数は最大値
        public event Action OnDead;

        /// <summary>
        ///     ヒットした際にダメージを受ける
        /// </summary>
        /// <param name="damage"></param>
        public void AddHealthDamage(float damage)
        {
            _health -= damage;
            OnHealthChanged?.Invoke(_health, _maxHealth);

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
            _health = Mathf.Min(_health + amount, _maxHealth);
            OnHealthChanged?.Invoke(_health, _maxHealth);
        }

        /// <summary>
        ///     体力が0になった時の処理
        /// </summary>
        public void Dead()
        {
            _health = 0;
            OnHealthChanged?.Invoke(_health, _maxHealth);
            OnDead?.Invoke();
        }

        private readonly float _maxHealth;
        [SerializeField, ReadOnly] private float _health;
    }
}
