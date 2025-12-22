using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

[CustomEditClass]
public class EmoteOption : MonoBehaviour
{
  public EmoteType m_EmoteType;
  public string m_StringTag;
  public EmoteType m_FallbackEmoteType;
  public string m_FallbackStringTag;
  public MeshRenderer m_Backplate;
  public UberText m_Text;
  public GameObject m_VisualEmoteImage;
  [CustomEditField(T = EditType.SPELL)]
  public string m_SpeechBubbleSpellPath;
  public bool m_RenderSpellOnSpeechBubbleLayer = true;
  private EmoteType m_currentEmoteType;
  private string m_currentStringTag;
  private Vector3 m_startingScale;
  private bool m_textIsGrey;
  private Spell m_speechBubbleSpell;

  private void Awake()
  {
    this.UpdateEmoteType();
    if ((Object) this.m_Text != (Object) null)
      this.m_Text.gameObject.SetActive(false);
    if ((Object) this.m_Backplate != (Object) null)
      this.m_Backplate.enabled = false;
    if ((Object) this.m_VisualEmoteImage != (Object) null)
      this.m_VisualEmoteImage.SetActive(false);
    if (!string.IsNullOrEmpty(this.m_SpeechBubbleSpellPath))
    {
      this.m_speechBubbleSpell = SpellManager.Get().GetSpell(this.m_SpeechBubbleSpellPath);
      if ((Object) this.m_speechBubbleSpell == (Object) null)
        Error.AddDevFatalUnlessWorkarounds("EmoteOption.Awake() - \"{0}\" does not have a Spell component.", (object) this.m_SpeechBubbleSpellPath);
      SpellUtils.SetupSpell(this.m_speechBubbleSpell, (Component) this);
      if ((Object) this.m_speechBubbleSpell != (Object) null && (Object) this.m_Backplate != (Object) null && this.m_RenderSpellOnSpeechBubbleLayer)
      {
        Renderer component1 = this.m_Backplate.GetComponent<Renderer>();
        if ((Object) component1 != (Object) null)
        {
          int layer = this.m_Backplate.gameObject.layer;
          SetRenderQue component2 = this.m_Backplate.GetComponent<SetRenderQue>();
          int num = (Object) component2 != (Object) null ? component2.queue : 0;
          int renderQueue = component1.GetMaterial().renderQueue + num;
          LayerUtils.SetLayer((Component) this.m_speechBubbleSpell, layer);
          RenderUtils.SetRenderQueue(this.m_speechBubbleSpell.gameObject, renderQueue, true);
        }
      }
    }
    this.m_startingScale = this.transform.localScale;
    this.transform.localScale = Vector3.zero;
  }

  private void Update()
  {
    if ((Object) this.m_Text == (Object) null && (Object) this.m_VisualEmoteImage == (Object) null)
      return;
    if (EmoteHandler.Get().EmoteSpamBlocked())
    {
      if (this.m_textIsGrey)
        return;
      this.m_textIsGrey = true;
      if ((Object) this.m_Text != (Object) null)
        this.m_Text.TextColor = new Color(0.5372549f, 0.5372549f, 0.5372549f);
      if (!((Object) this.m_VisualEmoteImage != (Object) null))
        return;
      this.m_VisualEmoteImage.GetComponent<Renderer>().GetMaterial().color = new Color(1f, 1f, 1f, 0.5f);
    }
    else
    {
      if (!this.m_textIsGrey)
        return;
      this.m_textIsGrey = false;
      if ((Object) this.m_Text != (Object) null)
        this.m_Text.TextColor = new Color(0.0f, 0.0f, 0.0f);
      if (!((Object) this.m_VisualEmoteImage != (Object) null))
        return;
      this.m_VisualEmoteImage.GetComponent<Renderer>().GetMaterial().color = new Color(1f, 1f, 1f, 1f);
    }
  }

  public void DoClick()
  {
    EmoteHandler.Get().ResetTimeSinceLastEmote();
    EmoteType emoteType = this.m_currentEmoteType;
    EmoteType emoteResponseType = EmoteHandler.Get().GetEmoteResponseType(this.m_currentEmoteType);
    Card heroCard = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
    if (EmoteHandler.Get().ShouldUseEmoteResponse(this.m_currentEmoteType, Player.Side.FRIENDLY) && heroCard.GetEmoteEntry(emoteResponseType) != null)
      emoteType = emoteResponseType;
    Notification.SpeechBubbleDirection directionOverride = GameState.Get().GetGameEntity().GetEmoteDirectionOverride(emoteType);
    heroCard.PlayEmote(emoteType, directionOverride);
    Network.Get().SendEmote(emoteType);
    EmoteHandler.Get().HideEmotes();
  }

  public void Enable()
  {
    this.m_Backplate.enabled = true;
    if ((Object) this.m_Text != (Object) null)
      this.m_Text.gameObject.SetActive(true);
    if ((Object) this.m_VisualEmoteImage != (Object) null)
      this.m_VisualEmoteImage.gameObject.SetActive(true);
    this.GetComponent<Collider>().enabled = true;
    iTween.Stop(this.gameObject);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) this.m_startingScale, (object) "time", (object) 0.5f, (object) "ignoretimescale", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
    if (!((Object) this.m_speechBubbleSpell != (Object) null))
      return;
    TransformUtil.CopyWorld((Component) this.m_speechBubbleSpell, this.gameObject);
    this.m_speechBubbleSpell.transform.localScale = Vector3.one;
    this.m_speechBubbleSpell.ActivateState(SpellStateType.BIRTH);
  }

  public void Disable()
  {
    this.GetComponent<Collider>().enabled = false;
    iTween.Stop(this.gameObject);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) Vector3.zero, (object) "time", (object) 0.1f, (object) "ignoretimescale", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "FinishDisable"));
    if (!((Object) this.m_speechBubbleSpell != (Object) null))
      return;
    this.m_speechBubbleSpell.ActivateState(SpellStateType.DEATH);
  }

  public void HandleMouseOut() => iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) this.m_startingScale, (object) "time", (object) 0.2f, (object) "ignoretimescale", (object) true));

  public void HandleMouseOver() => iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) (this.m_startingScale * 1.1f), (object) "time", (object) 0.2f, (object) "ignoretimescale", (object) true));

  public void UpdateEmoteType()
  {
    Player friendlySidePlayer = GameState.Get()?.GetFriendlySidePlayer();
    if (friendlySidePlayer != null && this.ShouldUseFallbackEmote(friendlySidePlayer))
    {
      this.m_currentEmoteType = this.m_FallbackEmoteType;
      this.m_currentStringTag = this.m_FallbackStringTag;
    }
    else
    {
      this.m_currentEmoteType = this.m_EmoteType;
      this.m_currentStringTag = this.m_StringTag;
    }
    if (!((Object) this.m_Text != (Object) null))
      return;
    this.m_Text.Text = GameStrings.Get(this.m_currentStringTag);
  }

  public bool ShouldPlayerUseEmoteOverride(Player player)
  {
    if (player == null)
      return false;
    Card heroCard = player.GetHeroCard();
    return !((Object) heroCard == (Object) null) && heroCard.GetEmoteEntry(this.m_EmoteType) != null;
  }

  public bool CanPlayerUseEmoteType(Player player)
  {
    if (player == null)
      return false;
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.USES_PREMIUM_EMOTES))
      return true;
    Card heroCard = player.GetHeroCard();
    return !((Object) heroCard == (Object) null) && (heroCard.GetEmoteEntry(this.m_EmoteType) != null || heroCard.GetEmoteEntry(this.m_FallbackEmoteType) != null);
  }

  public bool HasEmoteTypeForPlayer(EmoteType emoteType, Player player) => this.ShouldUseFallbackEmote(player) ? emoteType == this.m_FallbackEmoteType : emoteType == this.m_EmoteType;

  private bool ShouldUseFallbackEmote(Player player)
  {
    if (player == null)
      return false;
    Card heroCard = player.GetHeroCard();
    return !((Object) heroCard == (Object) null) && heroCard.GetEmoteEntry(this.m_EmoteType) == null && heroCard.GetEmoteEntry(this.m_FallbackEmoteType) != null;
  }

  private void FinishDisable()
  {
    if (this.GetComponent<Collider>().enabled)
      return;
    this.m_Backplate.enabled = false;
    if (!((Object) this.m_Text != (Object) null))
      return;
    this.m_Text.gameObject.SetActive(false);
  }
}
