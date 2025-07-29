using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.Card;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame
{
    public class CardPresenter
    {
        public CardPresenter(CardUseCase cardUseCase, ICardUIManager cardUIManager)
        {
            cardUseCase.OnCardAddedToDeck += HandleAddCard;
            cardUseCase.OnCardRemovedFromDeck += HandleRemoveCard;

            _cardUIManager = cardUIManager;
        }

        private ICardUIManager _cardUIManager;

        private void HandleAddCard(CardEntity card)
        {
            _cardUIManager.HanldeAddCard(card);
        }

        private void HandleRemoveCard(CardEntity card)
        {
            _cardUIManager.HandleRemoveCard(card);
        }
    }
}
