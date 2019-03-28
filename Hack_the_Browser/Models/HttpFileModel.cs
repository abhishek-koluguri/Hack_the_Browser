namespace Hack_the_Browser.Models
{
    public class HttpFileModel
    {
        public string FileName { get; set; }
        public string MediaType { get; set; }
        public byte[] Buffer { get; set; }
        

        public HttpFileModel() { }

        public HttpFileModel(string fileName, string mediaType, byte[] buffer)
        {
            FileName = fileName;
            MediaType = mediaType;
            Buffer = buffer;
            
        }
    }
}
