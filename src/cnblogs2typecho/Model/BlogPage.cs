using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnblogs2typecho.Model
{
    public class BlogPage
    {
        public int PageIndex { get; set; }

        public List<Blog> Blogs { get; set; }
    }
}
