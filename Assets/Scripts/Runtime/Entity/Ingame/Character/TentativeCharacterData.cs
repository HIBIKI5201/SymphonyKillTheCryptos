namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    ///     バフなどで一時的に変更されるプレイヤーのステータスを表すクラスです。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TentativeCharacterData<T> : IAttackableData, IHittableData
        where T : IAttackableData, IHittableData
    {
        public TentativeCharacterData(T data)
        {
            _attackPower = data.AttackPower;
            _criticalChance = data.CriticalChance;
            _criticalDamage = data.CriticalDamage;
            _maxHealth = data.MaxHealth;
            _armor = data.Armor;

            _data = data;
        }

        /// <summary>
        /// プレイヤー名を取得します。
        /// </summary>
        public string Name => _data.Name;
        /// <summary>
        /// 攻撃力を取得します。
        /// </summary>
        public float AttackPower => _attackPower;

        /// <summary>
        /// クリティカルヒットの確率を取得します（%）。
        /// </summary>
        public float CriticalChance => _criticalChance;

        /// <summary>
        /// クリティカルヒット時のダメージ倍率を取得します。
        /// </summary>
        public float CriticalDamage => _criticalDamage;

        /// <summary>
        /// 最大体力を取得します。
        /// </summary>
        public float MaxHealth => _maxHealth;

        /// <summary>
        /// 防御力を取得します（割合軽減）。
        /// </summary>
        public float Armor => _armor;

        private readonly T _data;
        private float _attackPower = 10;
        private float _criticalChance = 3;
        private float _criticalDamage = 2;
        private float _maxHealth = 100;
        private float _armor = 25;
    }
}
