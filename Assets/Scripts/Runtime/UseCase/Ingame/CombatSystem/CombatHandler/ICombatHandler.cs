using Cryptos.Runtime.Entity.Ingame.Character;

namespace Cryptos.Runtime.UseCase.Ingame.CombatSystem
{
    /// <summary>
    /// 戦闘計算の各ステップを処理するハンドラーの共通インターフェースです。
    /// </summary>
    public interface ICombatHandler
    {
        /// <summary>
        /// 戦闘コンテキストを受け取り、特定の計算処理を適用した新しいコンテキストを返します。
        /// </summary>
        /// <param name="context">現在の戦闘コンテキスト。</param>
        /// <returns>計算処理適用後の新しい戦闘コンテキスト。</returns>
        public CombatContext Execute(CombatContext context);
    }
}
