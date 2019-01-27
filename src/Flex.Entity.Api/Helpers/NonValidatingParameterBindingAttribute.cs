using System;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Flex.Entity.Api.Helpers
{
    /// <summary>
    /// An attribute to disable WebApi model validation for a particular type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    internal sealed class NonValidatingParameterBindingAttribute : ParameterBindingAttribute
    {
        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {
            return parameter.BindWithFormatter(parameter.Configuration.Formatters, bodyModelValidator: null);
        }
    }
}