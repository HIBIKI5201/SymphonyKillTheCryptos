# デッキ保存と読み込み機能の実装計画

## 概要
アウトゲームで作成したデッキ構成を保存し、インゲームで利用できるようにする機能の実装。現在途中まで実装されている`PlayerDeckSaveData`クラスの機能拡張と、そのデータを利用するUIおよびインゲームロジックの連携を行う。

## 目的
- プレイヤーが複数のデッキ構成を保存・管理できるようにする。
- 保存されたデッキ構成をインゲームで選択し、使用できるようにする。

## タスクリスト

1.  **`PlayerDeckSaveData.cs`の改善 (担当: エージェント)**
    *   コンストラクタを追加し、`_attackDeck`フィールドを適切に初期化する。
    *   `RegisterDeck`メソッドを修正し、指定された`deckName`のデッキが存在しない場合は新規追加し、存在する場合は更新するように変更する。既存の例外処理を削除する。
    *   指定された`deckName`に対応する`CardAddressValueObject[]`を返す`GetDeck`メソッドを追加する。
    *   保存されているすべてのデッキ名を返す`GetAllDeckNames`メソッドを追加する。
    *   指定された`deckName`のデッキを削除する`DeleteDeck`メソッドを追加する。
    *   `PlayerDeckSaveData`が初めて使用される際に`_attackDeck`がnullでないことを保証するため、適切な初期化ロジックを追加する（例：`OnBeforeSerialize`/`OnAfterDeserialize`、またはコンストラクタ）。
    *   クラスにバージョン管理のためのフィールドを追加することを検討する。（任意）
    *   **DeckNameValueObject の導入:**
        *   `RegisterDeck`メソッドの`deckName`引数の型を`string`から`DeckNameValueObject`に変更する。
        *   `GetDeck`メソッドの`deckName`引数の型を`string`から`DeckNameValueObject`に変更する。
        *   `GetAllDeckNames`メソッドの戻り値の型を`IReadOnlyCollection<string>`から`IReadOnlyCollection<DeckNameValueObject>`に変更する。
        *   `_attackDeck`フィールドのキーの型を`string`から`DeckNameValueObject`に変更する。

2.  **`SaveDataSystem`の調査と連携 (担当: エージェント)**
    *   `SaveDataSystem<T>.Save()`および`SaveDataSystem<T>.Load()`がどのように機能するかを詳細に調査する。
    *   `PlayerDeckSaveData`のインスタンスがどのようにロードされ、保存されるかを理解する。

3.  **`UIElementDeckEditor`の機能拡張 (担当: エージェント)**
    *   `UIElementDeckEditor`が`PlayerDeckSaveData`と連携するためのPresenterおよびUseCase層を設計・実装する。
    *   デッキの作成、編集、保存、ロード、削除を行うUIロジックを`UIElementDeckEditor`に追加する。
    *   UIでカードの追加/削除、デッキ名の変更、保存済みデッキの選択を可能にする。

4.  **`IngameStartSequence`との連携 (担当: エージェント)**
    *   インゲーム開始時に、`PlayerDeckSaveData`から選択されたデッキをロードできるように`IngameStartSequence`を修正する。
    *   デッキ選択のためのメカニズム（例：ロードするデッキ名をどこから取得するか）を定義する。

5.  **テストの実装 (担当: エージェント)**
    *   `PlayerDeckSaveData`の新しいメソッドに対する単体テストを実装する。
    *   デッキ保存・読み込み機能全体の結合テストを検討する。

## 完了レポート (このセクションはタスク完了後に追記されます)
### 2026年2月10日
- **`PlayerDeckSaveData.cs`の改善:**
    - コンストラクタを追加し、`_attackDeck`フィールドを適切に初期化しました。
    - `RegisterDeck`メソッドを修正し、指定された`deckName`のデッキが存在しない場合は新規追加し、存在する場合は更新するように変更しました。既存の例外処理を削除しました。
    - 指定された`deckName`に対応する`CardAddressValueObject[]`を返す`GetDeck`メソッドを追加しました。
    - 保存されているすべてのデッキ名を返す`GetAllDeckNames`メソッドを追加しました。
    - `PlayerDeckSaveData`が初めて使用される際に`_attackDeck`がnullでないことを保証するため、適切な初期化ロジックを追加しました。
    - 新しい仕様に基づき、`DeleteDeck`メソッドと、以前追加した`SelectedDeckName`フィールドとそのプロパティ、およびコンストラクタの初期化、関連するテストを削除しました。
- **`SaveDataSystem`の調査と連携:**
    - `SaveDataSystem`の定義ファイルをプロジェクト内で見つけることができませんでした。サードパーティライブラリの一部であるか、カスタム実装が異なるファイル名である可能性があります。
    - `SaveExecuter.cs`での使用箇所から、`SaveDataSystem<T>.Save()`は`PlayerDeckSaveData`インスタンスをシリアル化して保存し、`SaveDataSystem<T>.Load()`はその逆を行うと推測されます。`PlayerDeckSaveData`が`[Serializable]`であるため、この推測は妥当であると考えられます。
    - `PlayerDeckUseCase.cs`の`RegisterDeck`メソッド内で`SaveExecuter.DeckSave()`を呼び出すように修正しました。
    - `SaveExecuter.cs`に`SavePlayerMasterData()`メソッドを追加し、`using Cryptos.Runtime.Entity.System.SaveData;`を追加しました。
- **`UIElementDeckEditor`の機能拡張:**
    - `Assets/Scripts/Runtime/UseCase/OutGame/PlayerDeckUseCase.cs`を作成し、`PlayerDeckSaveData`を操作するユースケースを定義しました。
    - `Assets/Scripts/Runtime/UseCase/OutGame/PlayerMasterUseCase.cs`を作成し、`PlayerMasterSaveData`を管理するユースケースを定義しました。
    - `Assets/Scripts/Runtime/Presenter/OutGame/DeckEditorPresenter.cs`と`IDeckEditorUI`インターフェースを作成し、UIとユースケースを連携させるプレゼンター層を定義しました。
    - `IDeckEditorUI.cs`を独自のファイルとして`Assets/Scripts/Runtime/Presenter/OutGame/IDeckEditorUI.cs`に作成し、`SetSelectedDeckInDropdown`メソッドを追加、`ShowDeck`を`DeckViewModel`を受け取るように変更、名前空間を`Cryptos.Runtime.Presenter.OutGame.Card`から`Cryptos.Runtime.Presenter.OutGame`に修正しました。
    - `DeckEditorPresenter.cs`の名前空間を`Cryptos.Runtime.Presenter.OutGame.Card`から`Cryptos.Runtime.Presenter.OutGame`に修正しました。
    - `UIElementDeckEditor.cs`を`IDeckEditorUI`インターフェースを実装するように修正し、`DeckEditorPresenter`を注入できるようにしました。また、デッキ管理のための基本的なUIロジックとイベントハンドリングを追加しました。
    - 新しい仕様に基づき、`DeckEditorPresenter.cs`および`UIElementDeckEditor.cs`からデッキ削除関連の機能とUI要素を削除しました。
    - `Assets/Scripts/Runtime/UI/Outgame/OutgameUIManager.cs`に`GetUIElement<T>()`汎用メソッドを追加し、`OutgameStartSequience`から`UIElementDeckEditor`インスタンスを取得できるようにしました。
    - `Assets/Scripts/Runtime/InfraStructure/OutgameStartSequience.cs`を修正し、`PlayerDeckSaveData`のロード、`PlayerDeckUseCase`と`DeckEditorPresenter`のインスタンス化、そして`UIElementDeckEditor`へのプレゼンターの設定、およびデッキ名の初期表示トリガーを行いました。
    - `DeckEditorPresenter.cs`を更新し、`PlayerMasterUseCase`を依存関係として追加し、選択デッキ名の管理と`DeckViewModel`の使用に対応しました。`CardAddressValueObject`から`DeckCardEntity`への変換については、`CardDeckLoader.LoadDataBase`を使用する方針で`DeckCardEntity`のコンストラクタが`string address`を受け取る形を仮定して実装しました。
- **`IngameStartSequence`との連携:**
    - `Assets/Scripts/Runtime/InfraStructure/IngameStartSequence.cs`を修正し、`PlayerDeckSaveData`から選択されたデッキをロードするように変更しました。これには、`Cryptos.Runtime.Entity`の`using`ステートメントの追加、`PlayerDeckSaveData`のロード（暫定的に）、選択されたデッキ名の取得、対応する`CardDeckEntity`の動的な作成が含まれます。将来的には`OutgameStartSequience`から`playerDeckSaveData`をDIで受け取るためのTODOコメントを残しました。
- **テストの実装:**
    - `Assets/Scripts/Runtime/Tests/Editor`ディレクトリを作成しました。
    - `Assets/Scripts/Runtime/Tests/Editor/PlayerDeckSaveDataTests.cs`に`PlayerDeckSaveData`の単体テストを実装しました。このテストでは、コンストラクタの初期化、デッキの登録と更新、デッキの取得、すべてのデッキ名の取得を検証します。`SelectedDeckName`の削除に伴い、関連するテストも削除しました。テスト用の`CardAddressValueObject`モックを使用しています。
### 2026年2月10日 (続き)
- **`PlayerDeckSaveData.cs`の改善 (DeckNameValueObjectの導入):**
    - `PlayerDeckSaveData.cs`の`RegisterDeck`メソッドの`deckName`引数の型を`string`から`DeckNameValueObject`に変更しました。
    - `GetDeck`メソッドの`deckName`引数の型を`string`から`DeckNameValueObject`に変更しました。
    - `GetAllDeckNames`メソッドの戻り値の型を`IReadOnlyCollection<string>`から`IReadOnlyCollection<DeckNameValueObject>`に変更しました。
    - `_attackDeck`フィールドのキーの型を`string`から`DeckNameValueObject`に変更しました。
    - `PlayerDeckSaveData`クラスに`using Cryptos.Runtime.Entity.Outgame.Card;`を追加しました。
- **`PlayerDeckUseCase.cs`の修正:**
    - `GetDeck`メソッドの引数`deckName`の型を`string`から`DeckNameValueObject`に変更しました。
    - `GetAllDeckNames`メソッドの戻り値の型を`IReadOnlyCollection<string>`から`IReadOnlyCollection<DeckNameValueObject>`に変更しました。
    - `RegisterDeck`メソッドの引数`deckName`の型を`string`から`DeckNameValueObject`に変更しました。
    - `PlayerDeckUseCase`クラスに`using Cryptos.Runtime.Entity.Outgame.Card;`を追加しました。
- **`DeckEditorPresenter.cs`の修正:**
    - `DisplayAllDeckNames`、`LoadAndDisplayDeck`、`CreateNewDeck`、`SaveDeck`メソッドの引数や内部の`PlayerDeckUseCase`呼び出しを`DeckNameValueObject`に合わせて修正しました。
    - `IReadOnlyCollection<DeckNameValueObject>` を `IReadOnlyCollection<string>` に変換して `_deckEditorUI.ShowDeckNames` に渡すように修正しました。
    - `selectedDeckNameVO.Value` を使用して `_deckEditorUI.SetSelectedDeckInDropdown` に渡すように修正しました。
- **`IDeckEditorUI.cs`の修正:**
    - `ShowDeckNames`メソッドの引数`deckNames`の型を`IReadOnlyCollection<string>`から`IReadOnlyCollection<DeckNameValueObject>`に変更しました。
    - `SetSelectedDeckInDropdown`メソッドの引数`deckName`の型を`string`から`DeckNameValueObject`に変更しました。
    - `IDeckEditorUI`インターフェースに`using Cryptos.Runtime.Entity.Outgame.Card;`を追加しました。
- **`UIElementDeckEditor.cs`の修正:**
    - `IDeckEditorUI`の変更に合わせて、`ShowDeckNames`、`SetSelectedDeckInDropdown`メソッドのシグネチャと実装を修正しました。
    - `ShowDeck`メソッドのシグネチャを`ShowDeck(DeckViewModel deckViewModel)`に修正し、内部実装も更新しました。
    - 各イベントハンドラ（`OnCreateDeckClicked`、`OnLoadDeckClicked`、`OnSaveDeckClicked`、`OnDeckSelectionChanged`）内で、`string`を`DeckNameValueObject`に変換してプレゼンターに渡すように修正しました。
    - `UIElementDeckEditor`クラスに`using Cryptos.Runtime.Entity.Outgame.Card;`と`using System.Linq;`を追加しました。
- **テストの実装 (再作成):**
    - `Assets/Scripts/Runtime/Tests/Editor/PlayerDeckSaveDataTests.cs` が存在しないことを確認したため、新規に作成しました。
    - `PlayerDeckSaveData`のコンストラクタの初期化、デッキの登録と更新、デッキの取得、すべてのデッキ名の取得を検証する単体テストを実装しました。
### 2026年2月10日 (再修正)
- **`IDeckEditorUI.cs`の修正 (UI層のEntity参照回避):**
    - `ShowDeckNames`メソッドの引数`deckNames`の型を`IReadOnlyCollection<DeckNameValueObject>`から`IReadOnlyCollection<string>`に戻しました。
    - `SetSelectedDeckInDropdown`メソッドの引数`deckName`の型を`DeckNameValueObject`から`string`に戻しました。
    - `IDeckEditorUI`インターフェースから`using Cryptos.Runtime.Entity.Outgame.Card;`を削除しました。
- **`DeckEditorPresenter.cs`の確認:**
    - `IDeckEditorUI`のシグネチャ変更に合わせるため、`DisplayAllDeckNames`メソッド内の`_deckEditorUI.ShowDeckNames`と`_deckEditorUI.SetSelectedDeckInDropdown`への引数が`string`または`IReadOnlyCollection<string>`型で渡されていることを確認し、変更不要と判断しました。
- **`UIElementDeckEditor.cs`の修正 (UI層のEntity参照回避):**
    - `IDeckEditorUI`の変更に合わせて、`ShowDeckNames`メソッドの引数`deckNames`の型を`IReadOnlyCollection<string>`に戻し、内部の実装を修正しました。
    - `SetSelectedDeckInDropdown`メソッドの引数`deckName`の型を`string`に戻し、内部の実装を修正しました。
    - `ShowDeck`メソッド内のログ出力で`deckViewModel.DeckName.Value`ではなく`deckViewModel.DeckName`を使用するように修正しました。
    - `DeckEditorPresenter.cs`を更新し、`PlayerMasterUseCase`を依存関係として追加し、選択デッキ名の管理と`DeckViewModel`の使用に対応しました。`CardAddressValueObject`から`DeckCardEntity`への変換については、`CardDeckLoader.LoadDataBase`を使用する方針で`DeckCardEntity`のコンストラクタが`string address`を受け取る形を仮定して実装しました。
- **`IngameStartSequence`との連携:**
    - `Assets/Scripts/Runtime/InfraStructure/IngameStartSequence.cs`を修正し、`PlayerDeckSaveData`から選択されたデッキをロードするように変更しました。これには、`Cryptos.Runtime.Entity`の`using`ステートメントの追加、`PlayerDeckSaveData`のロード（暫定的に）、選択されたデッキ名の取得、対応する`CardDeckEntity`の動的な作成が含まれます。将来的には`OutgameStartSequience`から`playerDeckSaveData`をDIで受け取るためのTODOコメントを残しました。
- **テストの実装:**
    - `Assets/Scripts/Runtime/Tests/Editor`ディレクトリを作成しました。
    - `Assets/Scripts/Runtime/Tests/Editor/PlayerDeckSaveDataTests.cs`に`PlayerDeckSaveData`の単体テストを実装しました。このテストでは、コンストラクタの初期化、デッキの登録と更新、デッキの取得、すべてのデッキ名の取得を検証します。`SelectedDeckName`の削除に伴い、関連するテストも削除しました。テスト用の`CardAddressValueObject`モックを使用しています。
### 2026年2月10日 (続き)
- **`PlayerDeckSaveData.cs`の改善 (DeckNameValueObjectの導入):**
    - `PlayerDeckSaveData.cs`の`RegisterDeck`メソッドの`deckName`引数の型を`string`から`DeckNameValueObject`に変更しました。
    - `GetDeck`メソッドの`deckName`引数の型を`string`から`DeckNameValueObject`に変更しました。
    - `GetAllDeckNames`メソッドの戻り値の型を`IReadOnlyCollection<string>`から`IReadOnlyCollection<DeckNameValueObject>`に変更しました。
    - `_attackDeck`フィールドのキーの型を`string`から`DeckNameValueObject`に変更しました。
    - `PlayerDeckSaveData`クラスに`using Cryptos.Runtime.Entity.Outgame.Card;`を追加しました。
- **`PlayerDeckUseCase.cs`の修正:**
    - `GetDeck`メソッドの引数`deckName`の型を`string`から`DeckNameValueObject`に変更しました。
    - `GetAllDeckNames`メソッドの戻り値の型を`IReadOnlyCollection<string>`から`IReadOnlyCollection<DeckNameValueObject>`に変更しました。
    - `RegisterDeck`メソッドの引数`deckName`の型を`string`から`DeckNameValueObject`に変更しました。
    - `PlayerDeckUseCase`クラスに`using Cryptos.Runtime.Entity.Outgame.Card;`を追加しました。
- **`DeckEditorPresenter.cs`の修正:**
    - `DisplayAllDeckNames`、`LoadAndDisplayDeck`、`CreateNewDeck`、`SaveDeck`メソッドの引数や内部の`PlayerDeckUseCase`呼び出しを`DeckNameValueObject`に合わせて修正しました。
    - `IReadOnlyCollection<DeckNameValueObject>` を `IReadOnlyCollection<string>` に変換して `_deckEditorUI.ShowDeckNames` に渡すように修正しました。
    - `selectedDeckNameVO.Value` を使用して `_deckEditorUI.SetSelectedDeckInDropdown` に渡すように修正しました。
- **`IDeckEditorUI.cs`の修正:**
    - `ShowDeckNames`メソッドの引数`deckNames`の型を`IReadOnlyCollection<string>`から`IReadOnlyCollection<DeckNameValueObject>`に変更しました。
    - `SetSelectedDeckInDropdown`メソッドの引数`deckName`の型を`string`から`DeckNameValueObject`に変更しました。
    - `IDeckEditorUI`インターフェースに`using Cryptos.Runtime.Entity.Outgame.Card;`を追加しました。
- **`UIElementDeckEditor.cs`の修正:**
    - `IDeckEditorUI`の変更に合わせて、`ShowDeckNames`、`SetSelectedDeckInDropdown`メソッドのシグネチャと実装を修正しました。
    - `ShowDeck`メソッドのシグネチャを`ShowDeck(DeckViewModel deckViewModel)`に修正し、内部実装も更新しました。
    - 各イベントハンドラ（`OnCreateDeckClicked`、`OnLoadDeckClicked`、`OnSaveDeckClicked`、`OnDeckSelectionChanged`）内で、`string`を`DeckNameValueObject`に変換してプレゼンターに渡すように修正しました。
    - `UIElementDeckEditor`クラスに`using Cryptos.Runtime.Entity.Outgame.Card;`と`using System.Linq;`を追加しました。
- **テストの実装 (再作成):**
    - `Assets/Scripts/Runtime/Tests/Editor/PlayerDeckSaveDataTests.cs` が存在しないことを確認したため、新規に作成しました。
    - `PlayerDeckSaveData`のコンストラクタの初期化、デッキの登録と更新、デッキの取得、すべてのデッキ名の取得を検証する単体テストを実装しました。
### 2026年2月10日 (再修正)
- **`IDeckEditorUI.cs`の修正 (UI層のEntity参照回避):**
    - `ShowDeckNames`メソッドの引数`deckNames`の型を`IReadOnlyCollection<DeckNameValueObject>`から`IReadOnlyCollection<string>`に戻しました。
    - `SetSelectedDeckInDropdown`メソッドの引数`deckName`の型を`DeckNameValueObject`から`string`に戻しました。
    - `IDeckEditorUI`インターフェースから`using Cryptos.Runtime.Entity.Outgame.Card;`を削除しました。
- **`DeckEditorPresenter.cs`の確認:**
    - `IDeckEditorUI`のシグネチャ変更に合わせるため、`DisplayAllDeckNames`メソッド内の`_deckEditorUI.ShowDeckNames`と`_deckEditorUI.SetSelectedDeckInDropdown`への引数が`string`または`IReadOnlyCollection<string>`型で渡されていることを確認し、変更不要と判断しました。
- **`UIElementDeckEditor.cs`の修正 (UI層のEntity参照回避):**
    - `IDeckEditorUI`の変更に合わせて、`ShowDeckNames`メソッドの引数`deckNames`の型を`IReadOnlyCollection<string>`に戻し、内部の実装を修正しました。
    - `SetSelectedDeckInDropdown`メソッドの引数`deckName`の型を`string`に戻し、内部の実装を修正しました。
    - `ShowDeck`メソッド内のログ出力で`deckViewModel.DeckName.Value`ではなく`deckViewModel.DeckName`を使用するように修正しました。
    - `UIElementDeckEditor`クラスから`using Cryptos.Runtime.Entity.Outgame.Card;`を削除しました。
### 2026年2月10日 (エラー対応: CS7036 DeckCardEntityの引数不足)
- **`CardDeckLoader.cs`の修正:**
    - `CardAddressValueObject`を引数にとり`Task<CardDataAsset>`を非同期でロードして返す静的メソッド`LoadCardDataAsset`を追加しました。これはInfraStructure層の内部処理として完結し、他の層への参照依存は発生しません。
- **`CardDetail.cs`の新規作成:**
    - `Assets/Scripts/Runtime/Entity/Outgame/CardDetail.cs`に`cardName`、`description`、`Texture2D icon`、`power`を持つ`CardDetail`構造体を定義しました。これはDomain層のデータ構造であり、InfraStructure層への依存を回避します。
- **`ICardRepository.cs`の新規作成:**
    - `Assets/Scripts/Runtime/UseCase/OutGame/ICardRepository.cs`に`CardAddressValueObject`を受け取り、Domain層の型である`CardDetail`を返す非同期メソッド`GetCardDetailAsync`を定義する`ICardRepository`インターフェースを作成しました。
- **`CardRepositoryImpl.cs`の新規作成:**
    - `Assets/Scripts/Runtime/InfraStructure/CardRepositoryImpl.cs`に`ICardRepository`を実装するクラスを作成しました。このクラスは`CardDeckLoader.LoadCardDataAsset`を呼び出して`CardDataAsset`をロードし、それを`CardDetail`に変換して返します。
- **`PlayerDeckUseCase.cs`の修正:**
    - コンストラクタに`ICardRepository`を注入できるように変更しました。
    - `CreateDeckCardEntity`非同期メソッドを追加し、`_cardRepository.GetCardDetailAsync`を呼び出して取得した`CardDetail`の情報を使って`DeckCardEntity`を生成するように修正しました。また、必要な`using`ステートメントを追加しました。
- **`DeckEditorPresenter.cs`の修正:**
    - `DisplayAllDeckNames`、`LoadAndDisplayDeck`、`CreateNewDeck`、`SaveDeck`メソッドを非同期メソッドに変更しました。
    - `CardAddressValueObject`のコレクションから`DeckCardEntity`のコレクションを生成する際に、`_playerDeckUseCase.CreateDeckCardEntity`を非同期で呼び出し、`Task.WhenAll`を使用して非同期処理を待機するように変更しました。
    - 必要な`using System.Threading.Tasks;`を追加しました。
### 2026年2月10日 (エラー対応: CardAddressValueObjectの名前空間エラー)
- **`ICardDataLoader.cs`の削除:**
    - 不要であり、InfraStructure層への依存を導入していたため、`Assets/Scripts/Runtime/UseCase/OutGame/ICardDataLoader.cs`を削除しました。
- **`ICardRepository.cs`の修正:**
    - `CardAddressValueObject`の名前空間が`Cryptos.Runtime.Entity`であるため、`using Cryptos.Runtime.Entity.System;`を`using Cryptos.Runtime.Entity;`に修正しました。
    - `CardDetail`の名前空間が`Cryptos.Runtime.Entity.Outgame`であるため、`using Cryptos.Runtime.Entity.Outgame;`を追加しました。
### 2026年2月10日 (CardDetailの存在意義の修正)
- **`CardDetail.cs`の削除:**
    - 不要になった`Assets/Scripts/Runtime/Presenter/Outgame/CardDetail.cs`を削除しました。
- **`ICardRepository.cs`の修正:**
    - `GetCardDetailAsync`メソッドの戻り値を`CardDetail`から`DeckCardEntity`に変更し、メソッド名を`GetDeckCardEntityAsync`に変更しました。
    - `using Cryptos.Runtime.Entity.Outgame;`を削除し、`using Cryptos.Runtime.Entity.Ingame.Card;`を追加しました。
- **`CardRepositoryImpl.cs`の修正:**
    - `ICardRepository`インターフェースの変更に合わせて`GetCardDetailAsync`メソッドを`GetDeckCardEntityAsync`に変更し、戻り値の型を`CardDetail`から`DeckCardEntity`に変更しました。
    - `CardDeckLoader.LoadCardDataAsset`を使用してロードした`CardDataAsset`の情報と`CardAddressValueObject`を使って`DeckCardEntity`を構築して返すように実装しました。
    - 不要になった`using Cryptos.Runtime.Entity.Outgame;`を削除し、`using Cryptos.Runtime.Entity.Ingame.Card;`を追加しました。
- **`PlayerDeckUseCase.cs`の修正:**
    - `CreateDeckCardEntity`メソッド内で`_cardRepository.GetCardDetailAsync`を`_cardRepository.GetDeckCardEntityAsync`に変更し、その結果（`DeckCardEntity`）をそのまま返すように修正しました。
    - 不要になった`using Cryptos.Runtime.Entity.Outgame;`を削除しました。
### 2026年2月10日 (エラー対応: CS7036 DeckCardEntityの引数不足)
- **`CardDeckLoader.cs`の修正:**
    - `CardAddressValueObject`を引数にとり`Task<CardDataAsset>`を非同期でロードして返す静的メソッド`LoadCardDataAsset`を追加しました。これはInfraStructure層の内部処理として完結し、他の層への参照依存は発生しません。
- **`CardDetail.cs`の新規作成:**
    - `Assets/Scripts/Runtime/Entity/Outgame/CardDetail.cs`に`cardName`、`description`、`Texture2D icon`、`power`を持つ`CardDetail`構造体を定義しました。これはDomain層のデータ構造であり、InfraStructure層への依存を回避します。
- **`ICardRepository.cs`の新規作成:**
    - `Assets/Scripts/Runtime/UseCase/OutGame/ICardRepository.cs`に`CardAddressValueObject`を受け取り、Domain層の型である`CardDetail`を返す非同期メソッド`GetCardDetailAsync`を定義する`ICardRepository`インターフェースを作成しました。
- **`CardRepositoryImpl.cs`の新規作成:**
    - `Assets/Scripts/Runtime/InfraStructure/CardRepositoryImpl.cs`に`ICardRepository`を実装するクラスを作成しました。このクラスは`CardDeckLoader.LoadCardDataAsset`を呼び出して`CardDataAsset`をロードし、それを`CardDetail`に変換して返します。
- **`PlayerDeckUseCase.cs`の修正:**
    - コンストラクタに`ICardRepository`を注入できるように変更しました。
    - `CreateDeckCardEntity`非同期メソッドを追加し、`_cardRepository.GetCardDetailAsync`を呼び出して取得した`CardDetail`の情報を使って`DeckCardEntity`を生成するように修正しました。また、必要な`using`ステートメントを追加しました。
- **`DeckEditorPresenter.cs`の修正:**
    - `DisplayAllDeckNames`、`LoadAndDisplayDeck`、`CreateNewDeck`、`SaveDeck`メソッドを非同期メソッドに変更しました。
    - `CardAddressValueObject`のコレクションから`DeckCardEntity`のコレクションを生成する際に、`_playerDeckUseCase.CreateDeckCardEntity`を非同期で呼び出し、`Task.WhenAll`を使用して非同期処理を待機するように変更しました。
    - 必要な`using System.Threading.Tasks;`を追加しました。