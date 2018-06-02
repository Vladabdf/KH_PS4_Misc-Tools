using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GnfHelper
{
    public class ImageConversionCommand
    {
        public string FilePath { get; set; }
        public ImageType ImgType { get; set; }

        public ImageConversionCommand(string path)
        {
            FilePath = path;
        }

        //Code from http://csharphelper.com/blog/2016/12/provide-gamma-correction-for-an-image-in-c/
        private Bitmap AdjustGamma(Image image)
        {
            // Set the ImageAttributes object's gamma value.
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetGamma(.45f);

            // Draw the image onto the new bitmap
            // while applying the new gamma value.
            Point[] points =
            {
                new Point(0, 0),
                new Point(image.Width, 0),
                new Point(0, image.Height),
            };
            Rectangle rect =
                new Rectangle(0, 0, image.Width, image.Height);

            // Make the result bitmap.
            Bitmap bm = new Bitmap(image.Width, image.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.DrawImage(image, points, rect,
                    GraphicsUnit.Pixel, attributes);
            }

            // Return the result.
            return bm;
        }
        public void Save()
        {
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "temp", Path.GetFileName(FilePath));

            string savePath = Path.ChangeExtension(Path.Combine(Directory.GetCurrentDirectory(), "output", Path.GetFileName(FilePath)), ".gnf");
            var image = AdjustGamma(Image.FromFile(FilePath));
            if (ImgType == ImageType.Bc3Unorm)
            {
                var target = image.Clone(
                    new Rectangle(0, 0, image.Width, image.Height),
                    PixelFormat.Format8bppIndexed);
                image = target;
            }
            image.Save(tempPath, ImageFormat.Png);

            Process p = new Process();
            p.StartInfo.FileName = "orbis-image2gnf.exe";
            p.StartInfo.Arguments = $"-f {ImgType.ToString()} -m 1 -i \"{tempPath}\" -o \"{savePath}\"";
            p.Start();
            p.WaitForExit();
        }
    }
    public enum ImageType
    {
        Bc3Unorm, R8G8B8A8Unorm
    }
}
