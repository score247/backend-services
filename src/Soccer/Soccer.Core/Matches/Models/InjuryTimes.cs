using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Matches.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class InjuryTimes
    {
        public InjuryTimes() { }

        [JsonConstructor]
        public InjuryTimes(
            int firstHaft,
            int secondHalf,
            int firstHaftExtra,
            int secondHalfExtra)
        {
            FirstHaft = firstHaft;
            SecondHalf = secondHalf;
            FirstHaftExtra = firstHaftExtra;
            SecondHalfExtra = secondHalfExtra;
        }

        public int FirstHaft { get; }

        public int SecondHalf { get; }

        public int FirstHaftExtra { get; }

        public int SecondHalfExtra { get; }
    }
}