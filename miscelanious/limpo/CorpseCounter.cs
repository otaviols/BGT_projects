using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CorpseCounter : MonoBehaviour
{
  private static UnityEvent s_initializeEvent;
  private static UnityEvent s_updateEvent;
  private static UnityEvent s_hidePhoneManaTrayEvent;
  private static UnityEvent s_showPhoneManaTrayEvent;
  public UberText m_textbox;
  public GameObject m_symbol;
  public Player.Side m_side;
  private bool m_shown;
  private int m_numOfCorpses;
  private const int NUM_CAP = 99;
  private Vector3 m_startingScale;
  private bool m_initialized;
  private PlayMakerFSM m_FSMComponent;

  static CorpseCounter()
  {
    if (CorpseCounter.s_initializeEvent == null)
      CorpseCounter.s_initializeEvent = new UnityEvent();
    if (CorpseCounter.s_updateEvent == null)
      CorpseCounter.s_updateEvent = new UnityEvent();
    if (CorpseCounter.s_hidePhoneManaTrayEvent == null)
      CorpseCounter.s_hidePhoneManaTrayEvent = new UnityEvent();
    if (CorpseCounter.s_showPhoneManaTrayEvent != null)
      return;
    CorpseCounter.s_showPhoneManaTrayEvent = new UnityEvent();
  }

  private void OnEnable()
  {
    this.m_FSMComponent = this.GetComponent<PlayMakerFSM>();
    CorpseCounter.s_initializeEvent.AddListener(new UnityAction(this.DelayThenInitialize));
    CorpseCounter.s_updateEvent.AddListener(new UnityAction(this.UpdateText));
    CorpseCounter.s_hidePhoneManaTrayEvent.AddListener(new UnityAction(this.PhoneManaTrayHideFX));
    CorpseCounter.s_showPhoneManaTrayEvent.AddListener(new UnityAction(this.PhoneManaTrayShowFX));
  }

  private void OnDisable()
  {
    this.m_FSMComponent = (PlayMakerFSM) null;
    CorpseCounter.s_initializeEvent.RemoveListener(new UnityAction(this.DelayThenInitialize));
    CorpseCounter.s_updateEvent.RemoveListener(new UnityAction(this.UpdateText));
    CorpseCounter.s_hidePhoneManaTrayEvent.RemoveListener(new UnityAction(this.PhoneManaTrayHideFX));
    CorpseCounter.s_showPhoneManaTrayEvent.RemoveListener(new UnityAction(this.PhoneManaTrayShowFX));
  }

  public static void InitializeAll()
  {
    if (CorpseCounter.s_initializeEvent == null)
      return;
    CorpseCounter.s_initializeEvent.Invoke();
  }

  public static void UpdateTextAll()
  {
    if (CorpseCounter.s_updateEvent == null)
      return;
    CorpseCounter.s_updateEvent.Invoke();
  }

  public static void HidePhoneManaTray()
  {
    if (CorpseCounter.s_hidePhoneManaTrayEvent == null)
      return;
    CorpseCounter.s_hidePhoneManaTrayEvent.Invoke();
  }

  public static void ShowPhoneManaTray()
  {
    if (CorpseCounter.s_showPhoneManaTrayEvent == null)
      return;
    CorpseCounter.s_showPhoneManaTrayEvent.Invoke();
  }

  public bool IsShown() => this.m_shown;

  private IEnumerator DelayedInitialization()
  {
    yield return (object) new WaitForSeconds(1f);
    this.Initialize();
  }

  private void DelayThenInitialize() => this.StartCoroutine(this.DelayedInitialization());

  private void Initialize()
  {
    if (this.m_initialized)
      return;
    this.m_startingScale = this.m_textbox.gameObject.transform.localScale;
    this.m_initialized = true;
    this.UpdateText();
  }

  private void UpdateText()
  {
    this.Initialize();
    if ((Object) this.m_textbox == (Object) null)
      Debug.LogWarningFormat("UpdateText() is called with no textbox set.");
    else if ((Object) this.m_symbol == (Object) null)
      Debug.LogWarningFormat("UpdateText() is called with no symbol set.");
    else if (this.ShouldShowCorpseCounter())
    {
      int availableCorpses = this.GetPlayer().GetNumAvailableCorpses();
      if (availableCorpses > this.m_numOfCorpses)
        this.CorpseCountIncreaseFX();
      else if (availableCorpses < this.m_numOfCorpses)
        this.CorpseCountDecreaseFX();
      if (availableCorpses != this.m_numOfCorpses)
        this.Jiggle();
      this.m_numOfCorpses = availableCorpses;
      this.m_symbol.SetActive(true);
      this.m_textbox.Text = this.m_numOfCorpses <= 99 ? this.m_numOfCorpses.ToString() : 99.ToString() + "+";
      this.m_shown = true;
    }
    else
    {
      this.m_symbol.SetActive(false);
      this.m_textbox.Text = "";
      this.m_shown = false;
    }
  }

  private Player GetPlayer() => this.m_side != Player.Side.FRIENDLY ? GameState.Get().GetOpposingSidePlayer() : GameState.Get().GetFriendlySidePlayer();

  private bool ShouldShowCorpseCounter()
  {
    Player player = this.GetPlayer();
    return player != null && player.GetHero() != null && player.GetHero().HasClass(TAG_CLASS.DEATHKNIGHT);
  }

  private void Jiggle()
  {
    iTween.Stop(this.m_textbox.gameObject);
    this.m_textbox.gameObject.transform.localScale = this.m_startingScale;
    iTween.PunchScale(this.m_textbox.gameObject, Vector3.one, 1f);
  }

  private void CorpseCountIncreaseFX()
  {
    if (!((Object) this.m_FSMComponent != (Object) null))
      return;
    this.m_FSMComponent.SendEvent("Increase");
  }

  private void CorpseCountDecreaseFX()
  {
    if (!((Object) this.m_FSMComponent != (Object) null))
      return;
    this.m_FSMComponent.SendEvent("Decrease");
  }

  private void PhoneManaTrayHideFX()
  {
    if (!((Object) this.m_FSMComponent != (Object) null))
      return;
    this.m_FSMComponent.SendEvent("HideManaTray");
  }

  private void PhoneManaTrayShowFX()
  {
    if (!((Object) this.m_FSMComponent != (Object) null))
      return;
    this.m_FSMComponent.SendEvent("ShowManaTray");
  }
}
