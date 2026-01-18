using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.UseCase.Ingame.Card;
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
    public class InGameLoopUseCase : IWaveHandler
    {
        private readonly CardUseCase _cardUseCase;
        private readonly LevelUseCase _levelUseCase;
        private readonly WaveUseCase _waveUseCase;
        private readonly Func<LevelUpgradeOption[], Task<LevelUpgradeOption>> _onLevelUpSelectNodeCallback;

        private readonly IInGameLoopWaveHandler _inGameLoopWaveHandler;
        private readonly ILevelUpPhaseHandler _levelUpPhaseHandler;
        private readonly Action _onGameEndedCallback;

        /// <summary>
        ///     InGameLoopUseCaseの新しいインスタンスを初期化する。
        /// </summary>
        public InGameLoopUseCase(
            CardUseCase cardUseCase,
            LevelUseCase levelUseCase,
            WaveUseCase waveUseCase,
            Func<LevelUpgradeOption[], Task<LevelUpgradeOption>> onLevelUpSelectNodeCallback,
            IInGameLoopWaveHandler inGameLoopWaveHandler,
            ILevelUpPhaseHandler levelUpPhaseHandler,
            Action onGameEndedCallback)
        {
            _cardUseCase = cardUseCase;
            _levelUseCase = levelUseCase;
            _waveUseCase = waveUseCase;
            _onLevelUpSelectNodeCallback = onLevelUpSelectNodeCallback;
            _inGameLoopWaveHandler = inGameLoopWaveHandler;
            _levelUpPhaseHandler = levelUpPhaseHandler;
            _onGameEndedCallback = onGameEndedCallback;
        }

        /// <summary>
        ///     ゲーム開始シーケンスを実行する。
        /// </summary>
        public async ValueTask StartGameAsync()
        {
            _inGameLoopWaveHandler.OnGameStarted();
            Debug.Log("InGameLoopUseCase: ゲーム開始！");

            await InitializeGame();
        }

        /// <summary>
        ///     ゲーム開始時の初期化処理を実行する。
        /// </summary>
        private async ValueTask InitializeGame()
        {
            // プレイヤーデータロード、カードデッキ初期化、最初のウェーブ設定などの初期化処理を行う。
            Debug.Log("InGameLoopUseCase: ゲーム初期化完了。");
        }

        /// <summary>
        ///     ウェーブが完了した際に呼び出され、次のウェーブへの遷移とレベルアップ処理を行う。
        /// </summary>
        public async Task OnWaveCompleted()
        {
            // 経験値を追加する。
            _levelUseCase.AddLevelProgress(_waveUseCase.CurrentWave);

            // 次のウェーブへ遷移する。
            WaveEntity nextWave = _waveUseCase.NextWave();

            // レベルアップ処理の確認と実行を行う。
            if (_levelUseCase.LevelUpQueue.Any())
            {
                _levelUpPhaseHandler.OnLevelUpPhaseStarted();
                
                while (_levelUseCase.LevelUpQueue.TryDequeue(out var newLevel))
                {
                    Debug.Log($"InGameLoopUseCase: レベルアップ！ 新しいレベル: {newLevel}");
                    // レベルアップ処理（UI表示、ノード選択、効果適用）をLevelUseCaseに委譲する。
                    await _levelUseCase.HandleLevelUpAsync(_onLevelUpSelectNodeCallback);
                }
                
                _levelUpPhaseHandler.OnLevelUpPhaseEnded();
            }

            if (nextWave == null)
            {
                Debug.Log("InGameLoopUseCase: 全てのウェーブが終了しました。");
                _inGameLoopWaveHandler.OnGameEnded();
                _onGameEndedCallback?.Invoke();
                return;
            }

            Debug.Log($"InGameLoopUseCase: 次のウェーブへ: {nextWave.name}");
            _inGameLoopWaveHandler.OnWaveChanged(nextWave);
        }
    }
}