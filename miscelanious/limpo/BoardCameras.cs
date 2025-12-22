using System.Collections;
using UnityEngine;

public class BoardCameras : MonoBehaviour
{
  public AudioListener m_AudioListener;
  public float m_FieldOfViewDefault = 34.87045f;
  public float m_FieldOfViewZoomed = 25f;
  public AnimationCurve m_ZoomCurve;
  private static BoardCameras s_instance;

  private void Awake()
  {
    BoardCameras.s_instance = this;
    if (!((Object) LoadingScreen.Get() != (Object) null))
      return;
    LoadingScreen.Get().NotifyMainSceneObjectAwoke(this.gameObject);
  }

  private void OnDestroy() => BoardCameras.s_instance = (BoardCameras) null;

  public static BoardCameras Get() => BoardCameras.s_instance;

  public AudioListener GetAudioListener() => this.m_AudioListener;

  public IEnumerator TweenCameraFieldOfView(float finalFieldOfView, float tweenTime)
  {
    float initialFieldOfView = Camera.main.fieldOfView;
    if ((double) finalFieldOfView != (double) initialFieldOfView)
    {
      Camera[] boardCameras = this.GetCameras();
      float timer = 0.0f;
      while ((double) timer < (double) tweenTime)
      {
        timer += Time.deltaTime;
        foreach (Camera camera in boardCameras)
          camera.fieldOfView = Mathf.Lerp(initialFieldOfView, finalFieldOfView, this.m_ZoomCurve.Evaluate(timer / tweenTime));
        yield return (object) null;
      }
      foreach (Camera camera in boardCameras)
        camera.fieldOfView = finalFieldOfView;
    }
  }

  private Camera[] GetCameras() => this.transform.GetComponentsInChildren<Camera>();
}
