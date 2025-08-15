using SymphonyFrameWork.Attribute;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    [Serializable]
    public sealed class CharacterEntity<T> : ICharacter
        where T : class, IAttackableData, IHittableData
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

        public IAttackableData AttackableData => _entityData;
        public IHittableData HitableData => _entityData;

        public float GetAttackPower()
        {
            float power = _entityData.AttackPower;

            //クリティカル時に倍率を掛ける
            if (UnityEngine.Random.Range(0, 1) < _entityData.CriticalChance)
            {
                power *= _entityData.CriticalDamage;
            }

            return power;
        }

        public void AddHealthDamage(CombatContext damage) => _healthEntity.AddHealthDamage(damage.Damage);
        public void AddHealthHeal(float amount) => _healthEntity.AddHealthHeal(amount);
        public void Dead() => _healthEntity.Dead();

        private readonly T _entityData;
        
        [SerializeField, ReadOnly]
        private readonly HealthEntity _healthEntity;
    }
}
