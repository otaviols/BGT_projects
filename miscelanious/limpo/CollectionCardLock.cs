using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class CollectionCardLock : MonoBehaviour
{
  [SerializeField]
  private GameObject m_allyBg;
  [SerializeField]
  private GameObject m_spellBg;
  [SerializeField]
  private GameObject m_weaponBg;
  [SerializeField]
  private GameObject m_locationBg;
  [SerializeField]
  private GameObject m_lockPlate;
  [SerializeField]
  private GameObject m_signatureLockPlate;
  [SerializeField]
  private GameObject m_signatureBg;
  [SerializeField]
  private GameObject m_bannedRibbon;
  [SerializeField]
  private UberText m_lockText;
  [SerializeField]
  private GameObject m_lockPlateBone;
  [SerializeField]
  private GameObject m_weaponLockPlateBone;
  [SerializeField]
  private GameObject m_heroLockPlateBone;
  private EntityDef m_entityDef;
  private string m_lockReason;

  public void UpdateLockVisual(Actor actor, CollectionCardVisual.LockType lockType, string reason)
  {
    this.m_entityDef = actor.GetEntityDef();
    if (this.m_entityDef == null || lockType == CollectionCardVisual.LockType.NONE)
    {
      this.gameObject.SetActive(false);
    }
    else
    {
      this.m_lockReason = reason;
      this.gameObject.SetActive(true);
      this.m_bannedRibbon.SetActive(false);
      this.m_allyBg.SetActive(false);
      this.m_spellBg.SetActive(false);
      this.m_weaponBg.SetActive(false);
      this.m_locationBg.SetActive(false);
      this.m_signatureBg.SetActive(false);
      GameObject gameObject;
      switch (this.m_entityDef.GetCardType())
      {
        case TAG_CARDTYPE.HERO:
          gameObject = this.m_allyBg;
          this.m_lockPlate.transform.localPosition = this.m_heroLockPlateBone.transform.localPosition;
          break;
        case TAG_CARDTYPE.MINION:
          gameObject = this.m_allyBg;
          this.m_lockPlate.transform.localPosition = this.m_lockPlateBone.transform.localPosition;
          break;
        case TAG_CARDTYPE.SPELL:
          gameObject = this.m_spellBg;
          this.m_lockPlate.transform.localPosition = this.m_lockPlateBone.transform.localPosition;
          break;
        case TAG_CARDTYPE.WEAPON:
          gameObject = this.m_weaponBg;
          this.m_lockPlate.transform.localPosition = this.m_weaponLockPlateBone.transform.localPosition;
          break;
        case TAG_CARDTYPE.LOCATION:
          gameObject = this.m_locationBg;
          this.m_lockPlate.transform.localPosition = this.m_lockPlateBone.transform.localPosition;
          break;
        default:
          gameObject = this.m_spellBg;
          break;
      }
      float num = 0.0f;
      switch (lockType)
      {
        case CollectionCardVisual.LockType.MAX_COPIES_IN_DECK:
          num = 0.0f;
          this.SetLockText(GameStrings.Format("GLUE_COLLECTION_LOCK_MAX_DECK_COPIES", (object) (this.m_entityDef.IsElite() ? 1 : 2)));
          break;
        case CollectionCardVisual.LockType.NO_MORE_INSTANCES:
          num = 1f;
          this.SetLockText(GameStrings.Get("GLUE_COLLECTION_LOCK_NO_MORE_INSTANCES"));
          break;
        case CollectionCardVisual.LockType.NOT_PLAYABLE:
          num = 1f;
          this.SetLockText(GameStrings.Get("GLUE_COLLECTION_LOCK_CARD_NOT_PLAYABLE"));
          break;
        case CollectionCardVisual.LockType.BANNED:
          this.m_bannedRibbon.SetActive(true);
          this.m_lockPlate.SetActive(false);
          this.m_signatureLockPlate.SetActive(false);
          this.m_signatureBg.SetActive(false);
          gameObject.SetActive(false);
          return;
      }
      this.m_lockPlate.SetActive(true);
      this.m_lockPlate.GetComponent<Renderer>().GetMaterial().SetFloat("_Desaturate", num);
      if (actor.GetPremium() == TAG_PREMIUM.SIGNATURE)
      {
        this.m_signatureLockPlate.SetActive(true);
        this.m_signatureLockPlate.GetComponent<Renderer>().GetMaterial().SetFloat("_Desaturate", num);
        this.m_signatureBg.GetComponent<Renderer>().GetMaterial().SetFloat("_Desaturate", num);
        this.m_signatureBg.SetActive(true);
        gameObject.SetActive(false);
      }
      else
      {
        gameObject.SetActive(true);
        gameObject.GetComponent<Renderer>().GetMaterial().SetFloat("_Desaturate", num);
        this.m_signatureLockPlate.SetActive(false);
      }
      this.SetLockText(this.m_lockReason);
    }
  }

  public void SetLockText(string text) => this.m_lockText.Text = text;

  public void Hide() => this.gameObject.SetActive(false);
}
