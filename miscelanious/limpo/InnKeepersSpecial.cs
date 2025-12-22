using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Content.Delivery;
using Hearthstone;
using Hearthstone.Http;
using MiniJSON;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InnKeepersSpecial : MonoBehaviour
{
  public GameObject adImage;
  public GameObject adBackground;
  public PegUIElement adButton;
  public UberText adButtonText;
  public UberText adTitle;
  public UberText adSubtitle;
  public GameObject content;
  private Vector3 m_titleOrgPos;
  private Vector3 m_subtitleOrgPos;
  private List<InnKeepersSpecialAd> m_allAdsFromServer;
  private const char KEY_VALUE_PAIR_OPTIONS_SEPARATOR = ';';
  private const char HASH_COUNT_OPTIONS_SEPARATOR = ',';
  private const int DEFAULT_MAX_MESSAGE_VIEW_COUNT = 3;
  private string m_url;
  private GeneralStoreMode m_storeMode;
  private AdventureDbId m_adventureDbId;
  private AdventureModeDbId m_adventureModeDbId;
  private GameSaveKeyId m_adventureClientGameSaveKey;
  private static InnKeepersSpecial s_instance;
  private bool m_loadedSuccessfully;
  private bool m_forceShowIks;
  private bool m_forceOnetime;
  private bool m_calledOnInit;
  private bool m_isShown;
  private bool m_wasInteractedWith;
  private bool m_adsDependOnAdventureGameSaveData;
  private bool m_adsDependOnTavernBrawlProgress;
  private bool m_adsDependOnRecruitProgress;
  private bool m_adsDependOnAccountLicenseInfo;
  private bool m_adsDependOnCollectionProgress;
  private bool m_adventureGameSaveDataReceived;
  private bool m_tavernBrawlInfoReceived;
  private bool m_tavernBrawlPlayerRecordReceived;
  private bool m_recruitProgressReceived;
  private bool m_accountLicenseInfoReceived;
  private bool m_collectionProgressReceived;
  private bool m_bnetButtonsLocked;
  private bool m_readyToDisplay;
  private List<Action> m_readyToDisplayListeners = new List<Action>();
  private List<Action> m_loadedSuccessfullyListeners = new List<Action>();
  private BaseIKSContentProvider m_contentHandler = (BaseIKSContentProvider) new ContentStackIKSContentProvider();
  private Action m_OnClickCallback;

  public static InnKeepersSpecial Get()
  {
    InnKeepersSpecial.Init();
    return InnKeepersSpecial.s_instance;
  }

  public InnKeepersSpecialAd AdToDisplay => !this.m_allAdsFromServer.Any<InnKeepersSpecialAd>() ? new InnKeepersSpecialAd() : this.m_allAdsFromServer[0];

  public static void Init()
  {
    if (!((UnityEngine.Object) InnKeepersSpecial.s_instance == (UnityEngine.Object) null))
      return;
    InnKeepersSpecial.s_instance = AssetLoader.Get().InstantiatePrefab((AssetReference) "InnKeepersSpecial.prefab:fe19b8065e74440e4bf42d73cbbf3662").GetComponent<InnKeepersSpecial>();
    OverlayUI.Get().AddGameObject(InnKeepersSpecial.s_instance.gameObject);
    InnKeepersSpecial.s_instance.m_forceShowIks = Options.Get().GetBool(Option.FORCE_SHOW_IKS);
    InnKeepersSpecial.s_instance.m_titleOrgPos = InnKeepersSpecial.s_instance.adTitle.transform.localPosition;
    InnKeepersSpecial.s_instance.m_subtitleOrgPos = InnKeepersSpecial.s_instance.adSubtitle.transform.localPosition;
  }

  public bool LoadedSuccessfully() => this.m_loadedSuccessfully;

  public bool IsShown => this.m_isShown;

  public bool ProcessingResponse { get; set; }

  public void InitializeURLAndUpdate()
  {
    this.Hide();
    InnKeepersSpecial.MigrationIKSOptions();
    this.InitializeJsonURL(string.Empty);
    this.adButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.Click));
    this.RegisterAllDependencyListeners();
    this.Update();
  }

  public void InitializeJsonURL(string customURL) => this.m_contentHandler.InitializeJsonURL(customURL);

  public void ResetAdUrl() => this.m_forceOnetime = true;

  private void Start() => this.Hide();

  private static void MigrationIKSOptions()
  {
    Options.Get().DeleteOption(Option.IKS_LAST_DOWNLOAD_TIME);
    Options.Get().DeleteOption(Option.IKS_CACHE_AGE);
    Options.Get().DeleteOption(Option.IKS_LAST_DOWNLOAD_RESPONSE);
  }

  private void RegisterAllDependencyListeners()
  {
    Network network = Network.Get();
    if (network == null)
      return;
    network.RegisterNetHandler((object) TavernBrawlInfo.PacketID.ID, new Network.NetHandler(this.TavernBrawlInfoReceivedCallback));
    network.RegisterNetHandler((object) TavernBrawlPlayerRecordResponse.PacketID.ID, new Network.NetHandler(this.TavernBrawlPlayerRecordReceivedCallback));
    network.RegisterNetHandler((object) RecruitAFriendDataResponse.PacketID.ID, new Network.NetHandler(this.RecruitProgressReceivedCallback));
    network.RegisterNetHandler((object) AccountLicensesInfoResponse.PacketID.ID, new Network.NetHandler(this.AccountLicensesInfoResponseReceivedCallback));
    CollectionManager.Get().RegisterOnInitialCollectionReceivedListener(new Action(this.CollectionProgressReceivedCallback));
  }

  private void RemoveAllDependencyListeners()
  {
    Network network = Network.Get();
    if (network == null)
      return;
    network.RemoveNetHandler((object) TavernBrawlInfo.PacketID.ID, new Network.NetHandler(this.TavernBrawlInfoReceivedCallback));
    network.RemoveNetHandler((object) TavernBrawlPlayerRecordResponse.PacketID.ID, new Network.NetHandler(this.TavernBrawlPlayerRecordReceivedCallback));
    network.RemoveNetHandler((object) RecruitAFriendDataResponse.PacketID.ID, new Network.NetHandler(this.RecruitProgressReceivedCallback));
    network.RemoveNetHandler((object) AccountLicensesInfoResponse.PacketID.ID, new Network.NetHandler(this.AccountLicensesInfoResponseReceivedCallback));
    CollectionManager.Get().RemoveOnInitialCollectionReceivedListener(new Action(this.CollectionProgressReceivedCallback));
  }

  private void RequestDataForDependencies()
  {
    Network network = Network.Get();
    if (this.m_adsDependOnTavernBrawlProgress && !this.m_tavernBrawlInfoReceived)
      network.RequestTavernBrawlInfo(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);
    if (this.m_adsDependOnTavernBrawlProgress && !this.m_tavernBrawlPlayerRecordReceived)
      network.RequestTavernBrawlPlayerRecord(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);
    if (this.m_adsDependOnRecruitProgress && !this.m_recruitProgressReceived)
      network.RequestRecruitAFriendData();
    if (this.m_adsDependOnAccountLicenseInfo && !this.m_accountLicenseInfoReceived)
      NetCache.Get().RefreshNetObject<NetCache.NetCacheAccountLicenses>();
    if (this.m_adventureClientGameSaveKey == ~GameSaveKeyId.INVALID)
      return;
    GameSaveDataManager.Get().Request(this.m_adventureClientGameSaveKey, new GameSaveDataManager.OnRequestDataResponseDelegate(this.AdventureGameSaveDataReceivedCallback));
  }

  private void AdventureGameSaveDataReceivedCallback(bool success)
  {
    this.m_adventureGameSaveDataReceived = true;
    if (!this.m_adsDependOnAdventureGameSaveData)
      return;
    this.CheckReadyToDisplay();
  }

  private void TavernBrawlInfoReceivedCallback()
  {
    this.m_tavernBrawlInfoReceived = true;
    Network.Get().RemoveNetHandler((object) TavernBrawlInfo.PacketID.ID, new Network.NetHandler(this.TavernBrawlInfoReceivedCallback));
    if (!this.m_adsDependOnTavernBrawlProgress)
      return;
    this.CheckReadyToDisplay();
  }

  private void TavernBrawlPlayerRecordReceivedCallback()
  {
    this.m_tavernBrawlPlayerRecordReceived = true;
    Network.Get().RemoveNetHandler((object) TavernBrawlPlayerRecordResponse.PacketID.ID, new Network.NetHandler(this.TavernBrawlPlayerRecordReceivedCallback));
    if (!this.m_adsDependOnTavernBrawlProgress)
      return;
    this.CheckReadyToDisplay();
  }

  private void RecruitProgressReceivedCallback()
  {
    this.m_recruitProgressReceived = true;
    Network.Get().RemoveNetHandler((object) RecruitAFriendDataResponse.PacketID.ID, new Network.NetHandler(this.RecruitProgressReceivedCallback));
    if (!this.m_adsDependOnRecruitProgress)
      return;
    this.CheckReadyToDisplay();
  }

  private void AccountLicensesInfoResponseReceivedCallback()
  {
    this.m_accountLicenseInfoReceived = true;
    Network.Get().RemoveNetHandler((object) AccountLicensesInfoResponse.PacketID.ID, new Network.NetHandler(this.AccountLicensesInfoResponseReceivedCallback));
    if (!this.m_adsDependOnAccountLicenseInfo)
      return;
    this.CheckReadyToDisplay();
  }

  private void CollectionProgressReceivedCallback()
  {
    this.m_collectionProgressReceived = true;
    CollectionManager.Get().RemoveOnInitialCollectionReceivedListener(new Action(this.CollectionProgressReceivedCallback));
    if (!this.m_adsDependOnCollectionProgress)
      return;
    this.CheckReadyToDisplay();
  }

  private void CheckReadyToDisplay()
  {
    this.m_readyToDisplay = (!this.m_adsDependOnAdventureGameSaveData || this.m_adventureGameSaveDataReceived) && (!this.m_adsDependOnAccountLicenseInfo || this.m_accountLicenseInfoReceived) && (!this.m_adsDependOnRecruitProgress || this.m_recruitProgressReceived) && (!this.m_adsDependOnTavernBrawlProgress || this.m_tavernBrawlInfoReceived && this.m_tavernBrawlPlayerRecordReceived) && (!this.m_adsDependOnCollectionProgress || this.m_collectionProgressReceived);
    if (!this.m_readyToDisplay)
      return;
    foreach (Action action in this.m_readyToDisplayListeners.ToArray())
      action();
  }

  public void RegisterReadyToDisplayCallback(Action callback)
  {
    if (!this.m_readyToDisplayListeners.Contains(callback))
      this.m_readyToDisplayListeners.Add(callback);
    if (!this.m_readyToDisplay)
      return;
    callback();
  }

  public void RegisterLoadedSuccessfullyCallback(Action callback)
  {
    if (!this.m_loadedSuccessfullyListeners.Contains(callback))
      this.m_loadedSuccessfullyListeners.Add(callback);
    if (!this.m_loadedSuccessfully)
      return;
    callback();
  }

  public static void RegisterClickCallback(Action callback)
  {
    if ((UnityEngine.Object) InnKeepersSpecial.s_instance == (UnityEngine.Object) null)
      return;
    InnKeepersSpecial.s_instance.m_OnClickCallback -= callback;
    InnKeepersSpecial.s_instance.m_OnClickCallback += callback;
  }

  public static void UnregisterClickCallback(Action callback)
  {
    if ((UnityEngine.Object) InnKeepersSpecial.s_instance == (UnityEngine.Object) null)
      return;
    InnKeepersSpecial.s_instance.m_OnClickCallback -= callback;
  }

  public static bool CheckShow()
  {
    if ((UnityEngine.Object) InnKeepersSpecial.s_instance == (UnityEngine.Object) null)
      return false;
    if (!InnKeepersSpecial.s_instance.LoadedSuccessfully())
    {
      Log.InnKeepersSpecial.Print("Skipping IKS! IKS Views not incremented. loadedSuccessfully={0}", (object) InnKeepersSpecial.Get().LoadedSuccessfully());
      return false;
    }
    int val = Options.Get().GetInt(Option.IKS_VIEW_ATTEMPTS, 0) + 1;
    Options.Get().SetInt(Option.IKS_VIEW_ATTEMPTS, val);
    bool flag1 = val > 3;
    int num = 0;
    bool flag2 = Options.Get().GetBool(Option.FORCE_SHOW_IKS);
    if (ReturningPlayerMgr.Get().SuppressOldPopups)
    {
      Log.InnKeepersSpecial.Print("Skipping IKS! ReturningPlayerMgr.Get().SuppressOldPopups={1}!", (object) ReturningPlayerMgr.Get().SuppressOldPopups);
      return false;
    }
    if (!(flag1 | flag2))
    {
      Log.InnKeepersSpecial.Print("Skipping IKS! views={0} lastShownViews={1}", (object) val, (object) num);
      return false;
    }
    Log.InnKeepersSpecial.Print("Showing IKS!");
    InnKeepersSpecial.s_instance.LockBnetButtons();
    InnKeepersSpecial.s_instance.ShowAdAndIncrementViewCountWhenReady();
    return true;
  }

  public void ShowAdAndIncrementViewCountWhenReady()
  {
    if (this.m_allAdsFromServer == null || !this.m_allAdsFromServer.Any<InnKeepersSpecialAd>())
      this.Hide();
    else
      this.RegisterReadyToDisplayCallback((Action) (() =>
      {
        if (!this.m_allAdsFromServer.Any<InnKeepersSpecialAd>())
          return;
        this.RegisterLoadedSuccessfullyCallback((Action) (() =>
        {
          this.IncremenetViewCountOfDisplayedAdInStorage();
          this.Show();
        }));
      }));
  }

  public void Show()
  {
    float num = 0.5f;
    this.content.SetActive(true);
    Material material = RendererExtension.GetMaterial(this.adImage.gameObject.GetComponent<Renderer>());
    material.color = material.color with { a = 0.0f };
    iTween.FadeTo(this.adImage.gameObject, iTween.Hash((object) "amount", (object) 1f, (object) "time", (object) num, (object) "easeType", (object) iTween.EaseType.linear));
    this.adTitle.Show();
    iTween.ValueTo(this.adTitle.gameObject, iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) num, (object) "easeType", (object) iTween.EaseType.linear, (object) "onupdate", (object) (Action<object>) (newVal => this.adTitle.TextAlpha = (float) newVal)));
    this.adSubtitle.Show();
    iTween.ValueTo(this.adSubtitle.gameObject, iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) num, (object) "easeType", (object) iTween.EaseType.linear, (object) "onupdate", (object) (Action<object>) (newVal => this.adSubtitle.TextAlpha = (float) newVal)));
    this.m_isShown = true;
    this.m_wasInteractedWith = false;
  }

  public void Hide()
  {
    this.content.SetActive(false);
    this.adTitle.Hide();
    this.adSubtitle.Hide();
    this.m_isShown = false;
  }

  public static void Close()
  {
    if (!((UnityEngine.Object) InnKeepersSpecial.s_instance != (UnityEngine.Object) null))
      return;
    InnKeepersSpecial.s_instance.CloseInternal();
  }

  private void CloseInternal()
  {
    if (this.m_isShown && !this.m_wasInteractedWith)
      TelemetryManager.Client().SendIKSIgnored(this.AdToDisplay.CampaignName, this.AdToDisplay.ImageUrl);
    this.Hide();
    this.UnlockBnetButtons();
    this.RemoveAllDependencyListeners();
    this.m_readyToDisplayListeners.Clear();
    this.m_loadedSuccessfullyListeners.Clear();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    InnKeepersSpecial.s_instance = (InnKeepersSpecial) null;
  }

  private void Click(UIEvent e)
  {
    Log.InnKeepersSpecial.Print("IKS on release! Link: " + this.AdToDisplay.Link + " Game Action: " + this.AdToDisplay.GameAction);
    this.m_wasInteractedWith = true;
    TelemetryManager.Client().SendIKSClicked(this.AdToDisplay.CampaignName, this.AdToDisplay.ImageUrl);
    this.SetAdViewCountInStorage(this.AdToDisplay.GetHash(), this.AdToDisplay.MaxViewCount + 1);
    if (!string.IsNullOrEmpty(this.AdToDisplay.GameAction))
    {
      DeepLinkManager.ExecuteDeepLink(this.AdToDisplay.GameAction.Split(' '), DeepLinkManager.DeepLinkSource.INNKEEPERS_SPECIAL, false);
      WelcomeQuests.OnNavigateBack();
      this.Hide();
    }
    else if (!string.IsNullOrEmpty(this.AdToDisplay.Link))
    {
      if (PlatformSettings.IsMobile())
      {
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
        info.m_showAlertIcon = false;
        info.m_headerText = GameStrings.Format("GLUE_INNKEEPERS_SPECIAL_CONFIRM_POPUP_HEADER");
        info.m_text = GameStrings.Get("GLUE_INNKEEPERS_SPECIAL_CONFIRM_POPUP_MESSAGE");
        info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
        info.m_disableBnetBar = true;
        AlertPopup.ResponseCallback responseCallback = (AlertPopup.ResponseCallback) ((response, userdata) =>
        {
          if (response != AlertPopup.Response.CONFIRM)
            return;
          Application.OpenURL(this.AdToDisplay.Link);
        });
        info.m_responseCallback = responseCallback;
        DialogManager.Get().ShowPopup(info);
      }
      else
        Application.OpenURL(this.AdToDisplay.Link);
    }
    else
      Debug.LogWarning((object) "InnKeepersSpecial Ad has no Game Action and Link is null or empty.");
    Action onClickCallback = this.m_OnClickCallback;
    if (onClickCallback == null)
      return;
    onClickCallback();
  }

  private void UpdateAdJson(string jsonResponse, object param)
  {
    if (!string.IsNullOrEmpty(jsonResponse))
    {
      JsonNode response;
      try
      {
        response = Json.Deserialize(jsonResponse) as JsonNode;
      }
      catch (Exception ex)
      {
        response = (JsonNode) null;
        Log.ContentConnect.PrintWarning("Aborting because of an invalid json response:\n{0}", (object) jsonResponse);
        Debug.LogError((object) ex.StackTrace);
      }
      this.m_allAdsFromServer = this.GetAllAdsFromJsonResponse(response);
      if (this.m_allAdsFromServer.Any<InnKeepersSpecialAd>())
      {
        this.CheckAdDependenciesAndRequestData(this.AdToDisplay.GameAction);
        this.RegisterReadyToDisplayCallback(new Action(this.VerifyAdToDisplayBasedOnResponses));
      }
    }
    this.ProcessingResponse = false;
  }

  private JsonList GetRootListNode(JsonNode response) => this.m_contentHandler.GetRootListNode(response);

  private List<InnKeepersSpecialAd> GetAllAdsFromJsonResponse(
    JsonNode response)
  {
    if (response == null)
      return new List<InnKeepersSpecialAd>();
    List<InnKeepersSpecialAd> fromJsonResponse = new List<InnKeepersSpecialAd>();
    try
    {
      JsonList rootListNode = this.GetRootListNode(response);
      if (rootListNode == null)
        return new List<InnKeepersSpecialAd>();
      Dictionary<string, int> ofAdsFromStorage = this.GetViewCountOfAdsFromStorage();
      Dictionary<string, int> values = new Dictionary<string, int>();
      foreach (object obj in (List<object>) rootListNode)
      {
        JsonNode adNode = obj as JsonNode;
        InnKeepersSpecialAd keepersSpecialAd = this.m_contentHandler.ReadInnKeepersSpecialAd(adNode);
        string hash = keepersSpecialAd.GetHash();
        int num = ofAdsFromStorage.TryGetValue(hash, out num) ? num : 0;
        values[hash] = num;
        keepersSpecialAd.CurrentViewCount = num;
        if (this.m_forceShowIks || num < keepersSpecialAd.MaxViewCount)
        {
          if (!string.IsNullOrEmpty(keepersSpecialAd.ClientVersion) && !this.m_forceShowIks && !StringUtils.CompareIgnoreCase(keepersSpecialAd.ClientVersion, "25.0"))
          {
            Log.InnKeepersSpecial.Print("Skipping IKS: {0}, mis-matched client version {0} != {1}", (object) keepersSpecialAd.CampaignName, (object) keepersSpecialAd.ClientVersion, (object) "25.0");
          }
          else
          {
            if (!string.IsNullOrEmpty(keepersSpecialAd.Platform))
            {
              string[] strArray = keepersSpecialAd.Platform.Trim().Split(',');
              bool flag = false;
              foreach (string str in strArray)
              {
                if (StringUtils.CompareIgnoreCase(str.Trim(), PlatformSettings.OS.ToString()))
                  flag = true;
              }
              if (!this.m_forceShowIks && !flag)
              {
                Log.InnKeepersSpecial.Print("Skipping IKS: {0}, supported on: {1}; current platform is {2}", (object) keepersSpecialAd.CampaignName, (object) keepersSpecialAd.Platform, (object) PlatformSettings.OS.ToString());
                continue;
              }
            }
            if (!string.IsNullOrEmpty(keepersSpecialAd.AndroidStore))
            {
              string[] strArray = keepersSpecialAd.AndroidStore.Trim().Split(',');
              bool flag = false;
              string b = AndroidDeviceSettings.Get().GetAndroidStore().ToString();
              foreach (string str in strArray)
              {
                if (StringUtils.CompareIgnoreCase(str.Trim(), b))
                  flag = true;
              }
              if (!this.m_forceShowIks && !flag)
              {
                Log.InnKeepersSpecial.Print("Skipping IKS: {0}, supported on: {1}; current android store is {2}", (object) keepersSpecialAd.CampaignName, (object) keepersSpecialAd.AndroidStore, (object) b);
                continue;
              }
            }
            if (!this.m_forceShowIks && HearthstoneApplication.IsPublic() && !keepersSpecialAd.Visibility)
              Log.InnKeepersSpecial.Print("Skipping IKS: {0}, not flagged as publicly visible", (object) (string) adNode["campaignName"]);
            else
              fromJsonResponse.Add(keepersSpecialAd);
          }
        }
      }
      this.WriteViewCountOfAdsToStorage(values);
      fromJsonResponse.Sort(new Comparison<InnKeepersSpecialAd>(InnKeepersSpecialAd.ComparisonDescending));
      return fromJsonResponse;
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ("Failed to get correct advertisement: " + (object) ex));
      return new List<InnKeepersSpecialAd>();
    }
  }

  private void VerifyAdToDisplayBasedOnResponses()
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null || !this.m_allAdsFromServer.Any<InnKeepersSpecialAd>())
      return;
    if (!this.m_forceShowIks && this.HasInteractedWithAdvertisedProduct(this.AdToDisplay.GameAction))
    {
      Log.InnKeepersSpecial.Print("Player has interacted with the advertised product. Skipping ad: " + this.AdToDisplay.GameAction);
      this.DiscardCurrentAdAndRequestNextAdData();
    }
    else
    {
      Log.InnKeepersSpecial.Print("Ad to display :" + this.AdToDisplay.Link);
      this.StartCoroutine(this.UpdateAdTexture());
    }
  }

  private void DiscardCurrentAdAndRequestNextAdData()
  {
    if (!this.m_allAdsFromServer.Any<InnKeepersSpecialAd>())
      return;
    this.m_allAdsFromServer.RemoveAt(0);
    if (!this.m_allAdsFromServer.Any<InnKeepersSpecialAd>())
      return;
    this.CheckAdDependenciesAndRequestData(this.AdToDisplay.GameAction);
  }

  private void Update()
  {
    if (this.m_calledOnInit && !this.m_forceOnetime || !this.m_contentHandler.Ready)
      return;
    this.Hide();
    this.ProcessingResponse = true;
    this.StartCoroutine(this.m_contentHandler.GetQuery(new ResponseProcessHandler(this.UpdateAdJson), (object) null, this.m_forceOnetime));
    this.m_forceOnetime = false;
    this.m_calledOnInit = true;
  }

  private IEnumerator UpdateAdTexture()
  {
    if (!string.IsNullOrEmpty(this.AdToDisplay.Title))
      this.adTitle.Text = this.AdToDisplay.Title.Replace("\\n", "\n");
    if (!string.IsNullOrEmpty(this.AdToDisplay.SubTitle))
      this.adSubtitle.Text = this.AdToDisplay.SubTitle.Replace("\\n", "\n");
    string imageUrl = this.AdToDisplay.ImageUrl;
    if (!string.IsNullOrEmpty(this.AdToDisplay.ImageUrl) && this.AdToDisplay.ImageUrl.StartsWith("//"))
      imageUrl = "http:" + this.AdToDisplay.ImageUrl;
    Log.InnKeepersSpecial.Print("image url is " + imageUrl);
    IHttpRequest textureHttpRequest = HttpRequestFactory.Get().CreateGetTextureRequest(imageUrl);
    yield return (object) textureHttpRequest.SendRequest();
    if (textureHttpRequest.IsNetworkError || textureHttpRequest.IsHttpError)
    {
      Debug.LogError((object) ("Failed to download image for Innkeeper's Special: " + imageUrl));
      Debug.LogError((object) textureHttpRequest.ErrorString);
      this.DiscardCurrentAdAndRequestNextAdData();
    }
    else
    {
      Texture responseAsTexture = textureHttpRequest.ResponseAsTexture;
      if (responseAsTexture.width == 8 && responseAsTexture.height == 8)
      {
        Debug.LogError((object) ("Failed to download image for Innkeeper's Special (got 8x8 dummy image): " + imageUrl));
        this.DiscardCurrentAdAndRequestNextAdData();
      }
      else
      {
        Material material = RendererExtension.GetMaterial(this.adImage.GetComponent<Renderer>());
        material.mainTexture = responseAsTexture;
        material.mainTexture.wrapMode = TextureWrapMode.Clamp;
        this.UpdateText();
        this.m_loadedSuccessfully = true;
        foreach (Action action in this.m_loadedSuccessfullyListeners.ToArray())
          action();
      }
    }
  }

  private void UpdateText()
  {
    if (!string.IsNullOrEmpty(this.AdToDisplay.ButtonText))
    {
      this.adButtonText.GameStringLookup = false;
      this.adButtonText.Text = this.AdToDisplay.ButtonText;
    }
    Vector3 titleOrgPos = this.m_titleOrgPos;
    titleOrgPos.x += (float) this.AdToDisplay.TitleOffsetX;
    titleOrgPos.y += (float) this.AdToDisplay.TitleOffsetY;
    this.adTitle.transform.localPosition = titleOrgPos;
    Vector3 subtitleOrgPos = this.m_subtitleOrgPos;
    subtitleOrgPos.x += (float) this.AdToDisplay.SubTitleOffsetX;
    subtitleOrgPos.y += (float) this.AdToDisplay.SubTitleOffsetY;
    this.adSubtitle.transform.localPosition = subtitleOrgPos;
    this.adTitle.FontSize = this.AdToDisplay.TitleFontSize;
    this.adSubtitle.FontSize = this.AdToDisplay.SubTitleFontSize;
  }

  public bool HasInteractedWithAdvertisedProduct(string gameAction)
  {
    if (string.IsNullOrEmpty(gameAction))
    {
      Log.InnKeepersSpecial.Print("IKS unable to check interaction for product with null gameAction.");
      return false;
    }
    string[] actionTokens = gameAction.Split(' ');
    if (actionTokens[0].Equals("store", StringComparison.OrdinalIgnoreCase))
    {
      if (actionTokens.Length <= 1)
        return false;
      string str = actionTokens[1];
      AdventureDbId adventureDbId = EnumUtils.SafeParse<AdventureDbId>(str, ignoreCase: true);
      HeroDbId heroDbId = EnumUtils.SafeParse<HeroDbId>(str, ignoreCase: true);
      StorePackType storePackType;
      int boosterId;
      DeepLinkManager.GetBoosterAndStorePackTypeFromGameAction(actionTokens, out boosterId, out storePackType);
      if (boosterId != 0)
      {
        if (storePackType == StorePackType.BOOSTER && boosterId == 181)
          return StoreManager.IsFirstPurchaseBundleOwned();
        if (storePackType == StorePackType.MODULAR_BUNDLE)
          return GameDbf.ModularBundleLayout.GetRecords((Predicate<ModularBundleLayoutDbfRecord>) (r => r.ModularBundleId == boosterId)).Any<ModularBundleLayoutDbfRecord>((Func<ModularBundleLayoutDbfRecord, bool>) (r => StoreManager.IsHiddenLicenseBundleOwned(r.HiddenLicenseId)));
      }
      else
      {
        if (adventureDbId != AdventureDbId.INVALID)
        {
          if (this.m_adventureClientGameSaveKey == ~GameSaveKeyId.INVALID)
            return false;
          long num;
          GameSaveDataManager.Get().GetSubkeyValue(this.m_adventureClientGameSaveKey, GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_ADVENTURE, out num);
          return num == 1L;
        }
        if (heroDbId != HeroDbId.INVALID)
        {
          string cardIdFromHeroDbId = GameUtils.GetCardIdFromHeroDbId((int) heroDbId);
          return CollectionManager.Get().IsCardInCollection(cardIdFromHeroDbId, TAG_PREMIUM.NORMAL);
        }
      }
      return false;
    }
    if (actionTokens[0].Equals("recruitafriend", StringComparison.OrdinalIgnoreCase))
      return RAFManager.Get().GetTotalRecruitCount() > 0U;
    if (actionTokens[0].Equals("tavernbrawl", StringComparison.OrdinalIgnoreCase))
      return TavernBrawlManager.Get().GamesPlayed > 0 || !TavernBrawlManager.Get().IsTavernBrawlActive(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);
    if (actionTokens[0].Equals("adventure", StringComparison.OrdinalIgnoreCase))
    {
      if (actionTokens.Length <= 1)
        return false;
      AdventureDbId adventureID = EnumUtils.SafeParse<AdventureDbId>(actionTokens[1], ignoreCase: true);
      return adventureID != AdventureDbId.INVALID && AdventureProgressMgr.Get().OwnsOneOrMoreAdventureWings(adventureID);
    }
    Log.InnKeepersSpecial.Print("IKS unrecognized game action: " + gameAction + " Unable to determine if the player has interacted with it previously. ");
    return false;
  }

  private void CheckAdDependenciesAndRequestData(string gameAction)
  {
    if (string.IsNullOrEmpty(gameAction))
    {
      this.CheckReadyToDisplay();
    }
    else
    {
      string[] actionTokens = gameAction.Split(' ');
      if (actionTokens[0].Equals("store", StringComparison.OrdinalIgnoreCase))
      {
        if (actionTokens.Length > 1)
        {
          string str = actionTokens[1];
          AdventureDbId adventureDbId = EnumUtils.SafeParse<AdventureDbId>(str, ignoreCase: true);
          HeroDbId heroDbId = EnumUtils.SafeParse<HeroDbId>(str, ignoreCase: true);
          int boosterId;
          StorePackType storePackType;
          DeepLinkManager.GetBoosterAndStorePackTypeFromGameAction(actionTokens, out boosterId, out storePackType);
          if (boosterId != 0)
          {
            if (storePackType == StorePackType.BOOSTER && boosterId == 181 || storePackType == StorePackType.MODULAR_BUNDLE)
              this.m_adsDependOnAccountLicenseInfo = true;
          }
          else if (adventureDbId != AdventureDbId.INVALID)
            this.m_adsDependOnAdventureGameSaveData = true;
          else if (heroDbId != HeroDbId.INVALID)
            this.m_adsDependOnCollectionProgress = true;
        }
      }
      else if (actionTokens[0].Equals("recruitafriend", StringComparison.OrdinalIgnoreCase))
        this.m_adsDependOnRecruitProgress = true;
      else if (actionTokens[0].Equals("tavernbrawl", StringComparison.OrdinalIgnoreCase))
        this.m_adsDependOnTavernBrawlProgress = true;
      else if (actionTokens[0].Equals("adventure", StringComparison.OrdinalIgnoreCase))
      {
        this.m_adsDependOnAdventureGameSaveData = true;
        if (actionTokens.Length > 1)
        {
          this.m_adventureDbId = EnumUtils.SafeParse<AdventureDbId>(actionTokens[1], ignoreCase: true);
          AdventureDataDbfRecord record = GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => (AdventureDbId) r.AdventureId == this.m_adventureDbId));
          if (record != null)
            this.m_adventureClientGameSaveKey = (GameSaveKeyId) record.GameSaveDataClientKey;
        }
      }
      this.RequestDataForDependencies();
      this.CheckReadyToDisplay();
    }
  }

  public void IncremenetViewCountOfDisplayedAdInStorage()
  {
    if (!this.m_allAdsFromServer.Any<InnKeepersSpecialAd>())
      return;
    this.SetAdViewCountInStorage(this.AdToDisplay.GetHash(), ++this.AdToDisplay.CurrentViewCount);
  }

  private void SetAdViewCountInStorage(string adHash, int count)
  {
    if (string.IsNullOrEmpty(adHash))
      return;
    Dictionary<string, int> ofAdsFromStorage = this.GetViewCountOfAdsFromStorage();
    ofAdsFromStorage[adHash] = count;
    this.WriteViewCountOfAdsToStorage(ofAdsFromStorage);
  }

  private Dictionary<string, int> GetViewCountOfAdsFromStorage()
  {
    Dictionary<string, int> ofAdsFromStorage = new Dictionary<string, int>();
    string str1 = Options.Get().GetString(Option.IKS_LAST_SHOWN_AD);
    if (string.IsNullOrEmpty(str1))
      return ofAdsFromStorage;
    string str2 = str1;
    char[] chArray1 = new char[1]{ ';' };
    foreach (string str3 in str2.Split(chArray1))
    {
      char[] chArray2 = new char[1]{ ',' };
      string[] strArray = str3.Split(chArray2);
      if (strArray.Length == 2)
      {
        string key = strArray[0];
        int result = int.TryParse(strArray[1], out result) ? result : 0;
        if (!ofAdsFromStorage.ContainsKey(key))
          ofAdsFromStorage.Add(key, result);
      }
    }
    return ofAdsFromStorage;
  }

  private void WriteViewCountOfAdsToStorage(Dictionary<string, int> values)
  {
    string val = string.Join(';'.ToString(), values.Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (kvp => kvp.Key + "," + (object) kvp.Value)).ToArray<string>());
    if (!string.IsNullOrEmpty(val))
      Options.Get().SetString(Option.IKS_LAST_SHOWN_AD, val);
    else
      Options.Get().DeleteOption(Option.IKS_LAST_SHOWN_AD);
  }

  private void LockBnetButtons()
  {
    if ((UnityEngine.Object) BaseUI.Get() == (UnityEngine.Object) null || this.m_bnetButtonsLocked)
      return;
    BaseUI.Get().m_BnetBar.RequestDisableButtons();
    this.m_bnetButtonsLocked = true;
  }

  private void UnlockBnetButtons()
  {
    if ((UnityEngine.Object) BaseUI.Get() == (UnityEngine.Object) null || !this.m_bnetButtonsLocked)
      return;
    BaseUI.Get().m_BnetBar.CancelRequestToDisableButtons();
    this.m_bnetButtonsLocked = false;
  }
}
