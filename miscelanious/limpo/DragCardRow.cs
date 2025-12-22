using UnityEngine;

public class DragCardRow : MonoBehaviour
{
  private float m_CursorX;
  private Rect dragRect;

  private void Awake()
  {
    BoxCollider component = this.gameObject.GetComponent<BoxCollider>();
    Vector3 min = component.bounds.min;
    Vector3 max = component.bounds.max;
    this.dragRect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
  }

  private void OnMouseDown() => this.m_CursorX = InputCollection.GetMousePosition().x;

  private void OnMouseDrag()
  {
    float x = InputCollection.GetMousePosition().x;
    this.transform.Translate((x - this.m_CursorX) * 0.01f, 0.0f, 0.0f);
    this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, this.dragRect.xMin, this.dragRect.xMax), this.transform.position.y, this.transform.position.z);
    this.m_CursorX = x;
  }
}
