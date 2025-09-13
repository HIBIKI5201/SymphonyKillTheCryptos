using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.CombatSystem;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    public class CardContentRandomAttack : CardContentBaseAttack
    {
        public override void Execute(ICharacter[] players, ICharacter[] targets)
        {
            IAttackable attacker = players[0];
            int index = Random.Range(0, targets.Length);
            IHittable hitter = targets[index];

            CombatContext context = CombatProcessor.Execute(attacker, hitter, _handlers);
            hitter.AddHealthDamage(context);
        }

        protected override ICombatHandler[] GetMyCombatHandler()
        {
            return new ICombatHandler[]
            {
                new MultiplyHandler(_damageScale)
            };
        }

        [SerializeField, Min(1), Tooltip("ダメージ倍率")]
        private float _damageScale = 1;
    }
}
