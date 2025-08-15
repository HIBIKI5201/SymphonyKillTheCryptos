using Cryptos.Runtime.Framework;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    [CreateAssetMenu(fileName = nameof(EnemyData), menuName = CryptosPathConstant.ASSET_PATH + nameof(EnemyData), order = 1)]
    public class EnemyData : ScriptableObject, IAttackableData, IHittableData
    {
        public float AttackPower => _attackPower;

        public float CriticalChance => _criticalChance;
        public float CriticalDamage => _criticalDamage;

        public float MaxHealth => _maxHealth;

        public float Armor => _armor;

        [SerializeField]
        private float _attackPower = 10;
        [SerializeField]
        private float _criticalChance = 3;
        [SerializeField]
        private float _criticalDamage = 2;
        [SerializeField]
        private float _maxHealth = 100;
        [SerializeField]
        private float _armor = 25;
    }
}
