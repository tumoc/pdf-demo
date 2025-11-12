namespace QuestDemo.Models
{
  public class CaseDetailExportModel : ExportModel
  {
    public string SectionKey { get; set; } = string.Empty;
    public int CaseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Status { get; set; }
    public List<string> Platform { get; set; } = new List<string>();
    public string Keywords { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public int Completed { get; set; }
    public int Waiting { get; set; }
    public int Failed { get; set; }
    public int Inprogress { get; set; }
    public List<JobDetailExportModel> JobDetailExports { get; set; } = new List<JobDetailExportModel>();
    public CaseDetailExportModel() {
      ExportName = CaseId + ".pdf";
    }
  }
}
