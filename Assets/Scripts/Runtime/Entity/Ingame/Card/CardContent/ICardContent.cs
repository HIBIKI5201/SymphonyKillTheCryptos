using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public interface ICardContent
    {
        public void TriggerEnterContent(GameObject player, params GameObject[] target);
    }
}