using Cryptos.Runtime.Presenter.Ingame.System;
using SymphonyFrameWork.System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter
{
    public class OutgameManager : MonoBehaviour
    {
        public async void StartIngame()
        {
            await SceneLoader.UnloadScene(SceneListEnum.Outgame.ToString());
            await SceneLoader.LoadScene(SceneListEnum.Ingame.ToString());
        }

        [SerializeReference, SubclassSelector]
        private IGameInstaller _outgameInstaller;

        private async void Start()
        {
            await SceneLoader.LoadScene(SceneListEnum.Stage.ToString());
            _outgameInstaller.GameInitialize();
        }
    }
}
