# 基底概念

- 大規模に拡張性のあるシステムであること
- 仕様の変更に柔軟であること
- 大人数での開発に対応できること

# アーキテクチャ

基礎概念はクリーンアーキテクチャ。
副次概念はドメイン駆動設計。

レイヤーを

- Entity
- UseCase
- Presenter
- UI
- InfraStructure

に分ける。

ただしUnityフレームワークへの依存は許容する。
Unityライフサイクルへの依存は抑える。

上位層への参照を行いたい場合は、自層にintarfaceを追加し、上位層がそれを実装してInfraStructureが注入する。

## 各レイヤーの説明

### Entity

データ層。ピュアクラス。
参照レイヤーはなし。

ロジックで使用するデータの保存層。
データに関するロジックを持つ。

### UseCase

処理層。ピュアクラス。
参照レイヤーはEntity。

Entityを操作してロジックを処理する。

### Presenter

伝達層。Unityフレームワーク/ピュアクラスのハイブリッド。
参照レイヤーはEntity/UseCase。

UseCaseの処理を呼び出したり、Entityのデータを他のUseCaseへ橋渡しする。
UI層へデータを受け渡す。UIからの入力をUseCaseに受け渡す。

UIへの受け渡しはViewModelを使用する。

### UI

表示層。Unityフレームワーク。
参照レイヤーはPresenter。

ゲームオブジェクトやレンダリングの操作を行う。
入力を受け取ってPresenterに受け渡す。

### InfraStructure

初期化層。Unityフレームワーク/ピュアクラスのハイブリッド。
参照レイヤーはEntity/UseCase/Presenter/UI。

Entity、UseCase、Presenter、UIのシステムの依存性注入を行う。

# クラス設計

最初に機能のPresenterを作成。

その機能に必要なロジックをUseCaseで実装。
PresenterからUseCaseを呼び出すようにする。

UseCaseで使用するデータをEntityとして生成。

PresenterはEntityの情報をViewModelに変換してUIに伝達。

UIはViewModelの情報をもとに表示を更新。

PresenterがUseCaseの初期化やUIへの参照に必要な情報をInfraStructureが注入。

### 型安全/ValueObject

特定のパラメータを明示的にするために専用のstructを設計する。

```csharp
// ピュアなintという型から、明示的なHealthPointという型に変える。
public readonly struct HealthPoint
{
		public HealthPoint(int value)
		{
				Value = value;
		}
		
		public readonly int Value;
}
```

### ViewModel

UIにデータを受け渡す一時的なデータカプセル。DataTransfarObjectの一種。

Presenter層に存在し、UIの更新時に引数として受け取る。

```csharp
// UIに渡す情報を持つ。
public readonly struct ViewModel
{
		public ViewModel(int data1, string data2)
		{
				Data1 = data1;
				Data2 = data2;
		}
		
		public readonly int Data1;
		public readonly string Data2:
}

// ViewModelを生成してUIに渡す。
public class Presenter
{
		public void Execute()
		{
				ViewModel vm = new(1, "abc");
				_ui.Show(vm);
		}
		
		private UI _ui;
}

// ViewModelを受け取って反映する。
public class UI
{
		public void Show(ViewModel vm)
		{
				_text.text = $"{vm.Data1} {vm.Data2}";
		}
		
		private Text _text;
}
```