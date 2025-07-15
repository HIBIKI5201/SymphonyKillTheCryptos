using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    public class CardUseCase
    {
        public CardUseCase(WordDataBase wordDataBase)
        {
            _cardDrawer = new(wordDataBase);
        }

        private readonly CardDrawer _cardDrawer;

        public CardEntity CreateCard(CardData data)
        {
            return _cardDrawer.GetNewCard(data);
        }

        public void InputCharToCard(CardEntity cardEntity, char input)
        {
            cardEntity.OnInputChar(input);
        }

        public void ExecuteCardEffect(CardEntity cardEntity, GameObject player, params GameObject[] target)
        {
            foreach (var content in cardEntity.CardData.Contents)
            {
                content.TriggerEnterContent(player, target);
            }
        }
    }
}