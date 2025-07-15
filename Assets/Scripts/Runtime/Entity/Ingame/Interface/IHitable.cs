using SymphonyFrameWork;

namespace Cryptos.Runtime.Entity
{
    /// <summary>
    ///     攻撃を受けられるオブジェクトのインターフェース
    /// </summary>
    public interface IHitable : IGameObject
    {
        public IHitableData HitableData { get; }
    }
}
