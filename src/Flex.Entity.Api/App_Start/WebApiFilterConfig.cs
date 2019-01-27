using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Flex.Entity.Api.CustomFilters;

namespace Flex.Entity.Api.App_Start
{
    /// <summary>
    /// 
    /// </summary>
    public class WebApiFilterConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            //config.Filters.Add(new ApplicationLogActionFilter());
        }
    }
}