using System;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public interface IIngameUIManager
    {
        public event Action OnResultWindowReturnButtonClicked;
        public void OnLevelUpgradeInputChar(char c);
        public void CardInputChar(char c);
        public void OpenResultWindow(ResultViewModel vm);
    }
}
