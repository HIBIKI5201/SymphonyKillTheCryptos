using Cryptos.Runtime.Presenter.OutGame;
using System;
using System.Collections.Generic;
using System.Linq; // 追加
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
        event Action IDeckEditorUI.OnEditButtonClicked { add { _onEditButtonClicked += value; } remove { _onEditButtonClicked -= value; } }
        event Action IDeckEditorUI.OnSaveButtonClicked { add { _onSaveButtonClicked += value; } remove { _onSaveButtonClicked -= value; } }
        event Action IDeckEditorUI.OnRoleSelected { add { _onRoleSelected += value; } remove { _onRoleSelected -= value; } }
        event Action IDeckEditorUI.OnCancelButtonClicked { add { _onCancelButtonClicked += value; } remove { _onCancelButtonClicked -= value; } }
        event Action<CardViewModel> IDeckEditorUI.OnOwnedCardSelected { add { _onOwnedCardSelected += value; } remove { _onOwnedCardSelected -= value; } }
        event Action IDeckEditorUI.OnCardSwapRequested { add { _onCardSwapRequested += value; } remove { _onCardSwapRequested -= value; } }
        event Action<UnityEngine.UIElements.NavigationMoveEvent.Direction> IDeckEditorUI.OnNavigateDeckCard { add { _onNavigateDeckCard += value; } remove { _onNavigateDeckCard -= value; } }

        private DeckEditorPresenter _presenter;
        // UI要素への参照
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

        // フォーカス制御用
        private Focusable _currentFocusedElement;
        private List<Focusable> _focusableElements;
        private List<Focusable> _leftAreaFocusables;
        private List<Focusable> _rightAreaTopFocusables; // カード表示エリア
        private List<Focusable> _rightAreaBottomFocusables; // カード選択エリア
        private enum FocusArea { LeftArea, RightAreaTop, RightAreaBottom }
        private FocusArea _currentFocusArea = FocusArea.LeftArea;
        private UIElementOutGameDeckEditorCard _selectedDeckCardForSwapUI; // デッキ内の交換対象カード

        /// <summary>
        ///     プレゼンターを設定する。
        /// </summary>
        /// <param name="presenter">デッキエディタプレゼンター。</param>
        public void SetPresenter(DeckEditorPresenter presenter)
        {
            _presenter = presenter;
        }

        public void Show()
        {
            style.visibility = Visibility.Visible;
        }


        protected override async ValueTask Initialize_S(VisualElement root)
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
            _editButton.clicked += () => _onEditButtonClicked?.Invoke();
            _saveButton.clicked += () => _onSaveButtonClicked?.Invoke();

            // フォーカス可能な要素を初期化
            _leftAreaFocusables = new List<Focusable> { _editButton, _saveButton, _roleSelectionArea };
            _rightAreaTopFocusables = new List<Focusable> { _adjacentCardLeft, _currentCard, _adjacentCardRight };
            // _rightAreaBottomFocusables は、動的に生成されるカードに対応するため、後で実装

            // 初期フォーカスを設定
            SetFocus(_editButton);
            _currentFocusArea = FocusArea.LeftArea;

            style.visibility = Visibility.Hidden;

            // Navigationイベントの登録
            RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
            RegisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);

            await Task.CompletedTask;
        }

        // IDeckEditorUIイベントのバッキングフィールド
        private Action _onEditButtonClicked;
        private Action _onSaveButtonClicked;
        private Action _onRoleSelected;
        private Action _onCancelButtonClicked;
        private Action<CardViewModel> _onOwnedCardSelected;
        private Action _onCardSwapRequested;
        private Action<UnityEngine.UIElements.NavigationMoveEvent.Direction> _onNavigateDeckCard;

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
                else if (evt.direction == NavigationMoveEvent.Direction.Right) // 右入力で右エリアへ
                {
                    // 編集ボタンがフォーカスされている場合のみ右エリアへ移動可能にする
                    if (_currentFocusedElement == _editButton)
                    {
                        _currentFocusArea = FocusArea.RightAreaTop;
                        SetFocus(_rightAreaTopFocusables[0]); // 右エリアの最初の要素にフォーカス
                        evt.StopPropagation();
                        return;
                    }
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
                int currentIndex = _rightAreaTopFocusables.IndexOf(_currentFocusedElement);
                int nextIndex = currentIndex;

                if (evt.direction == NavigationMoveEvent.Direction.Left)
                {
                    nextIndex = (currentIndex - 1 + _rightAreaTopFocusables.Count) % _rightAreaTopFocusables.Count;
                }
                else if (evt.direction == NavigationMoveEvent.Direction.Right)
                {
                    nextIndex = (currentIndex + 1) % _rightAreaTopFocusables.Count;
                }
                else if (evt.direction == NavigationMoveEvent.Direction.Down) // 下入力で下エリアへ
                {
                    // TODO: _rightAreaBottomFocusablesの初期化が必要
                    if (_rightAreaBottomFocusables != null && _rightAreaBottomFocusables.Any())
                    {
                        _currentFocusArea = FocusArea.RightAreaBottom;
                        SetFocus(_rightAreaBottomFocusables[0]);
                        evt.StopPropagation();
                        return;
                    }
                }
                else if (evt.direction == NavigationMoveEvent.Direction.Left) // 左入力で左エリアへ
                {
                    // TODO: キャンセル入力での左エリアへの移動はOnNavigationSubmitで処理
                }

                if (nextIndex != currentIndex)
                {
                    SetFocus(_rightAreaTopFocusables[nextIndex]);
                    _onNavigateDeckCard?.Invoke(evt.direction); // イベントを発火
                    evt.StopPropagation();
                }
            }
            else if (_currentFocusArea == FocusArea.RightAreaBottom)
            {
                if (!_rightAreaBottomFocusables.Any()) return; // カードがない場合は何もしない

                int currentIndex = _rightAreaBottomFocusables.IndexOf(_currentFocusedElement);
                if (currentIndex == -1) // 現在フォーカスされている要素が見つからない場合、最初の要素にフォーカス
                {
                    SetFocus(_rightAreaBottomFocusables[0]);
                    evt.StopPropagation();
                    return;
                }

                int nextIndex = currentIndex;
                int cardsPerRow = 5; // 仮のカード数（UXMLのレイアウトによる）

                if (evt.direction == NavigationMoveEvent.Direction.Up)
                {
                    nextIndex = (currentIndex - cardsPerRow + _rightAreaBottomFocusables.Count) % _rightAreaBottomFocusables.Count;
                }
                else if (evt.direction == NavigationMoveEvent.Direction.Down)
                {
                    nextIndex = (currentIndex + cardsPerRow) % _rightAreaBottomFocusables.Count;
                }
                else if (evt.direction == NavigationMoveEvent.Direction.Left)
                {
                    nextIndex = (currentIndex - 1);
                    if (nextIndex < 0)
                    {
                        nextIndex = 0; // 左端に達したら最初の要素に留まる
                    }
                }
                else if (evt.direction == NavigationMoveEvent.Direction.Right)
                {
                    nextIndex = (currentIndex + 1);
                    if (nextIndex >= _rightAreaBottomFocusables.Count)
                    {
                        nextIndex = _rightAreaBottomFocusables.Count - 1; // 右端に達したら最後の要素に留まる
                    }
                }

                if (nextIndex != currentIndex)
                {
                    SetFocus(_rightAreaBottomFocusables[nextIndex]);
                    evt.StopPropagation();
                }
            }
        }

        private void OnNavigationSubmit(NavigationSubmitEvent evt)
        {
            if (_currentFocusedElement == _editButton)
            {
                // 編集ボタンが押されたら右エリアにフォーカスを移動
                _currentFocusArea = FocusArea.RightAreaTop;
                SetFocus(_rightAreaTopFocusables[0]); // 右エリアの最初の要素にフォーカス
                evt.StopPropagation();
            }
            else if (_currentFocusedElement == _saveButton)
            {
                _onSaveButtonClicked?.Invoke(); // 保存処理を実行
                evt.StopPropagation();
            }
            else if (_currentFocusedElement == _roleSelectionArea)
            {
                _onRoleSelected?.Invoke(); // ロール切り替えイベントを発火
                evt.StopPropagation();
            }
            // 右エリアでの決定入力ロジック
            else if (_currentFocusArea == FocusArea.RightAreaTop)
            {
                // 上部のカードが選択された場合
                if (_currentFocusedElement is UIElementOutGameDeckEditorCard selectedDeckCard)
                {
                    // 以前選択されていたカードのハイライトを解除
                    if (_selectedDeckCardForSwapUI != null)
                    {
                        _selectedDeckCardForSwapUI.RemoveFromClassList("selected-deck-card");
                    }
                    // 新しく選択されたカードを保持し、ハイライト
                    _selectedDeckCardForSwapUI = selectedDeckCard;
                    _selectedDeckCardForSwapUI.AddToClassList("selected-deck-card");

                    UnityEngine.Debug.Log($"Top card selected for swap: {selectedDeckCard.CardData.CardExplanation}");
                    evt.StopPropagation();
                }
            }
            else if (_currentFocusArea == FocusArea.RightAreaBottom)
            {
                // 下部の所持カードが選択された場合
                if (_currentFocusedElement is UIElementOutGameDeckEditorCard selectedCard)
                {
                    if (_selectedDeckCardForSwapUI != null)
                    {
                        // 交換リクエストを発火
                        _onCardSwapRequested?.Invoke(); // このイベントでPresenterに交換処理を依頼

                        // 交換後、選択状態をクリア
                        _selectedDeckCardForSwapUI.RemoveFromClassList("selected-deck-card");
                        _selectedDeckCardForSwapUI = null;
                        ClearSelectedOwnedCard(); // 所持カードの選択もクリア

                        // TODO: デッキ表示の更新をPresenterに依頼する
                    }
                    else
                    {
                        // 上部のカードが選択されていない場合は、所持カードを選択状態にする
                        _onOwnedCardSelected?.Invoke(selectedCard.CardData);
                    }

                    UnityEngine.Debug.Log($"Owned card selected: {selectedCard.CardData.CardExplanation}");
                    evt.StopPropagation();
                }
            }
        }



        public void SetStatusText(string text)
        {
            // TODO: _statusElementにテキストを設定するロジック
            UnityEngine.Debug.Log($"Status Text: {text}");
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

        public void SetAdjacentCards(CardViewModel leftCard, CardViewModel rightCard)
        {
            _adjacentCardLeft.BindCardData(leftCard);
            _adjacentCardRight.BindCardData(rightCard);
            UnityEngine.Debug.Log($"Adjacent Cards: Left - {leftCard.CardExplanation}, Right - {rightCard.CardExplanation}");
        }

        public void SetOwnedCards(IReadOnlyList<CardViewModel> cards)
        {
            // TODO: _ownedCardListに所持カード一覧を設定するロジック
            _ownedCardList.Clear();
            _rightAreaBottomFocusables = new List<Focusable>(); // ここで初期化し直す
            foreach (var card in cards)
            {
                var cardUI = new UIElementOutGameDeckEditorCard();
                cardUI.BindCardData(card);
                _ownedCardList.Add(cardUI);
                _rightAreaBottomFocusables.Add(cardUI); // フォーカス可能要素として追加
            }
            UnityEngine.Debug.Log($"Owned Cards Count: {cards.Count}");
        }

        private UIElementOutGameDeckEditorCard _selectedOwnedCardUI; // 選択された所持カードのUI要素

        public void SetSelectedOwnedCard(CardViewModel card)
        {
            // 以前選択されていたカードがあればハイライトを解除
            if (_selectedOwnedCardUI != null)
            {
                _selectedOwnedCardUI.RemoveFromClassList("selected-owned-card");
            }

            // 新しく選択されたカードを特定し、ハイライト
            _selectedOwnedCardUI = _ownedCardList.Children().OfType<UIElementOutGameDeckEditorCard>()
                                                .FirstOrDefault(c => c.CardData.AddressStr == card.AddressStr);
            if (_selectedOwnedCardUI != null)
            {
                _selectedOwnedCardUI.AddToClassList("selected-owned-card");
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
    }
}
