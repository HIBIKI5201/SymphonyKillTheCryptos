using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Presenter.Ingame.Character;
using Cryptos.Runtime.Presenter.Ingame.Character.Player;
using Cryptos.Runtime.Presenter.System.Audio;
using Cryptos.Runtime.UseCase.Ingame.System;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    /// <summary>
    ///     ウェーブの進行を管理するクラス。
    /// </summary>
    public class WaveSystemPresenter
    {
        public WaveSystemPresenter(WaveEntity[] waveEntities,
            WavePathPresenter wavePathPresenter,
            SymphonyPresenter player, EnemyRepository enemyRepository,
            LevelUseCase levelUseCase, TentativeCharacterData symphonyData,
            IBGMPlayer bgmPlayer)
        {
            WaveUseCase waveUseCase = new(waveEntities);
            _waveUseCase = waveUseCase;
            _wavePath = wavePathPresenter;
            _symphony = player;
            _enemyRepository = enemyRepository;
            _levelUseCase = levelUseCase;
            _symphonyData = symphonyData;
            _bgmPlayer = bgmPlayer;
        }

        /// <summary> ウェーブ開始時 </summary>
        public event Action OnWaveStarted;
        /// <summary> ウェーブクリア時 </summary>
        public event Action OnWaveCleared;

        /// <summary> ウェーブ完遂時 </summary>
        public event Action OnAllWaveEnded;

        /// <summary>
        ///     ゲームを開始する。
        /// </summary>
        public async void GameStart()
        {
            await _wavePath.NextWave(_waveUseCase.CurrentWaveIndex); // 最初のウェーブ位置へ移動。

            // 最初のウェーブの敵を生成。
            WaveEntity nextWave = _waveUseCase.CurrentWave;
            CreateWaveEnemies(nextWave);
            _bgmPlayer.PlayBGM(nextWave.BGMCueName);

            OnWaveStarted?.Invoke();
        }

        private WaveUseCase _waveUseCase;
        private WavePathPresenter _wavePath;
        private SymphonyPresenter _symphony;
        private EnemyRepository _enemyRepository;
        private LevelUseCase _levelUseCase;
        private IBGMPlayer _bgmPlayer;

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
        /// <param name="nextWave"></param>
        private async void ChangeWave(WaveEntity nextWave)
        {
            OnWaveCleared?.Invoke();

            _levelUseCase.AddLevelProgress(nextWave);
            _symphony.ResetUsingCard();

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

            await _wavePath.NextWave(_waveUseCase.CurrentWaveIndex);
            CreateWaveEnemies(nextWave);

            _bgmPlayer.PlayBGM(nextWave.BGMCueName);

            OnWaveStarted?.Invoke();
        }

        private void CreateWaveEnemies(WaveEntity waveEntity)
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
