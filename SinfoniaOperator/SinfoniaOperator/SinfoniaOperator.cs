using SinfoniaOperator.API.Discord;

namespace SinfoniaOperator.Main
{
    public static class SinfoniaOperator
    {
        public static void Main(string[] args)
        {
            DiscordEnvironmentVariables dicsordVariables = VariableProvider.GetDiscordVariables();
            DiscordCommandClient discordClient = new(dicsordVariables);
        }
    }
}
