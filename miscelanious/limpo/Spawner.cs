using UnityEngine;

public class Spawner : MonoBehaviour
{
  public GameObject prefab;
  public bool spawnOnAwake;
  public bool destroyOnSpawn = true;

  protected virtual void Awake()
  {
    if (!this.spawnOnAwake)
      return;
    this.Spawn();
  }

  public GameObject Spawn()
  {
    GameObject gameObject = Object.Instantiate<GameObject>(this.prefab);
    gameObject.transform.parent = this.transform.parent;
    TransformUtil.CopyLocal(gameObject, (Component) this.transform);
    LayerUtils.SetLayer(gameObject, this.gameObject.layer);
    if (!this.destroyOnSpawn)
      return gameObject;
    Object.Destroy((Object) this.gameObject);
    return gameObject;
  }

  public T Spawn<T>() where T : MonoBehaviour
  {
    if ((Object) this.prefab.GetComponent<T>() != (Object) null)
      return this.Spawn().GetComponent<T>();
    Debug.Log((object) string.Format("The prefab for spawner {0} does not have component {1}", (object) this.gameObject.name, (object) typeof (T).Name));
    return default (T);
  }
}
