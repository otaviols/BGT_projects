using Hearthstone.UI;
using System.Collections;
using UnityEngine;

public class LuckyDrawButton : MonoBehaviour
{
  [SerializeField]
  private Clickable m_luckyDrawButtonClickable;
  private Widget m_widget;
  private VisualController m_visualController;
  private const string ENABLED = "ENABLED";
  private const string DISABLED = "DISABLED";
  private const string SHOWHIGHLIGHT = "SHOW_BLUE_HIGHLIGHT_RING_CODE";
  private const string HIDEHIGHLIGHT = "HIDE_BLUE_HIGHLIGHT_RING_CODE";

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    if ((Object) this.m_widget == (Object) null)
    {
      Log.ErrorReporter.PrintError("Error: [LuckyDrawButton] No Component of type WidgetTemplate found on {0} cannot instantiate LuckyDrawButton", (object) this.gameObject.name);
    }
    else
    {
      this.m_visualController = this.GetComponent<VisualController>();
      this.StartCoroutine(this.BindLuckyDrawDataModel());
    }
  }

  private IEnumerator BindLuckyDrawDataModel()
  {
    LuckyDrawManager luckyDrawManager = LuckyDrawManager.Get();
    while (luckyDrawManager.IsDataDirty())
      yield return (object) new WaitForSeconds(0.1f);
    luckyDrawManager.BindAllLuckyDrawDataModelToWidget(this.m_widget);
  }

  public void SetUserInteractionEnabled(bool enabled) => this.m_luckyDrawButtonClickable.enabled = enabled;
}
