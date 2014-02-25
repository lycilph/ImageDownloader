using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Image_test
{
    class Program
    {
        static void Main(string[] args)
        {
            var address = @"http://placehold.it/350x150.jpg";

            using (var client = new WebClient())
            {
                var data = client.DownloadData(address);
                Console.WriteLine("Data size: " + data.Length);

                using (var image = Image.FromStream(new MemoryStream(data)))
                {
                    Console.WriteLine("Image size: ({0}, {1})", image.Width, image.Height);
                }
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
