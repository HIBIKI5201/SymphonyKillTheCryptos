using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.UseCase.Ingame.Card;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Player
{
    /// <summary>
    ///     プレイヤーのマネージャークラス
    /// </summary>
    public class SymphonyPresenter : MonoBehaviour
    {
        public void Init(CardUseCase cardUseCase, PlayerPathContainer pathContainer)
        {
            cardUseCase.OnCardCompleted += HandleCardComplete;

            destroyCancellationToken.Register(() =>
            {
                if (cardUseCase != null)
                {
                    cardUseCase.OnCardCompleted -= HandleCardComplete;
                }
            });

            _cardUseCase = cardUseCase;
            _pathContainer = pathContainer;
        }

        [SerializeField]
        private ParticleSystem _muzzleFlash;

        private SymphonyAnimeManager _animeManager;
        private CardUseCase _cardUseCase;
        private PlayerPathContainer _pathContainer;

        private CardEntity _usingCard;

        private void Awake()
        {
            _animeManager = GetComponentInChildren<SymphonyAnimeManager>();
            if (_animeManager == null)
            {
                Debug.LogError("SymphonyAnimeManager is not found on the GameObject.");
            }
        }

        private void Start()
        {
            if (_animeManager == null) return;

            _animeManager.OnSkillTriggered += HandleSkillTriggered;
            _animeManager.OnSkillEnded += HandleSkillEnded;
        }

        private void HandleCardComplete(CardEntity cardEntity)
        {
            if (cardEntity == null) return;

            _animeManager.ActiveSkill(cardEntity.AnimationClipID);

            _usingCard = cardEntity;
        }

        private void HandleSkillTriggered(int index)
        {
            if (_usingCard == null) return;

            _cardUseCase.ExecuteCardEffect(_usingCard.GetContents(index));
        }

        private void HandleSkillEnded()
        {
            _usingCard = null;
        }
    }
}
