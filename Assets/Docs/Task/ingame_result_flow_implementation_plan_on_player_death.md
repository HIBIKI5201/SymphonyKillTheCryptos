# プレイヤー体力0時のインゲームリザルトフロー実装計画

## 目的
プレイヤーキャラクター（Symphony）の体力が0になった際にインゲームを終了し、リザルト画面を表示した後、アウトゲームシーンへ安全に遷移する機能を実装する。

## 現在の状況
- インゲームは全てのウェーブが終了した場合にのみ終了する。
- プレイヤーの体力0によるゲーム終了は考慮されていない。
- リザルト画面表示とアウトゲームへの遷移ロジックは実装済み。

## 実装の概要
1.  **Symphonyの体力監視**: Symphonyの現在の体力とその変更を監視する。
2.  **ゲームオーバー条件の追加**: Symphonyの体力が0になった場合にゲームオーバーとして処理するロジックを既存のゲームループに追加する。
3.  **リザルト画面のトリガー**: プレイヤーの体力0によるゲームオーバー時にもリザルト画面を表示し、アウトゲームへ遷移させる。

## 詳細なステップ

1.  **計画書の作成** (完了)
    - 本計画書を `Assets\Docs\Task\ingame_result_flow_implementation_plan_on_player_death.md` に作成。

2.  **Symphonyの体力情報とゲーム終了条件の調査** (完了)
    - Symphonyの体力は `CharacterEntity` が持つ `HealthEntity` で管理されており、死亡イベントとして `CharacterEntity.OnDead` が利用可能であることを確認。
    - `InGameLoopUseCase` が `ISymphonyPresenter` を介して `CharacterEntity` にアクセスし、`OnDead` イベントを購読できることを確認。
        - `ISymphonyPresenter.cs` に `event Action OnDead;` を追加。（ユーザーの変更による）
        - `SymphonyPresenter.cs` に `ISymphonyPresenter.OnDead` イベントの実装を追加。（ユーザーの変更による）

3.  **ゲーム終了条件の追加** (完了)
    - `InGameLoopUseCase.cs` を修正し、`StartGameAsync` メソッド内で `_symphonyPresenter.OnDead` イベントを購読。
    - `HandlePlayerDead` メソッドを実装し、プレイヤー死亡時に `EndGame("Game Over!", _levelUseCase.CurrentLevel)` を呼び出すように変更。
    - ゲームループ `GameLoopAsync` の中に `_isGameEnded` フラグによる中断処理を追加。
    - `EndGame` メソッドを追加し、ゲーム終了処理（`_inGameLoopWaveHandler.OnGameEnded()` と `_ingameLoopPresenter.RequestShowResult()`）を共通化し、ゲーム終了時のタイトルを引数として渡せるように変更。
    - `CleanupGame` メソッドを追加し、`OnDead` イベントの購読解除を行うように変更。

4.  **Presenterクラスを仲介する方式への変更** (完了)
    - `IngameUIManager.cs` の `OpenResultWindow` メソッドのシグネチャから `Action onReturnButtonClicked` を削除。
    - `IngameUIManager.cs` に `OnResultWindowReturnButtonClicked` イベントを追加し、`UIElementResultWindow.OnReturnButtonClicked` を購読してこのイベントを発火するように変更。
    - `IIngameLoopPresenter.cs` インターフェースを新規作成。
    - `IngameLoopPresenter.cs` クラスを新規作成し、`IIngameLoopPresenter` を実装。`IIngameUIManager` を通じてリザルト画面の表示を要求し、`IngameUIManager.OnResultWindowReturnButtonClicked` イベントを購読して自身の `OnResultWindowReturnButtonClicked` イベントを発火する。
    - `InGameLoopUseCase.cs` のコンストラクタで `IIngameLoopPresenter` を受け取り、`EndGame` メソッド内で `_ingameLoopPresenter.RequestShowResult` を呼び出すように変更。
    - `IngameStartSequence.cs` で `IngameLoopPresenter` をインスタンス化し、`InGameLoopUseCase` に注入。また、`ingameLoopPresenter.OnResultWindowReturnButtonClicked` イベントを購読し、その中でアウトゲームへのシーン遷移をトリガーするように変更。
    - `IngameStartSequence.cs` の `GoToOutGameSceneInternal` メソッド内の `_gameUIManager.CloseResultWindow` の引数を削除。

5.  **テスト** (ユーザーの実行待ち)
    - Unityエディタでプロジェクトを開き、プレイヤーの体力が0になった際に、リザルトUIが正しく表示され、ボタンを押すことでアウトゲームに遷移することを確認してください。

6.  **完了レポートの追記** (完了)
    - 本セクションにレポートを追記。

## 懸念事項/確認事項
- 複数のゲーム終了条件（全ウェーブクリア、プレイヤー体力0）がどのように連携するか。
- ゲームオーバー時のリザルト画面表示内容（例: スコアを0にするか、それまでのスコアを表示するか）。

---
### 完了レポート

プレイヤーキャラクター（Symphony）の体力が0になった際にも、ゲーム終了時にリザルト画面を表示し、アウトゲームへ遷移する機能が実装されました。

また、ユーザーのフィードバックに基づき、UI層がUseCase層に直接依存しないように、Presenterクラスを仲介する方式に設計を変更しました。

**変更点:**
- `Assets\Scripts\Runtime\UseCase\Ingame\Character\ISymphonyPresenter.cs` に `event Action OnDead;` を追加。（ユーザーの変更による）
- `Assets\Scripts\Runtime\Presenter\Ingame\Character\Player\SymphonyPresenter.cs` に `ISymphonyPresenter.OnDead` イベントの実装を追加。（ユーザーの変更による）
- `Assets\Scripts\Runtime\UseCase\Ingame\System\InGameLoopUseCase.cs` を修正し、以下の機能を追加しました。
    - ゲーム開始時にプレイヤーの死亡イベント `_symphonyPresenter.OnDead` を購読するようになりました。
    - プレイヤー死亡時に `HandlePlayerDead` メソッドが呼ばれ、ゲームを「Game Over!」として終了させます。
    - `_isGameEnded` フラグを導入し、ゲーム終了処理が重複して呼ばれないように制御しました。
    - ゲーム終了処理を `EndGame` メソッドとして共通化し、タイトルとスコアを引数で渡せるようにしました。
    - ゲーム終了時にイベントの購読解除を行う `CleanupGame` メソッドを追加しました。
    - `InGameLoopUseCase` のコンストラクタで `IIngameLoopPresenter` を受け取るように変更し、`EndGame` メソッド内で `_ingameLoopPresenter.RequestShowResult` を呼び出すように修正しました。
- `Assets\Scripts\Runtime\UI\Ingame\HUD\IngameUIManager.cs` を修正し、`OpenResultWindow` メソッドのシグネチャから `Action onReturnButtonClicked` を削除。また、`OnResultWindowReturnButtonClicked` イベントを追加し、`UIElementResultWindow.OnReturnButtonClicked` を購読してこのイベントを発火するように変更しました。
- `Assets\Scripts\Runtime\Presenter\Ingame\System\IIngameLoopPresenter.cs` インターフェースを新規作成し、`RequestShowResult` メソッドと `OnResultWindowReturnButtonClicked` イベントを定義しました。
- `Assets\Scripts\Runtime\Presenter\Ingame\System\IngameLoopPresenter.cs` クラスを新規作成し、`IIngameLoopPresenter` を実装。`IIngameUIManager` を通じてリザルト画面の表示を要求し、`IngameUIManager.OnResultWindowReturnButtonClicked` イベントを購読して自身の `OnResultWindowReturnButtonClicked` イベントを発火するようにしました。
- `Assets\Scripts\Runtime\InfraStructure\IngameStartSequence.cs` を修正し、`IngameLoopPresenter` をインスタンス化して `InGameLoopUseCase` に注入するように変更。また、`ingameLoopPresenter.OnResultWindowReturnButtonClicked` イベントを購読し、その中でアウトゲームへのシーン遷移 (`GoToOutGameSceneInternal`) をトリガーするようにしました。
- `Assets\Scripts\Runtime\InfraStructure\IngameStartSequence.cs` の `GoToOutGameSceneInternal` メソッド内の `_gameUIManager.CloseResultWindow` の引数を削除しました。

**今後のタスク:**
- Unityエディタでプロジェクトを開き、プレイヤーの体力が0になった際に、リザルト画面が正しく表示され、ボタンを押すことでアウトゲームに遷移することを確認してください。

上記が完了したら、次の指示をお願いします。
