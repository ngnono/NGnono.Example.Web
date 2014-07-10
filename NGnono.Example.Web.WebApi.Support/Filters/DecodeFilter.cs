using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace NGnono.Example.Web.WebApi.Support.Filters
{
    public class DecodeFilter : ActionFilterAttribute
    {
        private static readonly List<HttpMethod> SupportMethods = new List<HttpMethod>
            {
                HttpMethod.Post ,
                HttpMethod.Put,
                HttpMethod.Get
            };


        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            // If the request is either a PUT or POST, attempt to decode all strings//System.Net.WebRequestMethods.Http.Post
            if (!SupportMethods.Contains(actionContext.Request.Method)) return;

            // For each of the items in the PUT/POST
            foreach (var key in actionContext.ActionArguments.Keys)
            {
                //try
                //{
                // Get the type of the object
                //1.string
                var val = actionContext.ActionArguments[key];

                var type = val.GetType();

                //is string
                if (type == typeof (string))
                {
                    if (val == null)
                    {
                        continue;
                    }

                    actionContext.ActionArguments[key] = WebUtility.HtmlDecode(val.ToString());
                }

                // For each property of this object, html decode it if it is of type string
                //foreach (var propertyInfo in type.GetProperties())
                //{
                //    var prop = propertyInfo.GetValue(item);
                //    if (prop == null)
                //    {
                //        continue;
                //    }

                //    var s = prop as string;
                //    if (s != null)
                //    {
                //        propertyInfo.SetValue(item, WebUtility.HtmlDecode(s));
                //    }
                //}
                //}
                //catch { }
            }
        }
    }
}
