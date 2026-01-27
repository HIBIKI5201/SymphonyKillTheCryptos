using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.Character;
using SymphonyFrameWork.Utility;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Character.Player
{
    /// <summary>
    ///     プレイヤーのマネージャークラス
    /// </summary>
    public class SymphonyPresenter : MonoBehaviour, ISymphonyPresenter, ICardAnimationHandler
    {
        public CharacterEntity Self => _self;

        // CardExecutionUseCase への依存を追加
        private CardExecutionUseCase _cardExecutionUseCase;

        /// <summary>
        /// 自分のキャラクターエンティティを登録する。
        /// </summary>
        /// <param name="self"></param>
        public void Init(
            CharacterEntity self,
            SymphonyData data,
            CardExecutionUseCase useCase,
            ComboEntity comboEntity)
        {
            _self = self;
            _data = data;
            _cardExecutionUseCase = useCase;
            _comboEntity = comboEntity;

            ObserbeCombo();
        }

        /// <summary>
        ///     キューに登録されているカードをリセットする。
        /// </summary>
        public void ResetUsingCard()
        {
            _cardExecutionUseCase?.Reset(); // UseCase の Reset を呼び出す
        }

        /// <summary>
        ///    ターゲットのもとへ移動する。
        /// </summary>
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
        public async Task WaitForEndAttack()
        {
            await SymphonyTask.WaitUntil(() => !_animeManager.IsAttacking, destroyCancellationToken);
        }

        public event Action<int> OnSkillTriggered;
        public event Action OnSkillEnded;

        /// <summary>
        /// 指定されたアニメーションクリップIDでスキルアニメーションをアクティブにする。
        /// </summary>
        /// <param name="animationClipID">アクティブにするアニメーションクリップのID。</param>
        public void ActiveSkill(int animationClipID)
        {
            _animeManager.ActiveSkill(animationClipID);
        }

        [SerializeField]
        private Transform _spawnPoint;

        private CharacterEntity _self;
        private SymphonyData _data;
        private ISymphonyAnimeManager _animeManager;
        private ComboEntity _comboEntity;

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

            // アニメーションマネージャーのイベントを中継
            _animeManager.OnSkillTriggered += (index) => OnSkillTriggered?.Invoke(index);
            _animeManager.OnSkillEnded += () => OnSkillEnded?.Invoke();
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            _comboEntity?.Tick(delta);
        }

        private void ObserbeCombo()
        {
            _comboEntity.OnChangedCounter += HandleComboChanged;
            _comboEntity.OnComboReset += HandleComboReseted;
        }

        private void HandleComboChanged(int combo)
        {
            float speed = _data.GetComboStackSpeed(combo);
            _animeManager.ChangeSpeed(speed);

            Debug.Log($"スピード{speed}");
        }

        private void HandleComboReseted()
        {
            _animeManager.ChangeSpeed(1);
            Debug.Log($"スピード リセット");
        }
    }
}
