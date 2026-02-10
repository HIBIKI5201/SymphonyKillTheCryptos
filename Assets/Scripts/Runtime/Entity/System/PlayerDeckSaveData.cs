using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Entity.System.SaveData
{
    [Serializable]
    public class PlayerDeckSaveData
    {
        public PlayerDeckSaveData()
        {
            _attackDeck = new Dictionary<string, CardAddressValueObject[]>();

        }



        /// <summary>
        ///     デッキを登録または更新する。
        /// </summary>
        /// <param name="deckName">登録するデッキの名前。</param>
        /// <param name="deck">登録するカードアドレスの配列。</param>
        public void RegisterDeck(string deckName, CardAddressValueObject[] deck)
        {
            _attackDeck[deckName] = deck;
        }

        /// <summary>
        ///     指定された名前のデッキを取得する。
        /// </summary>
        /// <param name="deckName">取得するデッキの名前。</param>
        /// <returns>指定された名前のカードアドレスの配列。存在しない場合はnullを返す。</returns>
        public CardAddressValueObject[] GetDeck(string deckName)
        {
            _attackDeck.TryGetValue(deckName, out CardAddressValueObject[] deck);
            return deck;
        }

        /// <summary>
        ///     保存されているすべてのデッキの名前を取得する。
        /// </summary>
        /// <returns>保存されているデッキの名前のコレクション。</returns>
        public IReadOnlyCollection<string> GetAllDeckNames()
        {
            return _attackDeck.Keys;
        }





        [SerializeField]
        private Dictionary<string, CardAddressValueObject[]> _attackDeck;
    }
}
