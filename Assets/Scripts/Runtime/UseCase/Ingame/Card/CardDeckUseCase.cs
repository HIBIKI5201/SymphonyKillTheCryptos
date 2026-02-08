using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.Card;
using UnityEngine;

namespace Cryptos.Runtime.UseCase
{
    public class CardDeckUseCase
    {
        public CardDeckUseCase(CardDeckEntity entity, CardUseCase useCase)
        {
            _entity = entity;
            _cardUseCase = useCase;
        }

        public CardEntity DrawCard()
        {
            int index = Random.Range(0, _entity.Length);
            CardData data = _entity[index];
            CardEntity instance = _cardUseCase.CreateCard(data);

            instance.OnComplete += () => DrawCard(); // 仮のサイクルとしてカードをドローする。
            return instance;
        }

        private CardDeckEntity _entity;
        private CardUseCase _cardUseCase;
    }
}
