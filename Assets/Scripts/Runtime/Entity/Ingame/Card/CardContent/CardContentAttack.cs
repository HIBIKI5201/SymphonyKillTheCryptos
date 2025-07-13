using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardContentAttack : ICardContent
    {
        [SerializeField]
        private float _damage;

        public void TriggerEnterContent(GameObject player, params GameObject[] target)
        {
            Debug.Log($"CardContentAttack: Player {player.name} attacks target(s) with {_damage} damage.");
            foreach (var t in target)
            {
                Debug.Log($"  Target: {t.name}");
                // ここに実際のダメージ処理を記述
            }
        }
    }
}