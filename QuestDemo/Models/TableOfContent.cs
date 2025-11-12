namespace QuestDemo.Models
{
  public class TableOfContent
  {
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int PageNumber { get; set; }
    public string SectionKey { get; set; } = string.Empty;
    public List<TableOfContent> Contents { get; set; } = new List<TableOfContent>();
  }
}
