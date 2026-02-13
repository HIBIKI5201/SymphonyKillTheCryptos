using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Outgame.Card;
using Cryptos.Runtime.Entity.System.SaveData;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cryptos.Runtime.Entity.Ingame.Card; // DeckCardEntityのために追加

namespace Cryptos.Runtime.UseCase.OutGame
{
    /// <summary>
    ///     プレイヤーデッキの管理を行うユースケースである。
    /// </summary>
    public class PlayerDeckUseCase
    {
        private readonly PlayerDeckSaveData _playerDeckSaveData;

        /// <summary>
        ///     PlayerDeckUseCaseの新しいインスタンスを生成する。
        /// </summary>
        /// <param name="playerDeckSaveData">プレイヤーのデッキ保存データ。</param>
        public PlayerDeckUseCase(PlayerDeckSaveData playerDeckSaveData)
        {
            _playerDeckSaveData = playerDeckSaveData;
        }

        public void Initialize(DeckData[] datas)
        {
            if (_playerDeckSaveData == null) { return; }

            for (int i = 0; i < datas.Length; i++)
            {
                DeckData data = datas[i];
                if (!_playerDeckSaveData.IsExist(data.name))
                {
                    RegisterDeck(data.name, data.cardAddressValues);
                }
            }
        }

        /// <summary>
        ///     指定された名前のデッキを取得する。
        /// </summary>
        /// <param name="deckName">取得するデッキの名前。</param>
        /// <returns>指定された名前のカードアドレスの配列。存在しない場合はnullを返す。</returns>
        public CardAddressValueObject[] GetDeck(DeckNameValueObject deckName)
        {
            return _playerDeckSaveData.GetDeck(deckName);
        }

        /// <summary>
        ///     保存されているすべてのデッキの名前を取得する。
        /// </summary>
        /// <returns>保存されているデッキの名前のコレクション。</returns>
        public IReadOnlyCollection<DeckNameValueObject> GetAllDeckNames()
        {
            return _playerDeckSaveData.GetAllDeckNames();
        }

        /// <summary>
        ///     デッキを登録または更新する。
        /// </summary>
        /// <param name="deckName">登録するデッキの名前。</param>
        /// <param name="deck">登録するカードアドレスの配列。</param>
        public void RegisterDeck(DeckNameValueObject deckName, CardAddressValueObject[] deck)
        {
            _playerDeckSaveData.RegisterDeck(deckName, deck);
            SaveExecuter.DeckSave();
        }

        public struct DeckData
        {
            public DeckData(DeckNameValueObject name, CardAddressValueObject[] cardAddressValues)
            {
                this.name = name;
                this.cardAddressValues = cardAddressValues;
            }

            public DeckNameValueObject name;
            public CardAddressValueObject[] cardAddressValues;
        }
    }
}
