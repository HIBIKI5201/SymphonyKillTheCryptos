namespace SinfoniaOperator.API.Discord
{
    internal struct DiscordEnvironmentVariables
    {
        public DiscordEnvironmentVariables(
            string botToken,
            ulong channelID)
        {
            _botToken = botToken;
            _channelID = channelID;
        }

        public string BotToken => _botToken;
        public ulong ChannelID => _channelID;

        private readonly string _botToken;
        private readonly ulong _channelID;
    }
}
