using System.Collections;
using UnityEngine;

[RequireComponent(typeof (Actor))]
[RequireComponent(typeof (Spell))]
public class QuestlineController : MonoBehaviour
{
  public UberText m_ProgressText;
  public NestedPrefab m_QuestlineProgressUIContainer;
  public string m_QuestUIBoneName = "QuestUI";
  public float m_ProgressUpdateDelay = 1f;
  public ParticleSystem m_ProgressUpdateParticles;
  private GameState m_gameState;
  private InputManager m_inputManager;
  private Spell m_spell;
  private PlayMakerFSM m_spellFSM;
  private Actor m_actor;
  private Entity m_entity;
  private QuestlineProgressUI m_QuestlineProgressUI;
  private bool m_questCompleted;
  private int m_displayedQuestProgress;
  private int m_actualQuestProgress;
  private int m_questProgressTotal;
  private bool m_isScalingDown;

  private void Awake()
  {
    this.m_displayedQuestProgress = 0;
    this.m_actualQuestProgress = 0;
    this.m_isScalingDown = false;
    this.m_spell = this.GetComponent<Spell>();
    if ((Object) this.m_spell == (Object) null)
      Log.Gameplay.PrintError("QuestlineController.Awake(): GameObject " + this.gameObject.name + " does not have a Spell Component!");
    this.m_spell.AddSpellEventCallback(new Spell.SpellEventCallback(this.OnSpellEvent));
    this.m_spellFSM = this.m_spell.GetComponent<PlayMakerFSM>();
    this.m_actor = this.GetComponent<Actor>();
    if ((Object) this.m_actor == (Object) null)
      Log.Gameplay.PrintError("QuestlineController.Awake(): GameObject " + this.gameObject.name + " does not have an Actor Component!");
    this.m_gameState = GameState.Get();
    if (this.m_gameState == null)
      Log.Gameplay.PrintError("QuestlineController.Awake(): Gameobject " + this.gameObject.name + " could not initialize GameState!");
    this.m_inputManager = InputManager.Get();
    if (!((Object) this.m_inputManager == (Object) null))
      return;
    Log.Gameplay.PrintError("QuestlineController.Awake(): Gameobject " + this.gameObject.name + " could not initialize InputManager!");
  }

  private void Start()
  {
    this.m_QuestlineProgressUI = this.m_QuestlineProgressUIContainer.PrefabGameObject(true).GetComponent<QuestlineProgressUI>();
    this.m_QuestlineProgressUI.SetOriginalQuestActor(this.m_actor);
    this.m_QuestlineProgressUI.Hide();
    this.m_QuestlineProgressUI.transform.parent = Board.Get().FindBone(this.m_QuestUIBoneName);
    TransformUtil.Identity((Component) this.m_QuestlineProgressUI);
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
    this.StopCoroutine("WaitThenShowQuestlineUI");
    this.StartCoroutine("WaitThenShowQuestlineUI");
  }

  public void NotifyMousedOut()
  {
    this.StopCoroutine("WaitThenShowQuestlineUI");
    this.m_QuestlineProgressUI.Hide();
  }

  private IEnumerator WaitThenShowQuestlineUI()
  {
    if (this.IsEntityValid())
    {
      yield return (object) new WaitForSeconds(this.m_inputManager.m_MouseOverDelay);
      this.RefreshQuestProgressValues();
      this.m_QuestlineProgressUI.UpdateText(this.m_actualQuestProgress, this.m_questProgressTotal);
      this.m_QuestlineProgressUI.Show();
    }
  }

  public void UpdateQuestlineUI()
  {
    if (!this.IsEntityValid())
      return;
    this.StartCoroutine(this.UpdateQuestlineUIImpl());
  }

  private IEnumerator UpdateQuestlineUIImpl()
  {
    this.RefreshQuestProgressValues();
    if (this.m_actualQuestProgress != this.m_displayedQuestProgress)
    {
      this.m_displayedQuestProgress = Mathf.Min(this.m_displayedQuestProgress, this.m_actualQuestProgress);
      this.m_gameState.SetBusy(true);
      while (this.m_isScalingDown)
        yield return (object) null;
      if (this.m_actualQuestProgress < this.m_questProgressTotal)
        this.m_gameState.SetBusy(false);
      if (!this.m_spell.IsActive())
      {
        this.UpdateProgressText();
        this.m_spell.ActivateState(SpellStateType.ACTION);
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
      if (this.m_displayedQuestProgress >= this.m_questProgressTotal)
        this.CompleteQuest();
      else
        this.m_spell.ActivateState(SpellStateType.NONE);
    }
  }

  private IEnumerator UpdateQuestProgress()
  {
    while (this.m_displayedQuestProgress < this.m_actualQuestProgress)
    {
      ++this.m_displayedQuestProgress;
      this.UpdateProgressText();
      yield return (object) new WaitForSeconds(this.m_ProgressUpdateDelay);
    }
    this.m_isScalingDown = true;
    this.m_spellFSM.SendEvent("ScaleDown");
  }

  private void UpdateProgressText()
  {
    this.m_ProgressUpdateParticles.Stop();
    this.m_ProgressUpdateParticles.Play();
    this.m_ProgressText.Text = string.Format("{0}/{1}", (object) this.m_displayedQuestProgress, (object) this.m_questProgressTotal);
    this.m_QuestlineProgressUI.UpdateText(this.m_displayedQuestProgress, this.m_questProgressTotal);
  }

  private void CompleteQuest()
  {
    this.m_gameState.SetBusy(false);
    this.m_questCompleted = true;
    this.m_QuestlineProgressUI.Hide();
    this.m_spell.ActivateState(SpellStateType.DEATH);
  }

  private bool IsEntityValid()
  {
    if (this.m_entity == null)
    {
      this.m_entity = this.m_actor.GetEntity();
      if (this.m_entity == null)
        return false;
    }
    return true;
  }

  private void RefreshQuestProgressValues()
  {
    if (!this.IsEntityValid())
      return;
    this.m_actualQuestProgress = this.m_entity.GetTag(GAME_TAG.QUEST_PROGRESS);
    this.m_questProgressTotal = this.m_entity.GetTag(GAME_TAG.QUEST_PROGRESS_TOTAL);
  }
}
