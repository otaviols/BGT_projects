using System.Collections;
using UnityEngine;

public class MassDisenchantTab : PegUIElement
{
  public GameObject m_root;
  public GameObject m_highlight;
  public UberText m_amount;
  private static readonly Vector3 SELECTED_SCALE = new Vector3(0.6f, 0.6f, 0.6f);
  private static readonly float SELECTED_LOCAL_Y_OFFSET = 0.3822131f;
  private bool m_isSelected;
  private Vector3 m_originalLocalPos;
  private Vector3 m_originalScale;
  private bool m_isVisible;

  protected override void Awake()
  {
    base.Awake();
    this.m_highlight.SetActive(false);
    this.m_originalLocalPos = this.transform.localPosition;
    this.m_originalScale = this.transform.localScale;
  }

  private void Start()
  {
    this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRollover));
    this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRollout));
  }

  public void Show()
  {
    this.m_isVisible = true;
    this.m_root.SetActive(true);
    this.SetEnabled(true);
  }

  public void Hide()
  {
    this.m_isVisible = false;
    this.m_root.SetActive(false);
    this.SetEnabled(false);
  }

  public bool IsVisible() => this.m_isVisible;

  public void SetAmount(int amount) => this.m_amount.Text = amount.ToString();

  public void Select()
  {
    if (this.m_isSelected)
      return;
    this.m_isSelected = true;
    Hashtable args1 = iTween.Hash((object) "scale", (object) MassDisenchantTab.SELECTED_SCALE, (object) "time", (object) CollectiblePageManager.SELECT_TAB_ANIM_TIME, (object) "name", (object) "scale");
    iTween.StopByName(this.gameObject, "scale");
    iTween.ScaleTo(this.gameObject, args1);
    Vector3 originalLocalPos = this.m_originalLocalPos;
    originalLocalPos.y += MassDisenchantTab.SELECTED_LOCAL_Y_OFFSET;
    Hashtable args2 = iTween.Hash((object) "position", (object) originalLocalPos, (object) "isLocal", (object) true, (object) "time", (object) CollectiblePageManager.SELECT_TAB_ANIM_TIME, (object) "name", (object) "position");
    iTween.StopByName(this.gameObject, "position");
    iTween.MoveTo(this.gameObject, args2);
  }

  public void Deselect()
  {
    if (!this.m_isSelected)
      return;
    this.m_isSelected = false;
    Hashtable args1 = iTween.Hash((object) "scale", (object) this.m_originalScale, (object) "time", (object) CollectiblePageManager.SELECT_TAB_ANIM_TIME, (object) "name", (object) "scale");
    iTween.StopByName(this.gameObject, "scale");
    iTween.ScaleTo(this.gameObject, args1);
    Hashtable args2 = iTween.Hash((object) "position", (object) this.m_originalLocalPos, (object) "isLocal", (object) true, (object) "time", (object) CollectiblePageManager.SELECT_TAB_ANIM_TIME, (object) "name", (object) "position");
    iTween.StopByName(this.gameObject, "position");
    iTween.MoveTo(this.gameObject, args2);
  }

  private void OnRollover(UIEvent e) => this.m_highlight.SetActive(true);

  private void OnRollout(UIEvent e) => this.m_highlight.SetActive(false);
}
