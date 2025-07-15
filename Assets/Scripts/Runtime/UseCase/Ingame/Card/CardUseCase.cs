using Cryptos.Runtime.Entity;
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
            return _cardDrawer.CreateNewCard(data);
        }

        public void InputCharToCard(CardEntity cardEntity, char input)
        {
            cardEntity.OnInputChar(input);
        }

        public void ExecuteCardEffect(CardEntity cardEntity, IAttackable player, params IHitable[] targets)
        {
            ICardContent[] contents = cardEntity.CardData.Contents;

            if (contents == null || contents.Length == 0) return;

            foreach (var content in contents)
            {
                if (content == null) continue;

                try
                {
                    content.Execute(player, targets);
                }
                catch (Exception e)
                {
                    Debug.LogError($"コンテンツの実行に失敗しました: {content.GetType().Name}\n{e.Message}\nstack trace\n{e.StackTrace}");
                }
            }
        }
    }
}