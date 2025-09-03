using Cryptos.Runtime.Entity.Ingame.System;
using System;
using System.Threading.Tasks;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    /// <summary>
    ///     レベル管理を行うユースケース。
    /// </summary>
    public class LevelUseCase
    {
        public LevelUseCase(LevelUpgradeData data,
            Func<string[], Task<string>> onLevelUpSelect)
        {
            _levelEntity = new LevelEntity(data.LevelRequirePoints);

            _data = data;
            _onLevelUpSelect = onLevelUpSelect;
        }

        /// <summary>
        ///     ウェーブクリア時に経験値を追加します。
        /// </summary>
        /// <param name="waveEntity"></param>
        public bool AddLevelProgress(WaveEntity waveEntity)
        {
            return _levelEntity.AddLevelProgress(waveEntity.WaveExperiencePoint);
        }

        public async Task<string> LevelUpSelectAsync()
        {
            string selectedCard = await _onLevelUpSelect?.Invoke(_data.LevelCard);

            return selectedCard;
        }

        private LevelEntity _levelEntity;
        private LevelUpgradeData _data;

        private Func<string[], Task<string>> _onLevelUpSelect;
    }
}