using Cryptos.Runtime.Entity.Ingame.Character;
using System.Collections.Generic;

namespace Cryptos.Runtime.UseCase.Ingame.CombatSystem
{
    internal class CombatProssesor
    {
        private List<ICombatHandler> _combatHandlers = new();

        public CombatProssesor AddTo(ICombatHandler handler)
        {
            _combatHandlers.Add(handler);
            return this;
        }

        public CombatContext Excute(IAttackable attacker, IHitable target)
        {
            CombatContext context = new CombatContext(attacker.AttackableData, target.HitableData, attacker.AttackableData.AttackPower);

            foreach(var handler in _combatHandlers)
            {
                context = handler.Execute(context);
            }

            return context;
        }
    }
}
