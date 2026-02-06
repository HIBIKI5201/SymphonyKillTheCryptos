
namespace Cryptos.Runtime.Entity.Utility
{
    /// <summary>
    /// ステータス変更の種類を定義します。
    /// </summary>
    public enum StatModType
    {
        /// <summary>
        /// 加算
        /// </summary>
        Additive,
        /// <summary>
        /// 乗算 (%)
        /// </summary>
        Multiplier
    }

    /// <summary>
    /// ステータスへの変更を表すイミュータブルなクラス。
    /// </summary>
    public class StatModifier
    {
        public float Value => _value;
        public StatModType Type => _type;
        public int Priority => _priority;

        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        /// <param name="value">変更する値。</param>
        /// <param name="type">変更の種類（加算または乗算）。</param>
        /// <param name="priority">乗算の場合の優先度。</param>
        public StatModifier(float value, StatModType type, int priority = 0)
        {
            _value = value;
            _type = type;
            _priority = priority;
        }

        private readonly float _value;
        private StatModType _type;
        private int _priority;

    }
}
