using Blizzard.T5.Core.Utils;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class BaconBoardSkinBehaviour : MonoBehaviour
{
  private const string FSM_STATE_NAMES_WIN_STREAK = "WIN_STREAK_{0}";
  private const string FSM_STATE_NAMES_LOSE_STREAK = "LOSE_STREAK_{0}";
  private const string FSM_STATE_NAMES_TOP_FOUR = "HERO_TOP_4";
  private const string FSM_STATE_NAMES_HERO_DEFEAT = "HERO_DEFEAT_ENEMY";
  private const string FSM_STATE_NAMES_HERO_DEFEATED = "HERO_DEFEATED_ENEMY";
  private const string FSM_STATE_NAMES_MINION_DEFEAT = "MINION_DEFEAT_{0}";
  private const string FSM_STATE_NAMES_MINION_DEFEATED = "MINION_DEFEATED_{0}";
  private const string FSM_STATE_NAMES_MINION_DEFEAT_COUNT = "MINION_DEFEAT_COUNT_{0}";
  private const string FSM_STATE_NAMES_MINION_DEFEATED_COUNT = "MINION_DEFEATED_COUNT_{0}";
  private const string FSM_STATE_NAMES_MINION_TRIBE_DEFEAT = "MINION_DEFEAT_TRIBE_{0}";
  private const string FSM_STATE_NAMES_MINION_TRIBE_DEFEATED = "MINION_DEFEATED_TRIBE_{0}";
  private const string FSM_STATE_NAMES_HEALTH_AT_OR_BELOW = "HEALTH_AT_OR_BELOW_{0}";
  private const string FSM_STATE_NAMES_HERO_HEAVY_HIT = "HEAVY_HIT";
  private const string FSM_STATE_NAMES_MINION_HEAVY_HIT = "MINION_HEAVY_HIT";
  public TAG_BOARD_VISUAL_STATE m_BoardType;
  [Tooltip("If checked apply a default lighting transition using the color and timing variables below.")]
  public bool m_DefaultLightingEnabled = true;
  [Tooltip("If checked then this board has its own leaderboard frame and should hide the base one.")]
  [SerializeField]
  private bool m_HasOwnLeaderboardFrame;
  [Tooltip("If checked then this board has its own table top and should hide the base one.")]
  [SerializeField]
  private bool m_HasOwnTableTop;
  [Tooltip("Minimum minion damage required before it's considered a 'heavy hit' (exposed for designer tweaking).")]
  public int m_MinMinionHeavyHitDamage = 100;
  public Color m_AmbientColor;
  [FormerlySerializedAs("m_CombatAmbientTransitionDelay")]
  public float m_AmbientTransitionDelay = 0.5f;
  [FormerlySerializedAs("m_CombatAmbientTransitionTime")]
  public float m_AmbientTransitionTime = 0.25f;
  [HideInInspector]
  public float m_CombatAmbientTransitionDelay = 0.5f;
  [HideInInspector]
  public float m_CombatAmbientTransitionTime = 0.25f;
  [Tooltip("The number of seconds after the back-to-shop animation starts before this board skin object should be unloaded.")]
  public float m_UnloadDelay = 1.5f;
  public BaconBoardSkinBehaviour.BaconBoardSkinCorners m_Corners;
  public List<PlayMakerFSM> m_BoardStateChangingObjects;
  public List<TextureTweenController> m_BoardTextureChangingObjects;
  public List<string> m_UniqueFsmTriggerEventOrder = new List<string>();
  private List<string> m_DeferredFsmTriggerRequests = new List<string>();

  protected TAG_BOARD_VISUAL_STATE GetActivatedState() => this.m_BoardType;

  private void TransitionToActivatedState(TAG_BOARD_VISUAL_STATE newBoardState)
  {
    this.SetLighting();
    this.m_DeferredFsmTriggerRequests.Add(EnumUtils.GetString<TAG_BOARD_VISUAL_STATE>(newBoardState));
    this.ProcessDeferredFsmTriggerRequests();
  }

  private void TransitionFromActivatedState(TAG_BOARD_VISUAL_STATE newBoardState) => this.SetStateOnFsms(EnumUtils.GetString<TAG_BOARD_VISUAL_STATE>(newBoardState));

  private void StartAdditionalTransitions(TAG_BOARD_VISUAL_STATE newBoardState)
  {
    foreach (TextureTweenController textureChangingObject in this.m_BoardTextureChangingObjects)
    {
      if (!((UnityEngine.Object) textureChangingObject == (UnityEngine.Object) null))
      {
        if (newBoardState == this.GetActivatedState())
          textureChangingObject.StartForwardTransition();
        else
          textureChangingObject.StartReverseTransition();
      }
    }
  }

  public bool HasOwnLeaderboardFrame() => this.m_HasOwnLeaderboardFrame;

  public bool HasOwnTableTop() => this.m_HasOwnTableTop;

  public void CopyCornersFromSkin(BaconBoardSkinBehaviour source)
  {
    this.m_Corners.TL.CopyToBackside(source.m_Corners.TL.m_TopContainer);
    this.m_Corners.TR.CopyToBackside(source.m_Corners.TR.m_TopContainer);
    this.m_Corners.BL.CopyToBackside(source.m_Corners.BL.m_TopContainer);
    this.m_Corners.BR.CopyToBackside(source.m_Corners.BR.m_TopContainer);
  }

  public void SetBoardState(TAG_BOARD_VISUAL_STATE newBoardState)
  {
    if (newBoardState == this.GetActivatedState())
      this.TransitionToActivatedState(newBoardState);
    else
      this.TransitionFromActivatedState(newBoardState);
    this.StartAdditionalTransitions(newBoardState);
  }

  private void ProcessDeferredFsmTriggerRequests()
  {
    int index = -1;
    foreach (string fsmTriggerRequest in this.m_DeferredFsmTriggerRequests)
    {
      int num = this.m_UniqueFsmTriggerEventOrder.IndexOf(fsmTriggerRequest);
      if (num < 0)
        this.SetStateOnFsms(fsmTriggerRequest);
      else if (index < 0 || num < index)
        index = num;
    }
    if (0 <= index)
      this.SetStateOnFsms(this.m_UniqueFsmTriggerEventOrder[index]);
    this.m_DeferredFsmTriggerRequests.Clear();
  }

  public void RequestWinStreak(int winStreak)
  {
    foreach (PlayMakerFSM stateChangingObject in this.m_BoardStateChangingObjects)
    {
      for (int index = winStreak; index > 0; --index)
      {
        string stateName = string.Format("WIN_STREAK_{0}", (object) index);
        if (this.FsmContainsState(stateChangingObject, stateName))
        {
          this.m_DeferredFsmTriggerRequests.Add(stateName);
          break;
        }
      }
    }
  }

  public void RequestLoseStreak(int loseStreak)
  {
    foreach (PlayMakerFSM stateChangingObject in this.m_BoardStateChangingObjects)
    {
      for (int index = loseStreak; index > 0; --index)
      {
        string stateName = string.Format("LOSE_STREAK_{0}", (object) index);
        if (this.FsmContainsState(stateChangingObject, stateName))
        {
          this.m_DeferredFsmTriggerRequests.Add(stateName);
          break;
        }
      }
    }
  }

  public void RequestTopFourPlacement() => this.m_DeferredFsmTriggerRequests.Add("HERO_TOP_4");

  public void RequestFriendlyPlayerHealthAtOrBelow(int maxHealth, int currentHealth)
  {
    foreach (PlayMakerFSM stateChangingObject in this.m_BoardStateChangingObjects)
    {
      for (int index = currentHealth; index <= maxHealth; ++index)
      {
        string stateName = string.Format("HEALTH_AT_OR_BELOW_{0}", (object) index);
        if (this.FsmContainsState(stateChangingObject, stateName))
        {
          this.m_DeferredFsmTriggerRequests.Add(stateName);
          break;
        }
      }
    }
  }

  public void RequestFriendlyPlayerHasDefeatedMinion(string minionCardID) => this.m_DeferredFsmTriggerRequests.Add(string.Format("MINION_DEFEATED_{0}", (object) minionCardID.ToUpper()));

  public void RequestFriendlyPlayerHasDefeatedRace(TAG_RACE race)
  {
    if (race == TAG_RACE.ALL)
    {
      foreach (object name in Enum.GetNames(typeof (TAG_RACE)))
        this.m_DeferredFsmTriggerRequests.Add(string.Format("MINION_DEFEATED_TRIBE_{0}", name));
    }
    else
      this.m_DeferredFsmTriggerRequests.Add(string.Format("MINION_DEFEATED_TRIBE_{0}", (object) Enum.GetName(typeof (TAG_RACE), (object) race)));
  }

  public void RequestOpponentMinionPreviouslyDefeatedCount(int defeatedCount)
  {
    foreach (PlayMakerFSM stateChangingObject in this.m_BoardStateChangingObjects)
    {
      for (int index = defeatedCount; index > 0; --index)
      {
        string stateName = string.Format("MINION_DEFEATED_COUNT_{0}", (object) index);
        if (this.FsmContainsState(stateChangingObject, stateName))
        {
          this.m_DeferredFsmTriggerRequests.Add(stateName);
          break;
        }
      }
    }
  }

  public void RequestHasFriendlyPlayerDefeatedOpponent() => this.m_DeferredFsmTriggerRequests.Add("HERO_DEFEATED_ENEMY");

  public void PlayOpponentHeroDefeated() => this.SetStateOnFsms("HERO_DEFEAT_ENEMY");

  public void PlayOpponentMinionDefeated(EntityDef minion)
  {
    this.SetStateOnFsms(string.Format("MINION_DEFEAT_{0}", (object) minion.GetCardId().ToUpper()));
    if (minion.GetRaces().Contains(TAG_RACE.ALL))
    {
      foreach (object name in Enum.GetNames(typeof (TAG_RACE)))
        this.SetStateOnFsms(string.Format("MINION_DEFEAT_TRIBE_{0}", name));
    }
    else
    {
      foreach (int race in minion.GetRaces())
        this.SetStateOnFsms(string.Format("MINION_DEFEAT_TRIBE_{0}", (object) Enum.GetName(typeof (TAG_RACE), (object) (TAG_RACE) race)));
    }
  }

  public void PlayOpponentMinionDefeatedCount(int defeatedCount)
  {
    foreach (PlayMakerFSM stateChangingObject in this.m_BoardStateChangingObjects)
    {
      for (int index = defeatedCount; index > 0; --index)
      {
        string stateName = string.Format("MINION_DEFEAT_COUNT_{0}", (object) index);
        if (this.FsmContainsState(stateChangingObject, stateName))
        {
          stateChangingObject.SetState(stateName);
          break;
        }
      }
    }
  }

  public void CheckForHeroHeavyHitBoardEffects(Card sourceCard, Card targetCard)
  {
    if (!this.IsHeavyHit(sourceCard))
      return;
    if ((UnityEngine.Object) targetCard == (UnityEngine.Object) targetCard.GetHeroCard())
    {
      this.SetStateOnFsms("HEAVY_HIT");
    }
    else
    {
      if (!sourceCard.GetEntity().IsControlledByFriendlySidePlayer())
        return;
      this.SetStateOnFsms("MINION_HEAVY_HIT");
    }
  }

  public void CheatTriggerHeroHeavyHitBoardEffects() => this.SetStateOnFsms("HEAVY_HIT");

  public void CheatTriggerMinionHeavyHitBoardEffects() => this.SetStateOnFsms("MINION_HEAVY_HIT");

  public void CheatTriggerAllBoardEffects()
  {
    foreach (PlayMakerFSM stateChangingObject in this.m_BoardStateChangingObjects)
    {
      if (!((UnityEngine.Object) stateChangingObject == (UnityEngine.Object) null))
      {
        foreach (FsmState fsmState in stateChangingObject.FsmStates)
        {
          if (fsmState.Name != "SHOP" && fsmState.Name != "COMBAT")
            stateChangingObject.SetState(fsmState.Name);
        }
      }
    }
  }

  public void CheatTriggerDefeatMinion(string cardID) => this.SetStateOnFsms(string.Format("MINION_DEFEAT_{0}", (object) cardID.ToUpper()));

  public void DebugTriggerFSMState(string stateName) => this.SetStateOnFsms(stateName);

  private bool IsHeavyHit(Card sourceCard) => !sourceCard.GetEntity().IsControlledByOpposingSidePlayer() && sourceCard.GetEntity().GetATK() >= this.m_MinMinionHeavyHitDamage;

  private bool FsmContainsState(PlayMakerFSM fsm, string stateName)
  {
    foreach (FsmState fsmState in fsm.FsmStates)
    {
      if (stateName.Equals(fsmState.Name))
        return true;
    }
    return false;
  }

  private void SetStateOnFsms(string stateName)
  {
    foreach (PlayMakerFSM stateChangingObject in this.m_BoardStateChangingObjects)
    {
      if (this.FsmContainsState(stateChangingObject, stateName))
        stateChangingObject.SetState(stateName);
    }
  }

  public void SetLighting()
  {
    if (!this.m_DefaultLightingEnabled)
      return;
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) RenderSettings.ambientLight, (object) "to", (object) this.m_AmbientColor, (object) "delay", (object) this.m_AmbientTransitionDelay, (object) "time", (object) this.m_AmbientTransitionTime, (object) "easeType", (object) iTween.EaseType.easeInOutQuad, (object) "onupdate", (object) (Action<object>) (amount => RenderSettings.ambientLight = (Color) amount), (object) "onupdatetarget", (object) this.gameObject));
  }

  public void QueueToUnload(BaconBoard unloadTarget)
  {
    if (this.m_BoardType != TAG_BOARD_VISUAL_STATE.COMBAT)
      return;
    this.StartCoroutine(this.QueueToUnloadCoroutine(unloadTarget));
  }

  private IEnumerator QueueToUnloadCoroutine(BaconBoard unloadTarget)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BaconBoardSkinBehaviour sourceBehavior = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      unloadTarget.ProcessUnloadRequest(sourceBehavior);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(sourceBehavior.m_UnloadDelay);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public void OnValidate()
  {
    if (!this.gameObject.activeInHierarchy)
      return;
    RenderSettings.ambientLight = this.m_AmbientColor;
  }

  public void OnEnable()
  {
    if (Application.isPlaying)
      return;
    RenderSettings.ambientLight = this.m_AmbientColor;
  }

  [Serializable]
  public class BaconBoardSkinCorners
  {
    public BaconBoardSkinCorner TL;
    public BaconBoardSkinCorner TR;
    public BaconBoardSkinCorner BL;
    public BaconBoardSkinCorner BR;
  }
}
