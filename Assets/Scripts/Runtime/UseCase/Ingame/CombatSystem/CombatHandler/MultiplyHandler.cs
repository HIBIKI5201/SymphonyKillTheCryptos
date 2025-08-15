using Cryptos.Runtime.Entity.Ingame.Character;

namespace Cryptos.Runtime.UseCase.Ingame.CombatSystem
{
    internal class MultiplyHandler : ICombatHandler
    {
        public MultiplyHandler(float damageScale)
        {
            _damageScale = damageScale;
        }

        public CombatContext Execute(CombatContext context)
        {
            float newDamage = context.Damage * _damageScale;

            return new CombatContext(context.AttackerData, context.TargetData, newDamage);
        }

        private float _damageScale;
    }
}
