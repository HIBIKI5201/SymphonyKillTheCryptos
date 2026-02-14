using SymphonyFrameWork.Attribute;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Outgame.Story
{
    public class ScenarioNode
    {
        public ScenarioNode(string text, string name,
            bool isWaitForInput,
            IScenarioAction[] scenarioActions)
        {
            _text = text;
            _name = name;
            _isWaitForInput = isWaitForInput;
            _scenarioActions = scenarioActions;
        }

        public string Name => _name;
        public string Text => _text;
        public bool IsWaitForInput => _isWaitForInput;
        public IScenarioAction[] ScenarioActions => _scenarioActions;

        [SerializeField]
        private string _name = string.Empty;
        [SerializeField, TextArea(3, 10)]
        private string _text = string.Empty;
        [SerializeField]
        private bool _isWaitForInput = true;
        [SerializeReference, SubclassSelector]
        private IScenarioAction[] _scenarioActions = null;
    }
}
