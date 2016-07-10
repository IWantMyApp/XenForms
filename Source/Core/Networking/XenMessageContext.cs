using System;
using XenForms.Core.Messages;

namespace XenForms.Core.Networking
{
    /// <summary>
    /// Encapsulates a single toolbox request and designer response.
    /// </summary>
    public class XenMessageContext
    {
        public string Message { get; set; }
        public XenMessage Request { get; set; }
        public XenMessage Response { get; set; }
        internal bool ShouldQueue { get; set; }


        public XenMessageContext()
        {
            Request = null;
            Response = null;
        }


        public XenMessageContext(XenMessage request, XenMessage response)
        {
            Request = request;
            Response = response;
        }
    }


    public static class XenMessageContextExtensions
    {
        public static T Get<T>(this XenMessageContext ctx) where T : XenMessage
        {
            var request = ctx.Request as T;

            if (request != null)
            {
                return request;
            }

            var response = ctx.Response as T;
            return response;
        }


        public static T SetResponse<T>(this XenMessageContext ctx, Action<T> action = null) where T : Response, new()
        {
            var response = XenMessage.Create<T>();
            ctx.Response = response;

            action?.Invoke(response);

            return (T) ctx.Response;
        }


        public static T SetRequest<T>(this XenMessageContext ctx, Action<T> action = null) where T : Request, new()
        {
            var request = XenMessage.Create<T>();
            ctx.Request = request;

            action?.Invoke(request);

            return (T) ctx.Request;
        }
    }
}