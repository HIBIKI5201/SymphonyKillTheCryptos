### **計画書**
**タイトル:** レベルアップUIが表示されない問題の原因究明（フェーズ5：_nodeContainerのNullチェックとUIElementLevelUpgradeNode.InputCharの保護）

**目的:**
`UIElementLevelUpgradeWindow.OpenWindow` メソッド内で `_nodeContainer` が `null` でないことを確認し、また `UIElementLevelUpgradeWindow.InputChar` メソッドで `_nodes` 配列内の要素が `null` でないことを保証することで、`NullReferenceException` を解決する。

**作業内容:**
1.  `Assets/Docs/Task/` ディレクトリに新たな計画書ファイルを作成する。
2.  `Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs` ファイルの `OpenWindow` メソッドの冒頭に `_nodeContainer` が `null` であるか確認するログを追加する。
3.  `Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs` ファイルの `InputChar` メソッドの `foreach` ループ内で、`node` が `null` でないことを確認するガード句を追加する。
4.  `InputChar` メソッド内の非表示チェック後の `return;` を `continue;` に変更する。

**変更ファイル:**
*   `Assets/Docs/Task/レベルアップUIが表示されない問題の原因究明_フェーズ5.md`
*   `Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs`

**ログ追加箇所と変更箇所:**
`Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs` の `OpenWindow` メソッド内:

```csharp
public void OpenWindow(ReadOnlySpan<LevelUpgradeNodeViewModel> nodes)
{
    if (_nodeContainer == null)
    {
        Debug.LogError("UIElementLevelUpgradeWindow: _nodeContainer is null in OpenWindow!"); // 追加
        return; // Early exit to prevent NullReferenceException
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