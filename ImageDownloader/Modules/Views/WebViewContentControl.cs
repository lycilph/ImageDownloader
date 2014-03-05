using CefSharp;
using CefSharp.Wpf;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ImageDownloader.Modules.Views
{
    public class WebViewContentControl : ContentControl
    {
        private WebView internal_web_view;
        private bool internal_update = false;

        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }
        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(WebViewContentControl), new UIPropertyMetadata(string.Empty, OnAddressChanged));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(WebViewContentControl), new PropertyMetadata(true));

        public WebViewContentControl()
        {
            string data;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ImageDownloader.Modules.Resources.theme.css"))
            using (var reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }
            data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));

            var bs = new BrowserSettings
            {
                UserStyleSheetEnabled = true,
                UserStyleSheetLocation = @"data:text/css;charset=utf-8;base64," + data
            };
            internal_web_view = new WebView("http://www.google.com", bs);
            internal_web_view.PropertyChanged += WebViewPropertyChanged;
            Content = internal_web_view;
        }

        private void WebViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsLoading":
                    Dispatcher.Invoke(() => IsLoading = internal_web_view.IsLoading);
                    break;
                case "Address":
                    Dispatcher.Invoke(() => 
                        {
                            internal_update = true;
                            Address = internal_web_view.Address;
                            internal_update = false;
                        });
                    break;
            }
        }

        private static void OnAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wvcc = d as WebViewContentControl;
            if (wvcc == null) return;

            if (wvcc.internal_web_view.IsBrowserInitialized && !wvcc.internal_update)
                wvcc.internal_web_view.Load(wvcc.Address);
        }
    }
}
