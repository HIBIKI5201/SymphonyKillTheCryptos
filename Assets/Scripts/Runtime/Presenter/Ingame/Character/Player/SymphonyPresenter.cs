using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Word;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.UseCase.Ingame.Card;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Splines;

namespace Cryptos.Runtime.Presenter.Ingame.Character.Player
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
            ResetUsingCard();

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

        private ISymphonyAnimeManager _animeManager;
        private CardUseCase _cardUseCase;
        private PlayerPathContainer _pathContainer;


        private Queue<CardEntity> _usingCard = new();

        private void Awake()
        {
            _animeManager = GetComponentInChildren<ISymphonyAnimeManager>();
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

            _usingCard.Enqueue(cardEntity);

            if (_usingCard.Count <= 1) //もしスキル中でなければ発動する。
            {
                _animeManager.ActiveSkill(cardEntity.AnimationClipID);
            }
        }

        private void HandleSkillTriggered(int index)
        {
            if (!_usingCard.TryPeek(out var card)) return;

            _cardUseCase.ExecuteCardEffect(card.GetContents(index));
        }

        private void HandleSkillEnded()
        {
            if (!_usingCard.TryDequeue(out _)) return;

            //もし残っていれば発動する。
            if (_usingCard.TryPeek(out var nextCard))
            {
                _animeManager.ActiveSkill(nextCard.AnimationClipID);
            }
        }

        private void ResetUsingCard()
        {
            _usingCard.Clear();
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
