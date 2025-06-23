using SymphonyFrameWork.System;
using System.Threading.Tasks;

namespace Cryptos.Runtime.System
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
        }
    }
}