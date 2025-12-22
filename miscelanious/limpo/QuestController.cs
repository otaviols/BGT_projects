using System.Collections;
using UnityEngine;

[RequireComponent(typeof (Spell))]
[RequireComponent(typeof (Actor))]
public class QuestController : MonoBehaviour
{
  public UberText m_ProgressText;
  public NestedPrefab m_QuestProgressUIContainer;
  public string m_QuestUIBoneName = "QuestUI";
  public float m_ProgressUpdateDelay = 1f;
  [Tooltip("When incrementing quest progress if this is set it will set the text to the total progress. If the is not set the text increments one by one.")]
  public bool m_FullTextProgressOnUpdate;
  public ParticleSystem m_ProgressUpdateParticles;
  private Spell m_spell;
  private Actor m_actor;
  private Entity m_entity;
  private QuestProgressUI m_questProgressUI;
  private bool m_questCompleted;
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
      Log.Gameplay.PrintError("QuestController.Awake(): GameObject {0} does not have a Spell Component!", (object) this.gameObject.name);
    this.m_spell.AddSpellEventCallback(new Spell.SpellEventCallback(this.OnSpellEvent));
    this.m_actor = this.GetComponent<Actor>();
    if (!((Object) this.m_actor == (Object) null))
      return;
    Log.Gameplay.PrintError("QuestController.Awake(): GameObject {0} does not have an Actor Component!", (object) this.gameObject.name);
  }

  private void Start()
  {
    this.m_questProgressUI = this.m_QuestProgressUIContainer.PrefabGameObject(true).GetComponent<QuestProgressUI>();
    this.m_questProgressUI.SetOriginalQuestActor(this.m_actor);
    this.m_questProgressUI.Hide();
    this.m_questProgressUI.transform.parent = Board.Get().FindBone(this.m_QuestUIBoneName);
    TransformUtil.Identity((Component) this.m_questProgressUI);
  }

  public static string GetRewardCardIDFromQuestCardID(Entity ent)
  {
    int dbId = 53649;
    if (ent != null && ent.HasTag(GAME_TAG.QUEST_REWARD_DATABASE_ID))
      dbId = ent.GetTag(GAME_TAG.QUEST_REWARD_DATABASE_ID);
    return GameUtils.TranslateDbIdToCardId(dbId);
  }

  public void NotifyMousedOver()
  {
    if (this.m_questCompleted)
      return;
    this.StopCoroutine("WaitThenShowQuestUI");
    this.StartCoroutine("WaitThenShowQuestUI");
  }

  public void NotifyMousedOut()
  {
    this.StopCoroutine("WaitThenShowQuestUI");
    this.m_questProgressUI.Hide();
  }

  private IEnumerator WaitThenShowQuestUI()
  {
    yield return (object) new WaitForSeconds(InputManager.Get().m_MouseOverDelay);
    if (this.GetEntity() != null)
    {
      this.m_questProgressUI.UpdateText(this.m_currentQuestProgress, this.m_questProgressTotal);
      this.m_questProgressUI.Show();
    }
  }

  public void UpdateQuestUI() => this.StartCoroutine(this.UpdateQuestUIImpl());

  private IEnumerator UpdateQuestUIImpl()
  {
    Entity entity = this.GetEntity();
    if (entity != null)
    {
      int num = Mathf.Min(entity.GetTag(GAME_TAG.QUEST_PROGRESS), this.m_questProgressTotal);
      if (num != this.m_targetQuestProgress)
      {
        this.m_targetQuestProgress = num;
        GameState.Get().SetBusy(true);
        while (this.m_isScalingDown)
          yield return (object) null;
        if (this.m_targetQuestProgress < this.m_questProgressTotal)
          GameState.Get().SetBusy(false);
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
        this.CompleteQuest();
      else
        this.m_spell.ActivateState(SpellStateType.NONE);
    }
  }

  private IEnumerator UpdateQuestProgress()
  {
    bool done = false;
    while (!done)
    {
      yield return (object) new WaitForSeconds(this.m_ProgressUpdateDelay);
      if (this.m_currentQuestProgress < this.m_targetQuestProgress)
      {
        if (this.m_FullTextProgressOnUpdate)
          this.UpdateProgressText(this.m_targetQuestProgress);
        else
          this.UpdateProgressText(this.m_currentQuestProgress + 1);
      }
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
    this.m_questProgressUI.UpdateText(this.m_currentQuestProgress, this.m_questProgressTotal);
  }

  private void CompleteQuest()
  {
    GameState.Get().SetBusy(false);
    this.m_questCompleted = true;
    this.m_questProgressUI.Hide();
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
