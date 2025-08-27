using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.CombatSystem;
using System.Text;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    /// <summary>
    /// カード効果として、ターゲットに攻撃を行うクラスです。
    /// </summary>
    public class CardContentAllAttack : ICardContent
    {
        /// <summary>
        /// プレイヤーからターゲットに対して攻撃を実行します。
        /// ダメージ計算はCombatProcessorを介して行われます。
        /// </summary>
        /// <param name="players">攻撃の主体となるプレイヤーキャラクターの配列。</param>
        /// <param name="targets">攻撃の対象となるキャラクターの配列。</param>
        public void Execute(ICharacter[] players, ICharacter[] targets)
        {
            StringBuilder sb = new($"CardContentAttack: Player <b>{players[0]}</b>\n");

            IAttackable player = players[0];

            foreach (var target in targets)
            {
                CombatContext context = CombatProcessor.Execute(player, target, Handlers);

                target.AddHealthDamage(context);

                sb.Append($"\nTarget: {target}");
            }

            Debug.Log(sb.ToString());
        }

        [SerializeField, Min(0), Tooltip("ダメージの倍率。")]
        private float _damageScale = 1;

        private ICombatHandler[] Handlers
        {
            get
            {
                return new ICombatHandler[]
                {
                    new CriticalCalcHandler(),
                    new MultiplyHandler(_damageScale)
                };
            }
        }
    }
}
