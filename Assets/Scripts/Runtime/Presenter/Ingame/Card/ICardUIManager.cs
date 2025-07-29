using Cryptos.Runtime.Entity.Ingame.Card;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame
{
    public interface ICardUIManager
    {
        public void HanldeAddCard(CardEntity instance);
        public void HandleRemoveCard(CardEntity instance);
    }
}
