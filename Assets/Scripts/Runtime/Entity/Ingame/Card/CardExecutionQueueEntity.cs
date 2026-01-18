using System.Collections.Generic;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    /// <summary>
    /// カード実行のキューを管理するエンティティである。
    /// </summary>
    public class CardExecutionQueueEntity
    {
        private readonly Queue<CardEntity> _queue = new();

        public int Count => _queue.Count;

        public void Enqueue(CardEntity card)
        {
            _queue.Enqueue(card);
        }

        public bool TryDequeue(out CardEntity card)
        {
            return _queue.TryDequeue(out card);
        }

        public bool TryPeek(out CardEntity card)
        {
            return _queue.TryPeek(out card);
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }
}
