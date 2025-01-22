using CefSharp;
using cnblogs2typecho.Browser;
using cnblogs2typecho.DAL;
using cnblogs2typecho.Model;
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
    /// MigrationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MigrationWindow : TianXiaTech.BlurWindow
    {
        private TypechoDal typechoDal;

        public MigrationWindow(List<BlogPage> blogPages,TypechoDal typechoDal)
        {
            InitializeComponent();

            cbox_Blogs.ItemsSource = blogPages;
            cbox_Blogs.SelectedIndex = 0;

            this.typechoDal = typechoDal;
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.list.SelectedItem == null)
                return;

            var blog = this.list.SelectedItem as Blog;

            this.tbox_Catetory.Text = blog.Category;
            this.tbox_Slug.Text = blog.Slug;

            var tags = "";
            foreach (var tag in blog.Tags)
            {
                tags += tag + ";";
            }
            this.tbox_Tags.Text = tags;
            this.tbox_Title.Text = blog.Title;
            this.dpk_CreateDate.SelectedDate = blog.CreateDate;
            this.dpk_ModifyDate.SelectedDate = blog.ModifyDate;

            try
            {
                this.browser.LoadHtml(AppendHtmlHead(blog.Content));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string AppendHtmlHead(string html)
        {
            var body = $@"<!DOCTYPE html>
<html lang=""zh-CN"">
<head>
    <meta charset=""UTF-8"">
</head>
<body>
{html}
</body>
</html>
";
            return body;
        }

        private void BlurWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CefManager.Instance.Close();
            typechoDal.Close();
        }

        private void btn_SyncSelected_Click(object sender, RoutedEventArgs e)
        {
            if (this.list.SelectedItem == null)
                return;

            typechoDal.AddBlog(this.list.SelectedItem as Blog);
        }

        private void btn_SyncCurrentPage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_SyncAllPage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_SaveBlogSetting_Click(object sender, RoutedEventArgs e)
        {
            if (this.list.SelectedItem == null)
                return;

            var blog = this.list.SelectedItem as Blog;

            blog.Category = this.tbox_Catetory.Text;
            blog.Slug = this.tbox_Slug.Text;

            var tagText = this.tbox_Tags.Text;
            if(tagText.EndsWith(";"))
            {
                tagText = tagText.Substring(0, tagText.Length - 1);
            }
            blog.Tags = tagText.Split(';');

            blog.Title = this.tbox_Title.Text;
            blog.CreateDate = this.dpk_CreateDate.SelectedDate.Value;
        }
    }
}
