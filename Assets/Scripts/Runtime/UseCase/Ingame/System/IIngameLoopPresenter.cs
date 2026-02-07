using System;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public interface IIngameLoopPresenter
    {
        // ゲーム終了時のリザルト画面表示を要求するメソッド
        void RequestShowResult(string title, int score);

        // リザルト画面の「メインメニューに戻る」ボタンが押されたことを通知するイベント
        event Action OnResultWindowReturnButtonClicked;
    }
}