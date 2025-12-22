using Blizzard.T5.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProgressScreen : MonoBehaviour
{
  public HeroCoin m_coinPrefab;
  public UberText m_lessonTitle;
  public UberText m_missionProgressTitle;
  public GameObject m_currentLessonBone;
  public PegUIElement m_exitButton;
  public UberText m_exitButtonLabel;
  private static TutorialProgressScreen s_instance;
  private List<HeroCoin> m_heroCoins = new List<HeroCoin>();
  private HeroCoin.CoinPressCallback m_coinPressCallback;
  private bool m_showProgressSavedMessage;
  private readonly Map<TutorialProgress, ScenarioDbId> m_progressToNextMissionIdMap = new Map<TutorialProgress, ScenarioDbId>()
  {
    {
      TutorialProgress.NOTHING_COMPLETE,
      ScenarioDbId.TUTORIAL_HOGGER
    },
    {
      TutorialProgress.HOGGER_COMPLETE,
      ScenarioDbId.TUTORIAL_MILLHOUSE
    },
    {
      TutorialProgress.MILLHOUSE_COMPLETE,
      ScenarioDbId.TUTORIAL_CHO
    },
    {
      TutorialProgress.CHO_COMPLETE,
      ScenarioDbId.TUTORIAL_MUKLA
    },
    {
      TutorialProgress.MUKLA_COMPLETE,
      ScenarioDbId.TUTORIAL_NESINGWARY
    },
    {
      TutorialProgress.NESINGWARY_COMPLETE,
      ScenarioDbId.TUTORIAL_ILLIDAN
    }
  };
  private readonly Map<ScenarioDbId, TutorialProgressScreen.LessonAsset> m_missionIdToLessonAssetMap = new Map<ScenarioDbId, TutorialProgressScreen.LessonAsset>()
  {
    {
      ScenarioDbId.TUTORIAL_HOGGER,
      (TutorialProgressScreen.LessonAsset) null
    },
    {
      ScenarioDbId.TUTORIAL_MILLHOUSE,
      new TutorialProgressScreen.LessonAsset()
      {
        m_asset = "Tutorial_Lesson1.prefab:51767358bb10afc4aac7ccb7a3b1e650"
      }
    },
    {
      ScenarioDbId.TUTORIAL_CHO,
      new TutorialProgressScreen.LessonAsset()
      {
        m_asset = "Tutorial_Lesson2.prefab:e97505bb5b8f67d409a10f827bd6043b",
        m_phoneAsset = "Tutorial_Lesson2_phone.prefab:be0cc750f6cbe4dc8b2606e5cb2249ed"
      }
    },
    {
      ScenarioDbId.TUTORIAL_MUKLA,
      new TutorialProgressScreen.LessonAsset()
      {
        m_asset = "Tutorial_Lesson3.prefab:cf99927ebaeabe14d862587afce9545a"
      }
    },
    {
      ScenarioDbId.TUTORIAL_NESINGWARY,
      new TutorialProgressScreen.LessonAsset()
      {
        m_asset = "Tutorial_Lesson4.prefab:caee3936e34d4e2469626c4e523f1b09"
      }
    },
    {
      ScenarioDbId.TUTORIAL_ILLIDAN,
      new TutorialProgressScreen.LessonAsset()
      {
        m_asset = "Tutorial_Lesson5.prefab:847a9f6b271b15e4ca5363dc29d2a590"
      }
    }
  };
  private List<ScenarioDbfRecord> m_sortedMissionRecords = new List<ScenarioDbfRecord>();
  private const float START_SCALE_VAL = 0.5f;
  private Vector3 START_SCALE = new Vector3(0.5f, 0.5f, 0.5f);
  private Vector3 FINAL_SCALE = (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(7f, 1f, 7f),
    Phone = new Vector3(6.1f, 1f, 6.1f)
  };
  private Vector3 FINAL_SCALE_OVER_BOX = (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(86.5f, 13f, 86.5f),
    Phone = new Vector3(47.5f, 8f, 47.5f)
  };
  private PlatformDependentValue<Vector3> FINAL_POS = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(-8f, 5f, -5f),
    Phone = new Vector3(-8f, 5f, -4.58f)
  };
  private PlatformDependentValue<Vector3> FINAL_POS_OVER_BOX = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(0.0f, 24.5f, -0.2f),
    Phone = new Vector3(0.0f, 21f, -2.06f)
  };
  private Vector3 HERO_COIN_START;
  private const float HERO_SPACING = -0.2f;
  private bool IS_TESTING;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    TutorialProgressScreen.s_instance = this;
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.VignettePerspective with
    {
      Time = 0.5f,
      EaseType = iTween.EaseType.easeInOutQuad,
      Vignette = new VignetteParameters(1f)
    });
    this.m_lessonTitle.Text = GameStrings.Get("TUTORIAL_PROGRESS_LESSON_TITLE");
    this.m_missionProgressTitle.Text = GameStrings.Get("TUTORIAL_PROGRESS_TITLE");
    this.m_exitButton.gameObject.SetActive(false);
    this.InitMissionRecords();
  }

  private void OnDestroy()
  {
    NetCache.Get()?.UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.UpdateProgress));
    TutorialProgressScreen.s_instance = (TutorialProgressScreen) null;
  }

  public static TutorialProgressScreen Get() => TutorialProgressScreen.s_instance;

  public void StartTutorialProgress()
  {
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
    {
      if (GameState.Get().GetFriendlySidePlayer().GetTag<TAG_PLAYSTATE>(GAME_TAG.PLAYSTATE) == TAG_PLAYSTATE.WON)
      {
        GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActorSpell(SpellType.ENDGAME_WIN).ActivateState(SpellStateType.DEATH);
        this.m_showProgressSavedMessage = true;
      }
      Gameplay.Get().RemoveGamePlayNameBannerPhone();
    }
    this.LoadAllTutorialHeroEntities();
  }

  public void SetCoinPressCallback(HeroCoin.CoinPressCallback callback)
  {
    if (callback == null)
      return;
    this.m_coinPressCallback = (HeroCoin.CoinPressCallback) (() =>
    {
      this.Hide();
      callback();
    });
  }

  private void InitMissionRecords()
  {
    foreach (ScenarioDbfRecord record in GameDbf.Scenario.GetRecords())
    {
      if (record.AdventureId == 1)
      {
        if (Enum.IsDefined(typeof (ScenarioDbId), (object) record.ID))
          this.m_sortedMissionRecords.Add(record);
      }
    }
    this.m_sortedMissionRecords.Sort(new Comparison<ScenarioDbfRecord>(GameUtils.MissionSortComparison));
  }

  private void LoadAllTutorialHeroEntities()
  {
    for (int index = 0; index < this.m_sortedMissionRecords.Count; ++index)
    {
      string missionHeroCardId = GameUtils.GetMissionHeroCardId(this.m_sortedMissionRecords[index].ID);
      if (DefLoader.Get().GetEntityDef(missionHeroCardId) == null)
        Debug.LogError((object) string.Format("TutorialProgress.OnTutorialHeroEntityDefLoaded() - failed to load {0}", (object) missionHeroCardId));
    }
    this.SetupCoins();
    this.Show();
  }

  private void SetupCoins()
  {
    this.HERO_COIN_START = new Vector3(0.5f, 0.1f, 0.32f);
    Vector3 vector3 = Vector3.zero;
    for (int index = 0; index < this.m_sortedMissionRecords.Count; ++index)
    {
      int id = this.m_sortedMissionRecords[index].ID;
      HeroCoin heroCoin = UnityEngine.Object.Instantiate<HeroCoin>(this.m_coinPrefab);
      heroCoin.transform.parent = this.transform;
      heroCoin.gameObject.SetActive(false);
      heroCoin.SetCoinPressCallback(this.m_coinPressCallback);
      Vector2 crackTexture;
      switch (UnityEngine.Random.Range(0, 3))
      {
        case 1:
          crackTexture = new Vector2(0.25f, -1f);
          break;
        case 2:
          crackTexture = new Vector2(0.5f, -1f);
          break;
        default:
          crackTexture = new Vector2(0.0f, -1f);
          break;
      }
      if (index == 0)
        heroCoin.transform.localPosition = this.HERO_COIN_START;
      else
        heroCoin.transform.localPosition = new Vector3(vector3.x - 0.2f, vector3.y, vector3.z);
      string lessonAsset1 = (string) null;
      TutorialProgressScreen.LessonAsset lessonAsset2;
      this.m_missionIdToLessonAssetMap.TryGetValue((ScenarioDbId) id, out lessonAsset2);
      if (lessonAsset2 != null)
        lessonAsset1 = !(bool) UniversalInputManager.UsePhoneUI || string.IsNullOrEmpty(lessonAsset2.m_phoneAsset) ? lessonAsset2.m_asset : lessonAsset2.m_phoneAsset;
      if (!string.IsNullOrEmpty(lessonAsset1))
        heroCoin.SetLessonAsset(lessonAsset1);
      this.m_heroCoins.Add(heroCoin);
      Vector2 goldTexture = Vector2.zero;
      Vector2 grayTexture = Vector2.zero;
      switch (id)
      {
        case 3:
          goldTexture = new Vector2(0.0f, -0.25f);
          grayTexture = new Vector2(0.25f, -0.25f);
          break;
        case 4:
          goldTexture = new Vector2(0.5f, 0.0f);
          grayTexture = new Vector2(0.75f, 0.0f);
          break;
        case 181:
          goldTexture = new Vector2(0.5f, -0.25f);
          grayTexture = new Vector2(0.75f, -0.25f);
          break;
        case 201:
          goldTexture = new Vector2(0.0f, 0.0f);
          grayTexture = new Vector2(0.25f, 0.0f);
          break;
        case 248:
          goldTexture = new Vector2(0.0f, -0.5f);
          grayTexture = new Vector2(0.25f, -0.5f);
          break;
        case 249:
          goldTexture = new Vector2(0.5f, -0.5f);
          grayTexture = new Vector2(0.75f, -0.5f);
          break;
      }
      heroCoin.SetCoinInfo(goldTexture, grayTexture, crackTexture, id);
      vector3 = heroCoin.transform.localPosition;
    }
    LayerUtils.SetLayer(this.gameObject, GameLayer.IgnoreFullScreenEffects);
  }

  private void Show()
  {
    iTween.FadeTo(this.gameObject, 1f, 0.25f);
    bool flag = SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY;
    this.transform.position = (Vector3) (flag ? this.FINAL_POS : this.FINAL_POS_OVER_BOX);
    this.transform.localScale = this.START_SCALE;
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) (flag ? this.FINAL_SCALE : this.FINAL_SCALE_OVER_BOX), (object) "time", (object) 0.5f, (object) "oncomplete", (object) "OnScaleAnimComplete", (object) "oncompletetarget", (object) this.gameObject));
  }

  private void Hide()
  {
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) this.START_SCALE, (object) "time", (object) 0.5f, (object) "oncomplete", (object) "OnHideAnimComplete", (object) "oncompletetarget", (object) this.gameObject));
    iTween.FadeTo(this.gameObject, iTween.Hash((object) "alpha", (object) 0.0f, (object) "time", (object) 0.25f, (object) "delay", (object) 0.25f));
  }

  private void OnScaleAnimComplete()
  {
    if (this.IS_TESTING)
      this.UpdateProgress();
    else
      NetCache.Get().RegisterTutorialEndGameScreen(new NetCache.NetCacheCallback(this.UpdateProgress), new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
    foreach (HeroCoin heroCoin in this.m_heroCoins)
      heroCoin.FinishIntroScaling();
  }

  private void OnHideAnimComplete() => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

  private void UpdateProgress()
  {
    ScenarioDbId nextMissionId;
    if (this.IS_TESTING)
      nextMissionId = this.m_progressToNextMissionIdMap[TutorialProgress.HOGGER_COMPLETE];
    else
      nextMissionId = this.m_progressToNextMissionIdMap[NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>().CampaignProgress];
    int index1 = this.m_heroCoins.FindIndex((Predicate<HeroCoin>) (coin => (ScenarioDbId) coin.GetMissionId() == nextMissionId));
    for (int index2 = 0; index2 < this.m_heroCoins.Count; ++index2)
    {
      HeroCoin heroCoin = this.m_heroCoins[index2];
      if (index2 == index1 - 1)
        this.StartCoroutine(this.SetActiveToDefeated(heroCoin));
      else if (index2 < index1)
        heroCoin.SetProgress(HeroCoin.CoinStatus.DEFEATED);
      else if (index2 == index1)
      {
        this.StartCoroutine(this.SetUnrevealedToActive(heroCoin));
        string lessonAsset = heroCoin.GetLessonAsset();
        if (!string.IsNullOrEmpty(lessonAsset))
          AssetLoader.Get().InstantiatePrefab((AssetReference) lessonAsset, new PrefabCallback<GameObject>(this.OnTutorialImageLoaded));
      }
      else
        heroCoin.SetProgress(HeroCoin.CoinStatus.UNREVEALED);
    }
    if (!this.m_showProgressSavedMessage)
      return;
    UIStatus.Get().AddInfo(GameStrings.Get("TUTORIAL_PROGRESS_SAVED"));
    this.m_showProgressSavedMessage = false;
  }

  private void OnTutorialImageLoaded(AssetReference assetRef, GameObject go, object callbackData) => this.SetupTutorialImage(go);

  private void SetupTutorialImage(GameObject go)
  {
    LayerUtils.SetLayer(go, GameLayer.IgnoreFullScreenEffects);
    go.transform.parent = this.m_currentLessonBone.transform;
    go.transform.localScale = Vector3.one;
    go.transform.localEulerAngles = Vector3.zero;
    go.transform.localPosition = Vector3.zero;
  }

  private IEnumerator SetActiveToDefeated(HeroCoin coin)
  {
    coin.SetProgress(HeroCoin.CoinStatus.ACTIVE);
    coin.m_inputEnabled = false;
    yield return (object) new WaitForSeconds(1f);
    coin.SetProgress(HeroCoin.CoinStatus.ACTIVE_TO_DEFEATED);
  }

  private IEnumerator SetUnrevealedToActive(HeroCoin coin)
  {
    coin.SetProgress(HeroCoin.CoinStatus.UNREVEALED);
    coin.m_inputEnabled = false;
    yield return (object) new WaitForSeconds(2f);
    coin.SetProgress(HeroCoin.CoinStatus.UNREVEALED_TO_ACTIVE);
  }

  private void ExitButtonPress(UIEvent e)
  {
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    this.m_screenEffectsHandle.StopEffect(0.5f, iTween.EaseType.easeInOutQuad);
  }

  private class LessonAsset
  {
    public string m_asset;
    public string m_phoneAsset;
  }
}
