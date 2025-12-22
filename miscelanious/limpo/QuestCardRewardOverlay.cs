using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class QuestCardRewardOverlay : MonoBehaviour
{
  [Header("Reward Objects")]
  public MeshRenderer m_RewardOverlayRenderer;
  public GameObject m_RewardText;
  [Header("Reward Overlay Texture Settings")]
  public Texture m_MinionRewardOverlayTexture;
  public Texture m_LegendaryMinionRewardOverlayTexture;
  public Texture m_SpellRewardOverlayTexture;
  public Texture m_GoldenSpellRewardOverlayTexture;
  public Texture m_WeaponRewardOverlayTexture;
  public Texture m_LegendaryWeaponRewardOverlayTexture;
  public Texture m_HeroPowerRewardOverlayTexture;
  public Texture m_LocationRewardOverlayTexture;
  [Header("Reward Overlay Position Settings")]
  public Vector3 m_MinionRewardPosition;
  public Vector3 m_SpellRewardPosition;
  public Vector3 m_WeaponRewardPosition;
  public Vector3 m_HeroPowerRewardPosition;
  public Vector3 m_LocationRewardPosition;
  [Header("Reward Background Glow Reference Settings")]
  public MeshRenderer m_RewardBackGlowRenderer;
  public Material m_DefaultRewardBackGlowMaterial;
  public Material m_HeroPowerRewardBackGlowMaterial;

  public void SetEntityType(EntityDef def, bool isPremium)
  {
    if ((Object) this.m_RewardOverlayRenderer != (Object) null)
    {
      TAG_CARDTYPE cardType = def.GetCardType();
      Texture textureForCardType = this.GetOverlayTextureForCardType(cardType, isPremium, def.IsElite());
      Material material = this.m_RewardOverlayRenderer.GetMaterial();
      material.SetTexture("_MainTex", textureForCardType);
      material.SetTexture("_AddTex", textureForCardType);
      this.m_RewardOverlayRenderer.transform.localPosition = this.GetPositionForCardType(cardType);
    }
    if (!((Object) this.m_RewardBackGlowRenderer != (Object) null))
      return;
    this.m_RewardBackGlowRenderer.SetMaterial(def.IsHeroPower() ? this.m_HeroPowerRewardBackGlowMaterial : this.m_DefaultRewardBackGlowMaterial);
  }

  public void EnableRewardObjects()
  {
    this.m_RewardOverlayRenderer.gameObject.SetActive(true);
    this.m_RewardText.SetActive(true);
  }

  private Texture GetOverlayTextureForCardType(
    TAG_CARDTYPE cardType,
    bool isPremium,
    bool isElite)
  {
    switch (cardType)
    {
      case TAG_CARDTYPE.MINION:
        return !isElite ? this.m_MinionRewardOverlayTexture : this.m_LegendaryMinionRewardOverlayTexture;
      case TAG_CARDTYPE.SPELL:
        return !isPremium ? this.m_SpellRewardOverlayTexture : this.m_GoldenSpellRewardOverlayTexture;
      case TAG_CARDTYPE.WEAPON:
        return !isElite ? this.m_WeaponRewardOverlayTexture : this.m_LegendaryWeaponRewardOverlayTexture;
      case TAG_CARDTYPE.HERO_POWER:
        return this.m_HeroPowerRewardOverlayTexture;
      case TAG_CARDTYPE.LOCATION:
        return this.m_LocationRewardOverlayTexture;
      default:
        Debug.LogErrorFormat("Could not get quest overlay texture, unsupported type {0}", (object) cardType.ToString());
        return (Texture) null;
    }
  }

  private Vector3 GetPositionForCardType(TAG_CARDTYPE cardType)
  {
    switch (cardType)
    {
      case TAG_CARDTYPE.MINION:
        return this.m_MinionRewardPosition;
      case TAG_CARDTYPE.SPELL:
        return this.m_SpellRewardPosition;
      case TAG_CARDTYPE.WEAPON:
        return this.m_WeaponRewardPosition;
      case TAG_CARDTYPE.HERO_POWER:
        return this.m_HeroPowerRewardPosition;
      case TAG_CARDTYPE.LOCATION:
        return this.m_LocationRewardPosition;
      default:
        Debug.LogErrorFormat("Could not get quest overlay position, unsupported type {0}", (object) cardType.ToString());
        return new Vector3();
    }
  }
}
