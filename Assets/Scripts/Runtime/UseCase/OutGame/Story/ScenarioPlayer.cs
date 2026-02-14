using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Outgame.Story;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptos.Runtime.UseCase.OutGame.Story
{
    public class ScenarioPlayer
    {
        public ScenarioPlayer(ScenarioData scenarioData,
            ScenarioActionRepository actionRep,
            IStoryMessageViewModel viewModel,
            IPauseSubject pauseSubject)
        {
            _scenario = scenarioData;
            _actionRep = actionRep;
            _vm = viewModel;
            _pauseSub = pauseSubject;
        }

        public event Action<int> OnMoveNext;

        public ValueTask<bool> MoveNextAsync()
        {
            if (TryCancel())
            {
                return new(true);
            }

            _cts = new();
            ValueTask<bool> task = NextNodeAsync(_cts.Token);
            _task = task.AsTask();

            return task;
        }

        public void MoveIndex(int index)
        {
            _currentNodeIndex = index - 1; //入力の一つ前まで完了したことにする。
        }

        public bool TryCancel()
        {
            if (_cts == null)
            {
                return false;
            }

            if (_task != null && !_task.IsCompleted)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
                return true;
            }

            return false;
        }

        private readonly ScenarioData _scenario;
        private readonly ScenarioActionRepository _actionRep;
        private readonly IStoryMessageViewModel _vm;
        private readonly IPauseSubject _pauseSub;

        private CancellationTokenSource _cts;
        private Task _task;
        private int _currentNodeIndex = -1;
        private List<ValueTask> _tasks = new();

        private async ValueTask<bool> NextNodeAsync(CancellationToken token = default)
        {
            ScenarioNode node = null;

            do
            {
                _currentNodeIndex++;

                if (_scenario.Length <= _currentNodeIndex)
                {
                    return false;
                }

                OnMoveNext?.Invoke(_currentNodeIndex);

                node = _scenario[_currentNodeIndex];

                ValueTask textTask = _vm.SetTextAsync(node.Name, node.Text, token);
                _tasks.Add(textTask);

                foreach (IScenarioAction action in node.ScenarioActions)
                {
                    ValueTask task = action.ExecuteAsync(_actionRep, _pauseSub, token);
                    _tasks.Add(task);
                }

                try
                {
                    for (int i = 0; i < _tasks.Count; i++)
                    {
                        await _tasks[i];
                    }
                }
                finally
                {
                    _tasks.Clear();
                }
            }
            while (node != null && !node.IsWaitForInput);

            return true;
        }
    }
}
