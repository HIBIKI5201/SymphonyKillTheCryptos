using Cryptos.Runtime.Entity.Outgame.Story;
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
            await masterUI.FadeOut(FADE_DURATION);
            await SceneLoader.UnloadScene(SceneListEnum.Outgame.ToString());
            await SceneLoader.LoadScene(SceneListEnum.Ingame.ToString());
            await masterUI.FadeIn(FADE_DURATION);
        }

        public async void StartStory(int index)
        {
            ScenarioDataEntity scenario = new(index);
            ServiceLocator.RegisterInstance(scenario);

            IMasterUIManager masterUI = await ServiceLocator.GetInstanceAsync<IMasterUIManager>();
            await masterUI.FadeOut(FADE_DURATION);
            await SceneLoader.UnloadScene(SceneListEnum.Outgame.ToString());
            await SceneLoader.LoadScene(SceneListEnum.Story.ToString());
            await masterUI.FadeIn(FADE_DURATION);
        }

        private const float FADE_DURATION = 1f;

        [SerializeReference, SymphonySubclassSelector]
        private IGameInstaller _outgameInstaller;

        private async void Start()
        {
            try
            {
                await SceneLoader.LoadScene(SceneListEnum.Stage.ToString());
                await _outgameInstaller.GameInitialize();
            }
            catch (Exception ex) 
            {
                Debug.LogError(ex);
                return; 
            }
        }
    }
}
