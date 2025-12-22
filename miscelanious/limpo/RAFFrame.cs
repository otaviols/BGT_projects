using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAFFrame : UIBPopup
{
  private static RAFFrame s_Instance;
  public GeneralStoreHeroesContentDisplay m_heroDisplay;
  public UIBButton m_recruitFriendsButton;
  public HighlightState m_recruitFriendsButtonGlow;
  public UIBButton m_infoButton;
  public GameObject m_frame;
  public GameObject m_heroFrame;
  public GameObject m_progressFrame;
  public RAFLinkFrame m_linkFrame;
  public RAFInfo m_infoFrame;
  public List<RAFRecruitBar> m_recruitContainerList;
  public GameObject m_recruitCount;
  public UberText m_recruitCountText;
  public List<RAFChest> m_chestList;
  public GameObject m_heroRewardChestTooltip;
  public UberText m_heroRewardChestTooltipText;
  public Transform m_heroRewardChestTooltipHeroBone;
  public Transform m_heroRewardChestTooltipHeroPowerBone;
  public GameObject m_packRewardChestTooltip;
  public UberText m_packRewardChestTooltipText;
  public GameObject m_packRewardContainer;
  public UnopenedPack m_packReward;
  public GameObject m_totalResultLabel;
  public GameObject m_totalResult;
  public GameObject m_inputBlockerRenderer;
  private RAFChest m_heroChest;
  private Actor m_heroActor;
  private Actor m_heroPowerActor;
  private bool m_showHeroRewardChestTooltip;
  private PegUIElement m_inputBlockerPegUIElement;
  private bool m_isHeroDisplaySetup;
  private bool m_isHeroKeyArtShowing = true;
  private CollectionHeroDef m_collectionHeroDef;
  private MusicPlaylistType m_prevMusicPlaylist;
  private RAFFrame.Display m_shownDisplay;
  private ScreenEffectsHandle m_screenEffectsHandle;

  protected override void Awake()
  {
    base.Awake();
    RAFFrame.s_Instance = this;
    this.m_recruitFriendsButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRecruitFriendsButtonReleased));
    this.m_infoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInfoButtonReleased));
    this.m_heroDisplay.m_previewToggle.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnHeroPreviewToggle));
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected override void Start()
  {
    base.Start();
    this.m_heroChest = this.m_chestList[0];
    this.m_heroChest.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ShowHeroRewardTooltip));
    this.m_heroChest.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.HideHeroRewardTooltip));
    for (int index = 1; index < this.m_chestList.Count; ++index)
    {
      this.m_chestList[index].AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ShowPackRewardTooltip));
      this.m_chestList[index].AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.HidePackRewardTooltip));
    }
    this.m_packReward.SetCount(1);
    if (this.m_shownDisplay == RAFFrame.Display.NONE)
      this.ShowHeroFrame();
    this.UpdateRecruitFriendsButtonGlow();
    BnetBar bnetBar = BnetBar.Get();
    if (!((UnityEngine.Object) bnetBar != (UnityEngine.Object) null))
      return;
    bnetBar.OnMenuOpened += new Action(this.OnMenuOpened);
  }

  private void OnDestroy()
  {
    this.Hide(true);
    BnetBar bnetBar = BnetBar.Get();
    if ((UnityEngine.Object) bnetBar != (UnityEngine.Object) null)
      bnetBar.OnMenuOpened -= new Action(this.OnMenuOpened);
    RAFFrame.s_Instance = (RAFFrame) null;
  }

  public static RAFFrame Get() => RAFFrame.s_Instance;

  public override void Show()
  {
    if (this.IsShown())
      return;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
    {
      Time = 0.1f
    });
    if ((bool) UniversalInputManager.UsePhoneUI)
      BnetBar.Get().HideCurrencyFrames();
    this.transform.parent = BaseUI.Get().transform;
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.gameObject.layer);
    if ((UnityEngine.Object) this.m_inputBlockerPegUIElement != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_inputBlockerPegUIElement.gameObject);
      this.m_inputBlockerPegUIElement = (PegUIElement) null;
    }
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(firstByLayer, "RAFInputBlocker");
    LayerUtils.SetLayer(inputBlocker, this.gameObject.layer);
    this.m_inputBlockerPegUIElement = inputBlocker.AddComponent<PegUIElement>();
    this.m_inputBlockerPegUIElement.transform.parent = this.transform;
    this.m_inputBlockerPegUIElement.transform.localPosition = new Vector3(0.0f, -1f, 0.0f);
    this.m_inputBlockerPegUIElement.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerRelease));
    this.Show(false);
    TransformUtil.SetPosY((Component) this, this.transform.position.y + 100f);
    Options.Get().SetBool(Option.HAS_SEEN_RAF, true);
    FriendListFrame friendListFrame = ChatMgr.Get().FriendListFrame;
    if (!((UnityEngine.Object) friendListFrame != (UnityEngine.Object) null))
      return;
    friendListFrame.UpdateRAFButtonGlow();
  }

  protected override void Hide(bool animate)
  {
    if (!this.IsShown())
      return;
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    if ((UnityEngine.Object) this.m_linkFrame != (UnityEngine.Object) null)
      this.m_linkFrame.Hide();
    if ((UnityEngine.Object) this.m_inputBlockerPegUIElement != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_inputBlockerPegUIElement.gameObject);
      this.m_inputBlockerPegUIElement = (PegUIElement) null;
    }
    this.m_screenEffectsHandle.StopEffect();
    this.m_heroDisplay.ResetPreview();
    this.m_isHeroKeyArtShowing = true;
    this.StopHeroMusic();
    if ((bool) UniversalInputManager.UsePhoneUI)
      BnetBar.Get().RefreshCurrency();
    base.Hide(animate);
  }

  public void ShowProgressFrame()
  {
    this.m_heroFrame.SetActive(false);
    this.m_progressFrame.SetActive(true);
    if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
      AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d", new PrefabCallback<GameObject>(this.OnHeroActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null)
      AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Play_HeroPower.prefab:a3794839abb947146903a26be13e09af", new PrefabCallback<GameObject>(this.OnHeroPowerActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    this.m_shownDisplay = RAFFrame.Display.PROGRESS;
  }

  public void ShowHeroFrame()
  {
    this.m_heroFrame.SetActive(true);
    if (!this.m_isHeroDisplaySetup)
    {
      this.m_heroDisplay.SetKeyArtRenderer(this.m_heroDisplay.m_parentLite.m_renderQuad);
      this.m_heroDisplay.m_parentLite.m_renderToTexture.GetComponent<RenderToTexture>().m_RenderToObject = this.m_heroDisplay.m_renderArtQuad;
      CardHeroDbfRecord record = GameDbf.CardHero.GetRecord(17);
      using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(record.CardId))
        this.m_collectionHeroDef = GameUtils.LoadGameObjectWithComponent<CollectionHeroDef>(cardDef.CardDef.m_CollectionHeroDefPath);
      this.m_heroDisplay.UpdateFrame(record, 0, this.m_collectionHeroDef);
      this.m_isHeroDisplaySetup = true;
    }
    else
      this.m_heroDisplay.ResetPreview();
    this.m_progressFrame.SetActive(false);
    this.m_shownDisplay = RAFFrame.Display.HERO;
  }

  public void ResetProgressFrame()
  {
    foreach (RAFRecruitBar recruitContainer in this.m_recruitContainerList)
      recruitContainer.SetLocked(true);
    this.m_recruitCount.SetActive(false);
    this.m_totalResultLabel.SetActive(false);
    this.m_totalResult.SetActive(false);
    foreach (RAFChest chest in this.m_chestList)
      chest.SetOpen(false);
  }

  public void UpdateRecruitFriendsButtonGlow() => this.m_recruitFriendsButtonGlow.ChangeState(Options.Get().GetBool(Option.HAS_SEEN_RAF_RECRUIT_URL) ? ActorStateType.NONE : ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);

  public void SetProgress(int numRecruits)
  {
    this.ResetProgressFrame();
    for (int index = 0; index < numRecruits && index < 5; ++index)
    {
      RAFRecruitBar recruitContainer = this.m_recruitContainerList[index];
      recruitContainer.SetLocked(false);
      recruitContainer.SetBattleTag("GoodKnight#1234");
      recruitContainer.SetLevel(20);
      this.m_chestList[index].SetOpen(true);
    }
    if (numRecruits <= 5)
      return;
    this.m_recruitCount.gameObject.SetActive(true);
    int num = numRecruits - 5;
    this.m_recruitCountText.Text = GameStrings.Format("GLUE_RAF_PROGRESS_FRAME_RECRUIT_COUNT", (object) num, (object) num);
  }

  public void SetProgressData(uint totalRecruitCount, List<RAFManager.RecruitData> topRecruits)
  {
    this.ResetProgressFrame();
    if (totalRecruitCount == 0U)
    {
      Log.RAF.PrintError("SetProgressData() - totalRecruitCount is 0!");
      this.ShowHeroFrame();
    }
    else if (topRecruits == null)
    {
      Log.RAF.PrintError("SetProgressData() - topRecruits is NULL!");
      this.ShowHeroFrame();
    }
    else
    {
      for (int index = 0; index < topRecruits.Count; ++index)
      {
        RAFRecruitBar recruitContainer = this.m_recruitContainerList[index];
        recruitContainer.SetLocked(false);
        RAFManager.RecruitData topRecruit = topRecruits[index];
        string battleTag = topRecruit.m_recruitBattleTag == null ? GameStrings.Get("GAMEPLAY_UNKNOWN_OPPONENT_NAME") : topRecruit.m_recruitBattleTag;
        int progress = (int) topRecruit.m_recruit.Progress;
        recruitContainer.SetGameAccountId(topRecruit.m_recruit.GameAccountId);
        recruitContainer.SetBattleTag(battleTag);
        recruitContainer.SetLevel(progress);
        if (progress >= 20)
          this.m_chestList[index].SetOpen(true);
      }
      if (totalRecruitCount <= 5U)
        return;
      this.m_recruitCount.gameObject.SetActive(true);
      int num = (int) totalRecruitCount - 5;
      this.m_recruitCountText.Text = GameStrings.Format("GLUE_RAF_PROGRESS_FRAME_RECRUIT_COUNT", (object) num, (object) num);
    }
  }

  public void UpdateBattleTag(BnetId gameAccountId, string battleTag)
  {
    foreach (RAFRecruitBar recruitContainer in this.m_recruitContainerList)
    {
      if (recruitContainer.GetGameAccountId() == gameAccountId)
      {
        recruitContainer.SetBattleTag(battleTag);
        break;
      }
    }
  }

  public void ShowLinkFrame(string displayURL, string fullURL)
  {
    Options.Get().SetBool(Option.HAS_SEEN_RAF_RECRUIT_URL, true);
    this.UpdateRecruitFriendsButtonGlow();
    this.m_linkFrame.SetURL(displayURL, fullURL);
    this.m_linkFrame.Show();
  }

  public void DarkenInputBlocker(GameObject inputBlockerObject, float alpha)
  {
    RendererExtension.SetMaterial((Renderer) inputBlockerObject.AddComponent<MeshRenderer>(), RendererExtension.GetMaterial((Renderer) this.m_inputBlockerRenderer.GetComponent<MeshRenderer>()));
    inputBlockerObject.AddComponent<MeshFilter>().SetMesh(this.m_inputBlockerRenderer.GetComponent<MeshFilter>().GetMesh());
    BoxCollider component = inputBlockerObject.GetComponent<BoxCollider>();
    TransformUtil.SetLocalScaleXY(inputBlockerObject, component.size.x, component.size.y);
    component.size = new Vector3(1f, 1f, 0.0f);
    TransformUtil.SetLocalEulerAngleX(inputBlockerObject, 90f);
    RenderUtils.SetAlpha(inputBlockerObject, alpha);
  }

  private bool OnNavigateBack()
  {
    this.Hide(true);
    return true;
  }

  private void OnInputBlockerRelease(UIEvent e) => this.Hide(true);

  private void OnRecruitFriendsButtonReleased(UIEvent e)
  {
    if (this.m_infoFrame.gameObject.activeInHierarchy)
      return;
    string recruitDisplayUrl = RAFManager.Get().GetRecruitDisplayURL();
    if (recruitDisplayUrl == null)
      return;
    string recruitFullUrl = RAFManager.Get().GetRecruitFullURL();
    this.ShowLinkFrame(recruitDisplayUrl, recruitFullUrl);
  }

  private void OnInfoButtonReleased(UIEvent e) => this.m_infoFrame.Show();

  private void OnHeroPreviewToggle(UIEvent e)
  {
    this.m_isHeroKeyArtShowing = !this.m_isHeroKeyArtShowing;
    if (this.m_isHeroKeyArtShowing)
      this.StopHeroMusic();
    else
      this.PlayHeroMusic();
  }

  private void PlayHeroMusic()
  {
    if ((UnityEngine.Object) this.m_collectionHeroDef == (UnityEngine.Object) null)
    {
      Log.RAF.PrintWarning("RAFFrame.PlayHeroMusic - m_collectionHeroDef is NULL!");
    }
    else
    {
      MusicPlaylistType heroPlaylist = this.m_collectionHeroDef.m_heroPlaylist;
      if (heroPlaylist == MusicPlaylistType.Invalid)
        return;
      this.m_prevMusicPlaylist = MusicManager.Get().GetCurrentPlaylist();
      MusicManager.Get().StartPlaylist(heroPlaylist);
    }
  }

  private void StopHeroMusic()
  {
    if (this.m_prevMusicPlaylist == MusicPlaylistType.Invalid)
      return;
    MusicManager.Get().StartPlaylist(this.m_prevMusicPlaylist);
    this.m_prevMusicPlaylist = MusicPlaylistType.Invalid;
  }

  private void ShowHeroRewardTooltip(UIEvent e)
  {
    this.m_showHeroRewardChestTooltip = true;
    this.m_heroRewardChestTooltipText.Text = GameStrings.Get(this.m_heroChest.IsOpen() ? "GLUE_RAF_HERO_TOOLTIP_REDEEMED_TITLE" : "GLUE_RAF_HERO_TOOLTIP_TITLE");
    this.StartCoroutine(this.ShowHeroRewardTooltipWhenReady());
  }

  private IEnumerator ShowHeroRewardTooltipWhenReady()
  {
    while ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
      yield return (object) null;
    if (this.m_showHeroRewardChestTooltip)
      this.m_heroRewardChestTooltip.SetActive(true);
  }

  private void HideHeroRewardTooltip(UIEvent e)
  {
    this.m_showHeroRewardChestTooltip = false;
    this.m_heroRewardChestTooltip.SetActive(false);
  }

  private void ShowPackRewardTooltip(UIEvent e)
  {
    RAFChest element = e.GetElement() as RAFChest;
    this.m_packRewardChestTooltipText.Text = GameStrings.Get(element.IsOpen() ? "GLUE_RAF_PACK_TOOLTIP_REDEEMED_TITLE" : "GLUE_RAF_PACK_TOOLTIP_TITLE");
    this.m_packRewardChestTooltip.transform.position = element.m_tooltipBone.transform.position;
    this.m_packRewardChestTooltip.SetActive(true);
  }

  private void HidePackRewardTooltip(UIEvent e) => this.m_packRewardChestTooltip.SetActive(false);

  private void OnHeroActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Log.RAF.PrintWarning(string.Format("RAFFrame.OnHeroActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_heroActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
      {
        Log.RAF.PrintWarning(string.Format("RAFFrame.OnHeroActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        go.transform.parent = this.m_heroRewardChestTooltip.transform;
        go.transform.localScale = this.m_heroRewardChestTooltipHeroBone.localScale;
        go.transform.localPosition = this.m_heroRewardChestTooltipHeroBone.localPosition;
        this.m_heroActor.SetUnlit();
        LayerUtils.SetLayer(this.m_heroActor.gameObject, this.gameObject.layer);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_heroActor.m_healthObject);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_heroActor.m_attackObject);
        this.m_heroActor.Hide();
        string cardIdFromHeroDbId = GameUtils.GetCardIdFromHeroDbId(17);
        DefLoader.Get().LoadFullDef(cardIdFromHeroDbId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroFullDefLoaded));
      }
    }
  }

  private void OnHeroPowerActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("RAFFrame.OnHeroActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_heroPowerActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("RAFFrame.OnHeroActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        go.transform.parent = this.m_heroRewardChestTooltip.transform;
        go.transform.localScale = this.m_heroRewardChestTooltipHeroPowerBone.localScale;
        go.transform.localPosition = this.m_heroRewardChestTooltipHeroPowerBone.localPosition;
        this.m_heroPowerActor.SetUnlit();
        LayerUtils.SetLayer(this.m_heroPowerActor.gameObject, this.gameObject.layer);
        this.m_heroPowerActor.Hide();
        string powerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(GameDbf.CardHero.GetRecord(17).CardId);
        DefLoader.Get().LoadFullDef(powerCardIdFromHero, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroPowerFullDefLoaded));
      }
    }
  }

  private void OnHeroFullDefLoaded(
    string cardId,
    DefLoader.DisposableFullDef fullDef,
    object userData)
  {
    using (fullDef)
    {
      this.m_heroActor.SetPremium(TAG_PREMIUM.GOLDEN);
      this.m_heroActor.SetEntityDef(fullDef.EntityDef);
      this.m_heroActor.SetCardDef(fullDef.DisposableCardDef);
      this.m_heroActor.UpdateAllComponents();
      this.m_heroActor.SetUnlit();
      this.m_heroActor.transform.Rotate(new Vector3(-90f, 0.0f, 0.0f));
      this.m_heroActor.Show();
    }
  }

  private void OnHeroPowerFullDefLoaded(
    string cardId,
    DefLoader.DisposableFullDef def,
    object userData)
  {
    using (def)
    {
      this.m_heroPowerActor.SetCardDef(def.DisposableCardDef);
      this.m_heroPowerActor.SetEntityDef(def.EntityDef);
      this.m_heroPowerActor.UpdateAllComponents();
      this.m_heroPowerActor.SetUnlit();
      def.CardDef.m_AlwaysRenderPremiumPortrait = false;
      this.m_heroPowerActor.UpdateMaterials();
      this.m_heroPowerActor.transform.Rotate(new Vector3(-90f, 0.0f, 0.0f));
      this.m_heroPowerActor.Show();
      this.StartCoroutine(this.UpdateHeroSkinHeroPower());
    }
  }

  private IEnumerator UpdateHeroSkinHeroPower()
  {
    while ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
      yield return (object) null;
    while (!this.m_heroActor.HasCardDef)
      yield return (object) null;
    HeroSkinHeroPower componentInChildren = this.m_heroPowerActor.gameObject.GetComponentInChildren<HeroSkinHeroPower>();
    if (!((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null))
    {
      string cardId = this.m_heroActor.GetEntityDef().GetCardId();
      componentInChildren.m_Actor.AlwaysRenderPremiumPortrait = !GameUtils.IsVanillaHero(cardId);
      componentInChildren.m_Actor.UpdateMaterials();
    }
  }

  private void OnMenuOpened()
  {
    if (!this.m_shown)
      return;
    this.Hide(false);
  }

  private enum Display
  {
    NONE,
    HERO,
    PROGRESS,
  }
}
