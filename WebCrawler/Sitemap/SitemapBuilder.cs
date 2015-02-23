using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Panda.Utilities.Extensions;
using WebCrawler.Data;
using WebCrawler.LinkExtractor;
using WebCrawler.Utils;

namespace WebCrawler.Sitemap
{
    public class SitemapBuilder
    {
        private readonly string url;
        private readonly SitemapOptions options;
        private readonly ProcessProgress progress;
        private readonly ILinkExtractor link_extractor;

        public SitemapBuilder(string url, SitemapOptions options, ProcessProgress progress, ILinkExtractor link_extractor)
        {
            this.url = url;
            this.options = options;
            this.progress = progress;
            this.link_extractor = link_extractor;

            EnsureMinThreadCount();
        }

        private void EnsureMinThreadCount()
        {
            int min_worker, min_ioc;
            ThreadPool.GetMinThreads(out min_worker, out min_ioc);
            var thread_count = options.MaxThreadCount + 2;
            if (min_worker < thread_count)
                ThreadPool.SetMinThreads(thread_count, min_ioc);
        }

        private string GetSitemapRoot()
        {
            if (!url.EndsWith("/"))
                return url.Substring(0, url.LastIndexOf('/') + 1);
            return url;
        }

        public Task<SitemapNode> Build(ConcurrentQueue<Page> pages)
        {
            var item_queue = new ConcurrentQueue<string>();
            var page_queue = new BlockingCollection<Page>(pages);
            page_queue.CompleteAdding();

            var page_queue_processor_tasks = new List<Task>();
            for (var i = 0; i < options.MaxThreadCount; i++)
            {
                var task_progress = progress[i];
                var task = Task.Factory.StartNew(() =>
                {
                    foreach (var page in page_queue.GetConsumingEnumerable())
                    {
                        var files = link_extractor.Get(page);
                        files.Apply(item_queue.Enqueue);
                        task_progress.Report(string.Format("Found {0} files in {1}", files.Count, page.Uri));
                        progress.Report(string.Format("Total items found {0}", item_queue.Count));
                    }
                });
                page_queue_processor_tasks.Add(task);
            }

            return Task.WhenAll(page_queue_processor_tasks.ToArray())
                       .ContinueWith(parent =>
                       {
                           var root = GetSitemapRoot();
                           var node = new SitemapNode(root);
                           item_queue.Apply(i => node.Add(i, i));
                           return node;
                       });
        }
    }
}
