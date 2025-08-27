namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    /// 攻撃可能なオブジェクトの静的なステータスデータを定義するインターフェースです。
    /// </summary>
    public interface IAttackableData : IEntityData
    {
        /// <summary>
        /// 基本攻撃力を取得します。
        /// </summary>
        public float AttackPower { get; }

        /// <summary>
        /// クリティカルヒットの確率を取得します（%）。
        /// </summary>
        public float CriticalChance { get; }

        /// <summary>
        /// クリティカルヒット時のダメージ倍率を取得します。
        /// </summary>
        public float CriticalDamage { get; }
    }
}
