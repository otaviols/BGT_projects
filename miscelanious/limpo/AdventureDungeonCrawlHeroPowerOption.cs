using Hearthstone.UI;
using UnityEngine;

[CustomEditClass]
public class AdventureDungeonCrawlHeroPowerOption : AdventureOptionWidget
{
  [CustomEditField(Sections = "Bones")]
  public GameObject m_BigCardBone;
  private bool m_isDefLoaded;

  public override bool IsReady => base.IsReady && this.m_isDefLoaded;

  protected override void OnWidgetInstanceReady(WidgetInstance widgetInstance)
  {
    base.OnWidgetInstanceReady(widgetInstance);
    if ((Object) this.m_widgetInstance == (Object) null || this.m_databaseId == 0L)
      return;
    this.m_isDefLoaded = false;
    string cardId = GameUtils.TranslateDbIdToCardId((int) this.m_databaseId);
    DefLoader.Get().LoadFullDef(cardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnFullDefLoaded));
  }

  protected override void OnClickableReady(Clickable clickable)
  {
    base.OnClickableReady(clickable);
    if ((Object) this.m_clickable == (Object) null)
      return;
    this.m_clickable.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.Select()));
  }

  protected override void Rollover()
  {
    base.Rollover();
    if (!(this.m_rolloverCallback is AdventureDungeonCrawlHeroPowerOption.HeroPowerHoverOptionCallback rolloverCallback))
    {
      Log.Adventures.PrintError("rollover callback was null or was not a HeroPowerHoverOptionCallback!");
    }
    else
    {
      if (!this.m_dataModel.Locked)
        return;
      rolloverCallback(this.m_databaseId, this.m_BigCardBone);
    }
  }

  protected override void Rollout()
  {
    base.Rollout();
    if (!(this.m_rolloutCallback is AdventureDungeonCrawlHeroPowerOption.HeroPowerHoverOptionCallback rolloutCallback))
    {
      Log.Adventures.PrintError("rollout callback was null or was not a HeroPowerHoverOptionCallback!");
    }
    else
    {
      if (!this.m_dataModel.Locked)
        return;
      rolloutCallback(this.m_databaseId, this.m_BigCardBone);
    }
  }

  private void OnFullDefLoaded(string cardID, DefLoader.DisposableFullDef def, object userData)
  {
    using (def)
    {
      if (def == null)
      {
        Debug.LogErrorFormat("Unable to load FullDef for cardID={0}", (object) cardID);
      }
      else
      {
        this.m_isDefLoaded = true;
        Actor componentInChildren = this.m_widgetInstance.GetComponentInChildren<Actor>(true);
        componentInChildren.SetFullDef(def);
        componentInChildren.UpdateAllComponents();
        if (!(bool) (Object) this.m_widgetInstance)
          return;
        this.m_widgetInstance.TriggerEvent("SetUpState", new Widget.TriggerEventParameters());
      }
    }
  }

  public void Init(
    long heroPowerDbId,
    bool locked,
    string lockedText,
    bool completed,
    bool newlyUnlocked,
    AdventureOptionWidget.OptionAcknowledgedCallback acknowledgedCallback)
  {
    this.m_databaseId = heroPowerDbId;
    this.InitWidget((string) null, locked, lockedText, false, completed, newlyUnlocked, acknowledgedCallback);
    if (!((Object) this.m_widgetInstance != (Object) null))
      return;
    string cardId = GameUtils.TranslateDbIdToCardId((int) this.m_databaseId);
    DefLoader.Get().LoadFullDef(cardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnFullDefLoaded));
  }

  public override void Select()
  {
    base.Select();
    if (this.m_dataModel == null)
      Log.Adventures.PrintError("Attempting to set deck pouch option clickable events but data model was null!");
    else if (!(this.m_selectedCallback is AdventureDungeonCrawlHeroPowerOption.HeroPowerSelectedOptionCallback selectedCallback))
      Log.Adventures.PrintError("Attempting to set a callback for the AdventureDungeonCrawlHeroPowerOption, but no callback was provided!");
    else
      selectedCallback(this.m_databaseId, this.m_dataModel.Locked);
  }

  public override void SetVisible(bool isVisible)
  {
    if (isVisible == this.m_isVisible)
      return;
    base.SetVisible(isVisible);
    Actor componentInChildren = this.m_widgetInstance.GetComponentInChildren<Actor>(true);
    if ((Object) componentInChildren == (Object) null)
      Log.Adventures.PrintError("Tried to set hero power actor visibility but hero power actor was not found!");
    else if (isVisible)
    {
      this.m_widgetInstance.Show();
      componentInChildren.Show();
    }
    else
    {
      this.m_widgetInstance.Hide();
      componentInChildren.Hide();
    }
  }

  public delegate void HeroPowerSelectedOptionCallback(long heroPowerDbId, bool isLocked);

  public delegate void HeroPowerHoverOptionCallback(long heroPowerDbId, GameObject bigCardBone);
}
