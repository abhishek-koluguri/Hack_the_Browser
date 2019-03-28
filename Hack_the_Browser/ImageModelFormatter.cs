using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Hack_the_Browser.Models;
using Newtonsoft.Json;

namespace Hack_the_Browser
{
    /// <summary>
    /// Formatter responsible for reading multipart/form-data into an ImageModel.
    /// </summary>
    public class ImageModelFormatter : BufferedMediaTypeFormatter
    {
        private const string SupportedMediaType = "multipart/form-data";

        /// <summary>
        /// This formatter can take multipart/form-data and read it into an ImageModel.
        /// </summary>
        public ImageModelFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(SupportedMediaType));
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(ImageModel);
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
                
            if (!content.IsMimeMultipartContent())
            {
                throw new Exception("Unsupported Media Type");
            }

            var provider = Task.Run(() => content.ReadAsMultipartAsync()).GetAwaiter().GetResult();

            var jsonContent = provider.Contents.FirstOrDefault(c => c.Headers.ContentDisposition.Name.Replace("\"", string.Empty)
                .Equals("Data", StringComparison.OrdinalIgnoreCase));
            var imageModel = jsonContent != null ? JsonConvert.DeserializeObject<ImageModel>(jsonContent.ReadAsStringAsync().Result) : new ImageModel();

            foreach (var dataSection in provider.Contents.Where(c => !string.IsNullOrEmpty(c.Headers.ContentDisposition.FileName)))
            {
                var sectionName = dataSection.Headers.ContentDisposition.Name.Replace("\"", string.Empty);
                var fileName = dataSection.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                if (fileName.Contains("\\"))
                {
                    fileName = Path.GetFileName(fileName);
                }

                var mediaType = dataSection.Headers.ContentType.MediaType;

                var data = dataSection.ReadAsByteArrayAsync().GetAwaiter().GetResult();

                switch (sectionName.ToUpperInvariant())
                {
                    case "ANNOTATION":
                        imageModel.AnnotationFileModel = new HttpFileModel(fileName, mediaType, data);
                        break;
                    case "IMAGE":
                        imageModel.ImageFileModel = new HttpFileModel(fileName, mediaType, data);
                        break;
                }
            }

            return imageModel;
        }
    }
}
