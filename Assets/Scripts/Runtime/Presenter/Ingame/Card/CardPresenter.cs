using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.Card;

namespace Cryptos.Runtime.Presenter.Ingame.Card
{
    public class CardPresenter
    {
        public CardPresenter(CardUseCase cardUseCase,
            CardExecutionUseCase executionUseCase,
            ICardUIManager cardUIManager)
        {
            cardUseCase.OnCardAddedToDeck += HandleAddCard;
            cardUseCase.OnCardRemovedFromDeck += HandleMoveCardToStack;
            executionUseCase.OnCardExecuted += HandleRemoveCard;

            _cardUIManager = cardUIManager;
        }

        private ICardUIManager _cardUIManager;

        private void HandleAddCard(CardEntity card)
        {
            _cardUIManager.AddCard(new(card));
        }

        private void HandleRemoveCard(CardEntity card)
        {
            _cardUIManager.RemoveCard(new(card));
        }

        private void HandleMoveCardToStack(CardEntity card)
        {
            _cardUIManager.MoveCardToStack(new(card));
        }
    }
}
