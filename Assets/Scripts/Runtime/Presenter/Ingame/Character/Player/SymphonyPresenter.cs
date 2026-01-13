using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.Card;
using SymphonyFrameWork.Utility;
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

        /// <summary>
        ///     カードユースケースに攻撃完了イベントを登録する。
        ///     自分のキャラクターエンティティも登録する。
        /// </summary>
        /// <param name="cardUseCase"></param>
        /// <param name="self"></param>
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

        /// <summary>
        ///     キューに登録されているカードをリセットする。
        /// </summary>
        public void ResetUsingCard()
        {
            _usingCardQueue.Clear();
        }

        /// <summary>
        ///    ターゲットのもとへ移動する。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public void MovePosition()
        {
            Vector3 moveVec = _spawnPoint.position - transform.position;
            moveVec.Normalize();

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

        /// <summary>
        ///     攻撃アニメーションが終了するまで待機する。
        /// </summary>
        /// <returns></returns>
        public async Task WaitForEndAttack()
        {
            await SymphonyTask.WaitUntil(() => !_animeManager.IsAttacking, destroyCancellationToken);
        }

        [SerializeField]
        private Transform _spawnPoint;

        private CharacterEntity _self;
        private ISymphonyAnimeManager _animeManager;
        private CardUseCase _cardUseCase;


        private Queue<CardEntity> _usingCardQueue = new();

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

            _usingCardQueue.Enqueue(cardEntity);

            if (_usingCardQueue.Count <= 1) //もしスキル中でなければ発動する。
            {
                _animeManager.ActiveSkill(cardEntity.AnimationClipID);
            }
        }

        private void HandleSkillTriggered(int index)
        {
            if (!_usingCardQueue.TryPeek(out var card)) return;

            _cardUseCase.ExecuteCardEffect(card.GetContents(index));
        }

        private void HandleSkillEnded()
        {
            if (!_usingCardQueue.TryDequeue(out _)) return;

            //もし残っていれば発動する。
            if (_usingCardQueue.TryPeek(out var nextCard))
            {
                _animeManager.ActiveSkill(nextCard.AnimationClipID);
            }
        }

        private void OnDrawGizmos()
        {
            if (_spawnPoint == null) return;
            Gizmos.color = Color.cyan;

            Vector3 size = new Vector3(1, 2, 1);
            Gizmos.DrawWireCube(_spawnPoint.position + Vector3.up * size.y / 2, size);
        }
    }
}
