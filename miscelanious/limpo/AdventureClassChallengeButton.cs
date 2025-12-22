using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class AdventureClassChallengeButton : PegUIElement
{
  public UberText m_Text;
  public int m_ScenarioID;
  public HighlightState m_Highlight;
  public GameObject m_RootObject;
  public GameObject m_Chest;
  public GameObject m_Checkmark;
  public Transform m_UpBone;
  public Transform m_DownBone;

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_hero_mouse_over.prefab:653cc8000b988cd468d2210a209adce6", this.gameObject);
    this.m_Highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState) => this.m_Highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);

  public void Select(bool playSound)
  {
    if (playSound)
      SoundManager.Get().LoadAndPlay((AssetReference) "select_AI_opponent.prefab:a48887f01f79fa743a0c5de53a959b60", this.gameObject);
    this.m_Highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    this.SetEnabled(false);
    this.Depress();
  }

  public void Deselect()
  {
    this.m_Highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
    this.Raise(0.1f);
    this.SetEnabled(true);
  }

  public void SetPortraitMaterial(Material portraitMat) => RendererExtension.SetMaterial(this.m_RootObject.GetComponent<Renderer>(), 1, portraitMat);

  private void Raise(float time) => iTween.MoveTo(this.m_RootObject, iTween.Hash((object) "position", (object) this.m_UpBone.localPosition, (object) nameof (time), (object) time, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true));

  private void Depress() => iTween.MoveTo(this.m_RootObject, iTween.Hash((object) "position", (object) this.m_DownBone.localPosition, (object) "time", (object) 0.1f, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true));
}
