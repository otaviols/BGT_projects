using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class HeroBuddyWidget : MonoBehaviour
{
  public ProgressBar m_progressBarLeft;
  public ProgressBar m_progressBarRight;
  public MeshRenderer m_portraitMesh;
  public int portraitIndex;
  public UberText m_ProgressTextFriendly;
  public UberText m_ProgressTextEnemy;
  public bool m_alwaysShowProgressText;
  public float m_tilingX = 0.69f;
  public float m_tilingY = 0.69f;
  public float m_offsetX = 0.15f;
  public float m_offsetY = 0.15f;
  private float m_currentProgressValue;
  private UberText m_ProgressText;
  private bool m_warningSent;
  private bool m_showProgressText;
  private bool m_initialized;

  private void Init()
  {
    if (this.m_initialized)
      return;
    GameState gameState = GameState.Get();
    if (gameState == null)
      return;
    Actor component = this.gameObject.GetComponent<Actor>();
    if ((Object) component == (Object) null || component.GetEntity() == null)
      return;
    Entity hero = (component.GetEntity().IsControlledByOpposingSidePlayer() ? (Entity) gameState.GetOpposingSidePlayer() : (Entity) gameState.GetFriendlySidePlayer())?.GetHero();
    int num = 100 * (hero == null ? 0 : hero.GetTag(GAME_TAG.BACON_PLAYER_NUM_HERO_BUDDIES_GAINED)) + (hero == null ? 0 : hero.GetTag(GAME_TAG.BACON_HERO_BUDDY_PROGRESS));
    if (num > 200)
      num = 200;
    this.UpdateProgressBar((float) num / 200f);
    this.m_initialized = true;
  }

  public void ShowProgressText(bool value) => this.m_showProgressText = value;

  public void UpdateProgressBar(float newValue)
  {
    if ((Object) this.m_progressBarLeft == (Object) null || (Object) this.m_progressBarRight == (Object) null)
      return;
    if ((double) newValue < 0.5 || (double) this.m_currentProgressValue < 0.5)
      this.m_progressBarLeft.AnimateProgress(this.m_currentProgressValue, newValue);
    if ((double) newValue > 0.5 || (double) this.m_currentProgressValue > 0.5)
      this.m_progressBarRight.AnimateProgress(this.m_currentProgressValue, newValue);
    this.m_currentProgressValue = newValue;
    if ((Object) this.m_ProgressText == (Object) null)
      this.SetProgressText();
    if (!((Object) this.m_ProgressText != (Object) null))
      return;
    this.m_ProgressText.Text = string.Format("{0:0}%", (object) ((double) this.m_currentProgressValue < 0.5 ? 200.0 * (double) this.m_currentProgressValue : 200.0 * ((double) this.m_currentProgressValue - 0.5)));
  }

  private void LateUpdate()
  {
    this.Init();
    this.UpdatePortrait();
    this.UpdateProgressBarVisibility();
  }

  private void UpdatePortrait()
  {
    Actor component = this.gameObject.GetComponent<Actor>();
    if ((Object) component == (Object) null || !component.IsShown())
      return;
    Player.Side side = Player.Side.FRIENDLY;
    if ((Object) component != (Object) null && component.GetEntity() != null && component.GetEntity().IsControlledByOpposingSidePlayer())
      side = Player.Side.OPPOSING;
    Entity entity = side == Player.Side.FRIENDLY ? GameState.Get().GetFriendlySidePlayer().GetHero() : GameState.Get().GetOpposingSidePlayer().GetHero();
    if (entity == null)
      return;
    int heroBuddyCardId = entity.GetHeroBuddyCardId();
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(heroBuddyCardId))
    {
      if (cardDef == null)
        return;
      Material heroBuddyMaterial = cardDef.CardDef.GetBattlegroundHeroBuddyMaterial();
      if ((Object) heroBuddyMaterial == (Object) null)
      {
        if (!this.m_warningSent)
        {
          Debug.LogWarning((object) "HeroBuddyWidget.UpdatePortrait() - Missing hero buddy Mat");
          this.m_warningSent = true;
        }
        this.m_portraitMesh.GetMaterials()[this.portraitIndex].mainTexture = cardDef.CardDef.GetPortraitTexture(component.GetPremium());
        this.SetupDefaultPortraitMaterial();
      }
      else
      {
        this.m_portraitMesh.GetMaterials()[this.portraitIndex].mainTexture = heroBuddyMaterial.mainTexture;
        this.m_portraitMesh.GetMaterials()[this.portraitIndex].CopyPropertiesFromMaterial(heroBuddyMaterial);
      }
    }
  }

  private void SetupDefaultPortraitMaterial()
  {
    this.m_portraitMesh.GetMaterials()[this.portraitIndex].SetTextureOffset("_MainTex", new Vector2(this.m_offsetX, this.m_offsetY));
    this.m_portraitMesh.GetMaterials()[this.portraitIndex].SetTextureScale("_MainTex", new Vector2(this.m_tilingX, this.m_tilingY));
  }

  private void SetProgressText()
  {
    Actor component = this.gameObject.GetComponent<Actor>();
    Player.Side side = Player.Side.FRIENDLY;
    if ((Object) component != (Object) null && component.GetEntity() != null && component.GetEntity().IsControlledByOpposingSidePlayer())
      side = Player.Side.OPPOSING;
    this.m_ProgressText = side == Player.Side.FRIENDLY ? this.m_ProgressTextFriendly : this.m_ProgressTextEnemy;
  }

  private void UpdateProgressBarVisibility()
  {
    this.SetProgressText();
    if ((Object) this.m_ProgressText != (Object) null)
      this.m_ProgressText.gameObject.SetActive(false);
    this.m_ProgressText.gameObject.SetActive(this.m_alwaysShowProgressText || this.m_showProgressText);
  }
}
