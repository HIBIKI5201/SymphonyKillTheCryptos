using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public interface ICardContent
    {
        /// <summary>
        ///     カードの効果を発動する
        /// </summary>
        /// <param name="player"></param>
        /// <param name="targets"></param>
        public void Execute(IAttackable player, params IHitable[] targets);
    }
}