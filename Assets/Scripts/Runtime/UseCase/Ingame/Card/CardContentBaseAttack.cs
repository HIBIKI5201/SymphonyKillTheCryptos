using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.CombatSystem;
using System.Linq;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    public abstract class CardContentBaseAttack : ICardContent
    {
        public virtual void InitializeCombatHandler(ICombatHandler[] handlers)
        {
            ICombatHandler[] myHandlers = GetMyCombatHandler();
            ICombatHandler[] resultHandlers = handlers.Concat(myHandlers).ToArray();

            _handlers = resultHandlers;
        }

        public abstract void Execute(ICharacter[] players, ICharacter[] targets);

        protected ICombatHandler[] _handlers;
        protected abstract ICombatHandler[] GetMyCombatHandler();
    }
}