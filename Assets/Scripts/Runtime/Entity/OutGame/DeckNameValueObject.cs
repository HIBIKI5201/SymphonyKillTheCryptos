using System;

namespace Cryptos.Runtime.Entity.Outgame.Card
{
    [Serializable] 
    public readonly struct DeckNameValueObject
    {
        public DeckNameValueObject(string value)
        {
            _value = value;
        }

        public string Value => _value;

        private readonly string _value;
    }
}
