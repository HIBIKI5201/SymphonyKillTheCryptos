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
        private readonly string[] scenes = { "Stage" };

        Task IInitializeAsync.InitializeTask { get; set; }

        async Task IInitializeAsync.InitializeAsync()
        {
            await MultiSceneLoader.LoadScenes(scenes);
            if (SceneLoader.GetExistScene("Stage", out var stageScene))
            {
                SceneLoader.SetActiveScene(stageScene.name);
            }
        }
    }
}
