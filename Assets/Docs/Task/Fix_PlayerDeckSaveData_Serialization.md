# PlayerDeckSaveDataシリアライズ問題修正計画書

## 概要
`PlayerDeckSaveData`が`SaveDataSystem`を通じて保存・ロードできない問題を修正する。問題の原因は、`PlayerDeckSaveData`内で使用されているカスタムの`ValueObject`(`DeckNameValueObject`と`CardAddressValueObject`)がJson.NETで適切にシリアライズ・デシリアライズできない形式になっていることである。

## 問題点
- `PlayerDeckSaveData`内の`Dictionary<DeckNameValueObject, CardAddressValueObject[]>`がJson.NETで正しくシリアライズ・デシリアライズできない。
- `DeckNameValueObject`および`CardAddressValueObject`は`readonly struct`であり、privateな`_value`フィールドと引数付きコンストラクタのみを持つため、Json.NETがデシリアライズ時にインスタンスを再構築できない。

## 修正内容
1.  **`DeckNameValueObject.cs`の変更**:
    - `Newtonsoft.Json`名前空間を追加する。
    - コンストラクタに`[JsonConstructor]`属性を付与する。
    - `Value`プロパティに`[JsonProperty("Value")]`属性を付与する。

2.  **`CardAddressValueObject.cs`の変更**:
    - `Newtonsoft.Json`名前空間を追加する。
    - コンストラクタに`[JsonConstructor]`属性を付与する。
    - `Value`プロパティに`[JsonProperty("Value")]`属性を付与する。

これらの修正により、Json.NETが`DeckNameValueObject`と`CardAddressValueObject`の値を正しく読み書きできるようになり、`PlayerDeckSaveData`全体のシリアライズ・デシリアライズが可能になるはずである。

## 実施手順
1. 本計画書を`Assets\Docs\Task\Fix_PlayerDeckSaveData_Serialization.md`に作成。
2. `DeckNameValueObject.cs`を修正。
3. `CardAddressValueObject.cs`を修正。
4. 修正後、本計画書に完了レポートを追記。

## 懸念事項
- `SaveDataSystem`内で`JsonConvert.SerializeObject`が使用されているため、`Newtonsoft.Json`がプロジェクトに導入されている必要がある。

## 完了レポート
- `DeckNameValueObject.cs`に`using Newtonsoft.Json;`を追加し、コンストラクタに`[JsonConstructor]`、`Value`プロパティに`[JsonProperty("Value")]`属性を付与しました。
- `CardAddressValueObject.cs`に`using Newtonsoft.Json;`を追加し、コンストラクタに`[JsonConstructor]`、`Value`プロパティに`[JsonProperty("Value")]`属性を付与しました。
- これにより、`PlayerDeckSaveData`内で使用されている`DeckNameValueObject`と`CardAddressValueObject`がJson.NETで適切にシリアライズ・デシリアライズできるようになり、`SaveDataSystem`を通じて`PlayerDeckSaveData`の保存・ロードが可能になるはずです。