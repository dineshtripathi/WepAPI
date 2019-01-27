using System.Web;
using System.Web.Mvc;

namespace Flex.Entity.Api
{
    /// <summary>
    /// Change the Filter Config during startup
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Add Application Insight to the filter chain.
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandler.AiHandleErrorAttribute());
        }
    }
}
