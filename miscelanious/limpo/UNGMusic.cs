using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class UNGMusic : MonoBehaviour
{
  public List<MusicNote> m_MusicNotes;
  public string m_PlayedEvent;
  public List<int> m_NoteSequence;
  public GameObject m_RewardCollisionObject;
  public string m_RewardFSMName;
  public string m_RewardEvent;
  public GameObject m_ClickableFSM;
  public string m_ClickableFSMName;
  public string m_ClickableEvent;
  private List<int> m_CorrectNotesHit = new List<int>();

  private void Start() => this.m_CorrectNotesHit.Clear();

  private void Update() => this.HandleHits();

  private void HandleHits()
  {
    for (int index = 0; index < this.m_MusicNotes.Count; ++index)
    {
      if (this.m_MusicNotes[index] != null && InputCollection.GetMouseButtonUp(0) && this.IsOver(this.m_MusicNotes[index].m_CollisionObject))
      {
        this.m_MusicNotes[index].m_CollisionObject.GetComponent<PlayMakerFSM>().SendEvent(this.m_PlayedEvent);
        if (index == this.m_NoteSequence[this.m_CorrectNotesHit.Count])
        {
          this.m_CorrectNotesHit.Add(index);
          if (this.m_CorrectNotesHit.Count != this.m_NoteSequence.Count)
            break;
          foreach (PlayMakerFSM component in this.m_RewardCollisionObject.GetComponents<PlayMakerFSM>())
          {
            if (component.FsmName == this.m_RewardFSMName)
              component.SendEvent(this.m_RewardEvent);
            else if (component.FsmName == this.m_ClickableFSMName)
              component.SendEvent(this.m_ClickableEvent);
          }
          this.m_CorrectNotesHit.Clear();
          break;
        }
        this.m_CorrectNotesHit.Clear();
        if (index != this.m_NoteSequence[0])
          break;
        this.m_CorrectNotesHit.Add(index);
        break;
      }
    }
  }

  private bool IsOver(GameObject go) => (bool) (Object) go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);
}
