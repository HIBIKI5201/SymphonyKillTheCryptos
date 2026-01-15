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
        /// <summary> 新しいウェーブに変わった時に発火するイベントです。 </summary>
        public event Action<WaveEntity> OnWaveChanged;
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
            Debug.Log("InGameLoopUseCase: ゲーム開始！"); 

            await InitializeGame();
        }

        /// <summary>
        ///     ゲーム開始時の初期化処理を実行します。
        /// </summary>
        private async Task InitializeGame()
        {
            // プレイヤーデータロード、カードデッキ初期化、最初のウェーブ設定などの初期化処理を行います。
            Debug.Log("InGameLoopUseCase: ゲーム初期化完了");
            await Task.Delay(100); // 初期化処理の完了をシミュレートするための待機です。（仮）
        }

        /// <summary>
        /// ウェーブが完了した際に呼び出され、次のウェーブへの遷移とレベルアップ処理を行います。
        /// </summary>
        public async Task HandleWaveCompleted()
        {
             // 経験値を追加
            _levelUseCase.AddLevelProgress(_waveUseCase.CurrentWave);

            // 次のウェーブへ遷移します。
            WaveEntity nextWave = _waveUseCase.NextWave();

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

            if (nextWave == null)
            {
                Debug.Log("InGameLoopUseCase: 全てのウェーブが終了しました。");
                OnGameEnded?.Invoke();
                return;
            }
            
            Debug.Log($"InGameLoopUseCase: 次のウェーブへ: {nextWave.name}");
            OnWaveChanged?.Invoke(nextWave);
        }
}
}