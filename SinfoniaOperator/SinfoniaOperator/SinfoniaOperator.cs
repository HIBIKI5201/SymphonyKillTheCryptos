using SinfoniaOperator.API.Discord;
using SinfoniaOperator.API.Notion;

namespace SinfoniaOperator.Main
{
    public static class SinfoniaOperator
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Start Sinfonia Operator");
            DiscordEnvironmentVariables discordVariables = default;
            NotionEnvironmentVariables notionEnvironmentVariables = default;
            try
            {
                 discordVariables = VariableProvider.GetDiscordVariables();
                notionEnvironmentVariables = VariableProvider.GetNotionVariables();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            DiscordCommandClient discordClient = new(discordVariables);
            NotionCommandClient notionClient = new(notionEnvironmentVariables);

            await discordClient.Awake();
            Console.WriteLine("Discord Client Awaked");

            string context = await notionClient.GetTaskContent();

            await discordClient.PushTaskListAsync(context);
            Console.WriteLine("Sent Message");
        }
    }
}
