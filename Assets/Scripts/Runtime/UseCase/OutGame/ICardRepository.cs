using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Outgame.Card;
using System.Threading.Tasks;

namespace Cryptos.Runtime.UseCase.OutGame
{
    public interface ICardRepository
    {
        Task<DeckCardEntity> GetDeckCardEntityAsync(CardAddressValueObject address);
    }
}
