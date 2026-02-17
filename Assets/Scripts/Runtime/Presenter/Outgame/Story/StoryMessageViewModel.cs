using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Outgame.Story;
using Cryptos.Runtime.UseCase.OutGame.Story;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Presenter
{
    public class StoryMessageViewModel : IStoryMessageViewModel
    {
        public StoryMessageViewModel(NovelSettingEntity setting,
            IPauseSubject ps)
        {
            _setting = setting;
            _pauseSubject = ps;
        }

        public event Action<string, string> OnMessageReceived;

        public async ValueTask SetTextAsync(string name, string text, CancellationToken token = default)
        {
            _name = name;

            float showLength = 0;
            while (showLength < text.Length)
            {
                showLength += _setting.TextSpeed * Time.deltaTime;

                // 次に表示する文字数を計算。
                int nextShowLength = Mathf.Min(
                        (int)showLength, //速度に応じた数。
                        text.Length); // 最大文字数。

                SetText(text[..nextShowLength]);

                // 全文字が表示されていたら終了。
                if (text.Length == showLength) { return; }

                try // 1フレーム待機。
                {
                    await Awaitable.NextFrameAsync(token);

                    // ポーズ中は停止する。
                    await _pauseSubject.WaitResume(token);
                }
                catch (OperationCanceledException)
                {
                    // 文字送りがスキップされた場合には全表示して終了する。
                    SetText(text);
                    return;
                }
            }
        }

        private readonly NovelSettingEntity _setting;
        private readonly IPauseSubject _pauseSubject;

        private string _name;
        private string _text;

        private void SetText(string text)
        {
            _text = text;
            OnMessageReceived?.Invoke(_name, _text);
        }
    }
}
