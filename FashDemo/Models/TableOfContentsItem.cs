namespace FashDemo.Models
{
  public class TableOfContentsItem
  {
    public int ID { get; set; }
    public string Title { get; set; }
    public int Level { get; set; }
    public string SectionKey { get; set; }
    public int PageNumber { get; set; } 
  }
}
