using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ImageDownloader.Model;
using Panda.WebCrawler;
using Panda.WebCrawler.Extensions;
using Panda.WebCrawler.PageProcessor;

namespace ImageDownloader.Sitemap
{
    public class SitemapPageProcessor : IPageProcessor
    {
        private readonly BlockingCollection<Page> page_queue = new BlockingCollection<Page>();
        private readonly BlockingCollection<string> item_queue = new BlockingCollection<string>();
        private readonly string url;
        private readonly IProgress<string> progress;
        
        public SitemapPageProcessor(string url, IProgress<string> progress)
        {
            this.url = url;
            this.progress = progress;
        }

        public void Process(Page page)
        {
            page_queue.Add(page);
        }

        public void CompleteAdding()
        {
            page_queue.CompleteAdding();
        }

        public Task<SitemapNode> Generate()
        {
            return Task.Factory.StartNew(() =>
            {
                var file_extractor = new AllInternalFilesExtractor(url.GetHost());
                var node = new SitemapNode(url);

                var item_queue_processor_task = Task.Factory.StartNew(() =>
                {
                    foreach (var item in item_queue.GetConsumingEnumerable())
                    {
                        node.Add(item, item);
                        progress.Report(string.Format("{0} items left", item.Count()));
                    }
                });

                var page_queue_processor_tasks = new List<Task>();
                for (var i = 0; i < Settings.MaxSitemapThreadCount; i++)
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        foreach (var page in page_queue.GetConsumingEnumerable())
                        {
                            var files = file_extractor.Get(page);
                            files.Apply(item_queue.Add);
                        }
                    });
                    page_queue_processor_tasks.Add(task);
                }
                Task.WhenAll(page_queue_processor_tasks.ToArray()).Wait();

                item_queue.CompleteAdding();
                item_queue_processor_task.Wait();

                return node;
            });
        }
    }
}
