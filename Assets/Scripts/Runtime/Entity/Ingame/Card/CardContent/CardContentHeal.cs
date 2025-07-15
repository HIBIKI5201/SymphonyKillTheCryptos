using System.Text;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardContentHeal : ICardContent
    {
        [SerializeField]
        private float _healAmount;

        public void Execute(IAttackable player, params IHitable[] targets)
        {
            StringBuilder sb = new StringBuilder($"CardContentHeal: <b>{player.gameObject.name}</b> heals for <b>{_healAmount}</b> amount.");
            foreach(var t in targets)
            {
                t.AddHealthHeal(_healAmount);

                sb.Append($" target : {t.gameObject.name}");
            }

            Debug.Log(sb.ToString());

            // ここに実際の回復処理を記述
        }
    }
}