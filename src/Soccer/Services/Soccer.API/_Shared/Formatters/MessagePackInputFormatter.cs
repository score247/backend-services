using System;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;

namespace Soccer.API._Shared.Formatters
{
    public class MessagePackInputFormatter : IInputFormatter
    {
        private const string MessagePackAcceptType = "application/x-msgpack";
        private const string TextAcceptType = "text/plain";
        private const string Accept = "Accept";
        private readonly IFormatterResolver resolver;

        public MessagePackInputFormatter() : this(null)
        {
        }

        public MessagePackInputFormatter(IFormatterResolver resolver)
        {
            this.resolver = resolver ?? MessagePackSerializer.DefaultResolver;
        }

        public bool CanRead(InputFormatterContext context)
        {
            if (context.HttpContext.Request.Headers.TryGetValue(Accept, out var values)
                && values.Count > 0
                && (values[0].Contains(MessagePackAcceptType, StringComparison.InvariantCultureIgnoreCase)
                    || values[0].Contains(TextAcceptType, StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }

            return false;
        }

        public Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var result = MessagePackSerializer.NonGeneric.Deserialize(context.ModelType, request.Body, resolver);
            return InputFormatterResult.SuccessAsync(result);
        }
    }
}