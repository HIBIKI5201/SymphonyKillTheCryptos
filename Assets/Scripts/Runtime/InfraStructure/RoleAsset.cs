using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.InfraStructure.Ingame.DataAsset;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.OutGame.Card
{
    [CreateAssetMenu(fileName = nameof(RoleAsset),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(RoleAsset))]
    public class RoleAsset : ScriptableObject
    {
        public string Name => _name;
        public CardDeckAsset Deck => _deck;
        public CharacterData Status => _status;

        [SerializeField]
        private string _name;
        [SerializeField]
        private CardDeckAsset _deck;
        [SerializeField]
        private CharacterData _status;
    }
}
