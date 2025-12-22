using System;
using UnityEngine;

public class PegasusScene : MonoBehaviour
{
  protected object m_sceneTransitionPayload;
  protected string m_sceneName;

  protected virtual void Awake()
  {
    SceneMgr sceneMgr = SceneMgr.Get();
    if (sceneMgr != null)
      sceneMgr.SetScene(this);
    else
      Log.All.PrintWarning("PegasusScene.Awake called when SceneMgr is null!");
  }

  public virtual void PreUnload()
  {
  }

  public virtual bool IsUnloading() => false;

  public virtual void Unload()
  {
  }

  public virtual bool IsTransitioning() => false;

  public virtual bool HandleKeyboardInput()
  {
    if (BackButton.backKey != KeyCode.None && InputCollection.GetKeyUp(BackButton.backKey))
    {
      if (DialogManager.Get().ShowingDialog())
      {
        DialogManager.Get().GoBack();
        return true;
      }
      if (ChatMgr.Get().IsFriendListShowing() || ChatMgr.Get().IsChatLogFrameShown())
      {
        ChatMgr.Get().GoBack();
        return true;
      }
      if ((UnityEngine.Object) OptionsMenu.Get() != (UnityEngine.Object) null && OptionsMenu.Get().IsShown())
      {
        OptionsMenu.Get().Hide();
        return true;
      }
      if ((UnityEngine.Object) MiscellaneousMenu.Get() != (UnityEngine.Object) null && MiscellaneousMenu.Get().IsShown())
      {
        MiscellaneousMenu.Get().Hide();
        return true;
      }
      if ((UnityEngine.Object) BnetBar.Get() != (UnityEngine.Object) null && BnetBar.Get().IsGameMenuShown())
      {
        BnetBar.Get().HideGameMenu();
        return true;
      }
      if (Navigation.GoBack())
        return true;
    }
    return false;
  }

  public virtual void ExecuteSceneDrivenTransition(Action onTransitionCompleteCallback)
  {
    Log.All.PrintError("Scene.ExecuteSceneDrivenTransition - Function was not overridden!");
    onTransitionCompleteCallback();
  }

  public void SetSceneTransitionPayload(object payload) => this.m_sceneTransitionPayload = payload;

  public object GetSceneTransitionPayload() => this.m_sceneTransitionPayload;

  public void SetSceneName(string sceneName) => this.m_sceneName = sceneName;

  public virtual bool IsBlockingPopupDisplayManager() => false;
}
