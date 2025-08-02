using Cryptos.Runtime.Entity.Ingame.Card;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Card
{
    public interface ICardUIManager
    {
        public void HanldeAddCard(CardViewModel instance);
        public void HandleRemoveCard(CardViewModel instance);
    }
}
