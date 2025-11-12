namespace QuestDemo.Models
{
  public class DataOverview
  {
    public string SectionKey { get; set; } = string.Empty;
    public int Friends { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    public int Reels { get; set; }
    public int Photos { get; set; }
    public int Videos { get; set; }
    public int Music { get; set; }
    public int Sports { get; set; }
    public DataOverview() { }
  }
}
