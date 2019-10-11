namespace Soccer.Core.Matches.Models
{
    using MessagePack;

    [MessagePackObject]
    public class Commentary 
    {       
#pragma warning disable S109 // Magic numbers should not be used
        [Key(0)]
#pragma warning restore S109 // Magic numbers should not be used
        public string Text { get; set; }
    }
}