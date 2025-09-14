using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.CombatSystem
{
    /// <summary>
    ///     ターゲットの防御力によるダメージ減衰を適用する。
    /// </summary>
    public class ArmorCalcHandler : ICombatHandler
    {
        public CombatContext Execute(CombatContext context)
        {
            float armor = context.TargetData.Armor;
            float reduction = _armorThreshold / (_armorThreshold + armor); // f/(f+a)でa=fの時に減衰率50%になる。

            float damage = context.Damage * (1 - reduction);
            return new CombatContext(context, damage);
        }

        [SerializeField, Tooltip("装甲値がこの値と等しい場合、減衰率が50%になる基準点")]
        private float _armorThreshold = 100;
    }
}
