using UnityEngine;

public class HSDontDestroyOnLoad : MonoBehaviour
{
  private void Awake() => Object.DontDestroyOnLoad((Object) this.gameObject);
}
