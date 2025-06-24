using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    public class CardInstance
    {
        public CardInstance(CardData data)
        {
            _data = data;
        }

        public CardData CardData => _data;

        private CardData _data;
    }
}