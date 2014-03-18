using CefSharp;
using CefSharp.Wpf;
using ImageDownLoader.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ImageDownloader.Core.View
{
    public class WebViewContentControl : ContentControl, ILifeSpanHandler
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

        public string UserStyleSheet
        {
            get { return (string)GetValue(UserStyleSheetProperty); }
            set { SetValue(UserStyleSheetProperty, value); }
        }
        public static readonly DependencyProperty UserStyleSheetProperty =
            DependencyProperty.Register("UserStyleSheet", typeof(string), typeof(WebViewContentControl), new PropertyMetadata(string.Empty, OnUserStyleSheetChanged));

        public ICommand BackCommand
        {
            get { return (ICommand)GetValue(BackCommandProperty); }
            set { SetValue(BackCommandProperty, value); }
        }
        public static readonly DependencyProperty BackCommandProperty =
            DependencyProperty.Register("BackCommand", typeof(ICommand), typeof(WebViewContentControl), new PropertyMetadata(null));

        public ICommand ForwardCommand
        {
            get { return (ICommand)GetValue(ForwardCommandProperty); }
            set { SetValue(ForwardCommandProperty, value); }
        }
        public static readonly DependencyProperty ForwardCommandProperty =
            DependencyProperty.Register("ForwardCommand", typeof(ICommand), typeof(WebViewContentControl), new PropertyMetadata(null));

        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }
        public static readonly DependencyProperty RefreshCommandProperty =
            DependencyProperty.Register("RefreshCommand", typeof(ICommand), typeof(WebViewContentControl), new PropertyMetadata(null));
        
        public WebViewContentControl()
        {
            internal_web_view = new WebView();
            Content = internal_web_view;

            BackCommand = new RelayCommand(o => internal_web_view.Back(), o => internal_web_view.CanGoBack);
            ForwardCommand = new RelayCommand(o => internal_web_view.Forward(), o => internal_web_view.CanGoForward);
            RefreshCommand = new RelayCommand(o => internal_web_view.Reload(), o => !internal_web_view.IsLoading);
        }

        private void SetUserStyleSheet()
        {
            if (string.IsNullOrWhiteSpace(UserStyleSheet)) return;

            var name = Assembly.GetExecutingAssembly().GetManifestResourceNames().FirstOrDefault(n => n.ToLower().Contains(UserStyleSheet));
            if (name == null)
                throw new InvalidOperationException();

            string data;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
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

            internal_web_view.PropertyChanged -= WebViewPropertyChanged;
            internal_web_view.LifeSpanHandler = null;

            internal_web_view = new WebView(Address, bs);
            internal_web_view.PropertyChanged += WebViewPropertyChanged;
            internal_web_view.LifeSpanHandler = this;

            Content = internal_web_view;
        }

        private void WebViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsLoading":
                    Dispatcher.Invoke(() =>
                    { 
                        IsLoading = internal_web_view.IsLoading;
                        CommandManager.InvalidateRequerySuggested();
                    });
                    break;
                case "CanGoBack":
                case "CanGoForward":
                    Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
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

        private static void OnUserStyleSheetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wvcc = d as WebViewContentControl;
            if (wvcc == null) return;

            wvcc.SetUserStyleSheet();
        }

        public void OnBeforeClose(IWebBrowser browser)
        {
        }

        public bool OnBeforePopup(IWebBrowser browser, string url, ref int x, ref int y, ref int width, ref int height)
        {
            Dispatcher.Invoke(() => Address = url);
            return true;
        }
    }
}
