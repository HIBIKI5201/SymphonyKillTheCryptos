namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    /// 攻撃が可能なオブジェクトのインターフェースです。
    /// </summary>
    public interface IAttackable
    {
        /// <summary>
        /// 攻撃可能なオブジェクトの静的データを取得します。
        /// </summary>
        public IAttackableData AttackableData { get; }

        /// <summary>
        /// 基本的な攻撃力を計算して取得します。
        /// </summary>
        /// <returns>計算後のダメージ量。</returns>
        public float GetAttackPower();
    }
}
