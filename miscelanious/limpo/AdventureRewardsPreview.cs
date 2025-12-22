using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureRewardsPreview : MonoBehaviour
{
  [CustomEditField(Sections = "Cards Preview")]
  public GameObject m_CardsContainer;
  [SerializeField]
  private float m_CardWidth = 30f;
  [SerializeField]
  private float m_CardSpacing = 5f;
  [SerializeField]
  private float m_CardClumpAngleIncrement = 10f;
  [SerializeField]
  private Vector3 m_CardClumpSpacing = Vector3.zero;
  [CustomEditField(Sections = "Cards Preview")]
  public UberText m_HeaderTextObject;
  [CustomEditField(Sections = "Cards Preview")]
  public PegUIElement m_BackButton;
  [CustomEditField(Sections = "Cards Preview")]
  public GameObject m_ClickBlocker;
  [CustomEditField(Sections = "Cards Preview")]
  public UIBScrollable m_DisableScrollbar;
  [CustomEditField(Sections = "Cards Preview")]
  public float m_ShowHideAnimationTime = 0.15f;
  [CustomEditField(Sections = "Cards Preview")]
  public bool m_PreviewCardsExpandable;
  [CustomEditField(Sections = "Cards Preview/Hidden Cards")]
  public GameObject m_HiddenCardsLabelObject;
  [CustomEditField(Sections = "Cards Preview/Hidden Cards")]
  public UberText m_HiddenCardsLabel;
  [CustomEditField(Parent = "m_PreviewCardsExpandable", Sections = "Cards Preview")]
  public AdventureRewardsDisplayArea m_CardsPreviewDisplay;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_PreviewAppearSound;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_PreviewShrinkSound;
  private List<List<GameObject>> m_GameObjectBatches = new List<List<GameObject>>();
  private List<AdventureRewardsPreview.OnHide> m_OnHideListeners = new List<AdventureRewardsPreview.OnHide>();
  private int m_HiddenCardCount;
  private ScreenEffectsHandle m_screenEffectsHandle;

  [CustomEditField(Sections = "Cards Preview")]
  public float CardWidth
  {
    get => this.m_CardWidth;
    set
    {
      this.m_CardWidth = value;
      this.UpdateRewardPositions();
    }
  }

  [CustomEditField(Sections = "Cards Preview")]
  public float CardSpacing
  {
    get => this.m_CardSpacing;
    set
    {
      this.m_CardSpacing = value;
      this.UpdateRewardPositions();
    }
  }

  [CustomEditField(Sections = "Cards Preview")]
  public float CardClumpAngleIncrement
  {
    get => this.m_CardClumpAngleIncrement;
    set
    {
      this.m_CardClumpAngleIncrement = value;
      this.UpdateRewardPositions();
    }
  }

  [CustomEditField(Sections = "Cards Preview")]
  public Vector3 CardClumpSpacing
  {
    get => this.m_CardClumpSpacing;
    set
    {
      this.m_CardClumpSpacing = value;
      this.UpdateRewardPositions();
    }
  }

  private void Awake()
  {
    if ((UnityEngine.Object) this.m_BackButton != (UnityEngine.Object) null)
      this.m_BackButton.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => Navigation.GoBack()));
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  public void AddHideListener(AdventureRewardsPreview.OnHide dlg) => this.m_OnHideListeners.Add(dlg);

  public void RemoveHideListener(AdventureRewardsPreview.OnHide dlg) => this.m_OnHideListeners.Remove(dlg);

  private bool OnNavigateBack()
  {
    this.Show(false);
    return true;
  }

  public void SetHeaderText(string text) => this.m_HeaderTextObject.Text = GameStrings.Format("GLUE_ADVENTURE_REWARDS_PREVIEW_HEADER", (object) text);

  public void AddSpecificCards(List<string> cardIds)
  {
    foreach (string cardId in cardIds)
      this.AddCardBatch(new List<string>() { cardId });
  }

  public void AddSpecificCardBacks(List<int> cardBackIds)
  {
    foreach (int cardBackId in cardBackIds)
      this.AddCardBackBatch(new List<int>() { cardBackId });
  }

  public void AddSpecificBoosters(List<BoosterDbId> boosterIds)
  {
    foreach (BoosterDbId boosterId in boosterIds)
      this.AddBoosterBatch(new List<BoosterDbId>()
      {
        boosterId
      });
  }

  public void AddRewardBatch(int scenarioId) => this.AddRewardBatch(AdventureProgressMgr.Get().GetImmediateRewardsForDefeatingScenario(scenarioId));

  public void AddRewardBatch(List<RewardData> rewards)
  {
    List<string> cardIds = new List<string>();
    List<int> cardBackIds = new List<int>();
    List<BoosterDbId> boosterIds = new List<BoosterDbId>();
    foreach (RewardData reward in rewards)
    {
      switch (reward.RewardType)
      {
        case Reward.Type.BOOSTER_PACK:
          boosterIds.Add((BoosterDbId) ((BoosterPackRewardData) reward).Id);
          continue;
        case Reward.Type.CARD:
          cardIds.Add(((CardRewardData) reward).CardID);
          continue;
        case Reward.Type.CARD_BACK:
          cardBackIds.Add(((CardBackRewardData) reward).CardBackID);
          continue;
        case Reward.Type.RANDOM_CARD:
          Debug.LogWarning((object) "Random Card Rewards are not currently handled by adventure batch rewards.");
          continue;
        default:
          continue;
      }
    }
    this.AddCardBatch(cardIds);
    this.AddCardBackBatch(cardBackIds);
    this.AddBoosterBatch(boosterIds);
  }

  public void AddCardBatch(List<string> cardIds)
  {
    if (cardIds == null || cardIds.Count == 0)
      return;
    List<GameObject> cardBatch = new List<GameObject>();
    this.m_GameObjectBatches.Add(cardBatch);
    this.AddCardBatch(cardIds, cardBatch);
  }

  public void AddCardBackBatch(List<int> cardBackIds)
  {
    if (cardBackIds == null || cardBackIds.Count == 0)
      return;
    List<GameObject> cardBackBatch = new List<GameObject>();
    this.m_GameObjectBatches.Add(cardBackBatch);
    this.AddCardBackBatch(cardBackIds, cardBackBatch);
  }

  public void AddBoosterBatch(List<BoosterDbId> boosterIds)
  {
    if (boosterIds == null || boosterIds.Count == 0)
      return;
    List<GameObject> boosterBatch = new List<GameObject>();
    this.m_GameObjectBatches.Add(boosterBatch);
    this.AddBoosterBatch(boosterIds, boosterBatch);
  }

  public void SetHiddenCardCount(int hiddenCardCount) => this.m_HiddenCardCount = hiddenCardCount;

  public void Reset()
  {
    foreach (List<GameObject> gameObjectBatch in this.m_GameObjectBatches)
    {
      foreach (GameObject gameObject in gameObjectBatch)
      {
        if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) gameObject.gameObject);
      }
    }
    this.m_HiddenCardCount = 0;
    this.m_GameObjectBatches.Clear();
  }

  public void Show(bool show)
  {
    if ((UnityEngine.Object) this.m_ClickBlocker != (UnityEngine.Object) null)
      this.m_ClickBlocker.SetActive(show);
    if ((UnityEngine.Object) this.m_DisableScrollbar != (UnityEngine.Object) null)
      this.m_DisableScrollbar.Enable(!show);
    if (show)
    {
      this.UpdateRewardPositions();
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = this.m_ShowHideAnimationTime
      });
      this.gameObject.SetActive(true);
      iTween.ScaleFrom(this.gameObject, iTween.Hash((object) "scale", (object) (Vector3.one * 0.05f), (object) "time", (object) this.m_ShowHideAnimationTime));
      if (!string.IsNullOrEmpty(this.m_PreviewAppearSound))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_PreviewAppearSound);
      Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    }
    else
    {
      Vector3 origScale = this.transform.localScale;
      iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) (Vector3.one * 0.05f), (object) "time", (object) this.m_ShowHideAnimationTime, (object) "oncomplete", (object) (Action<object>) (o =>
      {
        this.gameObject.SetActive(false);
        this.transform.localScale = origScale;
        this.FireHideEvent();
      })));
      if (!string.IsNullOrEmpty(this.m_PreviewShrinkSound))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_PreviewShrinkSound);
      this.m_screenEffectsHandle.StopEffect();
    }
  }

  private void AddCardBatch(List<string> cardIds, List<GameObject> cardBatch)
  {
    if (cardIds == null || cardIds.Count == 0)
      return;
    for (int index = 0; index < cardIds.Count; ++index)
    {
      string cardId = cardIds[index];
      using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(cardId))
      {
        GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(fullDef.EntityDef, TAG_PREMIUM.NORMAL), AssetLoadingOptions.IgnorePrefabPosition);
        Actor actor = gameObject.GetComponent<Actor>();
        actor.SetFullDef(fullDef);
        actor.CreateBannedRibbon();
        GameUtils.SetParent((Component) actor, this.m_CardsContainer);
        LayerUtils.SetLayer((Component) actor, this.m_CardsContainer.gameObject.layer);
        cardBatch.Add(gameObject);
        if (this.m_PreviewCardsExpandable)
        {
          if ((UnityEngine.Object) this.m_CardsPreviewDisplay != (UnityEngine.Object) null)
          {
            PegUIElement pegUiElement = actor.m_cardMesh.gameObject.AddComponent<PegUIElement>();
            pegUiElement.GetComponent<Collider>().enabled = true;
            pegUiElement.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
            {
              if (this.m_CardsPreviewDisplay.IsShowing())
                return;
              this.m_CardsPreviewDisplay.ShowRewards(new List<RewardData>()
              {
                (RewardData) new CardRewardData(cardId, TAG_PREMIUM.NORMAL, 1)
              }, actor.transform.position, new Vector3?(actor.transform.position));
            }));
          }
        }
      }
    }
  }

  private void AddCardBackBatch(List<int> cardBackIds, List<GameObject> cardBackBatch)
  {
    if (cardBackIds == null || cardBackIds.Count == 0)
      return;
    foreach (int cardBackId in cardBackIds)
    {
      GameObject gameObject = CardBackManager.Get().LoadCardBackByIndex(cardBackId).m_GameObject;
      GameUtils.SetParent(gameObject, this.m_CardsContainer);
      LayerUtils.SetLayer(gameObject, this.m_CardsContainer.gameObject.layer);
      cardBackBatch.Add(gameObject);
    }
  }

  private void AddBoosterBatch(List<BoosterDbId> boosterIds, List<GameObject> boosterBatch)
  {
    if (boosterIds == null || boosterIds.Count == 0)
      return;
    foreach (BoosterDbId boosterId in boosterIds)
    {
      BoosterDbfRecord record = GameDbf.Booster.GetRecord((int) boosterId);
      if (record != null)
      {
        GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) record.PackOpeningPrefab, AssetLoadingOptions.IgnorePrefabPosition);
        gameObject.GetComponent<UnopenedPack>().m_SingleStack.m_RootObject.SetActive(true);
        GameUtils.SetParent(gameObject, this.m_CardsContainer);
        LayerUtils.SetLayer(gameObject, this.m_CardsContainer.gameObject.layer);
        boosterBatch.Add(gameObject);
      }
    }
  }

  private void UpdateRewardPositions()
  {
    int count = this.m_GameObjectBatches.Count;
    bool flag1 = this.m_HiddenCardCount > 0;
    bool flag2 = (UnityEngine.Object) this.m_HiddenCardsLabelObject != (UnityEngine.Object) null;
    if (flag1 & flag2)
      ++count;
    float num1 = (float) (((double) (count - 1) * (double) this.m_CardSpacing + (double) count * (double) this.m_CardWidth) * 0.5 - (double) this.m_CardWidth * 0.5);
    int num2 = 0;
    foreach (List<GameObject> gameObjectBatch in this.m_GameObjectBatches)
    {
      if (gameObjectBatch.Count != 0)
      {
        int num3 = 0;
        foreach (GameObject gameObject in gameObjectBatch)
        {
          if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) null))
          {
            Vector3 vector3 = this.m_CardClumpSpacing * (float) num3;
            vector3.x += (float) num2 * (this.m_CardSpacing + this.m_CardWidth) - num1;
            gameObject.transform.localScale = Vector3.one * 5f;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.Rotate(new Vector3(0.0f, 1f, 0.0f), (float) num3 * this.m_CardClumpAngleIncrement);
            gameObject.transform.localPosition = vector3;
            Actor component = gameObject.GetComponent<Actor>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            {
              component.SetUnlit();
              component.ContactShadow(true);
              component.UpdateAllComponents();
              component.Show();
            }
            ++num3;
          }
        }
        ++num2;
      }
    }
    if (flag1 & flag2)
    {
      Vector3 zero = Vector3.zero;
      zero.x += (float) num2 * (this.m_CardSpacing + this.m_CardWidth) - num1;
      this.m_HiddenCardsLabelObject.transform.localPosition = zero;
      this.m_HiddenCardsLabel.Text = string.Format("+{0}", (object) this.m_HiddenCardCount);
    }
    if (!flag2)
      return;
    this.m_HiddenCardsLabelObject.SetActive(flag1);
  }

  private void FireHideEvent()
  {
    foreach (AdventureRewardsPreview.OnHide onHide in this.m_OnHideListeners.ToArray())
      onHide();
  }

  public delegate void OnHide();
}
