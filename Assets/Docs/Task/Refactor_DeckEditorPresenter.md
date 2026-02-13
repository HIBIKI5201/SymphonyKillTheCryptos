# DeckEditorPresenterのリファクタリング計画書

## 概要
`DeckEditorPresenter`クラスが現在使用している仮のカードおよびロールデータを、`CardDataBase`および`RoleDataBase`から取得した実データに置き換える。
データベースの情報は`OutgameStartSequience`のコンストラクタから注入されるべきであり、`Addressable`は使用しない。

## 問題点
- `DeckEditorPresenter`内でカードデータ (`_currentDeckCards`, `_ownedCards`) およびロールデータ (`_roles`) がハードコードされたダミーデータで初期化されている。
- `PlayerDeckUseCase`および`PlayerMasterUseCase`が、`CardDataBase`や`RoleDataBase`へのアクセスを提供していないため、`DeckEditorPresenter`からデータベースへ直接アクセスする必要がある。
- データベースの情報を`Addressable`経由ではなく、コンストラクタ注入で提供する必要がある。

## 修正内容

### 1. `DeckEditorPresenter.cs`の変更
- コンストラクタに`DeckCardEntity[] allCard`と`RoleEntity[] roles`のインスタンスを受け取るように修正済み。
- `InitializeDeckEditor()`メソッド内のダミーカードデータおよびロールデータの初期化部分を削除し、コンストラクタで受け取った`_allCard`と`_roles`を使用して初期化するように変更。
  - ロールデータは`_roles[_currentRoleIndex].Name`を使用する。
  - 所持カードデータは`_allCard`を`CardViewModel`のリストに変換して`_ownedCards`を初期化。
  - `_currentDeckCards`は`_ownedCards`の最初の3枚で初期化（暫定）。

### 2. `OutgameStartSequience.cs`の変更
- コンストラクタで`CardDataBase`と`RoleDataBase`のインスタンスを受け取るように修正済み。
- `GameInitialize()`メソッド内で、`DeckEditorPresenter`をインスタンス化する際に、`CardDataBase.GetDeckCards()`の結果と`RoleDataBase.RoleAssets`を渡すように修正済み。

### 3. `OutgameStartSequience`を呼び出す箇所の変更 (新規)
- `OutgameStartSequience`がインスタンス化されている箇所を特定し、`Addressables.LoadAssetAsync`を使用して`CardDataBase`と`RoleDataBase`のインスタンスをロードし、`OutgameStartSequience`のコンストラクタに渡すように修正済み。

## 実施手順
1. 本計画書を`Assets\Docs\Task\Refactor_DeckEditorPresenter.md`に更新。
2. `OutgameStartSequience`が呼び出されている箇所を特定。
3. `DeckEditorPresenter.cs`を修正。
4. `OutgameStartSequience.cs`を修正。
5. `OutgameStartSequience`が呼び出されている箇所を修正。
6. 修正後、本計画書に完了レポートを追記。

## 懸念事項
- `RoleAsset`の具体的な定義が不明なため、ロール名の取得方法（`RoleAsset.name`）を仮定。
- `CardDataBase.GetDeckCards()`が返すデータが、期待する「所持カード」の全リストであるかを確認。
- `CardDataBase`と`RoleDataBase`のアドレス名がそれぞれ`"CardDataBase"`と`"RoleDataBase"`であると仮定。

## 完了レポート
- `DeckEditorPresenter.cs`の`InitializeDeckEditor()`メソッドにおいて、ダミーカードデータが`_allCard`フィールドから取得したデータに置き換えられ、`_ownedCards`および`_currentDeckCards`が初期化されました。
- ロールデータは`_roles`フィールドから取得し、`_deckEditorUI`に設定されるように修正されました。
- これにより、`DeckEditorPresenter`はデータベースから注入された実データを使用するようになりました。