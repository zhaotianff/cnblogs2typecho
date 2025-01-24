using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cnblogdownloader
{
    public class DownloadUtil
    {
        public async static Task<string> DownloadFile(string url,string dir)
        {
            await Task.Delay(1000);
            WebClient webClient = new WebClient();
            var fileName = System.IO.Path.GetFileName(url);

            if (System.IO.Directory.Exists(dir) == false)
                System.IO.Directory.CreateDirectory(dir);

            fileName = System.IO.Path.Combine(dir, fileName);
            try
            {
                webClient.DownloadFile(url, fileName);
                return fileName;
            }
            catch
            {
                return null;
            }
        }
    }
}
