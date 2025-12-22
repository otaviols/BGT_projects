using UnityEngine;

public class MobileActorGem : MonoBehaviour
{
  public UberText m_uberText;
  public MobileActorGem.GemType m_gemType;
  public Vector3 m_additionalOffset = Vector3.zero;

  private void Awake()
  {
    if (!PlatformSettings.IsMobile())
      return;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if (this.m_gemType == MobileActorGem.GemType.CardPlay)
      {
        this.gameObject.transform.localScale *= 1.6f;
        this.m_uberText.transform.localScale *= 0.9f;
        this.m_uberText.OutlineSize = 3.2f;
      }
      else if (this.m_gemType == MobileActorGem.GemType.CardHero_Attack)
      {
        this.gameObject.transform.localScale *= 1.6f;
        TransformUtil.SetLocalPosX(this.gameObject, this.gameObject.transform.localPosition.x - 0.075f);
        TransformUtil.SetLocalPosZ(this.gameObject, this.gameObject.transform.localPosition.z + 0.255f);
        this.m_uberText.transform.localScale *= 0.9f;
        this.m_uberText.OutlineSize = 3.2f;
      }
      else if (this.m_gemType == MobileActorGem.GemType.CardHero_Health)
      {
        this.gameObject.transform.localScale *= 1.6f;
        TransformUtil.SetLocalPosX(this.gameObject, this.gameObject.transform.localPosition.x + 0.05f);
        TransformUtil.SetLocalPosZ(this.gameObject, this.gameObject.transform.localPosition.z + 0.255f);
        this.m_uberText.transform.localPosition = new Vector3(0.0f, 0.154f, -0.0235f);
        this.m_uberText.OutlineSize = 3.6f;
      }
      else if (this.m_gemType == MobileActorGem.GemType.CardHero_Armor)
      {
        this.gameObject.transform.localScale *= 1.15f;
        TransformUtil.SetLocalPosX(this.gameObject, 0.06f);
        TransformUtil.SetLocalPosZ(this.gameObject, this.gameObject.transform.localPosition.z - 0.3f);
        this.m_uberText.transform.localScale *= 1.4f;
        this.m_uberText.FontSize = 50;
        this.m_uberText.CharacterSize = 8f;
        this.m_uberText.OutlineSize = 3.2f;
      }
      else if (this.m_gemType == MobileActorGem.GemType.CardHeroPower)
      {
        TransformUtil.SetLocalScaleXZ(this.gameObject, new Vector2(1.334f * this.gameObject.transform.localScale.x, 1.334f * this.gameObject.transform.localScale.z));
        TransformUtil.SetLocalScaleXY((Component) this.m_uberText, new Vector2(1.5f * this.m_uberText.transform.localScale.x, 1.5f * this.m_uberText.transform.localScale.y));
        TransformUtil.SetLocalPosZ((Component) this.m_uberText, this.m_uberText.transform.localPosition.z + 0.04f);
      }
      else if (this.m_gemType != MobileActorGem.GemType.CardLettuceAbility)
        ;
    }
    else if (this.m_gemType == MobileActorGem.GemType.CardPlay)
    {
      this.gameObject.transform.localScale *= 1.3f;
      this.m_uberText.transform.localScale *= 0.9f;
      this.m_uberText.OutlineSize = 3.2f;
    }
    TransformUtil.SetLocalPosX(this.gameObject, this.gameObject.transform.localPosition.x + this.m_additionalOffset.x);
    TransformUtil.SetLocalPosY(this.gameObject, this.gameObject.transform.localPosition.y + this.m_additionalOffset.y);
    TransformUtil.SetLocalPosZ(this.gameObject, this.gameObject.transform.localPosition.z + this.m_additionalOffset.z);
  }

  public enum GemType
  {
    CardPlay,
    CardHero_Health,
    CardHero_Attack,
    CardHero_Armor,
    CardHeroPower,
    CardLettuceAbility,
  }
}
