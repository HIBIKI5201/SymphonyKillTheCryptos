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

            if (cardEntity.AnimationClip == null)
            {
                Debug.LogWarning("CardEntity does not have an AnimationClip assigned.");
                return;
            }

            int index = _skillAnimationDataBase[cardEntity.AnimationClip];
            if (index < 0)
            {
                Debug.LogWarning($"AnimationClip {cardEntity.AnimationClip.name} does not have a valid SkillIndex.");
                return;
            }

            _animeManager.ActiveSkill(index);

        }
    }
}
