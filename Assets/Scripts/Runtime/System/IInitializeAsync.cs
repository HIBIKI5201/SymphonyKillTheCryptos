using System.Threading.Tasks;

namespace BeatKeeper.Runtime.System
{
    /// <summary>
    ///     非同期で初期化するインターフェース
    /// </summary>
    public interface IInitializeAsync
    {
        public Task InitializeTask { get; protected set; }
        public bool IsDone => InitializeTask != null ? InitializeTask.IsCompleted : false;

        /// <summary>
        ///     初期化を開始する
        /// </summary>
        /// <returns></returns>
        public async Task DoInitialize()
        {
            if (IsDone) return; //再初期化をブロック

            InitializeTask = InitializeAsync();
            await InitializeTask;

            InitializeTask = Task.CompletedTask; //軽量なTaskに変更
        }

        /// <summary>
        ///     初期化のフローを実装する
        /// </summary>
        /// <returns></returns>
        protected Task InitializeAsync();
    }
}