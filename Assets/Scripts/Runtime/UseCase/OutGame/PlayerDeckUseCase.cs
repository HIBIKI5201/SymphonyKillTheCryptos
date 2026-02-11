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
        private readonly ICardRepository _cardRepository; // 追加

        /// <summary>
        ///     PlayerDeckUseCaseの新しいインスタンスを生成する。
        /// </summary>
        /// <param name="playerDeckSaveData">プレイヤーのデッキ保存データ。</param>
        /// <param name="cardRepository">カードリポジトリ。</param> // 追加
        public PlayerDeckUseCase(PlayerDeckSaveData playerDeckSaveData, ICardRepository cardRepository) // 引数追加
        {
            _playerDeckSaveData = playerDeckSaveData;
            _cardRepository = cardRepository; // 追加
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
            SaveExecuter.DeckSave(); // 保存処理を呼び出す
        }

        /// <summary>
        ///     カードアドレスからDeckCardEntityを生成する。
        /// </summary>
        /// <param name="cardAddress">カードアドレス。</param>
        /// <returns>生成されたDeckCardEntity。</returns>
        public async Task<DeckCardEntity> CreateDeckCardEntity(CardAddressValueObject cardAddress)
        {
            var deckCardEntity = await _cardRepository.GetDeckCardEntityAsync(cardAddress); // GetDeckCardEntityAsync を呼び出す

            if (deckCardEntity == null)
            {
                Debug.LogError($"DeckCardEntity not found or invalid for address: {cardAddress.Value}");
                return null;
            }

            return deckCardEntity;
        }
    }
}
