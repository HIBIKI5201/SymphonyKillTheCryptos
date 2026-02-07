using System;

namespace Cryptos.Runtime.UseCase.Ingame.Character
{
    public interface ISymphonyPresenter
    {
        public void ResetUsingCard();
        public event Action OnDead;
    }
}
