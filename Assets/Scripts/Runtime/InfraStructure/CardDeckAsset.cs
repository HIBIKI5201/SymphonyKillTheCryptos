using UnityEngine;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.Entity.Ingame.Card;
using System;

namespace Cryptos.Runtime.InfraStructure.Ingame.DataAsset
{
    [CreateAssetMenu(fileName = nameof(CardDeckAsset),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(CardDeckAsset))]
    public class CardDeckAsset : ScriptableObject
    {
        public CardDeckEntity GetCardDeck(CombatPipelineAsset combatPipeline)
        {
            CardData[] cards = new CardData[_cardDataAssets.Length];

            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = _cardDataAssets[i].CreateCardData(combatPipeline);
            }

            CardDeckEntity deck = new(cards);
            return deck;
        }

        [SerializeField]
        private CardDataAsset[] _cardDataAssets;
    }
}
