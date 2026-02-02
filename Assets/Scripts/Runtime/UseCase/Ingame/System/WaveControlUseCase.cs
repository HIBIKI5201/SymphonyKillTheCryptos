using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.Character.Repository;
using Cryptos.Runtime.Entity.Ingame.System;
using System;
using System.Collections.Generic;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    /// <summary>
    /// ウェーブの進行と敵の管理を行うユースケースである。
    /// </summary>
    public class WaveControlUseCase : IDisposable
    {
        public event Action OnWaveCompleted;

        private readonly EnemyRepository _enemyRepository;
        private WaveStateEntity _currentWaveState;
        
        private readonly List<CharacterEntity> _spawnedEnemies = new();

        public WaveControlUseCase(EnemyRepository enemyRepository)
        {
            _enemyRepository = enemyRepository;
        }
        
        public void Dispose()
        {
            // 全てのイベント購読を解除する。
            foreach (var enemy in _spawnedEnemies)
            {
                enemy.OnDead -= HandleEnemyDead;
            }
            _spawnedEnemies.Clear();
        }

        /// <summary>
        /// 指定されたウェーブデータの敵を生成し、監視を開始する。
        /// </summary>
        public void SpawnEnemies(WaveEntity waveData)
        {
            Dispose(); // 前回の敵のイベント購読を確実に解除する。
            
            _currentWaveState = new WaveStateEntity(waveData.Enemies.Length);

            if (_currentWaveState.IsWaveCompleted)
            {
                // 敵がいないウェーブの場合は即座に完了を通知する。
                OnWaveCompleted?.Invoke();
                return;
            }

            foreach (var enemyData in waveData.Enemies)
            {
                CharacterEntity enemy = _enemyRepository.CreateEnemy(enemyData);
                enemy.OnDead += HandleEnemyDead;
                _spawnedEnemies.Add(enemy);
            }
        }

        private void HandleEnemyDead()
        {
            _currentWaveState.DecrementEnemyCount();

            if (_currentWaveState.IsWaveCompleted)
            {
                OnWaveCompleted?.Invoke();
            }
        }
    }
}
