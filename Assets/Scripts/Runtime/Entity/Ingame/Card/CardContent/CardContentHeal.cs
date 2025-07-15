using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardContentHeal : ICardContent
    {
        [SerializeField]
        private float _healAmount;

        public void Execute(IAttackable player, params IHitable[] target)
        {
            Debug.Log($"CardContentHeal: Player {player.gameObject.name} heals for {_healAmount} amount.");
            // ここに実際の回復処理を記述
        }
    }
}