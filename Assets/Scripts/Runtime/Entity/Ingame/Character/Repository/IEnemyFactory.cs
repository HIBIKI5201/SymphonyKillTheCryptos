namespace Cryptos.Runtime.Entity.Ingame.Character.Repository
{
    /// <summary>
    /// 敵エンティティを生成するためのファクトリーインターフェースです。
    /// 依存性逆転の原則により、Entity層にインターフェースを定義し、
    /// UseCase層で実装することで循環依存を回避します。
    /// </summary>
    public interface IEnemyFactory
    {
        /// <summary>
        /// 敵のエンティティを生成します。
        /// </summary>
        /// <param name="data">敵のデータ</param>
        /// <returns>生成された敵エンティティ</returns>
        CharacterEntity Create(CharacterData data);
    }
}

