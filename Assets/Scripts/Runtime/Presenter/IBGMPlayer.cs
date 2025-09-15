namespace Cryptos.Runtime.Presenter.System.Audio
{
    /// <summary>
    ///     BGMを再生するクラスのインターフェース。
    /// </summary>
    public interface IBGMPlayer
    {
        /// <summary>
        ///     指定されたキュー名のBGMを再生する。
        /// </summary>
        /// <param name="cueName"></param>
        public void PlayBGM(string cueName);
    }
}
