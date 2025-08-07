using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.Card;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Player
{
    /// <summary>
    ///     プレイヤーのマネージャークラス
    /// </summary>
    public class SymphonyManager : MonoBehaviour
    {
        public void Init(CardUseCase cardUseCase)
        {
            cardUseCase.OnCardCompleted += HandleCardComplete;
        }

        [SerializeField]
        private SymphonySkillAnimationDataBase _skillAnimationDataBase;

        [SerializeField]
        private ParticleSystem _muzzleFlash;

        private SymphonyAnimeManager _animeManager;

        private void Awake()
        {
            _animeManager = GetComponentInChildren<SymphonyAnimeManager>();
            if (_animeManager == null)
            {
                Debug.LogError("SymphonyAnimeManager is not found on the GameObject.");
            }
        }

        private void HandleCardComplete(CardEntity cardEntity)
        {
            if (cardEntity == null) return;

            _animeManager.ActiveSkill(cardEntity.AnimationClipID);

        }
    }
}
