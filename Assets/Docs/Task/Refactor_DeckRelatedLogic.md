# デッキ関連ロジックのリファクタリング計画書

## 概要
デッキ関連のロジック、特に`DeckEditorPresenter`の初期化とデータ管理をリファクタリングし、各メソッドの責務を明確にする。

## 問題点
- `DeckEditorPresenter.InitializeDeckEditor()`メソッドが複数の責務（ロールUI設定、所持カード初期化、デッキカード初期化）を担っている。
- `_currentDeckCards`の初期化が暫定的なロジック（`_ownedCards`の最初の3枚）で行われている。本来は`PlayerDeckUseCase`から保存済みデッキ情報をロードすべきである。
- （リバート対象）`CardViewModel`がUseCase層の`PlayerDeckUseCase`で参照され、アーキテクチャの責務が違反されていた。

## 修正内容

### 1. `DeckEditorPresenter.cs`の変更
- `InitializeDeckEditor()`メソッドを以下のように分割する。
    - `InitializeDeckEditor()`: 公開APIとしての初期化起点メソッド。内部でプライベートな初期化メソッドを呼び出す。
    - `private void InitializeRoleUI()`: ロール関連のUI設定のみを行う。
    - `private void InitializeOwnedCards()`: `_allCard`から`_ownedCards`を初期化する。
    - `private void InitializeDeckCards()`: `PlayerDeckUseCase`から保存済みデッキのアドレス (`CardAddressValueObject[]`) を取得し、それを元に`_allCard`から`DeckCardEntity`を抽出し、`CardViewModel`に変換して`_currentDeckCards`を初期化する。
- `InitializeDeckEditor()`メソッド自体は`void`のままとする。

### 2. `PlayerDeckUseCase.cs`の変更
- `PlayerDeckUseCase`のコンストラクタから`PlayerMasterUseCase`を削除（リバート）。
- `PlayerDeckUseCase`に、ユーザーの保存済みデッキのアドレス (`CardAddressValueObject[]`) をロードして返すメソッド（`public CardAddressValueObject[] GetSavedDeckAddresses(PlayerMasterUseCase playerMasterUseCase)`）を追加。

### 3. `OutgameStartSequience.cs`の変更
- `PlayerDeckUseCase`のインスタンス化時に`PlayerMasterUseCase`を渡すのを中止（リバート）。

## 実施手順
1. 本計画書を`Assets\Docs\Task\Refactor_DeckRelatedLogic.md`に作成。
2. `PlayerDeckUseCase.cs`に保存済みデッキ情報をロードするメソッドを追加。
3. `DeckEditorPresenter.cs`を修正し、`InitializeDeckEditor()`の分割と`_currentDeckCards`の初期化ロジックを改善。
4. `OutgameStartSequience.cs`を修正し、`PlayerDeckUseCase`のインスタンス化を修正。
5. 修正後、本計画書に完了レポートを追記。

## 懸念事項
- `RoleAsset`の具体的な定義が不明なため、ロール名の取得方法（`RoleAsset.name`）を仮定。
- `CardDataBase.GetDeckCards()`が返すデータが、期待する「所持カード」の全リストであるかを確認。
- `CardDataBase`と`RoleDataBase`のアドレス名がそれぞれ`"CardDataBase"`と`"RoleDataBase"`であると仮定。

## 完了レポート
- `PlayerDeckUseCase.cs`において、コンストラクタから`PlayerMasterUseCase`を削除し、`public CardAddressValueObject[] GetSavedDeckAddresses(PlayerMasterUseCase playerMasterUseCase)`メソッドを追加しました。このメソッドはUseCase層の責務として、保存されたデッキのカードアドレスのリストを返します。
- `OutgameStartSequience.cs`において、`PlayerDeckUseCase`のインスタンス化時に`PlayerMasterUseCase`を渡す処理を削除しました。
- `DeckEditorPresenter.cs`の`InitializeDeckCards()`メソッドにおいて、`_playerDeckUseCase.GetSavedDeckAddresses(_playerMasterUseCase)`を呼び出し、UseCase層から取得した`CardAddressValueObject[]`を元に、`_allCard`から`DeckCardEntity`を抽出し、`CardViewModel`に変換して`_currentDeckCards`を初期化するように修正しました。これにより、`CardViewModel`がPresenter層に留まるようにアーキテクチャの責務が遵守されました。
- `InitializeDeckEditor()`メソッドは`InitializeRoleUI()`, `InitializeOwnedCards()`, `InitializeDeckCards()`の3つのプライベートメソッドに分割され、各メソッドの責務が明確になりました。
- これにより、デッキ関連のロジックがより明確に分離され、`DeckEditorPresenter`の初期化が適切に行われるようになりました。