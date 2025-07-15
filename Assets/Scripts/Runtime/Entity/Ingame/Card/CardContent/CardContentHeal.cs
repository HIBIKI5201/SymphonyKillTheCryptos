using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    public class CardContentHeal : ICardContent
    {
        [SerializeField]
        private float _healAmount;

        public void Execute(IAttackable player, params IHitable[] target)
        {
            Debug.Log($"CardContentHeal: Player {player.gameObject.name} heals for {_healAmount} amount.\nhealth is {target[0].HitableData.Health}");
            // ここに実際の回復処理を記述
        }
    }
}