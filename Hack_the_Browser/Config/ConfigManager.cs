using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Hack_the_Browser.MetaDataRepositories;
using log4net;

namespace Hack_the_Browser.Config
{
    /// <summary>
    /// ConfigManager for Image Service
    /// </summary>
    public sealed class ConfigManager : IConfigManager, IAsyncInitialization
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int DefaultConversionBatchSize =5;

        private readonly IImageDataRepository _imageDataRepository;

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public Task Initialization { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="ConfigManager"/> class from being created.
        /// </summary>
        public ConfigManager(IImageDataRepository imageDataRepository)
        {
            _imageDataRepository = imageDataRepository;
            Initialization = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            LoadConfigSettings();
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string CacheLocation { get; set; }

        /// <summary>
        /// Gets or sets the host port number.
        /// </summary>
        /// <value>
        /// The host port number.
        /// </value>
        public int PortNumber { get; set; }

        /// <summary>
        /// Gets or sets the allocated space in gb.
        /// </summary>
        /// <value>
        /// The allocated space in gb.
        /// </value>
        public int AllocatedSpace { get; set; }


        /// <summary>
        /// Gets or sets the low water mark percent.
        /// </summary>
        /// <value>
        /// The low water mark percent.
        /// </value>
        public int LowWaterMark { get; set; }

        /// <summary>
        /// Gets or sets the size of the conversion batch.
        /// </summary>
        /// <value>
        /// The size of the conversion batch.
        /// </value>
        public int ConversionBatchSize { get; set; }

        /// <summary>
        /// Loads the configuration settings.
        /// </summary>
        private void LoadConfigSettings()
        {
            try
            {
                PortNumber = 8095;
                CacheLocation = Directory.GetCurrentDirectory() + "\\CacheLocation";
                AllocatedSpace = 30;
                LowWaterMark = 50;
            }
            catch (Exception ex)
            {
                Log.DebugFormat(ex.Message);
            }
        }
    }
}
