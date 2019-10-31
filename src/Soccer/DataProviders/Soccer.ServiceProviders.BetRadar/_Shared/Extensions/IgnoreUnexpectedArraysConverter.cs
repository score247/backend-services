using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Soccer.DataProviders.SportRadar._Shared.Extensions
{
    public class IgnoreUnexpectedArraysConverter<T> : IgnoreUnexpectedArraysConverterBase
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }
    }

    public class IgnoreUnexpectedArraysConverter : IgnoreUnexpectedArraysConverterBase
    {
        private readonly IContractResolver resolver;

        public IgnoreUnexpectedArraysConverter(IContractResolver resolver)
        {
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsPrimitive || objectType == typeof(string))
            {
                return false;
            }

            return resolver.ResolveContract(objectType) is JsonObjectContract;
        }
    }

    public abstract class IgnoreUnexpectedArraysConverterBase : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var contract = serializer.ContractResolver.ResolveContract(objectType);
            if (!(contract is JsonObjectContract))
            {
                throw new JsonSerializationException($"{objectType} is not a JSON object");
            }

            do
            {
                if (reader.TokenType == JsonToken.Null)
                {
                    return null;
                }
                else if (reader.TokenType == JsonToken.Comment)
                {
#pragma warning disable S3626 // Jump statements should not be redundant
                    continue;
#pragma warning restore S3626 // Jump statements should not be redundant
                }
                else if (reader.TokenType == JsonToken.StartArray)
                {
                    var array = JArray.Load(reader);

                    if (array.Count > 0)
                    {
                        throw new JsonSerializationException("Array was not empty.");
                    }

                    return null;
                }
                else if (reader.TokenType == JsonToken.StartObject)
                {
                    // Prevent infinite recursion by using Populate()
                    existingValue = existingValue ?? contract.DefaultCreator();
                    serializer.Populate(reader, existingValue);
                    return existingValue;
                }
                else
                {
                    throw new JsonSerializationException($"Unexpected token {reader.TokenType}");
                }
            }
            while (reader.Read());

            throw new JsonSerializationException("Unexpected end of JSON.");
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}