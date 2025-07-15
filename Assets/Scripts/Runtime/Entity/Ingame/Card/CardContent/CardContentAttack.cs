using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardContentAttack : ICardContent
    {
        [SerializeField]
        private float _damage;

        public void Execute(IAttackable player, params IHitable[] target)
        {
            Debug.Log($"CardContentAttack: Player {player.gameObject.name} attacks target(s) with {_damage} damage.");
            foreach (var t in target)
            {
                Debug.Log($"  Target: {t.gameObject.name}");
                // ここに実際のダメージ処理を記述
            }
        }
    }
}