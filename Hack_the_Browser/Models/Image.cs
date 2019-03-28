using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Hack_the_Browser.Models
{
    public class Image
    {
        [BsonId]
        public Guid ReferenceId { get; set; }
        public string ExternalId { get; set; }
        public string Extension { get; set; }
        public int ImageFileSize { get; set; }
        public int AnnotationFileSize { get; set; }
        public string ExternalData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int XResolution { get; set; }
        public int YResolution { get; set; }
        public int FrameCount { get; set; }
        public int RotationAngle { get; set; }
        public DateTime DateAccessed { get; set; }
        public int ReferenceVersion { get; set; }
        public string ETag { get; set; }
    }
}
