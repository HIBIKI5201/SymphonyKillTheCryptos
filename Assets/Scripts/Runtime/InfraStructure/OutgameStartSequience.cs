using Cryptos.Runtime.Entity.System.SaveData;
using Cryptos.Runtime.Presenter;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.Presenter.OutGame;
using Cryptos.Runtime.UI.Outgame;
using Cryptos.Runtime.UseCase.OutGame;
using SymphonyFrameWork.System;
using System.Threading.Tasks;
using UnityEngine; // For Debug.Log (temporary)

namespace Cryptos.Runtime.InfraStructure
{
    /// <summary>
    ///     アウトゲームの初期化シーケンスを管理するクラスである。
    /// </summary>
    public class OutgameStartSequience : IGameInstaller
    {
        public async ValueTask GameInitialize()
        {
            // サービスロケーターから必要なインスタンスを取得。
            OutgameUIManager uiManager = await ServiceLocator.GetInstanceAsync<OutgameUIManager>();
            OutgameManager outgameManager = await ServiceLocator.GetInstanceAsync<OutgameManager>();
            await InitializeUtility.WaitInitialize(uiManager);

            // プレイヤーデッキデータをロード。
            PlayerDeckSaveData playerDeckSaveData = SaveDataSystem<PlayerDeckSaveData>.Data;
            PlayerMasterSaveData playerMasterData = SaveDataSystem<PlayerMasterSaveData>.Data;

            CardRepositoryImpl cardRepositoryImpl = new CardRepositoryImpl();
            PlayerMasterUseCase playerMasterUseCase = new(playerMasterData);
            PlayerDeckUseCase playerDeckUseCase = new(playerDeckSaveData, cardRepositoryImpl);
            DeckEditorPresenter deckEditorPresenter = new(playerDeckUseCase, playerMasterUseCase);

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
