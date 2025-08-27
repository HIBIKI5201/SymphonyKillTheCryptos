using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.UseCase.Ingame.Card;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Splines;

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

        public async Task NextWave(int index)
        {
            index--; //ウェーブ2への移動はindex=1なので１引く

            float distance = 0f;
            while (MovePositionOnSpline(index, distance))
            {
                distance += _speed * Time.deltaTime;
                await Awaitable.NextFrameAsync();
            }

            EndMoveAnimation();
        }

        [SerializeField, Min(0)]
        private float _speed = 1f;

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

        /// <summary>
        ///    スプライン上を移動する
        /// </summary>
        /// <param name="index"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private bool MovePositionOnSpline(int index, float distance)
        {
            //スプライン上の位置と回転を取得
            bool isSuccess = _pathContainer
                .GetPositionAndRotationByDistance(index, distance,
                out Vector3 position, out Quaternion rotation);

            //アニメーションの更新
            Vector3 localMoveDir = transform.InverseTransformDirection((position - transform.position).normalized);
            _animeManager.SetDirX(localMoveDir.x);
            _animeManager.SetDirY(localMoveDir.z);
            _animeManager.SetVelocity(_speed);

            //座標更新
            transform.position = position;
            transform.rotation = rotation;

            return isSuccess;
        }

        private void EndMoveAnimation()
        {
            _animeManager.SetVelocity(0f);
            _animeManager.SetDirX(0f);
            _animeManager.SetVelocity(0f);
        }
    }
}
