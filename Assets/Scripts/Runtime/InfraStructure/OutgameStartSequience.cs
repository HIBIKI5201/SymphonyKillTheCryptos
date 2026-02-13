using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Outgame.Card;
using Cryptos.Runtime.Entity.System.SaveData;
using Cryptos.Runtime.Presenter;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.Presenter.OutGame;
using Cryptos.Runtime.UI.Outgame;
using Cryptos.Runtime.UseCase.OutGame;
using SymphonyFrameWork.System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure
{
    /// <summary>
    ///     アウトゲームの初期化シーケンスを管理するクラスである。
    /// </summary>
    public class OutgameStartSequience : IGameInstaller
    {
        [SerializeField]
        private RoleDataBase _roleData;
        [SerializeField]
        private CardDataBase _cardData;

        public async ValueTask GameInitialize()
        {
            // サービスロケーターから必要なインスタンスを取得。
            OutgameUIManager uiManager = await ServiceLocator.GetInstanceAsync<OutgameUIManager>();
            OutgameManager outgameManager = await ServiceLocator.GetInstanceAsync<OutgameManager>();
            await InitializeUtility.WaitInitialize(uiManager);

            RoleEntity[] roles = _roleData.RoleAssets.Select(a => a.GetEntity()).ToArray();
            // プレイヤーデッキデータをロード。
            PlayerDeckSaveData.Load();
            PlayerDeckSaveData playerDeckSaveData = PlayerDeckSaveData.Data;
            PlayerDeckUseCase playerDeckUseCase = new(playerDeckSaveData);
            playerDeckUseCase.Initialize
                (_roleData.RoleAssets
                .Select(ra => new PlayerDeckUseCase.DeckData(new(ra.Name), ra.Deck.CardDataAddresses))
                .ToArray());
            PlayerDeckSaveData.Save();

            CardRepositoryImpl cardRepositoryImpl = new CardRepositoryImpl();
            PlayerMasterSaveData playerMasterData = SaveDataSystem<PlayerMasterSaveData>.Data;
            PlayerMasterUseCase playerMasterUseCase = new(playerMasterData);
            playerMasterUseCase.Initialize(_roleData.RoleAssets[0].GetEntity());

            DeckCardEntity[] allCard = await _cardData.GetDeckCards();
            DeckEditorPresenter deckEditorPresenter = new(playerDeckUseCase, playerMasterUseCase, allCard, roles);
            deckEditorPresenter.RegisterUI(uiManager.DeckEditor);
            deckEditorPresenter.InitializeDeckEditor();

            // スタートボタンが押された際のイベントハンドラを設定
            uiManager.OnPressedStartButton += HandlePressedStartButton;

            void HandlePressedStartButton()
            {
                outgameManager.StartIngame();
                uiManager.OnPressedStartButton -= HandlePressedStartButton;
            }
        }
    }
}
