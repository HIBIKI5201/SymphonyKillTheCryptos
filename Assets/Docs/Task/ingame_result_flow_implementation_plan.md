# インゲームリザルトフロー実装計画

## 目的
インゲームが終了した後、リザルト画面を表示し、リザルト画面でのユーザー操作（例: 閉じるボタンクリック）を待機した後、メインメニューなどのアウトゲームシーンへ安全に遷移する機能を実装する。

## 現在の状況
- インゲームの終了を検知するロジックは存在するが、リザルト画面への遷移やアウトゲームへの戻り処理は未実装、または断片的にしか存在しない可能性がある。
- リザルト画面用のUIコンポーネント（プレハブやスクリプト）の有無は不明。
- シーン遷移の管理方法（例: SceneManagerの直接使用、Addressables、カスタムのシーン管理システム）は不明。

## 実装の概要
1.  **リザルト表示契機の特定**: インゲームの終了条件が満たされた際に、リザルト表示ロジックが呼び出されるようにする。
2.  **リザルトUIの表示**: リザルト情報を表示するためのUIコンポーネントをロード・表示する。新規作成が必要な場合は作成する。
3.  **ユーザー操作の待機**: リザルトUIが表示されている間、ユーザーがアウトゲームへ戻るための操作（例: ボタンクリック）を待機する。
4.  **アウトゲームへの遷移**: ユーザー操作後、アウトゲームシーンへ遷移する。

## 詳細なステップ

1.  **計画書の作成** (完了)
    - 本計画書を `Assets\Docs\Task\ingame_result_flow_implementation_plan.md` に作成。

2.  **既存コードの調査** (完了)
    - インゲーム終了をトリガーする場所を特定 (`InGameLoopUseCase.cs` の `_onGameEndedCallback`)。
    - 既存のシーン遷移ロジックを調査 (`SceneLoader` および `SceneListEnum.Outgame`)。
    - UI表示に関する既存のフレームワークやヘルパークラスの有無を調査 (`UnityEngine.UIElements`, `UIManagerBase`, `MasterUIManager`, `IngameUIManager`, `OutgameUIManager`, `ViewModel` の利用)。
    - リザルト表示に関する既存のプレハブやスクリプトの有無を調査 (既存のリザルトUIはなし)。

3.  **リザルトUIの作成と実装** (完了)
    - 新規にリザルトUIのUXML (`Assets\Arts\UI ToolKit\InGame\UXML\ResultWindow.uxml`) を作成。
    - 関連するC#スクリプト (`Assets\Scripts\Runtime\UI\Ingame\UIElementResultWindow.cs`) を作成。
        - `VisualElementBase` を継承するように修正。
        - `Open()` と `Close()` メソッドを実装し、`style.display` を切り替えることで表示・非表示を制御。
    - `IIngameUIManager` インターフェースに `OpenResultWindow` メソッドを追加。
    - `IngameUIManager.cs` に `UIElementResultWindow` をフィールドとして追加し、`InitializeDocumentAsync` で初期化およびルート要素への追加。
    - `IngameUIManager.cs` に `OpenResultWindow` と `CloseResultWindow` メソッドを実装。

4.  **インゲーム終了後のフロー接続** (完了)
    - `InGameLoopUseCase.cs` の `_onGameEndedCallback` の型を `Action<string, int>` に変更し、ゲーム終了時に仮のタイトルと現在のレベルをスコアとして渡すように修正。
    - `IngameStartSequence.cs` の `GoToOutGameScene` メソッドを `OnGameEndedCallback(string title, int score)` にリファクタリング。
    - `OnGameEndedCallback` の中で `_gameUIManager.OpenResultWindow(title, score, GoToOutGameSceneInternal)` を呼び出し、リザルト画面でのユーザー操作後にアウトゲームシーンへ遷移する `GoToOutGameSceneInternal` ローカルメソッドを登録。
    - `GoToOutGameSceneInternal` の中で、元のシーン遷移処理と `_gameUIManager.CloseResultWindow` を呼び出すように変更。

5.  **アウトゲームシーンへの遷移** (完了)
    - 上記「インゲーム終了後のフロー接続」の一部として実装済み。`SceneLoader.UnloadScene` と `SceneLoader.LoadScene` を利用。

6.  **テスト** (ユーザーの実行待ち)
    - Unityエディタでインゲームを終了させ、リザルトUIが正しく表示され、閉じる操作でアウトゲームシーンへ遷移することを確認してください。

7.  **完了レポートの追記** (完了)
    - 本セクションにレポートを追記。

## 懸念事項/確認事項
- シーン遷移時のデータの引き継ぎ方法（もし必要であれば）。
- リザルト画面で表示すべき具体的な情報とそのデータソース。
- 既存のUIシステムやフレームワークとの連携方法。
- Addressablesを使用している場合、リザルトUIのロード方法をAddressablesに対応させる必要がある。

---
### 完了レポート

インゲーム終了時にリザルト画面を表示し、ユーザーが操作した後にアウトゲームへ戻る機能の実装が完了しました。

**変更点:**
- `Assets\Arts\UI ToolKit\InGame\UXML\ResultWindow.uxml` を新規作成し、シンプルなリザルトUIを定義しました。
- `Assets\Scripts\Runtime\UI\Ingame\UIElementResultWindow.cs` を新規作成し、`ResultWindow.uxml` に対応するUI要素の表示・非表示、およびボタンイベントの通知機能を提供しました。`SymphonyVisualElement` が存在しないことを確認し、`VisualElementBase` を継承し、`Open()`/`Close()` メソッドを実装しました。
- `Assets\Scripts\Runtime\Presenter\Ingame\System\IIngameUIManager.cs` に `OpenResultWindow` メソッドを追加し、リザルトUI表示のインターフェースを定義しました。
- `Assets\Scripts\Runtime\UI\Ingame\HUD\IngameUIManager.cs` を修正し、`UIElementResultWindow` を管理するためのフィールドと、`IIngameUIManager` インターフェースの実装として `OpenResultWindow` および `CloseResultWindow` メソッドを追加しました。
- `Assets\Scripts\Runtime\UseCase\Ingame\System\InGameLoopUseCase.cs` を修正し、ゲーム終了時のコールバック `_onGameEndedCallback` の型を `Action<string, int>` に変更しました。これにより、リザルト画面に表示するタイトルとスコアを渡せるようになりました。
- `Assets\Scripts\Runtime\InfraStructure\IngameStartSequence.cs` を修正し、`InGameLoopUseCase` に渡すゲーム終了時のコールバックを `OnGameEndedCallback` メソッドとして実装しました。このメソッド内で `IngameUIManager.OpenResultWindow` を呼び出し、リザルト画面が閉じられた後にアウトゲームへのシーン遷移を行うようにしました。

**今後のタスク:**
- Unityエディタでプロジェクトを開き、インゲームを最後までプレイして、リザルト画面が正しく表示され、ボタンを押すことでアウトゲームに遷移することを確認してください。
- リザルト画面で表示する具体的な情報（スコア計算ロジック、クリアタイムなど）は、現在の実装では仮のデータ (`"Game Clear!"` と `_levelUseCase.CurrentLevel`) を渡しているため、必要に応じて `InGameLoopUseCase` や関連するUseCaseで計算し、`OnGameEndedCallback` の引数として渡すように変更してください。
- リザルト画面のUI/UXデザインを改善してください。

上記が完了したら、次の指示をお願いします。