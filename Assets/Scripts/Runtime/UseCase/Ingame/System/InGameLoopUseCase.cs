using System;
using System.Threading.Tasks;
using UnityEngine; // Debug.Logのために必要ですが、UseCaseのコアロジックからは切り離すべき

// 既存のCardUseCase, LevelUseCase, WaveUseCaseのnamespaceを考慮
using Cryptos.Runtime.UseCase.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.System;
using Cryptos.Runtime.Entity.Ingame.System; // LevelUpgradeNode のために追加

namespace Cryptos.Runtime.UseCase.Ingame
{
    /// <summary>
    /// ゲームのメインループ（フェーズ遷移、ターン管理など）を管理するユースケース。
    /// 主要なイベントを発火し、他のUseCaseやPresenterに処理を促します。
    /// </summary>
    public class InGameLoopUseCase
    {
        // 主要なゲームイベント
        public event Action OnGameStarted;
        public event Action OnTurnStarted;
        public event Action OnPlayerPhaseStarted;
        public event Action OnEnemyPhaseStarted;
        public event Action OnTurnEnded;
        public event Action OnGameEnded;

        // 他の主要なUseCaseへの依存
        private readonly CardUseCase _cardUseCase;
        private readonly LevelUseCase _levelUseCase;
        private readonly WaveUseCase _waveUseCase;
        // 必要に応じて他のUseCase, Repositoryなどを追加

        // コンストラクタによる依存性注入
        public InGameLoopUseCase(CardUseCase cardUseCase, LevelUseCase levelUseCase, WaveUseCase waveUseCase /*, ... */)
        {
            _cardUseCase = cardUseCase;
            _levelUseCase = levelUseCase;
            _waveUseCase = waveUseCase;
        }

        // ゲーム開始シーケンス
        public async Task StartGameAsync()
        {
            OnGameStarted?.Invoke();
            Debug.Log("InGameLoopUseCase: ゲーム開始！");

            await InitializeGame();
            await RunGameLoop();

            OnGameEnded?.Invoke();
            Debug.Log("InGameLoopUseCase: ゲーム終了！");
        }

        // ゲーム初期化処理
        private async Task InitializeGame()
        {
            // 例: プレイヤーデータロード、カードデッキ初期化、最初のウェーブ設定など
            Debug.Log("InGameLoopUseCase: ゲーム初期化完了");
            await Task.Delay(100); // 処理の待機（仮）
        }

        // メインゲームループ
        private async Task RunGameLoop()
        {
            while (true /* ゲーム終了条件 */)
            {
                OnTurnStarted?.Invoke();
                Debug.Log($"InGameLoopUseCase: ターン開始！現在のウェーブ: {_waveUseCase.CurrentWaveIndex + 1}");

                // プレイヤーフェーズ
                OnPlayerPhaseStarted?.Invoke();
                Debug.Log("InGameLoopUseCase: プレイヤーフェーズ開始");
                await HandlePlayerPhase(); // プレイヤーの行動待ちなど
                Debug.Log("InGameLoopUseCase: プレイヤーフェーズ終了");

                // 敵フェーズ
                OnEnemyPhaseStarted?.Invoke();
                Debug.Log("InGameLoopUseCase: 敵フェーズ開始");
                await HandleEnemyPhase(); // 敵の行動ロジック
                Debug.Log("InGameLoopUseCase: 敵フェーズ終了");

                OnTurnEnded?.Invoke();
                Debug.Log("InGameLoopUseCase: ターン終了");

                // レベルアップ処理の確認と実行
                while (_levelUseCase.LevelUpQueue.TryDequeue(out var newLevel))
                {
                    Debug.Log($"InGameLoopUseCase: レベルアップ！ 新しいレベル: {newLevel}");
                    // UIからのノード選択を待つ
                    LevelUpgradeNode selectedNode = await _levelUseCase.WaitLevelUpSelectAsync();
                    Debug.Log($"InGameLoopUseCase: 選択されたノード: {selectedNode?.NodeName}");
                    // 選択されたノードの効果を適用するUseCaseを呼び出す
                }

                // ウェーブ終了判定と次のウェーブへの遷移
                if (CheckWaveCompleted())
                {
                    Cryptos.Runtime.Entity.Ingame.System.WaveEntity nextWave = _waveUseCase.NextWave();
                    if (nextWave == null)
                    {
                        Debug.Log("InGameLoopUseCase: 全てのウェーブが終了しました。");
                        break; // 全ウェーブ終了でゲームループを抜ける
                    }
                    Debug.Log($"InGameLoopUseCase: 次のウェーブへ: {nextWave.name}");
                }

                await Task.Delay(1000); // ターン間の待機（仮）
            }
        }

        // プレイヤーフェーズ内の詳細な処理
        private async Task HandlePlayerPhase()
        {
            // 例: プレイヤーからの入力（カード選択、ターゲット選択）を待ち、CardUseCaseなどを呼び出す
            await Task.Delay(500); // 処理の待機（仮）
        }

        // 敵フェーズ内の詳細な処理
        private async Task HandleEnemyPhase()
        {
            // 例: 敵AIの行動ロジック、CardUseCaseなどを呼び出す
            await Task.Delay(500); // 処理の待機（仮）
        }

        // ウェーブ完了条件のチェック
        private bool CheckWaveCompleted()
        {
            // 例: 現在のウェーブの全ての敵が倒されたかなど
            return true; // 仮の条件
        }
    }
}
