using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.UseCase.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.System; // LevelUpgradeOption のため
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    /// <summary>
    ///     ゲームのメインループ（フェーズ遷移、ターン管理など）を管理するユースケースです。
    ///     主要なイベントを発火し、他のUseCaseやPresenterに処理を促します。
    /// </summary>
    public class InGameLoopUseCase
    {
        /// <summary> ゲーム開始時に発火するイベントです。 </summary>
        public event Action OnGameStarted;
        /// <summary> ターン開始時に発火するイベントです。 </summary>
        public event Action OnTurnStarted;
        /// <summary> プレイヤーフェーズ開始時に発火するイベントです。 </summary>
        public event Action OnPlayerPhaseStarted;
        /// <summary> 敵フェーズ開始時に発火するイベントです。 </summary>
        public event Action OnEnemyPhaseStarted;
        /// <summary> ターン終了時に発火するイベントです。 </summary>
        public event Action OnTurnEnded;
        /// <summary> ゲーム終了時に発火するイベントです。 </summary>
        public event Action OnGameEnded;

        /// <summary> カード関連のユースケースへの参照です。 </summary>
        private readonly CardUseCase _cardUseCase;
        /// <summary> レベル管理関連のユースケースへの参照です。 </summary>
        private readonly LevelUseCase _levelUseCase;
        /// <summary> ウェーブ管理関連のユースケースへの参照です。 </summary>
        private readonly WaveUseCase _waveUseCase;
        /// <summary> レベルアップ時にノード選択を行うためのコールバック関数。 </summary>
        private readonly Func<LevelUpgradeOption[], Task<LevelUpgradeOption>> _onLevelUpSelectNodeCallback;

        /// <summary>
        ///     InGameLoopUseCaseの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cardUseCase">カード関連のユースケース。</param>
        /// <param name="levelUseCase">レベル管理関連のユースケース。</param>
        /// <param name="waveUseCase">ウェーブ管理関連のユースケース。</param>
        /// <param name="onLevelUpSelectNodeCallback">レベルアップ時のノード選択コールバック。</param>
        public InGameLoopUseCase(CardUseCase cardUseCase, LevelUseCase levelUseCase, WaveUseCase waveUseCase,
            Func<LevelUpgradeOption[], Task<LevelUpgradeOption>> onLevelUpSelectNodeCallback)
        {
            _cardUseCase = cardUseCase;
            _levelUseCase = levelUseCase;
            _waveUseCase = waveUseCase;
            _onLevelUpSelectNodeCallback = onLevelUpSelectNodeCallback;
        }

        /// <summary>
        ///     ゲーム開始シーケンスを実行します。
        /// </summary>
        public async Task StartGameAsync()
        {
            OnGameStarted?.Invoke();
            Debug.Log("InGameLoopUseCase: ゲーム開始！"); // NOTE: ログ出力はUseCase層ではなく、外部のロギングサービスを介することが望ましいです。

            await InitializeGame();
            await RunGameLoop();

            OnGameEnded?.Invoke();
            Debug.Log("InGameLoopUseCase: ゲーム終了！"); // NOTE: ログ出力はUseCase層ではなく、外部のロギングサービスを介することが望ましいです。
        }

        /// <summary>
        ///     ゲーム開始時の初期化処理を実行します。
        /// </summary>
        private async Task InitializeGame()
        {
            // プレイヤーデータロード、カードデッキ初期化、最初のウェーブ設定などの初期化処理を行います。
            Debug.Log("InGameLoopUseCase: ゲーム初期化完了"); // NOTE: ログ出力はUseCase層ではなく、外部のロギングサービスを介することが望ましいです。
            await Task.Delay(100); // 初期化処理の完了をシミュレートするための待機です。（仮）
        }

        /// <summary>
        ///     メインゲームループを実行します。ゲーム終了条件が満たされるまでフェーズを繰り返し処理します。
        /// </summary>
        private async Task RunGameLoop()
        {
            // ゲーム終了条件が満たされるまでループを継続します。
            while (_waveUseCase.HasNextWave)
            {
                OnTurnStarted?.Invoke();
                Debug.Log($"InGameLoopUseCase: ターン開始！現在のウェーブ: {_waveUseCase.CurrentWaveIndex + 1}");

                // プレイヤーフェーズの処理を実行します。
                OnPlayerPhaseStarted?.Invoke();
                Debug.Log("InGameLoopUseCase: プレイヤーフェーズ開始");
                await HandlePlayerPhase(); // プレイヤーの行動が完了するのを待ちます。
                Debug.Log("InGameLoopUseCase: プレイヤーフェーズ終了");

                // 敵フェーズの処理を実行します。
                OnEnemyPhaseStarted?.Invoke();
                Debug.Log("InGameLoopUseCase: 敵フェーズ開始");
                await HandleEnemyPhase(); // 敵の行動が完了するのを待ちます。
                Debug.Log("InGameLoopUseCase: 敵フェーズ終了");

                OnTurnEnded?.Invoke();
                Debug.Log("InGameLoopUseCase: ターン終了");

                await Task.Delay(1000); // ターン間の処理を待機します。（仮）
            }
        }

        /// <summary>
        /// ウェーブが完了した際に呼び出され、次のウェーブへの遷移とレベルアップ処理を行います。
        /// </summary>
        public async Task HandleWaveCompleted()
        {
            // 次のウェーブへ遷移します。
            Cryptos.Runtime.Entity.Ingame.System.WaveEntity nextWave = _waveUseCase.NextWave();
            if (nextWave == null)
            {
                Debug.Log("InGameLoopUseCase: 全てのウェーブが終了しました。");
                // TODO: ゲーム終了処理を呼び出す
                return;
            }
            Debug.Log($"InGameLoopUseCase: 次のウェーブへ: {nextWave.name}");

            // レベルアップ処理の確認と実行を行います。
            // レベルアップキューに新しいレベルアップが検出された場合、選択処理を実行します。
            while (_levelUseCase.LevelUpQueue.TryDequeue(out var newLevel))
            {
                Debug.Log($"InGameLoopUseCase: レベルアップ！ 新しいレベル: {newLevel}");
                // UIからのノード選択を非同期で待機します。
                LevelUpgradeNode selectedNode = await _levelUseCase.WaitLevelUpSelectAsync(_onLevelUpSelectNodeCallback);
                Debug.Log($"InGameLoopUseCase: 選択されたノード: {selectedNode?.NodeName}");
                // 選択されたノードの効果を適用するUseCaseを呼び出します。
                // TODO: 選択されたノードの効果を適用するUseCaseを実装し、呼び出す
            }
        }

        /// <summary>
        ///     プレイヤーフェーズ内の詳細な処理を実行します。
        ///     プレイヤーからの入力（カード選択、ターゲット選択）を待ち、関連するUseCaseを呼び出します。
        /// </summary>
        private async Task HandlePlayerPhase()
        {
            // プレイヤーからの入力（カード選択、ターゲット選択）を待ち、CardUseCaseなどを呼び出すロジックをここに実装します。（例）
            await Task.Delay(500); // 処理の完了をシミュレートするための待機です。（仮）
        }

        /// <summary>
        ///     敵フェーズ内の詳細な処理を実行します。
        ///     敵AIの行動ロジックを実装し、関連するUseCaseを呼び出します。
        /// </summary>
        private async Task HandleEnemyPhase()
        {
            // 敵AIの行動ロジックを実行し、CardUseCaseなどを呼び出すロジックをここに実装します。（例）
            await Task.Delay(500); // 処理の完了をシミュレートするための待機です。（仮）
        }


    }
}