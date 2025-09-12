using Cryptos.Runtime.Entity.Utility;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    ///     バフなどで一時的に変更されるプレイヤーのステータスを表すクラスです。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TentativeCharacterData : ICharacterData
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="data">キャラクターのデータ。</param>
        public TentativeCharacterData(ICharacterData data)
        {
            //DynamicFloat変数を生成する
            _attackPower = new(data.AttackPower);
            _criticalChance = new(data.CriticalChance);
            _criticalDamage = new(data.CriticalDamage);
            _maxHealth = new(data.MaxHealth);
            _armor = new(data.Armor);

            _name = data.Name;
        }

        /// <summary>
        /// プレイヤー名を取得します。
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// 攻撃力を取得します。
        /// </summary>
        public float AttackPower => _attackPower.Value;

        /// <summary>
        /// クリティカルヒットの確率を取得します（%）。
        /// </summary>
        public float CriticalChance => _criticalChance.Value;

        /// <summary>
        /// クリティカルヒット時のダメージ倍率を取得します。
        /// </summary>
        public float CriticalDamage => _criticalDamage.Value;

        /// <summary>
        /// 最大体力を取得します。
        /// </summary>
        public float MaxHealth => _maxHealth.Value;

        /// <summary>
        /// 防御力を取得します（割合軽減）。
        /// </summary>
        public float Armor => _armor.Value;

        /// <summary>
        /// 新しいバフを設定します。
        /// </summary>
        /// <param name="type">バフの種類。</param>
        /// <param name="value">バフの値。</param>
        public void SetNewBuff(BuffType type, float value)
        {
            const int MULTIPLY_PRIORITY = 0;

            switch (type)
            {
                case BuffType.AttackPower: _attackPower.AddMultiplier(value, MULTIPLY_PRIORITY); break;
                case BuffType.CriticalChance: _criticalChance.AddMultiplier(value, MULTIPLY_PRIORITY); break;
                case BuffType.CriticalDamage: _criticalDamage.AddMultiplier(value, MULTIPLY_PRIORITY); break;
                case BuffType.MaxHealth: _maxHealth.AddMultiplier(value, MULTIPLY_PRIORITY); break;
                case BuffType.Armor: _armor.AddMultiplier(value, MULTIPLY_PRIORITY); break;
            }

            Debug.Log($"{_name} StatusUpgrade\nvalue {value}");
        }

        /// <summary>
        /// バフの種類。
        /// </summary>
        public enum BuffType
        {
            AttackPower,
            CriticalChance,
            CriticalDamage,
            MaxHealth,
            Armor
        }

        private readonly string _name;

        private DynamicFloatVariable _attackPower;
        private DynamicFloatVariable _criticalChance;
        private DynamicFloatVariable _criticalDamage;
        private DynamicFloatVariable _maxHealth;
        private DynamicFloatVariable _armor;
    }
}
