using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Matches.Models
{
    [MessagePackObject]
    public class Commentary
    {
        [JsonConstructor]
        public Commentary(string text)
        {
            Text = text;
        }

#pragma warning disable S109 // Magic numbers should not be used

        [Key(0)]
#pragma warning restore S109 // Magic numbers should not be used
        public string Text { get; }
    }
}