using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    public abstract class CharacterEntity : IAttackable, IHitable
    {
        public CharacterEntity(IHitableData data)
        {
            _healthEntity = new HealthEntity(data.MaxHealth);
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

        public abstract float GetAttackPower();

        public void AddHealthDamage(float damage) => _healthEntity.AddHealthDamage(damage);
        public void AddHealthHeal(float amount) => _healthEntity.AddHealthHeal(amount);
        public void Dead() => _healthEntity.Dead();

        private readonly HealthEntity _healthEntity;
    }
}
