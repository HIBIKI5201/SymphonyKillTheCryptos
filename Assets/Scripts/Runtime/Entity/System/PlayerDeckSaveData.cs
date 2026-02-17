using Cryptos.Runtime.Entity.Outgame.Card;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cryptos.Runtime.Entity.System.SaveData
{
    [Serializable]
    public class PlayerDeckSaveData
    {
        public static PlayerDeckSaveData Data;

        /// <summary>
        ///     デッキを登録または更新する。
        /// </summary>
        /// <param name="deckName">登録するデッキの名前。</param>
        /// <param name="deck">登録するカードアドレスの配列。</param>
        public void RegisterDeck(DeckNameValueObject deckName, CardAddressValueObject[] deck)
        {
            _deck[deckName] = deck;
        }

        /// <summary>
        ///     指定された名前のデッキを取得する。
        /// </summary>
        /// <param name="deckName">取得するデッキの名前。</param>
        /// <returns>指定された名前のカードアドレスの配列。存在しない場合はnullを返す。</returns>
        public CardAddressValueObject[] GetDeck(DeckNameValueObject deckName)
        {
            _deck.TryGetValue(deckName, out CardAddressValueObject[] deck);
            return deck;
        }

        /// <summary>
        ///     保存されているすべてのデッキの名前を取得する。
        /// </summary>
        /// <returns>保存されているデッキの名前のコレクション。</returns>
        public IReadOnlyCollection<DeckNameValueObject> GetAllDeckNames()
        {
            return _deck.Keys;
        }

        public static void Save()
        {
            // シリアライズできる形に整形する。
            Data._deckData = Data._deck.Select(
                kv => new SaveDataStruct(kv.Key.Value, kv.Value.Select(a => a.Value).ToArray())).ToArray();

            string text = JsonConvert.SerializeObject(Data);
            PlayerPrefs.SetString(typeof(PlayerDeckSaveData).Name, text);
            Debug.Log("[SaveDataSystem]\nデータをセーブしました\n" + text);
        }

        public static void Load()
        {
            string value = PlayerPrefs.GetString(typeof(PlayerDeckSaveData).Name);
            if (string.IsNullOrEmpty(value))
            {
                Debug.Log("[SaveDataSystem]\n" + typeof(PlayerDeckSaveData).Name + "のデータが見つからないので生成しました");
                Data = new PlayerDeckSaveData();
                return;
            }

            PlayerDeckSaveData saveData = JsonConvert.DeserializeObject<PlayerDeckSaveData>(value);
            if (saveData != null)
            {
                Debug.Log(string.Format("[{0}]\n{1}のデータがロードされました\n{2}", "SaveDataSystem", typeof(PlayerDeckSaveData).Name, saveData));
                Data = saveData;
                Data._deck = Data._deckData
                    .ToDictionary(kv => new DeckNameValueObject(kv.Name),
                    kv => kv.Value.Select(v => new CardAddressValueObject(v)).ToArray());
            }
            else
            {
                Debug.LogWarning("[SaveDataSystem]\n" + typeof(PlayerDeckSaveData).Name + "のロードが出来ませんでした\n新たなインスタンスを生成します");
                Data = new PlayerDeckSaveData();
            }
        }

        public bool IsExist(DeckNameValueObject deck) => _deck.ContainsKey(deck);

        private Dictionary<DeckNameValueObject, CardAddressValueObject[]> _deck = new();

        [JsonProperty]
        private SaveDataStruct[] _deckData = Array.Empty<SaveDataStruct>();

        [Serializable]
        private struct SaveDataStruct
        {
            public SaveDataStruct(string name, string[] value)
            {
                Name = name;
                Value = value;
            }

            public string Name;
            public string[] Value;
        }
    }
}
