using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Hack_the_Browser.Config;
using Hack_the_Browser.Devices;
using Hack_the_Browser.MetaDataRepositories;

namespace Hack_the_Browser.CastleWindsor
{
    /// <summary>
    /// Default Container for all objects created by Castle.
    /// </summary>
    public static class CastleContainer
    {

        /// <summary>
        /// Initializes the <see cref="CastleContainer"/> class.
        /// </summary>
        static CastleContainer()
        {
            IWindsorContainer container = new WindsorContainer();
            container.Install(FromAssembly.This());
            container.Register(Component.For<IConfigManager>().ImplementedBy(typeof(ConfigManager)).LifestyleSingleton());
            container.Register(Component.For<IDevice>().ImplementedBy(typeof(CacheDevice)).LifestyleSingleton());
            container.Register(Component.For<IImageDataRepository>().Instance(new MongoDbDataRepository("mongodb://localhost:27017/ImageService")).LifestyleSingleton());

            Default = container;
        }

        /// <summary>
        /// Gets the default container consumers of Castle Container should use.
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
        public static IWindsorContainer Default { get; private set; }

        public static T Resolve<T>()
        {
            return Default.Resolve<T>();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public static void Dispose()
        {
            Default.Dispose();
        }

    }
}
