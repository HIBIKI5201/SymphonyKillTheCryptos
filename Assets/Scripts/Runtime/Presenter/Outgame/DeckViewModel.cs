using Cryptos.Runtime.Entity.Outgame.Card;

namespace Cryptos.Runtime.Presenter
{
    public readonly struct DeckViewModel
    {
        public DeckViewModel(DeckNameValueObject deckName, DeckCardEntity[] cards)
        {
            _deckName = deckName.Value;
            _cards = cards;
        }

        public string DeckName => _deckName;

        private readonly string _deckName;
        private readonly DeckCardEntity[] _cards;
    }
}
