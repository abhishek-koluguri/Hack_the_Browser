using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Hack_the_Browser.Devices;
using Hack_the_Browser.MetaDataRepositories;
using Hack_the_Browser.Models;
using log4net;

namespace Hack_the_Browser.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents ImagesController
    /// </summary>
    public class ImagesController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IImageDataRepository _imageDataRepository;
        private readonly IDevice _device;
        private const string MediaType = "image/tif";

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImagesController" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="device">The device.</param>
        /// <param name="imageInfoProvider">The image information provider.</param>
        /// <param name="imageConversionProcessor">The image conversion processor.</param>
        public ImagesController(IImageDataRepository repository, IDevice device)
        {
            _imageDataRepository = repository;
            _device = device;
        }


        /// <summary>
        ///     Returns image for  ReferenceId and Page Number
        ///     URL format: http://localhost/api/images/referenceId
        /// </summary>
        /// <param name="referenceId">The reference identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetImage(string referenceId)
        {
            try
            {

                var refId = Guid.Parse(referenceId);
                Log.Info($"Received request for referenceid   {refId}");
                if (refId.Equals(Guid.Empty))
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "ExternalId can not be empty");


                var image = await _imageDataRepository.GetImageAsync(Guid.Parse(referenceId), false);

                if (image == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        $"does not exist for image {referenceId}.");

                var eTag = GetETag(Request).Trim('"');

                if (!string.IsNullOrEmpty(eTag) && eTag == image.ETag)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotModified);
                }

                Stream imageStream = await GetImageStream(image.ReferenceId, image.Extension);

                if (imageStream == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        $"Could not get image {referenceId}.");

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(imageStream)
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
                response.Headers.ETag = new EntityTagHeaderValue("\"" + image.ETag + "\"");
                response.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    MustRevalidate = true,
                    MaxAge = TimeSpan.FromSeconds(100000),
                    Private = true,
                    NoCache = false
                };

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        ///     Post the image to this service
        /// </summary>
        /// <param name="imageModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Upload(ImageModel imageModel)
        {
            Log.DebugFormat("Entered upload process for image with external ID {0}", imageModel.ExternalId);

            try
            {
                if (string.IsNullOrEmpty(imageModel.ExternalId))
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid external id.");

                Log.DebugFormat("Retrieving image from repository for image with external ID {0}",
                    imageModel.ExternalId);
                var referenceId = Guid.NewGuid();
                var image = new Image {ReferenceId = referenceId, ExternalId = imageModel.ExternalId};

                try
                {
                    var existingImageReferenceId =
                        await _imageDataRepository.GetReferenceIdAsync(imageModel.ExternalId);
                    var existingImage = await _imageDataRepository.GetImageAsync(existingImageReferenceId, false);

                    image.ReferenceVersion = existingImage.ReferenceVersion + 1;
                }
                catch (Exception)
                {
                    image.ReferenceVersion = 0;
                }

                Log.DebugFormat("Saving image with external ID {0} to device", imageModel.ExternalId);
                await _device.SaveImage(referenceId, imageModel);

                ImageMapping(imageModel, image);

                Log.DebugFormat("Saving image to database with external ID {0}", imageModel.ExternalId);
                image.ETag = Guid.NewGuid().ToString();
                await _imageDataRepository.SaveImageAsync(image);

                Log.DebugFormat("Completed upload process for image with external ID {0}", imageModel.ExternalId);
                return Request.CreateResponse(HttpStatusCode.OK, image);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        private Image ImageMapping(ImageModel fromImageModel, Image toImage)
        {
            if (toImage == null) toImage = new Image();

            toImage.Extension = fromImageModel.Extension;
            toImage.ExternalData = fromImageModel.ExternalData;
            toImage.RotationAngle = fromImageModel.RotationAngle;
            toImage.ImageFileSize = fromImageModel.ImageFileSize;
            toImage.AnnotationFileSize = fromImageModel.AnnotationFileSize;
            toImage.DateAccessed = DateTime.Now;

            return toImage;
        }

        private async Task<Stream> GetImageStream(Guid referenceId, string imageExtension)
        {
            return await _device.GetImage(referenceId, imageExtension);
        }

        private static string GetETag(HttpRequestMessage request)
        {
            return request.Headers.TryGetValues("ETag", out var values) ? values.FirstOrDefault() : "";
        }
    }
}