### **計画書**
**タイトル:** レベルアップUIが表示されない問題の原因究明（フェーズ1：ログによる処理追跡）

**目的:**
レベルアップ時にUIウィンドウが表示されない原因を究明するため、まずレベルアップフェーズが正しく開始され、各処理が実行されているかを確認する。

**作業内容:**
1.  `Assets/Docs/Task/` ディレクトリに計画書ファイルを作成する。（←現在このタスクを実行中）
2.  `Runtime/UseCase/Ingame/System/InGameLoopUseCase.cs` ファイルの `RunIngameLoop` メソッド内にデバッグログを追加し、レベルアップフェーズの開始、各レベルアップ処理、フェーズ終了を追跡できるようにする。

**変更ファイル:**
*   `Assets/Docs/Task/レベルアップUIが表示されない問題の原因究明_フェーズ1.md`
*   `Runtime/UseCase/Ingame/System/InGameLoopUseCase.cs`

**ログ追加箇所:**
`Runtime/UseCase/Ingame/System/InGameLoopUseCase.cs` の `RunIngameLoop` メソッド内の以下の部分。

```csharp
if (_levelUseCase.LevelUpQueue.Any())
{
    // ここにログを追加
    _levelUpPhaseHandler.OnLevelUpPhaseStarted();
    while (_levelUseCase.LevelUpQueue.TryDequeue(out var newLevel))
    {
        // ここにもログを追加
        await _levelUseCase.HandleLevelUpAsync(_onLevelUpSelectNodeCallback);
    }
    _levelUpPhaseHandler.OnLevelUpPhaseEnded();
    // ここにもログを追加
}
```

---
**完了レポート:**
`Runtime/UseCase/Ingame/System/InGameLoopUseCase.cs` ファイルの `GameLoopAsync` メソッド内に、レベルアップフェーズの開始、各レベルアップ処理の実行、フェーズ終了を示すデバッグログを追加しました。これにより、ゲーム実行時にログを確認することで、レベルアップ処理がどこまで進んでいるかを確認できるようになりました。