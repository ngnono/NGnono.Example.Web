using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ValueProviders;
using System.Web.ModelBinding;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;
using IValueProvider = System.Web.Http.ValueProviders.IValueProvider;
using ValueProviderResult = System.Web.Http.ValueProviders.ValueProviderResult;

namespace NGnono.Example.Web.WebApi.Support.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    public class NoLogAttribute : Attribute
    {

    }

    public class ExecInfoActionFilterAttribute : ActionFilterAttribute
    {
        public static int Count = 0;
        //private static string _outputFormat =
        //    "<h4>Debug Environment Info</h4><div class=\"debuginfo\"><table><tr><td>Web Server:</td><td>{0}</td></tr><tr><td>Browser:</td><td>{1}</td></tr><tr><td>Controller</td><td>{2}</td></tr><tr><td>Action:</td><td>{3}</td></tr><tr><td>Execution Time(ms):</td><td>{4}</td></tr></table></div>";

        private const string OutFormat = " exec{0}ms,total{1}times,startdt{2}";
        private const string DateFormat = "yyyy-MM-ddTHH:mm:ss.ffffffZ";
        private const string Key = "__action_duration__";
        private const string HeadKey = "x-exec-info";

        private static DateTime _startDateTime;

        static ExecInfoActionFilterAttribute()
        {
            _startDateTime = DateTime.Now;
        }

        #region methods


        private static bool SkipLogging(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<NoLogAttribute>().Any() ||
                    actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<NoLogAttribute>().Any();
        }

        private static void Start(HttpActionContext actionContext)
        {
            if (SkipLogging(actionContext))
            {
                return;
            }

            Count++;
            var stopWatch = new Stopwatch();
            actionContext.Request.Properties[Key] = stopWatch;
            stopWatch.Start();
        }

        private static void End(HttpActionExecutedContext actionExecutedContext)
        {
            if (!actionExecutedContext.Request.Properties.ContainsKey(Key))
            {
                return;
            }

            var stopWatch = actionExecutedContext.Request.Properties[Key] as Stopwatch;
            if (stopWatch != null)
            {
                stopWatch.Stop();
                var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
                var controllerName = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var elapsed = stopWatch.Elapsed;
                Debug.Print(string.Format("[Execution of{0}- {1} took {2}.]", controllerName, actionName, elapsed));

                actionExecutedContext.Response.Headers.TryAddWithoutValidation(HeadKey,
                                                                               String.Format(OutFormat,
                                                                                             elapsed.Milliseconds, Count,
                                                                                             _startDateTime.ToString(
                                                                                                 DateFormat)));
            }
        }

        #endregion


        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            Start(actionContext);

            base.OnActionExecuting(actionContext);
        }

        //public override System.Threading.Tasks.Task OnActionExecutingAsync(System.Web.Http.Controllers.HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        //{

        //    Start(actionContext);

        //    return base.OnActionExecutingAsync(actionContext, cancellationToken);
        //}


        //public override System.Threading.Tasks.Task OnActionExecutedAsync(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext, System.Threading.CancellationToken cancellationToken)
        //{
        //    End(actionExecutedContext);

        //    return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        //}


        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            End(actionExecutedContext);

            base.OnActionExecuted(actionExecutedContext);
        }
    }


    /// <summary>
    /// 参数初始化
    /// </summary>
    public class ParameterInitValueProvider : IValueProvider
    {
        private readonly Dictionary<string, string> _values;

        private static Dictionary<string, Type> _parameterFilter = new Dictionary<string, Type>();

        static ParameterInitValueProvider()
        {
            _parameterFilter.Add("StoreId", typeof(int?));
        }


        public ParameterInitValueProvider(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var key in actionContext.Request.Properties.Keys)
            {

            }
        }

        public bool ContainsPrefix(string prefix)
        {
            return _values.Keys.Contains(prefix);
        }

        public ValueProviderResult GetValue(string key)
        {
            string value;
            if (_values.TryGetValue(key, out value))
            {
                return new ValueProviderResult(value, value, CultureInfo.InvariantCulture);
            }
            return null;
        }
    }

    public class InitParameterValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(HttpActionContext actionContext)
        {
            return new ParameterInitValueProvider(actionContext);
        }
    }



    public class ModelBase
    {

    }


    public class Para : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var vals = actionContext.ActionArguments.Values;

            foreach (var val in vals)
            {
                if (val.GetType().IsAssignableFrom(typeof(ModelBase)))
                {

                }
            }



            base.OnActionExecuting(actionContext);
        }
    }


}
