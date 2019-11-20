using System.Collections.Generic;

namespace DBUp.Deployment.Models
{
    public class ConnectionConfiguration
    {
        public List<ConnectionItem> Connections { get; set; }
    }

    public class ConnectionItem
    {
        private static string ConnectionPattern = "Data Source={0};Port={1};Initial Catalog={2};Persist Security Info=True;User ID=root;Password=1234AA@PASS;Allow User Variables=True;";

        public string Host { get; set; }

        public string Port { get; set; }

        public string Database { get; set; }

        public override string ToString() => string.Format(ConnectionPattern, Host, Port, Database);
    }
}
