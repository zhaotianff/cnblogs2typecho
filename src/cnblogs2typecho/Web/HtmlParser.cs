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
    }
}
