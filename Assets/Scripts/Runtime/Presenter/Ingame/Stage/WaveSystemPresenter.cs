using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Presenter.Character.Player;
using Cryptos.Runtime.Presenter.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.System;
using System.Threading.Tasks;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public class WaveSystemPresenter
    {
        public WaveSystemPresenter(WaveEntity[] waveEntities, SymphonyPresenter player,  EnemyRepository enemyRepository)
        {
            WaveUseCase waveUseCase = new(waveEntities);
            waveUseCase.OnWaveChanged += HandleWaveChanged;

            _waveUseCase = waveUseCase;
            _symphony = player;
            _enemyRepository = enemyRepository;
        }

        public void GameStart()
        {
            WaveEnemysCreate(_waveUseCase.CurrentWave);
        }

        private async void HandleWaveChanged(WaveEntity newWave)
        {
            await _symphony.NextWave(_waveUseCase.CurrentWaveIndex);
            WaveEnemysCreate(newWave);
        }

        private void HandleEnemyDead()
        {
            _enemyCount--;
            if (_enemyCount <= 0)
            {
                _waveUseCase.NextWave();
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

        private WaveUseCase _waveUseCase;
        private SymphonyPresenter _symphony;
        private EnemyRepository _enemyRepository;

        private int _enemyCount = 0;
    }
}
