using System;
using System.Windows.Threading;
using Panda.Utilities;

namespace ImageDownloader.Utilities
{
    public class Timer : DisposableObject
    {
        private readonly IProgress<string> progress;
        private readonly DateTime start_time;
        private DispatcherTimer timer;
        private bool disposed;

        public int Elapsed { get { return (int)(DateTime.Now - start_time).TotalMilliseconds; } }

        public Timer(Dispatcher dispatcher, IProgress<string> progress)
        {
            this.progress = progress;
            start_time = DateTime.Now;
            timer = new DispatcherTimer(DispatcherPriority.Background, dispatcher);
            timer.Tick += OnTick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
        }

        private void OnTick(object sender, EventArgs args)
        {
            progress.Report(Math.Round((DateTime.Now - start_time).TotalSeconds, 1).ToString("N1") + " sec(s)");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            try
            {
                if (disposing)
                {
                    // Free any other managed objects here.
                    timer.Stop();
                    timer.Tick -= OnTick;
                    timer = null;
                }
                // Free any unmanaged objects here. 
            }
            finally
            {
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
