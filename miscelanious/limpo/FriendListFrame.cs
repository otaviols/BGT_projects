using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.UI;
using PegasusFSG;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendListFrame : MonoBehaviour
{
  public FriendListFrame.Me me;
  public FriendListFrame.Prefabs prefabs;
  public FriendListFrame.ListInfo listInfo;
  public TouchList items;
  public PlayerPortrait myPortrait;
  public PegUIElement addFriendButton;
  public PegUIElement removeFriendButton;
  public GameObject removeFriendButtonEnabledVisual;
  public GameObject removeFriendButtonDisabledVisual;
  public GameObject removeFriendButtonButtonGlow;
  public PegUIElement rafButton;
  public GameObject rafButtonEnabledVisual;
  public GameObject rafButtonDisabledVisual;
  public GameObject rafButtonButtonGlow;
  public GameObject rafButtonGlowBone;
  public TouchListScrollbar scrollbar;
  public NineSliceElement window;
  public PegUIElement fsgButton;
  public GameObject fsgButtonButtonGlow;
  public GameObject portraitBackground;
  public Material unrankedBackground;
  public Material rankedBackground;
  public GameObject innerShadow;
  public GameObject outerShadow;
  public GameObject temporaryAccountPaper;
  public GameObject temporaryAccountCover;
  public GameObject temporaryAccountDrawingBone;
  public GameObject temporaryAccountDrawing;
  public UIBButton temporaryAccountSignUpButton;
  public PegUIElement flyoutMenuButton;
  public GameObject flyoutMenu;
  public float flyoutMiddleFrameScaleOffsetForFSG;
  public float flyoutShadowScaleOffsetForFSG;
  public GameObject flyoutMiddleFrame;
  public GameObject flyoutMiddleShadow;
  public MultiSliceElement flyoutFrameContainer;
  public MultiSliceElement flyoutShadowContainer;
  public HighlightState flyoutButtonGlow;
  public GameObject friendFlyoutBone;
  private const int PatronCountHardLimit = 99;
  private const float UpdateItemsAfterScrollDelay = 0.5f;
  private float m_timeSinceLastScroll;
  private bool m_updateItemsAfterScroll;
  private AddFriendFrame m_addFriendFrame;
  private AlertPopup m_removeFriendPopup;
  private CameraOverridePass m_itemsCameraOverridePass;
  private FriendListFrame.FriendListEditMode m_editMode;
  private BnetPlayer m_friendToRemove;
  private bool m_flyoutOpen;
  private bool m_patronStrangersHidden;
  private SelectableMedal m_mySelectableMedal;
  private Coroutine m_updateFriendItemsWhenAvailableCoroutine;
  private List<FriendListFrame.PlayerUpdate> m_nearbyPlayerUpdates = new List<FriendListFrame.PlayerUpdate>();
  private List<FriendListFrame.PlayerUpdate> m_recentPlayerUpdates = new List<FriendListFrame.PlayerUpdate>();
  private BnetPlayerChangelist m_playersChangeList = new BnetPlayerChangelist();
  private float m_lastNearbyPlayersUpdate;
  private bool m_nearbyPlayersNeedUpdate;
  private const float NEARBY_PLAYERS_UPDATE_TIME = 10f;
  private bool m_recentPlayersNeedUpdate;
  private bool m_hasNearbyPlayers;
  private bool m_isRAFButtonEnabled = true;
  private bool m_isFSGButtonEnabled = true;
  private List<FriendListFrame.FriendListItem> m_allItems = new List<FriendListFrame.FriendListItem>();
  private FriendListFrame.VirtualizedFriendsListBehavior m_longListBehavior;
  private Dictionary<MobileFriendListItem.TypeFlags, FriendListItemHeader> m_headers = new Dictionary<MobileFriendListItem.TypeFlags, FriendListItemHeader>();

  public bool IsStarted { get; private set; }

  public bool ShowingAddFriendFrame => (UnityEngine.Object) this.m_addFriendFrame != (UnityEngine.Object) null;

  public bool IsInEditMode => this.m_editMode != 0;

  public FriendListFrame.FriendListEditMode EditMode => this.m_editMode;

  public bool IsFlyoutOpen => this.m_flyoutOpen;

  public event System.Action OnStarted;

  public event System.Action AddFriendFrameOpened;

  public event System.Action AddFriendFrameClosed;

  public event System.Action RemoveFriendPopupOpened;

  public event System.Action RemoveFriendPopupClosed;

  private void Awake()
  {
    this.InitButtons();
    this.RegisterFriendEvents();
    this.CreateItemsView();
    this.UpdateBackgroundCollider();
    bool flag = !UniversalInputManager.Get().IsTouchMode() || PlatformSettings.OS == OSCategory.PC;
    if ((UnityEngine.Object) this.scrollbar != (UnityEngine.Object) null)
      this.scrollbar.gameObject.SetActive(flag);
    if (BnetFriendMgr.Get().HasOnlineFriends() || BnetNearbyPlayerMgr.Get().HasNearbyStrangers())
      CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded();
    if (TemporaryAccountManager.IsTemporaryAccount())
    {
      this.items.GetComponent<BoxCollider>().enabled = false;
      this.temporaryAccountPaper.SetActive(true);
      this.temporaryAccountCover.SetActive(true);
      this.temporaryAccountDrawing.SetActive(true);
      this.temporaryAccountSignUpButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTemporaryAccountSignUpButtonPressed));
    }
    this.m_itemsCameraOverridePass.Schedule(CustomViewEntryPoint.BattleNetFriendList);
    Network.Get().OnDisconnectedFromBattleNet += new System.Action<BattleNetErrors>(this.OnDisconnectedFromBattleNet);
  }

  private void Start()
  {
    this.UpdateMyself();
    this.InitItems();
    this.UpdateRAFState();
    this.UpdateFSGState();
    TelemetryManager.Client().SendFriendsListView(SceneMgr.Get().GetMode().ToString());
    this.me.m_rankedMedalWidgetReference.RegisterReadyListener<Widget>(new System.Action<Widget>(this.OnMySelectableMedalWidgetReady));
    this.IsStarted = true;
    if (this.OnStarted == null)
      return;
    this.OnStarted();
  }

  private void OnDestroy()
  {
    this.m_itemsCameraOverridePass.Unschedule();
    this.UnregisterFriendEvents();
    this.CloseAddFriendFrame();
    if ((UnityEngine.Object) PegUI.Get() != (UnityEngine.Object) null)
      PegUI.Get().UnregisterFromRenderPassPriorityHitTest((Component) this);
    if (this.m_longListBehavior != null && this.m_longListBehavior.FreeList != null)
    {
      foreach (MobileFriendListItem free in this.m_longListBehavior.FreeList)
      {
        if ((UnityEngine.Object) free != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) free.gameObject);
      }
    }
    foreach (FriendListItemHeader friendListItemHeader in this.m_headers.Values)
    {
      if ((UnityEngine.Object) friendListItemHeader != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) friendListItemHeader.gameObject);
    }
    Network.Get().OnDisconnectedFromBattleNet -= new System.Action<BattleNetErrors>(this.OnDisconnectedFromBattleNet);
  }

  private void Update()
  {
    this.HandleKeyboardInput();
    bool flag = false;
    if (this.m_recentPlayersNeedUpdate)
    {
      this.HandleRecentPlayersChanged();
      flag = true;
    }
    if (this.m_nearbyPlayersNeedUpdate && (double) Time.realtimeSinceStartup >= (double) this.m_lastNearbyPlayersUpdate + 10.0)
    {
      this.HandleNearbyPlayersChanged();
      flag = true;
    }
    if (!this.m_updateItemsAfterScroll || (double) Time.realtimeSinceStartup - (double) this.m_timeSinceLastScroll <= 0.5)
      return;
    if (!flag)
      this.UpdateFriendItems();
    this.m_updateItemsAfterScroll = false;
  }

  private void OnEnable()
  {
    if (this.m_recentPlayersNeedUpdate)
      this.HandleRecentPlayersChanged();
    if (this.m_nearbyPlayersNeedUpdate)
      this.HandleNearbyPlayersChanged();
    if (this.m_playersChangeList.GetChanges().Count > 0)
    {
      this.DoPlayersChanged(this.m_playersChangeList);
      this.m_playersChangeList.GetChanges().Clear();
    }
    if (this.items.IsInitialized)
    {
      this.ResumeItemsLayout();
      this.SortAndRefreshTouchList();
    }
    this.UpdateMyself();
    this.items.ResetState();
    this.m_editMode = FriendListFrame.FriendListEditMode.NONE;
    this.m_friendToRemove = (BnetPlayer) null;
  }

  private void OnDisconnectedFromBattleNet(BattleNetErrors error)
  {
    this.m_longListBehavior.ReleaseAllItems();
    this.m_allItems.Clear();
  }

  public void SetWorldRect(float x, float z, float width, float height)
  {
    bool activeSelf = this.gameObject.activeSelf;
    this.gameObject.SetActive(true);
    this.window.SetEntireSize(width, height);
    Vector3 worldPoint = TransformUtil.ComputeWorldPoint(TransformUtil.ComputeSetPointBounds((Component) this.window), new Vector3(0.0f, 0.0f, 1f));
    this.transform.Translate(new Vector3(x, worldPoint.y, z) - worldPoint, Space.World);
    this.UpdateItemsList();
    this.UpdateItemsView();
    this.UpdateBackgroundCollider();
    this.UpdateDropShadow();
    this.gameObject.SetActive(activeSelf);
    if (!((UnityEngine.Object) this.temporaryAccountDrawingBone != (UnityEngine.Object) null) || !TemporaryAccountManager.IsTemporaryAccount())
      return;
    this.temporaryAccountDrawing.transform.position = this.temporaryAccountDrawingBone.transform.position;
  }

  public void SetWorldPosition(float x, float z) => this.SetWorldPosition(new Vector3(x, 0.0f, z));

  public void SetWorldPosition(Vector3 pos)
  {
    bool activeSelf = this.gameObject.activeSelf;
    this.gameObject.SetActive(true);
    this.transform.position = pos;
    this.UpdateItemsList();
    this.UpdateItemsView();
    this.UpdateBackgroundCollider();
    this.gameObject.SetActive(activeSelf);
    if (!((UnityEngine.Object) this.temporaryAccountDrawingBone != (UnityEngine.Object) null) || !TemporaryAccountManager.IsTemporaryAccount())
      return;
    this.temporaryAccountDrawing.transform.position = this.temporaryAccountDrawingBone.transform.position;
  }

  public void SetWorldHeight(float height)
  {
    bool activeSelf = this.gameObject.activeSelf;
    this.gameObject.SetActive(true);
    this.window.SetEntireHeight(height);
    this.UpdateItemsList();
    this.UpdateItemsView();
    this.UpdateBackgroundCollider();
    this.UpdateDropShadow();
    this.gameObject.SetActive(activeSelf);
    if (!((UnityEngine.Object) this.temporaryAccountDrawingBone != (UnityEngine.Object) null) || !TemporaryAccountManager.IsTemporaryAccount())
      return;
    this.temporaryAccountDrawing.transform.position = this.temporaryAccountDrawingBone.transform.position;
  }

  public void ShowAddFriendFrame(BnetPlayer player = null)
  {
    this.m_addFriendFrame = UnityEngine.Object.Instantiate<AddFriendFrame>(this.prefabs.addFriendFrame);
    this.m_addFriendFrame.Closed += new System.Action(this.CloseAddFriendFrame);
    if (player == null || BnetRecentPlayerMgr.Get().IsCurrentOpponent(player))
      return;
    this.m_addFriendFrame.SetPlayer(player);
  }

  public void CloseAddFriendFrame()
  {
    if ((UnityEngine.Object) this.m_addFriendFrame == (UnityEngine.Object) null)
      return;
    this.m_addFriendFrame.Close();
    if (this.AddFriendFrameClosed != null)
      this.AddFriendFrameClosed();
    this.m_addFriendFrame = (AddFriendFrame) null;
  }

  public void ShowRemoveFriendPopup(BnetPlayer friend)
  {
    this.m_friendToRemove = friend;
    if (this.m_friendToRemove == null)
      return;
    string uniqueName = FriendUtils.GetUniqueName(this.m_friendToRemove);
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_text = GameStrings.Format("GLOBAL_FRIENDLIST_REMOVE_FRIEND_ALERT_MESSAGE", (object) uniqueName),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnRemoveFriendPopupResponse)
    };
    DialogManager.Get().ShowPopup(info, new DialogManager.DialogProcessCallback(this.OnRemoveFriendDialogShown), (object) this.m_friendToRemove);
    if (this.RemoveFriendPopupOpened == null)
      return;
    this.RemoveFriendPopupOpened();
  }

  public void SelectFriend(BnetPlayer player)
  {
    foreach (FriendListFriendFrame renderedItem in this.GetRenderedItems<FriendListFriendFrame>())
    {
      Widget widget = renderedItem.GetWidget();
      if ((UnityEngine.Object) widget != (UnityEngine.Object) null)
      {
        if (renderedItem.GetFriend() == player)
          widget.TriggerEvent("SHOW_HIGHLIGHT");
        else
          widget.TriggerEvent("HIDE_HIGHLIGHT");
      }
    }
  }

  public void ClearHighlights()
  {
    foreach (FriendListFriendFrame renderedItem in this.GetRenderedItems<FriendListFriendFrame>())
    {
      Widget widget = renderedItem.GetWidget();
      if ((UnityEngine.Object) widget != (UnityEngine.Object) null)
        widget.TriggerEvent("HIDE_HIGHLIGHT");
    }
  }

  public void UpdateRAFButtonGlow()
  {
    this.rafButtonButtonGlow.SetActive(!Options.Get().GetBool(Option.HAS_SEEN_RAF) && this.m_isRAFButtonEnabled);
    this.UpdateFlyoutButtonGlow();
  }

  public void UpdateFSGButtonGlow()
  {
    this.fsgButtonButtonGlow.SetActive(!Options.Get().GetBool(Option.HAS_CLICKED_FIRESIDE_GATHERINGS_BUTTON) && this.m_isFSGButtonEnabled);
    this.UpdateFlyoutButtonGlow();
  }

  private void UpdateFlyoutButtonGlow() => this.flyoutButtonGlow.ChangeState(this.fsgButtonButtonGlow.activeSelf || this.rafButtonButtonGlow.activeSelf || this.IsFlyoutOpen ? ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE : ActorStateType.NONE);

  public OrientedBounds ComputeFrameWorldBounds() => TransformUtil.ComputeOrientedWorldBounds(this.gameObject, new List<GameObject>()
  {
    this.items.gameObject
  });

  public void SetRAFButtonEnabled(bool enabled)
  {
    if (this.m_isRAFButtonEnabled == enabled)
      return;
    this.m_isRAFButtonEnabled = enabled;
    this.rafButton.GetComponent<UIBHighlight>().EnableResponse = this.m_isRAFButtonEnabled;
    this.rafButtonEnabledVisual.SetActive(enabled);
    this.rafButtonDisabledVisual.SetActive(!enabled);
    if (this.m_isRAFButtonEnabled)
      this.rafButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRAFButtonReleased));
    else
      this.rafButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRAFButtonReleased));
    this.UpdateRAFButtonGlow();
  }

  public void SetFSGButtonEnabled()
  {
    bool flag = !FiresideGatheringManager.Get().IsCheckedIn;
    if (this.m_isFSGButtonEnabled == flag)
      return;
    this.m_isFSGButtonEnabled = flag;
    this.fsgButton.SetEnabled(this.m_isFSGButtonEnabled);
    this.SetupFSGButtonAndFixFrameLength(this.m_isFSGButtonEnabled, this.flyoutMiddleFrame.transform.localScale.y, this.flyoutMiddleShadow.transform.localScale.y);
    if (this.m_isFSGButtonEnabled)
      this.fsgButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnFSGButtonReleased));
    else
      this.fsgButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnFSGButtonReleased));
    this.UpdateFSGButtonGlow();
  }

  private void SetupFSGButtonAndFixFrameLength(
    bool enabled,
    float middleFrameScaleY,
    float middleShadowScaleY)
  {
    if (enabled)
    {
      this.fsgButton.gameObject.SetActive(true);
    }
    else
    {
      this.fsgButton.gameObject.SetActive(false);
      middleFrameScaleY -= this.flyoutMiddleFrameScaleOffsetForFSG;
      middleShadowScaleY -= this.flyoutShadowScaleOffsetForFSG;
      this.flyoutMiddleFrame.transform.localScale = new Vector3(this.flyoutMiddleFrame.transform.localScale.x, middleFrameScaleY, this.flyoutMiddleFrame.transform.localScale.z);
      this.flyoutMiddleShadow.transform.localScale = new Vector3(this.flyoutMiddleShadow.transform.localScale.x, middleShadowScaleY, this.flyoutMiddleShadow.transform.localScale.z);
      this.flyoutFrameContainer.UpdateSlices();
      this.flyoutShadowContainer.UpdateSlices();
    }
  }

  public void OpenFlyoutMenu()
  {
    if ((UnityEngine.Object) this.flyoutMenu == (UnityEngine.Object) null)
      return;
    this.m_flyoutOpen = true;
    this.flyoutMenu.SetActive(true);
    this.UpdateFlyoutButtonGlow();
  }

  public void CloseFlyoutMenu()
  {
    if ((UnityEngine.Object) this.flyoutMenu == (UnityEngine.Object) null)
      return;
    this.m_flyoutOpen = false;
    this.flyoutMenu.SetActive(false);
    this.UpdateFlyoutButtonGlow();
  }

  private void CreateItemsView()
  {
    if (this.m_itemsCameraOverridePass != null)
      return;
    this.m_itemsCameraOverridePass = new CameraOverridePass("FriendListFrameItemsView", (LayerMask) GameLayer.BattleNetFriendList.LayerBit());
    this.UpdateItemsView();
    if (!((UnityEngine.Object) PegUI.Get() != (UnityEngine.Object) null))
      return;
    PegUI.Get().RegisterForRenderPassPriorityHitTest((Component) this);
  }

  private void UpdateItemsList()
  {
    Transform bottomRightBone = this.GetBottomRightBone();
    Vector3 position1 = this.listInfo.topLeft.position;
    Vector3 position2 = bottomRightBone.position;
    this.items.transform.position = (position1 + position2) / 2f + Vector3.up * 5f;
    Vector3 vector3_1 = position2 - position1;
    this.items.ClipSize = new UnityEngine.Vector2(vector3_1.x, Math.Abs(vector3_1.z)) * 4f;
    if (!((UnityEngine.Object) this.innerShadow != (UnityEngine.Object) null))
      return;
    this.innerShadow.transform.position = this.items.transform.position;
    Vector3 vector3_2 = this.GetBottomRightBone().position - this.listInfo.topLeft.position;
    TransformUtil.SetLocalScaleToWorldDimension(this.innerShadow, new WorldDimensionIndex(Mathf.Abs(vector3_2.x), 0), new WorldDimensionIndex(Mathf.Abs(vector3_2.z), 2));
  }

  private void UpdateItemsView()
  {
    Camera bnetCamera = BaseUI.Get().GetBnetCamera();
    Transform bottomRightBone = this.GetBottomRightBone();
    Vector3 screenPoint1 = bnetCamera.WorldToScreenPoint(this.listInfo.topLeft.position);
    Vector3 screenPoint2 = bnetCamera.WorldToScreenPoint(bottomRightBone.position);
    GeneralUtils.Swap<float>(ref screenPoint1.y, ref screenPoint2.y);
    this.m_itemsCameraOverridePass.OverrideScissor(new Rect(screenPoint1.x, screenPoint1.y, screenPoint2.x - screenPoint1.x, screenPoint2.y - screenPoint1.y));
  }

  private void UpdateBackgroundCollider()
  {
    Renderer[] componentsInChildren = this.window.GetComponentsInChildren<Renderer>();
    Bounds bounds1 = new Bounds(this.transform.position, Vector3.zero);
    foreach (Renderer renderer in componentsInChildren)
    {
      Bounds bounds2 = renderer.bounds;
      if ((double) bounds2.size.x != 0.0)
      {
        bounds2 = renderer.bounds;
        if ((double) bounds2.size.y != 0.0)
        {
          bounds2 = renderer.bounds;
          if ((double) bounds2.size.z != 0.0)
            bounds1.Encapsulate(renderer.bounds);
        }
      }
    }
    Vector3 vector3_1 = this.transform.InverseTransformPoint(bounds1.min);
    Vector3 vector3_2 = this.transform.InverseTransformPoint(bounds1.max);
    BoxCollider boxCollider = this.GetComponent<BoxCollider>();
    if ((UnityEngine.Object) boxCollider == (UnityEngine.Object) null)
      boxCollider = this.gameObject.AddComponent<BoxCollider>();
    boxCollider.center = (vector3_1 + vector3_2) / 2f + Vector3.forward;
    boxCollider.size = vector3_2 - vector3_1;
  }

  private void UpdateDropShadow()
  {
    if ((UnityEngine.Object) this.outerShadow == (UnityEngine.Object) null)
      return;
    this.outerShadow.SetActive(!UniversalInputManager.Get().IsTouchMode());
  }

  private void UpdateMyself()
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    if (myPlayer != null && myPlayer.IsDisplayable())
    {
      BnetBattleTag battleTag = myPlayer.GetBattleTag();
      if (Options.Get().GetBool(Option.STREAMER_MODE))
        this.me.nameText.Text = string.Format("<color=#{0}>{1}</color>", (object) "5ecaf0ff", (object) GameStrings.Get("GAMEPLAY_HIDDEN_PLAYER_NAME"));
      else
        this.me.nameText.Text = string.Format("<color=#{0}>{1}</color> <size=32><color=#{2}>#{3}</color></size>", (object) "5ecaf0ff", (object) battleTag.GetName(), (object) "999999ff", (object) battleTag.GetNumber().ToString());
      MedalInfoTranslator rankPresenceField = RankMgr.Get().GetRankedMedalFromRankPresenceField(BnetPresenceMgr.Get().GetMyPlayer());
      this.UpdatePortrait((rankPresenceField != null && rankPresenceField.IsDisplayable()) | RankMgr.Get().GetBattlegroundsMedalFromRankPresenceField(BnetPresenceMgr.Get().GetMyPlayer().GetHearthstoneGameAccount(), out int _));
      this.UpdateMySelectableMedalWidget();
    }
    else
      this.me.nameText.Text = string.Empty;
  }

  private void InitItems()
  {
    BnetFriendMgr bnetFriendMgr = BnetFriendMgr.Get();
    BnetRecentPlayerMgr bnetRecentPlayerMgr = BnetRecentPlayerMgr.Get();
    BnetNearbyPlayerMgr bnetNearbyPlayerMgr = BnetNearbyPlayerMgr.Get();
    this.items.SelectionEnabled = true;
    this.items.SelectedIndexChanging += (TouchList.SelectedIndexChangingEvent) (index => index != -1);
    this.items.Scrolled += (System.Action) (() =>
    {
      this.m_timeSinceLastScroll = Time.realtimeSinceStartup;
      this.m_updateItemsAfterScroll = true;
    });
    this.SuspendItemsLayout();
    this.UpdateCurrentFiresideGatherings();
    this.UpdateFoundFiresideGatherings();
    this.InitFiresideGatheringPlayers();
    this.UpdateRequests(bnetFriendMgr.GetReceivedInvites(), (List<BnetInvitation>) null);
    this.UpdateAllFriends(bnetFriendMgr.GetFriends(), (List<BnetPlayer>) null);
    foreach (object recentPlayer in bnetRecentPlayerMgr.GetRecentPlayers())
      this.AddItem(new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.RecentPlayer, recentPlayer));
    foreach (object nearbyPlayer in bnetNearbyPlayerMgr.GetNearbyPlayers())
      this.AddItem(new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.NearbyPlayer, nearbyPlayer));
    this.UpdateAllHeaders();
    this.ResumeItemsLayout();
    this.SortAndRefreshTouchList();
    this.UpdateAllHeaderBackgrounds();
    this.UpdateSelectedItem();
    this.UpdateRAFButtonGlow();
    this.UpdateFSGButtonGlow();
    this.items.ScrollValue = 0.0f;
  }

  public void UpdateItems()
  {
    foreach (FriendListRequestFrame renderedItem in this.GetRenderedItems<FriendListRequestFrame>())
      renderedItem.UpdateInvite();
    this.UpdateFriendItems();
  }

  public void UpdateFriendItems()
  {
    if (this.m_updateFriendItemsWhenAvailableCoroutine != null)
      this.StopCoroutine(this.m_updateFriendItemsWhenAvailableCoroutine);
    foreach (FriendListFriendFrame renderedItem in this.GetRenderedItems<FriendListFriendFrame>())
      renderedItem.UpdateFriend();
  }

  public void UpdateFriendItemsWhenAvailable()
  {
    if (this.m_updateFriendItemsWhenAvailableCoroutine != null)
      this.StopCoroutine(this.m_updateFriendItemsWhenAvailableCoroutine);
    this.m_updateFriendItemsWhenAvailableCoroutine = this.StartCoroutine(this.UpdateFriendItemsWhenAvailableCoroutine());
  }

  private IEnumerator UpdateFriendItemsWhenAvailableCoroutine()
  {
    while (!FriendChallengeMgr.Get().AmIAvailable())
      yield return (object) null;
    this.m_updateFriendItemsWhenAvailableCoroutine = (Coroutine) null;
    this.UpdateFriendItems();
  }

  private void UpdateCurrentFiresideGatherings()
  {
    for (int index = this.m_allItems.Count - 1; index >= 0; --index)
    {
      if (this.m_allItems[index].ItemMainType == MobileFriendListItem.TypeFlags.CurrentFiresideGathering)
        this.m_allItems.RemoveAt(index);
    }
    FSGConfig currentFsg = FiresideGatheringManager.Get().CurrentFSG;
    if (currentFsg == null)
      return;
    this.AddItem(new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.CurrentFiresideGathering, (object) currentFsg));
  }

  private void UpdateFoundFiresideGatherings()
  {
    for (int index = this.m_allItems.Count - 1; index >= 0; --index)
    {
      if (this.m_allItems[index].ItemMainType == MobileFriendListItem.TypeFlags.FoundFiresideGathering)
        this.m_allItems.RemoveAt(index);
    }
    foreach (object fsG in FiresideGatheringManager.Get().GetFSGs())
      this.AddItem(new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.FoundFiresideGathering, fsG));
  }

  private void InitFiresideGatheringPlayers()
  {
    FiresideGatheringManager gatheringManager = FiresideGatheringManager.Get();
    this.UpdateFiresideGatheringPlayers(gatheringManager.DisplayablePatronList, (List<BnetPlayer>) null);
    if (!gatheringManager.CurrentFsgIsLargeScale)
      return;
    this.AddItem(new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.FiresideGatheringFooter, (object) GameStrings.Format("GLOBAL_FIRESIDE_GATHERING_PATRON_LIST_FOOTER_TEXT_LARGE_SCALE", (object) 99)));
  }

  private void UpdateFiresideGatheringPlayers(
    List<BnetPlayer> addedList,
    List<BnetPlayer> removedList)
  {
    if (FiresideGatheringManager.Get().DisplayablePatronCount >= FiresideGatheringManager.Get().FriendListPatronCountLimit)
    {
      this.m_patronStrangersHidden = true;
      if (removedList == null)
        removedList = new List<BnetPlayer>();
      List<BnetPlayer> collection = new List<BnetPlayer>();
      foreach (BnetPlayer displayablePatron in FiresideGatheringManager.Get().DisplayablePatronList)
      {
        if (!BnetFriendMgr.Get().IsFriend(displayablePatron))
          collection.Add(displayablePatron);
      }
      removedList.AddRange((IEnumerable<BnetPlayer>) collection);
      addedList?.RemoveAll(new Predicate<BnetPlayer>(collection.Contains));
      if (!this.m_allItems.Exists((Predicate<FriendListFrame.FriendListItem>) (item => item.ItemMainType == MobileFriendListItem.TypeFlags.FiresideGatheringFooter)))
      {
        int num = Mathf.Clamp(FiresideGatheringManager.Get().DisplayablePatronCount, FiresideGatheringManager.Get().FriendListPatronCountLimit, 99);
        string itemData;
        if (FiresideGatheringManager.Get().DisplayablePatronCount <= 99)
          itemData = GameStrings.Format("GLOBAL_FIRESIDE_GATHERING_PATRON_LIST_FOOTER_TEXT_SOFT_LIMIT", (object) num);
        else
          itemData = GameStrings.Format("GLOBAL_FIRESIDE_GATHERING_PATRON_LIST_FOOTER_TEXT_LARGE_SCALE", (object) 99);
        this.AddItem(new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.FiresideGatheringFooter, (object) itemData));
      }
    }
    else if (this.m_patronStrangersHidden)
    {
      this.m_patronStrangersHidden = false;
      this.RemoveItem(false, MobileFriendListItem.TypeFlags.FiresideGatheringFooter, (object) string.Empty);
      if (addedList == null)
        addedList = new List<BnetPlayer>();
      List<BnetPlayer> collection = new List<BnetPlayer>();
      foreach (BnetPlayer displayablePatron in FiresideGatheringManager.Get().DisplayablePatronList)
      {
        if (!BnetFriendMgr.Get().IsFriend(displayablePatron) && !addedList.Contains(displayablePatron))
          collection.Add(displayablePatron);
      }
      addedList.AddRange((IEnumerable<BnetPlayer>) collection);
    }
    if (removedList != null)
    {
      foreach (object removed in removedList)
        this.RemoveItem(false, MobileFriendListItem.TypeFlags.FiresideGatheringPlayer, removed);
    }
    if (addedList != null)
    {
      foreach (object added in addedList)
        this.AddItem(new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.FiresideGatheringPlayer, added));
    }
    this.UpdateFriendItems();
  }

  private void OnFiresideGatheringPresencePatronsUpdated(
    List<BnetPlayer> addedPatrons,
    List<BnetPlayer> removedPatrons)
  {
    this.UpdateFiresideGatheringPlayers(addedPatrons, removedPatrons);
    BnetFriendChangelist changelist = (BnetFriendChangelist) null;
    bool flag = false;
    if (addedPatrons != null)
    {
      foreach (BnetPlayer addedPatron in addedPatrons)
      {
        flag = true;
        if (BnetFriendMgr.Get().IsFriend(addedPatron))
        {
          if (changelist == null)
            changelist = new BnetFriendChangelist();
          changelist.AddRemovedFriend(addedPatron);
        }
      }
    }
    if (removedPatrons != null)
    {
      foreach (BnetPlayer removedPatron in removedPatrons)
      {
        flag = true;
        if (BnetFriendMgr.Get().IsFriend(removedPatron))
        {
          if (changelist == null)
            changelist = new BnetFriendChangelist();
          changelist.AddAddedFriend(removedPatron);
        }
      }
    }
    if (changelist != null)
    {
      this.OnFriendsChanged(changelist, (object) null);
    }
    else
    {
      if (!flag)
        return;
      this.SortAndRefreshTouchList();
    }
  }

  private void RemoveAllFiresideGatheringPlayers() => this.m_allItems.RemoveAll((Predicate<FriendListFrame.FriendListItem>) (item => item.ItemMainType == MobileFriendListItem.TypeFlags.FiresideGatheringPlayer || item.ItemMainType == MobileFriendListItem.TypeFlags.FiresideGatheringFooter));

  private void UpdateRequests(List<BnetInvitation> addedList, List<BnetInvitation> removedList)
  {
    if (removedList == null && addedList == null)
      return;
    if (removedList != null)
    {
      foreach (object removed in removedList)
        this.RemoveItem(false, MobileFriendListItem.TypeFlags.Request, removed);
    }
    foreach (FriendListRequestFrame renderedItem in this.GetRenderedItems<FriendListRequestFrame>())
      renderedItem.UpdateInvite();
    if (addedList == null)
      return;
    foreach (object added in addedList)
      this.AddItem(new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.Request, added));
  }

  private void UpdateAllFriends(List<BnetPlayer> addedList, List<BnetPlayer> removedList)
  {
    if (removedList == null && addedList == null)
      return;
    if (removedList != null)
    {
      foreach (object removed in removedList)
        this.RemoveItem(false, MobileFriendListItem.TypeFlags.Friend, removed);
    }
    this.UpdateFriendItems();
    if (addedList != null)
    {
      foreach (BnetPlayer added in addedList)
      {
        if (!FiresideGatheringManager.Get().IsPlayerInMyFSGAndDisplayable(added))
        {
          added.GetPersistentGameId();
          this.AddItem(new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.Friend, (object) added));
        }
      }
    }
    this.SortAndRefreshTouchList();
  }

  private FriendListFriendFrame FindRenderedBaseFriendFrame(BnetPlayer friend) => this.FindFirstRenderedItem<FriendListFriendFrame>((Predicate<FriendListFriendFrame>) (frame => frame.GetFriend() == friend));

  private void UpdateFriendFrame(BnetPlayer friend)
  {
    FriendListFriendFrame renderedBaseFriendFrame = this.FindRenderedBaseFriendFrame(friend);
    if (!((UnityEngine.Object) renderedBaseFriendFrame != (UnityEngine.Object) null))
      return;
    renderedBaseFriendFrame.UpdateFriend();
  }

  private MobileFriendListItem CreatePlayerFrame(
    BnetPlayer player,
    MobileFriendListItem.TypeFlags typeFlag)
  {
    FriendListFriendFrame friendListFriendFrame = UnityEngine.Object.Instantiate<FriendListFriendFrame>(this.prefabs.friendItem);
    UberText[] objs = UberText.EnableAllTextInObject(friendListFriendFrame.gameObject, false);
    friendListFriendFrame.Initialize(player, isRecentPlayerFrame: (typeFlag == MobileFriendListItem.TypeFlags.RecentPlayer));
    MobileFriendListItem visualItem = this.FinishCreateVisualItem<FriendListFriendFrame>(friendListFriendFrame, typeFlag, (ITouchListItem) this.FindHeader(typeFlag), friendListFriendFrame.gameObject);
    UberText.EnableAllTextObjects(objs, true);
    return visualItem;
  }

  private MobileFriendListItem CreateRequestFrame(BnetInvitation invite)
  {
    FriendListRequestFrame listRequestFrame = UnityEngine.Object.Instantiate<FriendListRequestFrame>(this.prefabs.requestItem);
    UberText[] objs = UberText.EnableAllTextInObject(listRequestFrame.gameObject, false);
    listRequestFrame.SetInvite(invite);
    MobileFriendListItem visualItem = this.FinishCreateVisualItem<FriendListRequestFrame>(listRequestFrame, MobileFriendListItem.TypeFlags.Request, (ITouchListItem) this.FindHeader(MobileFriendListItem.TypeFlags.Request), listRequestFrame.gameObject);
    UberText.EnableAllTextObjects(objs, true);
    return visualItem;
  }

  private MobileFriendListItem CreateFSGFrame(
    FSGConfig fsgConfig,
    MobileFriendListItem.TypeFlags flag)
  {
    FriendListFSGFrame friendListFsgFrame = UnityEngine.Object.Instantiate<FriendListFSGFrame>(this.prefabs.fsgItem);
    friendListFsgFrame.InitFrame(fsgConfig);
    return this.FinishCreateVisualItem<FriendListFSGFrame>(friendListFsgFrame, flag, (ITouchListItem) null, friendListFsgFrame.gameObject);
  }

  private MobileFriendListItem CreateFSGPlayerFrame(BnetPlayer friend)
  {
    FriendListFriendFrame friendListFriendFrame = UnityEngine.Object.Instantiate<FriendListFriendFrame>(this.prefabs.friendItem);
    UberText[] objs = UberText.EnableAllTextInObject(friendListFriendFrame.gameObject, false);
    friendListFriendFrame.Initialize(friend, true);
    MobileFriendListItem visualItem = this.FinishCreateVisualItem<FriendListFriendFrame>(friendListFriendFrame, MobileFriendListItem.TypeFlags.FiresideGatheringPlayer, (ITouchListItem) null, friendListFriendFrame.gameObject);
    UberText.EnableAllTextObjects(objs, true);
    return visualItem;
  }

  private MobileFriendListItem CreateFSGFooter(string text)
  {
    FriendListItemFooter friendListItemFooter = UnityEngine.Object.Instantiate<FriendListItemFooter>(this.prefabs.footerItem);
    friendListItemFooter.Text = text;
    return this.FinishCreateVisualItem<FriendListItemFooter>(friendListItemFooter, MobileFriendListItem.TypeFlags.FiresideGatheringFooter, (ITouchListItem) null, friendListItemFooter.gameObject);
  }

  private void UpdateAllHeaders()
  {
    this.UpdateRequestsHeader();
    if (NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().RecentFriendListDisplayEnabled)
      this.UpdatePlayersHeader(MobileFriendListItem.TypeFlags.RecentPlayer);
    this.UpdatePlayersHeader(MobileFriendListItem.TypeFlags.NearbyPlayer);
    this.UpdateFriendsHeader();
  }

  private void UpdateAllHeaderBackgrounds() => this.UpdateHeaderBackground(this.FindHeader(MobileFriendListItem.TypeFlags.Request));

  private void UpdateRequestsHeader(FriendListItemHeader header = null)
  {
    int num = 0;
    foreach (FriendListFrame.FriendListItem allItem in this.m_allItems)
    {
      if (allItem.ItemMainType == MobileFriendListItem.TypeFlags.Request)
        ++num;
    }
    if (num > 0)
    {
      string text = GameStrings.Format("GLOBAL_FRIENDLIST_REQUESTS_HEADER", (object) num);
      if ((UnityEngine.Object) header == (UnityEngine.Object) null)
      {
        header = this.FindOrCreateHeader(MobileFriendListItem.TypeFlags.Request);
        if (!this.DoesHeaderExist(MobileFriendListItem.TypeFlags.Request))
          this.AddItem(new FriendListFrame.FriendListItem(true, MobileFriendListItem.TypeFlags.Request, (object) null));
      }
      header.SetText(text);
    }
    else
    {
      if (!((UnityEngine.Object) header == (UnityEngine.Object) null))
        return;
      this.RemoveItem(true, MobileFriendListItem.TypeFlags.Request, (object) null);
    }
  }

  private void UpdatePlayersHeader(MobileFriendListItem.TypeFlags typeFlag)
  {
    int nearbyPlayerCount = 0;
    foreach (FriendListFrame.FriendListItem allItem in this.m_allItems)
    {
      if (allItem.ItemMainType == typeFlag && (typeFlag != MobileFriendListItem.TypeFlags.NearbyPlayer || !FiresideGatheringManager.Get().IsPlayerInMyFSGAndDisplayable(allItem.GetNearbyPlayer())))
        ++nearbyPlayerCount;
    }
    FriendListItemHeader orCreateHeader = this.FindOrCreateHeader(typeFlag);
    if (!this.DoesHeaderExist(typeFlag))
      this.AddItem(new FriendListFrame.FriendListItem(true, typeFlag, (object) null));
    switch (typeFlag)
    {
      case MobileFriendListItem.TypeFlags.NearbyPlayer:
        this.m_hasNearbyPlayers = nearbyPlayerCount > 0;
        string text = GameStrings.Format("GLOBAL_FRIENDLIST_NEARBY_PLAYERS_HEADER", (object) nearbyPlayerCount);
        FriendListNearbyPlayersHeader nearbyPlayersHeader = orCreateHeader as FriendListNearbyPlayersHeader;
        if ((UnityEngine.Object) nearbyPlayersHeader != (UnityEngine.Object) null)
        {
          nearbyPlayersHeader.SetText(nearbyPlayerCount);
          break;
        }
        orCreateHeader.SetText(text);
        Debug.LogError((object) "FriendListFrame: Could not cast header to type FriendListNearbyPlayersHeader.");
        break;
      case MobileFriendListItem.TypeFlags.RecentPlayer:
        orCreateHeader.SetText(GameStrings.Format("GLOBAL_FRIENDLIST_RECENT_PLAYERS_HEADER", (object) nearbyPlayerCount));
        break;
    }
    if (!((UnityEngine.Object) orCreateHeader != (UnityEngine.Object) null))
      return;
    orCreateHeader.SetToggleEnabled(false);
  }

  private List<FriendListFrame.FriendListItem> GetFriendItems()
  {
    List<FriendListFrame.FriendListItem> friendItems = new List<FriendListFrame.FriendListItem>();
    foreach (FriendListFrame.FriendListItem allItem in this.m_allItems)
    {
      if (allItem.ItemMainType == MobileFriendListItem.TypeFlags.Friend)
        friendItems.Add(allItem);
    }
    return friendItems;
  }

  private void UpdateFriendsHeader(FriendListItemHeader header = null)
  {
    List<FriendListFrame.FriendListItem> friendItems = this.GetFriendItems();
    int num = 0;
    foreach (FriendListFrame.FriendListItem friendListItem in friendItems)
    {
      BnetPlayer friend = friendListItem.GetFriend();
      if (friend.IsOnline() && !FiresideGatheringManager.Get().IsPlayerInMyFSGAndDisplayable(friend))
        ++num;
    }
    int count = friendItems.Count;
    string text;
    if (num == count)
      text = GameStrings.Format("GLOBAL_FRIENDLIST_FRIENDS_HEADER_ALL_ONLINE", (object) num);
    else
      text = GameStrings.Format("GLOBAL_FRIENDLIST_FRIENDS_HEADER", (object) num, (object) count);
    if ((UnityEngine.Object) header == (UnityEngine.Object) null)
    {
      header = this.FindOrCreateHeader(MobileFriendListItem.TypeFlags.Friend);
      if (!this.DoesHeaderExist(MobileFriendListItem.TypeFlags.Friend))
        this.AddItem(new FriendListFrame.FriendListItem(true, MobileFriendListItem.TypeFlags.Friend, (object) null));
    }
    header.SetText(text);
    header.SetToggleEnabled(false);
  }

  private void UpdateHeaderBackground(FriendListItemHeader itemHeader)
  {
    if ((UnityEngine.Object) itemHeader == (UnityEngine.Object) null)
      return;
    MobileFriendListItem component = itemHeader.GetComponent<MobileFriendListItem>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    TiledBackground tiledBackground;
    if ((UnityEngine.Object) itemHeader.Background == (UnityEngine.Object) null)
    {
      GameObject go = new GameObject("ItemsBackground");
      go.transform.parent = component.transform;
      TransformUtil.Identity(go);
      go.layer = 24;
      FriendListFrame.HeaderBackgroundInfo headerBackgroundInfo = (component.Type & MobileFriendListItem.TypeFlags.Request) != (MobileFriendListItem.TypeFlags) 0 ? this.listInfo.requestBackgroundInfo : this.listInfo.currentGameBackgroundInfo;
      go.AddComponent<MeshFilter>().mesh = headerBackgroundInfo.mesh;
      go.AddComponent<MeshRenderer>().SetMaterial(headerBackgroundInfo.material);
      tiledBackground = go.AddComponent<TiledBackground>();
      itemHeader.Background = go;
    }
    else
      tiledBackground = itemHeader.Background.GetComponent<TiledBackground>();
    tiledBackground.transform.parent = (Transform) null;
    MobileFriendListItem.TypeFlags typeFlags = component.Type ^ MobileFriendListItem.TypeFlags.Header;
    Bounds bounds = new Bounds(component.transform.position, Vector3.zero);
    foreach (ITouchListItem renderedItem in this.items.RenderedItems)
    {
      MobileFriendListItem mobileFriendListItem = renderedItem as MobileFriendListItem;
      if ((UnityEngine.Object) mobileFriendListItem != (UnityEngine.Object) null && (mobileFriendListItem.Type & typeFlags) != (MobileFriendListItem.TypeFlags) 0)
        bounds.Encapsulate(mobileFriendListItem.ComputeWorldBounds());
    }
    tiledBackground.transform.parent = component.transform.parent.transform;
    bounds.center = tiledBackground.transform.parent.transform.InverseTransformPoint(bounds.center);
    tiledBackground.SetBounds(bounds);
    TransformUtil.SetPosZ((Component) tiledBackground.transform, 2f);
    tiledBackground.gameObject.SetActive(itemHeader.IsShowingContents);
  }

  private FriendListItemHeader FindHeader(MobileFriendListItem.TypeFlags type)
  {
    type |= MobileFriendListItem.TypeFlags.Header;
    FriendListItemHeader header;
    this.m_headers.TryGetValue(type, out header);
    return header;
  }

  private bool DoesHeaderExist(MobileFriendListItem.TypeFlags type)
  {
    foreach (FriendListFrame.FriendListItem allItem in this.m_allItems)
    {
      if (allItem.IsHeader && allItem.SubType == type)
        return true;
    }
    return false;
  }

  private FriendListItemHeader FindOrCreateHeader(
    MobileFriendListItem.TypeFlags type)
  {
    type |= MobileFriendListItem.TypeFlags.Header;
    FriendListItemHeader userdata = this.FindHeader(type);
    if ((UnityEngine.Object) userdata == (UnityEngine.Object) null)
    {
      FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(true, type, (object) null);
      if (type == (MobileFriendListItem.TypeFlags.NearbyPlayer | MobileFriendListItem.TypeFlags.Header))
      {
        userdata = (FriendListItemHeader) UnityEngine.Object.Instantiate<FriendListNearbyPlayersHeader>(this.prefabs.nearbyPlayersHeaderItem);
        ((FriendListNearbyPlayersHeader) userdata).OnPanelOpened += new System.Action(this.CloseFlyoutMenu);
      }
      else
        userdata = UnityEngine.Object.Instantiate<FriendListItemHeader>(this.prefabs.headerItem);
      this.m_headers[type] = userdata;
      Option setoption = Option.FRIENDS_LIST_FRIEND_SECTION_HIDE;
      switch (friendListItem.SubType)
      {
        case MobileFriendListItem.TypeFlags.Friend:
        case MobileFriendListItem.TypeFlags.CurrentGame:
          setoption = Option.FRIENDS_LIST_FRIEND_SECTION_HIDE;
          break;
        case MobileFriendListItem.TypeFlags.NearbyPlayer:
          setoption = Option.FRIENDS_LIST_NEARBYPLAYER_SECTION_HIDE;
          break;
        case MobileFriendListItem.TypeFlags.Request:
          setoption = Option.FRIENDS_LIST_REQUEST_SECTION_HIDE;
          break;
      }
      userdata.SubType = friendListItem.SubType;
      userdata.Option = setoption;
      bool showHeaderSection = this.GetShowHeaderSection(setoption);
      userdata.SetInitialShowContents(showHeaderSection);
      userdata.ClearToggleListeners();
      userdata.AddToggleListener(new FriendListItemHeader.ToggleContentsFunc(this.OnHeaderSectionToggle), (object) userdata);
      UberText[] objs = UberText.EnableAllTextInObject(userdata.gameObject, false);
      this.FinishCreateVisualItem<FriendListItemHeader>(userdata, type, (ITouchListItem) null, (GameObject) null);
      UberText.EnableAllTextObjects(objs, true);
    }
    return userdata;
  }

  private void OnHeaderSectionToggle(bool show, object userdata)
  {
    FriendListItemHeader header = (FriendListItemHeader) userdata;
    this.SetShowHeaderSection(header.Option, show);
    this.items.RefreshList(this.m_allItems.FindIndex((Predicate<FriendListFrame.FriendListItem>) (item => item.IsHeader && item.SubType == header.SubType)), true);
    this.UpdateHeaderBackground(header);
  }

  public T FindFirstRenderedItem<T>(Predicate<T> predicate = null) where T : MonoBehaviour
  {
    foreach (ITouchListItem renderedItem in this.items.RenderedItems)
    {
      T component = renderedItem.GetComponent<T>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null && (predicate == null || predicate(component)))
        return component;
    }
    return default (T);
  }

  private List<T> GetRenderedItems<T>() where T : MonoBehaviour
  {
    List<T> renderedItems = new List<T>();
    foreach (ITouchListItem renderedItem in this.items.RenderedItems)
    {
      T component = renderedItem.GetComponent<T>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        renderedItems.Add(component);
    }
    return renderedItems;
  }

  private MobileFriendListItem FinishCreateVisualItem<T>(
    T obj,
    MobileFriendListItem.TypeFlags type,
    ITouchListItem parent,
    GameObject showObj)
    where T : MonoBehaviour
  {
    MobileFriendListItem visualItem = obj.gameObject.GetComponent<MobileFriendListItem>();
    if ((UnityEngine.Object) visualItem == (UnityEngine.Object) null)
    {
      visualItem = obj.gameObject.AddComponent<MobileFriendListItem>();
      if ((object) obj is FriendListFriendFrame)
        ((FriendListFriendFrame) (object) obj).InitializeMobileFriendListItem(visualItem);
      BoxCollider component = visualItem.GetComponent<BoxCollider>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.size = new Vector3(component.size.x, component.size.y, component.size.z + this.items.elementSpacing);
    }
    visualItem.Type = type;
    visualItem.SetShowObject(showObj);
    visualItem.SetParent(parent);
    if (visualItem.Selectable)
    {
      BnetPlayer selectedFriend = FriendMgr.Get().GetSelectedFriend();
      if (selectedFriend != null)
      {
        BnetPlayer bnetPlayer = (BnetPlayer) null;
        if ((object) obj is FriendListFriendFrame)
          bnetPlayer = ((FriendListFriendFrame) (object) obj).GetFriend();
        if (bnetPlayer != null && selectedFriend == bnetPlayer)
          visualItem.Selected();
      }
    }
    return visualItem;
  }

  private bool RemoveItem(bool isHeader, MobileFriendListItem.TypeFlags type, object itemToRemove)
  {
    int index = this.m_allItems.FindIndex((Predicate<FriendListFrame.FriendListItem>) (item =>
    {
      if (item.IsHeader != isHeader || item.SubType != type)
        return false;
      if (itemToRemove == null)
        return true;
      switch (type)
      {
        case MobileFriendListItem.TypeFlags.Friend:
        case MobileFriendListItem.TypeFlags.CurrentGame:
          return item.GetFriend() == (BnetPlayer) itemToRemove;
        case MobileFriendListItem.TypeFlags.NearbyPlayer:
          return item.GetNearbyPlayer() == (BnetPlayer) itemToRemove;
        case MobileFriendListItem.TypeFlags.RecentPlayer:
          return item.GetRecentPlayer() == (BnetPlayer) itemToRemove;
        case MobileFriendListItem.TypeFlags.FiresideGatheringFooter:
          return item.ItemMainType == MobileFriendListItem.TypeFlags.FiresideGatheringFooter;
        case MobileFriendListItem.TypeFlags.FiresideGatheringPlayer:
          return item.GetFiresideGatheringPlayer() == (BnetPlayer) itemToRemove;
        case MobileFriendListItem.TypeFlags.CurrentFiresideGathering:
        case MobileFriendListItem.TypeFlags.FoundFiresideGathering:
          return item.GetFSGConfig() == (FSGConfig) itemToRemove;
        case MobileFriendListItem.TypeFlags.Request:
          return item.GetInvite() == (BnetInvitation) itemToRemove;
        default:
          return false;
      }
    }));
    if (index < 0)
      return false;
    this.m_allItems.RemoveAt(index);
    return true;
  }

  private void AddItem(FriendListFrame.FriendListItem itemToAdd) => this.m_allItems.Add(itemToAdd);

  private void SuspendItemsLayout() => this.items.SuspendLayout();

  private void ResumeItemsLayout() => this.items.ResumeLayout(false);

  public void ToggleRemoveFriendsMode()
  {
    FriendListFrame.FriendListEditMode mode;
    switch (this.m_editMode)
    {
      case FriendListFrame.FriendListEditMode.NONE:
        mode = FriendListFrame.FriendListEditMode.REMOVE_FRIENDS;
        break;
      case FriendListFrame.FriendListEditMode.REMOVE_FRIENDS:
        mode = FriendListFrame.FriendListEditMode.NONE;
        break;
      default:
        Log.All.PrintError("FriendListFrame: Should not be toggling Remove Friends mode when in mode {0}!", (object) this.m_editMode);
        return;
    }
    this.SetEditFriendsMode(mode);
    this.removeFriendButtonDisabledVisual.SetActive(this.m_editMode == FriendListFrame.FriendListEditMode.REMOVE_FRIENDS);
    this.removeFriendButtonEnabledVisual.SetActive(this.m_editMode == FriendListFrame.FriendListEditMode.NONE);
    this.removeFriendButtonButtonGlow.SetActive(this.m_editMode == FriendListFrame.FriendListEditMode.REMOVE_FRIENDS);
  }

  private bool SetEditFriendsMode(FriendListFrame.FriendListEditMode mode)
  {
    this.m_editMode = mode;
    this.SortAndRefreshTouchList();
    this.UpdateFriendItems();
    return true;
  }

  public void ExitRemoveFriendsMode()
  {
    if (this.m_editMode != FriendListFrame.FriendListEditMode.REMOVE_FRIENDS)
      return;
    this.ToggleRemoveFriendsMode();
  }

  private void SortAndRefreshTouchList()
  {
    if (this.items.IsLayoutSuspended)
      return;
    this.m_allItems.Sort(new Comparison<FriendListFrame.FriendListItem>(this.ItemsSortCompare));
    if (this.m_longListBehavior == null)
    {
      this.m_longListBehavior = new FriendListFrame.VirtualizedFriendsListBehavior(this);
      this.items.LongListBehavior = (TouchList.ILongListBehavior) this.m_longListBehavior;
    }
    else
      this.items.RefreshList(0, true);
  }

  private int ItemsSortCompare(
    FriendListFrame.FriendListItem item1,
    FriendListFrame.FriendListItem item2)
  {
    if (!this.m_hasNearbyPlayers)
    {
      if ((item1.ItemFlags & MobileFriendListItem.TypeFlags.NearbyPlayer) != (MobileFriendListItem.TypeFlags) 0)
        return 1;
      if ((item2.ItemFlags & MobileFriendListItem.TypeFlags.NearbyPlayer) != (MobileFriendListItem.TypeFlags) 0)
        return -1;
    }
    int num1 = item2.ItemFlags.CompareTo((object) item1.ItemFlags);
    if (num1 != 0)
      return num1;
    switch (item1.ItemFlags)
    {
      case MobileFriendListItem.TypeFlags.Friend:
      case MobileFriendListItem.TypeFlags.CurrentGame:
        return FriendUtils.FriendSortCompare(item1.GetFriend(), item2.GetFriend());
      case MobileFriendListItem.TypeFlags.NearbyPlayer:
        return FriendUtils.FriendSortCompare(item1.GetNearbyPlayer(), item2.GetNearbyPlayer());
      case MobileFriendListItem.TypeFlags.RecentPlayer:
        return FriendUtils.RecentFriendSortCompare(item1.GetRecentPlayer(), item2.GetRecentPlayer());
      case MobileFriendListItem.TypeFlags.FiresideGatheringPlayer:
        return FiresideGatheringManager.Get().FiresideGatheringPlayerSort(item1.GetFiresideGatheringPlayer(), item2.GetFiresideGatheringPlayer());
      case MobileFriendListItem.TypeFlags.Request:
        BnetInvitation invite1 = item1.GetInvite();
        BnetInvitation invite2 = item2.GetInvite();
        int num2 = string.Compare(invite1.GetInviterName(), invite2.GetInviterName(), true);
        return num2 != 0 ? num2 : (int) ((long) invite1.GetInviterId().Low - (long) invite2.GetInviterId().Low);
      case MobileFriendListItem.TypeFlags.FoundFiresideGathering:
        FSGConfig fsgConfig1 = item1.GetFSGConfig();
        FSGConfig fsgConfig2 = item2.GetFSGConfig();
        return FiresideGatheringManager.Get().FiresideGatheringSort(fsgConfig1, fsgConfig2);
      default:
        return 0;
    }
  }

  private void RegisterFriendEvents()
  {
    BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    FriendChallengeMgr.Get().AddChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
    BnetRecentPlayerMgr.Get().AddChangeListener(new BnetRecentPlayerMgr.ChangeCallback(this.OnRecentPlayersChanged));
    BnetNearbyPlayerMgr.Get().AddChangeListener(new BnetNearbyPlayerMgr.ChangeCallback(this.OnNearbyPlayersChanged));
    SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    SpectatorManager.Get().OnInviteReceived += new SpectatorManager.InviteReceivedHandler(this.SpectatorManager_OnInviteReceivedOrSent);
    SpectatorManager.Get().OnInviteSent += new SpectatorManager.InviteSentHandler(this.SpectatorManager_OnInviteReceivedOrSent);
    Network.Get().RegisterNetHandler((object) RequestNearbyFSGsResponse.PacketID.ID, new Network.NetHandler(this.OnRequestNearbyFSGsResponse));
    FiresideGatheringManager.Get().OnJoinFSG += new FiresideGatheringManager.CheckedInToFSGCallback(this.OnJoinFSG);
    FiresideGatheringManager.Get().OnLeaveFSG += new FiresideGatheringManager.CheckedOutOfFSGCallback(this.OnLeaveFSG);
    FiresideGatheringManager.OnPatronListUpdated += new FiresideGatheringManager.OnPatronListUpdatedCallback(this.OnFiresideGatheringPresencePatronsUpdated);
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheFeatures), new System.Action(this.UpdateFSGState));
    NetCache.Get().RegisterUpdatedListener(typeof (FSGFeatureConfig), new System.Action(this.UpdateFSGState));
  }

  private void UnregisterFriendEvents()
  {
    BnetFriendMgr.RemoveChangeListenerFromInstance(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    BnetPresenceMgr.RemovePlayersChangedListenerFromInstance(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    FriendChallengeMgr.RemoveChangedListenerFromInstance(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
    BnetRecentPlayerMgr.Get()?.RemoveChangeListenerFromInstance(new BnetRecentPlayerMgr.ChangeCallback(this.OnRecentPlayersChanged));
    BnetNearbyPlayerMgr.RemoveChangeListenerFromInstance(new BnetNearbyPlayerMgr.ChangeCallback(this.OnNearbyPlayersChanged));
    SceneMgr.Get()?.UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    if (SpectatorManager.InstanceExists())
    {
      SpectatorManager spectatorManager = SpectatorManager.Get();
      spectatorManager.OnInviteReceived -= new SpectatorManager.InviteReceivedHandler(this.SpectatorManager_OnInviteReceivedOrSent);
      spectatorManager.OnInviteSent -= new SpectatorManager.InviteSentHandler(this.SpectatorManager_OnInviteReceivedOrSent);
    }
    Network.Get()?.RemoveNetHandler((object) RequestNearbyFSGsResponse.PacketID.ID, new Network.NetHandler(this.OnRequestNearbyFSGsResponse));
    FiresideGatheringManager gatheringManager = FiresideGatheringManager.Get();
    if (gatheringManager != null)
    {
      gatheringManager.OnJoinFSG -= new FiresideGatheringManager.CheckedInToFSGCallback(this.OnJoinFSG);
      gatheringManager.OnLeaveFSG -= new FiresideGatheringManager.CheckedOutOfFSGCallback(this.OnLeaveFSG);
      FiresideGatheringManager.OnPatronListUpdated -= new FiresideGatheringManager.OnPatronListUpdatedCallback(this.OnFiresideGatheringPresencePatronsUpdated);
    }
    NetCache.Get()?.RemoveUpdatedListener(typeof (NetCache.NetCacheFeatures), new System.Action(this.UpdateFSGState));
    NetCache.Get()?.RemoveUpdatedListener(typeof (FSGFeatureConfig), new System.Action(this.UpdateFSGState));
  }

  private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
  {
    this.SuspendItemsLayout();
    this.UpdateRequests(changelist.GetAddedReceivedInvites(), changelist.GetRemovedReceivedInvites());
    this.UpdateAllFriends(changelist.GetAddedFriends(), changelist.GetRemovedFriends());
    this.UpdateAllHeaders();
    this.ResumeItemsLayout();
    this.SortAndRefreshTouchList();
    this.UpdateAllHeaderBackgrounds();
    this.UpdateSelectedItem();
  }

  private void OnRecentPlayersChanged(
    BnetRecentOrNearbyPlayerChangelist changelist,
    object userData)
  {
    this.m_recentPlayersNeedUpdate = true;
    this.OnPlayerChangedCommon(changelist, this.m_recentPlayerUpdates);
    if (!this.gameObject.activeInHierarchy)
      return;
    this.HandleRecentPlayersChanged();
  }

  private void OnNearbyPlayersChanged(
    BnetRecentOrNearbyPlayerChangelist changelist,
    object userData)
  {
    this.m_nearbyPlayersNeedUpdate = true;
    this.OnPlayerChangedCommon(changelist, this.m_nearbyPlayerUpdates);
    if (!this.gameObject.activeInHierarchy || (double) Time.realtimeSinceStartup < (double) this.m_lastNearbyPlayersUpdate + 10.0)
      return;
    this.HandleNearbyPlayersChanged();
  }

  private void OnPlayerChangedCommon(
    BnetRecentOrNearbyPlayerChangelist changelist,
    List<FriendListFrame.PlayerUpdate> playerUpdates)
  {
    if (changelist.GetAddedStrangers() != null)
    {
      foreach (BnetPlayer addedStranger in changelist.GetAddedStrangers())
      {
        FriendListFrame.PlayerUpdate playerUpdate = new FriendListFrame.PlayerUpdate(FriendListFrame.PlayerUpdate.ChangeType.Added, addedStranger);
        playerUpdates.Remove(playerUpdate);
        playerUpdates.Add(playerUpdate);
      }
    }
    if (changelist.GetRemovedStrangers() != null)
    {
      foreach (BnetPlayer removedStranger in changelist.GetRemovedStrangers())
      {
        FriendListFrame.PlayerUpdate playerUpdate = new FriendListFrame.PlayerUpdate(FriendListFrame.PlayerUpdate.ChangeType.Removed, removedStranger);
        playerUpdates.Remove(playerUpdate);
        playerUpdates.Add(playerUpdate);
      }
    }
    if (changelist.GetAddedFriends() != null)
    {
      foreach (BnetPlayer addedFriend in changelist.GetAddedFriends())
      {
        FriendListFrame.PlayerUpdate playerUpdate = new FriendListFrame.PlayerUpdate(FriendListFrame.PlayerUpdate.ChangeType.Added, addedFriend);
        playerUpdates.Remove(playerUpdate);
        playerUpdates.Add(playerUpdate);
      }
    }
    if (changelist.GetRemovedFriends() != null)
    {
      foreach (BnetPlayer removedFriend in changelist.GetRemovedFriends())
      {
        FriendListFrame.PlayerUpdate playerUpdate = new FriendListFrame.PlayerUpdate(FriendListFrame.PlayerUpdate.ChangeType.Removed, removedFriend);
        playerUpdates.Remove(playerUpdate);
        playerUpdates.Add(playerUpdate);
      }
    }
    if (changelist.GetAddedPlayers() != null)
    {
      foreach (BnetPlayer addedPlayer in changelist.GetAddedPlayers())
      {
        FriendListFrame.PlayerUpdate playerUpdate = new FriendListFrame.PlayerUpdate(FriendListFrame.PlayerUpdate.ChangeType.Added, addedPlayer);
        playerUpdates.Remove(playerUpdate);
        playerUpdates.Add(playerUpdate);
      }
    }
    if (changelist.GetRemovedPlayers() == null)
      return;
    foreach (BnetPlayer removedPlayer in changelist.GetRemovedPlayers())
    {
      FriendListFrame.PlayerUpdate playerUpdate = new FriendListFrame.PlayerUpdate(FriendListFrame.PlayerUpdate.ChangeType.Removed, removedPlayer);
      playerUpdates.Remove(playerUpdate);
      playerUpdates.Add(playerUpdate);
    }
  }

  private void HandleRecentPlayersChanged()
  {
    if (!this.m_recentPlayersNeedUpdate)
      return;
    this.HandlePlayersChangedCommon(this.m_recentPlayerUpdates, MobileFriendListItem.TypeFlags.RecentPlayer);
    this.m_recentPlayersNeedUpdate = false;
  }

  private void HandleNearbyPlayersChanged()
  {
    if (!this.m_nearbyPlayersNeedUpdate)
      return;
    this.HandlePlayersChangedCommon(this.m_nearbyPlayerUpdates, MobileFriendListItem.TypeFlags.NearbyPlayer);
    this.m_nearbyPlayersNeedUpdate = false;
    this.m_lastNearbyPlayersUpdate = Time.realtimeSinceStartup;
  }

  private void HandlePlayersChangedCommon(
    List<FriendListFrame.PlayerUpdate> playerUpdates,
    MobileFriendListItem.TypeFlags typeFlag)
  {
    this.UpdateFriendItems();
    BnetFriendChangelist changelist = (BnetFriendChangelist) null;
    if (playerUpdates.Count > 0)
    {
      this.SuspendItemsLayout();
      foreach (FriendListFrame.PlayerUpdate playerUpdate in playerUpdates)
      {
        if (playerUpdate.Change == FriendListFrame.PlayerUpdate.ChangeType.Added)
        {
          if (typeFlag != MobileFriendListItem.TypeFlags.NearbyPlayer || !FiresideGatheringManager.Get().IsPlayerInMyFSGAndDisplayable(playerUpdate.Player))
            this.AddItem(new FriendListFrame.FriendListItem(false, typeFlag, (object) playerUpdate.Player));
        }
        else
          this.RemoveItem(false, typeFlag, (object) playerUpdate.Player);
      }
      playerUpdates.Clear();
      this.UpdateAllHeaders();
      this.ResumeItemsLayout();
      this.SortAndRefreshTouchList();
      this.UpdateAllHeaderBackgrounds();
      this.UpdateSelectedItem();
    }
    if (changelist == null)
      return;
    this.OnFriendsChanged(changelist, (object) null);
  }

  private void DoPlayersChanged(BnetPlayerChangelist changelist)
  {
    this.SuspendItemsLayout();
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    bool flag1 = false;
    bool flag2 = false;
    foreach (BnetPlayerChange change in changelist.GetChanges())
    {
      BnetPlayer oldPlayer = change.GetOldPlayer();
      BnetPlayer newPlayer = change.GetNewPlayer();
      if (newPlayer == myPlayer)
      {
        this.UpdateMyself();
        BnetGameAccount hearthstoneGameAccount = newPlayer.GetHearthstoneGameAccount();
        flag1 = oldPlayer == null || oldPlayer.GetHearthstoneGameAccount() == (BnetGameAccount) null ? hearthstoneGameAccount.CanBeInvitedToGame() : oldPlayer.GetHearthstoneGameAccount().CanBeInvitedToGame() != hearthstoneGameAccount.CanBeInvitedToGame();
      }
      else
      {
        if (oldPlayer == null || oldPlayer.GetBestName() != newPlayer.GetBestName())
          flag2 = true;
        this.UpdateFriendFrame(newPlayer);
      }
    }
    if (flag1)
      this.UpdateItems();
    else if (flag2)
      this.UpdateFriendItems();
    this.UpdateAllHeaders();
    this.UpdateAllHeaderBackgrounds();
    this.ResumeItemsLayout();
  }

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    if (this.gameObject.activeInHierarchy)
    {
      this.DoPlayersChanged(changelist);
    }
    else
    {
      List<BnetPlayerChange> changes = changelist.GetChanges();
      this.m_playersChangeList.GetChanges().AddRange((IEnumerable<BnetPlayerChange>) changes);
    }
  }

  private void OnRequestNearbyFSGsResponse()
  {
    Log.FiresideGatherings.Print("FriendListFrame.OnNearbyFSGsResponse");
    this.UpdateCurrentFiresideGatherings();
    this.UpdateFoundFiresideGatherings();
    this.SortAndRefreshTouchList();
  }

  private void OnJoinFSG(FSGConfig gathering)
  {
    Log.FiresideGatherings.Print("FriendListFrame.OnJoinFSG");
    this.UpdateCurrentFiresideGatherings();
    this.UpdateFiresideGatheringPlayers(FiresideGatheringManager.Get().DisplayablePatronList, (List<BnetPlayer>) null);
    this.SortAndRefreshTouchList();
    this.UpdateFSGState();
  }

  private void OnLeaveFSG(FSGConfig gathering)
  {
    Log.FiresideGatherings.Print("FriendListFrame.OnLeaveFSG");
    this.UpdateFoundFiresideGatherings();
    this.RemoveAllFiresideGatheringPlayers();
    this.SortAndRefreshTouchList();
    this.UpdateFSGState();
  }

  private void OnScenePreUnload(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.FRIENDLY:
      case SceneMgr.Mode.FATAL_ERROR:
        if ((UnityEngine.Object) ChatMgr.Get() != (UnityEngine.Object) null)
        {
          ChatMgr.Get().CloseFriendsList();
          break;
        }
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        break;
    }
  }

  private void SpectatorManager_OnInviteReceivedOrSent(OnlineEventType evt, BnetPlayer inviter) => this.UpdateFriendFrame(inviter);

  private void OnFriendChallengeChanged(
    FriendChallengeEvent challengeEvent,
    BnetPlayer player,
    FriendlyChallengeData challengeData,
    object userData)
  {
    if (player == BnetPresenceMgr.Get().GetMyPlayer())
      this.UpdateFriendItems();
    else
      this.UpdateFriendFrame(player);
  }

  private bool HandleKeyboardInput()
  {
    FatalErrorMgr.Get().HasError();
    return false;
  }

  private void OnAddFriendButtonReleased(UIEvent e)
  {
    this.CloseFlyoutMenu();
    if (!BnetFriendMgr.Get().IsFriendInviteFeatureEnabled)
    {
      PrivacyFeaturesPopup privacyPopup = AssetLoader.Get().InstantiatePrefab(AssetReference.op_Implicit("PrivacyPopups.prefab:99a8f571a8a35a54e90790c904bc94f8"), (AssetLoadingOptions) 0).GetComponent<PrivacyFeaturesPopup>();
      privacyPopup.Set(PrivacyFeatures.CHAT, BnetFriendMgr.Get().IsFriendInviteFeatureEnabled, (System.Action) (() => PrivacyGate.Get().SetFeature(PrivacyFeatures.CHAT, true)), (System.Action) (() =>
      {
        this.ClosePrivacyPopup(privacyPopup);
        this.OnAddFriendAllowed();
      }), (System.Action) (() => this.ClosePrivacyPopup(privacyPopup)));
      privacyPopup.Show();
    }
    else
      this.OnAddFriendAllowed();
  }

  private void ClosePrivacyPopup(PrivacyFeaturesPopup privacyPopup)
  {
    privacyPopup.Hide();
    UnityEngine.Object.Destroy((UnityEngine.Object) privacyPopup.gameObject, 1f);
  }

  private void OnAddFriendAllowed()
  {
    if ((UnityEngine.Object) this.m_addFriendFrame != (UnityEngine.Object) null)
    {
      this.CloseAddFriendFrame();
    }
    else
    {
      if (this.AddFriendFrameOpened != null)
        this.AddFriendFrameOpened();
      this.ShowAddFriendFrame(FriendMgr.Get().GetSelectedFriend());
    }
  }

  private void OnEditFriendsButtonReleased(UIEvent e) => this.ToggleRemoveFriendsMode();

  private void OnRAFButtonReleased(UIEvent e)
  {
    if (!this.m_isRAFButtonEnabled)
      return;
    RAFManager.Get().ShowRAFFrame();
    TelemetryManager.Client().SendClickRecruitAFriend();
  }

  private void OnRAFButtonOver(UIEvent e)
  {
    TooltipZone component = this.rafButton.GetComponent<TooltipZone>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    string bodytext = GameUtils.GetNextTutorial() != 0 ? GameStrings.Get("GLUE_RAF_TOOLTIP_LOCKED_DESC") : GameStrings.Get("GLUE_RAF_TOOLTIP_DESC");
    component.ShowSocialTooltip((Component) this.rafButton, GameStrings.Get("GLUE_RAF_TOOLTIP_HEADLINE"), bodytext, 75f, GameLayer.BattleNetDialog);
  }

  private void OnRAFButtonOut(UIEvent e)
  {
    TooltipZone component = this.rafButton.GetComponent<TooltipZone>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.HideTooltip();
  }

  private void OnFSGButtonReleased(UIEvent e)
  {
    Options.Get().SetBool(Option.HAS_CLICKED_FIRESIDE_GATHERINGS_BUTTON, true);
    FiresideGatheringManager.Get().ShowFindFSGDialog();
    ChatMgr.Get().CloseFriendsList();
  }

  private bool OnRemoveFriendDialogShown(DialogBase dialog, object userData)
  {
    BnetPlayer player = (BnetPlayer) userData;
    if (!BnetFriendMgr.Get().IsFriend(player))
      return false;
    this.m_removeFriendPopup = (AlertPopup) dialog;
    return true;
  }

  private void OnRemoveFriendPopupResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CONFIRM && this.m_friendToRemove != null)
      BnetFriendMgr.Get().RemoveFriend(this.m_friendToRemove);
    this.m_friendToRemove = (BnetPlayer) null;
    this.m_removeFriendPopup = (AlertPopup) null;
    if (this.RemoveFriendPopupClosed == null)
      return;
    this.RemoveFriendPopupClosed();
  }

  private void OnFlyoutButtonReleased(UIEvent e)
  {
    if (this.IsInEditMode)
      this.ExitRemoveFriendsMode();
    else if (this.m_flyoutOpen)
      this.CloseFlyoutMenu();
    else
      this.OpenFlyoutMenu();
    if (!ChatMgr.Get().IsChatLogUIShowing())
      return;
    ChatMgr.Get().CloseChatUI(false);
  }

  private void UpdateSelectedItem()
  {
    FriendListFriendFrame renderedBaseFriendFrame = this.FindRenderedBaseFriendFrame(FriendMgr.Get().GetSelectedFriend());
    if ((UnityEngine.Object) renderedBaseFriendFrame == (UnityEngine.Object) null)
    {
      if (this.items.SelectedIndex == -1)
        return;
      this.items.SelectedIndex = -1;
      if (!((UnityEngine.Object) this.m_removeFriendPopup != (UnityEngine.Object) null))
        return;
      this.m_removeFriendPopup.Hide();
      this.m_removeFriendPopup = (AlertPopup) null;
      if (this.RemoveFriendPopupClosed == null)
        return;
      this.RemoveFriendPopupClosed();
    }
    else
      this.items.SelectedIndex = this.items.IndexOf((ITouchListItem) renderedBaseFriendFrame.GetComponent<MobileFriendListItem>());
  }

  private void UpdateRAFState()
  {
    if (!SetRotationManager.Get().ShouldShowSetRotationIntro() && SceneMgr.Get().GetMode() != SceneMgr.Mode.LOGIN && !((UnityEngine.Object) WelcomeQuests.Get() != (UnityEngine.Object) null) && !TemporaryAccountManager.IsTemporaryAccount() && GameUtils.GetNextTutorial() == 0)
      return;
    this.SetRAFButtonEnabled(false);
  }

  private void UpdateFSGState() => this.SetFSGButtonEnabled();

  private void InitButtons()
  {
    this.addFriendButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnAddFriendButtonReleased));
    this.removeFriendButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnEditFriendsButtonReleased));
    this.rafButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRAFButtonReleased));
    this.rafButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRAFButtonOver));
    this.rafButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRAFButtonOut));
    this.fsgButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnFSGButtonReleased));
    this.flyoutMenuButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnFlyoutButtonReleased));
  }

  private bool GetShowHeaderSection(Option setoption) => !(bool) Options.Get().GetOption(setoption, (object) false);

  private void SetShowHeaderSection(Option sectionoption, bool show)
  {
    if (this.GetShowHeaderSection(sectionoption) == show)
      return;
    Options.Get().SetOption(sectionoption, (object) !show);
  }

  private Transform GetBottomRightBone() => !((UnityEngine.Object) this.scrollbar != (UnityEngine.Object) null) || !this.scrollbar.gameObject.activeSelf ? this.listInfo.bottomRight : this.listInfo.bottomRightWithScrollbar;

  private void OnTemporaryAccountSignUpButtonPressed(UIEvent e)
  {
    ChatMgr.Get().CloseFriendsList();
    TemporaryAccountManager.Get().ShowHealUpPage(TemporaryAccountManager.HealUpReason.FRIENDS_LIST);
  }

  private void OnMySelectableMedalWidgetReady(Widget widget)
  {
    this.m_mySelectableMedal = widget.GetComponentInChildren<SelectableMedal>();
    this.UpdateMySelectableMedalWidget();
  }

  private void UpdatePortrait(bool medalIsDisplayable)
  {
    if (medalIsDisplayable)
    {
      this.myPortrait.gameObject.SetActive(false);
      if (!((UnityEngine.Object) this.portraitBackground != (UnityEngine.Object) null))
        return;
      this.portraitBackground.GetComponent<Renderer>().SetMaterial(this.rankedBackground);
    }
    else
    {
      this.myPortrait.SetProgramId(BnetProgramId.HEARTHSTONE);
      this.myPortrait.gameObject.SetActive(true);
      if (!((UnityEngine.Object) this.portraitBackground != (UnityEngine.Object) null))
        return;
      this.portraitBackground.GetComponent<Renderer>().SetMaterial(this.unrankedBackground);
    }
  }

  private void UpdateMySelectableMedalWidget() => this.m_mySelectableMedal?.UpdateWidget(BnetPresenceMgr.Get().GetMyPlayer(), (System.Action) (() => this.UpdatePortrait(true)), (System.Action) (() => this.UpdatePortrait(true)), (System.Action) (() => this.UpdatePortrait(false)));

  public enum FriendListEditMode
  {
    NONE,
    REMOVE_FRIENDS,
  }

  private class PlayerUpdate
  {
    public FriendListFrame.PlayerUpdate.ChangeType Change;
    public BnetPlayer Player;

    public PlayerUpdate(FriendListFrame.PlayerUpdate.ChangeType c, BnetPlayer p)
    {
      this.Change = c;
      this.Player = p;
    }

    public override bool Equals(object obj) => obj is FriendListFrame.PlayerUpdate playerUpdate && this.Change == playerUpdate.Change && (BnetEntityId) this.Player.GetAccountId() == (BnetEntityId) playerUpdate.Player.GetAccountId();

    public override int GetHashCode() => this.Player.GetHashCode();

    public enum ChangeType
    {
      Added,
      Removed,
    }
  }

  [Serializable]
  public class Me
  {
    public UberText nameText;
    public AsyncReference m_rankedMedalWidgetReference;
  }

  [Serializable]
  public class Prefabs
  {
    public FriendListItemHeader headerItem;
    public FriendListItemFooter footerItem;
    public FriendListNearbyPlayersHeader nearbyPlayersHeaderItem;
    public FriendListRequestFrame requestItem;
    public FriendListFriendFrame friendItem;
    public FriendListFSGFrame fsgItem;
    public AddFriendFrame addFriendFrame;
  }

  [Serializable]
  public class ListInfo
  {
    public Transform topLeft;
    public Transform bottomRight;
    public Transform bottomRightWithScrollbar;
    public FriendListFrame.HeaderBackgroundInfo requestBackgroundInfo;
    public FriendListFrame.HeaderBackgroundInfo currentGameBackgroundInfo;
  }

  [Serializable]
  public class HeaderBackgroundInfo
  {
    public Mesh mesh;
    public Material material;
  }

  public struct FriendListItem
  {
    private object m_item;

    public MobileFriendListItem.TypeFlags ItemFlags { get; private set; }

    public bool IsHeader => MobileFriendListItem.ItemIsHeader(this.ItemFlags);

    public BnetPlayer GetFriend() => (this.ItemFlags & MobileFriendListItem.TypeFlags.Friend) == (MobileFriendListItem.TypeFlags) 0 ? (BnetPlayer) null : (BnetPlayer) this.m_item;

    public BnetPlayer GetRecentPlayer() => (this.ItemFlags & MobileFriendListItem.TypeFlags.RecentPlayer) == (MobileFriendListItem.TypeFlags) 0 ? (BnetPlayer) null : (BnetPlayer) this.m_item;

    public BnetPlayer GetNearbyPlayer() => (this.ItemFlags & MobileFriendListItem.TypeFlags.NearbyPlayer) == (MobileFriendListItem.TypeFlags) 0 ? (BnetPlayer) null : (BnetPlayer) this.m_item;

    public BnetInvitation GetInvite() => (this.ItemFlags & MobileFriendListItem.TypeFlags.Request) == (MobileFriendListItem.TypeFlags) 0 ? (BnetInvitation) null : (BnetInvitation) this.m_item;

    public FSGConfig GetFSGConfig() => (this.ItemFlags & MobileFriendListItem.TypeFlags.FoundFiresideGathering) == (MobileFriendListItem.TypeFlags) 0 && (this.ItemFlags & MobileFriendListItem.TypeFlags.CurrentFiresideGathering) == (MobileFriendListItem.TypeFlags) 0 ? (FSGConfig) null : (FSGConfig) this.m_item;

    public BnetPlayer GetFiresideGatheringPlayer() => (this.ItemFlags & MobileFriendListItem.TypeFlags.FiresideGatheringPlayer) == (MobileFriendListItem.TypeFlags) 0 ? (BnetPlayer) null : (BnetPlayer) this.m_item;

    public string GetText() => (string) this.m_item;

    public MobileFriendListItem.TypeFlags ItemMainType => this.IsHeader ? MobileFriendListItem.TypeFlags.Header : this.SubType;

    public MobileFriendListItem.TypeFlags SubType => this.ItemFlags & ~MobileFriendListItem.TypeFlags.Header;

    public override string ToString() => this.IsHeader ? string.Format("[{0}]Header", (object) this.SubType) : string.Format("[{0}]{1}", (object) this.ItemMainType, this.m_item);

    public System.Type GetFrameType()
    {
      switch (this.ItemMainType)
      {
        case MobileFriendListItem.TypeFlags.Header:
          return typeof (FriendListItemHeader);
        case MobileFriendListItem.TypeFlags.Friend:
        case MobileFriendListItem.TypeFlags.CurrentGame:
        case MobileFriendListItem.TypeFlags.NearbyPlayer:
        case MobileFriendListItem.TypeFlags.RecentPlayer:
        case MobileFriendListItem.TypeFlags.FiresideGatheringPlayer:
          return typeof (FriendListFriendFrame);
        case MobileFriendListItem.TypeFlags.FiresideGatheringFooter:
          return typeof (FriendListItemFooter);
        case MobileFriendListItem.TypeFlags.CurrentFiresideGathering:
        case MobileFriendListItem.TypeFlags.FoundFiresideGathering:
          return typeof (FriendListFSGFrame);
        case MobileFriendListItem.TypeFlags.Request:
          return typeof (FriendListRequestFrame);
        default:
          throw new Exception("Unknown ItemType: " + (object) this.ItemFlags + " (" + (object) (int) this.ItemFlags + ")");
      }
    }

    public FriendListItem(bool isHeader, MobileFriendListItem.TypeFlags itemType, object itemData)
      : this()
    {
      if (!isHeader && itemData == null)
        Log.All.Print("FriendListItem: itemData is null! itemType=" + (object) itemType);
      this.m_item = itemData;
      this.ItemFlags = itemType;
      if (isHeader)
        this.ItemFlags |= MobileFriendListItem.TypeFlags.Header;
      else
        this.ItemFlags &= ~MobileFriendListItem.TypeFlags.Header;
    }
  }

  private class VirtualizedFriendsListBehavior : TouchList.ILongListBehavior
  {
    private FriendListFrame m_friendList;
    private int m_cachedMaxVisibleItems = -1;
    private const int MAX_FREELIST_ITEMS = 20;
    private List<MobileFriendListItem> m_freelist;
    private HashSet<MobileFriendListItem> m_acquiredItems = new HashSet<MobileFriendListItem>();
    private Bounds[] m_boundsByType;

    public VirtualizedFriendsListBehavior(FriendListFrame friendList) => this.m_friendList = friendList;

    public List<MobileFriendListItem> FreeList => this.m_freelist;

    public int AllItemsCount => this.m_friendList.m_allItems.Count;

    public int MaxVisibleItems
    {
      get
      {
        if (this.m_cachedMaxVisibleItems >= 0)
          return this.m_cachedMaxVisibleItems;
        this.m_cachedMaxVisibleItems = 0;
        UnityEngine.Vector2 clipSize = this.m_friendList.items.ClipSize;
        Bounds prefabBounds1 = FriendListFrame.VirtualizedFriendsListBehavior.GetPrefabBounds(this.m_friendList.prefabs.requestItem.gameObject);
        Bounds prefabBounds2 = FriendListFrame.VirtualizedFriendsListBehavior.GetPrefabBounds(this.m_friendList.prefabs.friendItem.gameObject);
        float num = Mathf.Min(prefabBounds1.max.z - prefabBounds1.min.z, prefabBounds2.max.z - prefabBounds2.min.z);
        if ((double) num > 0.0)
          this.m_cachedMaxVisibleItems = Mathf.CeilToInt(clipSize.y / num) + 3;
        return this.m_cachedMaxVisibleItems;
      }
    }

    private static Bounds GetPrefabBounds(GameObject prefabGameObject)
    {
      GameObject go = UnityEngine.Object.Instantiate<GameObject>(prefabGameObject);
      go.SetActive(true);
      Bounds setPointBounds = TransformUtil.ComputeSetPointBounds(go);
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) go);
      return setPointBounds;
    }

    public int MinBuffer => 2;

    public int MaxAcquiredItems => this.MaxVisibleItems + 2 * this.MinBuffer;

    public bool IsItemShowable(int allItemsIndex)
    {
      if (allItemsIndex < 0 || allItemsIndex >= this.AllItemsCount)
        return false;
      FriendListFrame.FriendListItem allItem = this.m_friendList.m_allItems[allItemsIndex];
      if (allItem.IsHeader && !this.m_friendList.IsInEditMode || allItem.ItemMainType == MobileFriendListItem.TypeFlags.FiresideGatheringFooter && !this.m_friendList.IsInEditMode)
        return true;
      if (allItem.ItemMainType != MobileFriendListItem.TypeFlags.FoundFiresideGathering && allItem.ItemMainType != MobileFriendListItem.TypeFlags.CurrentFiresideGathering && allItem.ItemMainType != MobileFriendListItem.TypeFlags.FiresideGatheringPlayer)
      {
        FriendListItemHeader header = this.m_friendList.FindHeader(allItem.SubType);
        if ((UnityEngine.Object) header == (UnityEngine.Object) null || !header.IsShowingContents)
          return false;
      }
      if (allItem.ItemMainType == MobileFriendListItem.TypeFlags.FoundFiresideGathering && (FiresideGatheringManager.Get().IsCheckedIn || !SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB) && allItem.GetFSGConfig().IsInnkeeper && !allItem.GetFSGConfig().IsSetupComplete) || allItem.ItemMainType == MobileFriendListItem.TypeFlags.CurrentFiresideGathering && (!FiresideGatheringManager.Get().IsCheckedIn || !FiresideGatheringManager.Get().IsCheckedInToFSG(allItem.GetFSGConfig().FsgId)) || allItem.ItemFlags == MobileFriendListItem.TypeFlags.NearbyPlayer && FiresideGatheringManager.Get().IsPlayerInMyFSGAndDisplayable(allItem.GetNearbyPlayer()) || allItem.ItemFlags == MobileFriendListItem.TypeFlags.FiresideGatheringPlayer && (!allItem.GetFiresideGatheringPlayer().IsOnline() || (Blizzard.GameService.SDK.Client.Integration.FourCC) allItem.GetFiresideGatheringPlayer().GetBestProgramId() != (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE))
        return false;
      if (this.m_friendList.EditMode == FriendListFrame.FriendListEditMode.REMOVE_FRIENDS)
      {
        if (allItem.ItemMainType != MobileFriendListItem.TypeFlags.Header)
        {
          if (allItem.ItemFlags == MobileFriendListItem.TypeFlags.FiresideGatheringPlayer && !BnetFriendMgr.Get().IsFriend(allItem.GetFiresideGatheringPlayer()) || allItem.ItemFlags == MobileFriendListItem.TypeFlags.RecentPlayer || allItem.ItemFlags == MobileFriendListItem.TypeFlags.NearbyPlayer || allItem.ItemFlags == MobileFriendListItem.TypeFlags.FiresideGatheringFooter || allItem.ItemMainType == MobileFriendListItem.TypeFlags.FoundFiresideGathering || allItem.ItemMainType == MobileFriendListItem.TypeFlags.CurrentFiresideGathering)
            return false;
        }
        else if (allItem.ItemFlags == (MobileFriendListItem.TypeFlags.RecentPlayer | MobileFriendListItem.TypeFlags.Header) || allItem.ItemFlags == (MobileFriendListItem.TypeFlags.NearbyPlayer | MobileFriendListItem.TypeFlags.Header))
          return false;
      }
      return true;
    }

    public Vector3 GetItemSize(int allItemsIndex)
    {
      if (allItemsIndex < 0 || allItemsIndex >= this.AllItemsCount)
        return Vector3.zero;
      FriendListFrame.FriendListItem allItem = this.m_friendList.m_allItems[allItemsIndex];
      if (this.m_boundsByType == null)
        this.InitializeBoundsByTypeArray();
      return this.m_boundsByType[this.GetBoundsByTypeIndex(allItem.ItemMainType)].size;
    }

    private bool HasCollapsedHeaders
    {
      get
      {
        foreach (KeyValuePair<MobileFriendListItem.TypeFlags, FriendListItemHeader> header in this.m_friendList.m_headers)
        {
          if (!header.Value.IsShowingContents)
            return true;
        }
        return false;
      }
    }

    public void ReleaseAllItems()
    {
      if (this.m_acquiredItems.Count == 0)
        return;
      if (this.m_freelist == null)
        this.m_freelist = new List<MobileFriendListItem>();
      this.m_freelist.Clear();
      foreach (MobileFriendListItem acquiredItem in this.m_acquiredItems)
      {
        if (acquiredItem.IsHeader)
          acquiredItem.gameObject.SetActive(false);
        else if (this.m_freelist.Count >= 20)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) acquiredItem.gameObject);
        }
        else
        {
          this.m_freelist.Add(acquiredItem);
          acquiredItem.gameObject.SetActive(false);
        }
        acquiredItem.Unselected();
      }
      this.m_acquiredItems.Clear();
    }

    public void ReleaseItem(ITouchListItem item)
    {
      MobileFriendListItem mobileFriendListItem = item as MobileFriendListItem;
      if ((UnityEngine.Object) mobileFriendListItem == (UnityEngine.Object) null)
        throw new ArgumentException("given item is not MobileFriendListItem: " + (object) item);
      if (this.m_freelist == null)
        this.m_freelist = new List<MobileFriendListItem>();
      if (mobileFriendListItem.IsHeader)
        mobileFriendListItem.gameObject.SetActive(false);
      else if (this.m_freelist.Count >= 20)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) item.gameObject);
      }
      else
      {
        this.m_freelist.Add(mobileFriendListItem);
        mobileFriendListItem.gameObject.SetActive(false);
      }
      if (!this.m_acquiredItems.Remove(mobileFriendListItem))
        Log.All.Print("VirtualizedFriendsListBehavior.ReleaseItem item not found in m_acquiredItems: {0}", (object) mobileFriendListItem);
      mobileFriendListItem.Unselected();
    }

    public ITouchListItem AcquireItem(int index)
    {
      if (this.m_acquiredItems.Count >= this.MaxAcquiredItems)
        throw new Exception("Bug in ILongListBehavior? there are too many acquired items! index=" + (object) index + " max=" + (object) this.MaxAcquiredItems + " maxVisible=" + (object) this.MaxVisibleItems + " minBuffer=" + (object) this.MinBuffer + " acquiredItems.Count=" + (object) this.m_acquiredItems.Count + " hasCollapsedHeaders=" + this.HasCollapsedHeaders.ToString());
      if (index < 0 || index >= this.m_friendList.m_allItems.Count)
        throw new IndexOutOfRangeException(string.Format("Invalid index, {0} has {1} elements.", (object) DebugUtils.GetHierarchyPathAndType((UnityEngine.Object) this.m_friendList), (object) this.m_friendList.m_allItems.Count));
      FriendListFrame.FriendListItem item = this.m_friendList.m_allItems[index];
      MobileFriendListItem.TypeFlags itemMainType = item.ItemMainType;
      System.Type frameType = item.GetFrameType();
      if (this.m_freelist != null && !item.IsHeader)
      {
        int lastIndex = this.m_freelist.FindLastIndex((Predicate<MobileFriendListItem>) (m => !item.IsHeader ? (UnityEngine.Object) m.GetComponent(frameType) != (UnityEngine.Object) null : m.IsHeader));
        if (lastIndex >= 0 && (UnityEngine.Object) this.m_freelist[lastIndex] == (UnityEngine.Object) null)
        {
          for (int index1 = 0; index1 < this.m_freelist.Count; ++index1)
          {
            if ((UnityEngine.Object) this.m_freelist[index1] == (UnityEngine.Object) null)
            {
              this.m_freelist.RemoveAt(index1);
              --index1;
            }
          }
          lastIndex = this.m_freelist.FindLastIndex((Predicate<MobileFriendListItem>) (m => !item.IsHeader ? (UnityEngine.Object) m.GetComponent(frameType) != (UnityEngine.Object) null : m.IsHeader));
        }
        if (lastIndex >= 0)
        {
          MobileFriendListItem mobileFriendListItem = this.m_freelist[lastIndex];
          this.m_freelist.RemoveAt(lastIndex);
          mobileFriendListItem.gameObject.SetActive(true);
          switch (itemMainType)
          {
            case MobileFriendListItem.TypeFlags.Friend:
            case MobileFriendListItem.TypeFlags.NearbyPlayer:
            case MobileFriendListItem.TypeFlags.RecentPlayer:
            case MobileFriendListItem.TypeFlags.FiresideGatheringPlayer:
              FriendListFriendFrame component1 = mobileFriendListItem.GetComponent<FriendListFriendFrame>();
              component1.gameObject.SetActive(true);
              BnetPlayer player = (BnetPlayer) null;
              bool isFSGPatron = false;
              bool isRecentPlayerFrame = false;
              if (itemMainType <= MobileFriendListItem.TypeFlags.NearbyPlayer)
              {
                if (itemMainType != MobileFriendListItem.TypeFlags.Friend)
                {
                  if (itemMainType == MobileFriendListItem.TypeFlags.NearbyPlayer)
                    player = item.GetNearbyPlayer();
                }
                else
                  player = item.GetFriend();
              }
              else if (itemMainType != MobileFriendListItem.TypeFlags.RecentPlayer)
              {
                if (itemMainType == MobileFriendListItem.TypeFlags.FiresideGatheringPlayer)
                {
                  player = item.GetFiresideGatheringPlayer();
                  isFSGPatron = true;
                }
              }
              else
              {
                player = item.GetRecentPlayer();
                isRecentPlayerFrame = true;
              }
              component1.Initialize(player, isFSGPatron, isRecentPlayerFrame: isRecentPlayerFrame);
              FriendListItemHeader parent = isFSGPatron ? (FriendListItemHeader) null : this.m_friendList.FindHeader(itemMainType);
              this.m_friendList.FinishCreateVisualItem<FriendListFriendFrame>(component1, itemMainType, (ITouchListItem) parent, component1.gameObject);
              break;
            case MobileFriendListItem.TypeFlags.FiresideGatheringFooter:
              FriendListItemFooter component2 = mobileFriendListItem.GetComponent<FriendListItemFooter>();
              component2.Text = item.GetText();
              this.m_friendList.FinishCreateVisualItem<FriendListItemFooter>(component2, itemMainType, (ITouchListItem) null, component2.gameObject);
              break;
            case MobileFriendListItem.TypeFlags.CurrentFiresideGathering:
            case MobileFriendListItem.TypeFlags.FoundFiresideGathering:
              FriendListFSGFrame component3 = mobileFriendListItem.GetComponent<FriendListFSGFrame>();
              component3.InitFrame(item.GetFSGConfig());
              this.m_friendList.FinishCreateVisualItem<FriendListFSGFrame>(component3, itemMainType, (ITouchListItem) null, component3.gameObject);
              break;
            case MobileFriendListItem.TypeFlags.Request:
              FriendListRequestFrame component4 = mobileFriendListItem.GetComponent<FriendListRequestFrame>();
              component4.gameObject.SetActive(true);
              component4.SetInvite(item.GetInvite());
              this.m_friendList.FinishCreateVisualItem<FriendListRequestFrame>(component4, itemMainType, (ITouchListItem) this.m_friendList.FindHeader(itemMainType), component4.gameObject);
              component4.UpdateInvite();
              break;
            default:
              throw new NotImplementedException("VirtualizedFriendsListBehavior.AcquireItem[reuse] frameType=" + frameType.FullName + " itemType=" + (object) itemMainType);
          }
          this.m_acquiredItems.Add(mobileFriendListItem);
          return (ITouchListItem) mobileFriendListItem;
        }
      }
      MobileFriendListItem mobileFriendListItem1;
      switch (itemMainType)
      {
        case MobileFriendListItem.TypeFlags.Header:
          mobileFriendListItem1 = this.m_friendList.FindHeader(item.SubType).GetComponent<MobileFriendListItem>();
          break;
        case MobileFriendListItem.TypeFlags.Friend:
          mobileFriendListItem1 = this.m_friendList.CreatePlayerFrame(item.GetFriend(), MobileFriendListItem.TypeFlags.Friend);
          break;
        case MobileFriendListItem.TypeFlags.NearbyPlayer:
          mobileFriendListItem1 = this.m_friendList.CreatePlayerFrame(item.GetNearbyPlayer(), MobileFriendListItem.TypeFlags.NearbyPlayer);
          break;
        case MobileFriendListItem.TypeFlags.RecentPlayer:
          mobileFriendListItem1 = this.m_friendList.CreatePlayerFrame(item.GetRecentPlayer(), MobileFriendListItem.TypeFlags.RecentPlayer);
          break;
        case MobileFriendListItem.TypeFlags.FiresideGatheringFooter:
          mobileFriendListItem1 = this.m_friendList.CreateFSGFooter(item.GetText());
          break;
        case MobileFriendListItem.TypeFlags.FiresideGatheringPlayer:
          mobileFriendListItem1 = this.m_friendList.CreateFSGPlayerFrame(item.GetFiresideGatheringPlayer());
          break;
        case MobileFriendListItem.TypeFlags.CurrentFiresideGathering:
          mobileFriendListItem1 = this.m_friendList.CreateFSGFrame(item.GetFSGConfig(), MobileFriendListItem.TypeFlags.CurrentFiresideGathering);
          break;
        case MobileFriendListItem.TypeFlags.Request:
          mobileFriendListItem1 = this.m_friendList.CreateRequestFrame(item.GetInvite());
          break;
        case MobileFriendListItem.TypeFlags.FoundFiresideGathering:
          mobileFriendListItem1 = this.m_friendList.CreateFSGFrame(item.GetFSGConfig(), MobileFriendListItem.TypeFlags.FoundFiresideGathering);
          break;
        default:
          throw new NotImplementedException("VirtualizedFriendsListBehavior.AcquireItem[new] type=" + frameType.FullName);
      }
      this.m_acquiredItems.Add(mobileFriendListItem1);
      return (ITouchListItem) mobileFriendListItem1;
    }

    private void InitializeBoundsByTypeArray()
    {
      Array values = Enum.GetValues(typeof (MobileFriendListItem.TypeFlags));
      this.m_boundsByType = new Bounds[values.Length];
      for (int index = 0; index < values.Length; ++index)
      {
        MobileFriendListItem.TypeFlags itemType = (MobileFriendListItem.TypeFlags) values.GetValue(index);
        Component prefab = this.GetPrefab(itemType);
        this.m_boundsByType[this.GetBoundsByTypeIndex(itemType)] = (UnityEngine.Object) prefab == (UnityEngine.Object) null ? new Bounds() : FriendListFrame.VirtualizedFriendsListBehavior.GetPrefabBounds(prefab.gameObject);
      }
    }

    private int GetBoundsByTypeIndex(MobileFriendListItem.TypeFlags itemType)
    {
      switch (itemType)
      {
        case MobileFriendListItem.TypeFlags.Header:
          return 0;
        case MobileFriendListItem.TypeFlags.Friend:
          return 5;
        case MobileFriendListItem.TypeFlags.CurrentGame:
          return 4;
        case MobileFriendListItem.TypeFlags.NearbyPlayer:
          return 3;
        case MobileFriendListItem.TypeFlags.RecentPlayer:
          return 2;
        case MobileFriendListItem.TypeFlags.FiresideGatheringFooter:
          return 9;
        case MobileFriendListItem.TypeFlags.FiresideGatheringPlayer:
          return 7;
        case MobileFriendListItem.TypeFlags.CurrentFiresideGathering:
          return 8;
        case MobileFriendListItem.TypeFlags.Request:
          return 1;
        case MobileFriendListItem.TypeFlags.FoundFiresideGathering:
          return 6;
        default:
          throw new Exception("Unknown ItemType: " + (object) itemType + " (" + (object) (int) itemType + ")");
      }
    }

    private Component GetPrefab(MobileFriendListItem.TypeFlags itemType)
    {
      switch (itemType)
      {
        case MobileFriendListItem.TypeFlags.Header:
          return (Component) this.m_friendList.prefabs.headerItem;
        case MobileFriendListItem.TypeFlags.Friend:
        case MobileFriendListItem.TypeFlags.CurrentGame:
        case MobileFriendListItem.TypeFlags.NearbyPlayer:
        case MobileFriendListItem.TypeFlags.RecentPlayer:
        case MobileFriendListItem.TypeFlags.FiresideGatheringPlayer:
          return (Component) this.m_friendList.prefabs.friendItem;
        case MobileFriendListItem.TypeFlags.FiresideGatheringFooter:
          return (Component) this.m_friendList.prefabs.footerItem;
        case MobileFriendListItem.TypeFlags.CurrentFiresideGathering:
        case MobileFriendListItem.TypeFlags.FoundFiresideGathering:
          return (Component) this.m_friendList.prefabs.fsgItem;
        case MobileFriendListItem.TypeFlags.Request:
          return (Component) this.m_friendList.prefabs.requestItem;
        default:
          throw new Exception("Unknown ItemType: " + (object) itemType + " (" + (object) (int) itemType + ")");
      }
    }
  }
}
