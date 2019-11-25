using DbUp.Engine;
using System;

namespace DBUp.Deployment.PreProcessors
{
    public class DatabaseNamePreProcessor : IScriptPreprocessor
    {
        private readonly string Environment;

        public DatabaseNamePreProcessor(string environment) 
        {
            Environment = environment;
        }

        public string Process(string contents)
        {
            return contents.Replace("score247db", $"score247_local_{Environment}");
        }
    }
}
