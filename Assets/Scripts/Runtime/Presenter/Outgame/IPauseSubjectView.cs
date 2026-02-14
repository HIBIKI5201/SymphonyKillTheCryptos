using System;

namespace Cryptos.Runtime.Presenter.OutGame.Story
{
    public interface IPauseSubjectView
    {
        public event Action<bool> OnPause;
    }
}
