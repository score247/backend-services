using DbUp.Engine;
using System;

namespace DBUp.Deployment.PreProcessors
{
    public class DelimiterPreProcessor : IScriptPreprocessor
    {
        public string Process(string contents)
        {
            var startDelimiter = "DELIMITER $$" + Environment.NewLine;
            var endDelimiter = Environment.NewLine + "DELIMITER ;";

            var newContents = $"{startDelimiter} {ReplaceLastOccurrence(contents, "END", "END$$")} {endDelimiter}";

            return newContents;
        }

        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find, StringComparison.OrdinalIgnoreCase);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
    }
}
