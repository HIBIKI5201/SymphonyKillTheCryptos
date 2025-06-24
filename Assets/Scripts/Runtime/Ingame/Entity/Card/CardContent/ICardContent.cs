using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    public interface ICardContent
    {
        public void TriggerEnterContent(GameObject player, params GameObject[] target);
    }
}