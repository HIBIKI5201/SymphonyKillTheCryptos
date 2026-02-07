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

        public void RequestShowResult(int level, int wave, int skillPoint)
        {
            int time = -1; //TODO:一旦放置。

            ResultViewModel resultViewModel = new(level, wave, time, skillPoint);
            _ingameUIManager.OpenResultWindow(resultViewModel);
        }
    }
}
