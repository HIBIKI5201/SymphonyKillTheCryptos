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
            StringBuilder sb = new StringBuilder($"CardContentHeal: Player {player.gameObject.name} heals for {_healAmount} amount.");
            foreach(var t in targets)
            {
                sb.Append($" target : {t.gameObject.name} health is {t.HitableData.Health}");
            }

            Debug.Log(sb.ToString());

            // ここに実際の回復処理を記述
        }
    }
}