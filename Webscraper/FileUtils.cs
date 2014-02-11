using System.IO;
using System.Reflection;

namespace Webscraper
{
    public static class FileUtils
    {
        public static string GetExeDirectory()
        {
            string exe_path = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(exe_path);
        }
    }
}
