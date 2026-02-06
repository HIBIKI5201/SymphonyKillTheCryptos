### **計画書**
**タイトル:** レベルアップUIが表示されない問題の修正（NullReferenceException対策）

**目的:**
`UIElementLevelUpgradeWindow.InputChar` メソッドで発生していた `NullReferenceException` の対策と、`_nodeContainer` が未初期化のまま `OpenWindow` が呼び出される可能性への対策を行う。

**作業内容:**
1.  `Assets/Docs/Task/` ディレクトリに新たな計画書ファイルを作成する。
2.  `Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs` ファイルの `OpenWindow` メソッドの冒頭に `_nodeContainer` が `null` であるか確認するガード句を追加する。
3.  `Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs` ファイルの `InputChar` メソッドの `foreach` ループ内で、`node` が `null` でないことを確認するガード句を追加する。
4.  `InputChar` メソッド内の非表示チェック後の `return;` を `continue;` に変更する。

**変更ファイル:**
*   `Assets/Docs/Task/レベルアップUIが表示されない問題の修正_NullReferenceException対策.md`
*   `Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs`

**変更箇所:**
`Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs` の `OpenWindow` メソッド内:

```csharp
public async ValueTask OpenWindow(Memory<LevelUpgradeNodeViewModel> nodes)
{
    if (_nodeContainer == null)
    {
        Debug.LogError("UIElementLevelUpgradeWindow: _nodeContainer is null in OpenWindow! UXML のロードまたは初期化に失敗しています。"); // エラーログを追加
        return; // NullReferenceException を防ぐための早期リターン
    }
    // ... 既存のコード ...
}
```

`Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs` の `InputChar` メソッド内:

```csharp
public void InputChar(char c)
{
    if (_nodes == null) { return; } // _nodes が null なら何もしない

    foreach (var node in _nodes)
    {
        if (node == null) { continue; } // node が null ならスキップ
        // 非表示なら更新しない。
        if (node.style.display == DisplayStyle.None) { continue; } // return から continue に変更

        node.OnInputChar(c);
    }
}
```

---
**完了レポート (後で追記):**

（作業完了後に追記します。）