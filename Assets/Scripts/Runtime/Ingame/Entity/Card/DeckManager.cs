using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    public class DeckManager : MonoBehaviour
    {
        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordData;

        private List<CardInstance> _deckCard = new();

        public void AddCardToDeck(CardData data)
        {
            List<WordData> words = new ();

            //カードの難易度までのワードを追加
            for (int i = 0; i < data.CardDifficulty; i++) 
            {
                words.Concat(_wordData.WordData[i]);
            }

            CardInstance instance = new(data, words.ToArray());
            _deckCard.Add(instance);
        }
    }
}