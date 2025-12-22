using Hearthstone.UI;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class CollectionMercVisual : PegUIElement, IDraggableCollectionVisual
{
  private float m_wiggleIntensity;
  private Quaternion m_originalButtonRotation;
  private int m_positionIndex;
  private Listable m_contentsController;
  private WidgetInstance m_widgetInstance;
  private DeckTrayMercListContent m_listContentController;
  private Transform m_cachedTransform;

  protected override void Awake()
  {
    base.Awake();
    this.m_cachedTransform = this.transform;
  }

  private void Start()
  {
    this.m_contentsController = this.gameObject.GetComponentInParentsOnly<Listable>();
    this.m_widgetInstance = this.gameObject.GetComponentInParentsOnly<WidgetInstance>();
    this.m_listContentController = this.gameObject.GetComponentInParentsOnly<DeckTrayMercListContent>();
  }

  private void OnEnable()
  {
    this.m_originalButtonRotation = this.m_cachedTransform.localRotation;
    this.SetPositionIndexFromWidgetItems();
  }

  private void Update()
  {
    float stopTweenDuration1 = this.m_listContentController.m_rearrangeStartStopTweenDuration;
    float stopTweenDuration2 = this.m_listContentController.m_rearrangeStartStopTweenDuration;
    float rearrangeWiggleFrequency = this.m_listContentController.m_rearrangeWiggleFrequency;
    float rearrangeWiggleAmplitude = this.m_listContentController.m_rearrangeWiggleAmplitude;
    Vector3 rearrangeWiggleAxis = this.m_listContentController.m_rearrangeWiggleAxis;
    int num = !this.m_listContentController.IsReorderingAllowed || this.m_listContentController.DraggingDeckBox == null ? 0 : (this.m_listContentController.DraggingDeckBox != this ? 1 : 0);
    bool flag1 = (double) this.m_wiggleIntensity > 0.0;
    this.m_wiggleIntensity = num == 0 ? Mathf.Clamp01(this.m_wiggleIntensity - Time.deltaTime / stopTweenDuration2) : Mathf.Clamp01(this.m_wiggleIntensity + Time.deltaTime / stopTweenDuration1);
    bool flag2 = (double) this.m_wiggleIntensity > 0.0;
    if (!(flag1 | flag2))
      return;
    this.m_cachedTransform.localRotation = Quaternion.AngleAxis(rearrangeWiggleAmplitude * this.m_wiggleIntensity * Mathf.Cos((float) this.m_positionIndex + Time.time * rearrangeWiggleFrequency), rearrangeWiggleAxis) * this.m_originalButtonRotation;
  }

  protected override void OnHold()
  {
    if ((Object) this.m_listContentController == (Object) null || this.m_listContentController.IsTouchDragging)
      return;
    this.m_listContentController.StartDragToReorder((IDraggableCollectionVisual) this);
  }

  public void OnStopDragToReorder() => this.SetPositionIndexFromWidgetItems();

  private void SetPositionIndexFromWidgetItems()
  {
    if (!(bool) (Object) this.m_contentsController)
      return;
    this.m_positionIndex = 0;
    using (IEnumerator<WidgetInstance> enumerator = this.m_contentsController.WidgetItems.GetEnumerator())
    {
      while (enumerator.MoveNext() && !((Object) enumerator.Current == (Object) this.m_widgetInstance))
        ++this.m_positionIndex;
    }
  }
}
