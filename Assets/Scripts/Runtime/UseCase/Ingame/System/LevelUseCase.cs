using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;
using UnityEngine;

public class LevelUseCase
{
    public LevelUseCase(SymphonyData data)
    {
        _levelEntity = new LevelEntity();
        _data = data;
    }

    public void AddLevelProgress(WaveEntity waveEntity)
    {
        float levelProgress = waveEntity.WaveExperiencePoint / 100f;
        _levelEntity.AddLevelProgress(levelProgress);
    }

    private LevelEntity _levelEntity;
    private SymphonyData _data;
}
