## 計画書: ArgumentException の原因調査

### 1. 問題の特定

`Cryptos.Runtime.UI.Ingame.Manager.IngameUIManager.RegisterComboCountHandler` メソッド内で `ArgumentException` が発生している。
エラー発生箇所: `Assets/Scripts/Runtime/UI/Ingame/HUD/IngameUIManager.cs:143`

### 2. 調査の目的

`ArgumentException` が発生する具体的な原因を特定し、解決策を検討するための情報を収集する。

### 3. 調査手順

1. **該当コードの確認:** `Assets/Scripts/Runtime/UI/Ingame/HUD/IngameUIManager.cs` の143行目付近のコードを読み込み、`RegisterComboCountHandler` メソッドの内容を確認する。特に、引数 `ComboViewModel vm` の使用方法に注目する。
2. **呼び出し元の確認:** スタックトレースから、`IngameUIManager.RegisterComboCountHandler` が `IngameStartSequence.GameInitialize` から呼ばれていることを確認。`Assets/Scripts/Runtime/InfraStructure/IngameStartSequence.cs:172` も確認し、どのような `ComboViewModel` が渡されているかを調査する。
3. **`ComboViewModel` の確認:** `ComboViewModel` の定義を検索し、その構造と値の生成方法を理解する。
4. **`ArgumentException` の発生条件の推測:** 上記の情報から、どのような条件で `ArgumentException` が発生するのかを推測する。例えば、`null` の引数が渡されている、期待される範囲外の値が渡されているなど。

### 4. レポート内容

調査結果を以下の項目でまとめる。

- エラーの概要
- 関連コードスニペット
- エラーの原因と推測される条件
- 提案される解決策 (もし特定できれば)

### 5. 完了報告

調査完了後、本計画書に完了レポートを追記する。
