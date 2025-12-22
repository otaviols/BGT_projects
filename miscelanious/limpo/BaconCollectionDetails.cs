using Hearthstone;
using Hearthstone.UI;
using System;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public abstract class BaconCollectionDetails : MonoBehaviour
{
  [SerializeField]
  private float m_animationTime = 0.15f;
  [SerializeField]
  private UberText m_DebugText;
  [SerializeField]
  protected Widget m_widget;
  protected bool m_animating;
  protected bool m_isShown;
  protected Vector3 m_originalScale;
  private ScreenEffectsHandle m_screenEffectsHandle;

  protected abstract bool ValidateDataModels(IDataModel dataModel, IDataModel pageDataModel);

  public abstract void AssignDataModels(IDataModel dataModel, IDataModel pageDataModel);

  protected abstract void ClearDataModels();

  protected abstract void DetailsEventListener(string eventName);

  protected abstract string DebugTextValue { get; }

  protected virtual void Start()
  {
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
    {
      Debug.LogError((object) (((object) this).GetType().Name + ": No widget found, will not be able to show."));
    }
    else
    {
      this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.DetailsEventListener));
      this.gameObject.SetActive(false);
      this.m_originalScale = this.transform.localScale;
      this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
    }
  }

  public bool CanShow(IDataModel dataModel, IDataModel pageDataModel)
  {
    if (this.m_animating || this.m_isShown)
      return false;
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
    {
      Debug.LogError((object) (((object) this).GetType().Name + ": No widget assigned, cannot show."));
      return false;
    }
    if (this.ValidateDataModels(dataModel, pageDataModel))
      return true;
    Debug.LogError((object) (((object) this).GetType().Name + ": Invalid data models assigned, cannot show"));
    return false;
  }

  public virtual void Show()
  {
    this.m_isShown = true;
    if (CollectionManager.Get() != null && (UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() != (UnityEngine.Object) null)
    {
      CollectiblePageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager();
      if ((UnityEngine.Object) pageManager != (UnityEngine.Object) null)
      {
        pageManager.EnablePageTurn(false);
        pageManager.EnablePageTurnArrows(false);
      }
    }
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
    {
      Time = this.m_animationTime
    });
    this.gameObject.SetActive(true);
    this.m_animating = true;
    this.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) this.m_originalScale, (object) "time", (object) this.m_animationTime, (object) "easeType", (object) iTween.EaseType.easeOutCirc, (object) "oncomplete", (object) new Action<object>(this.OnShowAnimationComplete)));
    this.ShowDebugText();
  }

  public bool CanHide() => !this.m_animating && this.m_isShown;

  public virtual void Hide()
  {
    this.m_isShown = false;
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager != null && (UnityEngine.Object) collectionManager.GetCollectibleDisplay() != (UnityEngine.Object) null)
    {
      CollectiblePageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager();
      if ((UnityEngine.Object) pageManager != (UnityEngine.Object) null)
      {
        pageManager.EnablePageTurn(true);
        pageManager.EnablePageTurnArrows(true);
      }
    }
    this.m_screenEffectsHandle.StopEffect();
    this.m_animating = true;
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) this.m_animationTime, (object) "easeType", (object) iTween.EaseType.easeOutCirc, (object) "oncomplete", (object) new Action<object>(this.OnHideAnimationComplete)));
    this.ClearDataModels();
    LuckyDrawManager.Get()?.UpdateAllRewardsOwnedStatus();
  }

  public void Unload()
  {
    if (!((UnityEngine.Object) this.m_widget != (UnityEngine.Object) null))
      return;
    this.m_widget.RemoveEventListener(new Widget.EventListenerDelegate(this.DetailsEventListener));
  }

  protected virtual void OnShowAnimationComplete(object objectData) => this.m_animating = false;

  protected virtual void OnHideAnimationComplete(object objectData)
  {
    this.m_animating = false;
    if (!(bool) (UnityEngine.Object) this.gameObject || this.m_isShown)
      return;
    this.transform.localScale = this.m_originalScale;
    this.gameObject.SetActive(false);
  }

  private void ShowDebugText()
  {
    if ((UnityEngine.Object) this.m_DebugText == (UnityEngine.Object) null)
      return;
    if (HearthstoneApplication.IsInternal() && Options.Get().GetBool(Option.DEBUG_SHOW_BATTLEGROUND_SKIN_IDS))
    {
      this.m_DebugText.Text = this.DebugTextValue;
      this.m_DebugText.gameObject.SetActive(true);
    }
    else
    {
      this.m_DebugText.Text = string.Empty;
      this.m_DebugText.gameObject.SetActive(false);
    }
  }
}
