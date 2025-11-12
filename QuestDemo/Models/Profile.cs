namespace QuestDemo.Models
{
  public class Profile
  {
    public string SectionKey { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string OrtherName { get; set; } = string.Empty;
    public string NamePronunciation { get; set; } = string.Empty;
    public string Pronouns { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;
    public string FavouriteQuotes { get; set; } = string.Empty;
    public string CreationDate { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Ads { get; set; } = string.Empty;
    public List<string> Websites { get; set; } = new List<string>();
    public List<string> SocialLinks { get; set; } = new List<string>();
    public Profile() { }
  }
}
