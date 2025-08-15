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
            _cardUseCase = cardUseCase;
            _cardUseCase.OnCardCompleted += HandleCardComplete;

            destroyCancellationToken.Register(() =>
            {
                if (_cardUseCase != null)
                {
                    Debug.Log("Unsubscribing from OnCardCompleted event in SymphonyManager.");
                    _cardUseCase.OnCardCompleted -= HandleCardComplete;
                }
            });
        }

        [SerializeField]
        private ParticleSystem _muzzleFlash;

        private SymphonyAnimeManager _animeManager;
        private CardUseCase _cardUseCase;

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
