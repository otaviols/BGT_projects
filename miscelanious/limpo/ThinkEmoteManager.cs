using UnityEngine;

public class ThinkEmoteManager : MonoBehaviour
{
  private float m_secondsSinceAction;
  public const float DEFAULT_THINK_EMOTE_DELAY = 20f;
  private static ThinkEmoteManager s_instance;

  private void Awake() => ThinkEmoteManager.s_instance = this;

  private void OnDestroy() => ThinkEmoteManager.s_instance = (ThinkEmoteManager) null;

  public static ThinkEmoteManager Get() => ThinkEmoteManager.s_instance;

  private void Update()
  {
    GameState gameState = GameState.Get();
    if (gameState == null || !gameState.IsMainPhase())
      return;
    float? nullable1 = gameState.GetGameEntity().GetThinkEmoteDelayOverride();
    if (!nullable1.HasValue)
      nullable1 = new float?(20f);
    this.m_secondsSinceAction += Time.deltaTime;
    double secondsSinceAction = (double) this.m_secondsSinceAction;
    float? nullable2 = nullable1;
    double valueOrDefault = (double) nullable2.GetValueOrDefault();
    if (!(secondsSinceAction > valueOrDefault & nullable2.HasValue) || TurnTimer.Get().IsRopeActive() || EndTurnButton.Get().IsInWaitingState() && !GameMgr.Get().IsBattlegrounds())
      return;
    this.PlayThinkEmote();
  }

  private void PlayThinkEmote()
  {
    this.m_secondsSinceAction = 0.0f;
    GameState.Get().GetGameEntity().OnPlayThinkEmote();
  }

  public void NotifyOfActivity() => this.m_secondsSinceAction = 0.0f;
}
