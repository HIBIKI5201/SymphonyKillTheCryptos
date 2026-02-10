using Cryptos.Runtime.Entity.Ingame.Card;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Entity
{
    [Serializable]
    public class PlayerDeckSaveData
    {
        public void RegisterDeck(string deckName, CardAddressValueObject[] deck)
        {
            if (_attackDeck.ContainsKey(deckName))
            {
                _attackDeck[deckName] = deck;
            }
            else
            {
                throw new Exception($"{deckName} Deck is null");
            }
        }

        [SerializeField]
        private Dictionary<string, CardAddressValueObject[]> _attackDeck;
    }
}
