using Cryptos.Runtime.Presenter.OutGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // IDeckEditorUIのイベント実装
        public event Action OnEditButtonClicked;
        public event Action OnSaveButtonClicked;
        public event Action OnRoleSelected;
        public event Action OnCancelButtonClicked;
        public event Action<CardViewModel> OnOwnedCardSelected;
        public event Action OnCardSwapRequested;
        public event Action<NavigationMoveEvent.Direction> OnNavigateDeckCard;

        public void Show()
        {
            style.visibility = Visibility.Visible;
            SetFocus(_saveButton);
        }

        public void SetRoleCharacter(string characterName)
        {
            // TODO: _roleSelectionAreaにキャラクターアイコンを設定するロジック
            UnityEngine.Debug.Log($"Role Character: {characterName}");
        }

        public void SetCurrentCard(CardViewModel card)
        {
            _currentCard.BindCardData(card);
            UnityEngine.Debug.Log($"Current Card: {card.CardExplanation}");
        }

        public void SetStatusText(string text)
        {
            // TODO: _statusElementにテキストを設定するロジック
            UnityEngine.Debug.Log($"Status Text: {text}");
        }

        public void SetAdjacentCards(CardViewModel leftCard, CardViewModel rightCard)
        {
            _adjacentCardLeft.BindCardData(leftCard);
            _adjacentCardRight.BindCardData(rightCard);
            UnityEngine.Debug.Log($"Adjacent Cards: Left - {leftCard.CardExplanation}, Right - {rightCard.CardExplanation}");
        }

        public void SetOwnedCards(IReadOnlyList<CardViewModel> cards)
        {
            _ownedCardList.Clear();
            foreach (var card in cards)
            {
                UIElementOutGameDeckEditorCard cardUI = new();
                cardUI.BindCardData(card);
                _ownedCardList.Add(cardUI);
                _ownedCardList.Add(cardUI);
            }
            UnityEngine.Debug.Log($"Owned Cards Count: {cards.Count}");
        }

        public void SetSelectedOwnedCard(CardViewModel card)
        {
            // 以前選択されていたカードがあればハイライトを解除
            if (_selectedOwnedCardUI != null)
            {
                _selectedOwnedCardUI.RemoveFromClassList("selected-owned-card");
            }

            // 新しく選択されたカードを特定し、ハイライト
            _selectedOwnedCardUI =
                _ownedCardList
                .Children()
                .OfType<UIElementOutGameDeckEditorCard>()
                .FirstOrDefault(c => c.CardData.AddressStr == card.AddressStr);

            if (_selectedOwnedCardUI != null)
            {
                _selectedOwnedCardUI.AddToClassList("selected-owned-card");
                ScrollTo(_selectedOwnedCardUI); // 選択されたカードまでスクロール
            }
        }

        public void ClearSelectedOwnedCard()
        {
            if (_selectedOwnedCardUI != null)
            {
                _selectedOwnedCardUI.RemoveFromClassList("selected-owned-card");
                _selectedOwnedCardUI = null;
            }
        }


        protected override ValueTask Initialize_S(VisualElement root)
        {
            // 各UI要素への参照を取得
            _statusElement = root.Q<VisualElement>(STATUS_ELEMENT_NAME);
            _editButton = root.Q<Button>(EDIT_BUTTON_NAME);
            _saveButton = root.Q<Button>(SAVE_BUTTON_NAME);
            _roleSelectionArea = root.Q<VisualElement>(ROLE_SELECTION_AREA_NAME);
            _deckElement = root.Q<VisualElement>(DECK_ELEMENT_NAME);
            _adjacentCardLeft = root.Q<UIElementOutGameDeckEditorCard>(ADJACENT_CARD_LEFT_NAME);
            _currentCard = root.Q<UIElementOutGameDeckEditorCard>(CURRENT_CARD_NAME);
            _adjacentCardRight = root.Q<UIElementOutGameDeckEditorCard>(ADJACENT_CARD_RIGHT_NAME);
            _cardSelectElement = root.Q<VisualElement>(CARD_SELECT_ELEMENT_NAME);
            _ownedCardList = root.Q<VisualElement>(OWNED_CARD_LIST_NAME);

            // イベントハンドラの設定
            _editButton.clicked += ClickedEditButton;
            _saveButton.clicked += ClickedSaveButton;

            // フォーカス可能な要素を初期化
            _leftAreaFocusables = new List<Focusable> { _editButton, _saveButton, _roleSelectionArea };

            // 初期フォーカスを設定
            SetFocus(_editButton);
            _currentFocusArea = FocusArea.LeftArea;

            style.visibility = Visibility.Hidden;

            // Navigationイベントの登録
            RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
            RegisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);

            return default;
        }

        // UI要素の定数
        private const string STATUS_ELEMENT_NAME = "status";
        private const string EDIT_BUTTON_NAME = "Edit";
        private const string SAVE_BUTTON_NAME = "Save";
        private const string ROLE_SELECTION_AREA_NAME = "role-selection-area";
        private const string DECK_ELEMENT_NAME = "deck";
        private const string ADJACENT_CARD_LEFT_NAME = "adjacent-card-left";
        private const string CURRENT_CARD_NAME = "current-card";
        private const string ADJACENT_CARD_RIGHT_NAME = "adjacent-card-right";
        private const string CARD_SELECT_ELEMENT_NAME = "card-select";
        private const string OWNED_CARD_LIST_NAME = "owned-card-list";

        private VisualElement _statusElement;
        private Button _editButton;
        private Button _saveButton;
        private VisualElement _roleSelectionArea;
        private VisualElement _deckElement;
        private UIElementOutGameDeckEditorCard _adjacentCardLeft;
        private UIElementOutGameDeckEditorCard _currentCard;
        private UIElementOutGameDeckEditorCard _adjacentCardRight;
        private VisualElement _cardSelectElement;
        private VisualElement _ownedCardList;

        private Focusable _currentFocusedElement;
        private List<Focusable> _focusableElements;
        private List<Focusable> _leftAreaFocusables;

        private FocusArea _currentFocusArea = FocusArea.LeftArea;

        private int _currentDeckCardIndex;
        private int _currentOwnedCardIndex;
        private UIElementOutGameDeckEditorCard _selectedOwnedCardUI;


        private enum FocusArea
        { 
            LeftArea, 
            RightAreaTop, 
            RightAreaBottom 
        }

        private void ClickedEditButton()
        {
            SelectedRightUpper();
            OnEditButtonClicked?.Invoke();
        }

        private void ClickedSaveButton()
        {
            style.visibility = Visibility.Hidden;
            OnSaveButtonClicked?.Invoke();
        }

        private void SetFocus(Focusable newFocus)
        {
            _currentFocusedElement = newFocus;
            if (_currentFocusedElement != null)
            {
                _currentFocusedElement.Focus();
            }
        }

        private void OnNavigationMove(NavigationMoveEvent evt)
        {
            // 左エリアのフォーカス移動ロジック
            if (_currentFocusArea == FocusArea.LeftArea)
            {
                int currentIndex = _leftAreaFocusables.IndexOf(_currentFocusedElement);
                int nextIndex = currentIndex;

                if (evt.direction == NavigationMoveEvent.Direction.Up)
                {
                    nextIndex = (currentIndex - 1 + _leftAreaFocusables.Count) % _leftAreaFocusables.Count;
                }
                else if (evt.direction == NavigationMoveEvent.Direction.Down)
                {
                    nextIndex = (currentIndex + 1) % _leftAreaFocusables.Count;
                }
                else if (evt.direction == NavigationMoveEvent.Direction.Right)
                {
                }

                if (nextIndex != currentIndex)
                {
                    SetFocus(_leftAreaFocusables[nextIndex]);
                    evt.StopPropagation();
                }
            }
            // 右エリアのフォーカス移動ロジック
            else if (_currentFocusArea == FocusArea.RightAreaTop)
            {
                int currentIndex = _currentDeckCardIndex;
                int nextIndex = currentIndex;

                if (evt.direction == NavigationMoveEvent.Direction.Left)
                {
                    nextIndex = (currentIndex - 1);
                }
                else if (evt.direction == NavigationMoveEvent.Direction.Right)
                {
                    nextIndex = (currentIndex + 1);
                }

                if (nextIndex != currentIndex)
                {
                    OnNavigateDeckCard?.Invoke(evt.direction);
                    evt.StopPropagation();
                }
            }
            else if (_currentFocusArea == FocusArea.RightAreaBottom)
            {
                if (!_ownedCardList.Children().Any()) return; // カードがない場合は何もしない

                int totalCards = _ownedCardList.Children().Count();
                int previousIndex = _currentOwnedCardIndex;

                if (evt.direction == NavigationMoveEvent.Direction.Left)
                {
                    _currentOwnedCardIndex = (_currentOwnedCardIndex - 1 + totalCards) % totalCards;
                }
                else if (evt.direction == NavigationMoveEvent.Direction.Right)
                {
                    _currentOwnedCardIndex = (_currentOwnedCardIndex + 1) % totalCards;
                }

                if (previousIndex != _currentOwnedCardIndex)
                {
                    // 選択されたカードの情報をPresenterに通知し、スクロール位置を調整してもらう
                    var selectedCardUI = _ownedCardList.Children().ElementAt(_currentOwnedCardIndex) as UIElementOutGameDeckEditorCard;
                    if (selectedCardUI != null)
                    {
                        OnOwnedCardSelected?.Invoke(selectedCardUI.CardData);
                    }
                    evt.StopPropagation();
                }
            }
        }

        private void OnNavigationSubmit(NavigationSubmitEvent evt)
        {
            if (_currentFocusedElement == _editButton)
            {
                evt.StopPropagation();
            }
            else if (_currentFocusedElement == _saveButton)
            {
                OnSaveButtonClicked?.Invoke(); // 保存処理を実行
                evt.StopPropagation();
            }
            else if (_currentFocusedElement == _roleSelectionArea)
            {
                OnRoleSelected?.Invoke(); // ロール切り替えイベントを発火
                evt.StopPropagation();
            }
            // 右エリアでの決定入力ロジック
            else if (_currentFocusArea == FocusArea.RightAreaTop)
            {
                evt.StopPropagation();
            }
            else if (_currentFocusArea == FocusArea.RightAreaBottom)
            {
                // 下部の所持カードが選択された場合
                if (_currentFocusedElement is UIElementOutGameDeckEditorCard selectedCard)
                {
                    OnOwnedCardSelected?.Invoke(selectedCard.CardData); // 所持カードを選択状態にする
                    UnityEngine.Debug.Log($"Owned card selected: {selectedCard.CardData.CardExplanation}");
                    evt.StopPropagation();
                }
            }
        }

        private void SelectedRightUpper()
        {
            _currentFocusArea = FocusArea.RightAreaTop;
            _ownedCardList.Focus();
        }

        private void ScrollTo(UIElementOutGameDeckEditorCard card)
        {

        }
    }
}
