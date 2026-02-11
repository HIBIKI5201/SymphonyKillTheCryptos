using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Outgame.Card;
using Cryptos.Runtime.UseCase.OutGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptos.Runtime.Presenter.OutGame
{
    /// <summary>
    ///     デッキ編集画面のUIとユースケースを繋ぐプレゼンターである。
    /// </summary>
    public class DeckEditorPresenter
    {
        private readonly PlayerDeckUseCase _playerDeckUseCase;
        private readonly PlayerMasterUseCase _playerMasterUseCase;
        private IDeckEditorUI _deckEditorUI;

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
        public async Task DisplayAllDeckNames()
        {
            IReadOnlyCollection<DeckNameValueObject> deckNames = _playerDeckUseCase.GetAllDeckNames();
            _deckEditorUI.ShowDeckNames(deckNames.Select(dn => dn.Value).ToList());

            // 現在選択されているデッキがあれば、それをUIに反映
            DeckNameValueObject selectedDeckNameVO = _playerMasterUseCase.GetSelectedDeckName();
            if (!string.IsNullOrEmpty(selectedDeckNameVO.Value) && deckNames.Any(dn => dn.Value == selectedDeckNameVO.Value))
            {
                _deckEditorUI.SetSelectedDeckInDropdown(selectedDeckNameVO.Value);
                await LoadAndDisplayDeck(selectedDeckNameVO.Value);
            }
        }

        /// <summary>
        ///     指定された名前のデッキをロードし、UIに表示する。
        /// </summary>
        /// <param name="deckName">ロードするデッキの名前。</param>
        public async Task LoadAndDisplayDeck(string name)
        {
            DeckNameValueObject deckName = new(name);

            CardAddressValueObject[] deck = _playerDeckUseCase.GetDeck(deckName);
            if (deck != null)
            {
                // DeckViewModelに変換してUIに渡す。
                var deckCardEntityTasks = deck.Select(async cardAddress => await _playerDeckUseCase.CreateDeckCardEntity(cardAddress));
                DeckCardEntity[] deckCardEntities = await Task.WhenAll(deckCardEntityTasks);

                DeckViewModel deckViewModel = new(deckName, deckCardEntities);
                _deckEditorUI.ShowDeck(deckViewModel);
                _playerMasterUseCase.SetSelectedDeckName(deckName);
            }
            else
            {
                _deckEditorUI.ShowErrorMessage($"デッキ '{deckName.Value}' が見つかりませんでした。");
            }
        }

        /// <summary>
        ///     新しいデッキを作成し、UIに表示する。
        /// </summary>
        /// <param name="deckNameVO">新しいデッキの名前。</param>
        public async void CreateNewDeck(string name)
        {
            DeckNameValueObject deckNameVO = new(name);

            CardAddressValueObject[] emptyDeck = Array.Empty<CardAddressValueObject>();
            _playerDeckUseCase.RegisterDeck(deckNameVO, emptyDeck);

            var deckCardEntityTasks = emptyDeck.Select(async cardAddress => await _playerDeckUseCase.CreateDeckCardEntity(cardAddress));
            DeckCardEntity[] deckCardEntities = await Task.WhenAll(deckCardEntityTasks);

            DeckViewModel deckViewModel = new(deckNameVO, deckCardEntities);
            _deckEditorUI.ShowDeck(deckViewModel);
            _deckEditorUI.ShowSuccessMessage($"デッキ '{deckNameVO.Value}' を作成しました。");
            _playerMasterUseCase.SetSelectedDeckName(deckNameVO);
            await DisplayAllDeckNames();
        }

        /// <summary>
        ///     現在のデッキを保存する。
        /// </summary>
        /// <param name="deckName">保存するデッキの名前。</param>
        /// <param name="deck">保存するカードアドレスの配列。</param>
        public async void SaveDeck(DeckNameValueObject deckName, CardAddressValueObject[] deck) // async に変更
        {
            _playerDeckUseCase.RegisterDeck(deckName, deck);
            _playerMasterUseCase.SetSelectedDeckName(deckName);
            _deckEditorUI.ShowSuccessMessage($"デッキ '{deckName.Value}' を保存しました。");
            await DisplayAllDeckNames();
        }
    }
}
