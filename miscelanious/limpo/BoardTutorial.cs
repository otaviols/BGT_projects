using UnityEngine;

public class BoardTutorial : MonoBehaviour
{
  public GameObject m_Highlight;
  public GameObject m_EnemyHighlight;
  public Light m_ManaSpotlight;
  private static BoardTutorial s_instance;
  private bool m_highlightEnabled;
  private bool m_enemyHighlightEnabled;

  private void Awake()
  {
    BoardTutorial.s_instance = this;
    RenderUtils.EnableRenderers(this.m_Highlight, false);
    RenderUtils.EnableRenderers(this.m_EnemyHighlight, false);
    if (!((Object) LoadingScreen.Get() != (Object) null))
      return;
    LoadingScreen.Get().NotifyMainSceneObjectAwoke(this.gameObject);
  }

  private void OnDestroy() => BoardTutorial.s_instance = (BoardTutorial) null;

  public static BoardTutorial Get() => BoardTutorial.s_instance;

  public void EnableHighlight(bool enable)
  {
    if (this.m_highlightEnabled == enable)
      return;
    this.m_highlightEnabled = enable;
    this.UpdateHighlight();
  }

  public void EnableEnemyHighlight(bool enable)
  {
    if (this.m_enemyHighlightEnabled == enable)
      return;
    this.m_enemyHighlightEnabled = enable;
    this.UpdateEnemyHighlight();
  }

  public void EnableFullHighlight(bool enable)
  {
    this.EnableHighlight(enable);
    this.EnableEnemyHighlight(enable);
  }

  public bool IsHighlightEnabled() => this.m_highlightEnabled;

  private void UpdateHighlight()
  {
    if (this.m_highlightEnabled)
    {
      RenderUtils.EnableRenderers(this.m_Highlight, this.m_highlightEnabled);
      this.m_Highlight.GetComponent<Animation>().Play("Glow_PlayArea_Player_On");
    }
    else
      this.m_Highlight.GetComponent<Animation>().Play("Glow_PlayArea_Player_Off");
  }

  private void UpdateEnemyHighlight()
  {
    if (this.m_enemyHighlightEnabled)
    {
      RenderUtils.EnableRenderers(this.m_EnemyHighlight, this.m_enemyHighlightEnabled);
      this.m_EnemyHighlight.GetComponent<Animation>().Play("Glow_PlayArea_Player_On");
    }
    else
      this.m_EnemyHighlight.GetComponent<Animation>().Play("Glow_PlayArea_Player_Off");
  }
}
