using UnityEngine;

public class ReturnBoardDustVFX : MonoBehaviour
{
  private void OnParticleSystemStopped() => Board.Get().ReturnDisabledDustVFX(this.gameObject);
}
