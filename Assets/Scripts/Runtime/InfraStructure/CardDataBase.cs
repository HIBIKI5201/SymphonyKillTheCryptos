using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Outgame.Card;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.InfraStructure.Ingame.Card;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure
{
    [CreateAssetMenu(fileName = nameof(CardDataBase),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(CardDataBase))]
    public class CardDataBase : ScriptableObject
    {
        public async Task<DeckCardEntity[]> GetDeckCards()
        {
            CardAddressValueObject[] addresses = CardDeckLoader.GetAddress(_cardDataAddresses);
            DeckCardEntity[] cards = await CardDeckLoader.LoadDataBase(addresses);
            return cards;
        }

        [SerializeField]
        private string[] _cardDataAddresses;
    }
}
