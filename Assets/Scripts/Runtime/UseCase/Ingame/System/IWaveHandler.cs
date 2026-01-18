using System.Threading.Tasks;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    /// <summary>
    ///     Wave完了時のイベントを処理するインターフェースである。
    /// </summary>
    public interface IWaveHandler
    {
        /// <summary>
        ///     Waveが完了したことを通知する。
        /// </summary>
        Task OnWaveCompleted();
    }
}
