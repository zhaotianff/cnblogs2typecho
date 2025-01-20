using cnblogs2typecho.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace cnblogs2typecho.Browser
{
    public class CefManager
    {
        private static volatile CefManager instance;
        private static object obj = new object();

        private static AutoResetEvent chromeAutoResetEvent = new AutoResetEvent(false);
        private CefSharpWindow cefSharpWindow = new CefSharpWindow();

        public static CefManager Instance
        {
            get
            {
                if(instance == null)
                {
                    lock(obj)
                    {
                        if (instance == null)
                            instance = new CefManager();
                    }
                }

                return instance;
            }
        }

        public CefManager()
        {
            cefSharpWindow.Show();
            cefSharpWindow.Visibility = Visibility.Hidden;
        }

        public void SetManualClosingHanlder(Action<string> action)
        {
            cefSharpWindow.OnManualClosingHandler = action;
        }

        public void Show()
        {
            cefSharpWindow.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            cefSharpWindow.Visibility = Visibility.Hidden;
        }

        public void Open(string url)
        {
            cefSharpWindow.Open(url);
        }

        public void Close()
        {
            cefSharpWindow.Close();
        }

        public async Task<string> OpenWithWait(string url, int timeout = 10000)
        {
            var source = "";
            this.cefSharpWindow.OnLoadedCallback = ChromiumWebBrowserLoadEnd;
            Open(url);
            
            try
            {
                await Task.Run(() => {
                    while (true)
                    {
                        var waitResult = chromeAutoResetEvent.WaitOne(timeout);

                        if (waitResult == true)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                source = cefSharpWindow.HtmlSource;
                            });
                        }

                        break;
                    }
                });

                return source;
            }
            catch
            {
                return source;
            }
        }

        private void ChromiumWebBrowserLoadEnd()
        {
            chromeAutoResetEvent.Set();
        }

    }
}
