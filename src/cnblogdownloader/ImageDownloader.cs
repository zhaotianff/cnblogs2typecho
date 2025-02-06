using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnblogdownloader
{
    public class ImageDownloader
    {
        private const string BASEURL = "{0}/{1}.html";


        public async Task<string> DownloadCnblogImages(string cnBlogsContent, string directory, string baseDir,string siteUrl,string articleName)
        {
            directory = GetFullDirectory(directory);
        
            var result = await GenerateBlogContent(cnBlogsContent, directory, baseDir);
            CreateImageWatermark(result.Item2, articleName, baseDir, siteUrl);
         
            return result.Item1;
        }

        private void CreateImageWatermark(List<string> imagePathList, string articleName, string baseDir,string siteUrl)
        {
            foreach (var imagePath in imagePathList)
            {
                if (System.IO.Path.GetExtension(imagePath).ToLower() == ".gif")
                    continue;

                try
                {
                    var newPath = baseDir + imagePath.Replace("/", "\\");
                    var waterMark = string.Format(BASEURL, siteUrl, articleName);
                    ExifEditor.DrawWatermark(newPath, waterMark);

                    if (System.IO.Path.GetExtension(imagePath).ToLower() == ".png")
                    {
                        ExifEditor.EmbedTextWatermark(newPath, waterMark);
                    }
                    else
                    {
                        ExifEditor.EmbedDefaultWatermark(newPath);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        private string GetFullDirectory(string directory)
        {
            var files = System.IO.Directory.GetFiles(Environment.CurrentDirectory + "\\download");
            var random = new Random();
            var file = files[random.Next(0, files.Length)];
            file = System.IO.Path.GetFileNameWithoutExtension(file);
            file = System.Text.RegularExpressions.Regex.Replace(file, "\\(\\S+\\)", "");
            return $"{directory}\\{DateTime.Now.Year}\\{DateTime.Now.Month}\\{file}";
        }

        private async Task<Tuple<string, List<string>>> GenerateBlogContent(string cnBlogsContent, string directory, string baseDir)
        {
            var cnBlogsImagePathList = new List<string>();
            var relativeImagePathlist = new List<string>();
            var tempBlogContent = "<html><body>" + cnBlogsContent + "</body></html>";

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(tempBlogContent);
            var imageNodes = doc.DocumentNode.SelectNodes("//img");

            if (imageNodes == null)
            {
                return new Tuple<string, List<string>>(cnBlogsContent, relativeImagePathlist);
            }

            foreach (var imageNode in imageNodes)
            {
                try
                {
                    var url = imageNode.Attributes["src"].Value;
                    var imageName = await DownloadUtil.DownloadFile(url, directory);
                    imageName = imageName.Replace(baseDir, "").Replace("\\", "/");
                    cnBlogsImagePathList.Add(url);
                    relativeImagePathlist.Add(imageName);

                    var initialImgTagHtml = imageNode.OuterHtml;

                    if (cnBlogsContent.Contains(initialImgTagHtml) == false)
                    {
                        initialImgTagHtml = imageNode.OuterHtml.Replace(">", " />");
                    }

                    imageNode.Attributes["src"].Value = imageName;
                    cnBlogsContent = cnBlogsContent.Replace(initialImgTagHtml, imageNode.OuterHtml.Replace(">", " />"));
                }
                catch
                {
                    continue;
                }
            }
            return new Tuple<string, List<string>>(cnBlogsContent, relativeImagePathlist);
        }

    }
}
