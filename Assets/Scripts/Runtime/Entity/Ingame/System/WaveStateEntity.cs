namespace Cryptos.Runtime.Entity.Ingame.System
{
    /// <summary>
    /// 現在のウェーブの状態を管理するエンティティである。
    /// </summary>
    public class WaveStateEntity
    {
        /// <summary>
        /// 生存している敵の数。
        /// </summary>
        public int AliveEnemyCount { get; private set; }

        /// <summary>
        /// ウェーブが完了したかどうか。
        /// </summary>
        public bool IsWaveCompleted => AliveEnemyCount <= 0;

        /// <summary>
        /// WaveStateEntityの新しいインスタンスを初期化する。
        /// </summary>
        /// <param name="initialEnemyCount">初期の敵の数。</param>
        public WaveStateEntity(int initialEnemyCount)
        {
            AliveEnemyCount = initialEnemyCount;
        }

        /// <summary>
        /// 生存している敵の数を1減らす。
        /// </summary>
        public void DecrementEnemyCount()
        {
            if (AliveEnemyCount > 0)
            {
                AliveEnemyCount--;
            }
        }
    }
}
