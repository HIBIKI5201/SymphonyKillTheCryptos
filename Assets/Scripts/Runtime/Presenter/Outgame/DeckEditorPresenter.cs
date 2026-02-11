using Codice.Client.Common.FsNodeReaders;
using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Outgame.Card;
using Cryptos.Runtime.UseCase.OutGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine; // 追加

namespace Cryptos.Runtime.Presenter.OutGame
{
    /// <summary>
    ///     カード表示用のViewModel。UIはこれを通してカード情報を参照する。
    /// </summary>
    public readonly struct CardViewModel
    {
        public readonly string Name;
        public readonly Texture2D CardIcon;
        public readonly string CardExplanation;
        public readonly CardAddressValueObject Address; // Add Address property
        public string AddressStr => Address.Value;

        // Remove constructor with CardData entity
        // public CardViewModel(CardData entity)
        // {
        //     CardIcon = entity.CardIcon;
        //     CardExplanation = entity.CardExplanation;
        // }

        public CardViewModel(DeckCardEntity entity) // New constructor
        {
            Name = entity.CardName;
            Address = entity.Address;
            CardIcon = entity.CardIcon;
            CardExplanation = entity.CardExplanation;
        }

        public CardViewModel(string name, CardAddressValueObject address, Texture2D cardIcon, string cardExplanation) // Modified constructor
        {
            Name = name;
            Address = address;
            CardIcon = cardIcon;
            CardExplanation = cardExplanation;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CardViewModel other)) return false;
            return Address.Equals(other.Address);
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }
    }

    /// <summary>
    ///     デッキ編集画面のUIとユースケースを繋ぐプレゼンターである。
    /// </summary>
    public class DeckEditorPresenter
    {
        private readonly PlayerDeckUseCase _playerDeckUseCase;
        private readonly PlayerMasterUseCase _playerMasterUseCase;
        private IDeckEditorUI _deckEditorUI;

        private List<string> _roles = new List<string> { "Warrior", "Mage", "Healer" }; // 仮のロールリスト
        private int _currentRoleIndex = 0;

        private List<CardViewModel> _currentDeckCards = new List<CardViewModel>(); // 現在のデッキのスロットごとのカード
        private List<CardViewModel> _ownedCards = new List<CardViewModel>(); // 所持している全カード

        private int _currentDeckCardIndex = 0; // 現在デッキ内で選択されているカードのインデックス
        private CardViewModel? _selectedOwnedCard;
        private CardViewModel? _selectedOwnedCardForSwap; // 交換のために選択された所持カード

        /// <summary>
        ///     DeckEditorPresenterの新しいインスタンスを生成する。
        /// </summary>
        /// <param name="playerDeckUseCase">プレイヤーデッキのユースケース。</param>
        /// <param name="playerMasterUseCase">プレイヤーマスターデータのユースケース。</param> // Added
        public DeckEditorPresenter(PlayerDeckUseCase playerDeckUseCase, PlayerMasterUseCase playerMasterUseCase)
        {
            _playerDeckUseCase = playerDeckUseCase;
            _playerMasterUseCase = playerMasterUseCase;
        }

        public void RegisterUI(IDeckEditorUI ui)
        {
            _deckEditorUI = ui;

            // UIからのイベントを購読
            _deckEditorUI.OnEditButtonClicked += OnEdit;
            _deckEditorUI.OnSaveButtonClicked += OnSave;
            _deckEditorUI.OnRoleSelected += OnRoleChange;
            _deckEditorUI.OnCancelButtonClicked += OnCancel;
            _deckEditorUI.OnOwnedCardSelected += OnOwnedCardSelected;
            _deckEditorUI.OnCardSwapRequested += OnRequestCardSwap;
            _deckEditorUI.OnNavigateDeckCard += OnNavigateDeckCard; // イベントを購読
        }

        /// <summary>
        ///     デッキエディタの初期化を行う。
        /// </summary>
        public void InitializeDeckEditor()
        {
            // ロール情報や初期デッキ情報などをUIに渡すロジック
            _deckEditorUI.SetStatusText($"現在のロール: {_roles[_currentRoleIndex]}");
            _deckEditorUI.SetRoleCharacter(_roles[_currentRoleIndex]);

            // ダミーカードデータ
            var dummyCard1 = new CardViewModel("a", new CardAddressValueObject("card001"), Texture2D.redTexture, "Description for Card A");
            var dummyCard2 = new CardViewModel("b", new CardAddressValueObject("card002"), Texture2D.blackTexture, "Description for Card B");
            var dummyCard3 = new CardViewModel("c", new CardAddressValueObject("card003"), Texture2D.grayTexture, "Description for Card C");
            var dummyCard4 = new CardViewModel("d", new CardAddressValueObject("card004"), Texture2D.whiteTexture, "Description for Card D");

            _currentDeckCards = new List<CardViewModel> { dummyCard1, dummyCard2, dummyCard3 }; // デッキ内のカード
            _ownedCards = new List<CardViewModel> { dummyCard1, dummyCard2, dummyCard3, dummyCard4 }; // 所持カード

            // UIを更新
            _deckEditorUI.SetCurrentCard(_currentDeckCards[0]);
            _deckEditorUI.SetAdjacentCards(dummyCard3, _currentDeckCards[1]);
            _deckEditorUI.SetOwnedCards(_ownedCards);
            UpdateDeckCardDisplay();
        }

        private void OnEdit()
        {
            UnityEngine.Debug.Log("Presenter: Edit button clicked!");
            // TODO: 編集モードへの遷移ロジック
        }

        private event Action OnDeckSavedAndClosed; // デッキ保存後、画面を閉じるためのイベント

        private void OnSave()
        {
            // デッキを保存
            DeckNameValueObject deckName = new("DefaultDeck"); // 仮のデッキ名
            CardAddressValueObject[] deckAddresses = _currentDeckCards.Select(card => card.Address).ToArray();
            _playerDeckUseCase.RegisterDeck(deckName, deckAddresses);

            // 保存成功メッセージをUIに表示
            // _deckEditorUI.ShowSuccessMessage($"デッキ '{deckName.Value}' を保存しました。");

            OnDeckSavedAndClosed?.Invoke(); // デッキ保存後、画面を閉じるイベントを発火
            UnityEngine.Debug.Log("Presenter: Deck saved!");
        }

        private void OnRoleChange()
        {
            _currentRoleIndex = (_currentRoleIndex + 1) % _roles.Count;
            string newRole = _roles[_currentRoleIndex];
            _deckEditorUI.SetStatusText($"現在のロール: {newRole}");
            _deckEditorUI.SetRoleCharacter(newRole);
            UnityEngine.Debug.Log($"Presenter: Role changed to {newRole}!");
            // TODO: ロール変更に伴うデッキ制限やUIの更新ロジック
        }

        private void OnCancel()
        {
            UnityEngine.Debug.Log("Presenter: Cancel button clicked!");
            // TODO: キャンセル処理ロジック
        }

        private void OnOwnedCardSelected(CardViewModel card)
        {
            _selectedOwnedCard = card;
            _deckEditorUI.SetSelectedOwnedCard(card);
            UnityEngine.Debug.Log($"Presenter: Owned card selected for swap: {card.CardExplanation}");
        }

        private void OnRequestCardSwap()
        {
            if (_selectedOwnedCard == null || _currentDeckCards.Count == 0)
            {
                UnityEngine.Debug.LogWarning("Presenter: Cannot swap cards. No card selected for swap or deck is empty.");
                return;
            }

            // 現在デッキで選択されているカード
            CardViewModel targetDeckCard = _currentDeckCards[_currentDeckCardIndex];

            // _ownedCards から _selectedCardForSwap を削除し、targetDeckCard を追加
            _ownedCards.Remove(_selectedOwnedCard.Value);
            _ownedCards.Add(targetDeckCard);

            // _currentDeckCards の該当位置を _selectedCardForSwap で置き換える
            _currentDeckCards[_currentDeckCardIndex] = _selectedOwnedCard.Value;

            // UIを更新
            UpdateDeckCardDisplay();
            _deckEditorUI.SetOwnedCards(_ownedCards);
            _deckEditorUI.ClearSelectedOwnedCard(); // 選択状態をクリア

            _selectedOwnedCard = null; // 交換完了後、選択カードをクリア

            UnityEngine.Debug.Log($"Presenter: Cards swapped! Deck card {_currentDeckCardIndex} replaced with {_selectedOwnedCard.Value.CardExplanation}");
        }

        private void OnNavigateDeckCard(UnityEngine.UIElements.NavigationMoveEvent.Direction direction)
        {
            if (_currentDeckCards.Count == 0) return;

            if (direction == UnityEngine.UIElements.NavigationMoveEvent.Direction.Left)
            {
                _currentDeckCardIndex = (_currentDeckCardIndex - 1 + _currentDeckCards.Count) % _currentDeckCards.Count;
            }
            else if (direction == UnityEngine.UIElements.NavigationMoveEvent.Direction.Right)
            {
                _currentDeckCardIndex = (_currentDeckCardIndex + 1) % _currentDeckCards.Count;
            }

            UpdateDeckCardDisplay();
        }

        private void UpdateDeckCardDisplay()
        {
            if (_currentDeckCards.Count == 0) return;

            var currentCard = _currentDeckCards[_currentDeckCardIndex];
            var leftCard = _currentDeckCards[(_currentDeckCardIndex - 1 + _currentDeckCards.Count) % _currentDeckCards.Count];
            var rightCard = _currentDeckCards[(_currentDeckCardIndex + 1) % _currentDeckCards.Count];

            _deckEditorUI.SetCurrentCard(currentCard);
            _deckEditorUI.SetAdjacentCards(leftCard, rightCard);
        }
    }
}
