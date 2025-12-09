using Cryptos.Runtime.Presenter;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.UI.Outgame;
using SymphonyFrameWork.System;

namespace Cryptos.Runtime.InfraStructure
{
    public class OutgameStartSequience : IGameInstaller
    {
        public async void GameInitialize()
        {
            OutgameUIManager uiManager = await ServiceLocator.GetInstanceAsync<OutgameUIManager>();
            OutgameManager outgameManager = await ServiceLocator.GetInstanceAsync<OutgameManager>();

            uiManager.OnPressedStartButton += HandlePressedStartButton;

            void HandlePressedStartButton()
            {
                outgameManager.StartIngame();
                uiManager.OnPressedStartButton -= HandlePressedStartButton;
            }
        }

    }
}
