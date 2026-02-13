using UnityEngine;

namespace Cryptos.Runtime.Entity.Outgame.Card
{
    public class DeckCardEntity
    {
        public DeckCardEntity(CardAddressValueObject address,
            string cardName,
            string cardExplanation,
            Texture2D cardIcon,
            int cardDifficulty)
        {
            _address = address;
            _cardName = cardName;
            _cardExplanation = cardExplanation;
            _cardIcon = cardIcon;
            _cardDifficulty = cardDifficulty;
        }

        public CardAddressValueObject Address => _address;
        public string CardName => _cardName;
        public string CardExplanation => _cardExplanation;
        public Texture2D CardIcon => _cardIcon;
        public int CardDifficulty => _cardDifficulty;

        private readonly CardAddressValueObject _address;
        private readonly string _cardName;
        private readonly string _cardExplanation;
        private readonly Texture2D _cardIcon;
        private readonly int _cardDifficulty;
    }
}
