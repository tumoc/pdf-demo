namespace QuestDemo.Models
{
  public class ExportModel
  {
    public DateTime DateGenerated { get; set; }
    public string ExportPath { get; set; } = string.Empty;
    public string ExportName { get; set; } = string.Empty;
  }
}
