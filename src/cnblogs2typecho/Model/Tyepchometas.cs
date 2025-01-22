using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnblogs2typecho.Model
{
    public class Typechometa
    {
        public int mid { get; set; }

        public string name { get; set; }

        public string slug { get; set; }

        public string type { get; set; }

        public string description { get; set; }

        public int count { get; set; }

        public int order { get; set; }

        public int parent { get; set; }
    }
}
