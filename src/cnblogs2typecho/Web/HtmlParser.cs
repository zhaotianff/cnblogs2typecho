using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnblogs2typecho.Web
{
    public class HtmlParser
    {
        private string html;
        private HtmlAgilityPack.HtmlDocument doc;

        public HtmlParser(string html)
        {
            this.html = html;
            doc = new HtmlDocument();
            doc.LoadHtml(html);
        }

        public string Title
        {
            get
            {
                return doc.DocumentNode.SelectSingleNode("//title").InnerText;
            }
        }

        public string GetFirstTagElementText(string tagName)
        {
            return doc.DocumentNode.SelectSingleNode($"//{tagName}").InnerText;
        }

        public string XPathText(string xpath)
        {
            return doc.DocumentNode.SelectSingleNode(xpath).InnerText;
        }

        public string XPathAttributeText(string xpath,string attribute)
        {
            return doc.DocumentNode.SelectSingleNode(xpath).Attributes[attribute].Value;
        }

        public HtmlNodeCollection XpathQuery(string xpath)
        {
            return doc.DocumentNode.SelectNodes(xpath);
        }

        public string CnBlogTitle
        {
            get => doc.DocumentNode.SelectSingleNode("//h1[@class='postTitle']").InnerText.Trim();
        }

        public string CnBlogCategory
        {
            get
            {
                var categoryElement = doc.DocumentNode.SelectSingleNode("//div[@id='BlogPostCategory']/a");

                if (categoryElement != null)
                    return categoryElement.InnerText.Trim();

                return "未分类";
            }
        }
         
        public string CnBlogContent 
        { 
            get => doc.DocumentNode.SelectSingleNode("//div[@id='cnblogs_post_body']").InnerHtml.Trim(); 
        }

        public DateTime CnBlogCreateDate 
        { 
            get
            {
                var dateSpan = doc.DocumentNode.SelectSingleNode("//span[@id='post-date']");

                DateTime createDate = DateTime.Now;
                DateTime updateDate = DateTime.Now;

                DateTime.TryParse(dateSpan.InnerText, out createDate);
                DateTime.TryParse(dateSpan.Attributes["data-date-updated"].Value, out updateDate);

                CnBlogModifyDate = updateDate;

                return createDate;
            }
        }

        public DateTime CnBlogModifyDate 
        {
            get;
            private set;
        }

        public string[] CnBlogTags 
        { 
            get
            {
                var metaElementList = doc.DocumentNode.Element("html").Element("head").Elements("meta");

                if (metaElementList == null)
                    return new string[] { "未分类" };

                foreach (var metaElement in metaElementList)
                {
                    if (metaElement.Attributes["name"] != null && metaElement.Attributes["name"].Value == "keywords")
                    {
                        return metaElement.Attributes["content"].Value.Split(',');
                    }
                }

                return new string[] { };
            }
        }
    }
}
