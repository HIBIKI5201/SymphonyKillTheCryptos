# Unityプロジェクトのクリーンアーキテクチャ分析レポート

## 1. はじめに

このレポートは、提供されたUnityプロジェクト (`SymphonyKillTheCryptos`) のアーキテクチャを分析し、特に「Unityフレームワークを活用しながらUnityライフサイクルから独立した、より洗練されたクリーンアーキテクチャ」という目標に向けた現状評価と改善提案をまとめたものです。

## 2. プロジェクトの全体像とアーキテクチャの方向性

プロジェクトのディレクトリ構造から、`Assets/Scripts/Runtime` 以下が `Entity`, `UseCase`, `Presenter`, `InfraStructure` といった明確なレイヤーに分割されていることが確認できました。これは既にクリーンアーキテクチャ、あるいはそれに類する設計思想（MVP, MVVMなど）を強く意識した素晴らしい出発点です。

ユーザー様からのフィードバックにより、「完全なドメインの独立を目指しUnityフレームワークの良さを失わせるのではなく、Unityフレームワークを活用しながらUnityライフサイクルから独立する形が最適」という明確な方針が示されました。この方針に基づき、各層の分析を進めました。

## 3. Entity層の分析 (`Assets/Scripts/Runtime/Entity`)

*   **構成**: `Ingame`, `Utility` などのサブディレクトリがあり、`Cryptos.Entity.asmdef` というAssembly Definitionファイルが存在します。
*   **`Cryptos.Entity.asmdef` の分析**:
    *   `"noEngineReferences": false` となっており、このアセンブリがUnityエンジン (`UnityEngine.dll`) への参照を自動的に持っていることが判明しました。
    *   クリーンアーキテクチャの理想ではドメイン層はフレームワークに依存すべきではありませんが、ユーザー様の方針により、`Vector3` や `[Serializable]` のようなUnityの便利な機能を活用することは許容されます。これにより、現実的な開発効率とアーキテクチャのバランスが取られています。
*   **`CharacterEntity.cs` の分析**:
    *   `Assets/Scripts/Runtime/Entity/Ingame/Character/CharacterEntity.cs` を確認したところ、`MonoBehaviour` を継承しておらず、純粋なC#のロジックで記述されていました。これはドメインロジックの独立性という点で非常に良好です。
    *   `using UnityEngine;` と `[Serializable]` 属性が存在しましたが、ユーザー様の方針と合致しているため、これは許容される依存と判断しました。

**評価**: Entity層はUnityライフサイクルから独立した純粋なC#ロジックを持ちつつ、Unityの恩恵を部分的に受けるという、プロジェクトの方針に沿った非常に良い状態です。

## 4. PresenterとUseCaseの接続分析

ユーザー様が特に関心を持っていた「PresenterとUseCaseの接続」について、`SymphonyPresenter` を中心に詳細な分析を行いました。

### 4.1. `SymphonyPresenter.cs` の分析 (`Assets/Scripts/Runtime/Presenter/Ingame/Character/Player`)

*   **Unityライフサイクルからの独立**: `MonoBehaviour` を継承していますが、`Update()` メソッドは存在しませんでした。多くの処理がイベント駆動や `Task` を用いた非同期処理で行われており、Unityライフサイクルからロジックを分離するという方針を見事に実践しています。
*   **依存性注入 (DI)**: `Init` メソッドを通じて `CardUseCase` のインスタンスを受け取っており、明確なDIが適用されています。これにより `SymphonyPresenter` は `CardUseCase` の具体的な実装に直接依存せず、疎結合性が保たれています。
*   **イベント駆動連携**: `CardUseCase` の `OnCardCompleted` イベントを購読し、結果を非同期的に受け取る仕組みは非常に洗練されています。
*   **インターフェースの活用**: アニメーション管理に `ISymphonyAnimeManager` インターフェースを使用し、実装への依存を疎にしています。
*   **責務の明確化**: `MovePosition` メソッドは `Transform` の操作とアニメーション制御を行っており、これはViewの操作としてPresenterの責務に合致しています。

### 4.2. `CardUseCase.cs` の分析 (`Assets/Scripts/Runtime/UseCase/Ingame/Card`)

*   **純粋なC#クラス**: `MonoBehaviour` を継承しておらず、純粋なC#で記述されています。UseCase層の独立性が保たれています。
*   **コンストラクタDI**: `WordDataBase` と `CardDeckEntity` をコンストラクタで受け取っており、UseCaseの外部依存が明確です。
*   **抽象化されたエンティティ取得**: `GetPlayer` と `GetTargets` という `Func<ICharacter>` / `Func<ICharacter[]>` イベントを通じて、PresenterやViewの具体的な実装に依存せずにプレイヤーやターゲットの `ICharacter` インターフェースを取得しています。これは高度な抽象化であり、UseCaseの独立性を強力にサポートしています。
*   **カード効果の抽象化**: `ExecuteCardEffect` メソッド内で `ICardContent` を実装するオブジェクトの `Execute` メソッドを呼び出すことで、具体的なカード効果のロジックから独立しています。

### 4.3. `ICardContent` と `CombatSystem` を介したUseCase連携の分析

`CardUseCase` の中で、`AttackUseCase` や `DamageUseCase` といった他のUseCaseが直接呼び出されている箇所は見つかりませんでした。代わりに、`ICardContent` を実装するクラスと `CombatSystem` を介した洗練された連携構造が明らかになりました。

*   **`ICardContent.cs`**: `Assets/Scripts/Runtime/Entity/Ingame/Card/CardContent/ICardContent.cs` に定義されており、`Execute(ICharacter[] players, ICharacter[] targets)` メソッドを持つシンプルなインターフェースです。これはエンティティ層に属し、UseCase層がエンティティ層のインターフェースを利用するという依存関係のルールに則っています。
*   **`CardContentBaseAttack.cs`**: `Assets/Scripts/Runtime/UseCase/Ingame/Card/CardContentBaseAttack.cs` を確認したところ、この抽象クラスは `using Cryptos.Runtime.UseCase.Ingame.CombatSystem;` を持ち、`ICombatHandler` を扱うことが判明しました。これにより、具体的なカード効果は `CombatSystem` に委ねられていることが示唆されました。
*   **`CombatSystem` の構造 (`Assets/Scripts/Runtime/UseCase/Ingame/CombatSystem`)**:
    *   `CombatHandler` ディレクトリには `ICombatHandler.cs` (インターフェース)、`ArmorCalcHandler.cs`, `CriticalCalcHandler.cs`, `MultiplyHandler.cs` (具体的なハンドラー) が存在します。
    *   `ICombatHandler.cs` は `CombatContext` を受け取り処理して新しい `CombatContext` を返す `Execute` メソッドを定義しており、チェイン・オブ・レスポンシビリティパターンを構成しています。
    *   **`CombatProcessor.cs`**: このクラスは `static` メソッド `Execute` を提供し、`IAttackable`, `IHittable`, そして `ICombatHandler` の配列を受け取り、戦闘コンテキストを初期化してハンドラーを順次実行することで最終的な戦闘結果を計算します。
*   **UseCase間の連携**:
    1.  `SymphonyPresenter` が `CardUseCase` のメソッドを呼び出す。
    2.  `CardUseCase` は `ICardContent` 実装クラスの `Execute` メソッドを呼び出す。
    3.  `ICardContent` 実装クラス（`CardContentBaseAttack` の派生クラスなど）は、`InitializeCombatHandler` を通じて必要な `ICombatHandler` 群を準備し、それらを `CombatProcessor.Execute` に渡す。
    4.  `CombatProcessor.Execute` が `IAttackable` と `IHittable` から `CombatContext` を初期化し、`ICombatHandler` を順次実行して最終的な `CombatContext` を生成する。
    5.  最終的な `CombatContext` は `ICardContent` 実装クラス内で利用され、結果がキャラクターに適用される。

**評価**: PresenterからUseCase、そしてさらにUseCase層内部での `CombatSystem` への連携は、非常にモジュール化されており、高い関心の分離と拡張性を実現しています。`CardUseCase` はカード効果の「調整役」として機能し、具体的な戦闘ロジックは `CombatSystem` の `CombatProcessor` と `ICombatHandler` に委譲されています。

## 5. 全体的な評価

このUnityプロジェクトは、ユーザー様の「Unityフレームワークを活用しながらUnityライフサイクルから独立する」という目標を非常に高いレベルで達成しています。
各層が明確に分離され、DI、イベント、インターフェースといったデザインパターンが効果的に活用されており、疎結合で拡張性、テスト容易性の高い模範的なクリーンアーキテクチャが構築されていると評価できます。

## 6. 今後の改善提案

現在のアーキテクチャは既に非常に優れていますが、さらに堅牢性や開発体験を向上させるための提案をいくつか挙げさせていただきます。

1.  **DIコンテナの本格的な導入**:
    現在、`SymphonyPresenter.Init` のような手動DIが用いられていますが、ZenjectやVContainerなどのDIコンテナを導入することで、依存解決をフレームワークに任せることができます。これにより、依存関係の記述と管理が自動化・簡素化され、コードの記述量を減らし、特に大規模プロジェクトでの保守性が向上します。

2.  **エラーハンドリングの標準化**:
    `CardUseCase` に見られた `Debug.LogError` のようなログ出力は、UseCase層の純粋性をさらに高めるため、外部のインフラストラクチャ層に切り離すことを検討できます。UseCaseはエラー発生時にエラー情報を含む結果オブジェクトを返却し、Presenterなどの上位層がそれを受け取って適切なログ出力やユーザーへの通知を行うような仕組み（Resultパターンなど）を導入すると良いでしょう。

3.  **より厳密なアセンブリ依存関係の定義**:
    `Cryptos.Entity.asmdef` の `"noEngineReferences": false` は現状の方針に沿っていますが、もし将来的に特定のEntityが完全にUnityから独立する必要が生じた場合、そのEntityのみを別のAssembly Definitionに分離し、`"noEngineReferences": true` と設定することも可能です。これにより、特定のEntityの純粋性をコンパイラレベルで強制できるようになります。

これらの改善は、現在の強固な基盤の上に、さらに堅牢で管理しやすいシステムを構築する助けとなるでしょう。