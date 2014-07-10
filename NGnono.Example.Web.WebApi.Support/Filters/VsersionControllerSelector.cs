using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Filters;

namespace NGnono.Example.Web.WebApi.Support.Filters
{
    /// <summary>
    /// 不符合 业务的实现方式
    /// </summary>
    public class VersionControllerSelector : DefaultHttpControllerSelector
    {
        //config.Services.Replace(typeof(IHttpControllerSelector), new VsersionControllerSelector(config));

        //private HttpConfiguration _config;
        public VersionControllerSelector(HttpConfiguration config)
            : base(config)
        {
            // _config = config;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllers = GetControllerMapping(); //Will ignore any controls in same name even if they are in different namepsace

            var routeData = request.GetRouteData();
            if (string.IsNullOrWhiteSpace(routeData.Route.RouteTemplate))
            {
                return base.SelectController(request);
            }
            var controllerName = routeData.Values["controller"].ToString();

            HttpControllerDescriptor controllerDescriptor;

            if (controllers.TryGetValue(controllerName, out controllerDescriptor))
            {

                //var version = GetVersionFromQueryString(request);
                //var version = GetVersionFromHeader(request);
                var version = GetVersion(request);

                if (!String.IsNullOrEmpty(version))
                {
                    var versionedControllerName = String.Concat(controllerName, "V", version);

                    HttpControllerDescriptor versionedControllerDescriptor;
                    if (controllers.TryGetValue(versionedControllerName, out versionedControllerDescriptor))
                    {
                        return versionedControllerDescriptor;
                    }
                }
                return controllerDescriptor;
            }

            return null;
        }

        private string GetVersion(HttpRequestMessage request)
        {
            var hv1 = GetVersionFromHttpHeader(request);
            if (!String.IsNullOrEmpty(hv1))
            {
                return hv1;
            }

            var hv2 = GetVersionFromAcceptHeaderVersion(request);
            if (!String.IsNullOrEmpty(hv2))
            {
                return hv2;
            }

            var hv3 = GetVersionFromQueryString(request);
            if (!String.IsNullOrEmpty(hv3))
            {
                return hv3;
            }

            return String.Empty;
        }

        private static string GetVersionFromAcceptHeaderVersion(HttpRequestMessage request)
        {
            var acceptHeader = request.Headers.Accept;

            foreach (var mime in acceptHeader)
            {
                if (mime.MediaType == "application/json")
                {
                    var version = mime.Parameters.FirstOrDefault(v => v.Name.Equals("version", StringComparison.OrdinalIgnoreCase));

                    if (version != null)
                    {
                        return version.Value;
                    }
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        private static string GetVersionFromHttpHeader(HttpRequestMessage request)
        {
            const string headerName = "X-api-version";

            if (request.Headers.Contains(headerName))
            {
                var versionHeader = request.Headers.GetValues(headerName).FirstOrDefault();
                if (versionHeader != null)
                {
                    return versionHeader;
                }
            }

            return String.Empty;
        }

        private string GetVersionFromQueryString(HttpRequestMessage request)
        {
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);

            var version = query["v"];

            if (version != null)
            {
                return version;
            }

            return String.Empty;
        }
    }


    public class VresionActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            base.OnActionExecuting(actionContext);
        }
    }


    /// <summary>
    /// 用户信息
    /// </summary>
    [DataContract]
    public class UserProfile
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 系统管理员,如果是系统管理员可以管理所有门店和专柜
        /// </summary>
        [DataMember]
        public bool IsSystem { get; set; }

        /// <summary>
        /// 专柜列表
        /// </summary>
        [DataMember]
        [System.Obsolete("暂时不可用")]
        public IEnumerable<int> SectionIds { get; set; }

        /// <summary>
        /// 门店列表
        /// </summary>
        [DataMember]
        public IEnumerable<int> StoreIds { get; set; }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        [DataMember]
        public IEnumerable<string> Roles { get; set; }
    }


    internal struct FieldDefine
    {
        public static string UserProfilePropertiesName = "UserProfile";
    }

    public class UserProfileMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            //保存UserProfile
            request.Properties.Add(FieldDefine.UserProfilePropertiesName, new UserProfile
                {
                    Id = 9999,
                    StoreIds = new int[] { }
                });


            return base.SendAsync(request, cancellationToken);
        }
    }


    public class SimpleDataRoleAttribute : ActionFilterAttribute
    {
        private readonly string _userProfileName;
        private readonly string _validParamsName;

        public SimpleDataRoleAttribute(string validParamsName)
            : this(validParamsName, FieldDefine.UserProfilePropertiesName)
        {
        }

        public SimpleDataRoleAttribute(string validParamsName, string userProfileName)
        {
            _userProfileName = userProfileName;
            _validParamsName = validParamsName;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            object obj;
           
            if (!actionContext.Request.Properties.TryGetValue(_userProfileName, out obj))
            {
                throw new ArgumentNullException(_userProfileName);
            }

            var userProfile = obj as UserProfile;
            if (userProfile == null)
            {
                throw new InvalidCastException(_userProfileName);
            }

            if (String.IsNullOrEmpty(_validParamsName))
            {
                return;
            }

            if (userProfile.StoreIds == null)
            {
                return;
            }

            var param = actionContext.Request.RequestUri.ParseQueryString();
            if (param.AllKeys.Contains(_validParamsName))
            {
                var valid = param[_validParamsName];

                var a = Int32.Parse(valid);

                if (userProfile.StoreIds.Contains(a))
                {
                    return;
                }
            }
            else
            {
                throw new ArgumentNullException(_validParamsName);
            }

            throw new HttpRequestValidationException();
        }
    }
}