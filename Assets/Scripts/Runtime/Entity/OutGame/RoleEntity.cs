using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Outgame.Card;

namespace Cryptos.Runtime.Entity
{
    public class RoleEntity
    {
        public RoleEntity(string name,
            DeckNameValueObject deck,
            CharacterData character)
        {
            _name = name;
            _deck = deck;
            _character = character;
        }

        public string Name => _name;
        public DeckNameValueObject Deck => _deck;
        public CharacterData Character => _character;

        private readonly string _name;
        private readonly DeckNameValueObject _deck;
        private readonly CharacterData _character;
    }
}
