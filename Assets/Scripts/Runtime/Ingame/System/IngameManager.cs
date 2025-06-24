using Cryptos.Runtime.System;
using SymphonyFrameWork;
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
            await MultiSceneLoader.LoadScene(scenes);
        }
    }
}
