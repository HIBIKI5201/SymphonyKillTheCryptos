using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.System.SaveData;
using System.Collections.Generic;

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

        /// <summary>
        ///     指定された名前のデッキを取得する。
        /// </summary>
        /// <param name="deckName">取得するデッキの名前。</param>
        /// <returns>指定された名前のカードアドレスの配列。存在しない場合はnullを返す。</returns>
        public CardAddressValueObject[] GetDeck(string deckName)
        {
            return _playerDeckSaveData.GetDeck(deckName);
        }

        /// <summary>
        ///     保存されているすべてのデッキの名前を取得する。
        /// </summary>
        /// <returns>保存されているデッキの名前のコレクション。</returns>
        public IReadOnlyCollection<string> GetAllDeckNames()
        {
            return _playerDeckSaveData.GetAllDeckNames();
        }

        /// <summary>
        ///     デッキを登録または更新する。
        /// </summary>
        /// <param name="deckName">登録するデッキの名前。</param>
        /// <param name="deck">登録するカードアドレスの配列。</param>
        public void RegisterDeck(string deckName, CardAddressValueObject[] deck)
        {
            _playerDeckSaveData.RegisterDeck(deckName, deck);
            SaveExecuter.DeckSave(); // 保存処理を呼び出す
        }

    }
}
