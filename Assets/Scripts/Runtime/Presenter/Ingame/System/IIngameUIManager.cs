using System;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public interface IIngameUIManager
    {
        public void OnLevelUpgradeInputChar(char c);
        public void CardInputChar(char c);
        public void OpenResultWindow(string title, int score, Action onReturnButtonClicked);
    }
}
