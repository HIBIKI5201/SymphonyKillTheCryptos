using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.CombatSystem;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    /// <summary>
    /// カード効果として、ターゲットに攻撃を行うクラスです。
    /// </summary>
    public class CardContentAllAttack : CardContentBaseAttack
    {
        public override void InitializeCombatHandler(ICombatHandler[] handlers)
        {
            ICombatHandler[] myHandlers = GetMyCombatHandler();
            _handlers = handlers.Concat(myHandlers).ToArray();
        }

        /// <summary>
        /// プレイヤーからターゲットに対して攻撃を実行します。
        /// ダメージ計算はCombatProcessorを介して行われます。
        /// </summary>
        /// <param name="players">攻撃の主体となるプレイヤーキャラクターの配列。</param>
        /// <param name="targets">攻撃の対象となるキャラクターの配列。</param>
        public override void Execute(ICharacter[] players, ICharacter[] targets)
        {
            StringBuilder sb = new($"CardContentAttack: Player <b>{players[0]}</b>\n");

            IAttackable player = players[0];

            foreach (var target in targets)
            {
                CombatContext context = CombatProcessor.Execute(player, target, _handlers);

                target.AddHealthDamage(context);

                sb.Append($"\nTarget: {target}");
            }

            Debug.Log(sb.ToString());
        }

        [SerializeField, Min(0), Tooltip("ダメージの倍率。")]
        private float _damageScale = 1;

        protected override ICombatHandler[] GetMyCombatHandler()
        {
            return new ICombatHandler[]
            {
                new MultiplyHandler(_damageScale)
            };
        }

    }
}
