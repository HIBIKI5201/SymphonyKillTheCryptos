using System;
using System.Collections.Generic;

namespace Cryptos.Runtime.Presenter.OutGame
{
    /// <summary>
    ///     デッキエディタUIが実装すべきインターフェースである。
    /// </summary>
    public interface IDeckEditorUI
    {
        event Action OnEditButtonClicked;
        event Action OnSaveButtonClicked;
        event Action OnRoleSelected;
        event Action OnCancelButtonClicked;
        event Action<CardViewModel> OnOwnedCardSelected;
        event Action OnCardSwapRequested;
        event Action<UnityEngine.UIElements.NavigationMoveEvent.Direction> OnNavigateDeckCard;

        void SetStatusText(string text);
        void SetRoleCharacter(string characterName);

        void SetCurrentCard(CardViewModel card);
        void SetAdjacentCards(CardViewModel leftCard, CardViewModel rightCard);
        void SetOwnedCards(IReadOnlyList<CardViewModel> cards);
        void SetSelectedOwnedCard(CardViewModel card);
        void ClearSelectedOwnedCard();
    }
}