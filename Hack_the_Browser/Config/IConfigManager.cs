namespace Hack_the_Browser.Config
{
    public interface IConfigManager
    {
        /// <summary>
        /// Gets or sets the cache location.
        /// </summary>
        /// <value>
        /// The cache location.
        /// </value>
        string CacheLocation { get; set; }
        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        /// <value>
        /// The port number.
        /// </value>
        int PortNumber { get; set; }
        /// <summary>
        /// Gets or sets the allocated space.
        /// </summary>
        /// <value>
        /// The allocated space.
        /// </value>
        int AllocatedSpace { get; set; }
        /// <summary>
        /// Gets or sets the low water mark.
        /// </summary>
        /// <value>
        /// The low water mark.
        /// </value>
        int LowWaterMark { get; set; }
        /// <summary>
        /// Gets or sets the size of the conversion batch.
        /// </summary>
        /// <value>
        /// The size of the conversion batch.
        /// </value>
        int ConversionBatchSize { get; set; }
    }
}
