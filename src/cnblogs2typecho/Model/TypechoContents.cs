using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnblogs2typecho.Model
{
    public class TypechoContents
    {
        public int cid { get; set; } // `cid` int(10) unsigned NOT NULL AUTO_INCREMENT
        public string title { get; set; } // `title` varchar(150) DEFAULT NULL
        public string slug { get; set; } // `slug` varchar(150) DEFAULT NULL
        public uint created { get; set; } = 0; // `created` int(10) unsigned DEFAULT '0'
        public uint modified { get; set; } = 0; // `modified` int(10) unsigned DEFAULT '0'
        public string text { get; set; } // `text` longtext
        public uint order { get; set; } = 0; // `order` int(10) unsigned DEFAULT '0'
        public uint authorId { get; set; } = 0; // `authorId` int(10) unsigned DEFAULT '0'
        public string template { get; set; } // `template` varchar(32) DEFAULT NULL
        public string type { get; set; } = "post"; // `type` varchar(16) DEFAULT 'post'
        public string status { get; set; } = "publish"; // `status` varchar(16) DEFAULT 'publish'
        public string password { get; set; } // `password` varchar(32) DEFAULT NULL
        public uint commentsNum { get; set; } = 0; // `commentsNum` int(10) unsigned DEFAULT '0'
        public char allowComment { get; set; } = '0'; // `allowComment` char(1) DEFAULT '0'
        public char allowPing { get; set; } = '0'; // `allowPing` char(1) DEFAULT '0'
        public char allowFeed { get; set; } = '0'; // `allowFeed` char(1) DEFAULT '0'
        public uint parent { get; set; } = 0; // `parent` int(10) unsigned DEFAULT '0'
        public int views { get; set; } = 0; // `views` int(11) DEFAULT '0'
        public int agree { get; set; } = 0; // `agree` int(11) NOT NULL DEFAULT '0'
        public int? likes { get; set; } = 0; // `likes` int(10) DEFAULT '0'
    }
}
