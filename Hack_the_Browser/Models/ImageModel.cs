using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using log4net;

namespace Hack_the_Browser.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageModel
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>
        /// The external identifier.
        /// </value>
        [Required]
        public string ExternalId { get; set; }
        /// <summary>
        /// Gets or sets the external data.
        /// </summary>
        /// <value>
        /// The external data.
        /// </value>
        public string ExternalData { get; set; }
        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        public string Extension { get; set; }
        /// <summary>
        /// Gets or sets the rotation angle.
        /// </summary>
        /// <value>
        /// The rotation angle.
        /// </value>
        public int RotationAngle { get; set; }
        /// <summary>
        /// Gets or sets the image file model.
        /// </summary>
        /// <value>
        /// The image file model.
        /// </value>
        public HttpFileModel ImageFileModel { get; set; }
        /// <summary>
        /// Gets or sets the annotation file model.
        /// </summary>
        /// <value>
        /// The annotation file model.
        /// </value>
        public HttpFileModel AnnotationFileModel { get; set; }

        /// <summary>
        /// Gets the size of the image file.
        /// </summary>
        /// <value>
        /// The size of the image file.
        /// </value>
        public int ImageFileSize => ImageFileModel?.Buffer.Length ?? 0;

        /// <summary>
        /// Gets the size of the annotation file.
        /// </summary>
        /// <value>
        /// The size of the annotation file.
        /// </value>
        public int AnnotationFileSize => AnnotationFileModel?.Buffer.Length ?? 0;

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            try
            {
                return
                    $"ExternalId={ExternalId}, Extension={Extension}, RotationAngle={RotationAngle},ExternalData={ExternalData} \n, ImageSize={ImageFileSize},AnnotationFileSzie={AnnotationFileSize.ToString(CultureInfo.InvariantCulture)}";
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            return "";
        }
    }
}
