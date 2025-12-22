using System.Collections.Generic;
using UnityEngine;

public class Gryphon : MonoBehaviour
{
  public float m_HeadRotationSpeed = 15f;
  public float m_MinFocusTime = 1.2f;
  public float m_MaxFocusTime = 5.5f;
  public int m_PlayAnimationPercent = 20;
  public int m_LookAtHeroesPercent = 20;
  public int m_LookAtTurnButtonPercent = 75;
  public float m_TurnButtonLookAwayTime = 0.5f;
  public float m_SnapWaitTime = 1f;
  public Transform m_HeadBone;
  public GameObject m_SnapCollider;
  private float m_WaitStartTime;
  private float m_RandomWeightsTotal;
  private Vector3 m_LookAtPosition;
  private Animator m_Animator;
  private EndTurnButton m_EndTurnButton;
  private Transform m_EndTurnButtonTransform;
  private UniversalInputManager m_UniversalInputManager;
  private AnimatorStateInfo m_CurrentBaseLayerState;
  private AudioSource m_ScreechSound;
  private float m_lastScreech;
  private float m_idleEndTime;
  private static int lookState = Animator.StringToHash("Base Layer.Look");
  private static int cleanState = Animator.StringToHash("Base Layer.Clean");
  private static int screechState = Animator.StringToHash("Base Layer.Screech");

  private void Start()
  {
    this.m_Animator = this.GetComponent<Animator>();
    this.m_UniversalInputManager = UniversalInputManager.Get();
    this.m_ScreechSound = this.GetComponent<AudioSource>();
    this.m_SnapWaitTime = Random.Range(5f, 20f);
    this.m_Animator.SetLayerWeight(1, 1f);
  }

  private void LateUpdate()
  {
    bool flag = false;
    this.m_CurrentBaseLayerState = this.m_Animator.GetCurrentAnimatorStateInfo(0);
    if (this.m_UniversalInputManager != null)
    {
      if (GameState.Get() != null && GameState.Get().IsMulliganManagerActive())
        return;
      if (this.m_UniversalInputManager.InputIsOver(this.gameObject))
        flag = InputCollection.GetMouseButtonDown(0);
      if (flag)
      {
        if ((double) Time.time - (double) this.m_lastScreech > 5.0)
        {
          this.m_Animator.SetBool("Screech", true);
          SoundManager.Get().Play(this.m_ScreechSound);
          this.m_lastScreech = Time.time;
        }
      }
      else
        this.m_Animator.SetBool("Screech", false);
    }
    if (this.m_CurrentBaseLayerState.fullPathHash == Gryphon.lookState || this.m_CurrentBaseLayerState.fullPathHash == Gryphon.cleanState || this.m_CurrentBaseLayerState.fullPathHash == Gryphon.screechState)
      return;
    this.m_Animator.SetBool("Look", false);
    this.m_Animator.SetBool("Clean", false);
    this.PlayAniamtion();
  }

  private void FindEndTurnButton()
  {
    this.m_EndTurnButton = EndTurnButton.Get();
    if ((Object) this.m_EndTurnButton == (Object) null)
      return;
    this.m_EndTurnButtonTransform = this.m_EndTurnButton.transform;
  }

  private void FindSomethingToLookAt()
  {
    List<Vector3> vector3List = new List<Vector3>();
    ZoneMgr zoneMgr = ZoneMgr.Get();
    if ((Object) zoneMgr == (Object) null)
    {
      this.PlayAniamtion();
    }
    else
    {
      foreach (Zone zone in zoneMgr.FindZonesOfType<ZonePlay>())
      {
        foreach (Card card in zone.GetCards())
        {
          if (card.IsMousedOver())
          {
            this.m_LookAtPosition = card.transform.position;
            return;
          }
          vector3List.Add(card.transform.position);
        }
      }
      if (Random.Range(0, 100) < this.m_LookAtHeroesPercent)
      {
        foreach (Zone zone in ZoneMgr.Get().FindZonesOfType<ZoneHero>())
        {
          foreach (Card card in zone.GetCards())
          {
            if (card.IsMousedOver())
            {
              this.m_LookAtPosition = card.transform.position;
              return;
            }
            vector3List.Add(card.transform.position);
          }
        }
      }
      if (vector3List.Count > 0)
      {
        int index = Random.Range(0, vector3List.Count);
        this.m_LookAtPosition = vector3List[index];
      }
      else
        this.PlayAniamtion();
    }
  }

  private void PlayAniamtion()
  {
    if ((double) Time.time < (double) this.m_idleEndTime)
      return;
    if ((double) Random.value > 0.5)
    {
      this.m_idleEndTime = Time.time + 4f;
      this.m_Animator.SetBool("Look", false);
      this.m_Animator.SetBool("Clean", false);
    }
    else if ((double) Random.value > 0.25)
      this.m_Animator.SetBool("Look", true);
    else
      this.m_Animator.SetBool("Clean", true);
  }

  private bool LookAtTurnButton()
  {
    if ((Object) this.m_EndTurnButton == (Object) null)
      this.FindEndTurnButton();
    if ((Object) this.m_EndTurnButton == (Object) null || !this.m_EndTurnButton.IsInNMPState() || !((Object) this.m_EndTurnButtonTransform != (Object) null))
      return false;
    this.m_LookAtPosition = this.m_EndTurnButtonTransform.position;
    return true;
  }

  private void AniamteHead()
  {
    if (this.m_CurrentBaseLayerState.fullPathHash == Gryphon.lookState || this.m_CurrentBaseLayerState.fullPathHash == Gryphon.cleanState || this.m_CurrentBaseLayerState.fullPathHash == Gryphon.screechState)
      return;
    this.m_HeadBone.rotation = Quaternion.Slerp(this.m_HeadBone.rotation, Quaternion.LookRotation(this.m_LookAtPosition - this.m_HeadBone.position), Time.deltaTime * this.m_HeadRotationSpeed);
  }
}
