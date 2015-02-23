using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Panda.Utilities.Extensions;
using WebCrawler.LinkExtractor;
using WebCrawler.PageProcessor;
using WebCrawler.PageProvider;
using WebCrawler.Utils;

namespace WebCrawler.Crawler
{
    public class Crawler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        // Input
        private readonly CrawlerOptions options;
        private readonly ProcessStatus status;
        private readonly ILinkExtractor link_extractor;
        private readonly IPageProcessor page_processor;
        private readonly IPageProviderFactory page_provider_factory;
        // Working variables
        private readonly ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        private readonly ConcurrentBag<string> visited = new ConcurrentBag<string>();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private int execution_count;

        public Crawler(CrawlerOptions options, ProcessStatus status)
        {
            this.options = options;
            this.status = status;

            link_extractor = options.LinkExtractor;
            page_processor = options.PageProcessor;
            page_provider_factory = options.PageProviderFactory;

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

        private Task CreateProcessorTask(int id)
        {
            var progress = status[id];
            return Task.Factory.StartNew(() =>
            {
                log.Debug("Starting consumer " + id);

                using (var page_provider = page_provider_factory.Create())
                {
                    while (true)
                    {
                        if (cts.Token.IsCancellationRequested)
                            break;

                        if (!queue.Any())
                        {
                            Thread.Sleep(options.ThreadDelay);
                            continue;
                        }

                        Interlocked.Increment(ref execution_count);

                        string url;
                        if (queue.TryDequeue(out url) && !visited.Contains(url))
                        {
                            visited.Add(url);
                            var page = page_provider.Get(url);

                            page_processor.Process(page);

                            link_extractor.Get(page)
                                          .Except(queue)
                                          .Except(visited)
                                          .Apply(queue.Enqueue);

                            progress.Report(string.Format("Processed {0} in {1} ms", url, page.DownloadTime));
                            log.Trace("{0} processed {1} in {2} ms [queue {3}, visited {4}]", id, url, page.DownloadTime, queue.Count, visited.Count);
                        }

                        Interlocked.Decrement(ref execution_count);
                    }

                    progress.Report("Status: " + page_provider.Status());
                    log.Debug("Stopping consumer {0} [{1}]", id, page_provider.Status());
                }
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private Task CreateFinalizerTask()
        {
            return Task.Factory.StartNew(() =>
            {
                status.Report("Starting crawler");
                log.Debug("Starting finalizer");

                while (true)
                {
                    if (!queue.Any() && execution_count == 0)
                        cts.Cancel();
                    else
                        Thread.Sleep(options.ThreadDelay);

                    if (cts.Token.IsCancellationRequested)
                        break;

                    status.Report(string.Format("Queued {0}, Visited {1}", queue.Count, visited.Count));
                }

                status.Report("Status: Done");
                log.Debug("Stopping finalizer");
            }, TaskCreationOptions.LongRunning);
        }

        public Task Start()
        {
            queue.Enqueue(options.Url);
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
