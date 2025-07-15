using Cryptos.Runtime.Framework;
using UnityEngine;

namespace Cryptos.Runtime.Entity
{
    /// <summary>
    ///     Symphonyのデータを保持するクラス。
    /// </summary>
    [CreateAssetMenu(fileName = nameof(SymphonyData), menuName = CryptosPathConstant.ASSET_PATH + nameof(SymphonyData), order = 1)]
    public class SymphonyData : ScriptableObject, IAttackableData, IHitableData
    {
        public float AttackPower => _attackPower;

        public float CriticalChance => _criticalChance;
        public float CriticalDamage => _criticalDamage;

        public float Health => _health;
        public float MaxHealth => _maxHealth;

        public float Armor => _armor;

        [SerializeField]
        private float _attackPower;
        [SerializeField]
        private float _criticalChance;
        [SerializeField]
        private float _criticalDamage;
        [SerializeField]
        private float _health;
        [SerializeField]
        private float _maxHealth;
        [SerializeField]
        private float _armor;
    }
}
