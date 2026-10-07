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
    ///     カード表示は ScrollView の自動レイアウトで行い、resolvedStyle への依存を排除している。
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
        ///     デッキ内の全カード分の要素を生成し ScrollView に追加する。
        /// </summary>
        public async void SetDeckCards(IReadOnlyList<CardViewModel> cards)
        {
            await InitializeTask;

            _deckCards = cards;

            // 既存カードをクリア
            _deckScrollView.Clear();

            // デッキ内の全カード分の要素を生成
            _deckCardElements = new UIElementOutGameDeckEditorCard[cards.Count];
            for (int i = 0; i < cards.Count; i++)
            {
                UIElementOutGameDeckEditorCard card = new(InitializeType.None);
                _deckScrollView.Add(card);
                _deckCardElements[i] = card;
            }

            // 全カードの初期化完了を待つ
            foreach (var card in _deckCardElements)
            {
                await card.InitializeTask;
            }

            // カードデータをバインド
            for (int i = 0; i < _deckCardElements.Length; i++)
            {
                _deckCardElements[i].BindCardData(_deckCards[i]);
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
        ///     全カードの初期化完了を待ってから ScrollView に追加する。
        /// </summary>
        public async void SetOwnedCards(IReadOnlyList<CardViewModel> cards)
        {
            await InitializeTask;

            _ownCards = cards;

            // 既存カードをクリア
            _ownScrollView.Clear();

            _ownCardElements = new UIElementOutGameDeckEditorCard[_ownCards.Count];
            for (int i = 0; i < _ownCards.Count; i++)
            {
                UIElementOutGameDeckEditorCard card = new(InitializeType.None);
                _ownScrollView.Add(card);
                _ownCardElements[i] = card;
            }

            // 全カードの初期化完了を待ち、カードデータをバインドする。
            for (int i = 0; i < _ownCardElements.Length; i++)
            {
                UIElementOutGameDeckEditorCard card = _ownCardElements[i];
                await card.InitializeTask;
                card.BindCardData(_ownCards[i]);
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
            _deckScrollView = root.Q<ScrollView>(DECK_SCROLL_NAME);
            _ownScrollView = root.Q<ScrollView>(CARD_SELECT_ELEMENT_NAME);

            // ScrollView のコンテンツコンテナをカード横並び配置に設定
            _deckScrollView.contentContainer.style.flexDirection = FlexDirection.Row;
            _deckScrollView.contentContainer.style.alignItems = Align.Center;
            _deckScrollView.contentContainer.style.paddingLeft = 20;
            _deckScrollView.contentContainer.style.paddingRight = 20;
            _deckScrollView.contentContainer.style.height = Length.Percent(100);

            _ownScrollView.contentContainer.style.flexDirection = FlexDirection.Row;
            _ownScrollView.contentContainer.style.alignItems = Align.Center;
            _ownScrollView.contentContainer.style.paddingLeft = 10;
            _ownScrollView.contentContainer.style.paddingRight = 10;

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
        private const string DECK_SCROLL_NAME = "deck-scroll";
        private const string CARD_SELECT_ELEMENT_NAME = "card-select";

        private const string DECK_FOCUS_CLASS_NAME = "deck-focus";
        private const string OWN_FOCUS_CLASS_NAME = "own-focus";
        private const string LEFT_AREA_DISABLED_CLASS = "left-area-disabled";
        private const string DECK_CARD_SELECTED_CLASS = "deck-card-selected";
        private const string OWN_CARD_SELECTED_CLASS = "own-card-selected";

        private VisualElement _statusElement;
        private Button _editButton;
        private Button _saveButton;
        private VisualElement _roleSelectionArea;
        private VisualElement _deckElement;       // フォーカス・クラス管理用ラッパー
        private ScrollView _deckScrollView;        // カードスクロール用
        private ScrollView _ownScrollView;         // 所持カードスクロール用
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
            _ownScrollView.RemoveFromClassList(OWN_FOCUS_CLASS_NAME);
            if (area == FocusArea.RightAreaTop)
            {
                _deckElement.AddToClassList(DECK_FOCUS_CLASS_NAME);
            }
            else if (area == FocusArea.RightAreaBottom)
            {
                _ownScrollView.AddToClassList(OWN_FOCUS_CLASS_NAME);
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
            SetFocus(_ownScrollView);
        }

        private void DeckScroll(int dir)
        {
            if (_deckCards == null || _deckCardElements == null) return;
            int nextIndex = Math.Clamp(_currentDeckCardIndex + dir, 0, _deckCards.Count - 1);
            _currentDeckCardIndex = nextIndex;
            DeckScrollTo(nextIndex);
        }

        /// <summary>
        ///     指定インデックスのカードをハイライトし、ScrollView でスクロール表示する。
        ///     resolvedStyle.width への依存を排除し、ScrollTo API のみで制御する。
        /// </summary>
        private void DeckScrollTo(int index)
        {
            if (_deckCardElements == null || _deckCards == null) return;

            // 選択クラスを更新
            for (int i = 0; i < _deckCardElements.Length; i++)
            {
                if (i == index)
                    _deckCardElements[i].AddToClassList(DECK_CARD_SELECTED_CLASS);
                else
                    _deckCardElements[i].RemoveFromClassList(DECK_CARD_SELECTED_CLASS);
            }

            // ScrollView で選択カードが見えるようにスクロール
            if (index >= 0 && index < _deckCardElements.Length)
            {
                _deckScrollView.ScrollTo(_deckCardElements[index]);
            }
        }

        private void OwnScroll(int dir)
        {
            if (_ownCardElements == null) return;
            int nextIndex = Math.Clamp(_currentOwnedCardIndex + dir, 0, _ownCardElements.Length - 1);
            _currentOwnedCardIndex = nextIndex;
            OwnScrollTo(nextIndex);
        }

        /// <summary>
        ///     指定インデックスの所持カードをハイライトし、ScrollView でスクロール表示する。
        /// </summary>
        private void OwnScrollTo(int index)
        {
            if (_ownCardElements == null || _ownCards == null) return;

            // 選択クラスを更新
            for (int i = 0; i < _ownCardElements.Length; i++)
            {
                if (i == index)
                    _ownCardElements[i].AddToClassList(OWN_CARD_SELECTED_CLASS);
                else
                    _ownCardElements[i].RemoveFromClassList(OWN_CARD_SELECTED_CLASS);
            }

            // ScrollView で選択カードが見えるようにスクロール
            if (index >= 0 && index < _ownCardElements.Length)
            {
                _ownScrollView.ScrollTo(_ownCardElements[index]);
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