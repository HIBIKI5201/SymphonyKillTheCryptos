using Cryptos.Runtime.Presenter;
using Cryptos.Runtime.Presenter.OutGame;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Outgame.Deck
{
    /// <summary>
    ///     デッキエディタのUI要素を管理するクラスである。
    /// </summary>
    [UxmlElement]
    public partial class UIElementDeckEditor : VisualElementBase, IDeckEditorUI
    {
        private DeckEditorPresenter _presenter;

        private TextField _newDeckNameInput;
        private Button _createDeckButton;
        private DropdownField _deckSelectionDropdown;
        private Button _loadDeckButton;
        private Button _saveDeckButton;
        private VisualElement _currentDeckCardHolder;

        /// <summary>
        ///     UIElementDeckEditorの新しいインスタンスを生成する。
        /// </summary>
        public UIElementDeckEditor() : base("DeckEditor") { }

        /// <summary>
        ///     プレゼンターを設定する。
        /// </summary>
        /// <param name="presenter">デッキエディタプレゼンター。</param>
        public void SetPresenter(DeckEditorPresenter presenter)
        {
            _presenter = presenter;
            _presenter.RegisterUI(this);
        }

        public void ShowDeckNames(IReadOnlyCollection<string> deckNames)
        {
            _deckSelectionDropdown.choices = new List<string>(deckNames);
            if (deckNames.Count > 0 && string.IsNullOrEmpty(_deckSelectionDropdown.value))
            {
                _deckSelectionDropdown.value = _deckSelectionDropdown.choices[0];
            }
            else if (deckNames.Count == 0)
            {
                _deckSelectionDropdown.value = string.Empty;
            }
        }

        public void SetSelectedDeckInDropdown(string deckName)
        {
            _deckSelectionDropdown.value = deckName;
        }


        public void ShowDeck(DeckViewModel deckViewModel)
        {
            // TODO: カード情報を表示するロジックを実装
            Debug.Log($"デッキ '{deckViewModel.DeckName}' を表示中。カード数: {deckViewModel.DeckName.Length}");
            _currentDeckCardHolder.Clear(); // 一旦クリア
            // ここにカードVisualElementを生成して_currentDeckCardHolderに追加するロジックが入る
        }

        public void ShowErrorMessage(string message)
        {
            Debug.LogError($"[DeckEditorUI Error]: {message}");
            // TODO: 実際のUIにエラーメッセージを表示する
        }

        public void ShowSuccessMessage(string message)
        {
            Debug.Log($"[DeckEditorUI Success]: {message}");
            // TODO: 実際のUIに成功メッセージを表示する
        }

        protected override async ValueTask Initialize_S(VisualElement root)
        {
            _newDeckNameInput = root.Q<TextField>(NEW_DECK_NAME_INPUT);
            _createDeckButton = root.Q<Button>(CREATE_DECK_BUTTON);
            _deckSelectionDropdown = root.Q<DropdownField>(DECK_SELECTION_DROPDOWN);
            _loadDeckButton = root.Q<Button>(LOAD_DECK_BUTTON);
            _saveDeckButton = root.Q<Button>(SAVE_DECK_BUTTON);
            _currentDeckCardHolder = root.Q<VisualElement>(CURRENT_DECK_CARD_HOLDER);

            _createDeckButton.clicked += OnCreateDeckClicked;
            _loadDeckButton.clicked += OnLoadDeckClicked;
            _saveDeckButton.clicked += OnSaveDeckClicked;

            _deckSelectionDropdown.RegisterValueChangedCallback(OnDeckSelectionChanged);

            await _presenter.DisplayAllDeckNames();
        }

        private const string NEW_DECK_NAME_INPUT = "new-deck-name-input";
        private const string CREATE_DECK_BUTTON = "create-deck-button";
        private const string DECK_SELECTION_DROPDOWN = "deck-selection-dropdown";
        private const string LOAD_DECK_BUTTON = "load-deck-button";
        private const string SAVE_DECK_BUTTON = "save-deck-button";
        private const string CURRENT_DECK_CARD_HOLDER = "current-deck-card-holder";

        private void OnCreateDeckClicked()
        {
            string newDeckNameStr = _newDeckNameInput.value;
            if (string.IsNullOrWhiteSpace(newDeckNameStr))
            {
                ShowErrorMessage("新しいデッキ名を入力してください。");
                return;
            }
            _presenter.CreateNewDeck(newDeckNameStr);
        }

        private void OnLoadDeckClicked()
        {
            string selectedDeckNameStr = _deckSelectionDropdown.value;
            if (string.IsNullOrWhiteSpace(selectedDeckNameStr))
            {
                ShowErrorMessage("ロードするデッキを選択してください。");
                return;
            }
            _ = _presenter.LoadAndDisplayDeck(selectedDeckNameStr);
        }

        private void OnSaveDeckClicked()
        {
            string currentDeckNameStr = _deckSelectionDropdown.value;
            if (string.IsNullOrWhiteSpace(currentDeckNameStr))
            {
                ShowErrorMessage("保存するデッキを選択してください。");
                return;
            }
            // TODO: 現在編集中のデッキのカード情報を取得するロジック
            //CardAddressValueObject[] currentDeckCards = new CardAddressValueObject[0]; // Placeholder
            //_presenter.SaveDeck();
        }

        private void OnDeckSelectionChanged(ChangeEvent<string> evt)
        {
            _ = _presenter.LoadAndDisplayDeck(evt.newValue);
        }

    }
}
