using UnityEngine;

public class LayerUtils
{
  public static void SetLayer(GameObject go, int layer, int? ignoredLayer = null)
  {
    if (!ignoredLayer.HasValue || go.layer != ignoredLayer.Value)
      go.layer = layer;
    foreach (Component component in go.transform)
      LayerUtils.SetLayer(component.gameObject, layer, ignoredLayer);
  }

  public static void SetLayer(Component c, int layer) => LayerUtils.SetLayer(c.gameObject, layer);

  public static void SetLayer(GameObject go, GameLayer layer) => LayerUtils.SetLayer(go, (int) layer);

  public static void SetLayer(Component c, GameLayer layer) => LayerUtils.SetLayer(c.gameObject, (int) layer);

  public static void ReplaceLayer(GameObject parentObject, GameLayer newLayer, GameLayer oldLayer)
  {
    if ((GameLayer) parentObject.layer == oldLayer)
      parentObject.layer = (int) newLayer;
    foreach (Component component in parentObject.transform)
      LayerUtils.ReplaceLayer(component.gameObject, newLayer, oldLayer);
  }
}
