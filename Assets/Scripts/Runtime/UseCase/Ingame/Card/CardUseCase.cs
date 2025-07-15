using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    public class CardUseCase
    {
        private readonly WordManager _wordManager = new();

        public CardEntity CreateCard(CardData data, WordDataBase wordDatabase)
        {
            if (Mathf.Abs(data.WordRange.x - data.WordRange.y) < 1)
            {
                Debug.LogWarning($"{data.name}のWordRnageが不適切です\n二つの距離は0から{wordDatabase.WordData.Length - 1}までです");
                return null;
            }

            WordData[] words = wordDatabase.WordData[data.WordRange.x..data.WordRange.y];
            return new CardEntity(data, words, _wordManager);
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