using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardContentAttack : ICardContent
    {
        [SerializeField]
        private float _damageScale;

        public void Execute(IAttackable player, params IHitable[] targets)
        {
            Debug.Log($"CardContentAttack: Player <b>{player.gameObject.name}</b> attacks targets with {_damageScale} damage.");
            foreach (var t in targets)
            {
                Debug.Log($"  Target: {t.gameObject.name}");
                // ここに実際のダメージ処理を記述
            }
        }
    }
}