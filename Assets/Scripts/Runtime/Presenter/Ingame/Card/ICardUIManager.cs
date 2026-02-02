namespace Cryptos.Runtime.Presenter.Ingame.Card
{
    public interface ICardUIManager
    {
        public void AddCard(CardViewModel instance);
        public void RemoveCard(CardViewModel instance);
        public void MoveCardToStack(CardViewModel instance);
    }
}
