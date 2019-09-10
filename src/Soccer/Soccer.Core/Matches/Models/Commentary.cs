namespace Soccer.Core.Matches.Models
{
    using MessagePack;
    using Score247.Shared.Base;

    [MessagePackObject]
    public class Commentary : BaseModel
    {
        [Key(2)]
        public string Text { get; set; }
    }
}