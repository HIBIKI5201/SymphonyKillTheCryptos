using Cryptos.Runtime.Entity.Ingame.System;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public interface IInGameLoopWaveHandler
    {
        void OnGameStarted();
        void OnWaveChanged(WaveEntity nextWave);
        void OnGameEnded();
    }
}
