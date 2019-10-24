using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soccer.Core.Matches.Models;
using Svg;

namespace Soccer.API.Matches.Helpers
{
    public interface IMatchLineupsGenerator
    {
        string Generate(MatchLineups matchLineups);
    }

    public class MatchLineupsSvgGenerator : IMatchLineupsGenerator
    {
        private readonly string svgFolderPath;
        private readonly Func<string, SvgDocument> getSvgDocumentFunc;

        public MatchLineupsSvgGenerator(
            string svgFolderPath,
            Func<string, SvgDocument> getSvgDocumentFunc)
        {
            this.svgFolderPath = svgFolderPath;
            this.getSvgDocumentFunc = getSvgDocumentFunc;
        }

        public string Generate(MatchLineups matchLineups)
        {
            var lineupsDoc = getSvgDocumentFunc($"{svgFolderPath}/stadium.svg");

            lineupsDoc.FlushStyles(true);

            var lineupsSvg = lineupsDoc.GetXML();

            return lineupsSvg.Substring(lineupsSvg.IndexOf("<svg", StringComparison.OrdinalIgnoreCase));
        }
    }
}
