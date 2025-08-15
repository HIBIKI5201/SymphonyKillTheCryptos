using Cryptos.Runtime.Entity.Ingame.Character;
using System.Text;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    /// <summary>
    /// カード効果として、ターゲットの体力を回復するクラスです。
    /// </summary>
    public class CardContentHeal : ICardContent
    {
        /// <summary>
        /// ターゲットの体力を指定量だけ回復します。
        /// </summary>
        /// <param name="players">効果の主体となるプレイヤーキャラクターの配列。</param>
        /// <param name="targets">回復の対象となるキャラクターの配列。</param>
        public void Execute(ICharacter[] players, ICharacter[] targets)
        {
            StringBuilder sb = new StringBuilder($"CardContentHeal: <b>{players[0]}</b> heals for <b>{_healAmount}</b> amount.");
            foreach(var t in targets)
            {
                t.AddHealthHeal(_healAmount);

                sb.Append($" target : {t}");
            }

            Debug.Log(sb.ToString());
        }

        [SerializeField, Min(1), Tooltip("回復量。")]
        private float _healAmount = 10;
    }
}
