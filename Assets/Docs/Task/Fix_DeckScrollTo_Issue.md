# DeckScrollTo関数修正計画書

## 概要
UIElementDeckEditorのDeckScrollTo関数において、カードのVisualElementが正しく整列しない問題を修正する。

## 問題点
DeckScrollTo関数内で、ループ変数`i`を使わず常に同じカード要素を参照しているため、カードが正しく配置されない。
また、_currentDeckCardIndexが更新されないため、スクロールが正しく機能しない可能性がある。

## 修正内容
1. DeckScrollTo関数内で、`UIElementOutGameDeckEditorCard card = _deckCardElements[index];` を `UIElementOutGameDeckEditorCard card = _deckCardElements[i];` に修正する。
2. DeckScrollTo関数の引数`index`を`_currentDeckCardIndex`に代入する。

## 実施手順
1. 本計画書を`Assets\Docs\Task`に作成。
2. `Assets/Scripts/Runtime/UI/Outgame/UIElementDeckEditor.cs`を修正。
3. 本計画書に完了レポートを追記。

## 完了レポート
（ここに完了時に追記します）