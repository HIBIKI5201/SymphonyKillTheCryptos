using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardContentHeal : ICardContent
    {
        [SerializeField]
        private float _healAmount;

        public void TriggerEnterContent(GameObject player, params GameObject[] target)
        {
            //プレイヤーを回復する
        }
    }
}