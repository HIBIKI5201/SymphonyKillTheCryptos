using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Presenter.Ingame.Character.Player;
using Cryptos.Runtime.Presenter.System.Audio;
using Cryptos.Runtime.UseCase.Ingame.System;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    /// <summary>
    ///     ウェーブの進行を管理するクラスである。
    /// </summary>
    public class WaveSystemPresenter : IInGameLoopWaveHandler
    {
        private readonly WaveUseCase _waveUseCase;
        private readonly WavePathPresenter _wavePath;
        private readonly SymphonyPresenter _symphony;
        private readonly IBGMPlayer _bgmPlayer;
        private readonly IWaveStateReceiver _waveStateReceiver;
        private readonly WaveControlUseCase _waveControlUseCase;

        private TaskCompletionSource<bool> _waveCompletionSource;

        /// <summary>
        ///     WaveSystemPresenterの新しいインスタンスを初期化する。
        /// </summary>
        public WaveSystemPresenter(
            WaveUseCase waveUseCase,
            WavePathPresenter wavePathPresenter,
            SymphonyPresenter player,
            IBGMPlayer bgmPlayer,
            IWaveStateReceiver waveStateReceiver,
            WaveControlUseCase waveControlUseCase)
        {
            _waveUseCase = waveUseCase;
            _wavePath = wavePathPresenter;
            _symphony = player;
            _bgmPlayer = bgmPlayer;
            _waveStateReceiver = waveStateReceiver;
            _waveControlUseCase = waveControlUseCase;
            
            _waveControlUseCase.OnWaveCompleted += HandleWaveCompleted;
        }

        /// <summary>
        ///     ゲーム開始時に最初のウェーブを開始する。
        /// </summary>
        public Task OnGameStarted()
        {
            _waveCompletionSource = new TaskCompletionSource<bool>();

            WaveEntity nextWave = _waveUseCase.CurrentWave;
            _bgmPlayer.PlayBGM(nextWave.BGMCueName);

            _wavePath.NextWave(_waveUseCase.CurrentWaveIndex);

            _waveControlUseCase.SpawnEnemies(nextWave);

            _waveStateReceiver.OnWaveStarted();

            return _waveCompletionSource.Task;
        }

        /// <summary>
        ///     次のウェーブに移行する。
        /// </summary>
        /// <param name="nextWave">次のWaveエンティティ。</param>
        public Task OnWaveChanged(WaveEntity nextWave)
        {
            _waveCompletionSource = new TaskCompletionSource<bool>();

            _waveStateReceiver.OnWaveCleared();

            _symphony.ResetUsingCard();

            _wavePath.NextWave(_waveUseCase.CurrentWaveIndex);

            _waveControlUseCase.SpawnEnemies(nextWave);

            _bgmPlayer.PlayBGM(nextWave.BGMCueName);

            _waveStateReceiver.OnWaveStarted();

            return _waveCompletionSource.Task;
        }

        /// <summary>
        ///     ゲーム終了時の処理を実行する。
        /// </summary>
        public void OnGameEnded()
        {
            _waveControlUseCase.Dispose();
            _waveControlUseCase.OnWaveCompleted -= HandleWaveCompleted;
            
            _waveStateReceiver.OnWaveCleared();
        }

        private void HandleWaveCompleted()
        {
            _waveCompletionSource.TrySetResult(true);
        }
    }
}
