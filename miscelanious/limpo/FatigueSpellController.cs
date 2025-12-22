using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatigueSpellController : SpellController
{
  private const float FATIGUE_DRAW_ANIM_TIME = 1.2f;
  private const float FATIGUE_DRAW_SCALE_TIME = 1f;
  private static readonly Vector3 FATIGUE_ACTOR_START_SCALE = new Vector3(0.88f, 0.88f, 0.88f);
  private static readonly Vector3 FATIGUE_ACTOR_FINAL_SCALE = Vector3.one;
  private static readonly Vector3 FATIGUE_ACTOR_INITIAL_LOCAL_ROTATION = new Vector3(270f, 270f, 0.0f);
  private static readonly Vector3 FATIGUE_ACTOR_FINAL_LOCAL_ROTATION = Vector3.zero;
  private const float FATIGUE_HOLD_TIME = 0.8f;
  private Network.HistTagChange m_fatigueTagChange;
  private Actor m_fatigueActor;

  protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    if (!this.HasSourceCard(taskList))
      return false;
    this.m_fatigueTagChange = (Network.HistTagChange) null;
    List<PowerTask> taskList1 = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      Network.PowerHistory power = taskList1[index].GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = (Network.HistTagChange) power;
        if (histTagChange.Tag == 22)
          this.m_fatigueTagChange = histTagChange;
      }
    }
    if (this.m_fatigueTagChange == null)
      return false;
    this.SetSource(taskList.GetSourceEntity().GetCard());
    return true;
  }

  protected override void OnProcessTaskList() => AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hand_Fatigue.prefab:ae394ca0bb29a964eb4c7eeb555f2fae", new PrefabCallback<GameObject>(this.OnFatigueActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);

  private void OnFatigueActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
    {
      Debug.LogWarning((object) string.Format("FatigueSpellController.OnFatigueActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
      this.DoFinishFatigue();
    }
    else
    {
      Actor component = go.GetComponent<Actor>();
      if ((Object) component == (Object) null)
      {
        Debug.LogWarning((object) string.Format("FatigueSpellController.OnFatigueActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
        this.DoFinishFatigue();
      }
      else
      {
        Player controller = this.GetSource().GetController();
        Player.Side controllerSide = controller.GetControllerSide();
        bool flag = controllerSide == Player.Side.FRIENDLY;
        this.m_fatigueActor = component;
        UberText nameText = this.m_fatigueActor.GetNameText();
        if ((Object) nameText != (Object) null)
          nameText.Text = GameStrings.Get("GAMEPLAY_FATIGUE_TITLE");
        int num = this.m_fatigueTagChange.Value;
        if (controller.HasTag(GAME_TAG.DOUBLE_FATIGUE_DAMAGE))
          num *= (int) Mathf.Pow(2f, (float) controller.GetTag(GAME_TAG.DOUBLE_FATIGUE_DAMAGE));
        UberText powersText = this.m_fatigueActor.GetPowersText();
        if ((Object) powersText != (Object) null)
          powersText.Text = GameStrings.Format("GAMEPLAY_FATIGUE_TEXT", (object) num);
        component.SetCardBackSideOverride(new Player.Side?(controllerSide));
        component.UpdateCardBack();
        ZoneDeck zoneDeck = flag ? GameState.Get().GetFriendlySidePlayer().GetDeckZone() : GameState.Get().GetOpposingSidePlayer().GetDeckZone();
        zoneDeck.DoFatigueGlow();
        this.m_fatigueActor.transform.localEulerAngles = FatigueSpellController.FATIGUE_ACTOR_INITIAL_LOCAL_ROTATION;
        this.m_fatigueActor.transform.localScale = FatigueSpellController.FATIGUE_ACTOR_START_SCALE;
        this.m_fatigueActor.transform.position = zoneDeck.transform.position;
        iTween.MoveTo(this.m_fatigueActor.gameObject, iTween.Hash((object) "path", (object) new Vector3[3]
        {
          this.m_fatigueActor.transform.position,
          new Vector3(this.m_fatigueActor.transform.position.x, this.m_fatigueActor.transform.position.y + 3.6f, this.m_fatigueActor.transform.position.z),
          Board.Get().FindBone("FatigueCardBone").position
        }, (object) "time", (object) 1.2f, (object) "easetype", (object) iTween.EaseType.easeInSineOutExpo));
        iTween.RotateTo(this.m_fatigueActor.gameObject, iTween.Hash((object) "rotation", (object) FatigueSpellController.FATIGUE_ACTOR_FINAL_LOCAL_ROTATION, (object) "time", (object) 1.2f, (object) "delay", (object) 0.15f));
        iTween.ScaleTo(this.m_fatigueActor.gameObject, FatigueSpellController.FATIGUE_ACTOR_FINAL_SCALE, 1f);
        this.StartCoroutine(this.WaitThenFinishFatigue(0.8f));
      }
    }
  }

  private IEnumerator WaitThenFinishFatigue(float seconds)
  {
    yield return (object) new WaitForSeconds(seconds);
    this.DoFinishFatigue();
  }

  private void DoFinishFatigue()
  {
    Spell spell = this.GetSource().GetActor().GetSpell(SpellType.FATIGUE_DEATH);
    spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnFatigueDamageFinished));
    spell.ActivateState(SpellStateType.BIRTH);
  }

  private void OnFatigueDamageFinished(Spell spell, object userData)
  {
    spell.RemoveFinishedCallback(new Spell.FinishedCallback(this.OnFatigueDamageFinished));
    if ((Object) this.m_fatigueActor == (Object) null)
    {
      this.OnFinishedTaskList();
    }
    else
    {
      Spell spell1 = this.m_fatigueActor.GetSpell(SpellType.DEATH);
      if ((Object) spell1 == (Object) null)
      {
        this.OnFinishedTaskList();
      }
      else
      {
        Actor fatigueActor = this.m_fatigueActor;
        this.m_fatigueActor = (Actor) null;
        spell1.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnFatigueDeathSpellFinished), (object) fatigueActor);
        spell1.Activate();
        this.OnFinishedTaskList();
      }
    }
  }

  private void OnFatigueDeathSpellFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    Actor actor = (Actor) userData;
    if ((Object) actor != (Object) null)
      actor.Destroy();
    this.OnFinished();
  }
}
