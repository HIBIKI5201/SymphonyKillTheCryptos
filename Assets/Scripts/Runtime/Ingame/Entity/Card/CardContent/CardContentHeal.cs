using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
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