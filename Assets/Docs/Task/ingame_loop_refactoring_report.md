# インゲームループの再構築と責務の明確化レポート

## 1. タスクの目的

本タスクは、`Assets/Scripts/Runtime/UseCase/Ingame/System/InGameLoopUseCase.cs` を中心としたインゲームループのアーキテクチャを、プロジェクトのクリーンアーキテクチャ原則に沿って完成させ、各コンポーネントの責務を明確化することを目的としました。

## 2. 検出された課題

コードベース分析において、以下の課題が検出されました。

*   **UseCase層とPresenter層間の責務の重複**:
    *   `InGameLoopUseCase` (UseCase層) と `WaveSystemPresenter` (Presenter層) の両方が、ウェーブ遷移ロジックの一部を管理していました。
    *   特に `WaveSystemPresenter` が、Presenter層であるにも関わらず、`WaveUseCase.NextWave()` を呼び出すなど、状態遷移の責務を担っていました。
*   **UseCase層のPresenter層への不適切な依存**:
    *   `LevelUseCase` がコンストラクタで `Func<LevelUpgradeNode[], Task<LevelUpgradeNode>>` を受け取っており、これはPresenter層のUI操作ロジックに直接依存する形となっていました。
    *   `LevelUpgradeNode` (Entity層) を `IngameUIManager` (UI層) が直接参照しており、レイヤー間の依存関係ルールに違反していました。
*   **`IngameStartSequence` の責務の曖昧さ**:
    *   `IngameStartSequence` (Infrastructure層) が、インスタンス生成、DI、イベント購読といった役割を集中して担っており、依存関係が不明瞭になる可能性がありました。
*   **イベント駆動への移行が不完全**:
    *   初期リファクタリング後も `InGameLoopUseCase` に `RunGameLoop` という `while` ループが残存しており、ロジックがイベント駆動とループ駆動で混在していました。これにより、ウェーブ進行管理の責務が `InGameLoopUseCase` と `WaveSystemPresenter` に分散し、混乱を招いていました。

## 3. 解決策の概要

各層（UseCase、Presenter、Infrastructure）の責務を再定義し、DIとイベント駆動を徹底することで、上記課題を解決しました。

*   **UseCase層 (`InGameLoopUseCase`, `LevelUseCase`, `WaveUseCase`) の純粋化**:
    *   `InGameLoopUseCase` を、**イベント駆動によるゲーム進行管理の中心**として再定義。`RunGameLoop` メソッドを完全に削除し、`HandleWaveCompleted` のようなイベントハンドラで状態遷移（ウェーブ遷移、レベルアップ、ゲーム終了）を一元的に管理するようにしました。
    *   `LevelUseCase` はレベルアップロジック（経験値管理、ノード候補生成）に専念し、UI操作ロジックへの依存を排除しました。
    *   `WaveUseCase` はウェーブのデータ管理と、次のウェーブが存在するかどうかの純粋な状態提供に専念するようにしました。
    *   **新規 `LevelUpgradeOption` の導入**: UseCase層で抽象化されたレベルアップ選択肢のデータ構造を定義し、UseCase層がPresenter層のViewModelに直接依存しないようにしました。
*   **Presenter層 (`WaveSystemPresenter`, `IngameUIManager`) の責務の特化**:
    *   `WaveSystemPresenter` は、UI表示、演出、BGM再生、敵の生成といった、純粋なPresenterとしての役割に特化させました。`WaveUseCase.NextWave()` の呼び出しを削除し、ウェーブ完了を `OnWaveCompleted` イベントでUseCase層に通知するだけの責務としました。
    *   `IngameUIManager` は `LevelUpgradeNode` の代わりに `LevelUpgradeNodeViewModel` を扱うことで、UI層としての責務に集中し、Entity層への直接的な依存を排除しました。
*   **Infrastructure層 (`IngameStartSequence`) の責務の明確化**:
    *   `IngameStartSequence` を、各コンポーネントのインスタンス生成と、**UseCaseとPresenter間のイベント購読の「橋渡し役」**として明確に位置づけました。
    *   `InGameLoopUseCase` のイベント (`OnGameStarted`, `OnWaveChanged`, `OnGameEnded`) を `WaveSystemPresenter` のメソッド (`GameStart`, `ChangeWave`) やシーン遷移ロジック (`GoToOutGameScene`) に接続しました。
    *   `WaveSystemPresenter` の `OnWaveCompleted` イベントを `InGameLoopUseCase.HandleWaveCompleted` メソッドに接続しました。

## 4. 具体的な変更点

### 4.1. `LevelUpgradeOption.cs` (新規作成)

*   `Assets/Scripts/Runtime/UseCase/Ingame/System/LevelUpgradeOption.cs` に新規作成しました。
*   `LevelUpgradeNode` の情報をラップし、UseCase層がPresenter層のViewModelに依存しないようにするための抽象化レイヤーとして機能します。

### 4.2. `WaveUseCase.cs`

*   `HasNextWave` プロパティを追加し、`InGameLoopUseCase` がゲーム終了条件を判断するために利用できるようにしました。
*   `AddLevelProgress` メソッドを `LevelUseCase` に追加し、経験値加算の責務を移譲しました。

### 4.3. `InGameLoopUseCase.cs`

*   **`RunGameLoop` メソッドと、ターンベースの関連イベント (`OnTurnStarted` 等)を完全に削除**し、イベント駆動モデルに統一しました。
*   `public event Action<WaveEntity> OnWaveChanged;` イベントを追加しました。
*   `HandleWaveCompleted()` メソッドを修正し、経験値加算、次のウェーブへの遷移、レベルアップ処理、そして `OnWaveChanged` (次のウェーブがある場合) または `OnGameEnded` (ない場合) のイベント発行という、ゲーム進行の中心的責務を担うようにしました。
*   `StartGameAsync` メソッドからループ処理の呼び出しを削除し、ゲームの初期化と `OnGameStarted` イベントの発行に専念させました。
*   コンストラクタのシグネチャを `Func<LevelUpgradeOption[], Task<LevelUpgradeOption>>` に変更し、UIへの依存を排除しました。

### 4.4. `LevelUseCase.cs`

*   コンストラクタからUI操作に関連する `Func` デリゲートを削除しました。
*   `WaitLevelUpSelectAsync` メソッドが `Func<LevelUpgradeOption[], Task<LevelUpgradeOption>>` を引数として受け取るように変更し、UIから完全に独立させました。
*   `AddLevelProgress` メソッドを追加し、ウェーブ完了時の経験値加算ロジックをここに集約しました。

### 4.5. `WaveSystemPresenter.cs`

*   `public event Action OnWaveCompleted;` イベントを追加しました。
*   `HandleEnemyDead()` メソッド内から、**`_waveUseCase.NextWave()` と `ChangeWave()` の呼び出しを削除**し、`OnWaveCompleted` イベントを発行するだけのシンプルな実装に変更しました。
*   `ChangeWave()` メソッドのアクセス修飾子を `public` に変更し、`InGameLoopUseCase` からのイベントで呼び出せるようにしました。また、ゲーム終了判定ロジックを削除しました。
*   コンストラクタから `LevelUseCase` を削除し、DIを整理しました。

### 4.6. `IngameUIManager.cs`

*   `LevelUpSelectAsync` メソッドのシグネチャを `Task<LevelUpgradeNodeViewModel> LevelUpSelectAsync(LevelUpgradeNodeViewModel[] nodes)` に変更し、UI層がViewModelにのみ依存するようにしました。

### 4.7. `IngameStartSequence.cs`

*   `LevelUseCase`, `WaveUseCase`, `InGameLoopUseCase` のインスタンスを生成し、`ServiceLocator` に登録しました。
*   `InGameLoopUseCase` のコンストラクタに、`ingameUIManager.LevelUpSelectAsync` をラップして `LevelUpgradeOption` を扱うコールバック関数を渡すようにしました。
*   **イベント購読の接続を更新**:
    *   `inGameLoopUseCase.OnGameStarted += waveSystem.GameStart;`
    *   `waveSystem.OnWaveCompleted += async () => await inGameLoopUseCase.HandleWaveCompleted();`
    *   `inGameLoopUseCase.OnWaveChanged += waveSystem.ChangeWave;` **(新規接続)**
    *   `inGameLoopUseCase.OnGameEnded += GoToOutGameScene;` **(購読先変更)**
*   `inGameLoopUseCase.StartGameAsync()` を最後に呼び出すことで、イベント駆動のゲームループを開始するようにしました。

## 5. 結果

本タスクを通じて、`InGameLoopUseCase` を中心としたインゲームループの構造が、クリーンアーキテクチャの原則に沿って大幅に改善されました。

*   **責務の明確化**: UseCase層がゲームロジックと状態遷移の唯一の権威となり、Presenter層はUseCaseからの通知を受けて表示を更新する責務に特化しました。ロジックの重複と分散が解消されました。
*   **疎結合化**: イベント駆動への完全な移行により、コンポーネント間の結合度がさらに低減され、変更による影響範囲が限定的になりました。
*   **テスト容易性の向上**: UseCase層がUIやPresenterの実装から完全に独立したため、ドメインロジックの単体テストが極めて容易になりました。
*   **拡張性の向上**: ゲーム進行ロジックが `InGameLoopUseCase` に集約されたことで、将来的な機能追加（例：特殊なウェーブ、ボス戦の導入）が、既存のコードに大きな影響を与えることなく行えるようになりました。
*   **コンパイルエラーの解消**: 修正プロセスで発生した閉じ括弧の不足などのコンパイルエラーもすべて解消されました。

これにより、プロジェクトの保守性、堅牢性、拡張性が大きく向上し、今後の開発を加速させる強固な基盤が構築されました。
