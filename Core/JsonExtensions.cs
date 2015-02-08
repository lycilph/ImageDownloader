using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using NLog;

namespace Core
{
    public static class JsonExtensions
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void WriteToFile<T>(string filename, T obj)
        {
            log.Info("Serializing object [{0}]", obj.GetType());
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            log.Info("Writing to file {0}", filename);
            File.WriteAllText(filename, json);
        }

        public static T ReadFromFile<T>(string filename)
        {
            log.Info("Reading from file {0}", filename);
            var json = File.ReadAllText(filename);

            log.Info("Deserializing object [{0}]", typeof(T));
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void ZipAndWriteToFile<T>(string filename, T obj)
        {
            log.Info("Serializing object [{0}]", obj.GetType());
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            log.Info("Zipping json");
            var bytes = Zip(json);

            log.Info("Writing to file {0}", filename);
            File.WriteAllBytes(filename, bytes);
        }

        public static T ReadFromFileAndUnzip<T>(string filename)
        {
            log.Info("Reading from file {0}", filename);
            var bytes = File.ReadAllBytes(filename);

            log.Info("Unzipping json");
            var json = Unzip(bytes);

            log.Info("Deserializing object [{0}]", typeof(T));
            return JsonConvert.DeserializeObject<T>(json);
        }

        private static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                using (var gs = new GZipStream(output, CompressionMode.Compress))
                {
                    input.CopyTo(gs);
                }

                return output.ToArray();
            }
        }

        private static string Unzip(byte[] bytes)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                using (var gs = new GZipStream(input, CompressionMode.Decompress))
                {
                    gs.CopyTo(output);
                }

                return Encoding.UTF8.GetString(output.ToArray());
            }
        }
    }
}
