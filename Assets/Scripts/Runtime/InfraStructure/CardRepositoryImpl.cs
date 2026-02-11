using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Outgame.Card;
using Cryptos.Runtime.InfraStructure.Ingame.Card;
using Cryptos.Runtime.UseCase.OutGame;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure
{
    public class CardRepositoryImpl : ICardRepository
    {
        public async Task<DeckCardEntity> GetDeckCardEntityAsync(CardAddressValueObject address) // メソッド名と戻り値を変更
        {
            var cardDataAsset = await CardDeckLoader.LoadCardDataAsset(address);

            if (cardDataAsset == null)
            {
                Debug.LogError($"CardDataAsset not found for address: {address.Value}");
                return null; // DeckCardEntity は class なので null を返す
            }

            return new DeckCardEntity(
                address,
                cardDataAsset.CardName,
                cardDataAsset.CardExplanation,
                cardDataAsset.CardIcon,
                cardDataAsset.CardDifficulty
            );
        }
    }
}
