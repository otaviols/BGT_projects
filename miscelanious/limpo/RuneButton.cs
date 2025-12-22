using PegasusShared;
using UnityEngine;

public class RuneButton : UIBButton
{
  private static RuneType[] m_runeOrder = new RuneType[4]
  {
    RuneType.RT_NONE,
    RuneType.RT_BLOOD,
    RuneType.RT_FROST,
    RuneType.RT_UNHOLY
  };
  [CustomEditField(Sections = "Button Objects")]
  public GameObject m_runeBlood;
  [CustomEditField(Sections = "Button Objects")]
  public GameObject m_runeFrost;
  [CustomEditField(Sections = "Button Objects")]
  public GameObject m_runeUnholy;
  [CustomEditField(Sections = "Button Objects")]
  public GameObject m_runeEmpty;
  [CustomEditField(Sections = "Button Objects")]
  public PlayMakerFSM m_runeFSM;
  [CustomEditField(Sections = "Button Objects")]
  public PlayMakerFSM m_runeHighlightFSM;
  [CustomEditField(Sections = "Button Objects")]
  public GameObject m_emptyRune;
  [CustomEditField(Sections = "Button Objects")]
  public GameObject m_emptyRuneHighlight;
  private const string BloodSpawnIn = "Blood_SpawnIn";
  private const string BloodSpawnOut = "Blood_SpawnOut";
  private const string FrostSpawnIn = "Frost_SpawnIn";
  private const string FrostSpawnOut = "Frost_SpawnOut";
  private const string UnholySpawnIn = "Unholy_SpawnIn";
  private const string UnholySpawnOut = "Unholy_SpawnOut";
  private const string HoverOn = "Hover";
  private const string HoverOff = "Hover_Off";
  private int m_runeIndex;

  public RuneType RuneType => (RuneType) this.m_runeIndex;

  public int ButtonIndex { get; private set; }

  public void SetRune(RuneType runeType, bool animate)
  {
    this.ShowRune(runeType, animate);
    this.m_runeIndex = (int) runeType;
  }

  private void ShowRune(RuneType runeType, bool animate)
  {
    switch (runeType)
    {
      case RuneType.RT_NONE:
        this.HideCurrentRune(animate);
        break;
      case RuneType.RT_BLOOD:
        if (animate)
        {
          this.m_runeFSM.SendEvent("Blood_SpawnIn");
        }
        else
        {
          this.m_runeBlood.SetActive(true);
          this.m_runeFrost.SetActive(false);
          this.m_runeUnholy.SetActive(false);
        }
        this.SetEmptyRuneVisible(false);
        break;
      case RuneType.RT_FROST:
        if (animate)
        {
          this.m_runeFSM.SendEvent("Frost_SpawnIn");
        }
        else
        {
          this.m_runeBlood.SetActive(false);
          this.m_runeFrost.SetActive(true);
          this.m_runeUnholy.SetActive(false);
        }
        this.SetEmptyRuneVisible(false);
        break;
      case RuneType.RT_UNHOLY:
        if (animate)
        {
          this.m_runeFSM.SendEvent("Unholy_SpawnIn");
        }
        else
        {
          this.m_runeBlood.SetActive(false);
          this.m_runeFrost.SetActive(false);
          this.m_runeUnholy.SetActive(true);
        }
        this.SetEmptyRuneVisible(false);
        break;
    }
  }

  private void SetEmptyRuneVisible(bool visible)
  {
    if (visible)
    {
      this.m_emptyRune.SetActive(true);
    }
    else
    {
      this.m_emptyRuneHighlight.SetActive(false);
      this.m_emptyRune.SetActive(false);
    }
  }

  private void HideCurrentRune(bool animate)
  {
    if (animate)
    {
      switch (this.RuneType)
      {
        case RuneType.RT_BLOOD:
          this.m_runeFSM.SendEvent("Blood_SpawnOut");
          break;
        case RuneType.RT_FROST:
          this.m_runeFSM.SendEvent("Frost_SpawnOut");
          break;
        case RuneType.RT_UNHOLY:
          this.m_runeFSM.SendEvent("Unholy_SpawnOut");
          break;
        default:
          this.m_runeBlood.SetActive(false);
          this.m_runeFrost.SetActive(false);
          this.m_runeUnholy.SetActive(false);
          break;
      }
    }
    else
    {
      this.m_runeBlood.SetActive(false);
      this.m_runeFrost.SetActive(false);
      this.m_runeUnholy.SetActive(false);
    }
    this.m_runeHighlightFSM.SendEvent("Hover_Off");
    this.SetEmptyRuneVisible(true);
  }

  public void ShowNextRune()
  {
    if (this.m_runeIndex == RuneButton.m_runeOrder.Length - 1)
    {
      this.HideCurrentRune(true);
      this.m_runeIndex = 0;
    }
    else
    {
      ++this.m_runeIndex;
      this.ShowRune((RuneType) this.m_runeIndex, true);
    }
  }

  public void Initialize(int buttonIndex, RuneType rune)
  {
    this.ButtonIndex = buttonIndex;
    this.SetRune(rune, true);
  }

  public void SetHighlighted(bool highlighted)
  {
    if (this.RuneType == RuneType.RT_NONE)
      this.m_emptyRuneHighlight.SetActive(highlighted);
    else
      this.m_runeHighlightFSM.SendEvent(highlighted ? "Hover" : "Hover_Off");
  }

  public void PlayDragEffect() => this.m_runeFSM.SendEvent("PickUp");

  public void StopDragEffect() => this.m_runeFSM.SendEvent("START");
}
