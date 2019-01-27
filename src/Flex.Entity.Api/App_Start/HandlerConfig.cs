using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Flex.Entity.Api.CustomFilters;
using Flex.Entity.Api.ErrorHandler;

namespace Flex.Entity.Api.App_Start
{
    /// <summary>
    /// 
    /// </summary>
    public class HandlerConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            //config.Services.Add(typeof (IExceptionHandler), new GlobalExceptionHandler());
            config.Services.Replace(typeof(IExceptionHandler), new OopsExceptionHandler(new GlobalExceptionHandler()));//
            config.MessageHandlers.Insert(0, new ResponseTransformHandler());

        }
    }
}