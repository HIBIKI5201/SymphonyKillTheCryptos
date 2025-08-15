namespace Cryptos.Runtime.Entity.Ingame.Character
{
    public interface IAttackableData
    {
        public float AttackPower { get; }

        /// <summary> クリティカル確率（%）</summary>
        public float CriticalChance { get; }

        /// <summary> クリティカル倍率 </summary>
        public float CriticalDamage { get; }
    }
}
