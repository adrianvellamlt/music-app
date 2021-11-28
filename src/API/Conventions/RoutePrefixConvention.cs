using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace API.Infrastructure
{
    public static class MvcOptionsExtensions
    {
        public static void UseGlobalRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            opts.Conventions.Add(new RoutePrefixConvention(routeAttribute));
        }

        public static void UseGlobalRoutePrefix(this MvcOptions opts, string prefix)
        {
            opts.UseGlobalRoutePrefix(new RouteAttribute(prefix));
        }
    }
    public class RoutePrefixConvention : IApplicationModelConvention
    {
        private AttributeRouteModel RoutePrefix { get; }

        public RoutePrefixConvention(IRouteTemplateProvider route)
        {
            RoutePrefix = new AttributeRouteModel(route);
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var selector in application.Controllers.SelectMany(c => c.Selectors))
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(RoutePrefix, selector.AttributeRouteModel);
                }
                else
                {
                    selector.AttributeRouteModel = RoutePrefix;
                }
            }
        }
    }
}