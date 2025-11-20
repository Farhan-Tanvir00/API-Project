namespace WebAPI.Authority
{
    public static class AppRepository
    {
        private static List<Application> _applications = new List<Application>
        {
            new Application
            {
                ApplicationId = 1,
                ApplicationName = "Application1",
                ClientId = "shirts_management_client_id",
                Secret = "shirts_management_secret",
                Scopes = "Read, Write"
            },
            new Application
            {
                ApplicationId = 2,
                ApplicationName = "Application2",
                ClientId = "orders_management_client_id",
                Secret = "orders_management_secret",
                Scopes = "read, write"
            }
        };

        public static Application? GetApplicationByClientId(string clientId)
        {
            return _applications.FirstOrDefault(app => app.ClientId == clientId);
        }
    }
}
