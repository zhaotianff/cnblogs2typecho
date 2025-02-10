using cnblogs2typecho.Browser;
using cnblogs2typecho.Crawler;
using cnblogs2typecho.DAL;
using cnblogs2typecho.Model;
using cnblogs2typecho.Properties;
using cnblogs2typecho.Web;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace cnblogs2typecho
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : TianXiaTech.BlurWindow
    {     
        private bool isCnblogsLoginSuccess = false;
        private CnBlogCrawler blogCrawler = new CnBlogCrawler();
        private TypechoDal typechoDal = new TypechoDal();

        public MainWindow()
        {
            InitializeComponent();
            LoadUserSettings();
        }

        private void btn_LoginCnblogs_Click(object sender, RoutedEventArgs e)
        {
            if (isCnblogsLoginSuccess == true)
                return;

            CefManager.Instance.Show();
            CefManager.Instance.SetManualClosingHanlder(OnCnblogsLoginSuccess);
            CefManager.Instance.Open(Urls.CnBlogsLoginUrl);
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

        private async void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            if(isCnblogsLoginSuccess == false)
            {
                MessageBox.Show("请先登录博客园");
                return;
            }

            if (typechoDal.OpenTypechoDatabase(this.tbox_Server.Text, this.tbox_Port.Text,
                this.tbox_Database.Text, this.tbox_User.Text,
                this.pbx_Password.Password) == false)
            {
                MessageBox.Show("登录typecho数据库失败");
                return;
            }

            WriteUserSettings();

            ProgressWindow progressWindow = new ProgressWindow();
            progressWindow.Show();

            List<BlogPage> blogPages = await blogCrawler.EnumAllBlogPage(progressWindow.UpdateProgress);

            MigrationWindow migrationWindow = new MigrationWindow(blogPages,this.typechoDal);
            migrationWindow.Show();
            Application.Current.MainWindow = migrationWindow;
            this.Close();
        }

        private void BlurWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CefManager.Instance.Close();
        }


        private void LoadUserSettings()
        {
            this.tbox_Server.Text = Settings.Default["Server"].ToString();
            this.tbox_User.Text = Settings.Default["User"].ToString();
            this.pbx_Password.Password = Settings.Default["Password"].ToString();
            this.tbox_Port.Text = Settings.Default["Port"].ToString();
            this.tbox_Database.Text = Settings.Default["Database"].ToString();
        }

        private void WriteUserSettings()
        {
            Settings.Default["Server"] = this.tbox_Server.Text;
            Settings.Default["User"] = this.tbox_User.Text;
            Settings.Default["Password"] = this.pbx_Password.Password;
            Settings.Default["Port"] = this.tbox_Port.Text;
            Settings.Default["Database"] = this.tbox_Database.Text;
            Settings.Default.Save();
        }
    }
}
