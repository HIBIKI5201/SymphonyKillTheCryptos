using System;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public interface IIngameLoopPresenter
    {
        public event Action OnResultWindowReturnButtonClicked;
        public void RequestShowResult(int level, int wave, int skillPoint);
    }
}