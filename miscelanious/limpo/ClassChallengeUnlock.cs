using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class ClassChallengeUnlock : Reward
{
  [CustomEditField(Sections = "Container")]
  public UIBObjectSpacing m_classFrameContainer;
  [CustomEditField(Sections = "Text Settings")]
  public UberText m_headerText;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_appearSound;
  private List<GameObject> m_classFrames = new List<GameObject>();
  private ScreenEffectsHandle m_screenEffectsHandle;

  protected override void Awake()
  {
    base.Awake();
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.m_rewardBanner.transform.localScale = this.m_rewardBanner.transform.localScale * 8f;
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  public static List<AdventureMissionDbfRecord> AdventureMissionsUnlockedByWingId(
    int wingId)
  {
    List<AdventureMissionDbfRecord> missionDbfRecordList = new List<AdventureMissionDbfRecord>();
    foreach (AdventureMissionDbfRecord record1 in GameDbf.AdventureMission.GetRecords())
    {
      if (record1.ReqWingId == wingId)
      {
        int scenarioId = record1.ScenarioId;
        ScenarioDbfRecord record2 = GameDbf.Scenario.GetRecord(scenarioId);
        if (record2 == null)
          Debug.LogError((object) string.Format("Unable to find Scenario record with ID: {0}", (object) scenarioId));
        else if (record2.ModeId == 4)
          missionDbfRecordList.Add(record1);
      }
    }
    return missionDbfRecordList;
  }

  protected override void InitData() => this.SetData((RewardData) new ClassChallengeUnlockData(), false);

  protected override void PlayShowSounds()
  {
  }

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_root.SetActive(true);
    this.m_classFrameContainer.UpdatePositions();
    foreach (GameObject classFrame in this.m_classFrames)
    {
      classFrame.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180f);
      Hashtable args = iTween.Hash((object) "amount", (object) new Vector3(0.0f, 0.0f, 540f), (object) "time", (object) 1.5f, (object) "easeType", (object) iTween.EaseType.easeOutElastic, (object) "space", (object) Space.Self);
      iTween.RotateAdd(classFrame, args);
    }
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
    {
      Time = 1f
    });
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_screenEffectsHandle.StopEffect(new Action(this.DestroyClassChallengeUnlock));
    this.m_root.SetActive(false);
  }

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals)
      return;
    if (!(this.Data is ClassChallengeUnlockData data))
    {
      Debug.LogWarning((object) string.Format("ClassChallengeUnlock.OnDataSet() - Data {0} is not ClassChallengeUnlockData", (object) this.Data));
    }
    else
    {
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      foreach (AdventureMissionDbfRecord missionDbfRecord in ClassChallengeUnlock.AdventureMissionsUnlockedByWingId(data.WingID))
      {
        int scenarioId = missionDbfRecord.ScenarioId;
        ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(scenarioId);
        if (record == null)
          Debug.LogError((object) string.Format("Unable to find Scenario record with ID: {0}", (object) scenarioId));
        else if (!string.IsNullOrEmpty(missionDbfRecord.ClassChallengePrefabPopup))
        {
          DbfLocValue shortName = record.ShortName;
          stringList1.Add(missionDbfRecord.ClassChallengePrefabPopup);
          stringList2.Add((string) shortName);
        }
        else
          Debug.LogWarning((object) string.Format("CLASS_CHALLENGE_PREFAB_POPUP not define for AdventureMission SCENARIO_ID: {0}", (object) scenarioId));
      }
      if (stringList1.Count == 0)
      {
        Debug.LogError((object) string.Format("Unable to find AdventureMission record with REQ_WING_ID: {0}.", (object) data.WingID));
      }
      else
      {
        this.m_headerText.Text = GameStrings.FormatPlurals("GLOBAL_REWARD_CLASS_CHALLENGE_HEADLINE", new GameStrings.PluralNumber[1]
        {
          new GameStrings.PluralNumber()
          {
            m_index = 0,
            m_number = stringList1.Count
          }
        });
        string headline = stringList1.Count <= 0 ? "" : string.Join(", ", stringList2.ToArray());
        string challengeRewardSource = (string) GameDbf.Wing.GetRecord(data.WingID).ClassChallengeRewardSource;
        this.SetRewardText(headline, string.Empty, challengeRewardSource);
        foreach (string assetRef in stringList1)
        {
          GameObject child = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef);
          if (!((UnityEngine.Object) child == (UnityEngine.Object) null))
          {
            GameUtils.SetParent(child, (Component) this.m_classFrameContainer);
            child.transform.localRotation = Quaternion.identity;
            this.m_classFrameContainer.AddObject(child);
            this.m_classFrames.Add(child);
          }
        }
        this.m_classFrameContainer.UpdatePositions();
        this.SetReady(true);
        this.EnableClickCatcher(true);
        this.RegisterClickListener(new Reward.OnClickedCallback(this.OnClicked));
      }
    }
  }

  private void OnClicked(Reward reward, object userData) => this.HideReward();

  private void DestroyClassChallengeUnlock() => UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.gameObject);
}
