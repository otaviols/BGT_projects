using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using System.Collections;
using UnityEngine;

public class NameBanner : MonoBehaviour
{
  private const float SKINNED_BANNER_STANDARD_OFFSET = 1f;
  private const float SKINNED_BANNER_MEDAL_OFFSET = 10f;
  private const float SKINNED_BANNER_MIN_SIZE = 12f;
  private const float SKINNED_MEDAL_BANNER_MIN_SIZE = 17f;
  private const float SKINNED_GAME_ICON_BANNER_MIN_SIZE = 17f;
  public GameObject m_alphaBannerSkinned;
  public GameObject m_alphaBannerBone;
  public GameObject m_medalBannerSkinned;
  public GameObject m_medalBannerBone;
  public GameObject m_modeIconBannerSkinned;
  public GameObject m_modeIconBannerBone;
  public GameObject m_alphaBanner;
  public GameObject m_alphaBannerLeft;
  public GameObject m_alphaBannerMiddle;
  public GameObject m_alphaBannerRight;
  public GameObject m_medalAlphaBanner;
  public GameObject m_medalAlphaBannerLeft;
  public GameObject m_medalAlphaBannerMiddle;
  public GameObject m_medalAlphaBannerRight;
  public bool m_canShowModeIcons;
  public bool m_isGameplayNameBanner;
  public GameModeIcon m_casualStandardGameModeIcon;
  public GameModeIcon m_casualWildGameModeIcon;
  public GameModeIcon m_arenaGameModeIcon;
  public GameModeIcon m_adventureGameModeIcon;
  public GameObject m_adventureIcon;
  public GameObject m_adventureShadow;
  public GameModeIcon m_friendlyGameModeIcon;
  public TavernBrawlGameModeIcon m_tavernBrawlGameModeIcon;
  public GameModeIcon m_heroicSessionBasedTavernBrawlIcon;
  public TavernBrawlGameModeIcon m_normalSessionBasedTavernBrawlIcon;
  public GameModeIcon m_pvpdrGameModeIcon;
  public GameObject m_nameText;
  public GameObject m_longNameText;
  public Transform m_nameBone;
  public Transform m_classBone;
  public Transform m_longNameBone;
  public Transform m_longClassBone;
  public Transform m_medalNameBone;
  public Transform m_medalClassBone;
  public Transform m_longMedalNameBone;
  public Transform m_longMedalClassBone;
  public AsyncReference m_rankedMedalWidgetReference;
  public UberText m_playerName;
  public UberText m_subtext;
  public UberText m_longPlayerName;
  public UberText m_longSubtext;
  public float FUDGE_FACTOR = 0.1915f;
  private const float MARGIN_FACTOR = 0.1562f;
  private int m_playerId;
  private Player.Side m_playerSide;
  private const float UNKNOWN_NAME_WAIT = 5f;
  private const float RANK_WAIT = 5f;
  private Transform m_nameBoneToUse;
  private Transform m_classBoneToUse;
  private UberText m_currentPlayerName;
  private UberText m_currentSubtext;
  private int m_missionId;
  private bool m_useLongName;
  private bool m_shouldCenterName = true;
  private FormatType m_formatType;
  private bool m_shouldShowRankedMedal;
  private bool m_initialized;
  private MedalInfoTranslator m_medalInfo;
  private RankedMedal m_rankedMedal;
  private RankedPlayDataModel m_rankedDataModel;
  private Widget m_medalWidget;
  private AssetHandle<Texture> m_gameModeIconTexture;

  private void Update() => this.UpdateAnchor();

  private void OnDestroy() => AssetHandle.SafeDispose<Texture>(ref this.m_gameModeIconTexture);

  public bool IsWaitingForMedal
  {
    get
    {
      if (!this.m_shouldShowRankedMedal)
        return false;
      return this.m_medalInfo == null || (UnityEngine.Object) this.m_medalWidget == (UnityEngine.Object) null || this.m_medalWidget.IsChangingStates;
    }
  }

  public void SetName(string name)
  {
    this.m_currentPlayerName.Text = name;
    if ((UnityEngine.Object) this.m_alphaBannerSkinned != (UnityEngine.Object) null)
      this.AdjustSkinnedBanner();
    else
      this.AdjustBanner();
  }

  private void SetMobilePositionOffset()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    TransformUtil.SetLocalPosX(this.gameObject, this.gameObject.transform.localPosition.x + BaseUI.Get().m_BnetBar.HorizontalMargin * 0.1562f);
  }

  private void AdjustBanner()
  {
    Vector3 worldScale = TransformUtil.ComputeWorldScale(this.m_currentPlayerName.gameObject);
    float num1 = this.FUDGE_FACTOR * worldScale.x * this.m_currentPlayerName.GetTextWorldSpaceBounds().size.x;
    float num2 = this.m_currentPlayerName.GetTextWorldSpaceBounds().size.x * worldScale.x + num1;
    float x1 = this.m_alphaBannerMiddle.GetComponent<Renderer>().bounds.size.x;
    float x2 = this.m_currentPlayerName.GetTextBounds().size.x;
    if ((UnityEngine.Object) this.m_medalAlphaBannerMiddle != (UnityEngine.Object) null)
    {
      MeshRenderer componentInChildren = this.m_medalAlphaBannerMiddle.GetComponentInChildren<MeshRenderer>(true);
      float x3 = componentInChildren.bounds.size.x;
      if ((double) num2 <= (double) x1)
        return;
      if (this.m_shouldShowRankedMedal)
      {
        TransformUtil.SetLocalScaleX(this.m_medalAlphaBannerMiddle, x2 / x3);
        TransformUtil.SetPoint(this.m_medalAlphaBannerRight, Anchor.LEFT, componentInChildren.gameObject, Anchor.RIGHT, new Vector3(0.0f, 0.0f, 0.0f));
      }
      else
      {
        TransformUtil.SetLocalScaleX(this.m_alphaBannerMiddle, num2 / x1);
        TransformUtil.SetPoint(this.m_alphaBannerRight, Anchor.LEFT, this.m_alphaBannerMiddle, Anchor.RIGHT, new Vector3(-num1, 0.0f, 0.0f));
      }
    }
    else
    {
      if ((double) num2 <= (double) x1)
        return;
      TransformUtil.SetLocalScaleX(this.m_alphaBanner, num2 / x1);
    }
  }

  private void AdjustSkinnedBanner()
  {
    bool flag = false;
    if (!this.m_shouldShowRankedMedal && this.ShouldShowGameIconBanner())
      flag = true;
    if ((UnityEngine.Object) this.m_currentPlayerName == (UnityEngine.Object) null)
      return;
    UberText currentPlayerName = this.m_currentPlayerName;
    if (this.m_shouldShowRankedMedal)
    {
      float x = (float) (-(double) currentPlayerName.GetTextBounds().size.x - 10.0);
      if ((double) x > -17.0)
        x = -17f;
      if (!((UnityEngine.Object) this.m_medalBannerBone != (UnityEngine.Object) null))
        return;
      Vector3 localPosition = this.m_medalBannerBone.transform.localPosition;
      this.m_medalBannerBone.transform.localPosition = new Vector3(x, localPosition.y, localPosition.z);
    }
    else if (flag)
    {
      float x = (float) (-(double) currentPlayerName.GetTextBounds().size.x - 10.0);
      if ((double) x > -17.0)
        x = -17f;
      if (!((UnityEngine.Object) this.m_modeIconBannerBone != (UnityEngine.Object) null))
        return;
      Vector3 localPosition = this.m_modeIconBannerBone.transform.localPosition;
      this.m_modeIconBannerBone.transform.localPosition = new Vector3(x, localPosition.y, localPosition.z);
    }
    else
    {
      float x = (float) (-(double) currentPlayerName.GetTextBounds().size.x - 1.0);
      if ((double) x > -12.0)
        x = -12f;
      if (!((UnityEngine.Object) this.m_alphaBannerBone != (UnityEngine.Object) null))
        return;
      Vector3 localPosition = this.m_alphaBannerBone.transform.localPosition;
      this.m_alphaBannerBone.transform.localPosition = new Vector3(x, localPosition.y, localPosition.z);
    }
  }

  public void SetSubtext(string subtext)
  {
    if ((UnityEngine.Object) this.m_currentSubtext != (UnityEngine.Object) null)
    {
      this.m_currentSubtext.gameObject.SetActive(true);
      this.m_currentSubtext.Text = subtext;
    }
    if (!((UnityEngine.Object) this.m_currentPlayerName != (UnityEngine.Object) null))
      return;
    this.m_currentPlayerName.transform.localPosition = (UnityEngine.Object) this.m_classBoneToUse == (UnityEngine.Object) null ? this.m_nameBoneToUse.localPosition : this.m_classBoneToUse.localPosition;
  }

  public void PositionNameText(bool shouldTween)
  {
    if (!this.m_shouldCenterName || (UnityEngine.Object) this.m_currentPlayerName == (UnityEngine.Object) null)
      return;
    if ((bool) UniversalInputManager.UsePhoneUI || !shouldTween)
      this.m_currentPlayerName.transform.position = this.m_nameBoneToUse.position;
    else
      iTween.MoveTo(this.m_currentPlayerName.gameObject, iTween.Hash((object) "position", (object) this.m_nameBoneToUse.localPosition, (object) "isLocal", (object) true, (object) "time", (object) 1f));
  }

  public void PositionNameText_Reconnect()
  {
    if ((UnityEngine.Object) this.m_currentPlayerName == (UnityEngine.Object) null)
      return;
    this.m_currentPlayerName.transform.position = this.m_nameBoneToUse.position;
    this.OnSubtextFadeComplete();
  }

  public void FadeOutSubtext()
  {
    if ((UnityEngine.Object) this.m_currentSubtext == (UnityEngine.Object) null)
      return;
    if (this.m_playerSide == Player.Side.OPPOSING & (GameUtils.IsExpansionAdventure(GameUtils.GetAdventureId(this.m_missionId)) && !GameUtils.IsClassChallengeMission(this.m_missionId)))
      this.m_shouldCenterName = false;
    else if (this.m_playerSide == Player.Side.FRIENDLY && !string.IsNullOrEmpty(GameState.Get().GetGameEntity().GetAlternatePlayerName()))
    {
      if ((UnityEngine.Object) this.m_adventureGameModeIcon != (UnityEngine.Object) null)
        this.m_adventureGameModeIcon.Show(false);
      iTween.FadeTo(this.gameObject, 0.0f, 1f);
    }
    else
      iTween.FadeTo(this.m_currentSubtext.gameObject, iTween.Hash((object) "alpha", (object) 0.0f, (object) "time", (object) 1f, (object) "oncomplete", (object) "OnSubtextFadeComplete", (object) "oncompletetarget", (object) this.gameObject));
  }

  public void OnSubtextFadeComplete() => this.m_currentSubtext.gameObject.SetActive(false);

  public void FadeIn()
  {
    if ((UnityEngine.Object) this.m_alphaBannerSkinned != (UnityEngine.Object) null)
      iTween.FadeTo(this.m_alphaBannerSkinned.gameObject, 1f, 1f);
    else
      iTween.FadeTo(this.m_alphaBanner.gameObject, 1f, 1f);
    iTween.FadeTo(this.m_currentPlayerName.gameObject, 1f, 1f);
  }

  public void Initialize(Player.Side side)
  {
    this.m_playerSide = side;
    this.m_currentPlayerName = this.m_playerName;
    this.m_currentSubtext = this.m_subtext;
    this.m_nameText.SetActive(true);
    this.m_useLongName = false;
    this.m_playerName.Text = string.Empty;
    this.m_nameBoneToUse = this.m_nameBone;
    if ((bool) (UnityEngine.Object) this.m_longNameText)
      this.m_longNameText.SetActive(false);
    this.m_missionId = GameMgr.Get().GetMissionId();
    this.m_formatType = GameMgr.Get().GetFormatType();
    this.m_shouldShowRankedMedal = GameUtils.IsGameTypeRanked();
    this.m_initialized = true;
    this.UpdateAnchor();
    if (!GameState.Get().GetBooleanGameOption(GameEntityOption.ALLOW_NAME_BANNER_MODE_ICONS))
      this.m_canShowModeIcons = false;
    if (!this.m_canShowModeIcons)
      this.m_shouldShowRankedMedal = false;
    if (!this.m_shouldShowRankedMedal)
      return;
    this.StartCoroutine(this.UpdateMedalWhenReady());
    this.m_rankedMedalWidgetReference.RegisterReadyListener<Widget>(new System.Action<Widget>(this.OnRankedMedalWidgetReady));
  }

  public void Show()
  {
    if (this.m_playerSide == Player.Side.OPPOSING && GameState.Get().GetBooleanGameOption(GameEntityOption.DISABLE_OPPONENT_NAME_BANNER))
      this.gameObject.SetActive(false);
    else
      this.StartCoroutine(this.UpdateName());
  }

  public Player.Side GetPlayerSide() => this.m_playerSide;

  public void Unload() => UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.gameObject);

  public void UseLongName()
  {
    this.m_currentPlayerName = this.m_longPlayerName;
    this.m_currentSubtext = this.m_longSubtext;
    this.m_longNameText.SetActive(true);
    this.m_nameText.SetActive(false);
    this.m_useLongName = true;
  }

  public void UpdateHeroNameBanner() => this.SetName(GameState.Get().GetPlayer(this.m_playerId).GetHero().GetName());

  public void UpdatePlayerNameBanner() => this.SetName(GameState.Get().GetPlayer(this.m_playerId).GetName());

  public void UpdateSubtext() => this.StartCoroutine(this.UpdateSubtextImpl());

  private void UpdateAnchor()
  {
    if (!this.m_initialized)
      return;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if (this.m_playerSide == Player.Side.FRIENDLY)
        OverlayUI.Get().AddGameObject(this.gameObject, CanvasAnchor.BOTTOM_RIGHT);
      else
        OverlayUI.Get().AddGameObject(this.gameObject, CanvasAnchor.BOTTOM_LEFT);
    }
    else
    {
      if (this.m_playerSide == Player.Side.FRIENDLY)
        OverlayUI.Get().AddGameObject(this.gameObject, CanvasAnchor.BOTTOM_LEFT);
      else
        OverlayUI.Get().AddGameObject(this.gameObject, CanvasAnchor.TOP_LEFT);
      this.transform.localPosition = GameState.Get().GetGameEntity().NameBannerPosition(this.m_playerSide);
    }
  }

  private static Player GetPlayerForSide(Player.Side side)
  {
    if (side == Player.Side.FRIENDLY)
      return GameState.Get().GetLocalSidePlayer();
    return side == Player.Side.OPPOSING ? GameState.Get().GetOpposingPlayer() : (Player) null;
  }

  private IEnumerator UpdateName()
  {
    while (GameState.Get().GetPlayerMap().Count == 0)
      yield return (object) null;
    Player p = (Player) null;
    while (p == null)
    {
      p = NameBanner.GetPlayerForSide(this.m_playerSide);
      yield return (object) null;
    }
    this.m_playerId = p.GetPlayerId();
    string name = p.GetName();
    if (p.IsHuman() && Options.Get().GetBool(Option.STREAMER_MODE) && !SpectatorManager.Get().IsInSpectatorMode())
      name = !p.IsLocalUser() ? GameStrings.Get("GAMEPLAY_MISSING_OPPONENT_NAME") : GameStrings.Get("GAMEPLAY_HIDDEN_PLAYER_NAME");
    if (p.IsLocalUser())
    {
      string alternatePlayerName = GameState.Get().GetGameEntity().GetAlternatePlayerName();
      if (!string.IsNullOrEmpty(alternatePlayerName))
        name = alternatePlayerName;
    }
    string nameBannerOverride = GameState.Get().GetGameEntity().GetNameBannerOverride(this.m_playerSide);
    if (!string.IsNullOrEmpty(nameBannerOverride))
      name = nameBannerOverride;
    float timeStart = Time.time;
    for (; string.IsNullOrEmpty(name); name = p.GetName())
    {
      if ((double) Time.time - (double) timeStart >= 5.0)
      {
        if (GameMgr.Get().GetReconnectType() == ReconnectType.GAMEPLAY)
        {
          string displayedPlayerName = GameMgr.Get().GetLastDisplayedPlayerName(this.m_playerId);
          if (!string.IsNullOrEmpty(displayedPlayerName))
            name = displayedPlayerName;
        }
        if (string.IsNullOrEmpty(name))
        {
          name = !p.IsLocalUser() ? GameStrings.Get("GAMEPLAY_MISSING_OPPONENT_NAME") : GameStrings.Get("GAMEPLAY_HIDDEN_PLAYER_NAME");
          break;
        }
        break;
      }
      yield return (object) null;
    }
    bool flag = false;
    if (this.ShouldShowGameIconBanner())
      flag = true;
    if (!this.m_canShowModeIcons)
      flag = false;
    if (this.m_shouldShowRankedMedal)
    {
      this.m_nameBoneToUse = this.m_useLongName ? this.m_longMedalNameBone : this.m_medalNameBone;
      this.m_classBoneToUse = this.m_useLongName ? this.m_longMedalClassBone : this.m_medalClassBone;
      if ((UnityEngine.Object) this.m_medalBannerSkinned == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_medalAlphaBanner != (UnityEngine.Object) null)
          this.m_medalAlphaBanner.SetActive(true);
      }
      else
      {
        this.m_medalBannerSkinned.SetActive(true);
        this.m_alphaBannerSkinned.SetActive(false);
        if ((UnityEngine.Object) this.m_medalAlphaBanner != (UnityEngine.Object) null)
          this.m_medalAlphaBanner.SetActive(false);
      }
      if ((UnityEngine.Object) this.m_alphaBanner != (UnityEngine.Object) null)
        this.m_alphaBanner.SetActive(false);
    }
    else if (flag)
    {
      this.m_nameBoneToUse = this.m_useLongName ? this.m_longMedalNameBone : this.m_medalNameBone;
      this.m_classBoneToUse = this.m_useLongName ? this.m_longMedalClassBone : this.m_medalClassBone;
      if ((UnityEngine.Object) this.m_modeIconBannerSkinned == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_medalAlphaBanner != (UnityEngine.Object) null)
          this.m_medalAlphaBanner.SetActive(true);
      }
      else
      {
        this.m_modeIconBannerSkinned.SetActive(true);
        this.m_alphaBannerSkinned.SetActive(false);
        if ((UnityEngine.Object) this.m_medalAlphaBanner != (UnityEngine.Object) null)
          this.m_medalAlphaBanner.SetActive(false);
      }
      if ((UnityEngine.Object) this.m_alphaBanner != (UnityEngine.Object) null)
        this.m_alphaBanner.SetActive(false);
    }
    else
    {
      this.m_nameBoneToUse = this.m_useLongName ? this.m_longNameBone : this.m_nameBone;
      this.m_classBoneToUse = this.m_useLongName ? this.m_longClassBone : this.m_classBone;
      if ((UnityEngine.Object) this.m_alphaBannerSkinned == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_alphaBanner != (UnityEngine.Object) null)
          this.m_alphaBanner.SetActive(true);
      }
      else
      {
        this.m_alphaBannerSkinned.SetActive(true);
        if ((UnityEngine.Object) this.m_alphaBanner != (UnityEngine.Object) null)
          this.m_alphaBanner.SetActive(false);
        this.m_medalBannerSkinned.SetActive(false);
      }
      if ((UnityEngine.Object) this.m_medalAlphaBanner != (UnityEngine.Object) null)
        this.m_medalAlphaBanner.SetActive(false);
      if ((UnityEngine.Object) this.m_medalBannerSkinned != (UnityEngine.Object) null)
        this.m_medalBannerSkinned.SetActive(false);
    }
    this.SetName(name);
    if (GameMgr.Get().IsTraditionalTutorial() || this.m_isGameplayNameBanner)
    {
      this.SetMobilePositionOffset();
      this.m_shouldCenterName = true;
      this.PositionNameText(false);
    }
    else
    {
      AdventureDbId adventureId = GameUtils.GetAdventureId(this.m_missionId);
      if (this.m_shouldShowRankedMedal)
      {
        if ((UnityEngine.Object) this.m_medalWidget != (UnityEngine.Object) null)
          this.m_medalWidget.Show();
      }
      else if (this.m_playerSide == Player.Side.FRIENDLY && !(bool) UniversalInputManager.UsePhoneUI)
      {
        if (GameUtils.ShouldShowAdventureModeIcon())
        {
          AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) adventureId);
          if ((UnityEngine.Object) this.m_adventureGameModeIcon != (UnityEngine.Object) null)
          {
            AssetLoader.Get().LoadAsset<Texture>(ref this.m_gameModeIconTexture, (AssetReference) record.GameModeIcon);
            RendererExtension.GetMaterial((Renderer) this.m_adventureIcon.GetComponent<MeshRenderer>()).SetTexture("_MainTex", (Texture) this.m_gameModeIconTexture);
            this.m_adventureGameModeIcon.Show(true);
          }
        }
        else if (GameUtils.ShouldShowCasualModeIcon())
        {
          if (this.m_formatType == FormatType.FT_STANDARD)
            this.m_casualStandardGameModeIcon.Show(true);
          else
            this.m_casualWildGameModeIcon.Show(true);
        }
        else if (GameUtils.ShouldShowArenaModeIcon())
        {
          this.m_arenaGameModeIcon.Show(true);
          uint num = p.GetArenaWins();
          uint numberOfMarks = p.GetArenaLosses();
          if ((BnetEntityId) p.GetGameAccountId() == (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId())
          {
            timeStart = Time.time;
            while (!DraftManager.Get().CanShowWinsLosses)
            {
              yield return (object) null;
              if ((double) Time.time - (double) timeStart >= 5.0)
                break;
            }
            num = (uint) DraftManager.Get().GetWins();
            numberOfMarks = (uint) DraftManager.Get().GetLosses();
          }
          this.m_arenaGameModeIcon.SetText(num.ToString());
          this.m_arenaGameModeIcon.ShowXMarks(numberOfMarks);
        }
        else if (GameUtils.ShouldShowFriendlyChallengeIcon())
        {
          TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
          if (tavernBrawlMission != null && tavernBrawlMission.missionId == GameMgr.Get().GetMissionId())
          {
            this.m_tavernBrawlGameModeIcon.Show(true);
            this.m_tavernBrawlGameModeIcon.ShowFriendlyChallengeBanner(true);
          }
          else
          {
            this.m_friendlyGameModeIcon.Show(true);
            this.m_friendlyGameModeIcon.ShowWildVines(this.m_formatType == FormatType.FT_WILD);
          }
        }
        else if (GameUtils.ShouldShowTavernBrawlModeIcon())
        {
          if (TavernBrawlManager.Get().IsCurrentSeasonSessionBased)
          {
            uint num = p.GetTavernBrawlWins();
            uint numberOfMarks = p.GetTavernBrawlLosses();
            if ((BnetEntityId) p.GetGameAccountId() == (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId())
            {
              num = (uint) TavernBrawlManager.Get().GamesWon;
              numberOfMarks = (uint) TavernBrawlManager.Get().GamesLost;
            }
            GameModeIcon gameModeIcon = TavernBrawlManager.Get().CurrentSeasonBrawlMode == TavernBrawlMode.TB_MODE_HEROIC ? this.m_heroicSessionBasedTavernBrawlIcon : (GameModeIcon) this.m_normalSessionBasedTavernBrawlIcon;
            gameModeIcon.Show(true);
            gameModeIcon.SetText(num.ToString());
            gameModeIcon.ShowXMarks(numberOfMarks);
          }
          else
          {
            this.m_tavernBrawlGameModeIcon.Show(true);
            this.m_tavernBrawlGameModeIcon.ShowFriendlyChallengeBanner(false);
          }
        }
        else if (GameUtils.ShouldShowPvpDrModeIcon())
        {
          this.m_pvpdrGameModeIcon.Show(true);
          uint duelsWins = p.GetDuelsWins();
          uint duelsLosses = p.GetDuelsLosses();
          this.m_pvpdrGameModeIcon.SetText(duelsWins.ToString());
          this.m_pvpdrGameModeIcon.ShowXMarks(duelsLosses);
        }
      }
      yield return (object) this.UpdateSubtextImpl();
      if (GameState.Get().GetGameEntity().ShouldDoAlternateMulliganIntro())
      {
        this.FadeOutSubtext();
        this.PositionNameText(false);
      }
    }
  }

  private IEnumerator UpdateSubtextImpl()
  {
    AdventureModeDbId adventureModeId = GameUtils.GetAdventureModeId(this.m_missionId);
    AdventureDbId adventureId = GameUtils.GetAdventureId(this.m_missionId);
    bool flag = GameUtils.IsExpansionAdventure(adventureId) && adventureModeId != AdventureModeDbId.CLASS_CHALLENGE;
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    Player player = GameState.Get().GetPlayer(this.m_playerId);
    if (gameEntity != null && gameEntity.GetNameBannerSubtextOverride(this.m_playerSide) != null)
      this.SetSubtext(gameEntity.GetNameBannerSubtextOverride(this.m_playerSide));
    else if (this.m_playerSide == Player.Side.OPPOSING & flag)
    {
      AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) adventureId, (int) adventureModeId);
      if (adventureDataRecord != null)
        this.SetSubtext(((string) adventureDataRecord.ShortName).ToUpper());
    }
    else
    {
      Entity hero = player.GetHero();
      bool isSetSubtextCalled = false;
      if (hero != null)
      {
        while (hero.GetClass() == TAG_CLASS.INVALID)
          yield return (object) null;
        if (hero.GetClass() != TAG_CLASS.NEUTRAL)
        {
          isSetSubtextCalled = true;
          this.SetSubtext(GameStrings.GetClassName(player.GetHero().GetClass()).ToUpper());
        }
      }
      if (!isSetSubtextCalled)
        this.m_currentPlayerName.transform.position = this.m_nameBoneToUse.position;
      hero = (Entity) null;
    }
    if (GameState.Get().GetGameEntity().ShouldDoAlternateMulliganIntro())
    {
      this.FadeOutSubtext();
      this.PositionNameText(false);
    }
    if (GameMgr.Get().IsReconnect() && GameState.Get().IsMainPhase())
      this.PositionNameText_Reconnect();
  }

  public void UpdateMedalChange(MedalInfoTranslator medalInfo)
  {
    medalInfo.CreateOrUpdateDataModel(this.m_formatType, ref this.m_rankedDataModel, RankedMedal.DisplayMode.Default);
    if ((UnityEngine.Object) this.m_rankedMedal == (UnityEngine.Object) null)
      return;
    if (!this.m_shouldShowRankedMedal || medalInfo == null || !medalInfo.IsDisplayable())
    {
      this.m_rankedMedal.gameObject.SetActive(false);
    }
    else
    {
      this.m_rankedMedal.gameObject.SetActive(true);
      this.m_rankedMedal.BindRankedPlayDataModel(this.m_rankedDataModel);
      this.m_medalWidget.Show();
    }
  }

  public void UpdatePvpDRInfo(PVPDRLobbyDataModel dataModel)
  {
    this.m_pvpdrGameModeIcon.SetText(dataModel.Wins.ToString());
    this.m_pvpdrGameModeIcon.ShowXMarks((uint) dataModel.Losses);
  }

  private bool ShouldShowGameIconBanner() => this.m_playerSide == Player.Side.FRIENDLY && !(bool) UniversalInputManager.UsePhoneUI && !GameUtils.IsPracticeMission(this.m_missionId) && !GameUtils.IsTutorialMission(this.m_missionId);

  private void OnRankedMedalWidgetReady(Widget widget)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
      return;
    widget.Hide();
    this.m_medalWidget = widget;
    this.m_rankedMedal = widget.GetComponentInChildren<RankedMedal>();
  }

  private IEnumerator UpdateMedalWhenReady()
  {
    if (this.m_medalInfo == null)
    {
      Player player = NameBanner.GetPlayerForSide(this.m_playerSide);
      float timeStart = Time.time;
      while (player.GetRank() == null || (UnityEngine.Object) this.m_rankedMedal == (UnityEngine.Object) null)
      {
        yield return (object) null;
        if ((double) Time.time - (double) timeStart >= 5.0)
          break;
      }
      this.m_medalInfo = player.GetRank();
      if (this.m_medalInfo == null || !this.m_medalInfo.IsDisplayable())
        this.m_shouldShowRankedMedal = false;
      if (this.m_shouldShowRankedMedal && this.m_playerSide == Player.Side.OPPOSING)
      {
        Player playerForSide = NameBanner.GetPlayerForSide(Player.Side.FRIENDLY);
        MedalInfoTranslator medalInfoTranslator = (MedalInfoTranslator) null;
        if (playerForSide != null)
          medalInfoTranslator = playerForSide.GetRank();
        if (playerForSide == null || medalInfoTranslator == null || !medalInfoTranslator.GetCurrentMedal(this.m_formatType).RankConfig.ShowOpponentRankInGame)
          this.m_shouldShowRankedMedal = false;
      }
      if (this.m_shouldShowRankedMedal)
        this.UpdateMedalChange(this.m_medalInfo);
    }
  }
}
