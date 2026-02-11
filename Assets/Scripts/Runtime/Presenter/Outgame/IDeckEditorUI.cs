using Cryptos.Runtime.Entity.Ingame.Card; // For DeckCardEntity
using Cryptos.Runtime.Presenter.OutGame; // For DeckViewModel
using System.Collections.Generic;
using UnityEngine.UIElements; // Add this for SetSelectedDeckInDropdown's internal workings if needed by UI implementation

namespace Cryptos.Runtime.Presenter.OutGame
{
    /// <summary>
    ///     デッキエディタUIが実装すべきインターフェースである。
    /// </summary>
    public interface IDeckEditorUI
    {
        /// <summary>
        ///     プレゼンターをバインドする。
        /// </summary>
        /// <param name="presenter"></param>
        public void SetPresenter(DeckEditorPresenter presenter);

        /// <summary>
        ///     デッキ名のリストを表示する。
        /// </summary>
        /// <param name="deckNames">表示するデッキ名のコレクション。</param>
        void ShowDeckNames(IReadOnlyCollection<string> deckNames);

        /// <summary>
        ///     ドロップダウンで選択されているデッキを設定する。
        /// </summary>
        /// <param name="deckName">選択するデッキの名前。</param>
        void SetSelectedDeckInDropdown(string deckName);

        /// <summary>
        ///     指定されたデッキを表示する。
        /// </summary>
        /// <param name="deckViewModel">表示するデッキのViewModel。</param>
        void ShowDeck(DeckViewModel deckViewModel);

        /// <summary>
        ///     エラーメッセージを表示する。
        /// </summary>
        /// <param name="message">表示するエラーメッセージ。</param>
        void ShowErrorMessage(string message);

        /// <summary>
        ///     成功メッセージを表示する。
        /// </summary>
        /// <param name="message">表示する成功メッセージ。</param>
        void ShowSuccessMessage(string message);
    }
}