namespace Cryptos.Runtime.Entity.Outgame.Story
{
    public interface IScenarioActionRepository
    {
        ICameraRepository CameraRepository { get; }
        ICharacterRepository CharacterRepository { get; }
    }
}
