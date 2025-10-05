using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Presenter.Ingame.Character.Player;
using Unity.Behavior;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Character.Enemy
{
    /// <summary>
    ///   敵キャラクターのモデルを操作するクラスです。
    /// </summary>
    public class EnemyModelPresenter : MonoBehaviour
    {
        public CharacterEntity Self => _self;
        public void Init(CharacterEntity self, SymphonyPresenter target,
            CombatPipelineWrapper combatPipeline)
        {
            if (self == null)
            {
                Debug.LogError("self entity is null");
                return;
            }

            if (target == null)
            {
                Debug.LogError("Target transform is not assigned.");
                return;
            }

            _self = self;
            _target = target.transform;

            self.OnDead += Dead;
            self.OnTakedDamage += HandleTakeDamage;

            destroyCancellationToken.Register(() =>
            {
                if (_self == null) return;
                _self.OnDead -= Dead;
                _self.OnTakedDamage -= HandleTakeDamage;
            });

            if (TryGetComponent<BehaviorGraphAgent>(out var agent))
            {
                BlackboardReference blackboard = agent.BlackboardReference;
                blackboard.SetVariableValue(_behavorSelfParameter, this);
                blackboard.SetVariableValue(_behaviorTargetParameter, target);
                blackboard.SetVariableValue(_behaviorPipelineParameter, combatPipeline);
            }
        }

        public void Dead()
        {
            Destroy(gameObject);
        }

        [SerializeField]
        private string _behavorSelfParameter = "SelfCharacter";
        [SerializeField]
        private string _behaviorTargetParameter = "TargetCharacter";
        [SerializeField]
        private string _behaviorPipelineParameter = "CombatPipeline";

        private CharacterEntity _self;
        private Transform _target;
        private IEnemyAnimeManager _animeManager;

        private void Awake()
        {
            _animeManager = GetComponentInChildren<IEnemyAnimeManager>();
            if (_animeManager == null)
            {
                Debug.LogError("SymphonyAnimeManager is not found on the GameObject.");
            }
        }

        private void Update()
        {
            if (_target == null) return;

            transform.rotation = Quaternion.LookRotation(_target.position - transform.position);
        }

        private void HandleTakeDamage(CombatContext cc)
        {
            _animeManager.Hit();
        }
    }
}
