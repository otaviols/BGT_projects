using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using PegasusUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAFManager : IService
{
  private bool m_isRAFLoading;
  private RAFFrame m_RAFFrame;
  private string m_rafDisplayURL;
  private string m_rafFullURL;
  private bool m_hasRAFData;
  private uint m_totalRecruitCount;
  private List<RAFManager.RecruitData> m_topRecruits;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    RAFManager rafManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Network network = serviceLocator.Get<Network>();
    network.RegisterNetHandler((object) ProcessRecruitAFriendResponse.PacketID.ID, new Network.NetHandler(rafManager.OnProcessRecruitResponse));
    network.RegisterNetHandler((object) RecruitAFriendURLResponse.PacketID.ID, new Network.NetHandler(rafManager.OnURLResponse));
    network.RegisterNetHandler((object) RecruitAFriendDataResponse.PacketID.ID, new Network.NetHandler(rafManager.OnDataResponse));
    HearthstoneApplication.Get().WillReset += new System.Action(rafManager.WillReset);
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (Network),
    typeof (SoundManager)
  };

  public void Shutdown()
  {
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
      hearthstoneApplication.WillReset -= new System.Action(this.WillReset);
    Network service = (Network) null;
    if (!ServiceManager.TryGet<Network>(out service))
      return;
    service.RemoveNetHandler((object) ProcessRecruitAFriendResponse.PacketID.ID, new Network.NetHandler(this.OnProcessRecruitResponse));
    service.RemoveNetHandler((object) RecruitAFriendURLResponse.PacketID.ID, new Network.NetHandler(this.OnURLResponse));
    service.RemoveNetHandler((object) RecruitAFriendDataResponse.PacketID.ID, new Network.NetHandler(this.OnDataResponse));
  }

  public static RAFManager Get() => ServiceManager.Get<RAFManager>();

  public void WillReset()
  {
    BnetPresenceMgr.Get().OnGameAccountPresenceChange -= new System.Action<PresenceUpdate[]>(this.OnPresenceChanged);
    this.m_RAFFrame = (RAFFrame) null;
    this.m_rafDisplayURL = (string) null;
    this.m_rafFullURL = (string) null;
    this.m_hasRAFData = false;
    this.m_totalRecruitCount = 0U;
    this.m_topRecruits = (List<RAFManager.RecruitData>) null;
  }

  public void InitializeRequests() => Network.Get().RequestProcessRecruitAFriend();

  public void ShowRAFFrame()
  {
    if (!this.m_hasRAFData)
    {
      Log.RAF.Print("Network.RequestRecruitAFriendData");
      Network.Get().RequestRecruitAFriendData();
    }
    Processor.CancelCoroutine(this.ShowRAFFrameWhenReady());
    Processor.RunCoroutine(this.ShowRAFFrameWhenReady());
  }

  public RAFFrame GetRAFFrame() => this.m_RAFFrame;

  public void ShowRAFHeroFrame()
  {
    if (!((UnityEngine.Object) this.m_RAFFrame != (UnityEngine.Object) null))
      return;
    this.m_RAFFrame.ShowHeroFrame();
  }

  public void ShowRAFProgressFrame()
  {
    if (!((UnityEngine.Object) this.m_RAFFrame != (UnityEngine.Object) null))
      return;
    this.m_RAFFrame.ShowProgressFrame();
  }

  public void SetRAFProgress(int progress)
  {
    if (!((UnityEngine.Object) this.m_RAFFrame != (UnityEngine.Object) null))
      return;
    this.m_RAFFrame.SetProgress(progress);
  }

  public string GetRecruitDisplayURL()
  {
    if (this.m_rafDisplayURL != null)
      return this.m_rafDisplayURL;
    Log.RAF.Print("Network.RequestRecruitAFriendURL");
    Network.Get().RequestRecruitAFriendUrl();
    return (string) null;
  }

  public string GetRecruitFullURL() => this.m_rafFullURL != null ? this.m_rafFullURL : (string) null;

  public void GotoRAFWebsite()
  {
    Processor.CancelCoroutine(this.SendToRAFWebsiteThenHide());
    Processor.RunCoroutine(this.SendToRAFWebsiteThenHide());
  }

  public uint GetTotalRecruitCount() => this.m_totalRecruitCount;

  private IEnumerator ShowRAFFrameWhenReady()
  {
    RAFManager rafManager = this;
    if ((UnityEngine.Object) rafManager.m_RAFFrame == (UnityEngine.Object) null && !rafManager.m_isRAFLoading)
    {
      rafManager.m_isRAFLoading = true;
      AssetLoader.Get().InstantiatePrefab((AssetReference) "RAF_main.prefab:5fa2642eb52ae469dbe27e96a7570e08", new PrefabCallback<GameObject>(rafManager.OnRAFLoaded));
    }
    while ((UnityEngine.Object) rafManager.m_RAFFrame == (UnityEngine.Object) null)
      yield return (object) null;
    while (!rafManager.m_hasRAFData)
      yield return (object) null;
    rafManager.m_RAFFrame.Show();
    ChatMgr.Get().CloseFriendsList();
  }

  private void OnRAFLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_isRAFLoading = false;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Log.RAF.PrintError("RAFManager.OnRAFLoaded() - FAILED to load RAFManager GameObject");
    }
    else
    {
      this.m_RAFFrame = go.GetComponent<RAFFrame>();
      if ((UnityEngine.Object) this.m_RAFFrame == (UnityEngine.Object) null)
      {
        Log.RAF.PrintError("RAFManager.OnRAFLoaded() - ERROR RAFManager GameObject has no " + (object) typeof (RAFFrame) + " component");
      }
      else
      {
        if (!this.m_hasRAFData)
          return;
        if (this.m_totalRecruitCount > 0U)
        {
          this.m_RAFFrame.SetProgressData(this.m_totalRecruitCount, this.m_topRecruits);
          this.m_RAFFrame.ShowProgressFrame();
        }
        else
          this.m_RAFFrame.ShowHeroFrame();
      }
    }
  }

  private void OnProcessRecruitResponse()
  {
  }

  private void OnURLResponse()
  {
    RecruitAFriendURLResponse afriendUrlResponse = Network.Get().GetRecruitAFriendUrlResponse();
    if (afriendUrlResponse == null || afriendUrlResponse.RafServiceStatus == RAFServiceStatus.RAFServiceStatus_NotAvailable || string.IsNullOrEmpty(afriendUrlResponse.RafUrl))
    {
      string format = "RAFManager.OnURLResponse() - Response not valid!";
      if (afriendUrlResponse != null)
        format += " " + (object) afriendUrlResponse.RafServiceStatus + ", " + afriendUrlResponse.RafUrl == null ? "null" : afriendUrlResponse.RafUrl;
      Log.RAF.PrintError(format);
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_RAF_ERROR_HEADER"),
        m_showAlertIcon = true,
        m_text = GameStrings.Get("GLUE_RAF_ERROR_BODY"),
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_responseCallback = (AlertPopup.ResponseCallback) null
      };
      DialogManager.Get().ShowPopup(info);
    }
    else
    {
      this.m_rafDisplayURL = afriendUrlResponse.RafUrl;
      Log.RAF.Print("Recruit URL = " + this.m_rafDisplayURL);
      if (!((UnityEngine.Object) this.m_RAFFrame != (UnityEngine.Object) null))
        return;
      this.m_rafFullURL = afriendUrlResponse.RafUrlFull;
      this.m_RAFFrame.ShowLinkFrame(this.m_rafDisplayURL, this.m_rafFullURL);
    }
  }

  private void OnDataResponse()
  {
    RecruitAFriendDataResponse afriendDataResponse = Network.Get().GetRecruitAFriendDataResponse();
    if (afriendDataResponse == null)
    {
      Log.RAF.PrintError("RAFManager.OnDataResponse() - Recruit Data is NULL!");
    }
    else
    {
      this.m_hasRAFData = true;
      this.m_totalRecruitCount = afriendDataResponse.TotalRecruitCount;
      this.m_topRecruits = new List<RAFManager.RecruitData>();
      BnetPresenceMgr.Get().OnGameAccountPresenceChange -= new System.Action<PresenceUpdate[]>(this.OnPresenceChanged);
      BnetPresenceMgr.Get().OnGameAccountPresenceChange += new System.Action<PresenceUpdate[]>(this.OnPresenceChanged);
      for (int index = 0; index < afriendDataResponse.TopRecruits.Count; ++index)
      {
        RAFManager.RecruitData recruitData = new RAFManager.RecruitData();
        this.m_topRecruits.Add(recruitData);
        recruitData.m_recruit = afriendDataResponse.TopRecruits[index];
        if (recruitData.m_recruit.GameAccountId == null)
        {
          Log.RAF.PrintWarning("RAFManager.OnDataResponse() - GameAccountId is NULL for recruit!");
        }
        else
        {
          BnetGameAccountId entityId = new BnetGameAccountId(recruitData.m_recruit.GameAccountId.Hi, recruitData.m_recruit.GameAccountId.Lo);
          List<PresenceFieldKey> presenceFieldKeyList = new List<PresenceFieldKey>();
          PresenceFieldKey presenceFieldKey = new PresenceFieldKey();
          presenceFieldKey.programId = BnetProgramId.BNET.GetValue();
          presenceFieldKey.groupId = 2U;
          presenceFieldKey.fieldId = 7U;
          presenceFieldKey.uniqueId = 0UL;
          presenceFieldKeyList.Add(presenceFieldKey);
          presenceFieldKey.programId = BnetProgramId.BNET.GetValue();
          presenceFieldKey.groupId = 2U;
          presenceFieldKey.fieldId = 3U;
          presenceFieldKey.uniqueId = 0UL;
          presenceFieldKeyList.Add(presenceFieldKey);
          presenceFieldKey.programId = BnetProgramId.BNET.GetValue();
          presenceFieldKey.groupId = 2U;
          presenceFieldKey.fieldId = 5U;
          presenceFieldKey.uniqueId = 0UL;
          presenceFieldKeyList.Add(presenceFieldKey);
          PresenceFieldKey[] array = presenceFieldKeyList.ToArray();
          BattleNet.RequestPresenceFields(true, (BnetEntityId) entityId, array);
        }
      }
      if (!((UnityEngine.Object) this.m_RAFFrame != (UnityEngine.Object) null))
        return;
      if (this.m_totalRecruitCount > 0U)
      {
        this.m_RAFFrame.SetProgressData(this.m_totalRecruitCount, this.m_topRecruits);
        this.m_RAFFrame.ShowProgressFrame();
      }
      else
        this.m_RAFFrame.ShowHeroFrame();
    }
  }

  private void OnPresenceChanged(PresenceUpdate[] updates)
  {
    if (this.m_topRecruits == null)
      return;
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    foreach (PresenceUpdate update in updates)
    {
      if (!(update.programId != (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.BNET) && update.groupId == 2U && update.fieldId == 5U)
      {
        BnetGameAccountId id = new BnetGameAccountId(update.entityId?.EntityId);
        BnetPlayer player = BnetUtils.GetPlayer(id);
        if (player != null && player != myPlayer && !(player.GetBattleTag() == (BnetBattleTag) null))
        {
          foreach (RAFManager.RecruitData topRecruit in this.m_topRecruits)
          {
            if ((long) topRecruit.m_recruit.GameAccountId.Lo == (long) id.Low && (long) topRecruit.m_recruit.GameAccountId.Hi == (long) id.High)
            {
              topRecruit.m_recruitBattleTag = player.GetBattleTag().GetString();
              Log.RAF.Print("Found Battle Tag for Game Account ID: " + topRecruit.m_recruitBattleTag);
              if ((UnityEngine.Object) this.m_RAFFrame != (UnityEngine.Object) null)
              {
                this.m_RAFFrame.UpdateBattleTag(topRecruit.m_recruit.GameAccountId, topRecruit.m_recruitBattleTag);
                break;
              }
              break;
            }
          }
        }
      }
    }
  }

  private IEnumerator SendToRAFWebsiteThenHide()
  {
    this.m_RAFFrame.m_infoFrame.m_okayButton.SetEnabled(false);
    string recruitAfriendLink = ExternalUrlService.Get().GetRecruitAFriendLink();
    if (!string.IsNullOrEmpty(recruitAfriendLink))
      Application.OpenURL(recruitAfriendLink);
    this.m_RAFFrame.m_infoFrame.Hide();
    yield break;
  }

  public class RecruitData
  {
    public PegasusUtil.RecruitData m_recruit;
    public string m_recruitBattleTag;
  }
}
