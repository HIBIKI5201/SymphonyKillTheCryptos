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
        
        void SetStatusText(string text);
        void SetRoleCharacter(string characterName);

        void SetDeckCards(IReadOnlyList<CardViewModel> card);
        void SetOwnedCards(IReadOnlyList<CardViewModel> cards);
    }
}