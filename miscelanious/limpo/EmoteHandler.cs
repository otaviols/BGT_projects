using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmoteHandler : MonoBehaviour
{
  public List<EmoteOption> m_DefaultEmotes;
  public List<EmoteOption> m_EmoteOverrides;
  public List<EmoteOption> m_HiddenEmotes;
  private List<EmoteOption> m_availableEmotes;
  private const float MIN_TIME_BETWEEN_EMOTES = 4f;
  private const float TIME_WINDOW_TO_BE_CONSIDERED_A_CHAIN = 5f;
  private const float SPAMMER_MIN_TIME_BETWEEN_EMOTES = 15f;
  private const float UBER_SPAMMER_MIN_TIME_BETWEEN_EMOTES = 45f;
  private const int NUM_EMOTES_BEFORE_CONSIDERED_A_SPAMMER = 20;
  private const int NUM_EMOTES_BEFORE_CONSIDERED_UBER_SPAMMER = 25;
  private const int NUM_CHAIN_EMOTES_BEFORE_CONSIDERED_SPAM = 2;
  private const int EMOTE_COUNT = 6;
  private const float MAX_TIME_FOR_EMOTE_RESPONSE = 6f;
  private const float EMOTE_RESPONSE_SERVER_DELAY_SLUSH = 2f;
  private const float DEFAULT_STARTING_TAUNT_DURATION = 2.5f;
  private static EmoteHandler s_instance;
  private bool m_emotesShown;
  private int m_shownAtFrame;
  private EmoteOption m_mousedOverEmote;
  private float m_timeSinceLastEmote = 4f;
  private int m_totalEmotes;
  private int m_chainedEmotes;
  private Map<EmoteType, float> m_timeSinceEmoteFinishedOpposing = new Map<EmoteType, float>();
  private Map<EmoteType, float> m_timeSinceEmoteFinishedFriendly = new Map<EmoteType, float>();
  private Map<EmoteType, float>.KeyCollection m_keyCollectionOpposing;
  private Map<EmoteType, float>.KeyCollection m_keyCollectionFriendly;
  private List<EmoteType> m_keyType = new List<EmoteType>();

  private void Awake()
  {
    EmoteHandler.s_instance = this;
    this.GetComponent<Collider>().enabled = false;
  }

  private void Start()
  {
    GameState.Get().RegisterHeroChangedListener(new GameState.HeroChangedCallback(this.OnHeroChanged));
    this.DetermineAvailableEmotes();
    this.m_keyCollectionOpposing = this.m_timeSinceEmoteFinishedOpposing.Keys;
    this.m_keyCollectionFriendly = this.m_timeSinceEmoteFinishedFriendly.Keys;
  }

  private void DetermineAvailableEmotes()
  {
    this.m_availableEmotes = new List<EmoteOption>();
    foreach (EmoteOption defaultEmote in this.m_DefaultEmotes)
    {
      this.m_availableEmotes.Add(defaultEmote);
      defaultEmote.gameObject.SetActive(true);
    }
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    for (int index = 0; index < 6; ++index)
    {
      int tag = friendlySidePlayer.GetTag((GAME_TAG) (740 + index));
      if (tag > 0 && tag < this.m_EmoteOverrides.Count && (this.m_EmoteOverrides[tag].ShouldPlayerUseEmoteOverride(friendlySidePlayer) || GameState.Get().GetBooleanGameOption(GameEntityOption.USES_PREMIUM_EMOTES)))
      {
        this.m_availableEmotes[index].gameObject.SetActive(false);
        this.m_availableEmotes[index] = this.m_EmoteOverrides[tag];
        TransformUtil.CopyWorld((Component) this.m_availableEmotes[index], (Component) this.m_DefaultEmotes[index]);
        this.m_availableEmotes[index].gameObject.SetActive(true);
      }
    }
  }

  private void OnDestroy() => EmoteHandler.s_instance = (EmoteHandler) null;

  private void Update()
  {
    if (GameState.Get() == null)
      return;
    this.m_timeSinceLastEmote += Time.unscaledDeltaTime;
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    if (opposingSidePlayer == null)
      return;
    Card heroCard = opposingSidePlayer.GetHeroCard();
    if ((Object) heroCard == (Object) null)
      return;
    this.UpdateTimeSinceEmoteFinished(heroCard, this.m_timeSinceEmoteFinishedOpposing, this.m_keyCollectionOpposing);
    this.UpdateTimeSinceEmoteFinished(GameState.Get().GetFriendlySidePlayer().GetHeroCard(), this.m_timeSinceEmoteFinishedFriendly, this.m_keyCollectionFriendly);
  }

  public void UpdateTimeSinceEmoteFinished(
    Card heroCard,
    Map<EmoteType, float> timeSinceEmoteFinished,
    Map<EmoteType, float>.KeyCollection keyCollection)
  {
    if ((Object) heroCard == (Object) null)
      return;
    EmoteEntry activeEmoteSound = heroCard.GetActiveEmoteSound();
    if (activeEmoteSound != null)
      timeSinceEmoteFinished[activeEmoteSound.GetEmoteType()] = 0.0f;
    this.m_keyType.Clear();
    this.m_keyType.AddRange((IEnumerable<EmoteType>) keyCollection);
    foreach (EmoteType key in this.m_keyType)
    {
      if (activeEmoteSound != null && key == activeEmoteSound.GetEmoteType())
      {
        timeSinceEmoteFinished[key] = 0.0f;
      }
      else
      {
        float num = timeSinceEmoteFinished[key] + Time.unscaledDeltaTime;
        timeSinceEmoteFinished[key] = num;
        if ((double) num > 8.0)
          timeSinceEmoteFinished.Remove(key);
      }
    }
  }

  public static EmoteHandler Get() => EmoteHandler.s_instance;

  public void ChangeAvailableEmotes()
  {
    this.HideEmotes();
    this.DetermineAvailableEmotes();
  }

  public void ResetTimeSinceLastEmote()
  {
    if ((double) this.m_timeSinceLastEmote < 9.0)
      ++this.m_chainedEmotes;
    else
      this.m_chainedEmotes = 0;
    this.m_timeSinceLastEmote = 0.0f;
  }

  public bool IsResponseEmote(EmoteType type) => type == EmoteType.MIRROR_GREETINGS;

  public bool ShouldUseEmoteResponse(EmoteType desiredEmoteType, Player.Side heroSide)
  {
    Card heroCard1 = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
    Card heroCard2 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
    int num1 = (int) desiredEmoteType;
    EmoteEntry emoteEntry1 = heroCard1.GetEmoteEntry((EmoteType) num1);
    if (emoteEntry1 != null && !string.IsNullOrEmpty(emoteEntry1.GetGameStringKey()))
    {
      EmoteEntry emoteEntry2 = heroCard2.GetEmoteEntry(desiredEmoteType);
      if (emoteEntry2 != null && emoteEntry2.GetGameStringKey() != emoteEntry1.GetGameStringKey())
        return false;
    }
    float num2 = 0.0f;
    float num3 = 6f;
    if (heroSide == Player.Side.FRIENDLY)
    {
      if (!this.m_timeSinceEmoteFinishedOpposing.TryGetValue(desiredEmoteType, out num2))
        return false;
    }
    else
    {
      if (!this.m_timeSinceEmoteFinishedFriendly.TryGetValue(desiredEmoteType, out num2))
        return false;
      num3 += 2f;
    }
    return (double) num2 <= (double) num3;
  }

  public EmoteType GetEmoteResponseType(EmoteType desiredEmoteType) => desiredEmoteType == EmoteType.GREETINGS ? EmoteType.MIRROR_GREETINGS : EmoteType.INVALID;

  public EmoteType GetEmoteAntiResponseType(EmoteType desiredEmoteType) => desiredEmoteType == EmoteType.MIRROR_GREETINGS ? EmoteType.GREETINGS : EmoteType.INVALID;

  public void ShowEmotes()
  {
    if (this.m_emotesShown || GameState.Get().IsBusy())
      return;
    this.m_shownAtFrame = Time.frameCount;
    this.m_emotesShown = true;
    this.GetComponent<Collider>().enabled = true;
    foreach (EmoteOption availableEmote in this.m_availableEmotes)
      availableEmote.Enable();
  }

  public void HideEmotes()
  {
    if (!this.m_emotesShown)
      return;
    this.m_mousedOverEmote = (EmoteOption) null;
    this.m_emotesShown = false;
    this.GetComponent<Collider>().enabled = false;
    foreach (EmoteOption availableEmote in this.m_availableEmotes)
      availableEmote.Disable();
  }

  public bool AreEmotesActive() => this.m_emotesShown;

  public void HandleInput()
  {
    RaycastHit hitInfo;
    if (!this.HitTestEmotes(out hitInfo))
      this.HideEmotes();
    else if (GameState.Get().IsBusy())
    {
      this.HideEmotes();
    }
    else
    {
      EmoteOption component = hitInfo.transform.gameObject.GetComponent<EmoteOption>();
      if ((Object) component == (Object) null)
      {
        if ((Object) this.m_mousedOverEmote != (Object) null)
        {
          this.m_mousedOverEmote.HandleMouseOut();
          this.m_mousedOverEmote = (EmoteOption) null;
        }
      }
      else if ((Object) this.m_mousedOverEmote == (Object) null)
      {
        this.m_mousedOverEmote = component;
        this.m_mousedOverEmote.HandleMouseOver();
      }
      else if ((Object) this.m_mousedOverEmote != (Object) component)
      {
        this.m_mousedOverEmote.HandleMouseOut();
        this.m_mousedOverEmote = component;
        component.HandleMouseOver();
      }
      if (!InputCollection.GetMouseButtonUp(0))
        return;
      if ((Object) this.m_mousedOverEmote != (Object) null)
      {
        if (this.EmoteSpamBlocked())
          return;
        ++this.m_totalEmotes;
        if (GameState.Get().GetGameEntity().HasTag(GAME_TAG.ALL_TARGETS_RANDOM))
        {
          List<EmoteOption> emoteOptionList = new List<EmoteOption>();
          foreach (EmoteOption emoteOption in this.m_availableEmotes.Concat<EmoteOption>((IEnumerable<EmoteOption>) this.m_HiddenEmotes))
          {
            if (emoteOption.CanPlayerUseEmoteType(GameState.Get().GetFriendlySidePlayer()))
              emoteOptionList.Add(emoteOption);
          }
          if (emoteOptionList.Count > 0)
          {
            int index = Random.Range(0, emoteOptionList.Count);
            emoteOptionList[index].DoClick();
          }
          else
            Log.All.PrintError("EmoteHandler.HandleInput() - No usable emotes exist.");
        }
        else
          this.m_mousedOverEmote.DoClick();
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
    return UniversalInputManager.Get().GetInputHitInfo((LayerMask) GameLayer.Default.LayerBit(), out hitInfo) && (Object) hitInfo.transform.gameObject.GetComponent<EmoteOption>() != (Object) null;
  }

  private bool IsVisualEmoteUnfinished()
  {
    if (GameState.Get() == null)
      return false;
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    if (friendlySidePlayer == null)
      return false;
    Card heroCard = friendlySidePlayer.GetHeroCard();
    return (Object) heroCard != (Object) null && heroCard.HasUnfinishedEmoteSpell();
  }

  public bool EmoteSpamBlocked()
  {
    if (this.IsVisualEmoteUnfinished())
      return true;
    if (GameMgr.Get().IsFriendly() || GameMgr.Get().IsAI())
      return false;
    if (this.m_totalEmotes >= 25)
      return (double) this.m_timeSinceLastEmote < 45.0;
    return this.m_totalEmotes >= 20 || this.m_chainedEmotes >= 2 ? (double) this.m_timeSinceLastEmote < 15.0 : (double) this.m_timeSinceLastEmote < 4.0;
  }

  public bool IsValidEmoteTypeForOpponent(EmoteType emoteType)
  {
    List<EmoteOption> emoteOptionList = new List<EmoteOption>();
    foreach (EmoteOption defaultEmote in this.m_DefaultEmotes)
      emoteOptionList.Add(defaultEmote);
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    for (int index = 0; index < 6; ++index)
    {
      int tag = opposingSidePlayer.GetTag((GAME_TAG) (740 + index));
      if (tag > 0 && tag < this.m_EmoteOverrides.Count && this.m_EmoteOverrides[tag].CanPlayerUseEmoteType(opposingSidePlayer))
        emoteOptionList[index] = this.m_EmoteOverrides[tag];
    }
    foreach (EmoteOption emoteOption in emoteOptionList)
    {
      if (emoteOption.HasEmoteTypeForPlayer(emoteType, opposingSidePlayer))
        return true;
    }
    if (GameState.Get().GetGameEntity().HasTag(GAME_TAG.ALL_TARGETS_RANDOM))
    {
      foreach (EmoteOption hiddenEmote in this.m_HiddenEmotes)
      {
        if (hiddenEmote.HasEmoteTypeForPlayer(emoteType, opposingSidePlayer))
          return true;
      }
    }
    if (this.IsResponseEmote(emoteType))
    {
      EmoteType antiResponseType = this.GetEmoteAntiResponseType(emoteType);
      if (this.ShouldUseEmoteResponse(antiResponseType, Player.Side.OPPOSING))
        return this.IsValidEmoteTypeForOpponent(antiResponseType);
    }
    return false;
  }

  private void OnHeroChanged(Player player, object userData)
  {
    if (!player.IsFriendlySide())
      return;
    this.DetermineAvailableEmotes();
    foreach (EmoteOption availableEmote in this.m_availableEmotes)
      availableEmote.UpdateEmoteType();
  }

  private bool HitTestEmotes(out RaycastHit hitInfo) => UniversalInputManager.Get().GetInputHitInfo((LayerMask) GameLayer.CardRaycast.LayerBit(), out hitInfo) && (this.IsMousedOverHero(hitInfo) || this.IsMousedOverSelf(hitInfo) || this.IsMousedOverEmote(hitInfo));

  private bool IsMousedOverHero(RaycastHit cardHitInfo)
  {
    Actor componentInParents = GameObjectUtils.FindComponentInParents<Actor>((Component) cardHitInfo.transform);
    if ((Object) componentInParents == (Object) null)
      return false;
    Card card = componentInParents.GetCard();
    return !((Object) card == (Object) null) && card.GetEntity().IsHero() && card.GetZone() is ZoneHero;
  }

  private bool IsMousedOverSelf(RaycastHit cardHitInfo) => (Object) this.GetComponent<Collider>() == (Object) cardHitInfo.collider;

  private bool IsMousedOverEmote(RaycastHit cardHitInfo)
  {
    foreach (EmoteOption availableEmote in this.m_availableEmotes)
    {
      if ((Object) cardHitInfo.transform == (Object) availableEmote.transform)
        return true;
    }
    return false;
  }

  public IEnumerator PlayStartingTaunts(GameObject mulliganGO)
  {
    Card heroCard = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
    Card heroPowerCard = GameState.Get().GetOpposingSidePlayer().GetHeroPowerCard();
    iTween.StopByName(mulliganGO, "HisHeroLightBlend");
    if ((Object) heroPowerCard != (Object) null)
    {
      while (!heroPowerCard.GetActor().IsShown())
        yield return (object) null;
      GameState.Get().GetGameEntity().FadeInActor(heroPowerCard.GetActor(), 0.4f);
    }
    while (!heroCard.GetActor().IsShown())
      yield return (object) null;
    GameState.Get().GetGameEntity().FadeInHeroActor(heroCard.GetActor());
    EmoteEntry emoteEntry1 = heroCard.GetEmoteEntry(EmoteType.START);
    bool flag = true;
    if (emoteEntry1 != null)
    {
      CardSoundSpell soundSpell = emoteEntry1.GetSoundSpell();
      if ((Object) soundSpell != (Object) null && (Object) soundSpell.DetermineBestAudioSource() == (Object) null)
        flag = false;
    }
    CardSoundSpell emoteSpell = (CardSoundSpell) null;
    if (flag)
      emoteSpell = heroCard.PlayEmote(EmoteType.START);
    if ((Object) emoteSpell != (Object) null)
    {
      while (emoteSpell.GetActiveState() != SpellStateType.NONE)
        yield return (object) null;
    }
    else
      yield return (object) new WaitForSeconds(2.5f);
    GameState.Get().GetGameEntity().FadeOutHeroActor(heroCard.GetActor());
    if ((Object) heroPowerCard != (Object) null)
      GameState.Get().GetGameEntity().FadeOutActor(heroPowerCard.GetActor());
    Card myHeroCard = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
    Card myHeroPowerCard = GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard();
    iTween.StopByName(mulliganGO, "MyHeroLightBlend");
    if ((Object) myHeroPowerCard != (Object) null)
      GameState.Get().GetGameEntity().FadeInActor(myHeroPowerCard.GetActor(), 0.4f);
    EmoteType emoteToPlay = EmoteType.START;
    EmoteEntry emoteEntry2 = myHeroCard.GetEmoteEntry(EmoteType.START);
    if (emoteEntry2 != null && !string.IsNullOrEmpty(emoteEntry2.GetGameStringKey()))
    {
      EmoteEntry emoteEntry3 = heroCard.GetEmoteEntry(EmoteType.START);
      if (emoteEntry3 != null && emoteEntry2.GetGameStringKey() == emoteEntry3.GetGameStringKey())
        emoteToPlay = EmoteType.MIRROR_START;
    }
    while (!myHeroCard.GetActor().IsShown())
      yield return (object) null;
    GameState.Get().GetGameEntity().FadeInHeroActor(myHeroCard.GetActor());
    emoteSpell = myHeroCard.PlayEmote(emoteToPlay, Notification.SpeechBubbleDirection.BottomRight);
    if ((Object) emoteSpell != (Object) null)
    {
      while (emoteSpell.GetActiveState() != SpellStateType.NONE)
        yield return (object) null;
    }
    else
      yield return (object) new WaitForSeconds(2.5f);
    GameState.Get().GetGameEntity().FadeOutHeroActor(myHeroCard.GetActor());
    if ((Object) myHeroPowerCard != (Object) null)
      GameState.Get().GetGameEntity().FadeOutActor(myHeroPowerCard.GetActor());
  }
}
