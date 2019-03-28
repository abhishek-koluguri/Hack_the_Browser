using System;

namespace Hack_the_Browser.Models
{
    public class FileHandleModel
    {
        public Guid ReferenceId { get; set; }
        public string FilePath { get; set; }
        public int PageCount { get; set; }
    }
}
