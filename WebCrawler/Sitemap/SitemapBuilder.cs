using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Panda.Utilities.Extensions;
using WebCrawler.Data;
using WebCrawler.LinkExtractor;
using WebCrawler.Utils;

namespace WebCrawler.Sitemap
{
    public class SitemapBuilder
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private const int ThreadCountOverhead = 2;

        // Input
        private readonly string url;
        private readonly SitemapOptions options;
        private readonly ProcessStatus status;
        private readonly ILinkExtractor link_extractor;

        public SitemapBuilder(string url, SitemapOptions options, ProcessStatus status)
        {
            this.url = url;
            this.options = options;
            this.status = status;

            link_extractor = options.LinkExtractor;

            EnsureMinThreadCount();
        }

        private void EnsureMinThreadCount()
        {
            int min_worker, min_ioc;
            ThreadPool.GetMinThreads(out min_worker, out min_ioc);
            var thread_count = options.MaxThreadCount + ThreadCountOverhead;
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
                var task_progress = status[i];
                var task = Task.Factory.StartNew(() =>
                {
                    foreach (var page in page_queue.GetConsumingEnumerable())
                    {
                        var files = link_extractor.Get(page);
                        files.Apply(item_queue.Enqueue);
                        task_progress.Report(string.Format("Found {0} files in {1}", files.Count, page.Uri));
                    }
                }, TaskCreationOptions.LongRunning);
                page_queue_processor_tasks.Add(task);
            }

            var cts = new CancellationTokenSource();
            var status_task = Task.Factory.StartNew(() =>
            {
                status.Report("Starting builder");
                log.Trace("Starting builder");

                while (true)
                {
                    var distinct_count = item_queue.Distinct().Count();
                    status.Report(string.Format("Total items found {0} [{1} unique]", item_queue.Count, distinct_count));

                    if (cts.Token.IsCancellationRequested)
                        break;
                        
                    // ReSharper disable MethodSupportsCancellation
                    Task.Delay(options.ThreadDelay).Wait();
                    // ReSharper restore MethodSupportsCancellation
                }

                status.Report("Status: Done");
                log.Trace("Stopping builder");
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            // ReSharper disable MethodSupportsCancellation
            Task.WhenAll(page_queue_processor_tasks.ToArray())
                .ContinueWith(parent => cts.Cancel());

            return status_task.ContinueWith(parent =>
            {
                var root = GetSitemapRoot();
                var node = new SitemapNode(root);
                item_queue.Apply(i => node.Add(i, i));
                return node;
            });
            // ReSharper restore MethodSupportsCancellation
        }
    }
}
