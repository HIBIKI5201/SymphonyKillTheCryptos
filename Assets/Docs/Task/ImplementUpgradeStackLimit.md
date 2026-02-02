# レベルアップ強化アイテムのスタック上限機能 実装計画書 (改訂版)

## 概要

レベルアップ時に取得する強化アイテム（LevelUpgradeNode）に、取得回数の上限（スタック上限）を設ける機能を実装する。

## 課題

当初の実装では、`TentativeCharacterData` が強化アイテムの取得回数を管理しており、キャラクターのデータ管理という責務を超えてレベルアップシステムの詳細に依存してしまっていた。これは関心の分離の原則に反するため、設計を修正する必要がある。

## 設計方針 (改訂)

1.  **`LevelUpgradeNode` に上限値プロパティを追加**
    *   `LevelUpgradeNode.cs` に、最大取得可能回数を定義する `MaxStack` プロパティを追加する。これは変更なし。

2.  **取得済みアイテムの回数記録責務の移管**
    *   `TentativeCharacterData.cs` から、取得済みアイテムの回数記録機能を **削除** する。
    *   レベルアップシステムのビジネスロジックを担当する `LevelUseCase.cs` が、取得済みの `LevelUpgradeNode` とその回数を記録するための `Dictionary<LevelUpgradeNode, int>` を保持する。

3.  **レベルアップ選択肢のフィルタリング機能の修正**
    *   `LevelUseCase.cs` の `WaitLevelUpSelectAsync` メソッドは、自身の持つ `Dictionary` を参照し、取得回数が `MaxStack` に達しているアイテムを選択肢から除外する。

4.  **取得アイテムの記録処理の修正**
    *   `LevelUseCase.cs` の `HandleLevelUpAsync` メソッド内で、プレイヤーが選択したアイテムを自身の `Dictionary` に記録（カウントを更新）する。

この修正により、`TentativeCharacterData` は純粋なデータホルダーとしての責務に集中し、`LevelUseCase` がレベルアップに関する状態管理とロジックの全責任を負うことになり、よりクリーンな設計となる。

## タスクリスト (改訂)

-   [x] `Assets/Scripts/Runtime/Entity/Ingame/System/LevelUpgradeNode.cs` の修正 (変更なし)
    -   [x] `_maxStack` フィールドの追加
    -   [x] `MaxStack` プロパティの追加
-   [x] `Assets/Scripts/Runtime/Entity/Ingame/Character/TentativeCharacterData.cs` の修正 (リファクタリング)
    -   [x] `_acquiredUpgrades` (Dictionary) フィールドを削除
    -   [x] `GetAcquiredCount` メソッドを削除
    -   [x] `AddAcquiredNode` メソッドを削除
-   [x] `Assets/Scripts/Runtime/UseCase/Ingame/System/LevelUseCase.cs` の修正 (リファクタリング)
    -   [x] `_acquiredUpgrades` (Dictionary) フィールドを追加
    -   [x] `WaitLevelUpSelectAsync` メソッドのフィルタリング処理を、自身の `_acquiredUpgrades` を参照するように修正
    -   [x] `HandleLevelUpAsync` メソッドの記録処理を、自身の `_acquiredUpgrades` を更新するように修正
-   [ ] 動作確認
    -   [ ] 特定の `LevelUpgradeNode` アセットの `MaxStack` を少ない回数（例: 1）に設定する。
    -   [ ] ゲームをプレイし、レベルアップを繰り返す。
    -   [ ] 一度取得したアイテムが、次以降のレベルアップ選択肢に表示されなくなることを確認する。
    -   [ ] `MaxStack` が未設定（または大きい値）のアイテムは、複数回表示されることを確認する。

## 完了レポート (改訂)

ユーザーからの設計に関する指摘を受け、リファクタリングを実施しました。

-   `TentativeCharacterData.cs` からレベルアップ関連のロジックを削除し、責務を純粋なキャラクターデータ管理に戻しました。
-   `LevelUseCase.cs` に取得済みアップグレードの管理機能を集約し、レベルアップに関する状態とロジックを自己完結させました。
-   `LevelUpgradeNode.cs` の変更はそのままに、新しい設計に適合するよう各クラスを修正しました。

これにより、SOLIDの原則に、より準拠したクリーンな設計になりました。
残りのタスクは動作確認です。
