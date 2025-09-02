using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    /// <summary>
    ///     レベル管理を行うユースケース。
    /// </summary>
    public class LevelUseCase
    {
        public LevelUseCase(SymphonyData data, float[] levelRequirePoints)
        {
            _levelEntity = new LevelEntity(levelRequirePoints);
            _data = data;
        }

        /// <summary>
        ///     ウェーブクリア時に経験値を追加します。
        /// </summary>
        /// <param name="waveEntity"></param>
        public void AddLevelProgress(WaveEntity waveEntity)
        {
            _levelEntity.AddLevelProgress(waveEntity.WaveExperiencePoint);
        }

        private LevelEntity _levelEntity;
        private SymphonyData _data;
    }
}