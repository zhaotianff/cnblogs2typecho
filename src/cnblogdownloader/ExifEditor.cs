using HiddenWatermark;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace cnblogdownloader
{
    public class ExifEditor
    {

        public static void DrawWatermark(string imageFilePath,string watermark)
        {
            using (MemoryStream ms = new MemoryStream(System.IO.File.ReadAllBytes(imageFilePath)))
            {
                Bitmap bitmap = (Bitmap)Image.FromStream(ms);

                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    var size = (float)(bitmap.Height * 0.02);
                    if (size < 10)
                        size = 10f;
                    using (Font arialFont = new Font("Arial", size, GraphicsUnit.Pixel))
                    {
                        var location = new Point(0, 0);
                        graphics.DrawString(watermark, arialFont, new SolidBrush(Color.FromArgb(120, Color.Silver)), location);
                    }
                }

                bitmap.Save(imageFilePath);//save the image file
            }
        }

        public static void EmbedDefaultWatermark(string imageFilePath)
        {
            var fileBytes = File.ReadAllBytes(imageFilePath);
            var newFileBytes = Watermark.Default.EmbedWatermark(fileBytes);
            System.IO.File.WriteAllBytes(imageFilePath, newFileBytes);
        }

        public static void EmbedTextWatermark(string imageFilePath,string text)
        {
            using(MemoryStream ms = new MemoryStream(System.IO.File.ReadAllBytes(imageFilePath)))
            {
                var newBitmap = Steganography.SteganographyHelper.embedText(text, (Bitmap)Bitmap.FromStream(ms));
                newBitmap.Save(imageFilePath);
            }
        }

        private static System.Drawing.Image GetImage(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);

                MemoryStream memoryStream = new MemoryStream(bytes);
                if (memoryStream != null)
                {
                    return System.Drawing.Image.FromStream(memoryStream);
                }
            }

            return null;
        }
    }
}
