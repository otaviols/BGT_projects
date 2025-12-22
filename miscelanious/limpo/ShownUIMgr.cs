using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;

public class ShownUIMgr : IService
{
  private ShownUIMgr.UI_WINDOW m_shownUI;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    yield break;
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown()
  {
  }

  public static ShownUIMgr Get() => ServiceManager.Get<ShownUIMgr>();

  public void SetShownUI(ShownUIMgr.UI_WINDOW uiWindow) => this.m_shownUI = uiWindow;

  public ShownUIMgr.UI_WINDOW GetShownUI() => this.m_shownUI;

  public void ClearShownUI() => this.m_shownUI = ShownUIMgr.UI_WINDOW.NONE;

  public enum UI_WINDOW
  {
    NONE,
    GENERAL_STORE,
    ARENA_STORE,
    TAVERN_BRAWL_STORE,
    QUEST_LOG,
  }
}
