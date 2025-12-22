using Hearthstone.Progression;
using Hearthstone.UI;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class AchievementTab : MonoBehaviour
{
  [SerializeField]
  private UberText m_PointsText;
  private const string START_POINT_HUD_ANIMATION = "CODE_START_POINT_HUD_ANIMATION";
  private const int ROLL_UP_TIME = 1;
  private WidgetTemplate m_widget;
  private int m_currentPointsValue;
  private Coroutine m_rollUpRoutine;

  private void Awake()
  {
    this.m_widget = this.GetComponent<WidgetTemplate>();
    AchievementManager.Get().OnPointsChanged += new AchievementManager.PointsChangedDelegate(this.OnPointsChanged);
  }

  private void OnEnable()
  {
    this.m_currentPointsValue = AchievementManager.Get().TotalPoints;
    this.m_PointsText.Text = this.m_currentPointsValue.ToString();
  }

  private void OnDestroy()
  {
    if (AchievementManager.Get() == null)
      return;
    AchievementManager.Get().OnPointsChanged -= new AchievementManager.PointsChangedDelegate(this.OnPointsChanged);
  }

  private void OnPointsChanged()
  {
    this.m_widget.TriggerEvent("CODE_START_POINT_HUD_ANIMATION", new Widget.TriggerEventParameters());
    if (this.m_rollUpRoutine != null)
      this.StopCoroutine(this.m_rollUpRoutine);
    this.m_rollUpRoutine = this.StartCoroutine(this.RollUpPoints());
  }

  private IEnumerator RollUpPoints()
  {
    int targetPointsValue = AchievementManager.Get().TotalPoints;
    float time = 0.0f;
    float rollupPoints = (float) this.m_currentPointsValue;
    while ((double) time < 1.0)
    {
      rollupPoints = Mathf.Lerp(rollupPoints, (float) targetPointsValue, time / 1f);
      time += Time.deltaTime;
      this.m_currentPointsValue = Mathf.FloorToInt(rollupPoints);
      this.m_PointsText.Text = this.m_currentPointsValue.ToString();
      yield return (object) null;
    }
    this.m_currentPointsValue = targetPointsValue;
    this.m_PointsText.Text = targetPointsValue.ToString();
  }
}
