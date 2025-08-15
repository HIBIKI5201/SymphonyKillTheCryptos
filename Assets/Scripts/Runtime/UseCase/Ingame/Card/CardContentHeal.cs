using Cryptos.Runtime.Entity.Ingame.Character;
using System.Text;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardContentHeal : ICardContent
    {
        [SerializeField, Min(1)]
        private float _healAmount = 10;

        public void Execute(ICharacter[] players, ICharacter[] targets)
        {
            StringBuilder sb = new StringBuilder($"CardContentHeal: <b>{players[0]}</b> heals for <b>{_healAmount}</b> amount.");
            foreach(var t in targets)
            {
                t.AddHealthHeal(_healAmount);

                sb.Append($" target : {t}");
            }

            Debug.Log(sb.ToString());

            // ここに実際の回復処理を記述
        }
    }
}