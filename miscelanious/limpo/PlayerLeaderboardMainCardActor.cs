using UnityEngine;

public class PlayerLeaderboardMainCardActor : Actor
{
  public UberText m_playerNameText;
  public UberText m_alternateNameText;
  public GameObject m_playerNameBackground;
  public GameObject m_fullSelectionHighlight;
  public GameObject m_lockIcon;
  public GameObject m_lockedHeroBackground;
  private const string BACON_ALTERNATE_NAME_STRING_ID = "GAMEPLAY_BACON_ALTERNATE_PLAYER_NAME";
  private UberText m_pausedHealthTextMesh;

  public void UpdatePlayerNameText(string text)
  {
    if (!((Object) this.m_playerNameText != (Object) null))
      return;
    this.m_playerNameText.Text = text;
  }

  public void UpdateAlternateNameText(string text)
  {
    if (!((Object) this.m_alternateNameText != (Object) null))
      return;
    this.m_alternateNameText.SetText(GameStrings.Get(GameStrings.Format("GAMEPLAY_BACON_ALTERNATE_PLAYER_NAME", (object) text)));
    this.m_alternateNameText.UpdateNow(true);
  }

  protected override void ShowImpl(bool ignoreSpells)
  {
    base.ShowImpl(ignoreSpells);
    if (!((Object) this.m_nameTextMesh != (Object) null))
      return;
    this.m_nameTextMesh.gameObject.SetActive(false);
    if (!(bool) (Object) this.m_nameTextMesh.RenderOnObject)
      return;
    this.m_nameTextMesh.RenderOnObject.GetComponent<Renderer>().enabled = false;
  }

  public void SetAlternateNameTextActive(bool active)
  {
    if (!((Object) this.m_alternateNameText != (Object) null))
      return;
    this.m_alternateNameText.gameObject.SetActive(active);
  }

  public void SetFullyHighlighted(bool highlighted) => this.m_fullSelectionHighlight.SetActive(highlighted);

  public void PauseHealthUpdates()
  {
    if ((Object) this.m_healthTextMesh == (Object) null)
      return;
    this.m_pausedHealthTextMesh = this.m_healthTextMesh;
    this.m_healthTextMesh = (UberText) null;
  }

  public void ResumeHealthUpdates()
  {
    if ((Object) this.m_pausedHealthTextMesh == (Object) null)
      return;
    this.m_healthTextMesh = this.m_pausedHealthTextMesh;
    this.m_pausedHealthTextMesh = (UberText) null;
    this.UpdateMinionStatsImmediately();
  }

  public void ToggleLockedHeroView(bool isOn)
  {
    this.m_lockedHeroBackground.SetActive(isOn);
    this.m_lockIcon.SetActive(isOn);
    if (isOn)
    {
      this.SetAlternateNameTextActive(false);
      this.m_playerNameBackground.SetActive(false);
      this.m_nameTextMesh.gameObject.SetActive(false);
      this.GetHealthObject().Hide();
      this.GetAttackObject().Hide();
    }
    this.SetFullyHighlighted(false);
  }

  public bool TryLegendarySlotIn() => !((Object) this.LegendaryHeroSkinConfig == (Object) null) && this.LegendaryHeroSkinConfig.TryActivateVFX_SocketIn();
}
