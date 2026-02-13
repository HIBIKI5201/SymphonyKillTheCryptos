using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Outgame.Card;
using Cryptos.Runtime.UseCase.OutGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public readonly CardAddressValueObject Address;
        public string AddressStr => Address.Value;

        public CardViewModel(DeckCardEntity entity)
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
            if (!(obj is CardViewModel other)) { return false; }
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
        /// <summary>
        ///     DeckEditorPresenterの新しいインスタンスを生成する。
        /// </summary>
        /// <param name="playerDeckUseCase">プレイヤーデッキのユースケース。</param>
        /// <param name="playerMasterUseCase">プレイヤーマスターデータのユースケース。</param> // Added
        public DeckEditorPresenter(
            PlayerDeckUseCase playerDeckUseCase,
            PlayerMasterUseCase playerMasterUseCase,
            DeckCardEntity[] allCard,
            RoleEntity[] roles
            )
        {
            _playerDeckUseCase = playerDeckUseCase;
            _playerMasterUseCase = playerMasterUseCase;
            _allCard = allCard;
            _roles = roles;
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
        }

        /// <summary>
        ///     デッキエディタの初期化を行う。
        /// </summary>
        public void InitializeDeckEditor()
        {
            // ロール情報や初期デッキ情報などをUIに渡す。
            _deckEditorUI.SetStatusText($"現在のロール: {_roles[_currentRoleIndex].Name}");
            _deckEditorUI.SetRoleCharacter(_roles[_currentRoleIndex].Name);

            _ownedCards = _allCard.Select(entity => new CardViewModel(entity)).ToList();

            // デッキ内のカードは、所持カードの最初の3枚を暫定的に設定
            _currentDeckCards = _ownedCards.Take(3).ToList();

            _deckEditorUI.SetDeckCards(_currentDeckCards);
            _deckEditorUI.SetOwnedCards(_ownedCards);
        }

        private event Action OnDeckSavedAndClosed; // デッキ保存後、画面を閉じるためのイベント

        private readonly PlayerDeckUseCase _playerDeckUseCase;
        private readonly PlayerMasterUseCase _playerMasterUseCase;
        private readonly DeckCardEntity[] _allCard;
        private readonly RoleEntity[] _roles;
        private IDeckEditorUI _deckEditorUI;

        private int _currentRoleIndex = 0;

        private List<CardViewModel> _currentDeckCards = new List<CardViewModel>();
        private List<CardViewModel> _ownedCards = new List<CardViewModel>();

        private int _currentDeckCardIndex = 0;
        private CardViewModel? _selectedOwnedCard;

        private void OnEdit()
        {
            UnityEngine.Debug.Log("Presenter: Edit button clicked!");
        }


        private void OnSave()
        {
            // デッキを保存
            DeckNameValueObject deckName = new("DefaultDeck"); // 仮のデッキ名
            CardAddressValueObject[] deckAddresses = _currentDeckCards.Select(card => card.Address).ToArray();
            _playerDeckUseCase.RegisterDeck(deckName, deckAddresses);

            // 保存成功メッセージをUIに表示
            // _deckEditorUI.ShowSuccessMessage($"デッキ '{deckName.Value}' を保存しました。");

            OnDeckSavedAndClosed?.Invoke(); // デッキ保存後、画面を閉じるイベントを発火
            Debug.Log("Presenter: Deck saved!");
        }

        private void OnRoleChange()
        {
            _currentRoleIndex = (_currentRoleIndex + 1) % _roles.Length;
            string newRole = _roles[_currentRoleIndex].Name;
            _deckEditorUI.SetStatusText($"現在のロール: {newRole}");
            _deckEditorUI.SetRoleCharacter(newRole);
            Debug.Log($"Presenter: Role changed to {newRole}!");
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

            _selectedOwnedCard = null; // 交換完了後、選択カードをクリア

            UnityEngine.Debug.Log($"Presenter: Cards swapped! Deck card {_currentDeckCardIndex} replaced with {_selectedOwnedCard.Value.CardExplanation}");
        }
    }
}
