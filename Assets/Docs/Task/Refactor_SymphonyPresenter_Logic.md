# SymphonyPresenter ロジック分離リファクタリング計画

## 目的

`SymphonyPresenter` が現在担当している、カード使用キューの管理と、それに伴うカード実行・アニメーション再生の制御ロジックを、`Entity` 層と `UseCase` 層に分離する。
`SymphonyPresenter` は `View` に関連する責務（アニメーションの実行、アニメーションイベントの中継）に専念させ、クリーンアーキテクチャの原則を強化し、コードの凝集度と保守性を向上させる。

## 計画

### ステップ1: CardExecutionQueueEntity の作成 (Entity層)

1.  `Assets/Scripts/Runtime/Entity/Ingame/Card/` に `CardExecutionQueueEntity.cs` を新規作成する。
2.  `private readonly Queue<CardEntity> _queue = new();` を内部に持つ。
3.  `Enqueue(CardEntity card)`, `TryDequeue(out CardEntity card)`, `TryPeek(out CardEntity card)`, `Clear()`, `Count` プロパティといった、Queueをラップするメソッドとプロパティを実装する。

### ステップ2: ICardAnimationHandler インターフェースの作成 (UseCase層)

1.  `Assets/Scripts/Runtime/UseCase/Ingame/Card/` に `ICardAnimationHandler.cs` を新規作成する。
2.  このインターフェースは、`UseCase` から `Presenter` のアニメーション機能を呼び出すための抽象層（ポート）となる。
3.  `void ActiveSkill(int animationClipID);` メソッドを定義する。
4.  `event Action<int> OnSkillTriggered;` と `event Action OnSkillEnded;` を定義し、`Presenter` から `UseCase` へアニメーションイベントを通知できるようにする。

### ステップ3: CardExecutionUseCase の作成 (UseCase層)

1.  `Assets/Scripts/Runtime/UseCase/Ingame/Card/` に `CardExecutionUseCase.cs` を新規作成する。
2.  **依存:** `CardUseCase`, `CardExecutionQueueEntity`, `ICardAnimationHandler` に依存し、コンストラクタで受け取る。
3.  **ロジックの移譲:**
    a. `SymphonyPresenter` の `HandleCardComplete`, `HandleSkillTriggered`, `HandleSkillEnded` のロジックをこのUseCaseに移譲する。
    b. `CardUseCase` の `OnCardCompleted` イベントを購読する `public void Initialize()` メソッドを実装する。
    c. `OnCardCompleted` のハンドラでは、`CardExecutionQueueEntity` にカードをエンキューし、キューの数が1つ以下なら `_cardAnimationHandler.ActiveSkill()` を呼び出す。
    d. `ICardAnimationHandler` の `OnSkillTriggered` イベントを購読し、ハンドラで `_cardUseCase.ExecuteCardEffect()` を呼び出す。
    e. `ICardAnimationHandler` の `OnSkillEnded` イベントを購読し、ハンドラで `CardExecutionQueueEntity` からカードをデキューし、次のカードがあれば `_cardAnimationHandler.ActiveSkill()` を呼び出す。
4.  `public void Reset()` メソッドを実装し、内部の `CardExecutionQueueEntity.Clear()` を呼び出す。
5.  `IDisposable` を実装し、`Dispose()` メソッドで全てのイベント購読を解除する。

### ステップ4: SymphonyPresenter の改修 (Presenter層)

1.  `_usingCardQueue` フィールドと、関連ロジック (`HandleCardComplete`, `HandleSkillTriggered`, `HandleSkillEnded`) を削除する。
2.  `ICardAnimationHandler` インターフェースを実装する。
    a. `ActiveSkill` メソッドは、内部の `_animeManager.ActiveSkill` を呼び出す実装とする。
    b. `OnSkillTriggered` と `OnSkillEnded` イベントは、`_animeManager` の同名イベントを中継して発火させる。
3.  `ResetUsingCard` メソッドの実装を、`CardExecutionUseCase` の `Reset()` メソッドを呼び出すように変更する。このため、`SymphonyPresenter` は `CardExecutionUseCase` に依存する。
4.  `Init()` メソッドから `cardUseCase.OnCardCompleted` の購読処理を削除する。

### ステップ5: DIコンテナ (IngameStartSequence) の修正

1.  `GameInitialize` メソッド内で `CardExecutionQueueEntity` と `CardExecutionUseCase` をインスタンス化する。
2.  `CardExecutionUseCase` の `Initialize()` を呼び出すために、インスタンス化の際に依存性を注入する。
    a. `CardExecutionUseCase` に `CardUseCase`, `CardExecutionQueueEntity`, そして `ICardAnimationHandler` の実装である `symphonyPresenter` を渡す。
3.  `SymphonyPresenter` に `CardExecutionUseCase` を注入する (`ResetUsingCard` のために必要)。
    -   `SymphonyPresenter` に `public void SetDependency(CardExecutionUseCase useCase)` のようなメソッドを追加する。
4.  `cardUseCase.OnCardCompleted` の購読処理を `SymphonyPresenter.Init` から `CardExecutionUseCase.Initialize` に移管するため、`IngameStartSequence` 内で `cardExecutionUseCase.Initialize()` を呼び出す。

---
## 完了レポート

2026年1月18日、計画書に記載されたすべてのリファクタリング作業を完了した。

### 実施内容

-   **Entity層:** `CardExecutionQueueEntity` を新設し、カード実行キューを管理するようにした。
-   **UseCase層:** `ICardAnimationHandler` インターフェースと `CardExecutionUseCase` を新設した。`CardExecutionUseCase` にカード使用キューのロジックとアニメーション制御のロジックを移譲した。`CardExecutionUseCase` は `ICardAnimationHandler` を介して `SymphonyPresenter` のアニメーション機能を呼び出し、イベントを購読する。
-   **Presenter層:** `SymphonyPresenter` からカード使用キュー管理ロジックを削除し、`ICardAnimationHandler` を実装するように修正。`CardExecutionUseCase` を注入するためのセッターメソッドを追加し、`ResetUsingCard` の実装を変更した。また、`Init` メソッドのシグネチャを修正し、不要な `CardUseCase` の参照を削除した。
-   **InfraStructure層:** `IngameStartSequence` で新しい `CardExecutionQueueEntity` と `CardExecutionUseCase` をインスタンス化し、`CardExecutionUseCase` の初期化と `SymphonyPresenter` への注入を行うように修正した。

### 結果

`SymphonyPresenter` が保持していたカード使用キュー管理とアニメーション制御のロジックが `UseCase` 層と `Entity` 層に適切に分離された。`SymphonyPresenter` は `View` の責務に専念するようになり、コードの凝集度と保守性が向上した。