using Cryptos.Runtime.Entity.Ingame.Character;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    /// <summary>
    /// 全てのカード効果が実装すべき共通のインターフェースです。
    /// </summary>
    public interface ICardContent
    {
        /// <summary>
        /// カードの効果を発動します。
        /// </summary>
        /// <param name="players">効果の主体となるプレイヤーキャラクターの配列。</param>
        /// <param name="targets">効果の対象となるキャラクターの配列。</param>
        public void Execute(ICharacter[] players, ICharacter[] targets);
    }
}
