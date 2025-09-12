using Cryptos.Runtime.Entity.Utility;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        /// <param name="value">バフの値。(%) </param>
        /// <param name="priority"> バフの優先度。 </param>
        public void SetNewBuff(BuffType type, float value, int priority = 0)
        {
            switch (type)
            {
                case BuffType.AttackPower: _attackPower.AddMultiplier(value, priority); break;
                case BuffType.CriticalChance: _criticalChance.AddMultiplier(value, priority); break;
                case BuffType.CriticalDamage: _criticalDamage.AddMultiplier(value, priority); break;
                case BuffType.MaxHealth: _maxHealth.AddMultiplier(value, priority); break;
                case BuffType.Armor: _armor.AddMultiplier(value, priority); break;
            }

            BuffLog(type);
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

        /// <summary>
        ///     バフのログを出力
        /// </summary>
        /// <param name="type"></param>
        [Conditional("UNITY_EDITOR")]
        private void BuffLog(BuffType type)
        {
            switch (type)
            {
                case BuffType.AttackPower: Debug.Log($"{_name} is buffed AttackPower {_attackPower.Value}"); break;
                case BuffType.CriticalChance: Debug.Log($"{_name} is buffed CriticalChance {_criticalChance.Value}"); break;
                case BuffType.CriticalDamage: Debug.Log($"{_name} is buffed CriticalDamage {_criticalDamage.Value}"); break;
                case BuffType.MaxHealth: Debug.Log($"{_name} is buffed MaxHealth {_maxHealth.Value}"); break;
                case BuffType.Armor: Debug.Log($"{_name} is buffed Armor {_armor.Value}"); break;
                default: Debug.LogWarning($"{_name} is buffed unknown"); break;
            }
        }
    }
}
