namespace QuestDemo.Models
{
  public class JobDetailExportModel : ExportModel
  {
    public int JobId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string SearchInput { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public int Status { get; set; }
    public Profile Profile { get; set; } = new Profile();
    public Information Information { get; set; } = new Information();
    public DataOverview DataOverview { get; set; } = new DataOverview();
    public JobDetailExportModel() {
      ExportName = JobId + ".pdf";
    }
  }
}
