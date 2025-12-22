using System.Collections;
using UnityEngine;

[RequireComponent(typeof (Actor))]
[RequireComponent(typeof (Spell))]
public class SideQuestController : MonoBehaviour
{
  public UberText m_ProgressText;
  public float m_ProgressUpdateDelay = 1f;
  public ParticleSystem m_ProgressUpdateParticles;
  private Spell m_spell;
  private Actor m_actor;
  private Entity m_entity;
  private int m_currentQuestProgress;
  private int m_questProgressTotal;
  private int m_targetQuestProgress;
  private bool m_isScalingDown;

  private void Awake()
  {
    this.m_currentQuestProgress = 0;
    this.m_targetQuestProgress = 0;
    this.m_isScalingDown = false;
    this.m_spell = this.GetComponent<Spell>();
    if ((Object) this.m_spell == (Object) null)
      Log.Gameplay.PrintError("SideQuestController.Awake(): GameObject {0} does not have a Spell Component!", (object) this.gameObject.name);
    this.m_spell.AddSpellEventCallback(new Spell.SpellEventCallback(this.OnSpellEvent));
    this.m_actor = this.GetComponent<Actor>();
    if (!((Object) this.m_actor == (Object) null))
      return;
    Log.Gameplay.PrintError("SideQuestController.Awake(): GameObject {0} does not have an Actor Component!", (object) this.gameObject.name);
  }

  public void UpdateQuestUI(bool allowQuestComplete) => this.StartCoroutine(this.UpdateQuestUIImpl(allowQuestComplete));

  private IEnumerator UpdateQuestUIImpl(bool allowQuestComplete)
  {
    Entity entity = this.GetEntity();
    if (entity != null)
    {
      int num = Mathf.Min(entity.GetTag(GAME_TAG.QUEST_PROGRESS), this.m_questProgressTotal);
      if (num != this.m_targetQuestProgress && (allowQuestComplete || num < this.m_questProgressTotal))
      {
        this.m_targetQuestProgress = num;
        GameState.Get().SetBusy(true);
        while (this.m_isScalingDown)
          yield return (object) null;
        if (!this.m_spell.IsActive())
        {
          this.UpdateProgressText(this.m_targetQuestProgress);
          this.m_spell.ActivateState(SpellStateType.ACTION);
        }
      }
    }
  }

  private void OnSpellEvent(string eventName, object eventData, object userData)
  {
    if (eventName == "ScaledUp")
    {
      this.StartCoroutine(this.UpdateQuestProgress());
    }
    else
    {
      if (!(eventName == "ScaledDown"))
        return;
      this.m_isScalingDown = false;
      if (this.m_currentQuestProgress >= this.m_questProgressTotal)
      {
        this.CompleteQuest();
      }
      else
      {
        GameState.Get().SetBusy(false);
        this.m_spell.ActivateState(SpellStateType.NONE);
      }
    }
  }

  private IEnumerator UpdateQuestProgress()
  {
    bool done = false;
    while (!done)
    {
      yield return (object) new WaitForSeconds(this.m_ProgressUpdateDelay);
      if (this.m_currentQuestProgress < this.m_targetQuestProgress)
        this.UpdateProgressText(this.m_currentQuestProgress + 1);
      else
        done = true;
    }
    this.m_isScalingDown = true;
    this.m_spell.GetComponent<PlayMakerFSM>().SendEvent("ScaleDown");
  }

  private void UpdateProgressText(int currentProgress)
  {
    this.m_currentQuestProgress = currentProgress;
    this.m_ProgressUpdateParticles.Stop();
    this.m_ProgressUpdateParticles.Play();
    this.m_ProgressText.Text = string.Format("{0}/{1}", (object) this.m_currentQuestProgress, (object) this.m_questProgressTotal);
  }

  private void CompleteQuest()
  {
    GameState.Get().SetBusy(false);
    Card card = this.m_entity.GetCard();
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      ZoneSecret zone = card.GetZone() as ZoneSecret;
      if ((Object) zone != (Object) null && zone.GetSideQuestCount() == 1)
        card.HideCard();
    }
    else
      card.HideCard();
    this.m_spell.ActivateState(SpellStateType.DEATH);
  }

  private Entity GetEntity()
  {
    if (this.m_entity == null)
    {
      this.m_entity = this.m_actor.GetEntity();
      if (this.m_entity != null)
      {
        this.m_currentQuestProgress = this.m_entity.GetTag(GAME_TAG.QUEST_PROGRESS);
        this.m_questProgressTotal = this.m_entity.GetTag(GAME_TAG.QUEST_PROGRESS_TOTAL);
      }
    }
    return this.m_entity;
  }
}
