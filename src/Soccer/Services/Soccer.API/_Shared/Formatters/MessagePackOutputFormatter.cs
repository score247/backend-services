using System;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;

namespace Soccer.API._Shared.Formatters
{
    public class MessagePackOutputFormatter : IOutputFormatter
    {
        private const string MessagePackAcceptType = "application/x-msgpack";
        private const string TextAcceptType = "text/plain";
        private const string Accept = "Accept";
        private readonly IFormatterResolver resolver;

        public MessagePackOutputFormatter() : this(null)
        {
        }

        public MessagePackOutputFormatter(IFormatterResolver resolver)
        {
            this.resolver = resolver ?? MessagePackSerializer.DefaultResolver;
        }

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            StringValues values;
            if (context.HttpContext.Request.Headers.TryGetValue(Accept, out values)
                && values.Count > 0
                && (values[0].Contains(MessagePackAcceptType, StringComparison.InvariantCultureIgnoreCase)
                    || values[0].Contains(TextAcceptType, StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }

            return false;
        }

        public Task WriteAsync(OutputFormatterWriteContext context)
        {
            context.HttpContext.Response.ContentType = MessagePackAcceptType;

            if (context.ObjectType == typeof(object))
            {
                if (context.Object == null)
                {
                    context.HttpContext.Response.Body.WriteByte(MessagePackCode.Nil);
                    return Task.CompletedTask;
                }
                else
                {
                    MessagePackSerializer.NonGeneric.Serialize(context.Object.GetType(), context.HttpContext.Response.Body, context.Object, resolver);
                    return Task.CompletedTask;
                }
            }
            else
            {
                //TODO: Ricky must review here
                var syncIOFeature = context.HttpContext.Features.Get<IHttpBodyControlFeature>();
                if (syncIOFeature != null)
                {
                    syncIOFeature.AllowSynchronousIO = true;
                }

                MessagePackSerializer.NonGeneric.Serialize(context.ObjectType, context.HttpContext.Response.Body, context.Object, resolver);

                return Task.CompletedTask;
            }
        }
    }
}