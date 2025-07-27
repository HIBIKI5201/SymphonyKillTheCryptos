using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardDeckEntity
    {
        public event Action<CardEntity> OnAddCardInstance;
        public event Action<CardEntity> OnRemoveCardInstance;

        public IReadOnlyList<CardEntity> DeckCardList => _deckCardList;

        public void AddCardToDeck(CardEntity cardEntity)
        {
            if (cardEntity == null)
            {
                Debug.LogWarning("カードエンティティがnullです");
                return;
            }
            _deckCardList.Add(cardEntity);
            OnAddCardInstance?.Invoke(cardEntity);
        }

        public void RemoveCardFromDeck(CardEntity cardEntity)
        {
            if (cardEntity == null)
            {
                Debug.LogWarning("カードエンティティがnullです");
                return;
            }
            if (_deckCardList.Remove(cardEntity))
            {
                OnRemoveCardInstance?.Invoke(cardEntity);
            }
            else
            {
                Debug.LogWarning("カードがデッキに存在しません");
            }
        }

        private readonly List<CardEntity> _deckCardList = new();
    }
}
