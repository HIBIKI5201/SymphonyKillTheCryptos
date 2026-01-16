using Cryptos.Runtime.Presenter;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.UI.Outgame;
using SymphonyFrameWork.System;
using System.Threading.Tasks;

namespace Cryptos.Runtime.InfraStructure
{
    public class OutgameStartSequience : IGameInstaller
    {
        public async ValueTask GameInitialize()
        {
            OutgameUIManager uiManager = await ServiceLocator.GetInstanceAsync<OutgameUIManager>();
            OutgameManager outgameManager = await ServiceLocator.GetInstanceAsync<OutgameManager>();

            await InitializeUtility.WaitInitialize(uiManager);

            uiManager.OnPressedStartButton += HandlePressedStartButton;

            void HandlePressedStartButton()
            {
                outgameManager.StartIngame();
                uiManager.OnPressedStartButton -= HandlePressedStartButton;
            }
        }
    }
}
