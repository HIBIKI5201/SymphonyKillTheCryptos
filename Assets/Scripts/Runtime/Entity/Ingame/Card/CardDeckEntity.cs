namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardDeckEntity
    {
        public CardDeckEntity(CardData[] cards)
        {
            _cards = cards;
        }

        public CardData this[int index] => _cards[index];
        public int Length => _cards.Length;
        public CardData[] CardData => _cards;

        private readonly CardData[] _cards;
    }
}
