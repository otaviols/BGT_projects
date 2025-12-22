using HutongGames.PlayMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureWingProgressDisplay_LOE : AdventureWingProgressDisplay
{
  public UberText m_hangingSignText;
  public PegUIElement m_hangingSignHitArea;
  public PegUIElement m_completeStaffHitArea;
  public List<GameObject> m_emptyStaffObjects = new List<GameObject>();
  public List<GameObject> m_visibleStaffObjects = new List<GameObject>();
  public List<GameObject> m_rodObjects = new List<GameObject>();
  public List<GameObject> m_headObjects = new List<GameObject>();
  public List<GameObject> m_pearlObjects = new List<GameObject>();
  [CustomEditField(Sections = "VO")]
  public string m_hangingSignQuotePrefab;
  [CustomEditField(Sections = "VO")]
  public string m_hangingSignQuoteVOLine;
  [CustomEditField(Sections = "VO")]
  public string m_completeStaffQuotePrefab;
  [CustomEditField(Sections = "VO")]
  public string m_completeStaffQuoteVOLine;
  private const string s_WingDisappearAnimateEventName = "OnWingDisappear";
  private const string s_WingReappearAnimateEventName = "OnWingReappear";
  private const string s_CompleteAnimationVarName = "AnimationComplete";
  private bool m_rodComplete;
  private bool m_headComplete;
  private bool m_pearlComplete;
  private bool m_finalWingComplete;
  private bool m_animating;

  private void Awake()
  {
    AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_emptyStaffObjects, true);
    AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_rodObjects, false);
    AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_headObjects, false);
    AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_pearlObjects, false);
    AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_visibleStaffObjects, false);
    if ((Object) this.m_hangingSignHitArea != (Object) null)
      this.m_hangingSignHitArea.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnHangingSignClick()));
    if (!((Object) this.m_completeStaffHitArea != (Object) null))
      return;
    this.m_completeStaffHitArea.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnCompleteStaffClick()));
  }

  private void Update()
  {
    if (!AdventureScene.Get().IsDevMode)
      return;
    if (InputCollection.GetKeyDown(KeyCode.C))
    {
      this.StartCoroutine(this.PlayCompleteAnimationCoroutine(this.GetComponent<PlayMakerFSM>(), "OnWingDisappear", (AdventureWingProgressDisplay.OnAnimationComplete) null, Option.INVALID));
    }
    else
    {
      if (!InputCollection.GetKeyDown(KeyCode.V))
        return;
      this.StartCoroutine(this.PlayCompleteAnimationCoroutine(this.GetComponent<PlayMakerFSM>(), "OnWingReappear", (AdventureWingProgressDisplay.OnAnimationComplete) null, Option.INVALID));
    }
  }

  public override void UpdateProgress(WingDbId wingDbId, bool linearComplete)
  {
    switch (wingDbId)
    {
      case WingDbId.LOE_TEMPLE_OF_ORSIS:
        this.m_rodComplete = linearComplete;
        break;
      case WingDbId.LOE_ULDAMAN:
        this.m_headComplete = linearComplete;
        break;
      case WingDbId.LOE_RUINED_CITY:
        this.m_pearlComplete = linearComplete;
        break;
      case WingDbId.LOE_HALL_OF_EXPLORERS:
        this.m_finalWingComplete = linearComplete;
        break;
    }
    this.UpdatePartVisibility();
  }

  public override bool HasProgressAnimationToPlay()
  {
    if (!this.m_rodComplete || !this.m_headComplete || !this.m_pearlComplete)
      return false;
    return this.m_finalWingComplete ? !Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_REAPPEAR, false) : !Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_DISAPPEAR, false);
  }

  public override void PlayProgressAnimation(
    AdventureWingProgressDisplay.OnAnimationComplete onAnimComplete = null)
  {
    if (!this.m_rodComplete || !this.m_headComplete || !this.m_pearlComplete)
    {
      if (onAnimComplete == null)
        return;
      onAnimComplete();
    }
    else
    {
      PlayMakerFSM component = this.GetComponent<PlayMakerFSM>();
      if ((Object) component == (Object) null)
      {
        if (onAnimComplete == null)
          return;
        onAnimComplete();
      }
      else if (!this.m_finalWingComplete)
      {
        if (Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_DISAPPEAR, false))
        {
          if (onAnimComplete == null)
            return;
          onAnimComplete();
        }
        else
          this.StartCoroutine(this.PlayCompleteAnimationCoroutine(component, "OnWingDisappear", onAnimComplete, Option.HAS_SEEN_LOE_STAFF_DISAPPEAR));
      }
      else if (Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_REAPPEAR, false))
      {
        if (onAnimComplete == null)
          return;
        onAnimComplete();
      }
      else
        this.StartCoroutine(this.PlayCompleteAnimationCoroutine(component, "OnWingReappear", onAnimComplete, Option.HAS_SEEN_LOE_STAFF_REAPPEAR));
    }
  }

  private void UpdatePartVisibility()
  {
    bool flag1 = Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_DISAPPEAR, false);
    if (this.m_finalWingComplete)
    {
      bool flag2 = Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_REAPPEAR, false);
      AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_emptyStaffObjects, false);
      AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_rodObjects, this.m_rodComplete & flag2);
      AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_headObjects, this.m_headComplete & flag2);
      AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_pearlObjects, this.m_pearlComplete & flag2);
      AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_visibleStaffObjects, true);
    }
    else
    {
      bool show1 = this.m_rodComplete && !flag1;
      bool show2 = this.m_headComplete && !flag1;
      bool show3 = this.m_pearlComplete && !flag1;
      bool show4 = show1 | show2 | show3;
      AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_emptyStaffObjects, !show4);
      AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_rodObjects, show1);
      AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_headObjects, show2);
      AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_pearlObjects, show3);
      AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_visibleStaffObjects, show4);
    }
    if ((Object) this.m_hangingSignText != (Object) null)
      this.m_hangingSignText.Text = flag1 ? GameStrings.Get("GLUE_ADVENTURE_LOE_STAFF_DISAPPEARED") : GameStrings.Get("GLUE_ADVENTURE_LOE_STAFF_RESERVED");
    if ((Object) this.m_completeStaffHitArea != (Object) null)
      this.m_completeStaffHitArea.gameObject.SetActive(this.m_finalWingComplete && this.m_rodComplete && this.m_headComplete && this.m_pearlComplete);
    if (!((Object) this.m_hangingSignHitArea != (Object) null))
      return;
    this.m_hangingSignHitArea.SetEnabled(!this.m_finalWingComplete && !this.m_rodComplete && !this.m_headComplete && !this.m_pearlComplete);
  }

  private static void SetObjectsVisibility(List<GameObject> objs, bool show)
  {
    foreach (GameObject gameObject in objs)
    {
      if ((Object) gameObject != (Object) null)
        gameObject.SetActive(show);
    }
  }

  private IEnumerator PlayCompleteAnimationCoroutine(
    PlayMakerFSM fsm,
    string eventName,
    AdventureWingProgressDisplay.OnAnimationComplete onAnimComplete,
    Option seenOption)
  {
    FsmBool animComplete = fsm.FsmVariables.FindFsmBool("AnimationComplete");
    fsm.SendEvent(eventName);
    this.m_animating = true;
    if (animComplete != null)
    {
      while (!animComplete.Value)
        yield return (object) null;
    }
    this.m_animating = false;
    if (seenOption != Option.INVALID)
      Options.Get().SetBool(seenOption, true);
    if (onAnimComplete != null)
      onAnimComplete();
  }

  private void OnHangingSignClick()
  {
    if (this.m_animating || this.m_rodComplete || this.m_headComplete || this.m_pearlComplete || string.IsNullOrEmpty(this.m_hangingSignQuotePrefab) || string.IsNullOrEmpty(this.m_hangingSignQuoteVOLine))
      return;
    string legacyAssetName = new AssetReference(this.m_hangingSignQuoteVOLine).GetLegacyAssetName();
    NotificationManager.Get().CreateCharacterQuote(this.m_hangingSignQuotePrefab, GameStrings.Get(legacyAssetName), this.m_hangingSignQuoteVOLine);
  }

  private void OnCompleteStaffClick()
  {
    if (this.m_animating || !this.m_rodComplete || !this.m_headComplete || !this.m_pearlComplete || !this.m_finalWingComplete || string.IsNullOrEmpty(this.m_completeStaffQuotePrefab) || string.IsNullOrEmpty(this.m_completeStaffQuoteVOLine))
      return;
    string legacyAssetName = new AssetReference(this.m_completeStaffQuoteVOLine).GetLegacyAssetName();
    NotificationManager.Get().CreateCharacterQuote(this.m_completeStaffQuotePrefab, GameStrings.Get(legacyAssetName), this.m_completeStaffQuoteVOLine);
  }
}
