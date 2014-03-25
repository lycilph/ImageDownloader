using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Core;
using ImageDownloader.Framework.MainMenu.ViewModels;
using ReactiveUI;
using System;

namespace ImageDownloader.Framework.Shell.Utils
{
    public class ContentWrapper : IDisposable
    {
        private bool disposed = false;
        private IDisposable subscription;

        public MenuItem MenuItem { get; set; }
        public IContent Content { get; set; }

        public ContentWrapper(MenuItem menu_item, IContent content)
        {
            MenuItem = menu_item;
            Content = content;

            subscription = Content.WhenAnyValue(x => x.DisplayName)
                                  .Subscribe(x => MenuItem.Text = x);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (subscription != null)
                    {
                        subscription.Dispose();
                        subscription = null;
                    }
                }

                disposed = true;
            }
        }
    }
}
