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
    /// ProgressWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressWindow : TianXiaTech.BlurWindow
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        public void UpdateProgress(double value,int page)
        {
            this.progress.Value += value;
            this.lbl_LoadingText.Content = $"正在读取第 {page} 页博客,请稍等...";

            if (progress.Value >= 100)
                this.Close();
        }
    }
}
