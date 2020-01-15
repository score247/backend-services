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
            byte firstHaft,
            byte secondHalf,
            byte firstHaftExtra,
            byte secondHalfExtra)
        {
            FirstHaft = firstHaft;
            SecondHalf = secondHalf;
            FirstHaftExtra = firstHaftExtra;
            SecondHalfExtra = secondHalfExtra;
        }

        public byte FirstHaft { get; }

        public byte SecondHalf { get; }

        public byte FirstHaftExtra { get; }

        public byte SecondHalfExtra { get; }
    }
}