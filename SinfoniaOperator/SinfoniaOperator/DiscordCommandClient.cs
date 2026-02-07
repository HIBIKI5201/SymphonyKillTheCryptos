using Discord;
using Discord.WebSocket;
using SinfoniaOperator.Utility;

namespace SinfoniaOperator.API.Discord
{
    internal class DiscordCommandClient
    {
        public DiscordCommandClient(DiscordEnvironmentVariables variables)
        {
            _variables = variables;
            DiscordSocketConfig config = new()
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.DirectMessages
            };
            _client = new(config);
        }

        public async Task Awake()
        {
            TaskCompletionSource<None> readyTcs = new();

            _client.Ready += () =>
            {
                readyTcs.SetResult(default);
                return Task.CompletedTask;
            };

            _client.Log += msg =>
            {
                Console.WriteLine(msg.ToString());
                return Task.CompletedTask;
            };

            try
            {
                await _client.LoginAsync(TokenType.Bot, _variables.BotToken);
                await _client.StartAsync();
                await readyTcs.Task;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Discordボットの起動に失敗しました: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        ///     タスクリストの文字列をDiscordに出力します。
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async ValueTask PushTaskListAsync(string content)
        {
            if (_client.GetChannel(_variables.ChannelID) is not IMessageChannel channel)
            {
                Console.WriteLine($"id:{_variables.ChannelID} のチャンネルがメッセージチャンネルにキャストできませんでした");

                return;
            }

            await channel.SendMessageAsync(content);
        }

        private readonly DiscordEnvironmentVariables _variables;
        private readonly DiscordSocketClient _client;
    }
}
