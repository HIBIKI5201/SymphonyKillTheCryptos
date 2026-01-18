using Cryptos.Runtime.Entity.Ingame.System;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    /// <summary>
    ///     インゲームのループイベントを処理するインターフェースである。
    /// </summary>
    public interface IInGameLoopWaveHandler
    {
        /// <summary>
        ///     ゲームが開始されたことを通知する。
        /// </summary>
        void OnGameStarted();

        /// <summary>
        ///     Waveが変更されたことを通知する。
        /// </summary>
        /// <param name="nextWave">次のWaveエンティティ。</param>
        void OnWaveChanged(WaveEntity nextWave);

        /// <summary>
        ///     ゲームが終了したことを通知する。
        /// </summary>
        void OnGameEnded();
    }
}
