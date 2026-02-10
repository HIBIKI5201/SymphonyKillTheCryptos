using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Outgame.Card;
using Cryptos.Runtime.InfraStructure.Ingame.DataAsset;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cryptos.Runtime.InfraStructure.Ingame.Card
{
    public static class CardDeckLoader
    {
        public static CardAddressValueObject[] GetAddress(string[] cardDataAddresses)
        {
            CardAddressValueObject[] cardAddressValueObject =
                new CardAddressValueObject[cardDataAddresses.Length];

            for (int i = 0; i < cardAddressValueObject.Length; i++)
            {
                cardAddressValueObject[i] = new CardAddressValueObject(cardDataAddresses[i]);
            }

            return cardAddressValueObject;
        }

        public static async Task<CardData[]> LoadDeck(CardAddressValueObject[] addresses, CombatPipelineAsset combatPipeline)
        {
            Dictionary<CardAddressValueObject, CardDataAsset> assets = new();
            foreach (CardAddressValueObject address in addresses)
            {
                if (assets.ContainsKey(address)) { continue; }

                AsyncOperationHandle<CardDataAsset> handle = Addressables.LoadAssetAsync<CardDataAsset>(address.Value);
                await handle.Task;
                assets.TryAdd(address, handle.Result);
            }

            CardData[] cards = new CardData[addresses.Length];
            for (int i = 0; i < addresses.Length; i++)
            {
                if (assets.TryGetValue(addresses[i], out CardDataAsset asset))
                {
                    cards[i] = asset.CreateCardData(combatPipeline);
                }
            }

            return cards;
        }

        public static async Task<DeckCardEntity[]> LoadDataBase(CardAddressValueObject[] addresses)
        {
            Dictionary<CardAddressValueObject, CardDataAsset> assets = new();
            foreach (CardAddressValueObject address in addresses)
            {
                if (assets.ContainsKey(address)) { continue; }

                AsyncOperationHandle<CardDataAsset> handle = Addressables.LoadAssetAsync<CardDataAsset>(address.Value);
                await handle.Task;
                assets.TryAdd(address, handle.Result);
            }

            DeckCardEntity[] cards = new DeckCardEntity[addresses.Length];
            for (int i = 0; i < cards.Length; i++)
            {
                CardAddressValueObject address = addresses[i];
                if (assets.TryGetValue(address, out CardDataAsset asset))
                {
                    cards[i] = new(address,
                        asset.CardName,
                        asset.CardExplanation,
                        asset.CardIcon,
                        asset.CardDifficulty);
                }
            }

            return cards;
        }
    }
}
