using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace NGnono.Example.Web.Local4Mvc
{
    public class LocalizationAttribute : ActionFilterAttribute
    {
        private const string LangCookieName = "__lang.CurrentUICulture";
        private const string LangRouteName = "lang";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.RouteData.Values[LangRouteName] != null &&
                     !String.IsNullOrWhiteSpace(filterContext.RouteData.Values[LangRouteName].ToString()))
            {
                //从路由数据(url)里设置语言
                var lang = filterContext.RouteData.Values[LangRouteName].ToString();
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);
            }
            else
            {
                //从cookie里读取语言设置
                var cookie = filterContext.HttpContext.Request.Cookies[LangCookieName];
                var langHeader = String.Empty;
                if (cookie != null)
                {
                    //根据cookie设置语言
                    langHeader = cookie.Value;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                }
                else
                {
                    //如果读取cookie失败则设置默认语言
                    if (filterContext.HttpContext.Request.UserLanguages != null)
                        langHeader = filterContext.HttpContext.Request.UserLanguages[0];
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                }
                //把语言值设置到路由值里
                filterContext.RouteData.Values[LangRouteName] = langHeader;
            }

            // 把设置保存进cookie
            var setCookie = new HttpCookie(LangCookieName, Thread.CurrentThread.CurrentUICulture.Name)
            {
                Expires = DateTime.Now.AddYears(1)
            };
            filterContext.HttpContext.Response.SetCookie(setCookie);

            base.OnActionExecuting(filterContext);
        }
    }

    public static class SwitchLanguageHelper
    {
        public class Language
        {
            public string Url { get; set; }
            public string ActionName { get; set; }
            public string ControllerName { get; set; }
            public RouteValueDictionary RouteValues { get; set; }
            public bool IsSelected { get; set; }

            public MvcHtmlString HtmlSafeUrl
            {
                get
                {
                    return MvcHtmlString.Create(Url);
                }
            }
        }

        public static Language LanguageUrl(this HtmlHelper helper, string cultureName, string languageRouteName = "lang", bool strictSelected = false)
        {
            // 设置输入的语言为小写
            cultureName = cultureName.ToLower();
            // 从view context中获取retrieve the route values from the view context
            var routeValues = new RouteValueDictionary(helper.ViewContext.RouteData.Values);
            // copy the query strings into the route values to generate the link
            var queryString = helper.ViewContext.HttpContext.Request.QueryString;
            foreach (string key in queryString)
            {
                if (queryString[key] != null && !String.IsNullOrWhiteSpace(key))
                {
                    if (routeValues.ContainsKey(key))
                    {
                        routeValues[key] = queryString[key];
                    }
                    else
                    {
                        routeValues.Add(key, queryString[key]);
                    }
                }
            }
            var actionName = routeValues["action"].ToString();
            var controllerName = routeValues["controller"].ToString();
            // set the language into route values
            routeValues[languageRouteName] = cultureName;
            // generate the language specify url
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            var url = urlHelper.RouteUrl("Localization", routeValues);
            // check whether the current thread ui culture is this language
            var currentLangName = Thread.CurrentThread.CurrentUICulture.Name.ToLower();
            var isSelected = strictSelected ? currentLangName == cultureName : currentLangName.StartsWith(cultureName);

            return new Language
             {
                 Url = url,
                 ActionName = actionName,
                 ControllerName = controllerName,
                 RouteValues = routeValues,
                 IsSelected = isSelected
             };
        }

        public static MvcHtmlString LanguageSelectorLink(this HtmlHelper helper, string cultureName, string selectedText, string unselectedText, IDictionary<string, object> htmlAttributes, string languageRouteName = "lang", bool strictSelected = false)
        {
            var language = helper.LanguageUrl(cultureName, languageRouteName, strictSelected);
            var link = helper.RouteLink(language.IsSelected ? selectedText : unselectedText, "Localization", language.RouteValues, htmlAttributes);

            return link;
        }
    }
}