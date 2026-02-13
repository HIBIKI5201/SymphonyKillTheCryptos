using System;
using Newtonsoft.Json; // 追加

namespace Cryptos.Runtime.Entity.Outgame.Card
{
    [Serializable] 
    public readonly struct DeckNameValueObject
    {
        [JsonConstructor]
        public DeckNameValueObject(string value)
        {
            _value = value;
        }

        [JsonProperty("Value")]
        public string Value => _value;

        private readonly string _value;
    }
}
