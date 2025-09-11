using SymphonyFrameWork.Attribute;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    /// キャラクターの体力に関する処理を管理するクラスです。
    /// </summary>
    [Serializable]
    public class HealthEntity
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="data">体力データ。</param>
        public HealthEntity(IHittableData data)
        {
            _data = data;
            _health = data.MaxHealth;
        }

        /// <summary>
        /// 体力が変化した際に発火します。
        /// </summary>
        [Tooltip("体力が変化した際に発火します。第一引数は現在値、第二引数は最大値。")]
        public event Action<float, float> OnHealthChanged;

        /// <summary>
        /// 体力が0になり、死亡した際に発火します。
        /// </summary>
        [Tooltip("死亡時に発火します。")]
        public event Action OnDead;

        /// <summary>
        /// 現在の体力を取得します。
        /// </summary>
        public float CurrentHealth => _health;

        /// <summary>
        /// ヒットした際にダメージを受けます。
        /// 体力は0未満にはなりません。
        /// </summary>
        /// <param name="damage">受けるダメージ量。</param>
        public void AddHealthDamage(float damage)
        {
            _health = Mathf.Max(_health - damage, 0); //体力は0未満にならないようにする。
            OnHealthChanged?.Invoke(_health, _data.MaxHealth);

            if (_health <= 0) //体力が無くなったら死亡。
            {
                Dead();
            }
        }

        /// <summary>
        /// 体力を回復します。
        /// 体力は最大値を超えません。
        /// </summary>
        /// <param name="amount">回復量。</param>
        public void AddHealthHeal(float amount)
        {
            _health = Mathf.Min(_health + amount, _data.MaxHealth); //体力は最大値を超えないようにする。
            OnHealthChanged?.Invoke(_health, _data.MaxHealth);
        }

        /// <summary>
        /// 体力が0になった時の処理を実行します。
        /// </summary>
        public void Dead()
        {
            _health = 0;
            OnHealthChanged?.Invoke(0, _data.MaxHealth);
            OnDead?.Invoke();
        }

        private readonly IHittableData _data;

        [SerializeField, ReadOnly, Tooltip("現在の体力。")]
        private float _health;
    }
}
