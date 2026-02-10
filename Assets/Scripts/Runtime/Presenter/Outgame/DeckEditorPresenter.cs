using Cryptos.Runtime.Entity.Ingame.Card; // For CardAddressValueObject and DeckCardEntity
using Cryptos.Runtime.Entity.Outgame.Card; // For DeckNameValueObject
using Cryptos.Runtime.UseCase.OutGame;
using System.Collections.Generic;
using System; // For Array.Empty
using System.Linq;
using Cryptos.Runtime.Entity; // For .Select().ToArray()

namespace Cryptos.Runtime.Presenter.OutGame
{
    /// <summary>
    ///     デッキ編集画面のUIとユースケースを繋ぐプレゼンターである。
    /// </summary>
    public class DeckEditorPresenter
    {
        private readonly PlayerDeckUseCase _playerDeckUseCase;
        private readonly PlayerMasterUseCase _playerMasterUseCase; // Added
        private IDeckEditorUI _deckEditorUI;

        /// <summary>
        ///     DeckEditorPresenterの新しいインスタンスを生成する。
        /// </summary>
        /// <param name="playerDeckUseCase">プレイヤーデッキのユースケース。</param>
        /// <param name="playerMasterUseCase">プレイヤーマスターデータのユースケース。</param> // Added
        public DeckEditorPresenter(PlayerDeckUseCase playerDeckUseCase, PlayerMasterUseCase playerMasterUseCase)
        {
            _playerDeckUseCase = playerDeckUseCase;
            _playerMasterUseCase = playerMasterUseCase; // Added
        }

        /// <summary>
        ///     UIを登録する。
        /// </summary>
        /// <param name="ui">デッキエディタUIのインターフェース。</param>
        public void RegisterUI(IDeckEditorUI ui)
        {
            _deckEditorUI = ui;
        }

        /// <summary>
        ///     すべてのデッキ名をUIに表示する。
        /// </summary>
        public void DisplayAllDeckNames()
        {
            IReadOnlyCollection<string> deckNames = _playerDeckUseCase.GetAllDeckNames();
            _deckEditorUI.ShowDeckNames(deckNames);
            
            // 現在選択されているデッキがあれば、それをUIに反映
            DeckNameValueObject selectedDeckNameVO = _playerMasterUseCase.GetSelectedDeckName();
            if (!string.IsNullOrEmpty(selectedDeckNameVO.Value) && deckNames.Contains(selectedDeckNameVO.Value))
            {
                _deckEditorUI.SetSelectedDeckInDropdown(selectedDeckNameVO.Value);
                LoadAndDisplayDeck(selectedDeckNameVO.Value); // 選択されているデッキをロード
            }
        }

        /// <summary>
        ///     指定された名前のデッキをロードし、UIに表示する。
        /// </summary>
        /// <param name="deckName">ロードするデッキの名前。</param>
        public void LoadAndDisplayDeck(string deckName)
        {
            CardAddressValueObject[] deck = _playerDeckUseCase.GetDeck(deckName);
            if (deck != null)
            {
                // DeckViewModelに変換してUIに渡す。
                DeckCardEntity[] deckCardEntities = deck.Select(cardAddress => new DeckCardEntity(cardAddress.Value)).ToArray();
                DeckViewModel deckViewModel = new(new DeckNameValueObject(deckName), deckCardEntities);
                _deckEditorUI.ShowDeck(deckViewModel);
                _playerMasterUseCase.SetSelectedDeckName(new DeckNameValueObject(deckName)); // 選択中のデッキをマスターデータに保存
            }
            else
            {
                _deckEditorUI.ShowErrorMessage($"デッキ '{deckName}' が見つかりませんでした。");
            }
        }

        /// <summary>
        ///     新しいデッキを作成し、UIに表示する。
        /// </summary>
        /// <param name="newDeckName">新しいデッキの名前。</param>
        public void CreateNewDeck(string newDeckName)
        {
            // ロール名であるため、既に存在するかのチェックは不要（上書きされるため）
            // ただし、UI側で重複作成を制限するべき
            // ここでは空のデッキを登録し、選択状態にする
            CardAddressValueObject[] emptyDeck = Array.Empty<CardAddressValueObject>();
            _playerDeckUseCase.RegisterDeck(newDeckName, emptyDeck);
            
            DeckCardEntity[] deckCardEntities = emptyDeck.Select(cardAddress => new DeckCardEntity(cardAddress.Address)).ToArray(); // Updated
            DeckViewModel deckViewModel = new(new DeckNameValueObject(newDeckName), deckCardEntities);
            _deckEditorUI.ShowDeck(deckViewModel);
            _deckEditorUI.ShowSuccessMessage($"デッキ '{newDeckName}' を作成しました。");
            _playerMasterUseCase.SetSelectedDeckName(new DeckNameValueObject(newDeckName)); // 新しく作成したデッキを選択状態にする
            DisplayAllDeckNames(); // デッキリストを更新
        }

        /// <summary>
        ///     現在のデッキを保存する。
        /// </summary>
        /// <param name="deckName">保存するデッキの名前。</param>
        /// <param name="deck">保存するカードアドレスの配列。</param>
        public void SaveDeck(string deckName, CardAddressValueObject[] deck)
        {
            _playerDeckUseCase.RegisterDeck(deckName, deck);
            _playerMasterUseCase.SetSelectedDeckName(new DeckNameValueObject(deckName)); // 選択中のデッキをマスターデータに保存
            _deckEditorUI.ShowSuccessMessage($"デッキ '{deckName}' を保存しました。");
            DisplayAllDeckNames(); // デッキリストを更新
        }
    }
}
