using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class EnterTheColiseumSpell : Spell
{
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_SpellStartSoundPrefab;
  public float m_survivorLiftHeight = 2f;
  public float m_LiftTime = 0.5f;
  public float m_LiftOffset = 0.1f;
  public float m_DestroyMinionDelay = 0.5f;
  public float m_LowerDelay = 1.5f;
  public float m_LowerOffset = 0.05f;
  public float m_LowerTime = 0.7f;
  public float m_LightingFadeTime = 0.5f;
  public float m_CameraShakeMagnitude = 0.075f;
  public iTween.EaseType m_liftEaseType = iTween.EaseType.easeInQuart;
  public iTween.EaseType m_lowerEaseType = iTween.EaseType.easeOutCubic;
  public iTween.EaseType m_lightFadeEaseType = iTween.EaseType.easeOutCubic;
  public Spell m_survivorSpellPrefab;
  public Spell m_DustSpellPrefab;
  public bool m_survivorsMeetInMiddle = true;
  public Spell m_ImpactSpellPrefab;
  public string m_RaiseSoundName;
  private List<Card> m_survivorCards;
  private bool m_effectsPlaying;
  private int m_numSurvivorSpellsPlaying;
  private ScreenEffectsHandle m_screenEffectsHandle;

  protected override void Awake()
  {
    base.Awake();
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.m_survivorCards = this.FindSurvivors();
    this.StartCoroutine(this.PerformActions());
  }

  private IEnumerator PerformActions()
  {
    EnterTheColiseumSpell theColiseumSpell = this;
    theColiseumSpell.m_effectsPlaying = true;
    foreach (Card survivorCard in theColiseumSpell.m_survivorCards)
    {
      if (!((UnityEngine.Object) survivorCard == (UnityEngine.Object) null))
      {
        survivorCard.SetDoNotSort(true);
        survivorCard.GetActor().SetUnlit();
        theColiseumSpell.LiftCard(survivorCard);
        yield return (object) new WaitForSeconds(theColiseumSpell.m_LiftOffset);
      }
    }
    ScreenEffectParameters parameters = new ScreenEffectParameters(ScreenEffectType.VIGNETTE, time: theColiseumSpell.m_LightingFadeTime, easeType: theColiseumSpell.m_lightFadeEaseType, vignette: new VignetteParameters?(new VignetteParameters(1f)));
    theColiseumSpell.m_screenEffectsHandle.StartEffect(parameters);
    if (!string.IsNullOrEmpty(theColiseumSpell.m_SpellStartSoundPrefab))
      SoundManager.Get().LoadAndPlay((AssetReference) theColiseumSpell.m_SpellStartSoundPrefab);
    theColiseumSpell.PlayDustCloudSpell();
    yield return (object) new WaitForSeconds(theColiseumSpell.m_LiftTime);
    foreach (Card survivorCard in theColiseumSpell.m_survivorCards)
    {
      if (!((UnityEngine.Object) survivorCard == (UnityEngine.Object) null))
        theColiseumSpell.PlaySurvivorSpell(survivorCard);
    }
    yield return (object) new WaitForSeconds(theColiseumSpell.m_DestroyMinionDelay);
    theColiseumSpell.OnSpellFinished();
    CameraShakeMgr.Shake(Camera.main, new Vector3(theColiseumSpell.m_CameraShakeMagnitude, theColiseumSpell.m_CameraShakeMagnitude, theColiseumSpell.m_CameraShakeMagnitude), 0.75f);
    yield return (object) new WaitForSeconds(theColiseumSpell.m_LowerDelay);
    while (theColiseumSpell.m_numSurvivorSpellsPlaying > 0)
      yield return (object) null;
    foreach (Card survivorCard in theColiseumSpell.m_survivorCards)
    {
      if (!((UnityEngine.Object) survivorCard == (UnityEngine.Object) null))
      {
        Zone zone = survivorCard.GetZone();
        if (zone is ZonePlay)
        {
          ZonePlay zonePlay = (ZonePlay) zone;
          theColiseumSpell.LowerCard(survivorCard.gameObject, zonePlay.GetCardPosition(survivorCard));
          yield return (object) new WaitForSeconds(theColiseumSpell.m_LowerOffset);
        }
      }
    }
    theColiseumSpell.m_screenEffectsHandle.StopEffect(theColiseumSpell.m_LightingFadeTime, theColiseumSpell.m_lightFadeEaseType);
    if ((UnityEngine.Object) theColiseumSpell.m_ImpactSpellPrefab != (UnityEngine.Object) null)
    {
      foreach (Card survivorCard in theColiseumSpell.m_survivorCards)
      {
        if (!((UnityEngine.Object) survivorCard == (UnityEngine.Object) null))
        {
          Spell spell = SpellManager.Get().GetSpell(theColiseumSpell.m_ImpactSpellPrefab);
          spell.transform.parent = survivorCard.gameObject.transform;
          spell.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
          // ISSUE: reference to a compiler-generated method
          spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(theColiseumSpell.\u003CPerformActions\u003Eb__24_0));
          spell.Activate();
          yield return (object) new WaitForSeconds(theColiseumSpell.m_LowerOffset);
        }
      }
    }
    yield return (object) new WaitForSeconds(theColiseumSpell.m_LowerTime);
    foreach (Card survivorCard in theColiseumSpell.m_survivorCards)
    {
      if (!((UnityEngine.Object) survivorCard == (UnityEngine.Object) null))
      {
        survivorCard.SetDoNotSort(false);
        survivorCard.GetActor().SetLit();
      }
    }
    foreach (Zone zone in ZoneMgr.Get().FindZonesOfType<ZonePlay>())
      zone.UpdateLayout();
    while (theColiseumSpell.m_effectsPlaying)
      yield return (object) null;
    theColiseumSpell.OnStateFinished();
  }

  private void LiftCard(Card card)
  {
    GameObject gameObject = card.gameObject;
    Vector3 position1 = gameObject.transform.position;
    Vector3 position2 = card.GetZone().gameObject.transform.position;
    Hashtable args = iTween.Hash((object) "time", (object) this.m_LiftTime, (object) "position", (object) new Vector3(this.m_survivorsMeetInMiddle ? position2.x : position1.x, position1.y + this.m_survivorLiftHeight, position1.z), (object) "onstart", (object) (Action<object>) (newVal => SoundManager.Get().LoadAndPlay((AssetReference) this.m_RaiseSoundName)), (object) "easetype", (object) this.m_liftEaseType);
    iTween.MoveTo(gameObject, args);
  }

  private void LowerCard(GameObject target, Vector3 finalPosition)
  {
    Hashtable args = iTween.Hash((object) "time", (object) this.m_LowerTime, (object) "position", (object) finalPosition, (object) "easetype", (object) this.m_lowerEaseType);
    iTween.MoveTo(target, args);
  }

  private List<Card> FindSurvivors()
  {
    List<Card> survivors = new List<Card>();
    foreach (GameObject target in this.m_targets)
    {
      Card component = target.GetComponent<Card>();
      bool flag = true;
      foreach (PowerTask task in this.m_taskList.GetTaskList())
      {
        Network.PowerHistory power = task.GetPower();
        if (power.Type == Network.PowerType.TAG_CHANGE)
        {
          Network.HistTagChange histTagChange = power as Network.HistTagChange;
          if (histTagChange.Tag == 360 && histTagChange.Value == 1)
          {
            Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
            if (entity == null)
              Log.Power.PrintWarning(string.Format("{0}.FindSurvivors() - WARNING trying to get entity with id {1} but there is no entity with that id", (object) this, (object) histTagChange.Entity));
            else if ((UnityEngine.Object) component == (UnityEngine.Object) entity.GetCard())
            {
              flag = false;
              break;
            }
          }
        }
      }
      if (flag)
        survivors.Add(component);
    }
    return survivors;
  }

  private void PlaySurvivorSpell(Card card)
  {
    if ((UnityEngine.Object) this.m_survivorSpellPrefab == (UnityEngine.Object) null)
      return;
    ++this.m_numSurvivorSpellsPlaying;
    Spell spell1 = SpellManager.Get().GetSpell(this.m_survivorSpellPrefab);
    spell1.transform.parent = card.GetActor().transform;
    spell1.AddFinishedCallback((Spell.FinishedCallback) ((spell, spellUserData) => --this.m_numSurvivorSpellsPlaying));
    spell1.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
    {
      if (spell.GetActiveState() != SpellStateType.NONE)
        return;
      SpellManager.Get().ReleaseSpell(spell);
    }));
    spell1.SetSource(card.gameObject);
    spell1.Activate();
  }

  private void PlayDustCloudSpell()
  {
    if ((UnityEngine.Object) this.m_DustSpellPrefab == (UnityEngine.Object) null)
      return;
    Spell spell1 = SpellManager.Get().GetSpell(this.m_DustSpellPrefab);
    spell1.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
    {
      if (spell.GetActiveState() != SpellStateType.NONE)
        return;
      SpellManager.Get().ReleaseSpell(spell);
    }));
    spell1.Activate();
  }
}
