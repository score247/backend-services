using MessagePack;
using Newtonsoft.Json;

namespace Score247.Shared.Base
{
    [MessagePackObject]
    public class BaseModel
    {
        [SerializationConstructor, JsonConstructor]
        protected BaseModel(string id, string name)
        {
            Id = id;
            Name = name;
        }

        [Key(0)]
        public string Id { get; }

        [Key(1)]
        public string Name { get; }
    }
}