using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FriendListItemHeader : PegUIElement, ITouchListItem
{
  public UberText m_Text;
  public GameObject m_Arrow;
  public Transform m_FoldinBone;
  public Transform m_FoldoutBone;
  public float m_AnimRotateTime = 0.25f;
  public bool m_toggleEnabled = true;
  public float m_textXOffsetWhenToggleDisabled = -0.2f;
  private List<FriendListItemHeader.ToggleContentsListener> m_ToggleEventListeners = new List<FriendListItemHeader.ToggleContentsListener>();
  private bool m_ShowContents = true;
  private MultiSliceElement m_multiSlice;

  public GameObject Background { get; set; }

  public Bounds LocalBounds { get; private set; }

  public void SetText(string text) => this.m_Text.Text = text;

  public bool IsHeader => true;

  public bool Visible
  {
    get => this.IsShowingContents;
    set
    {
    }
  }

  public bool IsShowingContents => this.m_ShowContents;

  public MobileFriendListItem.TypeFlags SubType { get; set; }

  public Option Option { get; set; }

  public void SetInitialShowContents(bool show)
  {
    this.m_ShowContents = show;
    if (!((Object) this.m_Arrow != (Object) null))
      return;
    this.m_Arrow.transform.rotation = this.GetCurrentBoneTransform().rotation;
  }

  public void AddToggleListener(FriendListItemHeader.ToggleContentsFunc func, object userdata)
  {
    FriendListItemHeader.ToggleContentsListener contentsListener = new FriendListItemHeader.ToggleContentsListener();
    contentsListener.SetCallback(func);
    contentsListener.SetUserData(userdata);
    this.m_ToggleEventListeners.Add(contentsListener);
  }

  public void ClearToggleListeners() => this.m_ToggleEventListeners.Clear();

  public new T GetComponent<T>() where T : Component => base.GetComponent<T>();

  public void SetToggleEnabled(bool enabled)
  {
    this.m_toggleEnabled = enabled;
    if (!enabled)
    {
      this.m_ShowContents = true;
      if ((Object) this.m_Arrow != (Object) null)
        TransformUtil.SetLocalPosX((Component) this.m_Text, this.m_textXOffsetWhenToggleDisabled);
    }
    if (!((Object) this.m_Arrow != (Object) null))
      return;
    this.m_Arrow.gameObject.SetActive(enabled);
  }

  protected override void Awake()
  {
    base.Awake();
    this.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnHeaderButtonReleased));
    if (!((Object) this.m_multiSlice == (Object) null))
      return;
    this.m_multiSlice = this.GetComponentInChildren<MultiSliceElement>();
    if (!(bool) (Object) this.m_multiSlice)
      return;
    this.m_multiSlice.UpdateSlices();
  }

  protected virtual void OnHeaderButtonReleased(UIEvent e)
  {
    if (!this.m_toggleEnabled)
      return;
    this.m_ShowContents = !this.m_ShowContents;
    foreach (FriendListItemHeader.ToggleContentsListener contentsListener in this.m_ToggleEventListeners.ToArray())
      contentsListener.Fire(this.m_ShowContents);
    this.UpdateFoldoutArrow();
  }

  private void UpdateFoldoutArrow()
  {
    if ((Object) this.m_Arrow == (Object) null || (Object) this.m_FoldinBone == (Object) null || (Object) this.m_FoldoutBone == (Object) null)
      return;
    iTween.RotateTo(this.m_Arrow, this.GetCurrentBoneTransform().rotation.eulerAngles, this.m_AnimRotateTime);
  }

  private Transform GetCurrentBoneTransform() => !this.m_ShowContents ? this.m_FoldinBone : this.m_FoldoutBone;

  public void OnScrollOutOfView()
  {
  }

  public void OnPositionUpdate()
  {
  }

  [SpecialName]
  GameObject ITouchListItem.get_gameObject() => this.gameObject;

  [SpecialName]
  Transform ITouchListItem.get_transform() => this.transform;

  public delegate void ToggleContentsFunc(bool show, object userdata);

  private class ToggleContentsListener : EventListener<FriendListItemHeader.ToggleContentsFunc>
  {
    public void Fire(bool show) => this.m_callback(show, this.m_userData);
  }
}
