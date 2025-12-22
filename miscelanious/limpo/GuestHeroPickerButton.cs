using UnityEngine;

public class GuestHeroPickerButton : HeroPickerButton
{
  private GuestHeroDbfRecord m_guestHero;

  public void SetGuestHero(GuestHeroDbfRecord guestHero) => this.m_guestHero = guestHero;

  public override GuestHeroDbfRecord GetGuestHero() => this.m_guestHero;

  public override void UpdateDisplay(DefLoader.DisposableFullDef def, TAG_PREMIUM premium)
  {
    base.UpdateDisplay(def, premium);
    if (this.m_guestHero == null)
    {
      this.SetClassname(string.Empty);
      this.m_heroClassIcon.SetActive(false);
    }
    else
    {
      this.m_heroClass = GameUtils.GetTagClassFromCardDbId(this.m_guestHero.CardId);
      this.SetClassname((string) this.m_guestHero.Name);
      this.SetClassIcon(this.GetClassIconMaterial(this.m_heroClass));
      this.SetupClassIconAndName();
    }
  }

  private void SetupClassIconAndName()
  {
    EntityDef entityDef = this.GetEntityDef();
    bool flag = (entityDef != null ? entityDef.GetTag(GAME_TAG.MULTIPLE_CLASSES) : 0) > 0;
    this.m_classLabel.transform.parent = flag ? this.m_bones.m_classLabelNoIcon : this.m_bones.m_classLabelOneLine;
    this.m_classLabel.transform.localPosition = Vector3.zero;
    this.m_classLabel.transform.localScale = Vector3.one;
    this.m_labelGradient.transform.parent = this.m_bones.m_gradientOneLine;
    this.m_labelGradient.transform.localPosition = Vector3.zero;
    this.m_labelGradient.transform.localScale = Vector3.one;
    this.m_heroClassIcon.SetActive(!flag);
  }
}
