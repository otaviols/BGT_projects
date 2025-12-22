using Hearthstone.UI;
using Hearthstone.UI.Core;
using UnityEngine;

public class TemporaryWidgetInstance : MonoBehaviour
{
  [SerializeField]
  private GameObject m_widgetPrefab;
  [SerializeField]
  private bool m_shouldLoad;
  private WidgetInstance m_instance;
  [SerializeField]
  [HideInInspector]
  private string m_prefabPath;

  private void Start() => this.EnforceHaveInstance(this.m_shouldLoad);

  private void OnDestroy()
  {
    if (!((Object) this.m_instance != (Object) null))
      return;
    this.DestroyInstance();
  }

  [Overridable]
  public bool ShouldLoad
  {
    get => this.m_shouldLoad;
    set
    {
      this.m_shouldLoad = value;
      this.EnforceHaveInstance(this.m_shouldLoad);
    }
  }

  public WidgetInstance Instance => this.m_instance;

  public bool IsReady
  {
    get
    {
      if (!this.m_shouldLoad)
        return true;
      return !((Object) this.m_instance == (Object) null) && this.m_instance.IsReady;
    }
  }

  public bool IsChangingStates => (Object) this.m_instance != (Object) null && this.m_instance.IsChangingStates;

  private void EnforceHaveInstance(bool haveInstance)
  {
    if (!Application.isPlaying)
      return;
    if (haveInstance && (Object) this.m_instance == (Object) null)
    {
      this.CreateInstance();
    }
    else
    {
      if (haveInstance || !((Object) this.m_instance != (Object) null))
        return;
      this.DestroyInstance();
    }
  }

  private void CreateInstance()
  {
    if ((Object) this.transform == (Object) null || (Object) this.m_instance != (Object) null)
      return;
    this.m_instance = WidgetInstance.Create(this.m_prefabPath);
    GameObject gameObject = this.m_instance.gameObject;
    gameObject.name = this.m_widgetPrefab.name;
    gameObject.transform.SetParent(this.transform, false);
  }

  private void DestroyInstance()
  {
    if ((Object) this.m_instance == (Object) null)
      return;
    Object.Destroy((Object) this.m_instance.gameObject);
    this.m_instance = (WidgetInstance) null;
  }
}
