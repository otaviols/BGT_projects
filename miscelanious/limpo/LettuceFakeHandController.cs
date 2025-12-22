using System;
using System.Collections.Generic;
using UnityEngine;

public class LettuceFakeHandController
{
  private readonly List<LettuceFakeHandController.OriginalSetting> m_originalSettings = new List<LettuceFakeHandController.OriginalSetting>();
  private readonly List<Actor> m_fakeHandActors = new List<Actor>();
  private bool m_shown;

  private bool ShouldShowOpposingFakeHand()
  {
    foreach (EntityBase opposingPlayer in GameState.Get().GetOpposingPlayers())
    {
      if (opposingPlayer.HasTag(GAME_TAG.LETTUCE_MERCENARIES_TO_NOMINATE))
        return true;
    }
    return false;
  }

  public void ShowOpposingFakeHand(Action onFinish)
  {
    if (this.m_shown)
      onFinish();
    else if (!this.ShouldShowOpposingFakeHand())
    {
      this.m_shown = false;
      onFinish();
    }
    else
    {
      Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
      List<Card> cards = opposingSidePlayer.GetDeckZone().GetCards();
      int count = cards.Count;
      if (count == 0)
      {
        this.m_shown = false;
        onFinish();
      }
      else
      {
        this.m_shown = true;
        this.m_originalSettings.Clear();
        ZoneHand handZone = opposingSidePlayer.GetHandZone();
        int animationCount = count;
        for (int index = 0; index < count; ++index)
        {
          if (index >= this.m_fakeHandActors.Count)
            this.m_fakeHandActors.Add(AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", AssetLoadingOptions.IgnorePrefabPosition).GetComponent<Actor>());
          Actor fakeHandActor = this.m_fakeHandActors[index];
          Card card = cards[index];
          Entity entity = card.GetEntity();
          fakeHandActor.SetCard(card);
          fakeHandActor.SetCardDefFromCard(card);
          fakeHandActor.SetEntity(entity);
          fakeHandActor.SetEntityDef(entity.GetEntityDef());
          fakeHandActor.SetCardBackSideOverride(new Player.Side?(entity.GetControllerSide()));
          fakeHandActor.UpdateAllComponents();
          this.m_originalSettings.Add(new LettuceFakeHandController.OriginalSetting()
          {
            m_Actor = card.GetActor(),
            m_Position = card.transform.position,
            m_LocalEulerAngles = card.transform.localEulerAngles,
            m_LocalScale = card.transform.localScale
          });
          card.transform.position = handZone.GetCardPosition(index, count);
          card.transform.localEulerAngles = handZone.GetCardRotation(index, count);
          card.transform.localScale = handZone.GetCardScale();
          fakeHandActor.Hide();
          card.SetActor(fakeHandActor);
          card.ActivateActorSpell(SpellType.SUMMON_IN, (Spell.FinishedCallback) ((spell, userData) =>
          {
            --animationCount;
            if (animationCount != 0)
              return;
            onFinish();
          }));
        }
      }
    }
  }

  public void HideOpposingFakeHand(Action onFinish)
  {
    if (!this.m_shown)
    {
      onFinish();
    }
    else
    {
      this.m_shown = false;
      List<Card> cards = GameState.Get().GetOpposingSidePlayer().GetDeckZone().GetCards();
      int animationCount = cards.Count;
      if (animationCount == 0)
      {
        onFinish();
      }
      else
      {
        for (int index = 0; index < cards.Count && index < this.m_fakeHandActors.Count && index < this.m_originalSettings.Count; ++index)
        {
          Card card = cards[index];
          Actor fakeActor = this.m_fakeHandActors[index];
          if ((UnityEngine.Object) fakeActor.transform.parent != (UnityEngine.Object) null)
          {
            LettuceFakeHandController.OriginalSetting setting = this.m_originalSettings[index];
            card.ActivateActorSpell(SpellType.SUMMON_OUT, (Spell.FinishedCallback) ((spell, userData) =>
            {
              fakeActor.ReleaseSpell(SpellType.SUMMON_OUT);
              fakeActor.SetCard((Card) null);
              card.SetActor(setting.m_Actor);
              card.transform.position = setting.m_Position;
              card.transform.localEulerAngles = setting.m_LocalEulerAngles;
              card.transform.localScale = setting.m_LocalScale;
              --animationCount;
              if (animationCount != 0)
                return;
              onFinish();
            }));
          }
        }
      }
    }
  }

  private struct OriginalSetting
  {
    public Actor m_Actor { get; set; }

    public Vector3 m_Position { get; set; }

    public Vector3 m_LocalEulerAngles { get; set; }

    public Vector3 m_LocalScale { get; set; }
  }
}
