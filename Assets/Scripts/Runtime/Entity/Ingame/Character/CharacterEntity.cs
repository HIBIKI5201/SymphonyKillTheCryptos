using SymphonyFrameWork.Attribute;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    /// キャラクターのインスタンスデータを表す汎用的なエンティティクラスです。
    /// IAttackableData と IHittableData の両方を実装したデータ型を扱うことができます。
    /// </summary>
    [Serializable]
    public class CharacterEntity : ICharacter
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="data">キャラクターのデータ。</param>
        public CharacterEntity(ICharacterData data)
        {
            _entityData = data;
            _healthEntity = new HealthEntity(data);
        }

        /// <summary> 体力が変化した際に発火します。第一引数は現在値、第二引数は最大値。 </summary>
        public event Action<float, float> OnHealthChanged
        {
            add => _healthEntity.OnHealthChanged += value;
            remove => _healthEntity.OnHealthChanged -= value;
        }

        /// <summary> 死亡時に発火します。 </summary>
        public event Action OnDead
        {
            add => _healthEntity.OnDead += value;
            remove => _healthEntity.OnDead -= value;
        }

        /// <summary> ダメージを受けた時のイベント </summary>
        public event Action<CombatContext> OnTakedDamage;

        /// <summary>
        /// 攻撃可能なデータ。
        /// </summary>
        public IAttackableData AttackableData => _entityData;
        /// <summary>
        /// 被ダメージ可能なデータ。
        /// </summary>
        public IHittableData HittableData => _entityData;
        /// <summary>
        /// キャラクター名。
        /// </summary>
        public string Name => _entityData.Name;
        /// <summary>
        /// 現在の体力。
        /// </summary>
        public float CurrentHealth => _healthEntity.CurrentHealth;

        /// <summary>
        /// キャラクターにダメージを与えます。
        /// </summary>
        /// <param name="damage">与えるダメージ情報を含んだCombatContext。</param>
        public void AddHealthDamage(CombatContext damage)
        {
            _healthEntity.AddHealthDamage(damage.Damage);
            OnTakedDamage?.Invoke(damage);
        }

        /// <summary>
        /// キャラクターの体力を回復します。
        /// </summary>
        /// <param name="amount">回復量。</param>
        public void AddHealthHeal(float amount) => _healthEntity.AddHealthHeal(amount);

        /// <summary>
        /// キャラクターを死亡状態にします。
        /// </summary>
        public void Dead() => _healthEntity.Dead();

        protected readonly ICharacterData _entityData;
        protected readonly HealthEntity _healthEntity;
    }
}
