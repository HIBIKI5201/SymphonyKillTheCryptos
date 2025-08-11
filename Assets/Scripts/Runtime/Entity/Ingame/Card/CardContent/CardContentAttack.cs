using Cryptos.Runtime.Entity.Ingame.Character;
using System.Text;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardContentAttack : ICardContent
    {
        [SerializeField, Min(0)]
        private float _damageScale = 1;

        public void Execute(IAttackable player, params IHitable[] targets)
        {
            StringBuilder sb = new($"CardContentAttack: Player <b>{player}</b>\n");

            float power = player.GetAttackPower() * _damageScale;
            sb.Append($"attacks targets with {power} damage.");

            foreach (var t in targets)
            {
                t.AddHealthDamage(power);

                sb.Append($"\nTarget: {t}");
            }

            Debug.Log(sb.ToString());
        }
    }
}