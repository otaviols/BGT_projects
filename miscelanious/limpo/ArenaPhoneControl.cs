using System;
using UnityEngine;

public class ArenaPhoneControl : MonoBehaviour
{
  public UberText m_ChooseText;
  public UberText m_ChooseDetailText;
  public GameObject m_ViewDeckButton;
  public BoxCollider m_ButtonCollider;
  public Vector3 m_CountAndViewDeckCollCenter;
  public Vector3 m_CountAndViewDeckCollSize;
  public Vector3 m_ViewDeckCollCenter;
  public Vector3 m_ViewDeckCollSize;
  private ArenaPhoneControl.ControlMode m_CurrentMode;

  private void Awake()
  {
    this.m_CurrentMode = ArenaPhoneControl.ControlMode.ChooseHero;
    this.m_ButtonCollider.enabled = false;
    this.m_ChooseText.Text = GameStrings.Get("GLUE_CHOOSE_YOUR_HERO");
  }

  [ContextMenu("ChooseHero")]
  public void SetModeChooseHero() => this.SetMode(ArenaPhoneControl.ControlMode.ChooseHero);

  [ContextMenu("ChooseCard")]
  public void SetModeChooseCard() => this.SetMode(ArenaPhoneControl.ControlMode.ChooseCard);

  [ContextMenu("CardCountViewDeck")]
  public void SetModeCardCountViewDeck() => this.SetMode(ArenaPhoneControl.ControlMode.CardCountViewDeck);

  [ContextMenu("ViewDeck")]
  public void SetModeViewDeck() => this.SetMode(ArenaPhoneControl.ControlMode.ViewDeck);

  [ContextMenu("ChooseHeroPower")]
  public void SetModeChooseHeroPower() => this.SetMode(ArenaPhoneControl.ControlMode.ChooseHeroPower);

  public void SetMode(ArenaPhoneControl.ControlMode mode)
  {
    if (mode == this.m_CurrentMode)
      return;
    switch (mode)
    {
      case ArenaPhoneControl.ControlMode.ChooseHero:
        this.m_ViewDeckButton.SetActive(false);
        this.m_ButtonCollider.enabled = false;
        this.m_ChooseText.Text = GameStrings.Get("GLUE_CHOOSE_YOUR_HERO");
        this.m_ChooseDetailText.Text = string.Empty;
        if (this.m_CurrentMode == ArenaPhoneControl.ControlMode.CardCountViewDeck)
        {
          this.RotateTo(180f, 0.0f);
          break;
        }
        break;
      case ArenaPhoneControl.ControlMode.ChooseHeroPower:
        this.m_ViewDeckButton.SetActive(false);
        this.m_ButtonCollider.enabled = false;
        this.m_ChooseText.Text = GameStrings.Get("GLUE_DRAFT_HERO_POWER_INSTRUCTIONS_TITLE");
        this.m_ChooseDetailText.Text = GameStrings.Get("GLUE_DRAFT_HERO_POWER_INSTRUCTIONS_DETAIL");
        if (this.m_CurrentMode == ArenaPhoneControl.ControlMode.CardCountViewDeck)
        {
          this.RotateTo(180f, 0.0f);
          break;
        }
        break;
      case ArenaPhoneControl.ControlMode.ChooseCard:
        this.m_ViewDeckButton.SetActive(false);
        this.m_ButtonCollider.enabled = false;
        this.m_ChooseText.Text = GameStrings.Get("GLUE_DRAFT_INSTRUCTIONS");
        this.m_ChooseDetailText.Text = string.Empty;
        if (this.m_CurrentMode == ArenaPhoneControl.ControlMode.CardCountViewDeck)
        {
          this.RotateTo(180f, 0.0f);
          break;
        }
        break;
      case ArenaPhoneControl.ControlMode.CardCountViewDeck:
        this.m_ButtonCollider.center = this.m_CountAndViewDeckCollCenter;
        this.m_ButtonCollider.size = this.m_CountAndViewDeckCollSize;
        this.m_ButtonCollider.enabled = true;
        this.RotateTo(0.0f, 180f);
        break;
      case ArenaPhoneControl.ControlMode.ViewDeck:
        this.m_ButtonCollider.center = this.m_ViewDeckCollCenter;
        this.m_ButtonCollider.size = this.m_ViewDeckCollSize;
        this.m_ViewDeckButton.SetActive(true);
        this.m_ButtonCollider.enabled = true;
        if (this.m_CurrentMode == ArenaPhoneControl.ControlMode.CardCountViewDeck)
        {
          this.RotateTo(180f, 0.0f);
          break;
        }
        break;
      case ArenaPhoneControl.ControlMode.Rewards:
        this.m_ViewDeckButton.SetActive(false);
        this.m_ButtonCollider.enabled = false;
        this.m_ChooseText.Text = string.Empty;
        this.m_ChooseDetailText.Text = string.Empty;
        if (this.m_CurrentMode == ArenaPhoneControl.ControlMode.CardCountViewDeck)
        {
          this.RotateTo(180f, 0.0f);
          break;
        }
        break;
    }
    this.m_CurrentMode = mode;
  }

  private void RotateTo(float rotFrom, float rotTo) => iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) rotFrom, (object) "to", (object) rotTo, (object) "time", (object) 1f, (object) "easetype", (object) iTween.EaseType.easeOutBounce, (object) "onupdate", (object) (Action<object>) (val => this.transform.localRotation = Quaternion.Euler((float) val, 0.0f, 0.0f))));

  public enum ControlMode
  {
    ChooseHero,
    ChooseHeroPower,
    ChooseCard,
    CardCountViewDeck,
    ViewDeck,
    Rewards,
  }
}
