# InGameLoopとWaveシステムの共依存解消リファクタリング計画

## 目的

`InGameLoopUseCase` と `WaveSystemPresenter` が `IWaveHandler` と `IInGameLoopWaveHandler` を介して相互に参照しあっている共依存関係を解消する。
`TaskCompletionSource` を利用して、`InGameLoopUseCase` -> `WaveSystemPresenter` という単方向の依存関係に修正し、コードの見通しとメンテナンス性を向上させる。

## 計画

### ステップ1: IInGameLoopWaveHandler インターフェースの修正

1. `OnGameStarted()` と `OnWaveChanged()` の戻り値を `void` から `Task` に変更する。

### ステップ2: WaveSystemPresenter の改修

1. `IWaveHandler` への依存（フィールド `_waveHandler` と `SetWaveHandler` メソッド）を削除する。
2. `private TaskCompletionSource<bool> _waveCompletionSource;` フィールドを追加する。
3. `OnGameStarted()` と `OnWaveChanged()` メソッドを以下のように修正する。
    a. メソッドの冒頭で `_waveCompletionSource = new TaskCompletionSource<bool>();` を実行する。
    b. 移動演出 `await _wavePath.NextWave(...)` の処理は **待たずに** 実行する (`await` を外す)。BGM再生や敵の生成は即座に行う。
    c. メソッドの戻り値として `_waveCompletionSource.Task` を返す。
4. 敵全滅を検知する `HandleEnemyDead()` メソッド内で、`_waveHandler.OnWaveCompleted()` の呼び出しを `_waveCompletionSource.TrySetResult(true);` に変更する。

### ステップ3: InGameLoopUseCase の改修

1. `IWaveHandler` インターフェースの実装をクラス定義から削除する。
2. `OnWaveCompleted()` メソッドを削除する。
3. ゲームのメインループを管理する新しいプライベートメソッド `private async Task GameLoopAsync()` を実装する。
4. `StartGameAsync()` は `GameLoopAsync()` を呼び出すだけにする。
5. `GameLoopAsync()` の中で、以下の処理を `while` ループで実行する。
    a. `_inGameLoopWaveHandler.OnWaveChanged(currentWave)` を呼び出し、完了通知用の `Task` を受け取る。
    b. `await` でその `Task` の完了（＝敵の全滅）を待つ。
    c. レベルアップ処理を実行する。
    d. `_waveUseCase.NextWave()` で次のウェーブに進む。`null` ならループを抜ける。
6. ループを抜けたらゲーム終了処理 (`OnGameEnded`) を呼び出す。

### ステップ4: DIコンテナ (IngameStartSequence) の修正

1. `InGameLoopUseCase` から `IWaveHandler` の実装が外れるため、`waveSystemPresenter.SetWaveHandler(inGameLoopUseCase);` の行を削除する。

---
## 完了レポート

2026年1月18日、計画書に記載されたすべてのリファクタリング作業を完了した。

### 実施内容

-   `IInGameLoopWaveHandler` の `OnGameStarted` と `OnWaveChanged` の戻り値を `Task` に変更した。
-   `WaveSystemPresenter` から `IWaveHandler` への依存を削除し、代わりに `TaskCompletionSource` を用いてウェーブ完了を通知するように修正した。
-   `InGameLoopUseCase` から `IWaveHandler` の実装を削除し、`OnWaveCompleted` メソッドを廃止した。代わりに `GameLoopAsync` メソッドを新設し、`WaveSystemPresenter` が返す `Task` を `await` することでウェーブ完了を待機し、次の処理へ進むようにロジックを再構築した。
-   `IngameStartSequence` から、不要になった `SetWaveHandler` の呼び出しを削除した。

### 結果

これにより、`InGameLoopUseCase` と `WaveSystemPresenter` の間の相互依存関係が解消され、`InGameLoopUseCase` -> `WaveSystemPresenter` の単方向の依存関係となった。コードの責務がより明確になり、見通しが向上した。