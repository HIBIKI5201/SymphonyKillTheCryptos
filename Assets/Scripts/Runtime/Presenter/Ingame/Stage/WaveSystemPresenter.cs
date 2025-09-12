using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Presenter.Ingame.Character.Player;
using Cryptos.Runtime.Presenter.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.System;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public class WaveSystemPresenter
    {
        public WaveSystemPresenter(WaveEntity[] waveEntities,
            SymphonyPresenter player, EnemyRepository enemyRepository,
            LevelUseCase levelUseCase, TentativeCharacterData symphonyData)
        {
            WaveUseCase waveUseCase = new(waveEntities);

            _waveUseCase = waveUseCase;
            _symphony = player;
            _enemyRepository = enemyRepository;
            _levelUseCase = levelUseCase;
            _symphonyData = symphonyData;
        }

        /// <summary> ウェーブ開始時 </summary>
        public event Action OnWaveStarted;
        /// <summary> ウェーブクリア時 </summary>
        public event Action OnWaveCleared;

        /// <summary> ウェーブ完遂時 </summary>
        public event Action OnAllWaveEnded;

        public void GameStart()
        {
            CreateWaveEnemys(_waveUseCase.CurrentWave);
            OnWaveStarted?.Invoke();
        }

        private WaveUseCase _waveUseCase;
        private SymphonyPresenter _symphony;
        private EnemyRepository _enemyRepository;
        private LevelUseCase _levelUseCase;

        private TentativeCharacterData _symphonyData;

        private int _enemyCount = 0;

        /// <summary>
        ///     敵が倒されたときの処理。
        /// </summary>
        private void HandleEnemyDead()
        {
            _enemyCount--;
            if (_enemyCount <= 0)
            {
                WaveEntity wave = _waveUseCase.NextWave(); // 全ての敵を倒したら次のウェーブへ。

                if (wave == null) //次のウェーブが無くなったら終了処理を発火
                {
                    OnAllWaveEnded?.Invoke();
                    return;
                }

                ChangeWave(wave);
            }
        }

        /// <summary>
        ///     ウェーブが変更されたときの処理。
        /// </summary>
        /// <param name="newWave"></param>
        private async void ChangeWave(WaveEntity newWave)
        {
            OnWaveCleared?.Invoke();

            _levelUseCase.AddLevelProgress(newWave);

            // レベルアップキューに溜まっている分を全て処理。
            while (_levelUseCase.LevelUpQueue.TryDequeue(out _))
            {
                LevelUpgradeNode upgradeNode = await _levelUseCase.WaitLevelUpSelectAsync();
                
                foreach (ILevelUpgradeEffect effect in upgradeNode.Effects)
                {
                    if (effect is LevelUpgradeStatusEffect statusEffect)
                    {
                        statusEffect.ApplyStatusEffect(_symphonyData);
                    }
                }

                Debug.Log($"Level Up! Selected Card: {upgradeNode}");
            }

            await _symphony.NextWave(_waveUseCase.CurrentWaveIndex);
            CreateWaveEnemys(newWave);

            OnWaveStarted?.Invoke();
        }

        private void CreateWaveEnemys(WaveEntity waveEntity)
        {
            CharacterData[] enemyData = waveEntity.Enemies;
            _enemyCount = enemyData.Length;

            foreach (var item in enemyData)
            {
                CharacterEntity enemy = _enemyRepository.CreateEnemy(item);
                enemy.OnDead += HandleEnemyDead;
            }
        }
    }
}
