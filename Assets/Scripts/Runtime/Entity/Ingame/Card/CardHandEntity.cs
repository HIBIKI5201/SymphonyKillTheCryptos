using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    /// <summary>
    ///     カードデッキのエンティティ。
    /// </summary>
    public class CardHandEntity
    {
        /// <summary>
        ///     カードがデッキに追加されたときに発行されます。
        /// </summary>
        public event Action<CardEntity> OnAddCardInstance;
        /// <summary>
        ///     カードがデッキから削除されたときに発行されます。
        /// </summary>
        public event Action<CardEntity> OnRemoveCardInstance;

        /// <summary>
        ///     デッキ内のカードの読み取り専用リスト。
        /// </summary>
        public IReadOnlyList<CardEntity> CardList => _cardList;

        /// <summary>
        ///     カードをデッキに追加します。
        /// </summary>
        /// <param name="cardEntity">追加するカード。</param>
        public void AddCardToDeck(CardEntity cardEntity)
        {
            if (cardEntity == null)
            {
                Debug.LogWarning("カードエンティティがnullです");
                return;
            }
            _cardList.Add(cardEntity);
            OnAddCardInstance?.Invoke(cardEntity);
        }

        /// <summary>
        /// カードをデッキから削除します。
        /// </summary>
        /// <param name="cardEntity">削除するカード。</param>
        public void RemoveCardFromDeck(CardEntity cardEntity)
        {
            if (cardEntity == null)
            {
                Debug.LogWarning("カードエンティティがnullです");
                return;
            }
            if (_cardList.Remove(cardEntity))
            {
                OnRemoveCardInstance?.Invoke(cardEntity);
            }
            else
            {
                Debug.LogWarning("カードがデッキに存在しません");
            }
        }

        private readonly List<CardEntity> _cardList = new();
    }
}
