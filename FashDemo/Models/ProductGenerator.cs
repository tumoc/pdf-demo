using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace FashDemo.Models
{
  public static class ProductGenerator
  {
    public static List<Product> GenerateProducts(int count)
    {
      var rnd = new Random();
      var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
      var images = Directory.GetFiles(baseDir, "*.jpg");

      var list = new List<Product>();

      for (int i = 1; i <= count; i++)
      {
        var imgPath = images[rnd.Next(images.Length)];
        list.Add(new Product
        {
          Id = i,
          Name = $"Sản phẩm #{i}",
          Price = Math.Round((decimal)(rnd.NextDouble() * 5000 + 100), 2),
          Image = ResizeImageToBytes(imgPath, 128, 128)
        });
      }
      return list;
    }
    private static byte[] ResizeImageToBytes(string path, int width, int height)
    {
      using var src = Image.FromFile(path);
      using var bmp = new Bitmap(width, height);

      using (var g = Graphics.FromImage(bmp))
      {
        g.CompositingQuality = CompositingQuality.HighSpeed;
        g.SmoothingMode = SmoothingMode.HighSpeed;
        g.InterpolationMode = InterpolationMode.Low;
        g.DrawImage(src, 0, 0, width, height);
      }

      using var ms = new MemoryStream();
      bmp.Save(ms, ImageFormat.Jpeg);
      return ms.ToArray();
    }
  }
}
