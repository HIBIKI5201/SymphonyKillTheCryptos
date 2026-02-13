# Implement Horizontal Card List for Owned Cards

## 1. 概要
インゲームのデッキエディタUIにおける所持カードの表示を、横スクロール可能なリストビューに変更する。右上と右下のカード選択はスライド選択方式であり、カード個々へのフォーカスは行わず、エリア単位でのフォーカス管理を行う。

## 2. ゴール
*   所持カードが横並びに表示され、左右にスクロール可能であること。
*   右上および右下のカードは、スライド選択方式でUIナビゲーションが機能すること。
*   既存のナビゲーション（キーボード操作など）と連携し、エリア単位でのフォーカス移動が適切に機能すること。

## 3. 影響範囲
*   `Assets\Scripts\Runtime\UI\Outgame\UIElementDeckEditor.cs`
*   `Assets\Scripts\Runtime\UI\Outgame\UIElementOutGameDeckEditorCard.cs` (レイアウト調整の可能性)
*   UXMLファイル (`Assets\Arts\UI ToolKit\OutGame\UXML\OutGameDeckEditor.uxml`など)
*   USSファイル (該当するスタイルシート)

## 4. 詳細計画

### 4.1. `UIElementDeckEditor.cs`の修正
*   **`SetOwnedCards`メソッド**:
    *   `_ownedCardList`のレイアウトを横並びにするために、USSクラスの適用またはStyleプロパティの直接設定を検討する。
    *   各`UIElementOutGameDeckEditorCard`を`_ownedCardList`に追加する際に、横並びになるように調整する。
*   **`OnNavigationMove`メソッド**:
    *   `FocusArea.RightAreaTop`および`FocusArea.RightAreaBottom`におけるナビゲーションロジックから、カード個々へのフォーカス移動ロジックを削除する。
    *   横方向の移動に対しては、現在選択されているカードがビューの中心に来るようにスクロールを制御するロジックを実装する。
*   **`SetSelectedOwnedCard`メソッド**:
    *   選択されたカードの視覚的なハイライトに加え、そのカードがビューの中心になるように`_ownedCardList`のスクロール位置を調整する。このメソッドは、個別のカードへのフォーカスを設定するものではないことに注意。
*   **`OnNavigationSubmit`メソッド**:
    *   `FocusArea.RightAreaTop`および`FocusArea.RightAreaBottom`における決定時の処理ロジックを、エリア単位での選択やスライド選択方式に合わせて調整する。特に、`_selectedDeckCardForSwapUI`の利用方法を見直す。

### 4.2. UXML/USSファイルの修正
*   **`OutGameDeckEditor.uxml`**:
    *   `_ownedCardList`（`VisualElement`）が横方向にカードを並べ、オーバーフローした場合にスクロール可能になるように設定する。
*   **USSファイル**:
    *   `_ownedCardList`および`UIElementOutGameDeckEditorCard`のスタイルを調整し、横並び表示とスクロールに適した見た目にする。カード間のスペース、サイズなどを調整する。

## 5. 実装時の考慮事項
*   **スクロールアニメーション**: カードが中心に移動する際、スムーズなアニメーションを導入することでUXを向上させる。Unity UI ToolkitのスクロールAPIを利用する。
*   **パフォーマンス**: 大量のカードが存在する場合でも、スクロールや選択時のパフォーマンスが低下しないように注意する。
*   **フォーカス管理**: エリア単位でのフォーカス移動とスライド選択方式に合わせたナビゲーションロジックが衝突しないように、慎重に行う。

## 6. 完了基準
*   所持カードが横並びに整列し、左右にスクロールできること。
*   右上および右下のカードエリアは、カード個々へのフォーカスではなく、エリア単位でのナビゲーションが機能すること。
*   選択された所持カード（フォーカスされたカードではなく、決定キーで選択されたカード）が、所持カードリストビューの中心に表示されること。
*   既存のカード選択・交換ロジックが引き続き正常に機能すること。
*   コンパイルエラーがないこと。

## 7. 今後の作業
*   上記の計画に基づき、コード修正とUXML/USS修正を行う。
*   機能の動作確認とデバッグを行う。
