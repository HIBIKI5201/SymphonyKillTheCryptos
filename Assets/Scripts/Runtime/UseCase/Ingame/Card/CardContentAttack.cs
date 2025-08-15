using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.CombatSystem;
using System.Text;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardContentAttack : ICardContent
    {
        public CardContentAttack()
        {
            _combatProssesor = new CombatProcessor()
                .AddTo(new CriticalCalcHandler());
        }

        public void Execute(ICharacter[] players, ICharacter[] targets)
        {
            StringBuilder sb = new($"CardContentAttack: Player <b>{players}</b>\n");

            IAttackable player = players[0];

            foreach (var terget in targets)
            {
                CombatContext context = _combatProssesor.Excute(player, terget);

                terget.AddHealthDamage(context);

                sb.Append($"\nTarget: {terget}");
            }

            Debug.Log(sb.ToString());
        }

        [SerializeField, Min(0)]
        private float _damageScale = 1;

        private CombatProcessor _combatProssesor;
    }
}