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

            _deckEditorUI.OnEditButtonClicked += OnEdit;
            _deckEditorUI.OnSaveButtonClicked += OnSave;
            _deckEditorUI.OnRoleSelected += OnRoleChange;
            _deckEditorUI.OnCancelButtonClicked += OnCancel;
            _deckEditorUI.OnOwnedCardSelected += OnOwnedCardSelected;
        }

        /// <summary>
        ///     デッキエディタの初期化を行う。
        /// </summary>
        public void InitializeDeckEditor()
        {
            InitializeRoleUI();
            InitializeOwnedCards();
            InitializeDeckCards(); 
        }

        private void InitializeRoleUI()
        {
            _deckEditorUI.SetStatusText($"現在のロール: {_roles[_currentRoleIndex].Name}");
            _deckEditorUI.SetRoleCharacter(_roles[_currentRoleIndex].Name);
        }

        private void InitializeOwnedCards()
        {
            _ownedCards = _allCard.Select(entity => new CardViewModel(entity)).ToList();
            _deckEditorUI.SetOwnedCards(_ownedCards);
        }

        private void InitializeDeckCards() // async Task にする必要はない
        {
            // UseCase層からCardAddressValueObject[]を取得
            DeckNameValueObject deckName = _playerMasterUseCase.GetSelectedDeckName();
            CardAddressValueObject[] savedDeckAddresses = _playerDeckUseCase.GetDeck(deckName);

            // CardAddressValueObject[] を DeckCardEntity[] に変換し、さらに CardViewModel[] に変換
            _currentDeckCards = new List<CardViewModel>();
            foreach (CardAddressValueObject address in savedDeckAddresses)
            {
                DeckCardEntity cardEntity = _allCard.FirstOrDefault(card => card.Address.Equals(address));
                if (cardEntity != null)
                {
                    _currentDeckCards.Add(new CardViewModel(cardEntity));
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Card entity not found for address: {address.Value} in _allCard.");
                }
            }

            // デッキ内のカードが空の場合のデフォルト処理（必要であれば）
            if (_currentDeckCards == null || _currentDeckCards.Count == 0)
            {
                // 例えば、_ownedCardsから最初のN枚をデフォルトデッキとする
                _currentDeckCards = _ownedCards.Take(3).ToList();
            }

            _deckEditorUI.SetDeckCards(_currentDeckCards);
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
            DeckNameValueObject deckName = new(_roles[_currentRoleIndex].Name);
            CardAddressValueObject[] deckAddresses = _currentDeckCards.Select(card => card.Address).ToArray();
            _playerDeckUseCase.RegisterDeck(deckName, deckAddresses);

            OnDeckSavedAndClosed?.Invoke();
            Debug.Log("Presenter: Deck saved!");
        }

        private void OnRoleChange()
        {
            _currentRoleIndex = (_currentRoleIndex + 1) % _roles.Length;
            string newRole = _roles[_currentRoleIndex].Name;
            _deckEditorUI.SetStatusText($"現在のロール: {newRole}");
            _deckEditorUI.SetRoleCharacter(newRole);
            Debug.Log($"Presenter: Role changed to {newRole}!");
        }

        private void OnCancel()
        {
            UnityEngine.Debug.Log("Presenter: Cancel button clicked!");
            // TODO: キャンセル処理ロジック
        }

        private void OnOwnedCardSelected(int index, CardViewModel card)
        {
            _selectedOwnedCard = card;
            UnityEngine.Debug.Log($"Presenter: Owned card selected for swap: {card.CardExplanation}");
            _currentDeckCards[index] = card;
        }
    }
}
