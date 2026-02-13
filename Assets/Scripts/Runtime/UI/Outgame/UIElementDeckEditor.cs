using Cryptos.Runtime.Presenter.OutGame;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Outgame.Deck
{
    /// <summary>
    ///     デッキエディタのUI要素を管理するクラスである。
    /// </summary>
    [UxmlElement]
    public partial class UIElementDeckEditor : VisualElementBase, IDeckEditorUI
    {
        public UIElementDeckEditor() : base("DeckEditor") { }

        public event Action OnEditButtonClicked;
        public event Action OnSaveButtonClicked;
        public event Action OnRoleSelected;
        public event Action OnCancelButtonClicked;
        public event Action<int, CardViewModel> OnOwnedCardSelected;

        public void Show()
        {
            Visible(true);
            SetFocus(_saveButton);
        }

        public void SetRoleCharacter(string characterName)
        {
            UnityEngine.Debug.Log($"Role Character: {characterName}");
        }

        public void SetDeckCards(IReadOnlyList<CardViewModel> cards)
        {
            // カードの一覧を計算。
            _deckCards = cards;

            const int DECK_CARD_ELEMENTS_LENGTH = 5;
            _deckCardElements = new UIElementOutGameDeckEditorCard[DECK_CARD_ELEMENTS_LENGTH];
            for (int i = 0; i < DECK_CARD_ELEMENTS_LENGTH; i++)
            {
                UIElementOutGameDeckEditorCard card = new();
                _deckElement.Add(card);
                _deckCardElements[i] = card;
            }
            DeckScrollTo(0);
        }

        public void SetStatusText(string text)
        {
            // TODO: _statusElementにテキストを設定するロジック
            UnityEngine.Debug.Log($"Status Text: {text}");
        }

        public async void SetOwnedCards(IReadOnlyList<CardViewModel> cards)
        {
            _ownCards = cards;
            _ownCardElements = new UIElementOutGameDeckEditorCard[_ownCards.Count];
            for (int i = 0; i < _ownCards.Count; i++)
            {
                UIElementOutGameDeckEditorCard card = new();
                _cardSelectElement.Add(card);
                _ownCardElements[i] = card;
            }

            for (int i = 0; i < _ownCardElements.Length; i++)
            {
                UIElementOutGameDeckEditorCard card = _ownCardElements[i];
                await card.InitializeTask;
                CardViewModel cardVM = _ownCards[i];
                card.BindCardData(cardVM);
            }

            OwnScroll(0);
        }


        protected override ValueTask Initialize_S(VisualElement root)
        {
            // 各UI要素への参照を取得
            _statusElement = root.Q<VisualElement>(STATUS_ELEMENT_NAME);
            _editButton = root.Q<Button>(EDIT_BUTTON_NAME);
            _saveButton = root.Q<Button>(SAVE_BUTTON_NAME);
            _roleSelectionArea = root.Q<VisualElement>(ROLE_SELECTION_AREA_NAME);
            _deckElement = root.Q<VisualElement>(DECK_ELEMENT_NAME);
            _cardSelectElement = root.Q<VisualElement>(CARD_SELECT_ELEMENT_NAME);

            // イベントハンドラの設定
            _editButton.clicked += ClickedEditButton;
            _saveButton.clicked += ClickedSaveButton;

            // フォーカス可能な要素を初期化
            _leftAreaFocusables = new List<Focusable> { _editButton, _saveButton, _roleSelectionArea };

            // 初期フォーカスを設定
            SetFocus(_editButton);
            _currentFocusArea = FocusArea.LeftArea;

            Visible(false);

            if (EventSystem.current.TryGetComponent(out InputSystemUIInputModule module))
            {
                _uiInputModule = module;
                module.move.action.performed += OnNavigationMove;
                module.submit.action.started += OnNavigationSubmit;
                module.cancel.action.started += OnNavigationCancel;
            }
            Debug.Log($"initialize deck editor{this.GetHashCode()}");
            return default;
        }

        // UI要素の定数
        private const string STATUS_ELEMENT_NAME = "status";
        private const string EDIT_BUTTON_NAME = "Edit";
        private const string SAVE_BUTTON_NAME = "Save";
        private const string ROLE_SELECTION_AREA_NAME = "role-selection-area";
        private const string DECK_ELEMENT_NAME = "deck";
        private const string CARD_SELECT_ELEMENT_NAME = "card-select";

        private VisualElement _statusElement;
        private Button _editButton;
        private Button _saveButton;
        private VisualElement _roleSelectionArea;
        private VisualElement _deckElement;
        private VisualElement _cardSelectElement;
        private InputSystemUIInputModule _uiInputModule;

        private Focusable _currentFocusedElement;
        private List<Focusable> _leftAreaFocusables;

        private UIElementOutGameDeckEditorCard[] _deckCardElements;
        private UIElementOutGameDeckEditorCard[] _ownCardElements;

        private FocusArea _currentFocusArea = FocusArea.LeftArea;

        private IReadOnlyList<CardViewModel> _deckCards;
        private IReadOnlyList<CardViewModel> _ownCards;
        private int _currentDeckCardIndex;
        private int _currentOwnedCardIndex;


        private enum FocusArea
        {
            LeftArea,
            RightAreaTop,
            RightAreaBottom
        }

        private void Visible(bool enable)
        {
            if (enable)
            {
                style.visibility = Visibility.Visible;
                _deckElement.style.display = DisplayStyle.Flex;
            }
            else
            {
                style.visibility = Visibility.Hidden;
                _deckElement.style.display = DisplayStyle.None;
            }
        }

        /// <summary>
        ///     デッキ画面に移行する。
        /// </summary>
        private void ClickedEditButton()
        {
            SelectedRightUpper();
            OnEditButtonClicked?.Invoke();
        }

        /// <summary>
        ///     ウィンドウを閉じる。
        /// </summary>
        private void ClickedSaveButton()
        {
            Visible(false);
            OnSaveButtonClicked?.Invoke();
        }

        /// <summary>
        ///     フォーカス対象を更新する。
        /// </summary>
        /// <param name="newFocus"></param>
        private void SetFocus(Focusable newFocus)
        {
            _currentFocusedElement = newFocus;
            if (_currentFocusedElement != null)
            {
                _currentFocusedElement.Focus();
            }
        }

        /// <summary>
        ///     ナビゲーションを手動で行う。
        /// </summary>
        /// <param name="evt"></param>
        private void OnNavigationMove(InputAction.CallbackContext context)
        {
            // 右エリアのフォーカス移動ロジック
            if (_currentFocusArea == FocusArea.RightAreaTop)
            {
                int dir = Mathf.RoundToInt(context.ReadValue<Vector2>().x);
                DeckScroll(dir);
            }
            else if (_currentFocusArea == FocusArea.RightAreaBottom)
            {
                int dir = Mathf.RoundToInt(context.ReadValue<Vector2>().x);
                OwnScroll(dir);
            }
        }

        private void OnNavigationSubmit(InputAction.CallbackContext context)
        {
            if (_currentFocusedElement == _roleSelectionArea)
            {
                OnRoleSelected?.Invoke();
            }
            else if (_currentFocusArea == FocusArea.RightAreaTop)
            {
                SelectedRightLower();
            }
            else if (_currentFocusArea == FocusArea.RightAreaBottom)
            {
                UIElementOutGameDeckEditorCard selectedCard = _ownCardElements[_currentOwnedCardIndex];
                OnOwnedCardSelected?.Invoke(_currentDeckCardIndex, selectedCard.CardData);

                SelectedRightUpper();
                _currentOwnedCardIndex = 0;
                OwnScrollTo(0);
                DeckScrollTo(_currentDeckCardIndex); // デッキのカードを更新するため。
            }
        }

        private void OnNavigationCancel(InputAction.CallbackContext context)
        {
            if (_currentFocusArea == FocusArea.LeftArea)
            {
                ClickedSaveButton();
            }
            else
            {
                LeftButtonsFocusable(true);
                SetFocus(_editButton);
                _currentFocusArea = FocusArea.LeftArea;
            }
        }

        private void SelectedRightUpper()
        {
            _currentFocusArea = FocusArea.RightAreaTop;
            SetFocus(_deckElement);
            LeftButtonsFocusable(false);
        }

        private void SelectedRightLower()
        {
            _currentFocusArea = FocusArea.RightAreaBottom;
            SetFocus(_cardSelectElement);
        }

        private void DeckScroll(int dir)
        {
            int nextIndex = _currentDeckCardIndex + dir;
            nextIndex = Math.Clamp(nextIndex, 0, _deckCards.Count - 1);
            DeckScrollTo(nextIndex);
            _currentDeckCardIndex = nextIndex;
        }

        private async void DeckScrollTo(int index)
        {
            int center = _deckCardElements.Length / 2;

            for (int i = 0; i < _deckCardElements.Length; i++)
            {
                UIElementOutGameDeckEditorCard card = _deckCardElements[i];
                await card.InitializeTask;
                int deckIndex = index + i - center;

                if (deckIndex < 0 || _deckCards.Count <= deckIndex)
                {
                    card.visible = false;
                    continue;
                }
                else
                {
                    card.visible = true;
                }

                CardViewModel cardVM = _deckCards[deckIndex];
                card.BindCardData(cardVM);

                switch (i) // ※5個の時しか動かない。
                {
                    case 0: ChangeCardStyle(card, 100, 0, 1); break;
                    case 1: ChangeCardStyle(card, 300, 0, 1); break;
                    case 2: ChangeCardStyle(card, 0, 0, 2.5f); break;
                    case 3: ChangeCardStyle(card, 0, 300, 1); break;
                    case 4: ChangeCardStyle(card, 0, 100, 1); break;
                }

                void ChangeCardStyle(UIElementOutGameDeckEditorCard card,
                    float left, float right, float scale)
                {
                    StyleLength rightStyle = 0 < right ? new(right) : new(StyleKeyword.Auto);
                    StyleLength leftStyle = 0 < left ? new(left) : new(StyleKeyword.Auto);

                    card.style.right = rightStyle;
                    card.style.left = leftStyle;
                    card.style.scale = new StyleScale(new Scale(new Vector2(scale, scale)));
                }
            }
        }

        private void OwnScroll(int dir)
        {
            int nextIndex = _currentOwnedCardIndex + dir;
            nextIndex = Math.Clamp(nextIndex, 0, _ownCardElements.Length - 1);
            OwnScrollTo(nextIndex);
            _currentOwnedCardIndex = nextIndex;
        }

        private async void OwnScrollTo(int index)
        {
            for (int i = 0; i < _ownCardElements.Length; i++)
            {
                UIElementOutGameDeckEditorCard card = _ownCardElements[i];
                await card.InitializeTask;
                int ownIndex = i - index;

                if (ownIndex < 0 || _ownCards.Count <= ownIndex)
                {
                    card.visible = false;
                    continue;
                }
                else
                {
                    card.visible = true;
                }

                const int MARGIN = 100;
                card.style.left = new(MARGIN * ownIndex);

                if (ownIndex == 0)
                {
                    card.style.scale = new StyleScale(new Scale(Vector2.one * 1.15f));
                }
                else
                {
                    card.style.scale = new StyleScale(new Scale(Vector2.one));
                }
            }
        }

        private void LeftButtonsFocusable(bool enable)
        {
            foreach (var item in _leftAreaFocusables)
            {
                item.focusable = enable;
            }
        }
    }
}
