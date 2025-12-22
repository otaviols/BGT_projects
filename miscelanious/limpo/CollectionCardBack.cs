using UnityEngine;

public class CollectionCardBack : MonoBehaviour
{
  public UberText m_name;
  public GameObject m_favoriteBanner;
  public GameObject m_nameFrame;
  private int m_cardBackId = -1;
  private int m_seasonId = -1;

  public void Awake()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_nameFrame.SetActive(false);
  }

  public void SetCardBackId(int id) => this.m_cardBackId = id;

  public int GetCardBackId() => this.m_cardBackId;

  public void SetSeasonId(int seasonId) => this.m_seasonId = seasonId;

  public int GetSeasonId() => this.m_seasonId;

  public void SetCardBackName(string name)
  {
    if ((Object) this.m_name == (Object) null)
      return;
    this.m_name.Text = name;
  }

  public void ShowFavoriteBanner(bool show)
  {
    if ((Object) this.m_favoriteBanner == (Object) null)
      return;
    this.m_favoriteBanner.SetActive(show);
  }
}
