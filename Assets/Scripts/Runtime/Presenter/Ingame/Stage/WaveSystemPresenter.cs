using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Presenter.Character.Player;
using Cryptos.Runtime.Presenter.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.System;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public class WaveSystemPresenter
    {
        public WaveSystemPresenter(WaveEntity[] waveEntities,
            SymphonyPresenter player, EnemyRepository enemyRepository,
            LevelUseCase levelUseCase)
        {
            WaveUseCase waveUseCase = new(waveEntities);
            waveUseCase.OnWaveChanged += HandleWaveChanged;

            _waveUseCase = waveUseCase;
            _symphony = player;
            _enemyRepository = enemyRepository;
            _levelUseCase = levelUseCase;
        }

        public void GameStart()
        {
            WaveEnemysCreate(_waveUseCase.CurrentWave);
        }

        private WaveUseCase _waveUseCase;
        private SymphonyPresenter _symphony;
        private EnemyRepository _enemyRepository;
        private LevelUseCase _levelUseCase;

        private int _enemyCount = 0;

        /// <summary>
        ///     ウェーブが変更されたときの処理。
        /// </summary>
        /// <param name="newWave"></param>
        private async void HandleWaveChanged(WaveEntity newWave)
        {
            await _symphony.NextWave(_waveUseCase.CurrentWaveIndex);
            WaveEnemysCreate(newWave);

            bool isLevelUp = _levelUseCase.AddLevelProgress(newWave);
        }

        /// <summary>
        ///     敵が倒されたときの処理。
        /// </summary>
        private void HandleEnemyDead()
        {
            _enemyCount--;
            if (_enemyCount <= 0)
            {
                _waveUseCase.NextWave(); // 全ての敵を倒したら次のウェーブへ。
            }
        }

        private void WaveEnemysCreate(WaveEntity waveEntity)
        {
            EnemyData[] enemyData = waveEntity.Enemies;
            _enemyCount = enemyData.Length;

            foreach (var item in enemyData)
            {
                CharacterEntity<EnemyData> enemy = _enemyRepository.CreateEnemy(item);
                enemy.OnDead += HandleEnemyDead;
            }
        }
    }
}
