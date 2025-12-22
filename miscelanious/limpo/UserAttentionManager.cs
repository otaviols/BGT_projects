using PegasusShared;
using System;
using System.Linq;

public static class UserAttentionManager
{
  private static UserAttentionBlocker s_blockedReasons;

  private static bool IsBlocked => UserAttentionManager.s_blockedReasons != 0;

  public static bool IsBlockedBy(UserAttentionBlocker attentionCategory) => attentionCategory != UserAttentionBlocker.NONE && (UserAttentionManager.s_blockedReasons & attentionCategory) == attentionCategory;

  public static bool CanShowAttentionGrabber(string callerName) => UserAttentionManager.CanShowAttentionGrabber(UserAttentionBlocker.NONE, callerName);

  public static bool CanShowAttentionGrabber(
    UserAttentionBlocker attentionCategory,
    string callerName)
  {
    return (UserAttentionManager.s_blockedReasons & ~attentionCategory) == 0;
  }

  public static void StartBlocking(UserAttentionBlocker attentionCategory)
  {
    if (UserAttentionManager.IsBlockedBy(attentionCategory))
      return;
    bool isBlocked = UserAttentionManager.IsBlocked;
    if (isBlocked)
      Error.AddDevFatal("UserAttentionBlocker.{0} already active, cannot StartBlocking {1}", (object) UserAttentionManager.DumpUserAttentionBlockers(nameof (StartBlocking)), (object) attentionCategory);
    UserAttentionManager.s_blockedReasons |= attentionCategory;
    UserAttentionManager.DumpUserAttentionBlockers("StartBlocking[" + (object) attentionCategory + "]");
    // ISSUE: reference to a compiler-generated field
    if (isBlocked || UserAttentionManager.OnBlockingStart == null)
      return;
    // ISSUE: reference to a compiler-generated field
    UserAttentionManager.OnBlockingStart(attentionCategory);
  }

  public static void StopBlocking(UserAttentionBlocker attentionCategory)
  {
    int num = UserAttentionManager.IsBlocked ? 1 : 0;
    UserAttentionManager.s_blockedReasons &= ~attentionCategory;
    if (num == 0)
      return;
    if (UserAttentionManager.s_blockedReasons == UserAttentionBlocker.NONE)
    {
      Log.UserAttention.Print("UserAttentionManager.StopBlocking[{0}] - all blockers cleared.", (object) attentionCategory);
      // ISSUE: reference to a compiler-generated field
      if (UserAttentionManager.OnBlockingEnd == null)
        return;
      // ISSUE: reference to a compiler-generated field
      UserAttentionManager.OnBlockingEnd();
    }
    else
      Log.UserAttention.Print("UserAttentionManager.StopBlocking[{0}]", (object) attentionCategory);
  }

  public static AvailabilityBlockerReasons GetAvailabilityBlockerReason(
    bool isFriendlyChallenge)
  {
    if (SpectatorManager.Get().IsInSpectatorMode())
      return AvailabilityBlockerReasons.SPECTATING_GAME;
    if (GameMgr.Get().IsFindingGame())
    {
      int num = GameMgr.Get().GetNextGameType() == GameType.GT_RANKED ? 1 : 0;
      FormatType nextFormatType = GameMgr.Get().GetNextFormatType();
      bool flag = nextFormatType == FormatType.FT_UNKNOWN ? RankMgr.Get().IsLegendRank(FormatType.FT_STANDARD) : RankMgr.Get().IsLegendRank(nextFormatType);
      if (num == 0 || !flag || !isFriendlyChallenge)
        return AvailabilityBlockerReasons.FINDING_GAME;
    }
    if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
      return AvailabilityBlockerReasons.HAS_FATAL_ERROR;
    if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.LOGIN))
      return AvailabilityBlockerReasons.LOGGING_IN;
    if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.STARTUP))
      return AvailabilityBlockerReasons.STARTING_UP;
    if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.GAMEPLAY))
    {
      if (GameMgr.Get().IsSpectator() || GameMgr.Get().IsNextSpectator())
        return AvailabilityBlockerReasons.SPECTATING_GAME;
      return GameMgr.Get().IsAI() || GameMgr.Get().IsNextAI() ? AvailabilityBlockerReasons.PLAYING_AI_GAME : AvailabilityBlockerReasons.PLAYING_NON_AI_GAME;
    }
    if (!GameUtils.IsAnyTutorialComplete())
      return AvailabilityBlockerReasons.TUTORIALS_INCOMPLETE;
    if (ShownUIMgr.Get().GetShownUI() == ShownUIMgr.UI_WINDOW.GENERAL_STORE || ShownUIMgr.Get().GetShownUI() == ShownUIMgr.UI_WINDOW.ARENA_STORE)
      return AvailabilityBlockerReasons.STORE_IS_SHOWN;
    if ((UnityEngine.Object) TavernBrawlDisplay.Get() != (UnityEngine.Object) null && TavernBrawlDisplay.Get().IsInDeckEditMode())
      return AvailabilityBlockerReasons.EDITING_TAVERN_BRAWL;
    if (CollectionManager.Get() != null && CollectionManager.Get().IsInEditMode())
      return AvailabilityBlockerReasons.EDITING_DECK;
    if ((UnityEngine.Object) NarrativeManager.Get() != (UnityEngine.Object) null && NarrativeManager.Get().IsShowingBlockingDialog())
      return AvailabilityBlockerReasons.IN_BLOCKING_NARRATIVE_DIALOG;
    if (SetRotationManager.Get().ShouldShowSetRotationIntro())
      return AvailabilityBlockerReasons.SHOULD_BE_SHOWING_SET_ROTATION;
    if (PopupDisplayManager.Get().IsShowing && !isFriendlyChallenge)
      return AvailabilityBlockerReasons.POPUP_SHOWING;
    return (UnityEngine.Object) DraftDisplay.Get() != (UnityEngine.Object) null && DraftDisplay.Get().GetDraftMode() == DraftDisplay.DraftMode.IN_REWARDS && !isFriendlyChallenge ? AvailabilityBlockerReasons.DRAFT_REWARDS_SHOWING : AvailabilityBlockerReasons.NONE;
  }

  private static string CurrentActiveBlockersString => string.Join(", ", Enum.GetValues(typeof (UserAttentionBlocker)).Cast<UserAttentionBlocker>().Where<UserAttentionBlocker>((Func<UserAttentionBlocker, bool>) (r => UserAttentionManager.IsBlockedBy(r))).Select<UserAttentionBlocker, string>((Func<UserAttentionBlocker, string>) (r => r.ToString())).ToArray<string>());

  public static string DumpUserAttentionBlockers(string callerName)
  {
    string activeBlockersString = UserAttentionManager.CurrentActiveBlockersString;
    Log.UserAttention.Print("UserAttentionManager:{0} current blockers: {1}", (object) callerName, (object) activeBlockersString);
    return activeBlockersString;
  }
}
