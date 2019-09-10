using MessagePack;

namespace Score247.Shared.Base
{
    using MessagePack;

    [MessagePackObject]
    public abstract class BaseModel
    {
        [Key(0)]
        public string Id { get; set; }

        [Key(1)]
        public string Name { get; set; }
    }
}