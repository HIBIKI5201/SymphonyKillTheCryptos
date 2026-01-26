using Cryptos.Runtime.Framework;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Character
{
    /// <summary>
    ///     キャラクターの静的なステータス情報を保持するScriptableObjectです。
    /// </summary>
    [CreateAssetMenu(fileName = nameof(CharacterData), menuName = CryptosPathConstant.ASSET_PATH + nameof(CharacterData), order = 1)]
    public class CharacterData : ScriptableObject, ICharacterData
    {
        /// <summary>
        /// キャラクターの名前を取得します。
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// 攻撃力を取得します。
        /// </summary>
        public float AttackPower => _attackPower;

        /// <summary>
        /// クリティカルヒットの確率を取得します（%）。
        /// </summary>
        public float CriticalChance => _criticalChance;

        /// <summary>
        /// クリティカルヒット時のダメージ倍率を取得します。
        /// </summary>
        public float CriticalDamage => _criticalDamage;

        /// <summary>
        /// 最大体力を取得します。
        /// </summary>
        public float MaxHealth => _maxHealth;

        /// <summary>
        /// 防御力を取得します（割合軽減）。
        /// </summary>
        public float Armor => _armor;

        [SerializeField, Tooltip("敵キャラクターの名前。")]
        private string _name = "Enemy";
        [SerializeField, Tooltip("攻撃力。")]
        private float _attackPower = 10;
        [SerializeField, Tooltip("クリティカル確率（%）。")]
        private float _criticalChance = 3;
        [SerializeField, Tooltip("クリティカルダメージ倍率。")]
        private float _criticalDamage = 2;
        [SerializeField, Tooltip("最大体力。")]
        private float _maxHealth = 100;
        [SerializeField, Tooltip("防御力（割合軽減）。")]
        private float _armor = 25;
    }
}

