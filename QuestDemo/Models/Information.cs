namespace QuestDemo.Models
{
  public class Information
  {
    public string SectionKey { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string DOB { get; set; } = string.Empty;
    public string POB { get; set; } = string.Empty;
    public string CurrentCity { get; set; } = string.Empty;
    public string Hometown { get; set; } = string.Empty;
    public string Languages { get; set; } = string.Empty;
    public string Religion { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public ProfileEducation Education { get; set; } = new ProfileEducation();
    public ProfileWork Work { get; set; } = new ProfileWork();
    public Information() { }
  }
}
