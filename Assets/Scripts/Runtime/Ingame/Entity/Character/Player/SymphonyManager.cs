using UnityEngine;

namespace Cryptos.Runtime
{
    /// <summary>
    ///     プレイヤーのマネージャークラス
    /// </summary>
    public class SymphonyManager : MonoBehaviour
    {


        private SymphonyAnimeManager _animeManager;

        private void Awake()
        {
            _animeManager = GetComponentInChildren<SymphonyAnimeManager>();
        }
    }
}
