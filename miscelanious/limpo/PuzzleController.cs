using System.Collections;
using UnityEngine;

[RequireComponent(typeof (Actor))]
[RequireComponent(typeof (Spell))]
public class PuzzleController : MonoBehaviour
{
  public UberText m_ProgressText;
  public NestedPrefab m_PuzzleProgressUIContainer;
  public string m_PuzzleUIBoneName = "QuestUI";
  public ParticleSystem m_ProgressUpdateParticles;
  public float m_PuzzleCompleteDelay = 1f;
  private Spell m_spell;
  private Actor m_actor;
  private Entity m_entity;
  private PuzzleProgressUI m_puzzleProgressUI;

  private void Awake()
  {
    this.m_spell = this.GetComponent<Spell>();
    if ((Object) this.m_spell == (Object) null)
      Log.Gameplay.PrintError("PuzzleController.Awake(): GameObject {0} does not have a Spell Component!", (object) this.gameObject.name);
    this.m_spell.AddSpellEventCallback(new Spell.SpellEventCallback(this.OnSpellEvent));
    this.m_actor = this.GetComponent<Actor>();
    if (!((Object) this.m_actor == (Object) null))
      return;
    Log.Gameplay.PrintError("PuzzleController.Awake(): GameObject {0} does not have an Actor Component!", (object) this.gameObject.name);
  }

  private void Start()
  {
    this.m_puzzleProgressUI = this.m_PuzzleProgressUIContainer.PrefabGameObject(true).GetComponent<PuzzleProgressUI>();
    this.m_puzzleProgressUI.Hide();
    this.m_puzzleProgressUI.transform.parent = Board.Get().FindBone(this.m_PuzzleUIBoneName);
    TransformUtil.Identity((Component) this.m_puzzleProgressUI);
  }

  public void OnDestroy() => this.NotifyMousedOut();

  public void NotifyMousedOver()
  {
    if (this.GetEntity().HasTag(GAME_TAG.PUZZLE_COMPLETED))
      return;
    this.StopCoroutine("WaitThenShowPuzzleUI");
    this.StartCoroutine("WaitThenShowPuzzleUI");
  }

  public void NotifyMousedOut()
  {
    this.StopCoroutine("WaitThenShowPuzzleUI");
    this.m_puzzleProgressUI.Hide();
  }

  private IEnumerator WaitThenShowPuzzleUI()
  {
    yield return (object) new WaitForSeconds(InputManager.Get().m_MouseOverDelay);
    if (this.GetEntity() != null)
    {
      this.m_puzzleProgressUI.UpdateText(this.GetEntity());
      this.m_puzzleProgressUI.Show();
    }
  }

  public void OnRealTimePuzzleCompleted(int newValue)
  {
    if (newValue != 1)
      return;
    EndTurnButton endTurnButton = EndTurnButton.Get();
    if (!((Object) endTurnButton != (Object) null))
      return;
    endTurnButton.AddInputBlocker();
  }

  public void UpdatePuzzleUI()
  {
    this.UpdateProgressText();
    if (!this.GetEntity().HasTag(GAME_TAG.PUZZLE_COMPLETED))
      return;
    GameState.Get().SetBusy(true);
    this.m_ProgressUpdateParticles.Stop();
    this.m_ProgressUpdateParticles.Play();
    this.m_spell.ActivateState(SpellStateType.ACTION);
  }

  private void OnSpellEvent(string eventName, object eventData, object userData)
  {
    if (!(eventName == "FlashCompleted"))
      return;
    GameState.Get().SetBusy(false);
  }

  private void UpdateProgressText()
  {
    this.m_ProgressText.Text = string.Format("{0}/{1}", (object) this.GetEntity().GetTag(GAME_TAG.PUZZLE_PROGRESS), (object) this.GetEntity().GetTag(GAME_TAG.PUZZLE_PROGRESS_TOTAL));
    this.m_puzzleProgressUI.UpdateText(this.GetEntity());
  }

  private Entity GetEntity()
  {
    if (this.m_entity == null)
      this.m_entity = this.m_actor.GetEntity();
    return this.m_entity;
  }
}
