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
    }
}
