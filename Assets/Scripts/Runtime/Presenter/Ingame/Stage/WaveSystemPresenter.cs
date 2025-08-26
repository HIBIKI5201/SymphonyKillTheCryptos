using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Presenter.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.System;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public class WaveSystemPresenter
    {
        public WaveSystemPresenter(WaveEntity[] waveEntities, EnemyRepository enemyRepository)
        {
            WaveUseCase waveUseCase = new(waveEntities);
            waveUseCase.OnWaveChanged += WaveEnemysCreate;

            _waveUseCase = waveUseCase;
            _enemyRepository = enemyRepository;
        }

        public void WaveEnemysCreate(WaveEntity waveEntity)
        {
            EnemyData[] enemyData = waveEntity.Enemies;
            foreach (var item in enemyData)
            {
                _enemyRepository.CreateEnemy(item);
            }
        }

        private WaveUseCase _waveUseCase;
        private EnemyRepository _enemyRepository;
    }
}
