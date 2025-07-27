using Cryptos.Runtime.Framework;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    [CreateAssetMenu(fileName = nameof(EnemyData), menuName = CryptosPathConstant.ASSET_PATH + nameof(EnemyData), order = 1)]
    public class EnemyData : ScriptableObject, IAttackableData, IHitableData
    {
        public float AttackPower => _attackPower;

        public float CriticalChance => _criticalChance;
        public float CriticalDamage => _criticalDamage;

        public float MaxHealth => _maxHealth;

        public float Armor => _armor;

        [SerializeField]
        private float _attackPower;
        [SerializeField]
        private float _criticalChance;
        [SerializeField]
        private float _criticalDamage;
        [SerializeField]
        private float _maxHealth;
        [SerializeField]
        private float _armor;
    }
}
