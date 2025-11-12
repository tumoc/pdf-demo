using System.IO;

namespace FashDemo.Models
{
  public class Product
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public byte[] Image { get; set; } // ảnh lưu dạng byte[]

    public static byte[] LoadImageBytes(string path)
    {
      return File.Exists(path) ? File.ReadAllBytes(path) : Array.Empty<byte>();
    }
  }
}
