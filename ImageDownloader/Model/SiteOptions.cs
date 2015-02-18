using System.Collections.Generic;

namespace ImageDownloader.Model
{
    public class SiteOptions
    {
        public bool UseCache { get; set; }
        public int CacheLifetime { get; set; }
        public string Folder { get; set; }
        public bool FlattenFilename { get; set; }
        public bool OnlySubpages { get; set; }
        public List<string> ExcludedExtensions { get; set; }
        public List<string> ExcludedStrings { get; set; }
    }
}
