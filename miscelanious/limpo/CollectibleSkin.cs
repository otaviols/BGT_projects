using UnityEngine;

public class CollectibleSkin : MonoBehaviour
{
  public GameObject m_favoriteBanner;
  public UberText m_favoriteBannerText;
  public GameObject m_shadow;
  public UberText m_name;
  public GameObject m_nameShadow;
  public UberText m_collectionManagerName;
  private bool m_showName = true;

  public virtual void Awake()
  {
    Actor component = this.gameObject.GetComponent<Actor>();
    if ((Object) component != (Object) null)
    {
      component.SetUseShortName(true);
      if ((bool) UniversalInputManager.UsePhoneUI)
        component.OverrideNameText((UberText) null);
    }
    this.ShowName = this.m_showName;
  }

  public bool ShowName
  {
    get => this.m_showName;
    set
    {
      this.m_showName = value;
      this.PopulateNameText();
      if (!((Object) this.m_nameShadow != (Object) null))
        return;
      this.m_nameShadow.gameObject.SetActive(this.m_showName && !(bool) UniversalInputManager.UsePhoneUI);
    }
  }

  protected virtual void PopulateNameText() => this.gameObject.GetComponent<Actor>().OverrideNameText(this.GetActiveNameText());

  public void ShowShadow(bool show)
  {
    if ((Object) this.m_shadow == (Object) null)
      return;
    this.m_shadow.SetActive(show);
  }

  public virtual void ShowFavoriteBanner(bool show)
  {
    if ((Object) this.m_favoriteBanner == (Object) null)
      return;
    this.m_favoriteBanner.SetActive(show);
  }

  public void ShowCollectionManagerText()
  {
    Actor component = this.gameObject.GetComponent<Actor>();
    if (!((Object) component != (Object) null))
      return;
    this.PopulateNameText();
    if (!component.isMissingCard())
      return;
    component.UpdateMissingCardArt();
  }

  protected UberText GetActiveNameText()
  {
    if (!this.m_showName)
      return (UberText) null;
    return !(bool) UniversalInputManager.UsePhoneUI ? this.m_name : this.m_collectionManagerName;
  }

  [ContextMenu("Toggle Missing Card Effect")]
  private void ToggleMissingCardEffect()
  {
    Actor component = this.gameObject.GetComponent<Actor>();
    if (!((Object) component != (Object) null))
      return;
    if (component.isMissingCard())
      component.DisableMissingCardEffect();
    else
      component.MissingCardEffect();
    component.UpdateAllComponents();
  }
}
