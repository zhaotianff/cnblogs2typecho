using cnblogs2typecho.Web;
using LungWorkStation.DAL.DbHelper;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cnblogs2typecho
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : TianXiaTech.BlurWindow
    {
        private CefSharpWindow cefSharpWindow = new CefSharpWindow();
        private bool isCnblogsLoginSuccess = false;


        public MainWindow()
        {
            InitializeComponent();

            cefSharpWindow.Show();
            cefSharpWindow.Visibility = Visibility.Hidden;
        }

        private void btn_LoginCnblogs_Click(object sender, RoutedEventArgs e)
        {
            if (isCnblogsLoginSuccess == true)
                return;

            cefSharpWindow.Owner = this;
            cefSharpWindow.Visibility = Visibility.Visible;
            cefSharpWindow.OnLoginSuccessAction = OnCnblogsLoginSuccess;
            cefSharpWindow.Open(Urls.CnBlogsLoginUrl);
        }

        private void OnCnblogsLoginSuccess(string html)
        {
            HtmlParser htmlParser = new HtmlParser(html);

            if(htmlParser.Title.Contains("开发者的网上家园"))
            {
                this.lbl_CnblogsStatus.Content = "登录成功";
                this.lbl_CnblogsStatus.Foreground = Brushes.Green;
                isCnblogsLoginSuccess = true;
            }
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            if(OpenTypechoDatabase() ==true)
            {
                MessageBox.Show("登录");
            }
        }

        private bool OpenTypechoDatabase()
        {
            try
            {
                var connectString = $"Server={this.tbox_Server.Text}; Port={this.tbox_Port.Text}; Database={this.tbox_Database.Text}; Uid={this.tbox_User.Text}; Pwd={this.pbx_Password.Password}";

                using(MariaDbHelper mariaDbHelper = new MariaDbHelper(connectString))
                {
                    mariaDbHelper.Open();

                    if (mariaDbHelper.ConnectionState == System.Data.ConnectionState.Open)
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
