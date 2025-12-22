using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class GuestHeroPickerDisplay : MonoBehaviour
{
  public AsyncReference m_trayControllerReference;
  public AsyncReference m_trayControllerReference_phone;
  private static GuestHeroPickerDisplay s_instance;
  private GuestHeroPickerTrayDisplay m_heroPickerTray;
  private Vector3 startOffset = new Vector3(-120f, 0.0f, 0.0f);
  private Vector3 startPosition;

  private void Awake()
  {
    if ((UnityEngine.Object) GuestHeroPickerDisplay.s_instance != (UnityEngine.Object) null)
      Debug.LogWarning((object) "GuestHeroPickerDisplay is supposed to be a singleton, but a second instance of it is being created!");
    GuestHeroPickerDisplay.s_instance = this;
    this.startPosition = this.transform.localPosition;
    this.transform.localPosition = this.startPosition + this.startOffset;
    this.m_trayControllerReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnTrayControllerReady));
    this.m_trayControllerReference_phone.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnTrayControllerReady));
    SoundManager.Get().Load(SoundUtils.SquarePanelSlideOnSFX);
    SoundManager.Get().Load(SoundUtils.SquarePanelSlideOffSFX);
  }

  private void OnTrayControllerReady(VisualController trayController)
  {
    this.m_heroPickerTray = trayController.GetComponentInChildren<GuestHeroPickerTrayDisplay>();
    if ((UnityEngine.Object) this.m_heroPickerTray == (UnityEngine.Object) null)
      Debug.LogError((object) "GuestHeroPickerTrayDisplay component not found in GuestHeroPickerTray object.");
    else if ((UnityEngine.Object) trayController == (UnityEngine.Object) null)
      Debug.LogError((object) "trayController was null in OnTrayControllerReady!");
    else
      this.m_heroPickerTray.InitAssets();
  }

  public void ShowTray()
  {
    Hashtable args = iTween.Hash((object) "position", (object) this.startPosition, (object) "time", (object) 1f, (object) "isLocal", (object) true, (object) "oncomplete", (object) "OnTrayShown", (object) "oncompletetarget", (object) this.gameObject, (object) "easeType", (object) iTween.EaseType.easeOutBounce);
    SoundManager.Get().LoadAndPlay(SoundUtils.SquarePanelSlideOnSFX);
    iTween.MoveTo(this.gameObject, args);
  }

  public void OnTrayShown()
  {
    this.m_heroPickerTray.EnableBackButton(true);
    if (!((UnityEngine.Object) PvPDungeonRunScene.Get() != (UnityEngine.Object) null))
      return;
    PvPDungeonRunScene.Get().OnHeroPickerShown();
  }

  public void HideTray(float delay = 0.0f)
  {
    Hashtable args = iTween.Hash((object) "position", (object) (this.startPosition + this.startOffset), (object) "time", (object) 1f, (object) "isLocal", (object) true, (object) "oncomplete", (object) "OnTrayHidden", (object) "oncompletetarget", (object) this.gameObject, (object) "easeType", (object) iTween.EaseType.easeOutBounce, (object) nameof (delay), (object) delay);
    SoundManager.Get().LoadAndPlay(SoundUtils.SquarePanelSlideOffSFX);
    iTween.MoveTo(this.gameObject, args);
  }

  private void OnTrayHidden()
  {
    this.m_heroPickerTray.Unload();
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.gameObject);
    if ((UnityEngine.Object) TavernBrawlDisplay.Get() != (UnityEngine.Object) null)
      TavernBrawlDisplay.Get().OnHeroPickerClosed();
    if (!((UnityEngine.Object) PvPDungeonRunScene.Get() != (UnityEngine.Object) null))
      return;
    PvPDungeonRunScene.Get().OnHeroPickerHidden();
  }

  public static GuestHeroPickerDisplay Get() => GuestHeroPickerDisplay.s_instance;
}
