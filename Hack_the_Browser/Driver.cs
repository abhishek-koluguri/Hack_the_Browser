using System;
using CastleWindsor;
using Hack_the_Browser.CastleWindsor;
using Hack_the_Browser.Config;
using Microsoft.Owin.Hosting;

namespace Hack_the_Browser
{
    public class Driver
    {
        private static IDisposable _webApplication;

        public static void Main(string[] args)
        {
            try
            {
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine("Starting ImageService.....please wait");
                log4net.Config.XmlConfigurator.Configure();

                IConfigManager configManager = CastleContainer.Resolve<IConfigManager>();
                ((IAsyncInitialization)configManager).Initialization.Wait();
                string baseAddress = "https://+:27071/";

                // Start OWIN host 
                var options = new StartOptions
                {
                    AppStartup = typeof(ApiStartup).AssemblyQualifiedName
                };
                options.Urls.Add(baseAddress);

                _webApplication = WebApp.Start<ApiStartup>(options);
                System.Console.WriteLine("Image Service Started sucessfully. Press <ENTER> to stop");
                System.Console.ReadKey();
                _webApplication.Dispose();
            }
            catch (Exception exception)
            {
                if (null != _webApplication) _webApplication.Dispose();
                System.Console.WriteLine(exception.Message);
            }
        }
    }
}
