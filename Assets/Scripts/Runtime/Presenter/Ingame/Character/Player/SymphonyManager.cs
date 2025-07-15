using Cryptos.Runtime.Entity;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Player
{
    /// <summary>
    ///     プレイヤーのマネージャークラス
    /// </summary>
    public class SymphonyManager : MonoBehaviour, IAttackable, IHitable
    {
        public IHitableData HitableData => _hitableData;
        public IAttackableData AttackableData => _attackableData;

        private IHitableData _hitableData;
        private IAttackableData _attackableData;


        private SymphonyAnimeManager _animeManager;

        private void Awake()
        {
            _animeManager = GetComponentInChildren<SymphonyAnimeManager>();
        }
    }
}
