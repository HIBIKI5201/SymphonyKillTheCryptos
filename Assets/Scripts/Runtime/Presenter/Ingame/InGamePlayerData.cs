using Cryptos.Runtime.Entity.Ingame.Card;

namespace Cryptos.Runtime.Presenter
{
    public class InGamePlayerData
    {
        public InGamePlayerData(CardDeckEntity deck)
        {
            _deck = deck;
        }

        public CardDeckEntity Deck => _deck;

        private readonly CardDeckEntity _deck;
    }
}
