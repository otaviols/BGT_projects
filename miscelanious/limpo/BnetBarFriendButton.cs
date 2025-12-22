using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using System.Collections.Generic;
using UnityEngine;

public class BnetBarFriendButton : FriendListUIElement
{
  public UberText m_OnlineCountText;
  public Color m_AnyOnlineColor;
  public Color m_AllOfflineColor;
  public Color m_FSGColor;
  public GameObject m_PendingInvitesIcon;
  public GameObject m_FSGSocialBar;
  public GameObject m_FSGGlow;
  public GameObject m_Background;
  private static BnetBarFriendButton s_instance;
  private static bool m_hasClickedWhileFSGGlowing;
  private Material m_backgroundMaterial;
  private float m_originalLightingBlend;

  protected override void Awake()
  {
    BnetBarFriendButton.s_instance = this;
    base.Awake();
    if ((Object) this.m_Background != (Object) null)
    {
      MeshRenderer component = this.m_Background.GetComponent<MeshRenderer>();
      if ((Object) component != (Object) null)
      {
        this.m_backgroundMaterial = component.GetMaterial();
        this.m_originalLightingBlend = this.m_backgroundMaterial.GetFloat("_LightingBlend");
      }
    }
    this.UpdateOnlineCount();
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    FiresideGatheringManager.OnPatronListUpdated += new FiresideGatheringManager.OnPatronListUpdatedCallback(this.OnFSGPatronsUpdated);
    FiresideGatheringManager.Get().OnJoinFSG += new FiresideGatheringManager.CheckedInToFSGCallback(this.OnJoinFSG);
    FiresideGatheringManager.Get().OnLeaveFSG += new FiresideGatheringManager.CheckedOutOfFSGCallback(this.OnLeaveFSG);
    FiresideGatheringManager.Get().OnNearbyFSGs += new FiresideGatheringManager.RequestNearbyFSGsCallback(this.OnNearbyFSGs);
    this.ShowPendingInvitesIcon(false);
  }

  protected override void OnDestroy()
  {
    if (BnetFriendMgr.Get() != null)
      BnetFriendMgr.Get().RemoveChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    if (BnetPresenceMgr.Get() != null)
      BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    FiresideGatheringManager.OnPatronListUpdated -= new FiresideGatheringManager.OnPatronListUpdatedCallback(this.OnFSGPatronsUpdated);
    if (FatalErrorMgr.Get() != null)
      FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    if (FiresideGatheringManager.Get() != null)
    {
      FiresideGatheringManager.Get().OnJoinFSG -= new FiresideGatheringManager.CheckedInToFSGCallback(this.OnJoinFSG);
      FiresideGatheringManager.Get().OnLeaveFSG -= new FiresideGatheringManager.CheckedOutOfFSGCallback(this.OnLeaveFSG);
      FiresideGatheringManager.Get().OnNearbyFSGs -= new FiresideGatheringManager.RequestNearbyFSGsCallback(this.OnNearbyFSGs);
    }
    BnetBarFriendButton.s_instance = (BnetBarFriendButton) null;
    base.OnDestroy();
  }

  public static BnetBarFriendButton Get() => BnetBarFriendButton.s_instance;

  public void HideTooltip()
  {
    TooltipZone component = this.GetComponent<TooltipZone>();
    if (!((Object) component != (Object) null))
      return;
    component.HideTooltip();
  }

  private void OnFriendsChanged(BnetFriendChangelist changelist, object userData) => this.UpdateOnlineCount();

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData) => this.UpdateOnlineCount();

  private void OnJoinFSG(FSGConfig gathering)
  {
    this.m_Background.SetActive(false);
    this.m_FSGSocialBar.SetActive(true);
    this.UpdateOnlineCount();
  }

  private void OnLeaveFSG(FSGConfig gathering)
  {
    this.m_Background.SetActive(true);
    this.m_FSGSocialBar.SetActive(false);
    this.UpdateOnlineCount();
  }

  private void OnNearbyFSGs()
  {
    if (BnetBarFriendButton.m_hasClickedWhileFSGGlowing)
      return;
    this.m_FSGGlow.SetActive(true);
  }

  private void OnFSGPatronsUpdated(List<BnetPlayer> addedList, List<BnetPlayer> removedList) => this.UpdateOnlineCount();

  private void OnFatalError(FatalErrorMessage message, object userData) => this.UpdateOnlineCount();

  public void UpdateOnlineCount()
  {
    if (FiresideGatheringManager.Get().IsCheckedIn)
    {
      this.m_OnlineCountText.TextColor = this.m_FSGColor;
      if (FiresideGatheringManager.Get().CurrentFsgIsLargeScale)
        this.m_OnlineCountText.Text = GameStrings.Get("GLOBAL_FIRESIDE_GATHERING_SOCIAL_BUTTON_LARGE_SCALE_LABEL");
      else
        this.m_OnlineCountText.Text = FiresideGatheringManager.Get().DisplayablePatronCount.ToString();
    }
    else
    {
      int onlineFriendCount = BnetFriendMgr.Get().GetOnlineFriendCount();
      this.m_OnlineCountText.TextColor = onlineFriendCount != 0 ? this.m_AnyOnlineColor : this.m_AllOfflineColor;
      this.m_OnlineCountText.Text = onlineFriendCount.ToString();
    }
  }

  public void ShowPendingInvitesIcon(bool show)
  {
    if (!((Object) this.m_PendingInvitesIcon != (Object) null) || this.m_PendingInvitesIcon.activeInHierarchy == show)
      return;
    this.m_PendingInvitesIcon.SetActive(show);
    this.m_OnlineCountText.gameObject.SetActive(!show);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9"));
    this.UpdateHighlight();
  }

  protected override void OnOut(PegUIElement.InteractionState oldState) => base.OnOut(oldState);

  public override void SetEnabled(bool enabled, bool isInternal = false)
  {
    base.SetEnabled(enabled, isInternal);
    if (!enabled)
      this.UpdateHighlight();
    if (!((Object) this.m_backgroundMaterial != (Object) null))
      return;
    this.m_backgroundMaterial.SetFloat("_LightingBlend", enabled ? this.m_originalLightingBlend : 0.8f);
  }

  protected override void OnRelease()
  {
    base.OnRelease();
    if (BnetBarFriendButton.m_hasClickedWhileFSGGlowing || !this.m_FSGGlow.activeInHierarchy)
      return;
    this.m_FSGGlow.SetActive(false);
    BnetBarFriendButton.m_hasClickedWhileFSGGlowing = true;
  }
}
