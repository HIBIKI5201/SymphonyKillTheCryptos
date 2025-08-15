namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    ///     攻撃が可能なオブジェクトのインターフェース
    /// </summary>
    public interface IAttackable
    {
        public IAttackableData AttackableData { get; }

        /// <summary>
        ///     ダメージを計算する
        /// </summary>
        /// <returns>ダメージ量</returns>
        public float GetAttackPower();
    }
}
