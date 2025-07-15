using SymphonyFrameWork;

namespace Cryptos.Runtime.Entity
{
    /// <summary>
    ///     攻撃が可能なオブジェクトのインターフェース
    /// </summary>
    public interface IAttackable : IGameObject
    {
        public IAttackableData AttackableData { get; }

        /// <summary>
        ///     ダメージを計算する
        /// </summary>
        /// <returns>ダメージ量</returns>
        public float GetAttackPower();
    }
}
