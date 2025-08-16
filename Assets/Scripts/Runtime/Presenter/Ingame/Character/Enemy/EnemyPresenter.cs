using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Presenter.Character.Player;
using Cryptos.Runtime.Presenter.Ingame.Character;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Cryptos.Runtime.Presenter.Character.Enemy
{
    public class EnemyPresenter : MonoBehaviour
    {
        public void Init(EnemyRepository repository, SymphonyPresenter player)
        {
            _enemyRepository = repository;
            repository.OnEnemyCreated += HandleEnemyCreated;

            destroyCancellationToken.Register(() =>
            {
                if (repository == null) return;
                repository.OnEnemyCreated -= HandleEnemyCreated;
            });

            _player = player;

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
        private SymphonyPresenter _player;

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

        private void HandleEnemyCreated(CharacterEntity<EnemyData> enemy)
        {
            if (enemy == null) return;
            // ここで敵キャラクターの表示や初期化を行う

            CreateEnemyModel(enemy);
        }

        private void CreateEnemyModel(CharacterEntity<EnemyData> enemy)
        {
            Debug.Log($"Creating enemy model for: {enemy}");

            if (_enemyModel == null)
            {
                Debug.LogError("EnemyModelPresenter is not assigned.");
                return;
            }

            if (!_spawnPointStack.TryPop(out Transform spawnPoint)) return;

            EnemyModelPresenter model = Instantiate(_enemyModel, spawnPoint.position, spawnPoint.rotation, _enemyContainer);
            model.Init(_player.transform);

            enemy.OnDead += model.Dead;
        }
    }
}
