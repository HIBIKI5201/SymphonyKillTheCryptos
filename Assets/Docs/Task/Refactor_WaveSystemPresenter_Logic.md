# WaveSystemPresenter ロジック分離リファクタリング計画

## 目的

`WaveSystemPresenter` が現在保持している、敵の数や状態の管理、敵の生成といったロジックとデータを、それぞれ `Entity` 層と `UseCase` 層に分離する。
`WaveSystemPresenter` は、`UseCase` を呼び出し、`View` に関連する処理（BGM再生、移動演出の開始など）に責務を集中させることで、クリーンアーキテクチャの原則を強化し、コードの凝集度と保守性を向上させる。

## 計画

### ステップ1: WaveStateEntity の作成 (Entity層)

1.  `Assets/Scripts/Runtime/Entity/Ingame/System/` に `WaveStateEntity.cs` を新規作成する。
2.  このクラスに、現在のウェーブで生存している敵の数を管理するための `public int AliveEnemyCount { get; private set; }` プロパティを持たせる。
3.  コンストラクタで敵の総数を受け取り、`AliveEnemyCount` を初期化する。
4.  敵が倒された際に呼ばれる `public void DecrementEnemyCount()` メソッドを実装する。このメソッド内で `AliveEnemyCount` をデクリメントする。
5.  `public bool IsWaveCompleted => AliveEnemyCount <= 0;` というプロパティを実装し、ウェーブ完了を判定できるようにする。

### ステップ2: WaveControlUseCase の作成 (UseCase層)

1.  `Assets/Scripts/Runtime/UseCase/Ingame/System/` に `WaveControlUseCase.cs` を新規作成する。
2.  このクラスは、`EnemyRepository` に依存するため、コンストラクタで受け取る。
3.  現在のウェーブ状態を保持するための `private WaveStateEntity _currentWaveState;` フィールドを持つ。
4.  敵を生成する `public void SpawnEnemies(WaveEntity waveData)` メソッドを実装する。
    a. メソッド内で `_currentWaveState = new WaveStateEntity(waveData.Enemies.Length);` のように `WaveStateEntity` を初期化する。
    b. `EnemyRepository` を使って敵を生成するループ処理を、`WaveSystemPresenter` の `CreateWaveEnemies` からこのメソッドに移譲する。
    c. 生成した各 `EnemyEntity` の `OnDead` イベントを購読し、イベントハンドラ `HandleEnemyDead` を紐付ける。
5.  敵の死亡イベントを処理する `private void HandleEnemyDead(CharacterEntity enemy)` メソッドを実装する。
    a. `_currentWaveState.DecrementEnemyCount()` を呼び出す。
    b. `_currentWaveState.IsWaveCompleted` が `true` になったら、ウェーブ完了を通知する。
6.  ウェーブ完了を外部に通知するために、`public event Action OnWaveCompleted;` を定義し、`HandleEnemyDead` の中で `IsWaveCompleted` が true になったら `OnWaveCompleted?.Invoke();` を呼び出す。
7.  生成した敵の `OnDead` イベントへの参照を保持し、次のウェーブが始まる際に購読解除するロジックを追加する（メモリリーク防止）。

### ステップ3: WaveSystemPresenter の改修 (Presenter層)

1.  フィールド `_enemyCount` と、メソッド `HandleEnemyDead`, `CreateWaveEnemies` を削除する。
2.  コンストラクタで `WaveControlUseCase` を受け取るように変更する。
3.  `OnGameStarted` と `OnWaveChanged` の中で、`CreateWaveEnemies` を呼び出していた箇所を `_waveControlUseCase.SpawnEnemies(nextWave);` の呼び出しに置き換える。
4.  `TaskCompletionSource` の完了通知 (`TrySetResult`) は、`WaveControlUseCase` の `OnWaveCompleted` イベントを購読し、そのイベントハンドラ内で行うように変更する。
    a. `WaveSystemPresenter` のコンストラクタまたは新しい `Init` メソッドで `_waveControlUseCase.OnWaveCompleted += HandleWaveCompleted;` のように購読する。
    b. `private void HandleWaveCompleted()` メソッドを新設し、その中で `_waveCompletionSource.TrySetResult(true);` を実行する。

### ステップ4: DIコンテナ (IngameStartSequence) の修正

1.  `GameInitialize` メソッド内で、`WaveControlUseCase` をインスタンス化する。`EnemyRepository` を渡す必要がある。
2.  `WaveSystemPresenter` のコンストラクタに、生成した `WaveControlUseCase` のインスタンスを渡すように修正する。

---
## 完了レポート

2026年1月18日、計画書に記載されたすべてのリファクタリング作業を完了した。

### 実施内容

-   **Entity層:** `WaveStateEntity` を新設し、ウェーブの敵数と完了状態を管理するようにした。
-   **UseCase層:** `WaveControlUseCase` を新設し、敵の生成ロジックと死亡監視ロジックを `WaveSystemPresenter` から移譲した。ウェーブ完了は `OnWaveCompleted` イベントで通知する。
-   **Presenter層:** `WaveSystemPresenter` から敵管理ロジックを削除し、新設した `WaveControlUseCase` を利用するように修正。`UseCase` からのイベントを受けて `TaskCompletionSource` を完了させるように変更した。
-   **InfraStructure層:** `IngameStartSequence` で新しい `WaveControlUseCase` をインスタンス化し、`WaveSystemPresenter` に注入するように修正した。

### 結果

`WaveSystemPresenter` が保持していたロジックとデータが `UseCase` 層と `Entity` 層に適切に分離された。`Presenter` は状態を持たず、`UseCase` の呼び出しと `View` への責務がより明確になった。これにより、クリーンアーキテクチャの原則が強化され、コード全体の保守性とテスト容易性が向上した。