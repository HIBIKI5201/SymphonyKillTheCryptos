namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public interface IWaveStateReceiver
    {
        void OnWaveStarted();
        void OnWaveCleared();
    }
}
