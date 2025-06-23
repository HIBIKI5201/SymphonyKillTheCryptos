using SymphonyFrameWork.System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeatKeeper.Runtime.System
{
    /// <summary>
    ///     シーンをロードする処理を担当する
    /// </summary>
    public static class MultiSceneLoader
    {
        /// <summary>
        ///     シーンを複数同時にロードして初期化処理も行う
        /// </summary>
        /// <param name="names"></param>
        public static async Task LoadScene(params string[] names)
        {
            Task[] tasks = new Task[names.Length];

            //シーンのロードを登録
            for (int i = 0; i < names.Length; i++)
            {
                tasks[i] = SceneLoader.LoadScene(names[i]);
            }

            await Task.WhenAll(tasks); //全てのロード終了まで待機

            tasks = new Task[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                //各シーンのInitializeを
                if (SceneLoader.GetExistScene(names[i], out var scene))
                {
                    tasks[i] = SceneInitialize(scene);
                }
            }

            await Task.WhenAll(tasks); //全ての初期化が終了するまで待機
        }

        private static async Task SceneInitialize(Scene scene)
        {
            GameObject[] objs = scene.GetRootGameObjects();

            if (objs.Length <= 0) return;

            List<Task> tasks = new();

            //オブジェクト達の初期化を登録
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].TryGetComponent<IInitializeAsync>
                    (out var initialize))
                {
                    tasks.Add(initialize.DoInitialize());
                }
            }

            if (tasks.Count <= 0) return; //タスクが無ければ終了

            await Task.WhenAll(tasks);
        }
    }
}