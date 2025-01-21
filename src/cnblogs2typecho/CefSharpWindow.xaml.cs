using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace cnblogs2typecho
{
    /// <summary>
    /// CefSharpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CefSharpWindow : TianXiaTech.BlurWindow
    {
        public Action<string> OnManualClosingHandler { get; set; }

        public string HtmlSource { get; private set; }

        public Action OnLoadedCallback { get; set; }

        public bool CancelExitFlag { get; set; } = true;

        public CefSharpWindow()
        {
            InitializeComponent();
        }

        public void Open(string url)
        {
            this.browser.Address = url;
            this.browser.FrameLoadEnd += Browser_FrameLoadEnd;
        }

        private async void Browser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            var html = await e.Browser.GetSourceAsync();
            HtmlSource = html;
            OnLoadedCallback?.Invoke();
        }

        private void BlurWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnManualClosingHandler?.Invoke(HtmlSource);
            e.Cancel = CancelExitFlag;
            this.Visibility = Visibility.Hidden;
        }
    }
}
