using System.Web;
using System.Web.Mvc;

namespace NGnono.Example.Web.Local4Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}