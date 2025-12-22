using System;
using UnityEngine;

public class DeferredEnableHandler : MonoBehaviour
{
  private event Action m_listener;

  public static void AttachTo(Component comp, Action callback)
  {
    if ((UnityEngine.Object) comp == (UnityEngine.Object) null)
      return;
    DeferredEnableHandler.AttachTo(comp.gameObject, callback);
  }

  public static void AttachTo(GameObject go, Action callback)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      return;
    (go.GetComponent<DeferredEnableHandler>() ?? go.AddComponent<DeferredEnableHandler>()).SetEnableListener(callback);
  }

  private void SetEnableListener(Action callback) => this.m_listener = callback;

  private void OnEnable()
  {
    if (this.m_listener != null)
      this.m_listener();
    UnityEngine.Object.Destroy((UnityEngine.Object) this);
  }
}
