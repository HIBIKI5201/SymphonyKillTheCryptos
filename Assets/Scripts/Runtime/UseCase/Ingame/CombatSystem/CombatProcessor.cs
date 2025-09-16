using Cryptos.Runtime.Entity.Ingame.Character;
using System.Collections.Generic;
using System.Linq;

namespace Cryptos.Runtime.UseCase.Ingame.CombatSystem
{
    /// <summary>
    /// 戦闘における一連のダメージ計算処理を統括するクラスです。
    /// 複数のICombatHandlerを登録し、順番に実行することで最終的なダメージを算出します。
    /// </summary>
    public class CombatProcessor
    {
        /// <summary>
        /// 登録された全ての戦闘ハンドラーを実行し、最終的な戦闘結果を計算します。
        /// </summary>
        /// <param name="attacker">攻撃側のキャラクター。</param>
        /// <param name="target">防御側のキャラクター。</param>
        /// <returns>全ての計算が完了した後の最終的なCombatContext。</returns>
        public static CombatContext Execute(IAttackable attacker, IHittable target, params ICombatHandler[] handlers)
        {
            CombatContext context = new CombatContext(attacker.AttackableData, target.HittableData, attacker.AttackableData.AttackPower);

            foreach (var handler in handlers)
            {
                context = handler.Execute(context);
            }

            return context;
        }

        public class HandlerData
        {
            public ICombatHandler[] GetAddHandlers()
            {
                // Add側を (キー, インスタンス, 種類) で展開
                var addHandlers = _addHHandlers
                    .SelectMany(kvp => kvp.Value.Select(v => new { Key = kvp.Key, Type = 0, Handler = v }));

                // Multiply側を (キー, インスタンス, 種類) で展開
                var multiplyHandlers = _multiplyHandler
                    .SelectMany(kvp => kvp.Value.Select(v => new { Key = kvp.Key, Type = 1, Handler = v }));

                // 両方を結合 → Key昇順, Type昇順(add=0が先)
                var merged = addHandlers
                    .Concat(multiplyHandlers)
                    .OrderBy(x => x.Key)
                    .ThenBy(x => x.Type)
                    .Select(x => x.Handler)
                    .ToArray();

                return merged;
            }

            public HandlerData AddHandler(HandlerType type, int id, ICombatHandler handler)
            {
                if (type == HandlerType.Add)
                {
                    if (!_addHHandlers.TryGetValue(id, out var handlers))
                    {
                        handlers = new ICombatHandler[0];
                    }
                    var newHandlers = new List<ICombatHandler>(handlers) { handler };
                    _addHHandlers[id] = newHandlers.ToArray();
                }
                else if (type == HandlerType.Multiply)
                {
                    if (!_multiplyHandler.TryGetValue(id, out var handlers))
                    {
                        handlers = new ICombatHandler[0];
                    }
                    var newHandlers = new List<ICombatHandler>(handlers) { handler };
                    _multiplyHandler[id] = newHandlers.ToArray();
                }

                return this;
            }

            private SortedDictionary<int, ICombatHandler[]> _addHHandlers = new();
            private SortedDictionary<int, ICombatHandler[]> _multiplyHandler = new();
        }

        public enum HandlerType
        {
            Multiply,
            Add,
        }
    }
}
