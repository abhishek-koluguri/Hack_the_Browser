using System;
using System.IO;
using System.Threading.Tasks;
using Hack_the_Browser.Models;

namespace Hack_the_Browser.Devices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDevice
    {
        Task<string> SaveImage(Guid referenceId, ImageModel imageModel);
        Task RemoveImages(Guid referenceId);
        Task<Stream> GetImage(Guid referenceId, string imageExtension);
    }
}
