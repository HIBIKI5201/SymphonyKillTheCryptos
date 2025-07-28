using Cryptos.Runtime.Framework;
using Cryptos.Runtime.Presenter.Ingame.Sequence;
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
            await MultiSceneLoader.LoadScenes(scenes);
            if (SceneLoader.GetExistScene(SceneListEnum.Stage.ToString(), out var stageScene))
            {
                SceneLoader.SetActiveScene(stageScene.name);
            }
            else
            {
                Debug.LogError($"Failed to load scene: {SceneListEnum.Stage}");
            }

            _ingameStartSequence.StartSequence();
        }

        [SerializeField]
        private IngameStartSequence _ingameStartSequence;
    }
}
