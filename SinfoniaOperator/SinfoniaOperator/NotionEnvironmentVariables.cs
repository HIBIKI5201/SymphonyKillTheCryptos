namespace SinfoniaOperator.API.Notion
{
    internal readonly struct NotionEnvironmentVariables
    {
        public NotionEnvironmentVariables(
            string token,
            string databaseID,
            string datePropertyName,
            string namePropertyName,
            string statusPropertyName,
            string taskStatusDoneName)
        {
            _token = token;
            _databaseID = databaseID;
            _datePropertyName = datePropertyName;
            _namePropertyName = namePropertyName;
            _statusPropertyName = statusPropertyName;
            _taskStatusDoneName = taskStatusDoneName;
        }

        public string Token => _token;
        public string DatabaseID => _databaseID;
        public string DatePropertyName => _datePropertyName;
        public string NamePropertyName => _namePropertyName;
        public string StatusPropertyName => _statusPropertyName;
        public string TaskStatusDoneName => _taskStatusDoneName;

        private readonly string _token;
        private readonly string _databaseID;
        private readonly string _datePropertyName;
        private readonly string _namePropertyName;
        private readonly string _statusPropertyName;
        private readonly string _taskStatusDoneName;
    }
}
