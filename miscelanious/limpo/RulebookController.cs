using Hearthstone.UI;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof (Actor))]
public class RulebookController : MonoBehaviour
{
  private const string FriendlyBoneName = "FriendlyChoice";
  private Actor m_actor;
  private Entity m_entity;
  private WidgetInstance m_hoverPopupWidget;

  private void Awake()
  {
    this.m_actor = this.GetComponent<Actor>();
    if (!((Object) this.m_actor == (Object) null))
      return;
    Log.Gameplay.PrintError("RulebookController.Awake(): GameObject {0} does not have an Actor Component!", (object) this.gameObject.name);
  }

  public void OnDestroy() => this.NotifyMousedOut();

  public void NotifyMousedOver()
  {
    this.StopCoroutine("WaitThenShowPopup");
    this.StartCoroutine("WaitThenShowPopup");
  }

  public void NotifyMousedOut()
  {
    this.StopCoroutine("WaitThenShowPopup");
    this.HidePopup();
  }

  private IEnumerator WaitThenShowPopup()
  {
    string widgetName = GameState.Get().GetStringGameOption(GameEntityOption.RULEBOOK_POPUP_PREFAB_PATH);
    if ((Object) this.m_hoverPopupWidget == (Object) null && !string.IsNullOrEmpty(widgetName))
    {
      this.m_hoverPopupWidget = WidgetInstance.Create(widgetName);
      this.m_hoverPopupWidget.transform.position = Vector3.up * 5000f;
      while (!this.m_hoverPopupWidget.IsReady)
        yield return (object) null;
    }
    if ((Object) this.m_hoverPopupWidget == (Object) null)
    {
      Log.Gameplay.PrintError("RulebookIconController.WaitThenShowPopup: Invalid popup path: {0}", (object) widgetName);
    }
    else
    {
      yield return (object) new WaitForSeconds(InputManager.Get().m_MouseOverDelay);
      if (this.GetEntity() != null)
        this.ShowPopup();
    }
  }

  private void ShowPopup()
  {
    this.m_hoverPopupWidget.Show();
    this.m_hoverPopupWidget.transform.localPosition = Board.Get().FindBone("FriendlyChoice").position;
    Spell componentInChildren = this.m_hoverPopupWidget.GetComponentInChildren<Spell>();
    if (!(bool) (Object) componentInChildren)
      return;
    componentInChildren.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnIntroSpellStateFinished));
    componentInChildren.ActivateState(SpellStateType.BIRTH);
  }

  private void HidePopup()
  {
    if ((Object) this.m_hoverPopupWidget == (Object) null)
      return;
    this.m_hoverPopupWidget.Hide();
  }

  private void OnIntroSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    Object.Destroy((Object) spell);
    spell = (Spell) null;
  }

  private Entity GetEntity()
  {
    if (this.m_entity == null)
      this.m_entity = this.m_actor.GetEntity();
    return this.m_entity;
  }
}
