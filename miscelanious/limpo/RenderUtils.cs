using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public static class RenderUtils
{
  public static readonly int MinRenderQueueValue = 1;
  public static readonly int MaxRenderQueueValue = 5000;

  public static void SetAlpha(GameObject go, float alpha) => RenderUtils.SetAlpha(go, alpha, false);

  public static void SetAlpha(GameObject go, float alpha, bool includeInactive)
  {
    foreach (Renderer componentsInChild in go.GetComponentsInChildren<Renderer>(includeInactive))
    {
      foreach (Material material in componentsInChild.GetMaterials())
      {
        if (material.HasProperty("_Color"))
        {
          Color color = material.color with
          {
            a = alpha
          };
          material.color = color;
        }
        else if (material.HasProperty("_TintColor"))
        {
          Color color = material.GetColor("_TintColor") with
          {
            a = alpha
          };
          material.SetColor("_TintColor", color);
        }
      }
      Light component;
      if (componentsInChild.TryGetComponent<Light>(out component))
      {
        Color color = component.color with
        {
          a = alpha
        };
        component.color = color;
      }
    }
    foreach (UberText componentsInChild in go.GetComponentsInChildren<UberText>(includeInactive))
    {
      Color textColor = componentsInChild.TextColor;
      componentsInChild.TextColor = new Color(textColor.r, textColor.g, textColor.b, alpha);
    }
  }

  public static int ClampRenderQueueValue(int value) => Mathf.Clamp(value, RenderUtils.MinRenderQueueValue, RenderUtils.MaxRenderQueueValue);

  public static void SetInvisibleRenderer(
    Renderer renderer,
    bool show,
    ref Map<Renderer, int> originalLayers)
  {
    if (originalLayers == null)
      originalLayers = new Map<Renderer, int>();
    if (!((Object) renderer != (Object) null))
      return;
    GameObject gameObject = renderer.gameObject;
    int layer = gameObject.layer;
    int num = layer;
    if (show && layer == 28 && !originalLayers.TryGetValue(renderer, out num))
      num = layer;
    if (!show && layer != 28)
    {
      originalLayers[renderer] = layer;
      num = 28;
    }
    gameObject.layer = num;
  }

  public static void SetRenderQueue(GameObject go, int renderQueue, bool includeInactive = false)
  {
    foreach (Renderer componentsInChild in go.GetComponentsInChildren<Renderer>(includeInactive))
    {
      Material material = componentsInChild.GetMaterial();
      if (!((Object) material == (Object) null))
        material.renderQueue = renderQueue;
    }
  }

  public static void EnableRenderers(GameObject go, bool enable) => RenderUtils.EnableRenderers(go, enable, false);

  public static void EnableRenderers(GameObject go, bool enable, bool includeInactive)
  {
    Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(includeInactive);
    if (componentsInChildren == null)
      return;
    foreach (Renderer renderer in componentsInChildren)
      renderer.enabled = enable;
  }

  public static void EnableColliders(GameObject go, bool enable)
  {
    Collider[] componentsInChildren = go.GetComponentsInChildren<Collider>();
    if (componentsInChildren == null)
      return;
    foreach (Collider collider in componentsInChildren)
      collider.enabled = enable;
  }

  public static void EnableRenderersAndColliders(GameObject go, bool enable)
  {
    Collider component1 = go.GetComponent<Collider>();
    if ((Object) component1 != (Object) null)
      component1.enabled = enable;
    Renderer component2 = go.GetComponent<Renderer>();
    if ((Object) component2 != (Object) null)
      component2.enabled = enable;
    foreach (Component component3 in go.transform)
      RenderUtils.EnableRenderersAndColliders(component3.gameObject, enable);
  }
}
