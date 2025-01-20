using cnblogs2typecho.Browser;
using cnblogs2typecho.Model;
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
        private bool isCnblogsLoginSuccess = false;

        public MainWindow()
        {
            InitializeComponent();
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

            //if(OpenTypechoDatabase() == false)
            //{
            //    MessageBox.Show("登录typecho数据库失败");
            //    return;
            //}

            List<BlogPage> blogPages = await EnumAllBlogPage();

            MigrationWindow migrationWindow = new MigrationWindow(blogPages);
            migrationWindow.Show();

            this.Close();
        }

        private async Task<List<BlogPage>> EnumAllBlogPage()
        {
            CefManager.Instance.Show();

            List<BlogPage> blogPages = new List<BlogPage>();

            var userName = await GetUserName();

            var firstPageHtml = await CefManager.Instance.OpenWithWait(string.Format(Urls.CnBLogsPageUrl, userName, "1"));

            var totalPage = GetTotalPage(firstPageHtml);
            var firstPage = GetPageBlog(firstPageHtml);
            blogPages.Add(firstPage);

            for (int i = 2; i <= totalPage; i++)
            {
                var pageHtml = await CefManager.Instance.OpenWithWait(string.Format(Urls.CnBLogsPageUrl, userName, i.ToString()));
                blogPages.Add(GetPageBlog(pageHtml));
            }

            return blogPages;
        }

        private BlogPage GetPageBlog(string html)
        {
            return new BlogPage();
        }

        private int GetTotalPage(string firstPageHtml)
        {
            HtmlParser htmlParser = new HtmlParser(firstPageHtml);
            var pageElement = htmlParser.XpathQuery("//div[@class='pager']");
            var pageStr = pageElement.ElementAt(pageElement.Count - 2);
            return Convert.ToInt32(pageStr.InnerText);
        }

        private async Task<string> GetUserName()
        {
            var html = await CefManager.Instance.OpenWithWait(Urls.CnBlogsHomeUrl);
            HtmlParser htmlParser = new HtmlParser(html);
            return htmlParser.XPathAttributeText("//div[@id='relation']", "data-alias");
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
