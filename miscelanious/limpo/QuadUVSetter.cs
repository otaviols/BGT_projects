using UnityEngine;

[ExecuteInEditMode]
public class QuadUVSetter : MonoBehaviour
{
  public Vector2 Origin;
  public float Width;
  public float Height;
  public Vector2 ImageSize;

  private void Start() => this.SetUVs();

  public void SetUVs()
  {
  }

  private void Update() => this.SetUVs();
}
