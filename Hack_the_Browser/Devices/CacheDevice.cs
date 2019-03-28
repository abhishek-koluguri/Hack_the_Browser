using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Hack_the_Browser.Config;
using Hack_the_Browser.MetaDataRepositories;
using Hack_the_Browser.Models;
using log4net;
using Vertafore.Cryptography;

namespace Hack_the_Browser.Devices
{
    /// <summary>
    /// Cache Device provides functions to create, delete and get images
    /// </summary>
    public class CacheDevice : IDevice
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string ImagesFolder = "Images";
        private readonly IConfigManager _configManager;
        private readonly IImageDataRepository _imageDataRepository;


        /// <summary>
        /// 
        /// </summary>
        public event EventHandler ImageReceived;


        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDevice" /> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="imageDataRepository">The image data repository.</param>
        public CacheDevice(IConfigManager configManager, IImageDataRepository imageDataRepository)
        {
            _configManager = configManager;
            _imageDataRepository = imageDataRepository;
        }


        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="referenceId">The reference identifier.</param>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public async Task<string> GetFilePath(Guid referenceId, string extension)
        {
            await ((IAsyncInitialization)_configManager).Initialization;
            var filePath = Path.Combine(_configManager.CacheLocation, referenceId.ToString(), $"source{extension}");
            return filePath;
        }

        /// <summary>
        /// Gets the source file path.
        /// </summary>
        /// <param name="referenceId">The reference identifier.</param>
        /// <returns></returns>
        public async Task<string> GetSourceFilePath(Guid referenceId)
        {
            return await GetSourceFilePath(referenceId.ToString());
        }

        /// <summary>
        /// Gets the source file path.
        /// </summary>
        /// <param name="referenceId">The reference identifier.</param>
        /// <returns></returns>
        public async Task<string> GetSourceFilePath(string referenceId)
        {
            await ((IAsyncInitialization)_configManager).Initialization;
            var directory = new DirectoryInfo(Path.Combine(_configManager.CacheLocation, referenceId));
            var sourceFile = directory.GetFiles("source.*").FirstOrDefault();
            return sourceFile?.FullName;
        }

        /// <summary>
        /// Gets the Decrypted Memory Stream of the given file path.
        /// </summary>
        /// <param name="filePath">File path for the encrypted source file.</param>
        /// <returns></returns>
        public static async Task<Stream> GetDecryptedSourceStreamAsync(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                var result = new MemoryStream();
                stream.CopyTo(result);

                return result;
            }
        }

        /// <summary>
        /// Saves the specified reference identifier.
        /// </summary>
        /// <param name="referenceId">The reference identifier.</param>
        /// <param name="imageModel">The image model.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">imageModel</exception>
        public async Task<string> SaveImage(Guid referenceId, ImageModel imageModel)
        {
            // check write permissions
            if (imageModel == null) throw new ArgumentNullException("imageModel");

            await ((IAsyncInitialization)_configManager).Initialization;

            var directoryInfo =
                Directory.CreateDirectory(Path.Combine(_configManager.CacheLocation, referenceId.ToString()));
            // this is for Images
            directoryInfo.CreateSubdirectory(ImagesFolder);

            var imageFilePath = Path.Combine(directoryInfo.FullName,
                $"Source{imageModel.Extension}");

            using (var file = File.OpenWrite(imageFilePath))
            {
                var encryptedData = EncryptionLibrary.EncryptData(imageModel.ImageFileModel.Buffer, EncryptionConfig.AesKey, EncryptionConfig.HmacKey);
                await file.WriteAsync(encryptedData, 0, encryptedData.Length);
            }

            return imageFilePath;
        }

        public Task RemoveImages(Guid referenceId)
        {
            throw new NotImplementedException();
        }

        public async Task<Stream> GetImage(Guid referenceId, string imageExtension)
        {
            await ((IAsyncInitialization)_configManager).Initialization;
            var folderPath = Path.Combine(_configManager.CacheLocation, referenceId.ToString());
            if (Directory.Exists(folderPath))
            {
                var imagePath = folderPath + $@"\Source" + imageExtension;
                if (!File.Exists(imagePath)) return null;
                using (var file = File.OpenRead(imagePath))
                {
                    var encryptedData = new byte[file.Length];
                    await file.ReadAsync(encryptedData, 0, (int)file.Length);
                    return new MemoryStream(EncryptionLibrary.DecryptData(encryptedData, EncryptionConfig.AesKey, EncryptionConfig.HmacKey));
                }
            }

            throw new Exception("Image not found");
        }
    }
}
