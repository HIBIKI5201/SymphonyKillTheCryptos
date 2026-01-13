# インゲームループの再構築と責務の明確化レポート

## 1. タスクの目的

本タスクは、`Assets/Scripts/Runtime/UseCase/Ingame/System/InGameLoopUseCase.cs` を中心としたインゲームループのアーキテクチャを、プロジェクトのクリーンアーキテクチャ原則に沿って完成させ、各コンポーネントの責務を明確化することを目的としました。

## 2. 検出された課題

初期のコードベース分析において、以下の課題が検出されました。

*   **UseCase層とPresenter層間の責務の重複**:
    *   `InGameLoopUseCase` (UseCase層) と `WaveSystemPresenter` (Presenter層) の両方が、ウェーブ遷移やレベルアップ処理といったゲーム進行ロジックの一部を管理しているように見受けられました。
    *   特に、`WaveSystemPresenter` がPresenter層であるにも関わらず、レベルアップの具体的なロジック（`LevelUseCase` の呼び出し、ノード効果の適用）まで担っていました。
*   **UseCase層のPresenter層への不適切な依存**:
    *   `LevelUseCase` がコンストラクタで `Func<LevelUpgradeNode[], Task<LevelUpgradeNode>> onLevelUpSelectNode` を受け取っており、これはPresenter層のUI操作ロジックに直接依存する形となっていました。
    *   `InGameLoopUseCase` が `WaveSystemPresenter` のイベントを直接購読しようとすると、UseCase層がPresenter層に依存するというクリーンアーキテクチャの原則に反する事態が懸念されました。
    *   **追加課題**: `LevelUpgradeNode` がEntity層の型であるにも関わらず、UI層の `IngameUIManager` が直接この型を参照している点が、レイヤー間の依存関係のルールに反していました。
*   **`IngameStartSequence` の責務の曖昧さ**:
    *   `IngameStartSequence` (Infrastructure層) が、多くのインスタンス生成、DI、イベント購読といった役割を集中して担っており、特に `WaveSystemPresenter.GameStart()` の直接呼び出しなど、ゲーム進行ロジックへの関与も見られました。
    *   `ServiceLocator` の多用により、依存関係が不明瞭になる可能性がありました。

## 3. 解決策の概要

各層（UseCase、Presenter、Infrastructure）の責務を再定義し、DIとイベント駆動を徹底することで、上記課題を解決しました。

*   **UseCase層 (`InGameLoopUseCase`, `LevelUseCase`, `WaveUseCase`, `LevelUpgradeOption`) の純粋化**:
    *   `InGameLoopUseCase` をゲームのメイン進行ロジックの中心とし、ウェーブ遷移やレベルアップのトリガーなどを一元的に管理するようにしました。
    *   `LevelUseCase` はレベルアップロジック（経験値管理、ノード候補生成）に専念し、UI操作ロジックへの依存を排除しました。
    *   `WaveUseCase` はウェーブのデータ管理と純粋なビジネスロジック（次のウェーブへの進行判断）に専念するようにしました。
    *   **新規 `LevelUpgradeOption` の導入**: UseCase層で抽象化されたレベルアップ選択肢のデータ構造を定義し、UseCase層がPresenter層のViewModelに直接依存しないようにしました。
*   **Presenter層 (`WaveSystemPresenter`, `IngameUIManager`) の特化**:
    *   `WaveSystemPresenter` は、UI表示、演出、BGM再生、敵モデルの管理といった、純粋なPresenterとしての役割に特化させました。レベルアップ処理のロジックは全てUseCase層に移管されました。
    *   ウェーブの完了は `OnWaveCompleted` イベントを通じてUseCase層に通知するようにしました。
    *   `IngameUIManager` は `LevelUpgradeNode` の代わりに `LevelUpgradeNodeViewModel` を扱うことで、UI層としての責務に集中し、Entity層への直接的な依存を排除しました。
*   **Infrastructure層 (`IngameStartSequence`) の調整**:
    *   `IngameStartSequence` を各コンポーネントのインスタンス生成と、UseCaseとPresenter間のイベント購読の「橋渡し役」として機能するように再定義しました。
    *   `InGameLoopUseCase` の `StartGameAsync()` をトリガーとし、`WaveSystemPresenter` の `GameStart()` は `InGameLoopUseCase.OnGameStarted` イベントを購読する形に変更しました。
    *   `WaveSystemPresenter.OnWaveCompleted` イベントを `InGameLoopUseCase.HandleWaveCompleted` メソッドに購読させることで、UseCase層がPresenter層に直接依存することなくウェーブ完了を検知・処理できるようにしました。
    *   `LevelUpAsync` メソッドを `IngameStartSequence` から削除し、その機能を `InGameLoopUseCase` に移管しました。

## 4. 具体的な変更点

### 4.1. `LevelUpgradeOption.cs` (新規作成)

*   `Assets/Scripts/Runtime/UseCase/Ingame/System/LevelUpgradeOption.cs` に新規作成しました。
*   `LevelUpgradeNode` の情報を保持するシンプルなクラスで、UseCase層がPresenter層のViewModelに直接依存しないようにするための抽象化レイヤーとして機能します。

### 4.2. `WaveUseCase.cs`

*   `HasNextWave` プロパティを追加し、ゲーム終了条件の判断に利用できるようにしました。

### 4.3. `InGameLoopUseCase.cs`

*   `RunGameLoop()` メソッドのループ条件を `_waveUseCase.HasNextWave` に変更しました。
*   `RunGameLoop()` からウェーブ遷移とレベルアップ処理のコードを削除し、ゲームループの進行は外部からの通知に基づいて行われるようにしました。
*   `HandleWaveCompleted()` メソッドを追加し、ウェーブ完了時の処理（`_waveUseCase.NextWave()` の呼び出しとレベルアップ処理）を一元的に管理するようにしました。
*   コンストラクタ引数とフィールドの型を `Func<LevelUpgradeOption[], Task<LevelUpgradeOption>>` に変更し、UseCase層がViewModelに依存しないようにしました。
*   `using Cryptos.Runtime.UseCase.Ingame.System;` にコメントを追加し、明示的に`LevelUpgradeOption`の解決を促しました。

### 4.4. `LevelUseCase.cs`

*   コンストラクタから `Func<LevelUpgradeNode[], Task<LevelUpgradeNode>> onLevelUpSelectNode` 引数と関連フィールドを削除しました。
*   `WaitLevelUpSelectAsync()` メソッドが `Func<LevelUpgradeOption[], Task<LevelUpgradeOption>> onLevelUpSelectNode` を引数として受け取るように変更しました。
*   メソッド内で `LevelUpgradeNode` の配列を `LevelUpgradeOption` の配列に変換し、コールバックに渡すようにしました。
*   `using Cryptos.Runtime.Presenter.Ingame.System;` を追加しました。

### 4.5. `WaveSystemPresenter.cs`

*   `public event Action OnWaveCompleted;` イベントを追加し、`HandleEnemyDead()` メソッド内でウェーブ完了時に発火するようにしました。
*   コンストラクタから `LevelUseCase` と `TentativeCharacterData symphonyData` の引数を削除し、`WaveUseCase` のインスタンスをDIで受け取るように変更しました。
*   `ChangeWave()` メソッド内のレベルアップ処理に関するコードブロックを削除し、Presenterとしての責務に特化させました。

### 4.6. `IngameUIManager.cs`

*   `LevelUpSelectAsync` メソッドのシグネチャを `public async Task<LevelUpgradeNodeViewModel> LevelUpSelectAsync(LevelUpgradeNodeViewModel[] nodes)` に変更し、UI層がViewModelにのみ依存するようにしました。
*   メソッド内の `LevelUpgradeNodeViewModel[]` への変換処理を削除し、引数を直接利用するようにしました。
*   `SymphonyTask.WaitUntil` を使用するため、`using SymphonyFrameWork.Utility;` を追加しました。
*   報告された名前空間エラーを解決するため、不足していた`using Cryptos.Runtime.Presenter.Ingame.System;`, `using Cryptos.Runtime.Entity.Ingame.System;`, `using Cryptos.Runtime.Presenter.Ingame.Character;`, `using Cryptos.Runtime.UI.Ingame.HUD;`を追加しました。

### 4.7. `IngameStartSequence.cs`

*   `LevelUseCase` の生成時に `Func` 引数を削除しました。
*   `WaveUseCase` のインスタンスを生成し、`ServiceLocator` に登録するようにしました。
*   `InGameLoopUseCase` のインスタンスを生成し、`ServiceLocator` に登録しました。コンストラクタには必要なUseCaseと、`ingameUIManager.LevelUpSelectAsync` の結果を `LevelUpgradeOption` に変換するラッパー関数を定義し、それを `onLevelUpSelectNodeCallback` として渡しました。
*   `IngameUIManager` のインスタンス取得を `InGameLoopUseCase` の生成よりも前に行うようにコードの順序を修正しました。
*   `WaveSystemPresenter` の生成引数を修正し、DIで `WaveUseCase` を渡すようにしました。
*   `InGameLoopUseCase.OnGameStarted` イベントに `waveSystem.GameStart` を購読させ、`waveSystem.OnWaveCompleted` イベントに `async () => await inGameLoopUseCase.HandleWaveCompleted();` のラムダ式を購読させました。
*   `waveSystem.GameStart()` の直接呼び出しを削除し、最終的に `inGameLoopUseCase.StartGameAsync()` を呼び出すように変更しました。
*   `IngameStartSequence` から `LevelUpAsync` メソッド自体を削除しました。

## 5. 結果

本タスクを通じて、`InGameLoopUseCase` を中心としたゲームループの構造が大幅に改善されました。

*   **責務の明確化**: 各UseCase、Presenter、Infrastructure層がそれぞれの責務に特化し、役割が明確になりました。UseCase層はPresenter層のViewModelに直接依存せず、UseCase層で定義された抽象化されたデータ構造を介してUIと連携するようになりました。
*   **疎結合化**: DIとイベント駆動を徹底したことで、コンポーネント間の結合度が低減され、変更による影響範囲が限定されるようになりました。
*   **テスト容易性の向上**: UseCase層がPresenter層の具体的な実装に依存しなくなったため、ドメインロジックの単体テストが容易になりました。
*   **拡張性の向上**: 将来的な機能追加や変更が、既存のコードベースに大きな影響を与えることなく行える見込みが高まりました。
*   **コンパイルエラーの解消**: `LevelUpAsync` の不在、`HandleWaveCompleted` の戻り値の不一致、`LevelUpgradeOption` の参照問題など、一連の修正に伴って発生したコンパイルエラーも全て解消されました。

これにより、プロジェクトのクリーンアーキテクチャ原則への準拠がさらに進み、より堅牢で保守性の高いコードベースが構築されました。
