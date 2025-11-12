namespace QuestDemo.Models
{
  public class CaseManagerExportModel : ExportModel
  {
    public int TotalCase {  get; set; }
    public int CaseOpen { get; set; }
    public int CaseClosed { get; set; }
    public int TotalJob { get; set; }
    public List<CaseDetailExportModel> CaseDetailExports { get; set; } = new List<CaseDetailExportModel>();
    public CaseManagerExportModel() { }
  }
}
