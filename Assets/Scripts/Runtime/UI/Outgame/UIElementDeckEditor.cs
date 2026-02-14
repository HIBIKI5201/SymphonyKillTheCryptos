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

        /// <summary>
        ///     デッキカードを設定する。
        ///     全カードの初期化完了を待ってからレイアウトを適用する。
        /// </summary>
        public async void SetDeckCards(IReadOnlyList<CardViewModel> cards)
        {
            _deckCards = cards;

            const int DECK_CARD_ELEMENTS_LENGTH = 5;
            _deckCardElements = new UIElementOutGameDeckEditorCard[DECK_CARD_ELEMENTS_LENGTH];
            for (int i = 0; i < DECK_CARD_ELEMENTS_LENGTH; i++)
            {
                UIElementOutGameDeckEditorCard card = new();
                _deckElement.Add(card);
                _deckCardElements[i] = card;
            }

            foreach (var card in _deckCardElements)
            {
                await card.InitializeTask;
            }

            DeckScrollTo(0);
        }

        public void SetStatusText(string text)
        {
            // TODO: _statusElementにテキストを設定するロジック
            UnityEngine.Debug.Log($"Status Text: {text}");
        }

        /// <summary>
        ///     所持カードを設定する。
        ///     全カードの初期化完了を待ってからレイアウトを適用する。
        /// </summary>
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

            // 全カードの初期化完了を待ち、カードデータをバインドする。
            for (int i = 0; i < _ownCardElements.Length; i++)
            {
                UIElementOutGameDeckEditorCard card = _ownCardElements[i];
                await card.InitializeTask;
                CardViewModel cardVM = _ownCards[i];
                card.BindCardData(cardVM);
                card.style.position = Position.Relative;
            }

            OwnScrollTo(0);
        }

        protected override ValueTask Initialize_S(VisualElement root)
        {
            // 各UI要素への参照を取得。
            _statusElement = root.Q<VisualElement>(STATUS_ELEMENT_NAME);
            _editButton = root.Q<Button>(EDIT_BUTTON_NAME);
            _saveButton = root.Q<Button>(SAVE_BUTTON_NAME);
            _roleSelectionArea = root.Q<VisualElement>(ROLE_SELECTION_AREA_NAME);
            _deckElement = root.Q<VisualElement>(DECK_ELEMENT_NAME);
            _cardSelectElement = root.Q<VisualElement>(CARD_SELECT_ELEMENT_NAME);

            // イベントハンドラの設定。
            _editButton.clicked += ClickedEditButton;
            _saveButton.clicked += ClickedSaveButton;

            // フォーカス可能な要素を初期化。
            _leftAreaFocusables = new List<Focusable> { _editButton, _saveButton, _roleSelectionArea };

            // 初期フォーカスを設定。
            SetFocus(_editButton);
            ChangeArea(FocusArea.LeftArea);

            Visible(false);

            if (EventSystem.current.TryGetComponent(out InputSystemUIInputModule module))
            {
                _uiInputModule = module;
                module.move.action.performed += OnNavigationMove;
                module.submit.action.started += OnNavigationSubmit;
                module.cancel.action.started += OnNavigationCancel;
            }
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);

            Debug.Log($"initialize deck editor{this.GetHashCode()}");
            return default;
        }

        // UI要素の定数。
        private const string STATUS_ELEMENT_NAME = "status";
        private const string EDIT_BUTTON_NAME = "Edit";
        private const string SAVE_BUTTON_NAME = "Save";
        private const string ROLE_SELECTION_AREA_NAME = "role-selection-area";
        private const string DECK_ELEMENT_NAME = "deck";
        private const string CARD_SELECT_ELEMENT_NAME = "card-select";

        private const string DECK_FOCUS_CLASS_NAME = "deck-focus";
        private const string OWN_FOCUS_CLASS_NAME = "own-focus";
        private const string LEFT_AREA_DISABLED_CLASS = "left-area-disabled";

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

        private void OnDetachedFromPanel(DetachFromPanelEvent e)
        {
            if (_uiInputModule != null)
            {
                _uiInputModule.move.action.performed -= OnNavigationMove;
                _uiInputModule.submit.action.started -= OnNavigationSubmit;
                _uiInputModule.cancel.action.started -= OnNavigationCancel;
            }
        }

        private void Visible(bool enable)
        {
            style.display = enable
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        private void ChangeArea(FocusArea area)
        {
            _currentFocusArea = area;

            _deckElement.RemoveFromClassList(DECK_FOCUS_CLASS_NAME);
            _cardSelectElement.RemoveFromClassList(OWN_FOCUS_CLASS_NAME);
            if (area == FocusArea.RightAreaTop)
            {
                _deckElement.AddToClassList(DECK_FOCUS_CLASS_NAME);
            }
            else if (area == FocusArea.RightAreaBottom)
            {
                _cardSelectElement.AddToClassList(OWN_FOCUS_CLASS_NAME);
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
        private void OnNavigationMove(InputAction.CallbackContext context)
        {
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
                DeckScrollTo(_currentDeckCardIndex);
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
                ChangeArea(FocusArea.LeftArea);
            }
        }

        private void SelectedRightUpper()
        {
            ChangeArea(FocusArea.RightAreaTop);
            _deckElement.AddToClassList(DECK_FOCUS_CLASS_NAME);

            _deckElement.pickingMode = PickingMode.Position;
            SetFocus(_deckElement);
            LeftButtonsFocusable(false);
        }

        private void SelectedRightLower()
        {
            ChangeArea(FocusArea.RightAreaBottom);
            _deckElement.pickingMode = PickingMode.Ignore;
            SetFocus(_cardSelectElement);
        }

        private void DeckScroll(int dir)
        {
            int nextIndex = Math.Clamp(_currentDeckCardIndex + dir, 0, _deckCards.Count - 1);
            _currentDeckCardIndex = nextIndex;
            DeckScrollTo(nextIndex);
        }

        private void DeckScrollTo(int index)
        {
            if (_deckCardElements == null || _deckCards == null) return;

            int center = _deckCardElements.Length / 2;


            float parentWidth = _deckElement.resolvedStyle.width;
            if (parentWidth <= 0 || float.IsNaN(parentWidth)) parentWidth = 1440f;

            for (int i = 0; i < _deckCardElements.Length; i++)
            {
                UIElementOutGameDeckEditorCard card = _deckCardElements[i];
                int deckIndex = index + i - center;

                if (deckIndex < 0 || _deckCards.Count <= deckIndex)
                {
                    card.style.display = DisplayStyle.None;
                    continue;
                }

                card.style.display = DisplayStyle.Flex;

                CardViewModel cardVM = _deckCards[deckIndex];
                card.BindCardData(cardVM);

                switch (i) // ※5個の時しか動かない。
                {
                    case 0: ChangeCardStyle(card, parentWidth * 0.055f, 0f, 1f); break;
                    case 1: ChangeCardStyle(card, parentWidth * 0.166f, 0f, 1f); break;
                    case 2: ChangeCardStyleCenter(card, 2.5f); break;
                    case 3: ChangeCardStyle(card, 0f, parentWidth * 0.166f, 1f); break;
                    case 4: ChangeCardStyle(card, 0f, parentWidth * 0.055f, 1f); break;
                }
            }
        }

        /// <summary>
        ///     左右どちらかにオフセットするカードのスタイルを設定する。
        /// </summary>
        private void ChangeCardStyle(UIElementOutGameDeckEditorCard card,
            float left, float right, float scale)
        {
            card.style.position = Position.Absolute;
            card.style.left = left > 0 ? new StyleLength(left) : new StyleLength(StyleKeyword.Auto);
            card.style.right = right > 0 ? new StyleLength(right) : new StyleLength(StyleKeyword.Auto);
            card.style.top = new StyleLength(StyleKeyword.Auto);
            card.style.bottom = new StyleLength(StyleKeyword.Auto);
            card.style.alignSelf = Align.Auto;
            card.style.scale = new StyleScale(new Scale(new Vector2(scale, scale)));
        }

        private void ChangeCardStyleCenter(UIElementOutGameDeckEditorCard card, float scale)
        {
            card.style.position = Position.Absolute;
            card.style.left = new StyleLength(StyleKeyword.Auto);
            card.style.right = new StyleLength(StyleKeyword.Auto);
            card.style.top = new StyleLength(StyleKeyword.Auto);
            card.style.bottom = new StyleLength(StyleKeyword.Auto);
            card.style.alignSelf = Align.Center;
            card.style.scale = new StyleScale(new Scale(new Vector2(scale, scale)));
        }

        private void OwnScroll(int dir)
        {
            int nextIndex = Math.Clamp(_currentOwnedCardIndex + dir, 0, _ownCardElements.Length - 1);
            _currentOwnedCardIndex = nextIndex;
            OwnScrollTo(nextIndex);
        }

        private void OwnScrollTo(int index)
        {
            if (_ownCardElements == null || _ownCards == null) return;

            for (int i = 0; i < _ownCardElements.Length; i++)
            {
                UIElementOutGameDeckEditorCard card = _ownCardElements[i];
                int ownIndex = i - index;

                // 表示範囲外のカードは display=None で非表示にする。
                if (ownIndex < 0 || _ownCards.Count <= ownIndex)
                {
                    card.style.display = DisplayStyle.None;
                    continue;
                }

                card.style.display = DisplayStyle.Flex;

                const int MARGIN = 20;
                card.style.marginLeft = ownIndex == 0 ? 0 : MARGIN;

                // 先頭カードのみ拡大してフォーカスを示す。
                card.style.scale = new StyleScale(new Scale(
                    ownIndex == 0 ? Vector2.one * 1.1f : Vector2.one * 0.9f));
            }
        }

        private void LeftButtonsFocusable(bool enable)
        {
            foreach (var item in _leftAreaFocusables)
            {
                if (item is VisualElement ve)
                {
                    if (enable)
                        ve.RemoveFromClassList(LEFT_AREA_DISABLED_CLASS);
                    else
                        ve.AddToClassList(LEFT_AREA_DISABLED_CLASS);
                }
            }
        }
    }
}