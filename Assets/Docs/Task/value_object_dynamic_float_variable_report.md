# Value Object化検討レポート：DynamicFloatVariable

## 目的
ドメイン駆動設計（DDD）の原則に基づき、`DynamicFloatVariable` クラスをValue Object（値オブジェクト）として再設計することで、コードの不変性、可読性、予測可能性、テスト容易性、および並行処理の安全性を向上させる。

## 現状分析：`DynamicFloatVariable.cs`
- **ファイルパス**: `Assets\Scripts\Runtime\Entity\Utility\DynamicFloatVariable.cs`
- **概要**: `float` 値をラップし、`StatModifier`（状態修飾子）を適用することで値を動的に変化させることができるクラス。バフによるキャラクターのステータス変化などに利用されている。
- **Value Object化に適している点**:
    - **概念のモデリング**: 「バフによる状態変化が可能なfloat変数」という明確なドメイン概念を表現している。
    - **等価性**: 基礎となる `float` 値と適用されている `StatModifier` のリストに基づいて等価性を判断するロジックを容易に実装できる。
    - **自己検証**: `Recalculate()` メソッドにより、`StatModifier` が適用された後の値の計算ロジックが内包されているため、生成時に一貫性を保ちやすい。
- **Value Object化に向けた課題（現状との差異）**:
    - **不変性**: `AddModifier()` や `RemoveModifier()` メソッドが自身の内部状態（`_modifiers` リストおよび `_value`）を変更するため、現在は可変である。Value Objectは一度生成されたら状態を変更できない不変性が必須要件。

## Value Object化のメリット
1.  **不変性の確保**: 一度生成された `DynamicFloatVariable` インスタンスは変更不可となり、より予測可能なコードになる。
2.  **テスト容易性**: 状態が変化しないため、テストが単純化され、独立したテストが可能になる。
3.  **並行処理の安全性**: 複数のスレッドから同時にアクセスされても競合状態が発生しないため、並行処理環境での安全性が高まる。
4.  **概念の明確化**: ドメインの概念がより明確にコードに反映され、誤用を防ぎやすくなる。
5.  **Side Effect の排除**: メソッド呼び出しによる予期せぬ副作用（Side Effect）のリスクが低減される。

## Value Object化するための具体的な変更案

1.  **コンストラクタの変更**:
    *   `baseValue` と、適用される `StatModifier` のリストを受け取るコンストラクタを定義する。
    *   コンストラクタ内で `Recalculate()` を呼び出し、最終的な `Value` を決定する。

2.  **不変性の確保**:
    *   `_value` フィールドを削除し、`Value` プロパティはコンストラクタで計算された最終値を保持するようにするか、`Recalculate()` を呼び出して返すようにする（初回計算のパフォーマンスを考慮）。
    *   `AddModifier(StatModifier mod)` メソッドを `public DynamicFloatVariable AddModifier(StatModifier mod)` のように変更する。このメソッドは、新しい `StatModifier` を追加した**新しい `DynamicFloatVariable` インスタンス**を返却するようにし、元のインスタンスは変更しない。
    *   `RemoveModifier(StatModifier mod)` メソッドも同様に、指定された `StatModifier` を除外した**新しい `DynamicFloatVariable` インスタンス**を返却するように変更する。
    *   `_modifiers` リストを読み取り専用にするか、新しいリストを生成して返すように変更する。

3.  **等価性の実装**:
    *   `Equals(object obj)` メソッドと `GetHashCode()` メソッドをオーバーライドする。
    *   `baseValue` と `_modifiers` リストの内容（`StatModifier` の値とタイプ、優先度など）に基づいて、2つの `DynamicFloatVariable` インスタンスが等しいかどうかを判断するように実装する。

## 変更による影響

1.  **`DynamicFloatVariable` の利用箇所**: `AddModifier()` や `RemoveModifier()` の呼び出し元では、返された新しい `DynamicFloatVariable` インスタンスを、既存の変数に再割り当てする必要があります。
    ```csharp
    // 変更前
    DynamicFloatVariable myFloat = new DynamicFloatVariable(100f);
    myFloat.AddModifier(new StatModifier(10f, StatModType.Additive, 0));

    // 変更後
    DynamicFloatVariable myFloat = new DynamicFloatVariable(100f);
    myFloat = myFloat.AddModifier(new StatModifier(10f, StatModType.Additive, 0)); // 再割り当てが必要
    ```
2.  **パフォーマンス**: `AddModifier()` や `RemoveModifier()` のたびに新しいインスタンスが生成されるため、オブジェクト生成のオーバーヘッドがわずかに増加する可能性があります。しかし、通常これは許容範囲内であり、不変性のメリットの方が大きいことが多いです。

## 今後の作業
上記変更案に基づき、`DynamicFloatVariable.cs` をValue Objectとしてリファクタリングする作業を行います。
リファクタリング後は、`DynamicFloatVariable` を利用している全ての箇所で、新しいインスタンスの再割り当てが正しく行われているかを確認し、必要に応じて修正します。
また、Value Objectとしての振る舞いが意図通りであるかを検証するためのテストを追加することも検討します。
