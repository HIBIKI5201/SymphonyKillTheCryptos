using Cryptos.Runtime.Entity;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Player
{
    /// <summary>
    ///     プレイヤーのマネージャークラス
    /// </summary>
    public class SymphonyManager : MonoBehaviour, IAttackable, IHitable
    {
        public IHitableData HitableData => _symphonyData;
        public IAttackableData AttackableData => _symphonyData;

        [SerializeField]
        private SymphonyData _symphonyData;

        private SymphonyAnimeManager _animeManager;

        private void Awake()
        {
            _animeManager = GetComponentInChildren<SymphonyAnimeManager>();
        }
    }
}
