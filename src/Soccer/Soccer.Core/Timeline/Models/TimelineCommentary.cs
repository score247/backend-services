using System.Collections.Generic;
using MessagePack;
using Score247.Shared.Base;
using Soccer.Core.Matches.Models;

namespace Soccer.Core.Timeline.Models
{
    [MessagePackObject]
    public class TimelineCommentary : BaseModel
    {       
        [Key(2)]
        public long TimelineId { get; set; }

        [Key(3)]
        public IReadOnlyList<Commentary> Commentaries { get; set; }
    }
}
