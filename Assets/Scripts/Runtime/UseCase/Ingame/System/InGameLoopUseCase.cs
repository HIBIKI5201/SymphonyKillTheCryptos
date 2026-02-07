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
        private bool _isGameEnded; // ゲーム終了状態を管理するフラグ

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
            Action<string, int> onGameEndedCallback,
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

            // プレイヤーの死亡イベントを購読
            _symphonyPresenter.OnDead += HandlePlayerDead;

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
        private readonly Action<string, int> _onGameEndedCallback;
        private readonly IDisposable[] _disposables;

        /// <summary>
        ///     ゲーム開始時の初期化処理を実行する。
        /// </summary>
        private void InitializeGame()
        {
            // プレイヤーデータロード、カードデッキ初期化、最初のウェーブ設定などの初期化処理を行う。
            Debug.Log("InGameLoopUseCase: ゲーム初期化完了。");
            _isGameEnded = false;
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
                if (_isGameEnded) break; // プレイヤー死亡でゲーム終了した場合、ループを抜ける

                // ウェーブ完了後の処理。
                _symphonyPresenter.ResetUsingCard();
                _levelUseCase.AddLevelProgress(_waveUseCase.CurrentWave);

                // レベルアップ処理。
                if (_levelUseCase.LevelUpQueue.Any())
                {
                    _levelUpPhaseHandler.OnLevelUpPhaseStarted();
                    while (_levelUseCase.LevelUpQueue.TryDequeue(out var newLevel))
                    {
                        Debug.Log($"InGameLoopUseCase: レベルアップ！ 新しいレベル: {newLevel}");
                        await _levelUseCase.HandleLevelUpAsync(_onLevelUpSelectNodeCallback);
                    }
                    _levelUpPhaseHandler.OnLevelUpPhaseEnded();
                }

                // 次のウェーブ。
                WaveEntity nextWave = _waveUseCase.NextWave();
                if (nextWave == null)
                {
                    Debug.Log("InGameLoopUseCase: 全てのウェーブが終了しました。");
                    EndGame("Game Clear!", _levelUseCase.CurrentLevel);
                    break; // ループを抜ける。
                }

                Debug.Log($"InGameLoopUseCase: 次のウェーブへ: {nextWave.name}");
                waveCompletionTask = _inGameLoopWaveHandler.OnWaveChanged(nextWave);
                await waveCompletionTask;
            }

            // ゲーム終了時のクリーンアップ
            CleanupGame();
        }

        /// <summary>
        /// プレイヤー死亡時の処理
        /// </summary>
        private void HandlePlayerDead()
        {
            if (_isGameEnded) return; // 既にゲーム終了状態の場合、重複して処理しない
            Debug.Log("InGameLoopUseCase: プレイヤーが死亡しました。");
            EndGame("Game Over!", _levelUseCase.CurrentLevel); // ゲームオーバーとして終了
        }

        /// <summary>
        /// ゲーム終了処理をまとめたメソッド
        /// </summary>
        /// <param name="resultTitle"></param>
        /// <param name="score"></param>
        private void EndGame(string resultTitle, int score)
        {
            if (_isGameEnded) return;
            _isGameEnded = true;

            _inGameLoopWaveHandler.OnGameEnded();
            _onGameEndedCallback?.Invoke(resultTitle, score);
        }

        /// <summary>
        /// ゲーム終了時のクリーンアップ処理
        /// </summary>
        private void CleanupGame()
        {
            // プレイヤーの死亡イベント購読解除
            _symphonyPresenter.OnDead -= HandlePlayerDead;

            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}