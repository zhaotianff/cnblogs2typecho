using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnblogs2typecho.Model
{
    public class Blog
    {
        public string Title { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public string Category { get; set; }

        public string[] Tags { get; set; }

        public string Slug { get; set; }

        public string Content { get; set; }
    }
}
