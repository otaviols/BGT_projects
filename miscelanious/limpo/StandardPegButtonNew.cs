using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class StandardPegButtonNew : PegUIElement
{
  public UberText m_buttonText;
  public ThreeSliceElement m_button;
  public ThreeSliceElement m_border;
  public ThreeSliceElement m_highlight;
  public GameObject m_upBone;
  public GameObject m_downBone;
  public float m_buttonWidth;
  public bool m_ExecuteInEditMode;
  private bool m_highlightLocked;
  private const float HIGHLIGHT_SCALE = 1.2f;
  private const float GRAY_FRAME_SCALE = 0.88f;

  public void SetText(string t) => this.m_buttonText.Text = t;

  public void SetWidth(float globalWidth)
  {
    this.m_button.SetWidth(globalWidth * 0.88f);
    if ((Object) this.m_border != (Object) null)
      this.m_border.SetWidth(globalWidth);
    Quaternion rotation = this.transform.rotation;
    this.transform.rotation = Quaternion.Euler(Vector3.zero);
    Vector3 size = this.m_button.GetSize();
    Vector3 worldScale = TransformUtil.ComputeWorldScale((Component) this.transform);
    Vector3 vector3 = new Vector3(size.x / worldScale.x, size.z / worldScale.z, size.y / worldScale.y);
    this.GetComponent<BoxCollider>().size = vector3;
    this.transform.rotation = rotation;
  }

  public void Show() => this.gameObject.SetActive(true);

  public void Hide() => this.gameObject.SetActive(false);

  public void Disable()
  {
    this.m_button.transform.localRotation = Quaternion.Euler(new Vector3(180f, 180f, 0.0f));
    this.SetEnabled(false);
  }

  public void Enable()
  {
    this.m_button.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 180f, 0.0f));
    this.SetEnabled(true);
  }

  public void Reset()
  {
    iTween.StopByName(this.m_button.gameObject, "rotation");
    this.m_button.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 180f, 0.0f));
  }

  public void LockHighlight()
  {
    this.m_highlight.gameObject.SetActive(true);
    this.m_highlightLocked = true;
  }

  public void UnlockHighlight()
  {
    this.m_highlight.gameObject.SetActive(false);
    this.m_highlightLocked = false;
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    if (this.m_highlightLocked)
      return;
    Hashtable tweenHashTable = iTweenManager.Get().GetTweenHashTable();
    tweenHashTable.Add((object) "amount", (object) new Vector3(90f, 0.0f, 0.0f));
    tweenHashTable.Add((object) "time", (object) 0.5f);
    tweenHashTable.Add((object) "name", (object) "rotation");
    iTween.StopByName(this.m_button.gameObject, "rotation");
    this.m_button.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 180f, 0.0f));
    iTween.PunchRotation(this.m_button.gameObject, tweenHashTable, false);
    this.m_highlight.gameObject.SetActive(true);
    SoundManager soundManager = SoundManager.Get();
    if (soundManager == null || !((Object) soundManager.GetConfig() != (Object) null))
      return;
    soundManager.LoadAndPlay((AssetReference) "Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9");
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    this.m_button.transform.localPosition = this.m_upBone.transform.localPosition;
    if (this.m_highlightLocked)
      return;
    this.m_highlight.gameObject.SetActive(false);
  }

  protected override void OnPress()
  {
    this.m_button.transform.localPosition = this.m_downBone.transform.localPosition;
    if (SoundManager.Get() == null || !((Object) SoundManager.Get().GetConfig() != (Object) null))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) "Back_Click.prefab:f7df4bfeab7ccff4198e670ca516da2e");
  }

  protected override void OnRelease() => this.m_button.transform.localPosition = this.m_upBone.transform.localPosition;
}
