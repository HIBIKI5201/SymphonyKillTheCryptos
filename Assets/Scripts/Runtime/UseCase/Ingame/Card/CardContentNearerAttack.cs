using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.CombatSystem;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    public class CardContentNearerAttack : CardContentBaseAttack
    {
        public override void Execute(ICharacter[] players, ICharacter[] targets)
        {
            IAttackable attaker = players[0];
            IHittable hitter = targets[0];

            CombatContext context = CombatProcessor.Execute(attaker, hitter, _handlers);
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