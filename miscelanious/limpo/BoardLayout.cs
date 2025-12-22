using Assets;
using UnityEngine;

public class BoardLayout : MonoBehaviour
{
  public Transform m_BoneParent;
  public Transform m_ColliderParent;

  public void Awake()
  {
    if (!((Object) LoadingScreen.Get() != (Object) null))
      return;
    LoadingScreen.Get().NotifyMainSceneObjectAwoke(this.gameObject);
  }

  public Transform FindBone(string name) => this.m_BoneParent.Find(name);

  public Collider FindCollider(string name)
  {
    Transform transform = this.m_ColliderParent.Find(name);
    return !((Object) transform == (Object) null) ? transform.GetComponent<Collider>() : (Collider) null;
  }

  public static string GetBoardLayoutPrefab(Scenario.BoardLayout boardLayout)
  {
    if (boardLayout == Scenario.BoardLayout.STANDARD)
      return "BoardStandardGame.prefab:b87d693f752160b43a25b7cec3787122";
    return boardLayout == Scenario.BoardLayout.LETTUCE ? "BoardLettuceGame.prefab:9e87f54ccdfb2d848b82dbba40b52df4" : (string) null;
  }
}
