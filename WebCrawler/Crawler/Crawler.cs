using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.LinkExtractor;
using WebCrawler.PageProcessor;
using WebCrawler.PageProvider;
using WebCrawler.Utils;

namespace WebCrawler.Crawler
{
    public class Crawler
    {
        // Input
        private readonly CrawlerOptions options;
        private readonly ProcessStatus status;
        private readonly ILinkExtractor link_extractor;
        private readonly IPageProcessor page_processor;
        // Working variables
        private readonly ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        private readonly ConcurrentBag<string> visited = new ConcurrentBag<string>();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private int execution_count = 0;

        public Crawler(CrawlerOptions options, ProcessStatus status)
        {
            this.options = options;
            this.status = status;

            link_extractor = options.LinkExtractor;
            page_processor = options.PageProcessor;

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

        private IPageProvider GetPageProvider()
        {
            return (options.UseCache
                ? (IPageProvider)new CachedPageProvider(options.Cache, options.UserAgent, options.RequestTimeout)
                : (IPageProvider)new WebPageProvider(options.UserAgent, options.RequestTimeout));
        }

        private Task CreateProcessorTask(int id)
        {
            var progress = status[id];
            var count = 0;
            return Task.Factory.StartNew(() =>
            {
                using (var page_provider = GetPageProvider())
                {
                    while (true)
                    {
                        if (cts.Token.IsCancellationRequested)
                            break;

                        progress.Report("Item " + count);
                        count++;
                        Task.Delay(250).Wait();
                    }
                }
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private Task CreateFinalizerTask()
        {
            return Task.Factory.StartNew(() =>
            {
                status.Report("Starting");
                Task.Delay(5000).Wait();
                status.Report("Done");
                cts.Cancel();
            }, TaskCreationOptions.LongRunning);
        }

        public Task Start()
        {
            // Create processor tasks
            var tasks = Enumerable.Range(0, options.MaxThreadCount)
                                  .Select(CreateProcessorTask)
                                  .ToList();
            // Create finalizer task
            tasks.Add(CreateFinalizerTask());

            return Task.WhenAll(tasks.ToArray());
        }
    }
}
