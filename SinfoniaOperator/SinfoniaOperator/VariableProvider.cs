using SinfoniaOperator.API.Discord;
using SinfoniaOperator.API.Notion;

namespace SinfoniaOperator.Main
{
    internal static class VariableProvider
    {
        public static DiscordEnvironmentVariables GetDiscordVariables()
        {
            string? discordBotToken = Environment.GetEnvironmentVariable(DISCORD_BOT_TOKEN);
            string? discordChannelID = Environment.GetEnvironmentVariable(DISCORD_CHANNEL_ID);

            if (string.IsNullOrEmpty(discordBotToken)) { throw new ArgumentNullException($"{DISCORD_BOT_TOKEN} is invalid"); }
            if (string.IsNullOrEmpty(discordChannelID)) { throw new ArgumentNullException($"{DISCORD_CHANNEL_ID} is invalid"); }

            if (!ulong.TryParse(discordChannelID, out ulong id)) { throw new ArgumentException($"{discordChannelID} of {DISCORD_CHANNEL_ID} is not uint type"); }

            return new DiscordEnvironmentVariables(
                discordBotToken,
                id
                );
        }

        public static NotionEnvironmentVariables GetNotionVariables()
        {
            string? notionToken = Environment.GetEnvironmentVariable(NOTION_TOKEN);
            string? databaseID = Environment.GetEnvironmentVariable(NOTION_DATABASE_ID);
            string? datePropertyName = Environment.GetEnvironmentVariable(NOTION_DATABASE_DATE_PROPERTY);
            string? namePropertyName = Environment.GetEnvironmentVariable(NOTION_DATABASE_NAME_PROPERTY);
            string? statusPropertyName = Environment.GetEnvironmentVariable(NOTION_DATABASE_STATUS_PROPERTY);
            string? taskStatusDoneName = Environment.GetEnvironmentVariable(NOTION_DATABASE_STATUS_TASK_DONE_PROPERTY);

            if (string.IsNullOrEmpty(notionToken)) { throw new ArgumentNullException($"{NOTION_TOKEN} is invalid"); }
            if (string.IsNullOrEmpty(databaseID)) { throw new ArgumentNullException($"{NOTION_DATABASE_ID} is invalid"); }
            if (string.IsNullOrEmpty(datePropertyName)) { throw new ArgumentNullException($"{NOTION_DATABASE_DATE_PROPERTY} is invalid"); }
            if (string.IsNullOrEmpty(namePropertyName)) { throw new ArgumentNullException($"{NOTION_DATABASE_NAME_PROPERTY} is invalid"); }
            if (string.IsNullOrEmpty(statusPropertyName)) { throw new ArgumentNullException($"{NOTION_DATABASE_STATUS_PROPERTY} is invalid"); }
            if (string.IsNullOrEmpty(taskStatusDoneName)) { throw new ArgumentException($"{NOTION_DATABASE_STATUS_TASK_DONE_PROPERTY} is invalid"); }

            return new NotionEnvironmentVariables(
                notionToken,
                databaseID,
                datePropertyName,
                namePropertyName,
                statusPropertyName,
                taskStatusDoneName                
                );
        }

        private const string DISCORD_BOT_TOKEN = "DISCORD_BOT_TOKEN";
        private const string DISCORD_CHANNEL_ID = "DISCORD_CHANNEL_ID";
        private const string NOTION_TOKEN = "NOTION_TOKEN";
        private const string NOTION_DATABASE_ID = "NOTION_DATABASE_ID";
        private const string NOTION_DATABASE_DATE_PROPERTY = "NOTION_DATABASE_DATE_PROPERTY";
        private const string NOTION_DATABASE_NAME_PROPERTY = "NOTION_DATABASE_NAME_PROPERTY";
        private const string NOTION_DATABASE_STATUS_PROPERTY = "NOTION_DATABASE_STATUS_PROPERTY";
        private const string NOTION_DATABASE_STATUS_TASK_DONE_PROPERTY = "NOTION_DATABASE_STATUS_TASK_DONE_PROPERTY";
    }
}
