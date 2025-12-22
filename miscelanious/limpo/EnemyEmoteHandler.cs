using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using System.Linq;
using UnityEngine;

public class EnemyEmoteHandler : MonoBehaviour
{
  public GameObject m_SquelchEmote;
  public MeshRenderer m_SquelchEmoteBackplate;
  public UberText m_SquelchEmoteText;
  public string m_SquelchStringTag;
  public string m_UnsquelchStringTag;
  [SerializeField]
  private string m_squelchAllStringTag;
  [SerializeField]
  private string m_unsquelchAllStringTag;
  private static EnemyEmoteHandler s_instance;
  private Vector3 m_squelchEmoteStartingScale;
  private bool m_emotesShown;
  private int m_shownAtFrame;
  private bool m_isInBattlegrounds;
  private bool m_squelchMousedOver;
  private Map<int, bool> m_squelched;
  private const int PLAYERS_IN_BATTLEGROUNDS = 8;

  private void Awake()
  {
    EnemyEmoteHandler.s_instance = this;
    this.GetComponent<Collider>().enabled = false;
    this.m_squelchEmoteStartingScale = this.m_SquelchEmote.transform.localScale;
    this.m_squelched = new Map<int, bool>(8);
    for (int index = 0; index < 8; ++index)
      this.m_squelched.Add(index + 1, false);
    this.m_SquelchEmoteText.gameObject.SetActive(false);
    this.m_SquelchEmoteBackplate.enabled = false;
    this.m_SquelchEmote.transform.localScale = Vector3.zero;
  }

  private void Start() => this.m_isInBattlegrounds = GameMgr.Get().IsBattlegrounds();

  private void OnDestroy() => EnemyEmoteHandler.s_instance = (EnemyEmoteHandler) null;

  public static EnemyEmoteHandler Get() => EnemyEmoteHandler.s_instance;

  public bool AreEmotesActive() => this.m_emotesShown;

  public bool IsSquelched(int playerId) => this.m_squelched.ContainsKey(playerId) && this.m_squelched[playerId];

  private bool AnySquelched()
  {
    foreach (bool flag in this.m_squelched.Values)
    {
      if (flag)
        return true;
    }
    return false;
  }

  public void ShowEmotes()
  {
    if (this.m_emotesShown)
      return;
    this.m_emotesShown = true;
    this.GetComponent<Collider>().enabled = true;
    this.m_shownAtFrame = Time.frameCount;
    this.m_SquelchEmoteText.Text = !this.AnySquelched() ? GameStrings.Get(this.m_isInBattlegrounds ? this.m_squelchAllStringTag : this.m_SquelchStringTag) : GameStrings.Get(this.m_isInBattlegrounds ? this.m_unsquelchAllStringTag : this.m_UnsquelchStringTag);
    this.m_SquelchEmoteBackplate.enabled = true;
    this.m_SquelchEmoteText.gameObject.SetActive(true);
    this.m_SquelchEmote.GetComponent<Collider>().enabled = true;
    iTween.Stop(this.m_SquelchEmote);
    iTween.ScaleTo(this.m_SquelchEmote, iTween.Hash((object) "scale", (object) this.m_squelchEmoteStartingScale, (object) "time", (object) 0.5f, (object) "ignoretimescale", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
  }

  public void HideEmotes()
  {
    if (!this.m_emotesShown)
      return;
    this.m_emotesShown = false;
    this.GetComponent<Collider>().enabled = false;
    this.m_SquelchEmote.GetComponent<Collider>().enabled = false;
    iTween.Stop(this.m_SquelchEmote);
    iTween.ScaleTo(this.m_SquelchEmote, iTween.Hash((object) "scale", (object) Vector3.zero, (object) "time", (object) 0.1f, (object) "ignoretimescale", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "FinishDisable"));
  }

  public void HandleInput()
  {
    RaycastHit hitInfo;
    if (!this.HitTestEmotes(out hitInfo))
    {
      this.HideEmotes();
    }
    else
    {
      if ((Object) hitInfo.transform.gameObject != (Object) this.m_SquelchEmote)
      {
        if (this.m_squelchMousedOver)
        {
          this.MouseOutSquelch();
          this.m_squelchMousedOver = false;
        }
      }
      else if (!this.m_squelchMousedOver)
      {
        this.m_squelchMousedOver = true;
        this.MouseOverSquelch();
      }
      if (!InputCollection.GetMouseButtonUp(0))
        return;
      if (this.m_squelchMousedOver)
      {
        this.DoSquelchClick();
      }
      else
      {
        if (!UniversalInputManager.Get().IsTouchMode() || Time.frameCount == this.m_shownAtFrame)
          return;
        this.HideEmotes();
      }
    }
  }

  public bool IsMouseOverEmoteOption()
  {
    RaycastHit hitInfo;
    return UniversalInputManager.Get().GetInputHitInfo((LayerMask) GameLayer.Default.LayerBit(), out hitInfo) && (Object) hitInfo.transform.gameObject == (Object) this.m_SquelchEmote;
  }

  private void MouseOverSquelch() => iTween.ScaleTo(this.m_SquelchEmote, iTween.Hash((object) "scale", (object) (this.m_squelchEmoteStartingScale * 1.1f), (object) "time", (object) 0.2f, (object) "ignoretimescale", (object) true));

  private void MouseOutSquelch() => iTween.ScaleTo(this.m_SquelchEmote, iTween.Hash((object) "scale", (object) this.m_squelchEmoteStartingScale, (object) "time", (object) 0.2f, (object) "ignoretimescale", (object) true));

  private void DoSquelchClick()
  {
    if (this.m_isInBattlegrounds)
    {
      int playerId = GameState.Get().GetCurrentPlayer().GetPlayerId();
      foreach (int key in this.m_squelched.Keys.ToList<int>())
      {
        if (key != playerId)
          this.m_squelched[key] = !this.m_squelched[key];
      }
    }
    else
      this.m_squelched[GameState.Get().GetOpposingPlayerId()] = !this.m_squelched[GameState.Get().GetOpposingPlayerId()];
    this.HideEmotes();
  }

  private bool HitTestEmotes(out RaycastHit hitInfo) => UniversalInputManager.Get().GetInputHitInfo((LayerMask) GameLayer.CardRaycast.LayerBit(), out hitInfo) && (this.IsMousedOverHero(hitInfo) || this.IsMousedOverSelf(hitInfo) || this.IsMousedOverEmote(hitInfo));

  private bool IsMousedOverHero(RaycastHit cardHitInfo)
  {
    Actor componentInParents = GameObjectUtils.FindComponentInParents<Actor>((Component) cardHitInfo.transform);
    if ((Object) componentInParents == (Object) null)
      return false;
    Card card = componentInParents.GetCard();
    return !((Object) card == (Object) null) && card.GetEntity().IsHero();
  }

  private bool IsMousedOverSelf(RaycastHit cardHitInfo) => (Object) this.GetComponent<Collider>() == (Object) cardHitInfo.collider;

  private bool IsMousedOverEmote(RaycastHit cardHitInfo) => (Object) cardHitInfo.transform == (Object) this.m_SquelchEmote.transform;
}
