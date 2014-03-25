using Caliburn.Micro;
using ImageDownloader.Contents.Host.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Model
{
    [Export(typeof(IJobFactory))]
    public class JobFactory : IJobFactory
    {
        public IHost Create()
        {
            var host = IoC.Get<IHost>();
            host.Model = new JobModel { Website = @"file:///C:/Private/GitHub/ImageDownloader/TestSite/index.html" };
            return host;
        }
    }
}
