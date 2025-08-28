using Cryptos.Runtime.Framework;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Ingame.System
{
    /// <summary>
    ///     インゲームシーンのマネジメントを行う
    /// </summary>
    public class IngameManager : MonoBehaviour, IInitializeAsync
    {
        private readonly string[] scenes = { SceneListEnum.Stage.ToString() };

        Task IInitializeAsync.InitializeTask { get; set; }

        async Task IInitializeAsync.InitializeAsync()
        {
            _ingameStartSequence.GameInitialize();

            await MultiSceneLoader.LoadScenes(scenes);
            if (SceneLoader.GetExistScene(SceneListEnum.Stage.ToString(), out var stageScene))
            {
                SceneLoader.SetActiveScene(stageScene.name);
            }
            else
            {
                Debug.LogError($"Failed to load scene: {SceneListEnum.Stage}");
            }
        }

        [SerializeReference, SubclassSelector]
        private IGameInstaller _ingameStartSequence;
    }
}
