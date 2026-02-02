using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.Presenter.System;
using SymphonyFrameWork.System;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter
{
    public class OutgameManager : MonoBehaviour
    {
        public async void StartIngame()
        {
            IMasterUIManager masterUI = await ServiceLocator.GetInstanceAsync<IMasterUIManager>();
            await masterUI.FadeOut(2, destroyCancellationToken);

            await SceneLoader.UnloadScene(SceneListEnum.Outgame.ToString());
            await SceneLoader.LoadScene(SceneListEnum.Ingame.ToString());
        }

        [SerializeReference, SymphonySubclassSelector]
        private IGameInstaller _outgameInstaller;

        private async void Start()
        {
            try
            {
                await SceneLoader.LoadScene(SceneListEnum.Stage.ToString());
                await _outgameInstaller.GameInitialize();
            }
            catch (Exception) { return; }
        }
    }
}
