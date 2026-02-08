using Cryptos.Runtime.Framework;
using Cryptos.Runtime.InfraStructure.Ingame.DataAsset;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure
{
    [CreateAssetMenu(fileName = nameof(DefaultInGamePlayerData),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(DefaultInGamePlayerData))]
    public class DefaultInGamePlayerData : ScriptableObject
    {
        public CardDeckAsset CardDeckAsset => _deckAsset;

        [SerializeField]
        private CardDeckAsset _deckAsset;
    }
}
