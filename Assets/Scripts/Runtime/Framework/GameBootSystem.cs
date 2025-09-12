using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cryptos.Runtime.Framework
{
    public class GameBootSystem : MonoBehaviour
    {
        [SerializeField, Tooltip("初期シーン")]
        private SceneListEnum _firstScene;

        private async void Awake()
        {
            //現在のシーンをアンロードする
            Scene activeScene = SceneManager.GetActiveScene();
            await SceneLoader.UnloadScene(activeScene.name);

            string sceneName = _firstScene.ToString();

            //初期シーンをロードする
            await SceneLoader.LoadScene(sceneName);

            Destroy(gameObject); //終了したら自己破棄する
        }
    }
}