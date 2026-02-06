# インゲームリザルトフロー変更に関するレポート

## 1. はじめに

本レポートは、インゲーム終了時のフローにおいて、リザルトウィンドウを導入し、ユーザー体験を向上させるための現状分析と変更の必要性について記述する。

## 2. 現状のインゲーム終了フロー

現在のインゲームは、終了すると直接アウトゲーム（メインメニューやステージ選択画面など）に遷移する。この遷移ロジックは、主に `IngameStartSequence.cs` 内の `GoToOutGameScene()` メソッドによって制御されている。

### 2.1 シーン遷移のトリガー

ゲーム終了のトリガーは `InGameLoopUseCase.cs` で検知され、その際にコンストラクタで渡された `onGameEndedCallback` が呼び出される。この `onGameEndedCallback` には `IngameStartSequence.cs` の `GoToOutGameScene()` メソッドが設定されている。

### 2.2 `GoToOutGameScene()` の挙動

`IngameStartSequence.GoToOutGameScene()` メソッドは以下の処理を実行する。

1.  `_isTransitioning` フラグにより、二重のシーン遷移を防ぐ。
2.  現在アクティブな `Stage` シーンと `Ingame` シーンをアンロードする。
3.  `Outgame` シーンをロードする。
4.  再度 `Stage` シーンをロードし、アクティブシーンに設定する。この `Stage` シーンはアウトゲームのメイン画面などで共通利用されるシーンであると推測される。

#### コードスニペット

```csharp
private async void GoToOutGameScene()
{
    if (_isTransitioning) return;
    _isTransitioning = true;

    await SceneLoader.UnloadScene(SceneListEnum.Stage.ToString());
    await SceneLoader.UnloadScene(SceneListEnum.Ingame.ToString());

    await SceneLoader.LoadScene(SceneListEnum.Outgame.ToString());
    await SceneLoader.LoadScene(SceneListEnum.Stage.ToString());
    SceneLoader.SetActiveScene(SceneListEnum.Stage.ToString());
}
```

## 3. 問題点

現状のフローでは、プレイヤーはインゲームでの結果（スコア、獲得アイテム、達成度など）を確認する機会がないままアウトゲームへ強制的に遷移させられるため、ゲーム体験として不完全である。プレイヤーは自身のパフォーマンスを即座にフィードバックとして得られず、達成感や次のゲームへのモチベーションに繋がりづらい可能性がある。

## 4. 改善提案

インゲーム終了時にリザルトウィンドウを挟むことで、プレイヤーにゲーム結果を提示し、より良いゲーム体験を提供する。リザルトウィンドウでのユーザー操作（例: 「次へ」ボタン押下）をトリガーとして、アウトゲームへの遷移を行うように変更する。

### 4.1 提案されるフロー

1.  `InGameLoopUseCase` がゲーム終了を検知。
2.  `InGameLoopUseCase` は `IngameResultUseCase` (新規導入) を呼び出し、リザルト表示を依頼する。
3.  `IngameResultUseCase` はリザルトウィンドウを表示し、結果データを設定する。
4.  プレイヤーがリザルトウィンドウで「次へ」などの操作を行うまで待機する。
5.  プレイヤー操作後、`IngameResultUseCase` はアウトゲームへのシーン遷移をトリガーする。この際、現在の `IngameStartSequence.GoToOutGameScene()` に相当するロジックを再利用またはリファクタリングして呼び出す。

## 5. 影響範囲と変更箇所（計画書より抜粋）

*   **`Assets/Scripts/Runtime/InfraStructure/Ingame/Sequence/IngameStartSequence.cs`**:
    *   `InGameLoopUseCase` のコンストラクタに渡すコールバックの変更。
    *   `GoToOutGameScene` メソッドの変更または移動。
*   **`Assets/Scripts/Runtime/UseCase/Ingame/System/InGameLoopUseCase.cs`**:
    *   ゲーム終了時のコールバック呼び出し箇所の変更。
*   **新規ファイル**:
    *   `Assets/Scripts/Runtime/UseCase/Ingame/Result/IngameResultUseCase.cs` (仮称)
    *   `Assets/Scripts/Runtime/Presenter/Ingame/Result/IngameResultPresenter.cs` (仮称)
    *   `Assets/Scripts/Runtime/UI/Ingame/Result/UIResultWindow.cs` (仮称) および関連するUXML/USSファイル。

## 6. 今後の作業

計画書に記載の通り、`IngameResultUseCase` の設計と実装、UIの実装、関連する`Presenter`および`ViewModel`の実装、既存の`UseCase`と`StartSequence`の修正、そしてテストコードの追加または修正を進める。
