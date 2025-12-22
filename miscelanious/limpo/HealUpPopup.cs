using Hearthstone.Login;
using System;

public class HealUpPopup : IDisposable
{
  private HealUpPopup.HealUpPopupCompletedTutorial m_tutorialCompleted;
  private Action m_onClosed;

  public bool IsHealUpPopupQueuedForDisplay { get; private set; }

  public bool ShowQueuedPopupAfterTutorial(Action onClosed = null)
  {
    if (onClosed != null)
      this.m_onClosed += onClosed;
    this.IsHealUpPopupQueuedForDisplay = false;
    bool skipDialog = CreateSkipHelper.ShowCreateSkipDialog(new Action(this.OnClosed));
    this.SetOptionEnumHealUpPopup(this.m_tutorialCompleted, skipDialog);
    return skipDialog;
  }

  public void Dispose()
  {
    this.IsHealUpPopupQueuedForDisplay = false;
    this.m_onClosed = (Action) null;
  }

  public void QueuePopupAterTutorialIfNotSeen(
    HealUpPopup.HealUpPopupCompletedTutorial tutorial,
    Action onClosed = null)
  {
    if (!CreateSkipHelper.IsCreateSkipScreenSupported() || this.HasSeenHealUpPopupForTutorial(tutorial) || !TemporaryAccountManager.IsTemporaryAccount())
      return;
    if (onClosed != null)
      this.m_onClosed += onClosed;
    this.m_tutorialCompleted = tutorial;
    this.IsHealUpPopupQueuedForDisplay = true;
  }

  public bool HasSeenHealUpPopupForTutorial(HealUpPopup.HealUpPopupCompletedTutorial tutorial) => Options.Get().GetBool(this.GetOptionEnumHealupPopup(tutorial));

  private void SetOptionEnumHealUpPopup(
    HealUpPopup.HealUpPopupCompletedTutorial tutorial,
    bool hasSeen)
  {
    Options.Get().SetBool(this.GetOptionEnumHealupPopup(tutorial), hasSeen);
  }

  private Option GetOptionEnumHealupPopup(HealUpPopup.HealUpPopupCompletedTutorial tutorial)
  {
    Option optionEnumHealupPopup = Option.INVALID;
    switch (tutorial)
    {
      case HealUpPopup.HealUpPopupCompletedTutorial.Traditional:
        optionEnumHealupPopup = Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_TRADITIONAL;
        break;
      case HealUpPopup.HealUpPopupCompletedTutorial.Battlegrounds:
        optionEnumHealupPopup = Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_BATTLEGROUNDS;
        break;
      case HealUpPopup.HealUpPopupCompletedTutorial.Mercenaries:
        optionEnumHealupPopup = Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_MERCENARIES;
        break;
    }
    return optionEnumHealupPopup;
  }

  private void OnClosed()
  {
    Action onClosed = this.m_onClosed;
    if (onClosed != null)
      onClosed();
    this.m_onClosed = (Action) null;
  }

  public enum HealUpPopupCompletedTutorial
  {
    Traditional,
    Battlegrounds,
    Mercenaries,
  }
}
