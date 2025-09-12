namespace Cryptos.Runtime.Entity.Ingame.Word
{
    public class WordMatcher
    {
        public WordMatcher(string word)
        {
            _word = word.ToUpper(); //判定用に大文字で保存
        }

        public int Count => _count;
        public bool IsCompleted => _word.Length <= _count;

        public bool ProccessInput(char c)
        {
            if (IsCompleted) return false;

            char upperC = char.ToUpper(c);

            // 文字がマッチしているか
            if (TryNext(upperC))
            {
                return true;
            }

            ResetCount(); // 間違っていたらリセットされる

            // 再度文字がマッチしているか
            if (TryNext(upperC))
            {
                return true;
            }

            return false;
        }

        public void ResetCount()
        {
            _count = 0;
        }

        private readonly string _word;

        private int _count;

        private bool TryNext(char c)
        {
            if (_word[_count] == c)
            {
                _count++;
                return true;
            }

            return false;
        }
    }
}
