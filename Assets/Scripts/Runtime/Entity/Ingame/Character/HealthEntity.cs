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
        public HealthEntity(IHitableData data)
        {
            _data = data;
            _health = data.MaxHealth;
        }

        public event Action<float, float> OnHealthChanged; //第一引数は現在値、第二引数は最大値
        public event Action OnDead;

        /// <summary>
        ///     ヒットした際にダメージを受ける
        /// </summary>
        /// <param name="damage"></param>
        public void AddHealthDamage(float damage)
        {
            _health = Mathf.Max(_health - damage, 0); //体力は0未満にならないようにする
            OnHealthChanged?.Invoke(_health, _data.MaxHealth);

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
            _health = Mathf.Min(_health + amount, _data.MaxHealth); //体力は最大値を超えないようにする
            OnHealthChanged?.Invoke(_health, _data.MaxHealth);
        }

        /// <summary>
        ///     体力が0になった時の処理
        /// </summary>
        public void Dead()
        {
            _health = 0;
            OnHealthChanged?.Invoke(0, _data.MaxHealth);
            OnDead?.Invoke();
        }

        private readonly IHitableData _data;
        [SerializeField, ReadOnly] private float _health;
    }
}
