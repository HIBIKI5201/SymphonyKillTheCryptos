namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    /// 攻撃と被弾の両方が可能な、ゲーム内のキャラクターを表すインターフェースです。
    /// </summary>
    public interface ICharacter : IAttackable, IHittable
    {
        public string Name { get; }
    }
}
