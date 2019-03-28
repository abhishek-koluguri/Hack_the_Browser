using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using CastleWindsor;
using Hack_the_Browser.CastleWindsor;
using Owin;

namespace Hack_the_Browser
{
    public class ApiStartup
    {
        /// <summary>
        /// This code configures Web API. The Startup class is specified as a type.
        ///  parameter in the WebApp.Start method.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        public void Configuration(IAppBuilder appBuilder)
        {

            var config = new HttpConfiguration
            {
                DependencyResolver = new WindsorDependencyResolver(CastleContainer.Default.Kernel)
            };

            var imageModelFormatter = new ImageModelFormatter();
            config.Formatters.Add(imageModelFormatter);

            RegisterServices(config);
            RegisterRoutes(config);
            RegisterFilters(config);

            HelpPageConfig.Register(config);
            //apply web api
            appBuilder.UseWebApi(config);
        }

        private static void RegisterFilters(HttpConfiguration config)
        {
            //config.Filters.Add(new WebApiPerformanceAttribute());
        }


        private static void RegisterServices(HttpConfiguration config)
        {
            //config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
            //config.Services.Add(typeof(IExceptionLogger), new GlobalExceptionLogger());
        }


        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="config">The configuration.</param>
        private static void RegisterRoutes(HttpConfiguration config)
        {

            //Upload Image and annotations
            config.Routes.MapHttpRoute(
              name: "UploadImage",
              routeTemplate: "api/Images",
              defaults: new { controller = "Images", action = "Upload" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );

            // Get Frame Image
            config.Routes.MapHttpRoute(
               name: "GetImage",
               routeTemplate: "api/Images/{referenceId}",
                   defaults: new { controller = "Images", action = "GetImage"},
                   constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                );

            config.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "api/{controller}/{id}"
                );
        }
    }
}
