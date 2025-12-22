using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Animation))]
public class Misdirection : Spell
{
  public float m_ReticleFadeInTime = 0.8f;
  public float m_ReticleFadeOutTime = 0.4f;
  public float m_ReticlePathTime = 3f;
  public float m_ReticleBlur = 0.005f;
  public float m_ReticleBlurFocusTime = 0.8f;
  public Color m_ReticleAttackColor = Color.red;
  public float m_ReticleAttackScale = 1.1f;
  public float m_ReticleAttackTime = 0.3f;
  public Vector3 m_ReticleAttackRotate = new Vector3(0.0f, 90f, 0.0f);
  public float m_ReticleAttackHold = 0.25f;
  public GameObject m_Reticle;
  public bool m_AllowTargetingInitialTarget;
  public int m_ReticlePathDesiredMinimumTargets = 3;
  public int m_ReticlePathDesiredMaximumTargets = 4;
  private GameObject m_ReticleInstance;
  private Material m_ReticleMaterial;
  private Card m_AttackingEntityCard;
  private Card m_InitialTargetCard;
  private Color m_OrgAmbient;

  public override bool AddPowerTargets()
  {
    if (!this.CanAddPowerTargets())
      return false;
    this.AddMultiplePowerTargets();
    return true;
  }

  protected override void OnAction(SpellStateType prevStateType) => this.StartAnimation();

  private void ResolveTargets()
  {
    List<GameObject> targets = this.GetTargets();
    if (targets.Count < 3)
      return;
    this.m_AttackingEntityCard = targets[1].GetComponent<Card>();
    GameState gameState = GameState.Get();
    GameEntity gameEntity = gameState.GetGameEntity();
    Entity entity1 = gameState.GetEntity(gameEntity.GetTag(GAME_TAG.PROPOSED_DEFENDER));
    if (entity1 != null)
    {
      this.m_InitialTargetCard = entity1.GetCard();
    }
    else
    {
      Entity entity2 = gameState.GetEntity(this.m_AttackingEntityCard.GetEntity().GetTag(GAME_TAG.CARD_TARGET));
      if (entity2 != null)
        this.m_InitialTargetCard = entity2.GetCard();
      else
        this.m_InitialTargetCard = targets[2].GetComponent<Card>();
    }
  }

  private void StartAnimation()
  {
    this.ResolveTargets();
    if ((UnityEngine.Object) this.m_InitialTargetCard == (UnityEngine.Object) null)
    {
      this.OnSpellFinished();
    }
    else
    {
      this.m_ReticleInstance = UnityEngine.Object.Instantiate<GameObject>(this.m_Reticle, this.m_InitialTargetCard.transform.position, Quaternion.identity);
      this.m_ReticleMaterial = this.m_ReticleInstance.GetComponentInChildren<MeshRenderer>().GetMaterial();
      this.m_ReticleMaterial.SetFloat("_Alpha", 0.0f);
      this.m_ReticleMaterial.SetFloat("_blur", this.m_ReticleBlur);
      this.StartCoroutine(this.ReticleFadeIn());
      this.StartCoroutine(this.AnimateReticle());
      AudioSource component = this.GetComponent<AudioSource>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      SoundManager.Get().Play(component);
    }
  }

  private IEnumerator ReticleFadeIn()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    Misdirection misdirection = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated method
    Action<object> action = new Action<object>(misdirection.\u003CReticleFadeIn\u003Eb__24_0);
    Hashtable args1 = iTween.Hash((object) "time", (object) misdirection.m_ReticleFadeInTime, (object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "onupdate", (object) action, (object) "onupdatetarget", (object) misdirection.m_ReticleInstance.gameObject);
    iTween.ValueTo(misdirection.m_ReticleInstance.gameObject, args1);
    Hashtable args2 = iTween.Hash((object) "time", (object) misdirection.m_ReticleFadeInTime, (object) "scale", (object) Vector3.one, (object) "easetype", (object) iTween.EaseType.easeOutBounce);
    iTween.ScaleTo(misdirection.m_ReticleInstance.gameObject, args2);
    return false;
  }

  private void SetReticleAlphaValue(float val) => this.m_ReticleMaterial.SetFloat("_Alpha", val);

  private IEnumerator AnimateReticle()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    Misdirection misdirection = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      Hashtable args = iTween.Hash((object) "path", (object) misdirection.BuildAnimationPath(), (object) "time", (object) misdirection.m_ReticlePathTime, (object) "easetype", (object) iTween.EaseType.easeInOutQuad, (object) "oncomplete", (object) "ReticleAnimationComplete", (object) "oncompletetarget", (object) misdirection.gameObject, (object) "orienttopath", (object) false);
      iTween.MoveTo(misdirection.m_ReticleInstance, args);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(misdirection.m_ReticleFadeInTime);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void ReticleAnimationComplete() => this.StartCoroutine(this.ReticleAttackAnimation());

  private IEnumerator ReticleAttackAnimation()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    Misdirection misdirection = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      misdirection.StartCoroutine(misdirection.ReticleFadeOut());
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated method
    Action<object> action1 = new Action<object>(misdirection.\u003CReticleAttackAnimation\u003Eb__28_0);
    Hashtable args1 = iTween.Hash((object) "time", (object) misdirection.m_ReticleAttackTime, (object) "from", (object) misdirection.m_ReticleMaterial.color, (object) "to", (object) misdirection.m_ReticleAttackColor, (object) "onupdate", (object) action1, (object) "onupdatetarget", (object) misdirection.gameObject);
    iTween.ValueTo(misdirection.gameObject, args1);
    Hashtable args2 = iTween.Hash((object) "time", (object) misdirection.m_ReticleAttackTime, (object) "scale", (object) misdirection.m_ReticleAttackScale, (object) "easetype", (object) iTween.EaseType.easeOutBounce);
    iTween.ScaleTo(misdirection.m_ReticleInstance, args2);
    Hashtable args3 = iTween.Hash((object) "time", (object) misdirection.m_ReticleAttackTime, (object) "rotation", (object) misdirection.m_ReticleAttackRotate, (object) "easetype", (object) iTween.EaseType.easeOutBounce);
    iTween.RotateTo(misdirection.m_ReticleInstance, args3);
    // ISSUE: reference to a compiler-generated method
    Action<object> action2 = new Action<object>(misdirection.\u003CReticleAttackAnimation\u003Eb__28_1);
    Hashtable args4 = iTween.Hash((object) "time", (object) misdirection.m_ReticleBlurFocusTime, (object) "from", (object) misdirection.m_ReticleBlur, (object) "to", (object) 0.0f, (object) "onupdate", (object) action2, (object) "onupdatetarget", (object) misdirection.gameObject);
    iTween.ValueTo(misdirection.gameObject, args4);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(misdirection.m_ReticleBlurFocusTime + misdirection.m_ReticleAttackHold);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private IEnumerator ReticleFadeOut()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    Misdirection misdirection = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      UnityEngine.Object.Destroy((UnityEngine.Object) misdirection.m_ReticleInstance);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    misdirection.OnSpellFinished();
    // ISSUE: reference to a compiler-generated method
    Action<object> action = new Action<object>(misdirection.\u003CReticleFadeOut\u003Eb__29_0);
    Hashtable args = iTween.Hash((object) "time", (object) misdirection.m_ReticleFadeOutTime, (object) "from", (object) 1f, (object) "to", (object) 0.0f, (object) "onupdate", (object) action, (object) "onupdatetarget", (object) misdirection.gameObject);
    iTween.ValueTo(misdirection.gameObject, args);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(misdirection.m_ReticleFadeOutTime);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private Vector3[] BuildAnimationPath()
  {
    Card[] possibleTargetCards = this.FindPossibleTargetCards();
    int num = UnityEngine.Random.Range(this.m_ReticlePathDesiredMinimumTargets, this.m_ReticlePathDesiredMaximumTargets);
    if (num >= possibleTargetCards.Length + 2)
      num = possibleTargetCards.Length + 2;
    if (possibleTargetCards.Length <= 1)
      return new Vector3[2]
      {
        this.m_InitialTargetCard.transform.position,
        this.GetTarget().transform.position
      };
    List<Vector3> vector3List = new List<Vector3>();
    vector3List.Add(this.m_InitialTargetCard.transform.position);
    GameObject gameObject1 = this.m_InitialTargetCard.gameObject;
    for (int index = 1; index < num; ++index)
    {
      GameObject gameObject2 = possibleTargetCards[UnityEngine.Random.Range(0, possibleTargetCards.Length - 1)].gameObject;
      if ((UnityEngine.Object) gameObject2 == (UnityEngine.Object) gameObject1)
      {
        gameObject2 = possibleTargetCards[UnityEngine.Random.Range(0, possibleTargetCards.Length - 1)].gameObject;
        if ((UnityEngine.Object) gameObject2 == (UnityEngine.Object) gameObject1)
          gameObject2 = !((UnityEngine.Object) gameObject2 == (UnityEngine.Object) possibleTargetCards[possibleTargetCards.Length - 1]) ? possibleTargetCards[possibleTargetCards.Length - 1].gameObject : possibleTargetCards[0].gameObject;
      }
      if (index == num - 1 && (UnityEngine.Object) gameObject2 == (UnityEngine.Object) this.GetTarget() && (UnityEngine.Object) gameObject2 == (UnityEngine.Object) gameObject1)
        gameObject2 = !((UnityEngine.Object) gameObject2 == (UnityEngine.Object) possibleTargetCards[possibleTargetCards.Length - 1]) ? possibleTargetCards[possibleTargetCards.Length - 1].gameObject : possibleTargetCards[0].gameObject;
      vector3List.Add(gameObject2.transform.position);
    }
    vector3List.Add(this.GetTarget().transform.position);
    return vector3List.ToArray();
  }

  private Card[] FindPossibleTargetCards()
  {
    List<Card> cardList = new List<Card>();
    ZoneMgr zoneMgr = ZoneMgr.Get();
    if ((UnityEngine.Object) zoneMgr == (UnityEngine.Object) null)
      return cardList.ToArray();
    foreach (Zone zone in zoneMgr.FindZonesOfType<ZonePlay>())
    {
      foreach (Card card in zone.GetCards())
      {
        if (!((UnityEngine.Object) card == (UnityEngine.Object) this.m_AttackingEntityCard) && (!((UnityEngine.Object) card == (UnityEngine.Object) this.m_InitialTargetCard) || this.m_AllowTargetingInitialTarget))
          cardList.Add(card);
      }
    }
    foreach (Zone zone in zoneMgr.FindZonesOfType<ZoneHero>())
    {
      foreach (Card card in zone.GetCards())
      {
        if (!((UnityEngine.Object) card == (UnityEngine.Object) this.m_AttackingEntityCard) && (!((UnityEngine.Object) card == (UnityEngine.Object) this.m_InitialTargetCard) || this.m_AllowTargetingInitialTarget))
          cardList.Add(card);
      }
    }
    return cardList.ToArray();
  }

  private Card[] GetOpponentZoneMinions()
  {
    List<Card> cardList = new List<Card>();
    foreach (Card card in GameState.Get().GetFirstOpponentPlayer(this.GetSourceCard().GetController()).GetBattlefieldZone().GetCards())
    {
      if (!((UnityEngine.Object) card == (UnityEngine.Object) this.m_AttackingEntityCard))
        cardList.Add(card);
    }
    return cardList.ToArray();
  }

  private Card GetCurrentPlayerHeroCard() => this.GetSourceCard().GetController().GetHeroCard();

  private Card GetOpponentHeroCard() => GameState.Get().GetFirstOpponentPlayer(this.GetSourceCard().GetController()).GetHeroCard();
}
