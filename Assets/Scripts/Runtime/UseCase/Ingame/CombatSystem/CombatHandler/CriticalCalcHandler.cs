using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.CombatSystem
{
    internal class CriticalCalcHandler : ICombatHandler
    {
        public CombatContext Execute(CombatContext context)
        {
            int criticalWeight = Mathf.FloorToInt(context.AttackerData.CriticalChance / 100);

            float remainCriticalChance = context.AttackerData.CriticalChance % 100;

            if (Random.Range(0, 100) < remainCriticalChance)
            {
                criticalWeight++;
            }

            float damageScale = 1 + (criticalWeight * context.AttackerData.CriticalDamage);
            float damage = context.Damage * damageScale;

            return new CombatContext(
                context.AttackerData,
                context.TargetData,
                damage
            );
        }
    }
}
