using Cryptos.Runtime.UseCase.Ingame.CombatSystem;
using UnityEngine;

namespace Cryptos.Runtime.Presenter
{
    public class CombatPipelineWrapper : ScriptableObject
    {
        public CombatPipelineWrapper(ICombatHandler[] combatHandlers)
        {
            _combatHaldlers = combatHandlers;
        }

        public ICombatHandler[] CombatHandlers => _combatHaldlers;

        ICombatHandler[] _combatHaldlers;
    }
}
