using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.Character.Repository;
using Cryptos.Runtime.Entity.Ingame.Word;
using Cryptos.Runtime.UseCase.Ingame.Card;
using System.Linq;

namespace Cryptos.Runtime.InfraStructure.Ingame.Utility
{
    public static class CardInitializer
    {
        public static CardInitializationData Initialize(
            WordDataBase wordDataBase,
            CharacterEntity symphony,
            EnemyRepository enemyRepo)
        {
            CardUseCase cardUseCase = new(wordDataBase, new());
            cardUseCase.GetPlayer += () => symphony;
            cardUseCase.GetTargets += () => enemyRepo.AllEnemies.ToArray();

            return new(cardUseCase);
        }

        public readonly struct CardInitializationData
        {
            public CardInitializationData(CardUseCase cardUseCase)
            {
                _cardUseCase = cardUseCase;
            }

            public CardUseCase CardUseCase => _cardUseCase;

            private readonly CardUseCase _cardUseCase;
        }
    }
}
