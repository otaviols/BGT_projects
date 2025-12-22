using Hearthstone.DataModels;
using Hearthstone.UI;
using System.Collections;
using UnityEngine;

public class LuckyDrawHammerSlot : MonoBehaviour
{
  [SerializeField]
  private PlayMakerFSM m_hammerPlaymaker;
  [SerializeField]
  private PlayMakerFSM m_firstTimeHammerPlaymaker;
  [SerializeField]
  private VisualController m_firstHammerVisualController;
  private Widget m_widget;
  private const string kFirstHammerAnimStartEventName = "First_Hammer_Claimed";
  private bool m_rewardTargetReceived;
  private const float kHammerAnimationTimeoutTime = 15f;

  public PlayMakerFSM HammerPlaymaker => this.m_hammerPlaymaker;

  private void Awake()
  {
    this.m_widget = this.GetComponent<Widget>();
    if ((Object) this.m_widget == (Object) null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawHammerSlot] Awake() m_widget was null!");
    }
    else
    {
      this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleEvent));
      if ((Object) this.m_hammerPlaymaker == (Object) null)
      {
        Error.AddDevWarning("UI Error", "[LuckyDrawHammerSlot] Awake() m_hammerPlaymaker was null!");
      }
      else
      {
        if (!((Object) this.m_firstTimeHammerPlaymaker == (Object) null))
          return;
        Error.AddDevWarning("UI Error", "[LuckyDrawHammerSlot] Awake() m_firstTimeHammerPlaymaker was null!");
      }
    }
  }

  private void HandleEvent(string eventName)
  {
    EventDataModel dataModel = this.m_widget.GetDataModel<EventDataModel>();
    if (!(eventName == "CODE_INITIALIZE_HAMMER"))
    {
      if (!(eventName == "CODE_HAMMER_SMASH_READY"))
      {
        if (!(eventName == "CODE_DO_FIRST_HAMMER_CLAIM_ANIMATION"))
        {
          if (!(eventName == "CODE_ANTICIPATION_FINISHED"))
            return;
          this.FlagAnticipationAnimationComplete();
        }
        else
          this.DoFirstHammerClaimAnim();
      }
      else if (dataModel == null)
        Error.AddDevWarning("Error", "[LuckyDrawHammerSlot] HandleEvent() event payload was null from {0} event", (object) eventName);
      else
        this.SetupHammerSmashTarget(dataModel);
    }
    else if (dataModel == null)
      Error.AddDevWarning("Error", "[LuckyDrawHammerSlot] HandleEvent() event payload was null from {0} event", (object) eventName);
    else
      this.InitializeHammerFSMVariables(dataModel);
  }

  public void DisplayFirstHammer()
  {
    if ((Object) this.m_firstHammerVisualController == (Object) null)
      Error.AddDevWarning("UI Error", "[LuckyDrawHammerSlot] DisplayFirstHammer() m_firstHammerVisualController was null!");
    else if (!LuckyDrawManager.Get().GetBattlegroundsLuckyDrawDataModel().HasUnclamedFree)
    {
      this.m_firstHammerVisualController.SetState("INACTIVE");
    }
    else
    {
      this.m_firstHammerVisualController.SetState("ACTIVE");
      NarrativeManager.Get().OnLuckyDrawEntered();
    }
  }

  private void InitializeHammerFSMVariables(EventDataModel eventPayload)
  {
    this.m_rewardTargetReceived = false;
    if (eventPayload == null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawHammerSlot] InitializeHammerFSMVariables() eventPayload was null!");
      LuckyDrawUtils.ShowErrorAndReturnToLobby();
    }
    else
    {
      Vector3 vector3_1 = this.m_hammerPlaymaker.FsmVariables.GetFsmVector3("HammerUpPosition").Value;
      Vector3 vector3_2 = this.m_hammerPlaymaker.FsmVariables.GetFsmVector3("HammerAnticipationOffset").Value;
      Vector3 localPosition = this.m_hammerPlaymaker.transform.localPosition;
      this.m_hammerPlaymaker.FsmVariables.GetFsmVector3("HammerIdlePosition").Value = localPosition;
      this.m_hammerPlaymaker.FsmVariables.GetFsmVector3("Calculated_HammerUpPosition").Value = localPosition + vector3_1;
      Vector3 vector3_3 = (Vector3) eventPayload.Payload + vector3_2;
      this.m_hammerPlaymaker.FsmVariables.GetFsmVector3("Calculated_HammerAnticipationPosition").Value = new Vector3(vector3_3.x, vector3_1.y + vector3_2.y, vector3_3.z);
    }
  }

  private void FlagAnticipationAnimationComplete() => this.StartCoroutine(this.WaitForTargetInfo());

  private IEnumerator WaitForTargetInfo()
  {
    float cancelTime = Time.time + 15f;
    while (!this.m_rewardTargetReceived)
    {
      if ((double) Time.time > (double) cancelTime)
      {
        Log.All.PrintError("Error [LuckyDrawHammerSlot] WaitForTargetInfo() timeout triggered while waiting for rewardTarget");
        LuckyDrawUtils.ShowErrorAndReturnToLobby();
        yield break;
      }
      else
        yield return (object) null;
    }
    this.m_hammerPlaymaker.SendEvent("Smash_Reward_Tile");
  }

  private void SetupHammerSmashTarget(EventDataModel eventPayload)
  {
    if (eventPayload == null)
    {
      Error.AddDevWarning("Error", "[LuckyDrawHammerSlot] PerformHammerSmashAnim() eventPayload was null!");
      LuckyDrawUtils.ShowErrorAndReturnToLobby();
    }
    else
    {
      Vector3 vector3_1 = this.m_hammerPlaymaker.FsmVariables.GetFsmVector3("HammerTargetOffset").Value;
      Vector3 vector3_2 = this.m_hammerPlaymaker.FsmVariables.GetFsmVector3("HammerDownOffset").Value;
      Vector3 vector3_3 = this.m_hammerPlaymaker.FsmVariables.GetFsmVector3("HammerUpPosition").Value;
      Vector3 payload = (Vector3) eventPayload.Payload;
      this.m_hammerPlaymaker.FsmVariables.GetFsmVector3("TileWorldPosition").Value = payload;
      Vector3 vector3_4 = payload + vector3_1;
      this.m_hammerPlaymaker.FsmVariables.GetFsmVector3("Calculated_TargetPosition").Value = new Vector3(vector3_4.x, vector3_3.y + vector3_1.y, vector3_4.z);
      this.m_hammerPlaymaker.FsmVariables.GetFsmVector3("Calculated_HammerDownPosition").Value = new Vector3(vector3_4.x, 0.0f, vector3_4.z) + vector3_2;
      this.m_rewardTargetReceived = true;
    }
  }

  private void DoFirstHammerClaimAnim() => this.m_firstTimeHammerPlaymaker.SendEvent("First_Hammer_Claimed");
}
