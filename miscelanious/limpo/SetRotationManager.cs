using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRotationManager : IService
{
  private bool? m_currentSetRotationActive;

  public SpecialEventType CurrentSetRotationEvent { get; private set; }

  public bool IsShowingSetRotationRelogPopup { get; private set; }

  public int CurrentSetRotationYear => SpecialEventManager.Get().GetEventStartTimeUtc(this.CurrentSetRotationEvent).GetValueOrDefault().Year;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    SetRotationManager setRotationManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    serviceLocator.Get<SpecialEventManager>().OnReceivedEventTimingsFromServer += new SpecialEventManager.OnReceivedEventTimingsFromServerDelegate(setRotationManager.OnReceivedEventTimingsFromServer);
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (SpecialEventManager),
    typeof (ReturningPlayerMgr)
  };

  public void Shutdown()
  {
  }

  public static SetRotationManager Get() => ServiceManager.Get<SetRotationManager>();

  public static bool HasSeenStandardModeTutorial() => Options.Get().GetBool(Option.HAS_SEEN_STANDARD_MODE_TUTORIAL, false);

  public bool ShowNewPlayerSetRotationPopupIfNeeded()
  {
    if (Options.Get().GetInt(Option.SET_ROTATION_INTRO_PROGRESS_NEW_PLAYER, 0) >= this.CurrentSetRotationYear || !RankMgr.Get().IsNewPlayer() || !this.IsThisYearsSetRotationEventActive() || !CollectionManager.Get().AccountHasRotatedBoosters(DateTime.UtcNow) && !CollectionManager.Get().AccountHasWildCards())
      return false;
    DialogManager.Get().ShowBasicPopup(UserAttentionBlocker.NONE, new BasicPopup.PopupInfo()
    {
      m_prefabAssetRefs = {
        "SetRotationNewPlayerPopup.prefab:ed707c931e185924eab67aa36770f8ec"
      },
      m_blurWhenShown = true,
      m_responseCallback = (BasicPopup.ResponseCallback) ((response, userData) => this.SetRotationIntroProgress())
    });
    return true;
  }

  public bool ShouldShowSetRotationIntro()
  {
    if (ReturningPlayerMgr.Get().IsInReturningPlayerMode || !this.IsThisYearsSetRotationEventActive() || this.IsShowingSetRotationRelogPopup || Options.Get().GetInt(Option.SET_ROTATION_INTRO_PROGRESS, 0) == this.CurrentSetRotationYear && SetRotationManager.HasSeenStandardModeTutorial())
      return false;
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager == null)
    {
      Debug.LogError((object) "ShouldShowSetRotationIntro: CollectionManager is NULL!");
      return false;
    }
    return collectionManager.ShouldAccountSeeStandardWild() && !this.Cheat_AutoCompleteSetRotationIntro();
  }

  public bool CheckForSetRotationRollover()
  {
    if (!this.m_currentSetRotationActive.HasValue || this.m_currentSetRotationActive.Value || SceneMgr.Get() == null || SceneMgr.Get().IsInGame() || !this.IsThisYearsSetRotationEventActive())
      return false;
    GameMgr service;
    if (ServiceManager.TryGet<GameMgr>(out service) && service.IsFindingGame())
      GameMgr.Get().CancelFindGame();
    Log.All.Print("Set Rotation has just occurred!  Forcing the client to restart.");
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_SET_ROTATION_ROLLOVER_HEADER"),
      m_text = GameStrings.Get((bool) HearthstoneApplication.AllowResetFromFatalError ? "GLOBAL_SET_ROTATION_ROLLOVER_BODY_MOBILE" : "GLOBAL_SET_ROTATION_ROLLOVER_BODY_DESKTOP"),
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_showAlertIcon = true,
      m_disableBnetBar = true,
      m_blurWhenShown = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if ((bool) HearthstoneApplication.AllowResetFromFatalError)
          HearthstoneApplication.Get().Reset();
        else
          HearthstoneApplication.Get().Exit();
      })
    });
    this.IsShowingSetRotationRelogPopup = true;
    this.m_currentSetRotationActive = new bool?(true);
    return true;
  }

  public void SetRotationIntroProgress()
  {
    Options.Get().SetInt(Option.SET_ROTATION_INTRO_PROGRESS, this.CurrentSetRotationYear);
    Options.Get().SetInt(Option.SET_ROTATION_INTRO_PROGRESS_NEW_PLAYER, this.CurrentSetRotationYear);
  }

  public int GetActiveSetRotationYear() => !SpecialEventManager.Get().IsEventActive(this.CurrentSetRotationEvent, false) ? this.CurrentSetRotationYear - 1 : this.CurrentSetRotationYear;

  public string GetActiveSetRotationYearLocalizedString() => this.GetActiveSetRotationYear() % 2 != 0 ? GameStrings.Get("GLUE_SET_ROTATION_ODD_YEAR") : GameStrings.Get("GLUE_SET_ROTATION_EVEN_YEAR");

  private IEnumerator PollForSetRotationRollover(float interval)
  {
    while (!this.CheckForSetRotationRollover())
      yield return (object) new WaitForSeconds(interval);
  }

  private void OnThisYearsSetRotationEventAdded(object userData)
  {
    this.m_currentSetRotationActive = new bool?(this.IsThisYearsSetRotationEventActive());
    if (this.m_currentSetRotationActive.Value)
      return;
    Processor.RunCoroutine(this.PollForSetRotationRollover(1f));
  }

  private void FindCurrentSetRotationEvent()
  {
    SpecialEventType? nullable1 = new SpecialEventType?();
    DateTime? nullable2 = new DateTime?();
    SpecialEventType? nullable3 = new SpecialEventType?();
    DateTime? nullable4 = new DateTime?();
    foreach (CardSetDbfRecord record in GameDbf.CardSet.GetRecords())
    {
      SpecialEventType contentLaunchEvent = record.ContentLaunchEvent;
      DateTime? eventStartTimeUtc = SpecialEventManager.Get().GetEventStartTimeUtc(contentLaunchEvent);
      DateTime now;
      DateTime? nullable5;
      if (eventStartTimeUtc.HasValue)
      {
        now = eventStartTimeUtc.Value;
        int year1 = now.Year;
        now = DateTime.Now;
        int year2 = now.Year;
        if (year1 == year2)
        {
          if (nullable2.HasValue)
          {
            nullable5 = eventStartTimeUtc;
            now = nullable2.Value;
            if ((nullable5.HasValue ? (nullable5.GetValueOrDefault() < now ? 1 : 0) : 0) == 0)
              goto label_7;
          }
          nullable2 = eventStartTimeUtc;
          nullable1 = new SpecialEventType?(contentLaunchEvent);
        }
      }
label_7:
      if (eventStartTimeUtc.HasValue)
      {
        now = eventStartTimeUtc.Value;
        int year = now.Year;
        now = DateTime.Now;
        int num = now.Year - 1;
        if (year == num)
        {
          if (nullable4.HasValue)
          {
            nullable5 = eventStartTimeUtc;
            now = nullable4.Value;
            if ((nullable5.HasValue ? (nullable5.GetValueOrDefault() > now ? 1 : 0) : 0) == 0)
              continue;
          }
          nullable4 = eventStartTimeUtc;
          nullable3 = new SpecialEventType?(contentLaunchEvent);
        }
      }
    }
    if (nullable1.HasValue)
      this.CurrentSetRotationEvent = nullable1.Value;
    else if (nullable3.HasValue)
      this.CurrentSetRotationEvent = nullable3.Value;
    else
      Debug.LogWarning((object) "Unable to find either first content launch event of year, or, latest content launch event");
  }

  private void OnReceivedEventTimingsFromServer()
  {
    this.FindCurrentSetRotationEvent();
    SpecialEventManager.Get().AddEventAddedListener(new SpecialEventManager.EventAddedCallback(this.OnThisYearsSetRotationEventAdded), this.CurrentSetRotationEvent);
  }

  private bool IsThisYearsSetRotationEventActive() => SpecialEventManager.Get().IsEventActive(this.CurrentSetRotationEvent, false);

  public bool Cheat_AutoCompleteSetRotationIntro()
  {
    if (!HearthstoneApplication.IsInternal() || !Options.Get().GetBool(Option.DISABLE_SET_ROTATION_INTRO, false))
      return false;
    this.SetRotationIntroProgress();
    Options.Get().SetBool(Option.HAS_SEEN_STANDARD_MODE_TUTORIAL, true);
    string message = "Set Rotation intro skipped due to disableSetRotationIntro=true";
    UIStatus.Get().AddInfo(message, 10f);
    return true;
  }
}
