using Cryptos.Runtime.Entity.Ingame.System;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public interface IInGameLoopHandler
    {
        void OnGameStarted();
        void OnWaveChanged(WaveEntity nextWave);
        void OnGameEnded();
    }
}
