using Cryptos.Runtime.Framework;
using Cryptos.Runtime.Presenter.System;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    /// <summary>
    ///     インゲームシーンのマネジメントを行う
    /// </summary>
    public class IngameManager : MonoBehaviour, IInitializeAsync
    {
        Task IInitializeAsync.InitializeTask { get; set; }

        async Task IInitializeAsync.InitializeAsync()
        {
            await _ingameStartSequence.GameInitialize();

            await SceneLoader.LoadScenes(_requireScenes.Select(s => s.ToString()).ToArray());
            if (SceneLoader.GetExistScene(SceneListEnum.Stage.ToString(), out Scene stageScene))
            {
                SceneLoader.SetActiveScene(stageScene.name);
            }
            else
            {
                Debug.LogError($"Failed to load scene: {SceneListEnum.Stage}");
            }

            IMasterUIManager masterUI = await ServiceLocator.GetInstanceAsync<IMasterUIManager>();
            await masterUI.FadeIn(2, destroyCancellationToken);
        }

        [SerializeField, Tooltip("依存しているシーン。")]
        private SceneListEnum[] _requireScenes = { SceneListEnum.Stage };

        [Space]

        [SerializeReference, SymphonySubclassSelector, Tooltip("初期化処理。")]
        private IGameInstaller _ingameStartSequence;
    }
}
