# SymphonyKillTheCryptos

## AssetStoreTools（サブモジュール）

`Assets/AssetStoreTools` は private リポジトリ [SymphonyKillTheCryptos_AssetStoreTools](https://github.com/HIBIKI5201/SymphonyKillTheCryptos_AssetStoreTools) をサブモジュールとして参照している（閲覧権限が必要）。クローン時にサブモジュールも取得する。

```bash
git clone --recurse-submodules https://github.com/HIBIKI5201/SymphonyKillTheCryptos.git
# クローン済みの場合
git submodule update --init
```

100MB を超えるため除外したファイルがある。Unity エディタを起動すると、取り込み直す Asset Store パッケージをダイアログで案内する（メニュー「Tools/AssetStoreTools/除外ファイルの復元案内」からも開ける）。
