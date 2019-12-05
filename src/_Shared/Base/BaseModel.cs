using MessagePack;
using Newtonsoft.Json;

namespace Score247.Shared.Base
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class BaseModel
    {
        [SerializationConstructor, JsonConstructor]
        protected BaseModel(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; protected set; }

        public string Name { get; protected set; }

        public override bool Equals(object obj)
          => (obj is BaseModel actualObj) && Id == actualObj.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}