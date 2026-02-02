# システム設計リファクタリング計画書

## 概要

システムの疎結合性と拡張性を向上させるため、以下の2つのリファクタリングを実施する。

1.  **レベルアップグレード記録のEntity分離**: `LevelUseCase` が保持しているアップグレード取得記録を、専門のEntityクラスに分離する。
2.  **Modifierクラスの導入**: `TentativeCharacterData` のステータス変更ロジックを、プリミティブな値ではなく `Modifier` オブジェクトを介して行うように修正する。

---

## 1. レベルアップグレード記録のEntity分離

### 課題

`LevelUseCase` が、アップグレードの取得記録（状態）と、それを用いたレベルアップ処理（ロジック）の両方の責務を持っている。状態とロジックを分離することで、各クラスの責務がより明確になる。

### 設計方針

1.  **`AcquiredUpgradeEntity` の作成**:
    *   `Assets/Scripts/Runtime/Entity/Ingame/System/` に `AcquiredUpgradeEntity.cs` を新規作成する。
    *   このクラスは `private readonly Dictionary<LevelUpgradeNode, int> _acquiredUpgrades` を内部に持ち、取得済みアップグレードの状態を管理する。
    *   `public int GetCount(LevelUpgradeNode node)` と `public void Add(LevelUpgradeNode node)` メソッドを公開し、外部からはこれらのメソッドを通してのみ状態を変更できるようにする。

2.  **`LevelUseCase` の修正**:
    *   `LevelUseCase` は `Dictionary` を直接持たず、代わりに `private readonly AcquiredUpgradeEntity _acquiredUpgradeEntity` のインスタンスを保持する。
    *   アップグレードの取得回数確認と記録は、すべて `_acquiredUpgradeEntity` の公開メソッドを介して行うように修正する。

---

## 2. Modifierクラスの導入

### 課題

現在、ステータスへの変更（バフなど）は `SetNewBuff(BuffType type, float value, int priority)` のように、`float` と `int` というプリミティブな値で渡されている。
これを `Modifier` という概念を表現するクラスでラップすることにより、どのような種類の変更なのかがより明確になり、コードの可読性と拡張性が向上する。

### 設計方針

1.  **`StatModifier` クラスの作成**:
    *   `Assets/Scripts/Runtime/Entity/Utility/` に `StatModifier.cs` を新規作成する。
    *   変更の種類（加算/乗算など）を示す `StatModType` (enum) を定義する。
    *   `Type`, `Value`, `Priority` などのプロパティを持つイミュータブルなデータクラスとして設計する。

2.  **`DynamicFloatVariable` の改修**:
    *   `AddMultiplier` や `AddAdditive` といった個別のメソッドの代わりに、`public void AddModifier(StatModifier modifier)` という単一の窓口を設ける。
    *   `RemoveModifier(StatModifier modifier)` も同様に実装する。
    *   `AddModifier` の内部で、`modifier.Type` に応じて既存の加算/乗算リストに振り分ける処理を行う。

3.  **`TentativeCharacterData` の改修**:
    *   `SetNewBuff` メソッドのシグネチャを `public void SetNewBuff(BuffType type, StatModifier modifier)` に変更する。
    *   `SetNewBuff` の内部では、対象となる `DynamicFloatVariable` の `AddModifier` メソッドを呼び出す。

4.  **`LevelUpgradeStatusEffect` 等の改修**:
    *   `ILevelUpgradeEffect` を実装するすべてのクラス（`AttackStatusUpgrade` など）を修正する。
    *   `ApplyStatusEffect` メソッド内で、`new StatModifier(...)` のように `StatModifier` インスタンスを生成し、`characterData.SetNewBuff` に渡すように処理を変更する。

---

## タスクリスト

### Task 1: Entity分離

-   [x] `Assets/Scripts/Runtime/Entity/Ingame/System/AcquiredUpgradeEntity.cs` の新規作成
-   [x] `LevelUseCase.cs` の修正
    -   [x] `_acquiredUpgrades` Dictionary を削除
    -   [x] `_acquiredUpgradeEntity` フィールドを追加
    -   [x] `WaitLevelUpSelectAsync` と `HandleLevelUpAsync` を `_acquiredUpgradeEntity` を使うように修正

### Task 2: Modifier導入

-   [x] `Assets/Scripts/Runtime/Entity/Utility/StatModifier.cs` の新規作成
-   [x] `Assets/Scripts/Runtime/Entity/Utility/DynamicFloatVariable.cs` の修正
    -   [x] `AddModifier` メソッドの追加
    -   [x] `RemoveModifier` メソッドの追加
-   [x] `Assets/Scripts/Runtime/Entity/Ingame/Character/TentativeCharacterData.cs` の修正
    -   [x] `SetNewBuff` メソッドのシグネチャと実装を `StatModifier` を使うように変更
-   [x] `ILevelUpgradeEffect` を実装するクラス群の修正
    -   [x] `LevelUpgradeStatusEffect.cs` の修正
    -   [x] `AttackStatusUpgrade.cs`, `HealthStatusUpgrade.cs` などの具象クラスで `StatModifier` を生成するように修正
-   [ ] 動作確認

## 完了レポート

計画書に記載した2つのリファクタリングタスクを完了しました。

1.  **レベルアップグレード記録のEntity分離**:
    `LevelUseCase`が保持していたアップグレード取得記録の状態を、新しい`AcquiredUpgradeEntity`クラスに分離しました。これにより、`LevelUseCase`はロジックに集中し、状態管理の責務が明確になりました。

2.  **Modifierクラスの導入**:
    ステータス変更の仕組みを、プリミティブな値を渡す方式から`StatModifier`オブジェクトを渡す方式にリファクタリングしました。`StatModifier`、`DynamicFloatVariable`、`TentativeCharacterData`、および各種`...StatusUpgrade`クラスを修正し、より堅牢で拡張性の高い設計を実現しました。

すべてのコード修正は完了しており、残るは動作確認のみです。
