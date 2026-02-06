### **計画書**
**タイトル:** レベルアップUIが表示されない問題の原因究明（フェーズ4：UIElementLevelUpgradeWindowの初期化追跡）

**目的:**
`UIElementLevelUpgradeWindow.cs` の `OpenWindow` メソッド内で `_nodes` 配列が正しく初期化され、`UIElementLevelUpgradeNode` インスタンスが追加されているかを確認する。

**作業内容:**
1.  `Assets/Docs/Task/` ディレクトリに新たな計画書ファイルを作成する。
2.  `Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs` ファイルの `OpenWindow` メソッド内にデバッグログを追加し、`_nodes` 配列の初期化と要素の追加状況を追跡できるようにする。

**変更ファイル:**
*   `Assets/Docs/Task/レベルアップUIが表示されない問題の原因究明_フェーズ4.md`
*   `Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs`

**ログ追加箇所:**
`Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs` の `OpenWindow` メソッド内の以下の部分。

```csharp
public void OpenWindow(ReadOnlySpan<LevelUpgradeNodeViewModel> nodes)
{
    Debug.Log($"UIElementLevelUpgradeWindow: OpenWindow が呼び出されました。ノードの数: {nodes.Length}"); // 追加
    _nodes = new UIElementLevelUpgradeNode[NODE_MAX];
    Debug.Log($"UIElementLevelUpgradeWindow: _nodes 配列を初期化しました。サイズ: {NODE_MAX}"); // 追加

    for (int i = 0; i < NODE_MAX; i++)
    {
        if (nodes.Length <= i) { break; }

        UIElementLevelUpgradeNode node = new();
        LevelUpgradeNodeViewModel nodeVM = nodes[i];

        node.SetData(nodeVM);

        _nodeContainer.Add(node);
        _nodes[i] = node;
        Debug.Log($"UIElementLevelUpgradeWindow: ノード {i} を追加しました。ノード名: {nodeVM.NodeName}"); // 追加
    }

    style.display = DisplayStyle.Flex;
    Debug.Log("UIElementLevelUpgradeWindow: ウィンドウを表示しました。"); // 追加
}
```

---
**完了レポート (後で追記):**

（作業完了後に追記します。）