using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
  private Quaternion rotation;

  private void Awake() => this.rotation = this.transform.rotation;

  private void LateUpdate() => this.transform.rotation = this.rotation;
}
