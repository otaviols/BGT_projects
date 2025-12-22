using UnityEngine;

public class BaconCollectionSkin : CollectibleSkin
{
  protected bool m_showingFavorited;
  public GameObject m_nameWrapper;
  public GameObject m_phoneUINameWrapper;
  public GameObject m_favoriteStateTextWrapper;
  public UberText m_favoriteStateUberText;
  public GameObject m_favoriteNameBackground;
  public GameObject m_nonFavoriteNameBackground;

  protected virtual string GetFavoritedText() => GameStrings.Get("GLUE_BACON_COLLECTION_FAVORITE");

  public override void ShowFavoriteBanner(bool show)
  {
    this.m_showingFavorited = show;
    this.PopulateNameText();
  }

  protected override void PopulateNameText()
  {
    if ((Object) null != (Object) this.m_favoriteNameBackground)
      this.m_favoriteNameBackground.SetActive(this.m_showingFavorited && this.ShowName);
    if ((Object) null != (Object) this.m_nonFavoriteNameBackground)
      this.m_nonFavoriteNameBackground.SetActive(!this.m_showingFavorited && this.ShowName);
    if (this.m_showingFavorited)
    {
      this.GetActiveNameWrapper().SetActive(false);
      this.m_favoriteStateTextWrapper.SetActive(true);
      this.m_favoriteStateUberText.Text = this.GetFavoritedText();
    }
    else
    {
      this.GetActiveNameWrapper().SetActive(true);
      this.m_favoriteStateTextWrapper.SetActive(false);
      base.PopulateNameText();
    }
  }

  protected GameObject GetActiveNameWrapper() => !(bool) UniversalInputManager.UsePhoneUI ? this.m_nameWrapper : this.m_phoneUINameWrapper;
}
