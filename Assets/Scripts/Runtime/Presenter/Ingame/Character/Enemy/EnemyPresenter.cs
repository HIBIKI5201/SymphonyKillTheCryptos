using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Presenter.Ingame.Character.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Character.Enemy
{
    /// <summary>
    ///     敵キャラクターモデルの生成と管理を行うクラスです。
    /// </summary>
    public class EnemyPresenter : MonoBehaviour
    {
        public Action<CharacterEntity, EnemyModelPresenter> OnCreatedEnemyModel;

        public void Init(EnemyRepository repository, SymphonyPresenter playerModel)
        {
            _enemyRepository = repository;
            repository.OnEnemyCreated += HandleEnemyCreated;

            destroyCancellationToken.Register(() =>
            {
                if (repository == null) return;
                repository.OnEnemyCreated -= HandleEnemyCreated;
            });

            _playerModel = playerModel;

            foreach (var enemy in repository.AllEnemies)
            {
                CreateEnemyModel(enemy);
            }
        }

        [SerializeField]
        private EnemyModelPresenter _enemyModel;

        [Space]
        [SerializeField]
        private Transform[] _spawnPoints;
        [SerializeField]
        private Transform _enemyContainer;

        private EnemyRepository _enemyRepository;
        private SymphonyPresenter _playerModel;

        private Stack<Transform> _spawnPointStack;

        private void Awake()
        {
            if (_enemyContainer == null)
            {
                _enemyContainer = transform;
            }

            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                Debug.LogError("Spawn points are not assigned or empty.");
                return;
            }

            _spawnPointStack = new(_spawnPoints);
        }

        private void HandleEnemyCreated(CharacterEntity enemy)
        {
            if (enemy == null) return;
            // ここで敵キャラクターの表示や初期化を行う

            CreateEnemyModel(enemy);
        }

        private void CreateEnemyModel(CharacterEntity enemy)
        {
            Debug.Log($"Creating enemy model for: {enemy}");

            if (_enemyModel == null)
            {
                Debug.LogError("EnemyModelPresenter is not assigned.");
                return;
            }

            if (!_spawnPointStack.TryPop(out Transform spawnPoint)) return;

            enemy.OnDead += () => _spawnPointStack.Push(spawnPoint);

            EnemyModelPresenter model = Instantiate(_enemyModel,
                spawnPoint.position, spawnPoint.rotation,
                _enemyContainer);
            model.Init(enemy, _playerModel);

            OnCreatedEnemyModel?.Invoke(enemy, model);
        }

        private void OnDrawGizmos()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0) return;
            Gizmos.color = Color.red;

            Vector3 size = new Vector3(1, 2, 1);
            foreach (var spawnPoint in _spawnPoints)
            {
                Gizmos.DrawWireCube(spawnPoint.position + Vector3.up * size.y / 2, size);
            }
        }
    }
}
