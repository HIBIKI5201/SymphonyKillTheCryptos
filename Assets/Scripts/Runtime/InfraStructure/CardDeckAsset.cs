using UnityEngine;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.Entity.Ingame.Card;
using System;
using Cryptos.Runtime.InfraStructure.Ingame.Card;
using Cryptos.Runtime.Entity;
using System.Threading.Tasks;

namespace Cryptos.Runtime.InfraStructure.Ingame.DataAsset
{
    [CreateAssetMenu(fileName = nameof(CardDeckAsset),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(CardDeckAsset))]
    public class CardDeckAsset : ScriptableObject
    {
        public async Task<CardDeckEntity> GetCardDeck(CombatPipelineAsset combatPipeline)
        {
            CardAddressValueObject[] addresses = CardDeckLoader.GetAddress(_cardDataAddresses);
            CardData[] cards = await CardDeckLoader.LoadDeck(addresses, combatPipeline);

            CardDeckEntity deck = new(cards);
            return deck;
        }

        [SerializeField]
        private string[] _cardDataAddresses;
    }
}
