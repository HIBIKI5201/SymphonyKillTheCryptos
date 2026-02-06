using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.UseCase.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.Character;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    /// <summary>
    ///     ゲームのメインループを管理するユースケースである。
    ///     主要なイベントを発火し、他のUseCaseやPresenterに処理を促す。
    /// </summary>
    public class InGameLoopUseCase
    {
        /// <summary>
        ///     InGameLoopUseCaseの新しいインスタンスを初期化する。
        /// </summary>
        public InGameLoopUseCase(
            CardUseCase cardUseCase,
            LevelUseCase levelUseCase,
            WaveUseCase waveUseCase,
            Func<LevelUpgradeOption[], Task<LevelUpgradeOption>> onLevelUpSelectNodeCallback,
            ISymphonyPresenter symphonyPresenter,
            IInGameLoopWaveHandler inGameLoopWaveHandler,
            ILevelUpPhaseHandler levelUpPhaseHandler,
            Action onGameEndedCallback,
            IDisposable[] disposables)
        {
            _cardUseCase = cardUseCase;
            _levelUseCase = levelUseCase;
            _waveUseCase = waveUseCase;
            _onLevelUpSelectNodeCallback = onLevelUpSelectNodeCallback;
            _symphonyPresenter = symphonyPresenter;
            _inGameLoopWaveHandler = inGameLoopWaveHandler;
            _levelUpPhaseHandler = levelUpPhaseHandler;
            _onGameEndedCallback = onGameEndedCallback;
            _disposables = disposables;
        }

        /// <summary>
        ///     ゲーム開始シーケンスを実行する。
        /// </summary>
        public async ValueTask StartGameAsync()
        {
            Debug.Log("InGameLoopUseCase: ゲーム開始！");

            InitializeGame();
            
            // Fire and forget
            _ = GameLoopAsync();
        }

        private readonly CardUseCase _cardUseCase;
        private readonly LevelUseCase _levelUseCase;
        private readonly WaveUseCase _waveUseCase;
        private readonly Func<LevelUpgradeOption[], Task<LevelUpgradeOption>> _onLevelUpSelectNodeCallback;
        private readonly ISymphonyPresenter _symphonyPresenter;
        private readonly IInGameLoopWaveHandler _inGameLoopWaveHandler;
        private readonly ILevelUpPhaseHandler _levelUpPhaseHandler;
        private readonly Action _onGameEndedCallback;
        private readonly IDisposable[] _disposables;

        /// <summary>
        ///     ゲーム開始時の初期化処理を実行する。
        /// </summary>
        private void InitializeGame()
        {
            // プレイヤーデータロード、カードデッキ初期化、最初のウェーブ設定などの初期化処理を行う。
            Debug.Log("InGameLoopUseCase: ゲーム初期化完了。");
        }
        
        /// <summary>
        /// ゲームのメインループ。ウェーブの完了を待ち、レベルアップ処理を挟んで次のウェーブへ進む。
        /// </summary>
        private async Task GameLoopAsync()
        {
            // 初回ウェーブ処理。
            Task waveCompletionTask = _inGameLoopWaveHandler.OnGameStarted();
            await waveCompletionTask;

            while (true)
            {
                // ウェーブ完了後の処理。
                _symphonyPresenter.ResetUsingCard();
                _levelUseCase.AddLevelProgress(_waveUseCase.CurrentWave);
                
                // レベルアップ処理。
                if (_levelUseCase.LevelUpQueue.Any())
                {
                    Debug.Log("InGameLoopUseCase: レベルアップフェーズ開始"); // 追加
                    _levelUpPhaseHandler.OnLevelUpPhaseStarted();
                    while (_levelUseCase.LevelUpQueue.TryDequeue(out var newLevel))
                    {
                        Debug.Log($"InGameLoopUseCase: レベルアップ処理中: Level {newLevel}"); // 変更
                        await _levelUseCase.HandleLevelUpAsync(_onLevelUpSelectNodeCallback);
                    }
                    _levelUpPhaseHandler.OnLevelUpPhaseEnded();
                    Debug.Log("InGameLoopUseCase: レベルアップフェーズ終了"); // 追加
                }

                // 次のウェーブ。
                WaveEntity nextWave = _waveUseCase.NextWave();
                if (nextWave == null)
                {
                    Debug.Log("InGameLoopUseCase: 全てのウェーブが終了しました。");
                    break; // ループを抜ける。
                }

                Debug.Log($"InGameLoopUseCase: 次のウェーブへ: {nextWave.name}");
                waveCompletionTask = _inGameLoopWaveHandler.OnWaveChanged(nextWave);
                await waveCompletionTask;
            }

            // ゲーム終了処理。
            _inGameLoopWaveHandler.OnGameEnded();
            _onGameEndedCallback?.Invoke();

            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}