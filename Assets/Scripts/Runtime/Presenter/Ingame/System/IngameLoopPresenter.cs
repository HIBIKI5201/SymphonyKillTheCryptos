using Cryptos.Runtime.UseCase.Ingame.System;
using System;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public class IngameLoopPresenter : IIngameLoopPresenter
    {
        public event Action OnResultWindowReturnButtonClicked;

        private readonly IIngameUIManager _ingameUIManager;

        public IngameLoopPresenter(IIngameUIManager ingameUIManager)
        {
            _ingameUIManager = ingameUIManager;
            _ingameUIManager.OnResultWindowReturnButtonClicked += () => OnResultWindowReturnButtonClicked?.Invoke();
        }

        public void RequestShowResult(string title, int score)
        {
            _ingameUIManager.OpenResultWindow(title, score);
        }
    }
}
