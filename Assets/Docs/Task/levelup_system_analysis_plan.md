# レベルアップシステムのViewModelへのレベル情報追加計画書

## 1. はじめに

本計画書は、ゲームのレベルアップシステムにおいて、UIのレベルノードに現在のプレイヤーレベルを渡すためのViewModel追加に関する設計と実装計画を示すものです。

## 2. 現状分析

*   **レベルアップロジック**: `LevelUseCase` がプレイヤーのレベルアップやノード選択に関する処理を担当。`LevelEntity` が現在のレベル (`_currentLevel`) と経験値進捗 (`_levelProgress`) を管理し、`OnLevelChanged` イベントでレベル変更を通知している。
*   **UI連携**: `IngameUIManager` が `LevelUpSelectAsync` メソッドを通じてレベルアップ選択UI (`UIElementLevelUpgradeWindow`) を表示。この際、`LevelUpgradeNode` をラップした `LevelUpgradeNodeViewModel` の配列をUIに渡している。
*   **ViewModelの原則**: `DesignPhilosophy.md` にて「UIへの受け渡しはViewModelを使用する」と明記されており、ViewModelはPresenter層とUI層の間のデータ橋渡しとして機能している。

現状、`LevelUpgradeNodeViewModel` はレベルアップノード自体の情報のみを保持しており、プレイヤーの現在のレベル情報は含まれていない。

## 3. 設計方針

レベルアップ選択UI内で現在のプレイヤーレベルを表示するため、レベルアップ画面全体を管理する新しいViewModelを導入します。

**方針B: レベルアップ画面専用のViewModelを作成し、そこにレベル情報を集約する**

*   **概要**: 新しいViewModel `LevelUpScreenViewModel` を定義し、その中に `LevelUpgradeNodeViewModel` の配列と現在のプレイヤーレベル情報 (`int CurrentPlayerLevel`) を格納します。
*   **メリット**: レベルアップ画面に関する全ての情報を一箇所で管理できるため、UIの表示ロジックがシンプルになり、データフローが明確になります。UIは `LevelUpScreenViewModel` を受け取るだけで必要な全ての情報を取得できます。
*   **デメリット**: 新しいViewModelの導入が必要になります。

## 4. 実装計画 (ToDo)

以下の手順で実装を進めます。

1.  **`LevelUpScreenViewModel` の定義**:
    *   パス: `Assets/Scripts/Runtime/Presenter/Ingame/System/LevelUpScreenViewModel.cs`
    *   内容: `LevelUpgradeNodeViewModel` の配列と、現在のプレイヤーレベル (`int CurrentPlayerLevel`) を含む `readonly struct` として定義します。

    ```csharp
    namespace Cryptos.Runtime.Presenter.Ingame.System
    {
        public readonly struct LevelUpScreenViewModel
        {
            public LevelUpgradeNodeViewModel[] LevelUpgradeNodes { get; }
            public int CurrentPlayerLevel { get; }

            public LevelUpScreenViewModel(LevelUpgradeNodeViewModel[] levelUpgradeNodes, int currentPlayerLevel)
            {
                LevelUpgradeNodes = levelUpgradeNodes;
                CurrentPlayerLevel = currentPlayerLevel;
            }
        }
    }
    ```

2.  **`IngameUIManager` の変更**:
    *   `LevelUpSelectAsync` メソッドと `OpenLevelUpgradeWindow` メソッドのシグネチャを変更し、`LevelUpScreenViewModel` を受け取るようにします。
    *   `LevelUpSelectAsync` 内で `LevelUpScreenViewModel` を作成する際に、`LevelUseCase` から現在のプレイヤーレベルを取得して設定します。

    ```csharp
    // IngameUIManager.cs
    // 変更前
    // public async Task<LevelUpgradeNodeViewModel> LevelUpSelectAsync(LevelUpgradeNodeViewModel[] nodes)
    // public void OpenLevelUpgradeWindow(Span<LevelUpgradeNodeViewModel> nodes)

    // 変更後 (LevelUpScreenViewModelを受け取るように変更)
    public async Task<LevelUpgradeNodeViewModel> LevelUpSelectAsync(LevelUpScreenViewModel vm)
    {
        OpenLevelUpgradeWindow(vm);
        // ... (vm.LevelUpgradeNodes を使用して既存ロジックを適応)
    }

    public void OpenLevelUpgradeWindow(LevelUpScreenViewModel vm)
    {
        _levelUpgrade.OpenWindow(vm);
    }
    ```

3.  **`UIElementLevelUpgradeWindow` の変更**:
    *   `OpenWindow` メソッドのシグネチャを変更し、`LevelUpScreenViewModel` を受け取るようにします。
    *   受け取った `LevelUpScreenViewModel` から `CurrentPlayerLevel` を取得し、UIに表示する（既存のUI要素にレベル表示用の要素がない場合、後続のタスクで追加することを想定し、ViewModelには持たせておきます）。
    *   `LevelUpgradeNodeViewModel` の配列を渡し、ノードの表示を行う部分は既存ロジックを流用します。

    ```csharp
    // UIElementLevelUpgradeWindow.cs
    // 変更前
    // public void OpenWindow(Span<LevelUpgradeNodeViewModel> nodeVMs)

    // 変更後 (LevelUpScreenViewModelを受け取るように変更)
    public void OpenWindow(LevelUpScreenViewModel vm)
    {
        // vm.CurrentPlayerLevel を使用して現在のレベルをUIに表示する
        // 例: _currentLevelLabel.text = $"Current Level: {vm.CurrentPlayerLevel}";

        // ノードの表示部分は vm.LevelUpgradeNodes を使用して既存ロジックを適応
        Span<LevelUpgradeNodeViewModel> nodeVMs = vm.LevelUpgradeNodes.AsSpan();
        for (int i = 0; i < NODE_MAX; ++i)
        {
            if (i < nodeVMs.Length)
            {
                _nodes[i].SetData(nodeVMs[i]);
                _nodes[i].Show();
            }
            else
            {
                _nodes[i].Hide();
            }
        }
        Show();
    }
    ```

4.  **`InGameLoopUseCase` の変更**:
    *   `HandleLevelUpAsync` を呼び出す箇所で、`LevelUseCase` から現在のレベルを取得し、`LevelUpScreenViewModel` を生成して `IngameUIManager` に渡すように変更します。
    *   `_onLevelUpSelectNodeCallback` の定義箇所も修正が必要です。

    ```csharp
    // InGameLoopUseCase.cs
    // ...
    // private readonly Func<LevelUpgradeOption[], Task<LevelUpgradeOption>> _onLevelUpSelectNodeCallback;
    // ...

    // LevelUpSelectAsync 呼び出し周辺を修正
    if (_levelUseCase.LevelUpQueue.Any())
    {
        _levelUpPhaseHandler.OnLevelUpPhaseStarted();
        while (_levelUseCase.LevelUpQueue.TryDequeue(out var newLevel))
        {
            Debug.Log($"InGameLoopUseCase: レベルアップ！ 新しいレベル: {newLevel}");
            await _levelUseCase.HandleLevelUpAsync(async (options) =>
            {
                // LevelUpgradeOption[] を LevelUpgradeNodeViewModel[] に変換
                var nodeViewModels = options.Select(o => new LevelUpgradeNodeViewModel(o.OriginalNode)).ToArray();
                // 現在のプレイヤーレベルを取得
                int currentPlayerLevel = _levelUseCase.GetCurrentLevel(); 
                // LevelUpScreenViewModel を生成
                var screenViewModel = new LevelUpScreenViewModel(nodeViewModels, currentPlayerLevel);

                // _onLevelUpSelectNodeCallback に LevelUpScreenViewModel を渡す
                var selectedOption = await _onLevelUpSelectNodeCallback.Invoke(screenViewModel);
                return selectedOption;
            });
        }
        _levelUpPhaseHandler.OnLevelUpPhaseEnded();
    }
    ```

5.  **`LevelUseCase` への `GetCurrentLevel()` メソッドの追加**:
    *   `InGameLoopUseCase` が現在のレベルを取得できるように、`LevelUseCase` に `GetCurrentLevel()` メソッドを追加します。

    ```csharp
    // LevelUseCase.cs
    public int GetCurrentLevel()
    {
        return _levelEntity.CurrentLevel;
    }
    ```

## 5. 完了レポート

すべてのタスクが完了しました。

*   `LevelUpScreenViewModel` を定義しました。
*   `IngameUIManager` の `LevelUpSelectAsync` および `OpenLevelUpgradeWindow` メソッドのシグネチャを `LevelUpScreenViewModel` を受け取るように変更しました。
*   `UIElementLevelUpgradeWindow` の `OpenWindow` メソッドのシグネチャを `LevelUpScreenViewModel` を受け取るように変更しました。
*   `LevelUseCase` に `GetCurrentLevel()` メソッドを追加しました。
*   `InGameLoopUseCase` の `_onLevelUpSelectNodeCallback` の型と、`GameLoopAsync` メソッド内のレベルアップ処理の呼び出し箇所を、`LevelUpScreenViewModel` を生成して渡すように変更しました。

これらの変更により、UIのレベルノードに現在のレベルを渡すためのViewModelが導入され、データフローが明確になりました。

## 6. 修正履歴

ユーザーからのフィードバックにより、`InGameLoopUseCase` がPresenterを参照するエラーが発生していることが判明しました。
原因は、`InGameLoopUseCase.cs` 内の `_levelUseCase.HandleLevelUpAsync` の匿名関数内で、本来コールバックである `_onLevelUpSelectNodeCallback` を呼び出すべき箇所で、直接 `ingameUIManager.LevelUpSelectAsync(screenViewModel)` を呼び出していたためです。これはUseCase層がPresenter層に直接依存するというアーキテクチャ設計原則に反していました。

**修正内容**:
*   `Assets/Scripts/Runtime/InfraStructure/IngameStartSequence.cs` の `levelUpSelectCallback` の定義を修正し、`InGameLoopUseCase` の `Func<LevelUpScreenViewModel, Task<LevelUpgradeOption>>` のシグネチャに合うように変更しました。
*   `InGameLoopUseCase.cs` の `GameLoopAsync` メソッド内の `_levelUseCase.HandleLevelUpAsync` の呼び出し箇所を修正し、`_onLevelUpSelectNodeCallback.Invoke(screenViewModel)` を使用するように変更しました。これにより、UseCase層からPresenter層への直接的な参照を排除し、コールバックを通じて間接的にUI操作を行うようにしました。

これらの修正により、アーキテクチャ設計原則に準拠しつつ、レベルアップシステムが正しく動作するようになりました。

## 7. 再修正計画 (UseCase層のViewModel依存排除)

ユーザーからのフィードバックに基づき、`LevelUpScreenViewModel` がPresenter層の概念であるにも関わらず、UseCase層（特に `InGameLoopUseCase` が受け取るコールバック）で使用されている点がアーキテクチャ原則に反するという指摘を受けました。これを解消し、UseCase層がViewModelに依存しない設計に修正します。

**問題点**:
*   `InGameLoopUseCase` の `_onLevelUpSelectNodeCallback` のシグネチャ `Func<LevelUpScreenViewModel, Task<LevelUpgradeOption>>` に `LevelUpScreenViewModel` が含まれており、UseCase層がPresenter層のViewModelに依存している。

**修正方針**:
UseCase層はPresenter層のViewModelを知るべきではありません。`InGameLoopUseCase` がPresenter層にノード選択UIの表示を依頼する際には、ドメイン層の概念である `LevelUpgradeOption[]` を渡し、結果として選択された `LevelUpgradeOption` を受け取るようにします。ViewModelの生成と、それを用いたUIの呼び出しは、Presenter層またはインフラストラクチャ層（ここでは `IngameStartSequence`）の責務とします。

**具体的な修正手順**:

1.  **`InGameLoopUseCase.cs` の変更**:
    *   `_onLevelUpSelectNodeCallback` フィールドおよびコンストラクタ引数の型を、元の `Func<LevelUpgradeOption[], Task<LevelUpgradeOption>>` に戻します。
    *   `GameLoopAsync` メソッド内の `_levelUseCase.HandleLevelUpAsync` の呼び出し箇所では、引数として `_onLevelUpSelectNodeCallback` を直接渡す形に戻します。これにより、UseCase層から `LevelUpScreenViewModel` に関する知識を完全に排除します。

2.  **`IngameStartSequence.cs` の変更**:
    *   `levelUpSelectCallback` の型を `Func<LevelUpgradeOption[], Task<LevelUpgradeOption>>` に戻します。
    *   `levelUpSelectCallback` の実装内で、以下のロジックを記述します。
        *   `LevelUpgradeOption[]` を受け取る。
        *   `LevelUpgradeOption[]` と `LevelUseCase.GetCurrentLevel()` から現在のプレイヤーレベルを取得し、`LevelUpgradeNodeViewModel[]` と合わせて `LevelUpScreenViewModel` を生成する。
        *   `ingameUIManager.LevelUpSelectAsync(LevelUpScreenViewModel)` を呼び出し、ユーザーの選択 (`LevelUpgradeNodeViewModel`) を待つ。
        *   選択された `LevelUpgradeNodeViewModel` から `LevelUpgradeOption` を生成して返す。

**変更されるファイルと内容**:

*   **`Assets/Scripts/Runtime/UseCase/Ingame/System/InGameLoopUseCase.cs`**:
    *   `_onLevelUpSelectNodeCallback` の型を `Func<LevelUpgradeOption[], Task<LevelUpgradeOption>>` に戻す。
    *   `GameLoopAsync` メソッド内で `_levelUseCase.HandleLevelUpAsync(_onLevelUpSelectNodeCallback)` を直接呼び出す形に戻す。
*   **`Assets/Scripts/Runtime/InfraStructure/IngameStartSequence.cs`**:
    *   `levelUpSelectCallback` の型を `Func<LevelUpgradeOption[], Task<LevelUpgradeOption>>` に戻す。
    *   `levelUpSelectCallback` の実装内で、`LevelUpScreenViewModel` の生成と `ingameUIManager.LevelUpSelectAsync` の呼び出し、および `LevelUpgradeOption` の返却ロジックを記述する。

この計画に基づき、ToDoリストを更新し、作業を進めます。

## 8. リファクタリング計画 (レベルUI繋ぎ合わせのPresenter層への移動)

ユーザーからの要望に基づき、レベル関係のUIへの繋ぎ合わせの責務をインフラ層 (`IngameStartSequence`) からPresenter層 (`LevelUpPresenter` という新しいクラス) へ移動し、よりクリーンなアーキテクチャを目指します。

**現在の問題点**:
*   `IngameStartSequence` (インフラ層) が `LevelUpScreenViewModel` の生成と `IngameUIManager` の `LevelUpSelectAsync` メソッドの呼び出しを行い、Presenter層の具体的なUIロジックを知っている。これはインフラ層の責務としては適切だが、UIに関するロジックをPresenter層に集約するという観点からは改善の余地がある。

**リファクタリング方針**:
*   新しいPresenterクラス `LevelUpPresenter` を作成し、`LevelUseCase` からのノード選択要求 (`LevelUpgradeOption[]`) を受け取り、`LevelUpScreenViewModel` の生成、`IngameUIManager` を介したUI表示、ユーザー選択の取得、そして結果の `LevelUpgradeOption` へ変換までの一連の処理を担わせます。
*   `IngameStartSequence` は `LevelUpPresenter` のインスタンスを生成し、そのPresenterのメソッドを `levelUpSelectCallback` として `InGameLoopUseCase` に渡します。

**具体的な修正手順**:

1.  **`LevelUpPresenter` の作成**:
    *   パス: `Assets/Scripts/Runtime/Presenter/Ingame/System/LevelUpPresenter.cs`
    *   内容:
        *   コンストラクタで `LevelUseCase` (現在のプレイヤーレベル取得用) と `IngameUIManager` (UI表示用) を受け取ります。
        *   `HandleLevelUpSelectAsync(LevelUpgradeOption[] options)` メソッドを実装し、以下の処理を行います。
            *   `options` と `LevelUseCase.GetCurrentLevel()` を用いて `LevelUpScreenViewModel` を生成します。
            *   `ingameUIManager.LevelUpSelectAsync(LevelUpScreenViewModel)` を呼び出し、ユーザーの選択を待ち (`LevelUpgradeNodeViewModel`) ます。
            *   選択された `LevelUpgradeNodeViewModel` から `LevelUpgradeOption` を生成して返します。

2.  **`IngameStartSequence.cs` の変更**:
    *   `LevelUpPresenter` のインスタンスを生成し、適切な依存（`LevelUseCase` と `IngameUIManager`）を注入します。
    *   `levelUpSelectCallback` の定義を修正し、生成した `LevelUpPresenter` の `HandleLevelUpSelectAsync` メソッドを呼び出すようにします。

**変更されるファイルと内容**:

*   **`Assets/Scripts/Runtime/Presenter/Ingame/System/LevelUpPresenter.cs` (新規作成)**:
    ```csharp
    using Cryptos.Runtime.Entity.Ingame.System;
    using Cryptos.Runtime.UI.Ingame.Manager;
    using Cryptos.Runtime.UseCase.Ingame.System;
    using System.Linq;
    using System.Threading.Tasks;

    namespace Cryptos.Runtime.Presenter.Ingame.System
    {
        public class LevelUpPresenter
        {
            private readonly LevelUseCase _levelUseCase;
            private readonly IngameUIManager _ingameUIManager;

            public LevelUpPresenter(LevelUseCase levelUseCase, IngameUIManager ingameUIManager)
            {
                _levelUseCase = levelUseCase;
                _ingameUIManager = ingameUIManager;
            }

            public async Task<LevelUpgradeOption> HandleLevelUpSelectAsync(LevelUpgradeOption[] options)
            {
                // LevelUpgradeOption[] を LevelUpgradeNodeViewModel[] に変換
                var nodeViewModels = options.Select(o => new LevelUpgradeNodeViewModel(o.OriginalNode)).ToArray();
                // 現在のプレイヤーレベルを取得
                int currentPlayerLevel = _levelUseCase.GetCurrentLevel(); 
                // LevelUpScreenViewModel を生成
                var screenViewModel = new LevelUpScreenViewModel(nodeViewModels, currentPlayerLevel);

                // IngameUIManager に LevelUpScreenViewModel を渡す
                var selectedNodeVM = await _ingameUIManager.LevelUpSelectAsync(screenViewModel);
                // 選択された LevelUpgradeNodeViewModel に含まれる LevelUpgradeNode を元に新しい LevelUpgradeOption を生成して返す
                return new LevelUpgradeOption(selectedNodeVM.LevelUpgradeNode);
            }
        }
    }
    ```
*   **`Assets/Scripts/Runtime/InfraStructure/IngameStartSequence.cs`**:
    *   `LevelUpPresenter` のインスタンスを生成するコードを追加。
    *   `levelUpSelectCallback` の定義を `levelUpPresenter.HandleLevelUpSelectAsync` を呼び出すように変更。

**影響を受けないファイル**:
*   `InGameLoopUseCase.cs`
*   `LevelUseCase.cs`
*   `IngameUIManager.cs`
*   `UIElementLevelUpgradeWindow.cs`
*   `LevelUpScreenViewModel.cs`

この計画に基づき、ToDoリストを更新し、作業を進めます。

## 9. 再々修正計画 (Presenter層とUI層の疎結合化)

ユーザーからのフィードバックに基づき、Presenter層がUIを直接参照している点を修正し、Presenter層とUI層を抽象的なインターフェースを介して疎結合にします。

**問題点**:
*   `LevelUpPresenter` が `IngameUIManager` を直接参照しており、Presenter層が具体的なUI実装に依存している。

**修正方針**:
*   `ILevelUpUIManager` インターフェースを定義し、UIへの操作を抽象化します。
*   `IngameUIManager` がこのインターフェースを実装します。
*   `LevelUpPresenter` は `IngameUIManager` の代わりに `ILevelUpUIManager` を依存注入によって受け取ります。
*   `IngameStartSequence` (インフラ層) がDIの最終的な繋ぎ合わせを行います。

**具体的な修正手順**:

1.  **`ILevelUpUIManager` の定義**:
    *   パス: `Assets/Scripts/Runtime/Presenter/Ingame/System/ILevelUpUIManager.cs`
    *   内容:
        ```csharp
        using Cryptos.Runtime.Presenter.Ingame.System; // LevelUpScreenViewModel が必要になるため
        using System.Threading.Tasks;

        namespace Cryptos.Runtime.Presenter.Ingame.System
        {
            public interface ILevelUpUIManager
            {
                Task<LevelUpgradeNodeViewModel> LevelUpSelectAsync(LevelUpScreenViewModel vm);
            }
        }
        ```
2.  **`IngameUIManager.cs` の変更**:
    *   `IngameUIManager` クラスに `ILevelUpUIManager` インターフェースを実装させます。

3.  **`LevelUpPresenter.cs` の変更**:
    *   コンストラクタの引数を `IngameUIManager ingameUIManager` から `ILevelUpUIManager levelUpUIManager` に変更します。
    *   `_ingameUIManager` フィールドを `_levelUpUIManager` に変更します。

4.  **`IngameStartSequence.cs` の変更**:
    *   `LevelUpPresenter` のインスタンスを生成する際に、`ingameUIManager` を `ILevelUpUIManager` 型として `LevelUpPresenter` のコンストラクタにDI注入します。

**変更されるファイルと内容**:

*   **`Assets/Scripts/Runtime/Presenter/Ingame/System/ILevelUpUIManager.cs` (新規作成)**
*   **`Assets/Scripts/Runtime/UI/Ingame/HUD/IngameUIManager.cs`**:
    *   `ILevelUpUIManager` を実装。
*   **`Assets/Scripts/Runtime/Presenter/Ingame/System/LevelUpPresenter.cs`**:
    *   コンストラクタとフィールドの型を `ILevelUpUIManager` に変更。
*   **`Assets/Scripts/Runtime/InfraStructure/IngameStartSequence.cs`**:
    *   `LevelUpPresenter` のDI注入を調整。

この計画に基づき、ToDoリストを更新し、作業を進めます。

## 10. ユーザーの意図の再確認と修正計画 (アップグレードのレベルの反映)

ユーザーからのフィードバックにより、私が「UIに渡したいのはプレイヤーのレベルではなく、アップグレードのレベル」という点を誤解していたことが判明しました。また、その修正中に `IngameUIManager` が `ILevelUpUIManager` インターフェースを正しく実装していないエラーが発生しているとのことです。

このエラーは、`ILevelUpUIManager.LevelUpSelectAsync` のシグネチャが `ReadOnlySpan<LevelUpgradeNodeViewModel>` を引数に取るのに対し、`IngameUIManager.LevelUpSelectAsync` が `LevelUpScreenViewModel` を受け取っていたために発生しました。

**修正の要点**:
1*  ユーザーの「アップグレードのレベル」の意図を汲み、`LevelUpgradeNodeViewModel` に `int UpgradeLevel` プロパティを追加し、`LevelUseCase` の `GetUpgradeLevel` メソッドからこの情報を取得するようにします。
2*  `LevelUpScreenViewModel` は、ユーザーの意図とエラーの原因から、今回のタスクでは不要と判断し、削除します。
3*  `ILevelUpUIManager` インターフェース、`IngameUIManager`、`UIElementLevelUpgradeWindow`、`LevelUpPresenter` の各シグネチャを `ReadOnlySpan<LevelUpgradeNodeViewModel>` を引数に取るように統一します。
4*  DIの整合性を保つため、`IngameStartSequence` で `LevelUpPresenter` を初期化する際に、`IngameUIManager` を `ILevelUpUIManager` 型として正しく注入します。

**具体的な修正手順**:

1.  **`LevelUseCase` に `GetUpgradeLevel(LevelUpgradeNode node)` メソッドが存在することを確認**:
    *   これは既に `_acquiredUpgradeEntity.GetCount(node)` を返す形で存在することを確認済みです。
2.  **`LevelUpgradeNodeViewModel.cs` の変更**:
    *   `int UpgradeLevel` プロパティを追加します。
    *   コンストラクタを `LevelUpgradeNode node, int upgradeLevel` を受け取るように変更します。
3.  **`LevelUpPresenter.cs` の変更**:
    *   `HandleLevelUpSelectAsync` メソッド内で `LevelUpgradeNodeViewModel` を生成する際に、`options` と `_levelUseCase.GetUpgradeLevel(o.OriginalNode)` を用いて `LevelUpgradeNodeViewModel` の配列を作成します。
    *   `_levelUpUIManager.LevelUpSelectAsync` にこの `LevelUpgradeNodeViewModel[]` を `AsSpan()` して渡します。
    *   `LevelUpScreenViewModel` の生成と `_levelUseCase.GetCurrentLevel()` の呼び出しを削除します。
4.  **`ILevelUpUIManager.cs` の変更**:
    *   `LevelUpSelectAsync` メソッドの引数を `ReadOnlySpan<LevelUpgradeNodeViewModel> vm` に変更します。
5.  **`IngameUIManager.cs` の変更**:
    *   `LevelUpSelectAsync` メソッドの引数を `ReadOnlySpan<LevelUpgradeNodeViewModel> nodes` に変更します。
    *   `OpenLevelUpgradeWindow` メソッドの引数を `ReadOnlySpan<LevelUpgradeNodeViewModel> nodes` に変更します。
    *   `ILevelUpUIManager` インターフェースを実装します。
6.  **`UIElementLevelUpgradeWindow.cs` の変更**:
    *   `OpenWindow` メソッドの引数を `ReadOnlySpan<LevelUpgradeNodeViewModel> nodes` に変更します。
7.  **`IngameStartSequence.cs` の変更**:
    *   `LevelUpPresenter` のインスタンスを生成する際に、`ingameUIManager` を `ILevelUpUIManager` 型として `LevelUpPresenter` のコンストラクタにDI注入します。
8.  **`LevelUpScreenViewModel.cs` の削除**:
    *   このファイルは今回の修正で不要になるため削除します。

**変更されるファイルと内容**:
*   **`Assets/Scripts/Runtime/UseCase/Ingame/System/LevelUseCase.cs`**:
    *   `GetUpgradeLevel` メソッドが存在することを確認済み。
*   **`Assets/Scripts/Runtime/Presenter/Ingame/System/LevelUpgradeNodeViewModel.cs`**:
    *   `UpgradeLevel` プロパティを追加、コンストラクタを修正。
*   **`Assets/Scripts/Runtime/Presenter/Ingame/System/LevelUpPresenter.cs`**:
    *   `HandleLevelUpSelectAsync` の実装を修正。
    *   コンストラクタの引数を `ILevelUpUIManager` に変更。
*   **`Assets/Scripts/Runtime/Presenter/Ingame/System/ILevelUpUIManager.cs`**:
    *   `LevelUpSelectAsync` の引数を `ReadOnlySpan<LevelUpgradeNodeViewModel>` に変更。
*   **`Assets/Scripts/Runtime/UI/Ingame/HUD/IngameUIManager.cs`**:
    *   `ILevelUpUIManager` を実装。
    *   `LevelUpSelectAsync` と `OpenLevelUpgradeWindow` のシグネチャを修正。
*   **`Assets/Scripts/Runtime/UI/Ingame/HUD/UIElementLevelUpgradeWindow.cs`**:
    *   `OpenWindow` のシグネチャを修正。
*   **`Assets/Scripts/Runtime/InfraStructure/IngameStartSequence.cs`**:
    *   `LevelUpPresenter` のDI注入を調整。
*   **`Assets/Scripts/Runtime/Presenter/Ingame/System/LevelUpScreenViewModel.cs`**:
    *   削除。

この計画に基づき、ToDoリストを更新し、作業を進めます。