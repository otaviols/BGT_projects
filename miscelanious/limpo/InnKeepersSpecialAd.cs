using System.Security.Cryptography;
using System.Text;

public class InnKeepersSpecialAd
{
  public string GameAction { get; set; }

  public int MaxViewCount { get; set; }

  public int CurrentViewCount { get; set; }

  public int Importance { get; set; }

  public long PublishDate { get; set; }

  public string CampaignName { get; set; }

  public string Link { get; set; }

  public string Contents { get; }

  public string Title { get; set; }

  public string SubTitle { get; set; }

  public string ButtonText { get; set; }

  public int TitleOffsetX { get; set; }

  public int TitleOffsetY { get; set; }

  public int SubTitleOffsetX { get; set; }

  public int SubTitleOffsetY { get; set; }

  public int TitleFontSize { get; set; }

  public int SubTitleFontSize { get; set; }

  public string ImageUrl { get; set; }

  public string ClientVersion { get; set; }

  public string Platform { get; set; }

  public string AndroidStore { get; set; }

  public bool Visibility { get; set; }

  public static int ComparisonDescending(InnKeepersSpecialAd a, InnKeepersSpecialAd b)
  {
    if (a.Importance < b.Importance)
      return 1;
    if (a.Importance > b.Importance)
      return -1;
    if (a.PublishDate < b.PublishDate)
      return 1;
    return a.PublishDate > b.PublishDate ? -1 : 0;
  }

  public string GetHash()
  {
    string s = this.GameAction + this.Contents + this.Title + this.SubTitle + this.ButtonText + this.Link;
    using (SHA256 shA256 = SHA256.Create())
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in shA256.ComputeHash(Encoding.UTF8.GetBytes(s)))
        stringBuilder.Append(num.ToString("x2"));
      return stringBuilder.ToString();
    }
  }
}
