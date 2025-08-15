using Cryptos.Runtime.Entity.Ingame.Character;

namespace Cryptos.Runtime.UseCase.Ingame.CombatSystem
{
    internal interface ICombatHandler
    {
        public CombatContext Execute(CombatContext context);
    }
}
