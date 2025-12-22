using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

public class LettuceRoleTab : BookTab
{
  public Vector3 m_MobileDeselectedLocalScale = new Vector3(10f, 10f, 10f);
  public Vector3 m_MobileSelectedLocalScale = new Vector3(12f, 12f, 12f);
  public float m_MobileSelectedLocalYPos = 0.1259841f;
  public float m_MobileDeselectedLocalYPos;
  public AsyncReference m_roleIconsReference;
  public AsyncReference m_clickFXReference;
  private TAG_ROLE m_roleTag;
  private VisualController m_roleIconsController;
  private VisualController m_clickFXController;

  public void Init(TAG_ROLE roleTag)
  {
    this.m_roleTag = roleTag;
    this.m_roleIconsReference.RegisterReadyListener<VisualController>((Action<VisualController>) (vc => this.m_roleIconsController = vc));
    this.m_clickFXReference.RegisterReadyListener<VisualController>((Action<VisualController>) (vc => this.m_clickFXController = vc));
    this.StartCoroutine(this.InitializeWhenReady());
    this.Init();
  }

  public TAG_ROLE GetRole() => this.m_roleTag;

  public override void SetLargeTab(bool large)
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
    {
      base.SetLargeTab(large);
    }
    else
    {
      if (large == this.m_showLargeTab)
        return;
      if (large)
      {
        this.transform.localPosition = this.transform.localPosition with
        {
          y = this.m_MobileSelectedLocalYPos
        };
        iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) this.m_MobileSelectedLocalScale, (object) "time", (object) BookTab.SELECT_TAB_ANIM_TIME, (object) "name", (object) "scale"));
        SoundManager.Get().LoadAndPlay((AssetReference) "class_tab_click.prefab:d9cb832f0de5c1947a97685e134ba0da", this.gameObject);
      }
      else
      {
        this.transform.localPosition = this.transform.localPosition with
        {
          y = this.m_MobileDeselectedLocalYPos
        };
        iTween.StopByName(this.gameObject, "scale");
        this.transform.localScale = this.m_MobileDeselectedLocalScale;
      }
      this.m_showLargeTab = large;
    }
  }

  public void PlayClickFX() => this.m_clickFXController.SetState("PLAY_CLICK_FX_code");

  public bool IsFinishedLoading() => (bool) (UnityEngine.Object) this.m_roleIconsController && (bool) (UnityEngine.Object) this.m_clickFXController;

  private IEnumerator InitializeWhenReady()
  {
    while (!this.IsFinishedLoading())
      yield return (object) null;
    this.m_roleIconsController.OwningWidget.TriggerEvent(this.m_roleTag.ToString(), new Widget.TriggerEventParameters());
  }
}
