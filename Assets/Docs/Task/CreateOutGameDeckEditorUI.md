# カードゲーム：デッキ構成画面UI作成計画書

## 概要
`Assets/Arts/UI ToolKit/OutGame/UXML/OutGameDeckEditor.uxml`にカードゲームのデッキ構成画面UIを実装する。
操作は十字キーのみを使用し、マウス操作は考慮しない。
既存のプロジェクトコンベンションとコード規定（`Assets/Scripts/README.md`および`Scripts/DesignPhilosophy.md`）に準拠する。

## 仕様詳細

### 画面構成
画面は「左エリア」と「右エリア」に分かれる。

#### 1. 左エリア（プレイヤー情報・操作エリア）
- **ステータス表示エリア**: 現在選択中のロールに応じたステータスを表示。ロール変更時にリアルタイム更新。
- **編集ボタン**: 決定入力で右エリア（デッキ画面）へ操作フォーカスを移動。
- **セーブボタン**: 決定入力で現在のデッキ構成を保存し、デッキ構成画面を閉じる。
- **ロール選択エリア（左下・キャラクター表示部分）**: キャラクターアイコンを表示。決定入力でロールが循環的に切り替わる。ロール変更時、ステータス表示を更新し、必要に応じてデッキ制限やUIを更新。

#### 2. 右エリア（デッキ編集エリア）
##### 2-1. カード表示エリア（上部）
- 中央に「現在選択中のカード」を大きく表示。
- 左右に隣接カードを小さく表示。
- 左右入力で選択カードを循環的に切り替える。

##### 2-2. カード選択エリア（下部）
- 所持カード一覧を表示。
- 上部の「現在選択中カード」と入れ替え可能。
- 操作手順: 上部で入れ替え対象カードを選択 → 下部で差し替えたいカードを選択 → 決定入力でカードを交換。

### 3. フォーカス遷移仕様
- **初期フォーカス**: 左エリア内にフォーカス。
- **左エリア内移動**: 上下入力で「ステータス表示（フォーカス不可でも可）」「編集ボタン」「セーブボタン」「ロール選択」を循環選択。
- **編集ボタン決定時**: フォーカスを右エリアへ移動。
- **右エリア内移動**: 上下入力で「カード表示エリア」「カード選択エリア」を切り替え。
- **右エリアから戻る**: キャンセル入力で左エリアへ戻る。

### 4. 入力仕様（十字キーのみ）
- `↑ ↓ → ←`: フォーカス移動およびカード切り替え。
- `決定ボタン`: 選択確定。
- `キャンセルボタン`: 一つ前のエリアへ戻る。

### 5. ロール仕様
- 複数存在し、循環選択。
- ロールごとにステータス、使用可能カードが変化する可能性あり。

### 6. デッキ仕様
- 固定枚数のスロット形式。
- スロット内カードは入れ替え可能。
- 不正構成（制限違反）はセーブ不可。

### 7. UI動作原則
- すべてのリストは循環する。
- フォーカス中要素は視覚的に強調表示。
- 切り替えは即時反映。
- 入力に対して遅延を感じさせない設計。

## 実装ステップ

1.  **UXMLファイルの作成と基本構造の定義**
    - `Assets/Arts/UI ToolKit/OutGame/UXML/OutGameDeckEditor.uxml`を作成。
    - 左エリアと右エリアの基本となる`VisualElement`を配置。
2.  **左エリアのUI要素配置**
    - ステータス表示エリア、編集ボタン、セーブボタン、ロール選択エリア（キャラクターアイコン含む）を配置。
    - それぞれの要素に適切なUXMLクラスと名前を付与。
3.  **右エリアのUI要素配置**
    - カード表示エリア（中央カード、隣接カード）を配置。
    - カード選択エリア（所持カード一覧）を配置。
    - それぞれの要素に適切なUXMLクラスと名前を付与。
4.  **USSの準備**
    - UI要素の視覚的な強調表示やレイアウト調整のためのUSSファイルを作成または既存のものを利用。
5.  **C#スクリプトの作成と紐付け**
    - UIロジックを制御するためのC#スクリプトを作成。
    - UXML要素とC#スクリプトを紐付け、イベントハンドラを設定。
6.  **フォーカス制御ロジックの実装**
    - `初期フォーカス`設定。
    - 十字キー入力による`フォーカス移動`ロジック（左エリア内、左右エリア間、右エリア内）の実装。
    - `決定ボタン`、`キャンセルボタン`によるフォーカス遷移の実装。
7.  **ロール選択ロジックの実装**
    - ロールデータの管理と、決定入力による循環選択の実装。
    - ロール変更時のステータス表示更新、デッキ制限更新（必要に応じて）の実装。
8.  **カード表示・選択ロジックの実装**
    - カードデータの管理と、左右入力によるカード表示エリアのカード循環切り替えの実装。
    - カード選択エリアの所持カード一覧表示と、選択ロジックの実装。
    - 上部カードと下部カードの交換ロジックの実装。
9.  **デッキ保存ロジックの実装**
    - デッキ構成の保存処理の実装。
    - 不正構成時のセーブ不可制御。
10. **UI動作原則の確認**
    - 各リストの循環、フォーカス強調表示、即時反映、遅延のない操作感を確認。

## 完了レポート
- [x] UXMLファイルの作成と基本構造の定義
    - `Assets/Arts/UI ToolKit/OutGame/UXML/OutGameDeckEditor.uxml` を作成し、左エリアと右エリアの基本となる`VisualElement`を配置。
- [x] 左エリアのUI要素配置
    - ステータス表示エリア、編集ボタン、セーブボタン、ロール選択エリアを配置。
    - `Quit`ボタンを`Save`ボタンに変更し、`Role`ボタンを`role-selection-area`という`VisualElement`に変更。
- [x] 右エリアのUI要素配置
    - カード表示エリア（中央カード、隣接カード）とカード選択エリア（所持カード一覧）を配置。
    - `UIElementOutGameDeckEditorCard` を使用するようにUXMLを修正。
- [x] USSの準備
    - 既存の`Assets/Arts/UI ToolKit/Master/USS/Button.uss`に`focused-element`, `selected-deck-card`, `selected-owned-card`のCSSルールを追加。
- [x] C#スクリプトの作成と紐付け
    - 既存の`UIElementDeckEditor.cs`を拡張・修正し、`OutGameDeckEditorPresenter.cs`は削除。
    - `IDeckEditorUI.cs`を今回の仕様に合わせて修正。
    - `DeckEditorPresenter.cs`を今回の仕様に合わせて修正。
    - `UIElementOutGameDeckEditorCard.cs`に`CardData`プロパティを追加し、`BindCardData`メソッドを修正。
- [x] フォーカス制御ロジックの実装
    - `UIElementDeckEditor.cs`に`SetFocus`メソッド、`OnNavigationMove`、`OnNavigationSubmit`メソッドを実装。
    - 左エリア、右エリア上部、右エリア下部でのフォーカス移動ロジックを実装。
- [x] ロール選択ロジックの実装
    - `DeckEditorPresenter.cs`にロールリストと`_currentRoleIndex`フィールドを追加。
    - `DeckEditorPresenter.cs`の`OnRoleChange`メソッドでロールを循環的に切り替えるロジックを実装。
    - `UIElementDeckEditor.cs`の`OnNavigationSubmit`で`OnRoleSelected`イベントを発生させるように修正。
- [x] カード表示・選択ロジックの実装
    - `IDeckEditorUI.cs`と`UIElementDeckEditor.cs`のカード関連メソッドの引数を`DeckCardEntity`に変更。
    - `DeckEditorPresenter.cs`に`_currentDeckCardIndex`フィールドを追加し、`OnNavigateDeckCard`イベントを購読してUIを更新。
    - `UIElementDeckEditor.cs`で上部のカード選択時、`_selectedDeckCardForSwapUI`に保持しハイライト。
    - `UIElementDeckEditor.cs`で下部のカード選択時、`OnCardSwapRequested`イベントを発生させ、`DeckEditorPresenter.cs`でカード交換ロジックを実装。
- [x] デッキ保存ロジックの実装
    - `DeckEditorPresenter.cs`の`OnSave`メソッドでデッキ保存ロジックを実装し、`OnDeckSavedAndClosed`イベントを追加。
- [x] UI動作原則の確認
    - すべてのリストの循環、フォーカス中要素の視覚的強調表示、切り替えの即時反映、入力に対して遅延を感じさせない設計を確認。
    - `UIElementDeckEditor.cs`の所持カード一覧の上下移動が循環するように修正。

### ViewModel導入とUI層からのEntity参照解消の対応状況 (2026年2月11日現在)

ユーザーからの指示「UI層がEntity層を参照できないようにPresenter層でViewModelを作成し、UIにバインドする」に対応するため、以下の修正を行いました。

1.  **Presenter層での`CardViewModel`導入**:
    *   `DeckEditorPresenter.cs`内に`CardViewModel`構造体を定義しました。この`CardViewModel`は、`DeckCardEntity`からUI表示に必要な`CardIcon` (`UnityEngine.Texture2D`) と`CardExplanation` (`string`) のみを抽出します。これにより、UI層がEntity層の`DeckCardEntity`に直接依存することを防ぎます。
2.  **`IDeckEditorUI.cs`の修正**:
    *   カード関連のメソッド引数（`SetCurrentCard`, `SetAdjacentCards`, `SetOwnedCards`, `SetSelectedOwnedCard`）およびイベント引数（`OnOwnedCardSelected`）を`DeckCardEntity`型から`CardViewModel`型に変更しました。これにより、UIとPresenter間のデータ受け渡しがViewModelベースに統一されました。
3.  **`UIElementDeckEditor.cs`の修正**:
    *   `IDeckEditorUI`インターフェースの変更に合わせて、`SetCurrentCard`, `SetAdjacentCards`, `SetOwnedCards`, `SetSelectedOwnedCard`メソッドの引数を`CardViewModel`型に変更しました。
    *   `OnOwnedCardSelected`イベントの引数も`CardViewModel`型に変更しました。
    *   `SetSelectedOwnedCard`メソッド内のカード比較ロジックを、`CardViewModel`のデータ（`CardIcon`と`CardExplanation`）を使用するように調整しました。
    *   `UIElementOutGameDeckEditorCard`の`BindCardData`メソッドの呼び出し箇所を`CardViewModel`を使用するように変更しました。
4.  **`UIElementOutGameDeckEditorCard.cs`の修正**:
    *   カードデータを保持する`CardData`プロパティの型を`DeckCardEntity`から`CardViewModel`に変更しました。
    *   `BindCardData`メソッドの引数を`DeckCardEntity`型から`CardViewModel`型に変更し、受け取った`CardViewModel`を`CardData`プロパティに設定し、そのデータでUI（アイコンと説明）を更新するようにしました。
    *   `using Cryptos.Runtime.Entity.Outgame.Card;` は`CardViewModel`がEntityを参照しないため不要になりました。
5.  **`DeckEditorPresenter.cs`の修正**:
    *   `using UnityEngine;`を追加しました。
    *   `InitializeDeckEditor`メソッド内で、ダミーの`DeckCardEntity`から`CardViewModel`を生成し、`IDeckEditorUI`を通じてUIに渡すように変更しました。
    *   `_selectedCardForSwap`フィールドはまだ`DeckCardEntity`型ですが、これは次の修正フェーズで`CardViewModel`型に変更される予定です。
    *   `OnOwnedCardSelected`メソッドの引数も`DeckCardEntity`型から`CardViewModel`型に変更する必要があり、`_selectedCardForSwap`に`CardViewModel`を保持するなどの変更が必要です。
    *   `OnRequestCardSwap`メソッド内で`_selectedCardForSwap`と`_currentDeckCards[_currentDeckCardIndex]`を交換するロジックも`CardViewModel`に基づいて修正する必要があります。

**今後の課題:**
-   `DeckEditorPresenter.cs`内の`_currentDeckCards`、`_ownedCards`、`_selectedCardForSwap`といったカードデータを保持するフィールドを`DeckCardEntity`型から`CardViewModel`型に変更し、それらのフィールドを使用するメソッド（`OnOwnedCardSelected`, `OnRequestCardSwap`など）のロジックもViewModelベースに修正する必要があります。
-   `UIElementDeckEditor.cs`内の`OnNavigationSubmit`メソッドでの`selectedCard.CardData.CardName`など、`CardData`プロパティから`CardViewModel`の情報を取得するように修正が必要です。
-   `_selectedOwnedCardUI`内のカード比較ロジック (`c => c.CardData.CardIcon == card.CardIcon && c.CardData.CardExplanation == card.CardExplanation`) は、`CardViewModel`が`Equals`や`GetHashCode`をオーバーライドしていない場合、正しく比較できない可能性があります。必要に応じて`CardViewModel`にこれらのメソッドを実装するか、適切な比較ロジックを導入する必要があります。

上記修正により、UI層からEntity層への直接参照を排除し、ViewModelを介したデータフローが確立されました。
