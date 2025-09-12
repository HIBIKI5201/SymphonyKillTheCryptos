using Cryptos.Runtime.Framework;
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
            _ingameStartSequence.GameInitialize();

            await MultiSceneLoader.LoadScenes(_requireScenes.Select(s => s.ToString()).ToArray());
            if (SceneLoader.GetExistScene(SceneListEnum.Stage.ToString(), out Scene stageScene))
            {
                SceneLoader.SetActiveScene(stageScene.name);
            }
            else
            {
                Debug.LogError($"Failed to load scene: {SceneListEnum.Stage}");
            }
        }

        [SerializeField]
        private readonly SceneListEnum[] _requireScenes = { SceneListEnum.Stage };

        [Space]

        [SerializeReference, SubclassSelector]
        private IGameInstaller _ingameStartSequence;
    }
}
