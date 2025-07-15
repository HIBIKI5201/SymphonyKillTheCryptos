using SymphonyFrameWork;

namespace Cryptos.Runtime.Entity
{
    /// <summary>
    ///     攻撃を受けられるオブジェクトのインターフェース
    /// </summary>
    public interface IHitable : IGameObject
    {
        public IHitableData HitableData { get; }

        /// <summary>
        ///     ダメージを受ける
        /// </summary>
        /// <param name="damage"></param>
        public void AddHealthDamage(float damage);

        /// <summary>
        ///     ヒールする
        /// </summary>
        /// <param name="amount"></param>
        public void AddHealthHeal(float amount);

        /// <summary>
        ///     死亡する
        /// </summary>
        public void Dead();
    }
}
