### **計画書**
**タイトル:** レベルアップUIが表示されない問題の原因究明（フェーズ2：IngameUIManagerのログ追跡）

**目的:**
`IngameUIManager` 内でレベルアップUIの表示・非表示メソッドが正しく呼び出されているかを確認する。

**作業内容:**
1.  `Assets/Docs/Task/` ディレクトリに新たな計画書ファイルを作成する。
2.  `Assets/Scripts/Runtime/UI/Ingame/HUD/IngameUIManager.cs` ファイルの `LevelUpSelectAsync` メソッド内にデバッグログを追加し、`OpenLevelUpgradeWindow` と `CloseLevelUpgradeWindow` の呼び出しを追跡できるようにする。

**変更ファイル:**
*   `Assets/Docs/Task/レベルアップUIが表示されない問題の原因究明_フェーズ2.md`
*   `Assets/Scripts/Runtime/UI/Ingame/HUD/IngameUIManager.cs`

**ログ追加箇所:**
`Assets/Scripts/Runtime/UI/Ingame/HUD/IngameUIManager.cs` の `LevelUpSelectAsync` メソッド内の以下の部分。

```csharp
public async Task<LevelUpgradeNodeViewModel> LevelUpSelectAsync(Memory<LevelUpgradeNodeViewModel> nodes)
{
    Debug.Log($"IngameUIManager: LevelUpSelectAsync が呼び出されました。選択肢の数: {nodes.Length}"); // 追加
    OpenLevelUpgradeWindow(nodes.Span);
    Debug.Log("IngameUIManager: OpenLevelUpgradeWindow を呼び出しました。"); // 追加

    // ... (既存のコード) ...

    CloseLevelUpgradeWindow();
    Debug.Log("IngameUIManager: CloseLevelUpgradeWindow を呼び出しました。"); // 追加
    return selectedNodeVM;
}
```

---
**完了レポート:**
`Assets/Scripts/Runtime/UI/Ingame/HUD/IngameUIManager.cs` ファイルの `LevelUpSelectAsync` メソッド内に、`LevelUpSelectAsync` の呼び出し、`OpenLevelUpgradeWindow` の呼び出し、`CloseLevelUpgradeWindow` の呼び出しを示すデバッグログを追加しました。これにより、これらのメソッドが正しく呼び出されているか、引数の情報が正しいかを確認できるようになりました。