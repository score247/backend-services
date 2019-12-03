using DbUp.Engine;

namespace DBUp.Deployment.PreProcessors
{
    public class DatabaseNamePreProcessor : IScriptPreprocessor
    {
        private readonly string DatabaseName;
        private readonly string Environment;

        public DatabaseNamePreProcessor(string databaseName, string environment) 
        {
            DatabaseName = databaseName;
            Environment = environment;
        }

        public string Process(string contents)
        {
            var formattedContents = contents.Replace("score247db", DatabaseName);
            formattedContents = formattedContents.Replace("environment", Environment);

            return formattedContents;
        }
    }
}
