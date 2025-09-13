using Cryptos.Runtime.Framework;
using Cryptos.Runtime.Presenter;
using Cryptos.Runtime.Presenter.Ingame.System;
using SymphonyFrameWork.System;

namespace Cryptos.Runtime.InfraStructure
{
    public class OutgameStartSequience : IGameInstaller
    {
        public async void GameInitialize()
        {
            InputBuffer buffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
            OutgameManager outgameManager = await ServiceLocator.GetInstanceAsync<OutgameManager>();

            buffer.OnAlphabetKeyPressed += HandleAlphabetKey;

            void HandleAlphabetKey(char c)
            {
                outgameManager.StartIngame();
                buffer.OnAlphabetKeyPressed -= HandleAlphabetKey;
            }
        }

    }
}
