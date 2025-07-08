using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardContentAttack : ICardContent
    {
        [SerializeField]
        private float _damage;

        public void TriggerEnterContent(GameObject player, params GameObject[] target)
        {
            //ターゲットにダメージを与える
        }
    }
}