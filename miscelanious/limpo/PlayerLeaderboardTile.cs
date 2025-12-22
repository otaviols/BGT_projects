using System.Collections.Generic;
using UnityEngine;

public class PlayerLeaderboardTile : MonoBehaviour
{
  public ProgressBar m_HealthBar;
  public GameObject m_IconSwords;
  public GameObject m_IconSkull;
  public GameObject m_IconFirst;
  public GameObject m_IconSecond;
  public GameObject m_IconThird;
  public GameObject m_IconFourth;
  public GameObject m_IconSplat;
  public PlayerLeaderboardIcon m_IconTechUp;
  public PlayerLeaderboardIcon m_IconHotStreak;
  public PlayerLeaderboardIcon m_IconTriple;
  public UberText m_splatText;
  private const string POP_OUT_PLAYMAKER_STATE = "PopOut";
  private const string POP_IN_PLAYMAKER_STATE = "PopIn";
  private const string REVEALED_PLAYMAKER_STATE = "Reveal";
  private const string UNREVEALED_PLAYMAKER_STATE = "Unrevealed";
  private const string TECH_LEVEL_PLAYMAKER_VARIABLE = "TechLevel";
  private Queue<PlayerLeaderboardManager.PlayerTileEvent> m_incomingNotifications;
  private bool m_notificationActive;
  private PlayerLeaderboardIcon m_currentAnimatingObject;
  private int m_ownerId = -1;

  public void Awake() => this.m_incomingNotifications = new Queue<PlayerLeaderboardManager.PlayerTileEvent>();

  public void Update()
  {
    if (!this.m_notificationActive && this.m_incomingNotifications != null && this.m_incomingNotifications.Count > 0)
    {
      this.ShowNotification(this.m_incomingNotifications.Dequeue());
    }
    else
    {
      if (this.m_IconTechUp.PlaymakerIsShowing() || this.m_IconTriple.PlaymakerIsShowing() || this.m_IconHotStreak.PlaymakerIsShowing())
        return;
      this.m_notificationActive = false;
    }
  }

  public void SetCurrentHealth(float healthPercent)
  {
    this.SetHealthBarActive((double) healthPercent > 0.0);
    this.SetSkullIconActive((double) healthPercent == 0.0);
    this.m_HealthBar.SetProgressBar(healthPercent);
  }

  public void SetSplatAmount(int value)
  {
    this.m_IconSplat.gameObject.SetActive(value != 0);
    this.m_splatText.gameObject.SetActive(value != 0);
    RenderUtils.EnableRenderers(this.m_IconSplat.GetComponent<DamageSplatSpell>().m_BloodSplat.gameObject, value != 0);
    this.m_splatText.Text = value.ToString();
  }

  public void SetHealthBarActive(bool active) => this.m_HealthBar.gameObject.SetActive(active);

  public void SetSwordsIconActive(bool active) => this.m_IconSwords.SetActive(active);

  public void SetSkullIconActive(bool active) => this.m_IconSkull.SetActive(active);

  public void SetPlaceIcon(int currentPlace)
  {
    this.m_IconFirst.SetActive(false);
    this.m_IconSecond.SetActive(false);
    this.m_IconThird.SetActive(false);
    this.m_IconFourth.SetActive(false);
    switch (currentPlace)
    {
      case 1:
        this.m_IconFirst.SetActive(true);
        break;
      case 2:
        this.m_IconSecond.SetActive(true);
        break;
      case 3:
        this.m_IconThird.SetActive(true);
        break;
      case 4:
        this.m_IconFourth.SetActive(true);
        break;
    }
  }

  public void SetTilePopOutActive(bool active)
  {
    PlayMakerFSM component = this.GetComponent<PlayMakerFSM>();
    if ((Object) component == (Object) null)
      return;
    component.SetState(active ? "PopOut" : "PopIn");
  }

  public void SetTileRevealed(bool revealed, bool isNextOpponent)
  {
    PlayMakerFSM component = this.GetComponent<PlayMakerFSM>();
    if ((Object) component == (Object) null)
      return;
    component.FsmVariables.GetFsmInt("IsNextOpponent").Value = isNextOpponent ? 1 : 0;
    component.SetState(revealed ? "Reveal" : "Unrevealed");
  }

  public void SetOwnerId(int playerId) => this.m_ownerId = playerId;

  public int GetOwnerId() => this.m_ownerId;

  public void NotifyEvent(
    PlayerLeaderboardManager.PlayerTileEvent notificationType)
  {
    this.m_incomingNotifications.Enqueue(notificationType);
  }

  private void ShowNotification(
    PlayerLeaderboardManager.PlayerTileEvent notificationType)
  {
    switch (notificationType)
    {
      case PlayerLeaderboardManager.PlayerTileEvent.TRIPLE:
        this.m_currentAnimatingObject = this.m_IconTriple;
        break;
      case PlayerLeaderboardManager.PlayerTileEvent.WIN_STREAK:
        this.m_currentAnimatingObject = this.m_IconHotStreak;
        break;
      case PlayerLeaderboardManager.PlayerTileEvent.TECH_LEVEL:
        int num1 = 1;
        if (GameState.Get().GetPlayerInfoMap().ContainsKey(this.m_ownerId) && GameState.Get().GetPlayerInfoMap()[this.m_ownerId].GetPlayerHero() != null)
          num1 = GameState.Get().GetPlayerInfoMap()[this.m_ownerId].GetPlayerHero().GetRealTimePlayerTechLevel();
        int num2 = Mathf.Clamp(num1, 1, 6);
        if (num2 == 1)
          return;
        this.m_IconTechUp.SetPlaymakerValue("TechLevel", num2);
        this.m_currentAnimatingObject = this.m_IconTechUp;
        break;
      default:
        return;
    }
    this.m_notificationActive = true;
    this.m_currentAnimatingObject.gameObject.SetActive(true);
    this.m_currentAnimatingObject.ClearText();
    this.m_currentAnimatingObject.PlaymakerShow();
  }
}
