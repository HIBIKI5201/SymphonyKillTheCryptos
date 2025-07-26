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

        public event Func<IAttackable> GetPlayer;

        public event Action<CardEntity> OnCardCompleted;

        public CardEntity CreateCard(CardData data)
        {
            return _cardDrawer.CreateNewCard(data);
        }

        public void InputCharToCard(CardEntity cardEntity, char input)
        {
            //入力をエンティティに渡す
            if (cardEntity.WordEntity.OnInputChar(input))
            {
                //入力が完了した時の処理
                ExecuteCardEffect(cardEntity.Data, GetPlayer?.Invoke());
                OnCardCompleted?.Invoke(cardEntity);
            }
        }

        public void ExecuteCardEffect(CardData cardData, IAttackable player, params IHitable[] targets)
        {
            if (player == null)
            {
                Debug.LogError("Player is null");
                return;
            }

            ICardContent[] contents = cardData.Contents;

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

        private readonly CardDrawer _cardDrawer;
    }
}