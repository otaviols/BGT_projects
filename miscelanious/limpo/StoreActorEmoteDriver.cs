using Hearthstone.UI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreActorEmoteDriver : MonoBehaviour
{
  private const float DefaultEmoteWaitTimeSec = 1.5f;
  private readonly Dictionary<EmoteType, EmoteEntry> m_actorEmoteLookup = new Dictionary<EmoteType, EmoteEntry>();
  private Notification m_activeNotification;
  private Actor m_actor;

  [Overridable]
  public Actor Actor
  {
    get => this.m_actor;
    set
    {
      if ((UnityEngine.Object) value == (UnityEngine.Object) this.m_actor)
        return;
      this.OnActorLoaded(value);
    }
  }

  public void PlayEmote(Actor owner, EmoteType emote, Action<int> onFinishedCallback = null)
  {
    if (emote == EmoteType.INVALID)
      return;
    if ((UnityEngine.Object) owner != (UnityEngine.Object) this.m_actor)
    {
      Debug.LogError((object) "StoreActorEmoteDriver: Failed to play emote as requesting actor does match loaded emotes actor.");
    }
    else
    {
      EmoteEntry emoteEntry;
      if (!this.m_actorEmoteLookup.TryGetValue(emote, out emoteEntry))
        Debug.LogWarning((object) ("StoreActorEmoteDriver: Failed to play emote: " + emote.ToString() + " as it is not support by loaded actor."));
      else
        this.InternalPlayEmote(emoteEntry, onFinishedCallback);
    }
  }

  public void StopEmote()
  {
    if (this.m_actorEmoteLookup == null || this.m_actorEmoteLookup.Count == 0)
      return;
    foreach (EmoteEntry emoteEntry in this.m_actorEmoteLookup.Values)
    {
      Spell soundSpell = (Spell) emoteEntry.GetSoundSpell(false);
      if ((bool) (UnityEngine.Object) soundSpell)
        soundSpell.Deactivate();
    }
    if (!((UnityEngine.Object) this.m_activeNotification != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_activeNotification);
  }

  private void InternalPlayEmote(EmoteEntry emoteEntry, Action<int> onFinishedCallback = null)
  {
    if (emoteEntry == null)
    {
      Debug.LogError((object) "StoreActorEmoteDriver: Failed to play emote as EmoteEntry was null!");
    }
    else
    {
      CardSoundSpell soundSpell1 = emoteEntry.GetSoundSpell();
      Spell spell = emoteEntry.GetSpell();
      if ((UnityEngine.Object) soundSpell1 != (UnityEngine.Object) null)
      {
        soundSpell1.Reactivate();
        if (soundSpell1.IsActive())
        {
          foreach (EmoteEntry emoteEntry1 in this.m_actorEmoteLookup.Values)
          {
            if (emoteEntry1 != emoteEntry)
            {
              Spell soundSpell2 = (Spell) emoteEntry1.GetSoundSpell(false);
              if ((bool) (UnityEngine.Object) soundSpell2)
                soundSpell2.Deactivate();
            }
          }
        }
      }
      string speechText = (string) null;
      if ((UnityEngine.Object) soundSpell1 != (UnityEngine.Object) null)
      {
        speechText = string.Empty;
        if (soundSpell1 is CardSpecificVoSpell)
        {
          CardSpecificVoData bestVoiceData = ((CardSpecificVoSpell) soundSpell1).GetBestVoiceData();
          if (bestVoiceData != null && !string.IsNullOrEmpty(bestVoiceData.m_GameStringKey))
            speechText = GameStrings.Get(bestVoiceData.m_GameStringKey);
        }
      }
      if (string.IsNullOrEmpty(speechText) && !string.IsNullOrEmpty(emoteEntry.GetGameStringKey()))
        speechText = GameStrings.Get(emoteEntry.GetGameStringKey());
      if ((UnityEngine.Object) this.m_activeNotification != (UnityEngine.Object) null)
        NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_activeNotification);
      if (!string.IsNullOrEmpty(speechText))
      {
        this.m_activeNotification = NotificationManager.Get().CreateSpeechBubble(new NotificationManager.SpeechBubbleOptions().WithSpeechText(speechText).WithSpeechBubbleDirection(Notification.SpeechBubbleDirection.BottomLeft).WithActor(this.m_actor).WithDestroyWhenNewCreated(true).WithParentToActor(true).WithVisualEmoteType(NotificationManager.VisualEmoteType.STORE).WithFinishCallback(onFinishedCallback));
        float delaySeconds = 1.5f;
        if ((UnityEngine.Object) soundSpell1 != (UnityEngine.Object) null)
        {
          AudioSource activeAudioSource = soundSpell1.GetActiveAudioSource();
          if ((bool) (UnityEngine.Object) activeAudioSource && (bool) (UnityEngine.Object) activeAudioSource.clip && (double) delaySeconds < (double) activeAudioSource.clip.length)
            delaySeconds = activeAudioSource.clip.length;
        }
        NotificationManager.Get().DestroyNotification(this.m_activeNotification, delaySeconds);
      }
      if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
      {
        VisualEmoteSpell visualEmoteSpell = spell as VisualEmoteSpell;
        if ((UnityEngine.Object) visualEmoteSpell != (UnityEngine.Object) null && visualEmoteSpell.m_PositionOnSpeechBubble && (UnityEngine.Object) this.m_activeNotification != (UnityEngine.Object) null)
        {
          visualEmoteSpell.SetSource(this.m_activeNotification.gameObject);
          visualEmoteSpell.Reactivate();
        }
        else
          spell.Reactivate();
      }
      this.m_actor.LegendaryHeroPortrait?.RaiseEmoteAnimationEvent(emoteEntry.GetEmoteType());
    }
  }

  private void OnActorLoaded(Actor actor)
  {
    foreach (EmoteEntry emoteEntry in this.m_actorEmoteLookup.Values)
      emoteEntry.Clear();
    this.m_actorEmoteLookup.Clear();
    this.m_actor = actor;
    if (this.m_actor.EmoteDefs == null || this.m_actor.EmoteDefs.Count == 0)
      return;
    foreach (EmoteEntryDef emoteDef in this.m_actor.EmoteDefs)
      this.m_actorEmoteLookup[emoteDef.m_emoteType] = new EmoteEntry(emoteDef.m_emoteType, emoteDef.m_emoteSpellPath, emoteDef.m_emoteSoundSpellPath, emoteDef.m_emoteGameStringKey, this.m_actor);
  }
}
