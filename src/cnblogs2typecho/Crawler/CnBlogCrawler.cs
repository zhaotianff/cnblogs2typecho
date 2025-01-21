using cnblogs2typecho.Browser;
using cnblogs2typecho.Model;
using cnblogs2typecho.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace cnblogs2typecho.Crawler
{
    public class CnBlogCrawler
    {
        public List<string> FalutList { get; private set; } = new List<string>();


        public async Task<List<BlogPage>> EnumAllBlogPage(Action<double,int> progressCallback)
        {
            List<BlogPage> blogPages = new List<BlogPage>();
            FalutList.Clear();

            var userName = await GetUserName();

            //ui
            progressCallback?.Invoke(5, 1);

            var firstPageHtml = await CefManager.Instance.OpenWithWait(string.Format(Urls.CnBLogsPageUrl, userName, "1"));

            var totalPage = GetTotalPage(firstPageHtml);
            var firstPage = await GetPageBlog(firstPageHtml, 1);
            blogPages.Add(firstPage);

            //ui
            var tick = 100d / totalPage;

            for (int i = 2; i <= totalPage; i++)
            {
                //ui
                progressCallback?.Invoke(tick, i);

                var pageHtml = await CefManager.Instance.OpenWithWait(string.Format(Urls.CnBLogsPageUrl, userName, i.ToString()));
                var blogPage = await GetPageBlog(pageHtml, i);
                blogPages.Add(blogPage);

                await Task.Delay(1000);
            }

            //ui
            progressCallback?.Invoke(100, 0);

            return blogPages;
        }

        public async Task<BlogPage> GetPageBlog(string html, int pageIndex)
        {
            BlogPage blogPage = new BlogPage();
            blogPage.PageIndex = pageIndex;
            blogPage.Blogs = new List<Blog>();

            HtmlParser htmlParser = new HtmlParser(html);
            var blogElementList = htmlParser.XpathQuery("//div[@class='postTitle']//a");

            foreach (var item in blogElementList)
            {
                try
                {
                    var url = item.Attributes["href"].Value;
                    Blog blog = await ParseBlog(url);

                    if(blog == null)
                    {
                        FalutList.Add(url);
                        continue;
                    }

                    blogPage.Blogs.Add(blog);

                    await Task.Delay(200);
                }
                catch
                {
                    if (item.Attributes["href"] != null)
                    {
                        FalutList.Add(item.Attributes["href"].Value);
                    }
                }
            }

            return blogPage;
        }

        public async Task<Blog> ParseBlog(string url)
        {
            Blog blog = new Blog();
            blog.Slug = url.Substring(url.LastIndexOf('/') + 1).Replace(".html","");

            await Task.Delay(200);

            var removeCopyCodeElementJs = GetRemoveElementByClassJs("cnblogs_code_toolbar");
            var removeNavigationElementJs = GetRemoveElementByClassJs("cnblogs-toc-button");

            var source = await CefManager.Instance.OpenWithWait(url);
            source = await CefManager.Instance.ExecuteJavascript(removeCopyCodeElementJs);
            source = await CefManager.Instance.ExecuteJavascript(removeNavigationElementJs);
            HtmlParser htmlParser = new HtmlParser(source);

            if(string.IsNullOrEmpty(source))
            {
                return null;
            }

            blog.Title = htmlParser.CnBlogTitle;
            blog.Category = htmlParser.CnBlogCategory;
            blog.Content = htmlParser.CnBlogContent;
            blog.CreateDate = htmlParser.CnBlogCreateDate;
            blog.ModifyDate = htmlParser.CnBlogModifyDate;
            blog.Tags = htmlParser.CnBlogTags;

            return blog;
        }

        private string GetRemoveElementByClassJs(string className)
        {
            return @"var elements = document.getElementsByClassName('" + className + @"');
while(elements.length > 0){
    elements[0].remove();
}";
        }

        public int GetTotalPage(string firstPageHtml)
        {
            HtmlParser htmlParser = new HtmlParser(firstPageHtml);
            var pageElement = htmlParser.XpathQuery("//div[@class='pager']//a");

            if (pageElement == null)
                return 0;

            var pageStr = pageElement.ElementAt(pageElement.Count - 2);
            return Convert.ToInt32(pageStr.InnerText);
        }

        public async Task<string> GetUserName()
        {
            var html = await CefManager.Instance.OpenWithWait(Urls.CnBlogsHomeUrl);
            HtmlParser htmlParser = new HtmlParser(html);
            return htmlParser.XPathAttributeText("//div[@id='relation']", "data-alias");
        }
    }
}
