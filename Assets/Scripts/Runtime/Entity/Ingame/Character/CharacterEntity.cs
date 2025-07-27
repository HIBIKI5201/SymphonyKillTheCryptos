using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    public class CharacterEntity<T> : IAttackable, IHitable
        where T : class, IAttackableData, IHitableData
    {
        public CharacterEntity(T data)
        {
            _entityData = data;
            _healthEntity = new HealthEntity(data);
        }

        [Tooltip("第一引数は現在値、第二引数は最大値")]
        public event Action<float, float> OnHealthChanged
        {
            add => _healthEntity.OnHealthChanged += value;
            remove => _healthEntity.OnHealthChanged -= value;
        }

        [Tooltip("死亡時のイベント")]
        public event Action OnDead
        {
            add => _healthEntity.OnDead += value;
            remove => _healthEntity.OnDead -= value;
        }

        public virtual float GetAttackPower()
        {
            float power = _entityData.AttackPower;

            //クリティカル時に倍率を掛ける
            if (UnityEngine.Random.Range(0, 1) < _entityData.CriticalChance)
            {
                power *= _entityData.CriticalDamage;
            }

            return power;
        }

        public virtual void AddHealthDamage(float damage) => _healthEntity.AddHealthDamage(damage);
        public virtual void AddHealthHeal(float amount) => _healthEntity.AddHealthHeal(amount);
        public virtual void Dead() => _healthEntity.Dead();

        protected readonly T _entityData;
        private readonly HealthEntity _healthEntity;
    }
}
