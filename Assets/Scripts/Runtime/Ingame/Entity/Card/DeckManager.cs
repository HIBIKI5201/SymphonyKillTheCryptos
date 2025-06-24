using Cryptos.Runtime.Ingame.UI;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using SymphonyFrameWork.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    public class DeckManager : MonoBehaviour, IInitializeAsync
    {
        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDatabase;

        Task IInitializeAsync.InitializeTask { get;  set; }
        private IngameUIManager _ingameUIManager;

        private readonly List<CardInstance> _deckCard = new();

        [SerializeField, Obsolete]
        private CardData _cardData;

        async Task IInitializeAsync.InitializeAsync()
        {
            _ingameUIManager = await ServiceLocator.GetInstanceAsync<IngameUIManager>();
        }

        private async void Start()
        {
            await SymphonyTask.WaitUntil(() => (this as IInitializeAsync).IsDone);
            AddCardToDeck(_cardData);
        }

        /// <summary>
        ///     カードを追加する
        /// </summary>
        /// <param name="data"></param>
        public void AddCardToDeck(CardData data)
        {
            List<WordData> words = new ();

            //カードの難易度までのワードを追加
            for (int i = 0; i <= data.CardDifficulty; i++) 
            {
                words.AddRange(_wordDatabase[i]);
            }

            if (words.Count <= 0)
            {
                Debug.LogError("ワードが一つもありません");
                return;
            }

            CardInstance instance = new(data, words.ToArray());
            _deckCard.Add(instance);

            _ingameUIManager?.UIElementDeck.AddCard(instance);
        }
    }
}