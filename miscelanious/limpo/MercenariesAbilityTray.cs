using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenariesAbilityTray : MonoBehaviour
{
  public float m_showTweenTime = 0.2f;
  public float m_hideTweenTime = 0.1f;
  public int m_maxAbilitiesOnTray = 4;
  public GameObject m_pcLeftBigCardBone;
  public GameObject m_pcRightBigCard3TrayBone;
  public GameObject m_pcRightBigCard4TrayBone;
  public GameObject m_mobileLeftBigCardBone;
  public GameObject m_mobileRightBigCard3TrayBone;
  public GameObject m_mobileRightBigCard4TrayBone;
  public Float_MobileOverride m_abilityPreviewScale;
  [SerializeField]
  public List<MercenariesAbilityTray.AbilityTrayBackgroundMapping> m_threeAbilityBackgrounds;
  [SerializeField]
  public List<MercenariesAbilityTray.AbilityTrayBackgroundMapping> m_fourAbilityBackgrounds;
  [SerializeField]
  public List<MercenariesAbilityTray.AbilityCoverMapping> m_abilityCovers;
  [SerializeField]
  public List<MercenariesAbilityTray.AbilityBoneMapping> m_abilityBones;
  public PlayMakerFSM PlaymakerFsm;
  private Entity m_abilityOwnerEntity;
  private List<Card> m_abilityCards;
  private List<Card> m_lastShownAbilityCards = new List<Card>();
  private bool m_isAnimatingHide;
  private bool m_isAnimatingShow;
  private Coroutine m_showCoroutine;
  private Coroutine m_hideCoroutine;
  private bool m_isVisible;
  private readonly Vector3 OFFSCREEN_POSITION = new Vector3(-5000f, -5000f, -5000f);

  public void Start()
  {
    this.PlaymakerFsm.FsmVariables.GetFsmFloat("ShowTweenTime").Value = this.m_showTweenTime;
    this.PlaymakerFsm.FsmVariables.GetFsmFloat("HideTweenTime").Value = this.m_hideTweenTime;
    if (!Debug.isDebugBuild)
      return;
    foreach (double num in this.m_abilityPreviewScale.GetValues())
    {
      if (num <= 0.0)
        Debug.LogError((object) ("m_abilityPreviewScale on object \"" + this.gameObject.name + "\" contains at least one invalid value for scale. All values should be positive numbers, roughly in range 1.0-3.0."));
    }
  }

  public void SetupForMercenary(Entity mercenaryEntity, List<Card> abilityCards)
  {
    if (mercenaryEntity == null)
      Log.Lettuce.PrintError("MercenariesAbilityTray.SetupForMercenary - null mercenary entity");
    this.m_abilityOwnerEntity = mercenaryEntity;
    this.m_abilityCards = new List<Card>((IEnumerable<Card>) abilityCards);
  }

  public void Show()
  {
    this.m_isVisible = true;
    if (this.m_showCoroutine != null)
    {
      this.StopCoroutine(this.m_showCoroutine);
      this.m_isAnimatingShow = false;
    }
    this.m_showCoroutine = this.StartCoroutine(this.ShowCoroutine());
  }

  public void Hide()
  {
    this.m_isVisible = false;
    if (this.m_showCoroutine != null)
    {
      this.StopCoroutine(this.m_showCoroutine);
      this.m_isAnimatingShow = false;
    }
    if (this.m_hideCoroutine != null)
    {
      this.StopCoroutine(this.m_hideCoroutine);
      this.m_isAnimatingHide = false;
    }
    this.m_hideCoroutine = this.StartCoroutine(this.HideCoroutine());
  }

  public bool IsAnimating() => this.m_isAnimatingHide || this.m_isAnimatingShow;

  public bool IsVisible() => this.m_isVisible;

  private void SetBackgroundForEntity(Entity entity, int numberOfAbilities)
  {
    int tag = entity.GetTag(GAME_TAG.LETTUCE_ROLE);
    this.HideAllBackgrounds();
    GameObject backgroundForRole;
    switch (tag)
    {
      case 1:
      case 2:
      case 3:
        backgroundForRole = this.GetBackgroundForRole((TAG_ROLE) tag, numberOfAbilities);
        break;
      default:
        backgroundForRole = this.GetBackgroundForRole(TAG_ROLE.INVALID, numberOfAbilities);
        break;
    }
    if ((UnityEngine.Object) backgroundForRole != (UnityEngine.Object) null)
      backgroundForRole.SetActive(true);
    List<GameObject> abilityCoversForRole = this.GetAbilityCoversForRole((TAG_ROLE) tag);
    if (abilityCoversForRole == null)
      return;
    for (int index = numberOfAbilities; index < abilityCoversForRole.Count; ++index)
      abilityCoversForRole[index].SetActive(true);
  }

  private void HideAllBackgrounds()
  {
    foreach (MercenariesAbilityTray.AbilityTrayBackgroundMapping abilityBackground in this.m_threeAbilityBackgrounds)
      abilityBackground.m_background.SetActive(false);
    foreach (MercenariesAbilityTray.AbilityTrayBackgroundMapping abilityBackground in this.m_fourAbilityBackgrounds)
      abilityBackground.m_background.SetActive(false);
    foreach (MercenariesAbilityTray.AbilityCoverMapping abilityCover in this.m_abilityCovers)
    {
      foreach (GameObject cover in abilityCover.m_covers)
        cover.SetActive(false);
    }
  }

  private GameObject GetBackgroundForRole(TAG_ROLE role, int numberOfAbilities)
  {
    foreach (MercenariesAbilityTray.AbilityTrayBackgroundMapping backgroundMapping in numberOfAbilities > 3 ? this.m_fourAbilityBackgrounds : this.m_threeAbilityBackgrounds)
    {
      if (backgroundMapping.m_role == role)
        return backgroundMapping.m_background;
    }
    return (GameObject) null;
  }

  private List<GameObject> GetAbilityCoversForRole(TAG_ROLE role)
  {
    foreach (MercenariesAbilityTray.AbilityCoverMapping abilityCover in this.m_abilityCovers)
    {
      if (abilityCover.m_role == role)
        return abilityCover.m_covers;
    }
    return (List<GameObject>) null;
  }

  private IEnumerator HideCoroutine()
  {
    this.m_isAnimatingHide = true;
    this.PlaymakerFsm.SendEvent("Death");
    yield return (object) new WaitForSeconds(this.m_hideTweenTime);
    this.EnsureLastShownAbilityCardsAreHidden();
    this.m_isAnimatingHide = false;
  }

  private void EnsureLastShownAbilityCardsAreHidden()
  {
    if (this.m_lastShownAbilityCards.Count == 0)
      return;
    foreach (Card shownAbilityCard in this.m_lastShownAbilityCards)
    {
      Actor actor = shownAbilityCard.GetActor();
      if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
        actor.gameObject.transform.position = this.OFFSCREEN_POSITION;
    }
    this.m_lastShownAbilityCards.Clear();
  }

  private IEnumerator ShowCoroutine()
  {
    while (this.m_isAnimatingHide)
      yield return (object) null;
    this.EnsureLastShownAbilityCardsAreHidden();
    this.m_isAnimatingShow = true;
    if (this.m_abilityOwnerEntity != null)
    {
      int count = this.m_abilityCards.Count;
      foreach (Card abilityCard in this.m_abilityCards)
      {
        if (abilityCard.GetEntity().GetZone() == TAG_ZONE.SETASIDE)
          --count;
      }
      this.SetBackgroundForEntity(this.m_abilityOwnerEntity, count);
      this.PlaymakerFsm.FsmVariables.GetFsmVector3("MercenaryPosition").Value = this.m_abilityOwnerEntity.GetCard().transform.position;
    }
    this.PlaymakerFsm.FsmVariables.GetFsmFloat("ShowTweenTime").Value = this.m_showTweenTime;
    for (int index = 0; index < this.m_maxAbilitiesOnTray; ++index)
    {
      string name = "AbilityActor" + (object) (index + 1);
      if (index >= this.m_abilityCards.Count)
      {
        this.PlaymakerFsm.FsmVariables.GetFsmGameObject(name).Value = (GameObject) null;
      }
      else
      {
        Card abilityCard = this.m_abilityCards[index];
        if ((UnityEngine.Object) abilityCard == (UnityEngine.Object) null || (UnityEngine.Object) abilityCard.GetActor() == (UnityEngine.Object) null)
        {
          this.PlaymakerFsm.FsmVariables.GetFsmGameObject(name).Value = (GameObject) null;
        }
        else
        {
          abilityCard.UpdateActorState();
          if (abilityCard.GetActor() is LettuceAbilityActor actor)
            actor.UpdateCheckMarkObject();
          this.SetAbilityBonePositionByTags(abilityCard, index);
          this.PlaymakerFsm.FsmVariables.GetFsmGameObject(name).Value = abilityCard.GetActor().gameObject;
        }
      }
    }
    this.PlaymakerFsm.SendEvent("Birth");
    this.m_lastShownAbilityCards.AddRange((IEnumerable<Card>) this.m_abilityCards);
    yield return (object) new WaitForSeconds(this.m_showTweenTime);
    this.m_isAnimatingShow = false;
  }

  private void SetAbilityBonePositionByTags(Card abilityCard, int abilityBoneIndex)
  {
    if (abilityBoneIndex < 0 || abilityBoneIndex > this.m_abilityBones.Count)
    {
      Log.Lettuce.PrintError("SetAbilityBonePositionByTags - Invalid index {0}", (object) abilityBoneIndex);
    }
    else
    {
      MercenariesAbilityTray.AbilityBoneMapping abilityBone = this.m_abilityBones[abilityBoneIndex];
      if (abilityCard.GetEntity().HasTag(GAME_TAG.LETTUCE_CURRENT_COOLDOWN))
        abilityBone.m_abilityBone.localPosition = abilityBone.m_socketedPosition.localPosition;
      else
        abilityBone.m_abilityBone.localPosition = abilityBone.m_shownPosition.localPosition;
    }
  }

  public void GetBigCardBones(out GameObject left, out GameObject right)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      left = this.m_mobileLeftBigCardBone;
      if (this.m_abilityCards.Count <= 3)
        right = this.m_mobileRightBigCard3TrayBone;
      else
        right = this.m_mobileRightBigCard4TrayBone;
    }
    else
    {
      left = this.m_pcLeftBigCardBone;
      if (this.m_abilityCards.Count <= 3)
        right = this.m_pcRightBigCard3TrayBone;
      else
        right = this.m_pcRightBigCard4TrayBone;
    }
  }

  public int GetTrayPositionOfAbility(Card abilityCard)
  {
    int entityId = abilityCard.GetEntity().GetEntityId();
    for (int index = 0; index < this.m_abilityCards.Count; ++index)
    {
      if (entityId == this.m_abilityCards[index].GetEntity().GetEntityId())
        return index;
    }
    return -1;
  }

  public float GetAbilityPreviewScaleForCurrentPlatform() => this.m_abilityPreviewScale.GetValueForScreen(PlatformSettings.Screen, (object) 1f);

  [Serializable]
  public class AbilityTrayBackgroundMapping
  {
    [SerializeField]
    public TAG_ROLE m_role;
    [SerializeField]
    public GameObject m_background;
  }

  [Serializable]
  public class AbilityCoverMapping
  {
    [SerializeField]
    public TAG_ROLE m_role;
    [SerializeField]
    public List<GameObject> m_covers;
  }

  [Serializable]
  public class AbilityBoneMapping
  {
    [SerializeField]
    public Transform m_abilityBone;
    [SerializeField]
    public Transform m_shownPosition;
    [SerializeField]
    public Transform m_socketedPosition;
  }
}
