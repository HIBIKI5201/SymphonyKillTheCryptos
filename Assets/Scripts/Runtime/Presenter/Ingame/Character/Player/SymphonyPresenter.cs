using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.UseCase.Ingame.Card;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Character.Player
{
    /// <summary>
    ///     プレイヤーのマネージャークラス
    /// </summary>
    public class SymphonyPresenter : MonoBehaviour
    {
        public CharacterEntity Self => _self;

        public void Init(CardUseCase cardUseCase, CharacterEntity self)
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
            _self = self;
        }

        public void ResetUsingCard()
        {
            _usingCard.Clear();
        }

        /// <summary>
        ///    スプライン上を移動する
        /// </summary>
        /// <param name="index"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public void MovePosition()
        {
            Vector3 moveVec = _spawnPoint.position - transform.position;

            //アニメーションの更新
            Vector3 localMoveDir = transform.InverseTransformDirection(moveVec.normalized);
            _animeManager.SetDirX(localMoveDir.x);
            _animeManager.SetDirY(localMoveDir.z);
            _animeManager.SetVelocity(moveVec.magnitude);

            transform.position = _spawnPoint.position;
            transform.rotation = _spawnPoint.rotation;
        }

        /// <summary>
        ///     移動アニメーション終了
        /// </summary>
        public void EndMoveAnimation()
        {
            _animeManager.SetVelocity(0f);
            _animeManager.SetDirX(0f);
            _animeManager.SetVelocity(0f);
        }

        [SerializeField]
        private Transform _spawnPoint;

        private CharacterEntity _self;
        private ISymphonyAnimeManager _animeManager;
        private CardUseCase _cardUseCase;


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
    }
}
