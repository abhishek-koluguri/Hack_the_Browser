using System;
using System.Threading.Tasks;
using Hack_the_Browser.Models;

namespace Hack_the_Browser.MetaDataRepositories
{
    /// <summary>
    /// ImageDataRepository provides functions to interact with database.
    /// </summary>
    public interface IImageDataRepository
    {
        Task<Guid> GetReferenceIdAsync(string imageModelExternalId);
        Task<Image> GetImageAsync(Guid existingImageReferenceId, bool b);
        Task SaveImageAsync(Image image);
    }
}
