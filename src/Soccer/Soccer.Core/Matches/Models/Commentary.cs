namespace Soccer.Core.Matches.Models
{
    using MessagePack;
    using Score247.Shared.Base;

    [MessagePackObject]
    public class Commentary : BaseModel
    {       
#pragma warning disable S109 // Magic numbers should not be used
        [Key(2)]
#pragma warning restore S109 // Magic numbers should not be used
        public string Text { get; set; }
    }
}