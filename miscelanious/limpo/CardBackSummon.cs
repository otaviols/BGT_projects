using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class CardBackSummon : MonoBehaviour
{
  private CardBackManager m_CardBackManager;
  private Actor m_Actor;
  private Spell m_Spell;

  private void OnEnable()
  {
    CardBackManager.Get()?.RegisterUpdateCardbacksListener(new CardBackManager.UpdateCardbacksCallback(this.UpdateCardBack));
    this.m_Spell = GameObjectUtils.FindComponentInParents<Spell>(this.gameObject);
    if ((Object) this.m_Spell == (Object) null)
    {
      Debug.LogWarning((object) "Failed to find Spell on CardBackSummon");
      this.UpdateEchoTexture();
    }
    else
      this.m_Spell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnStateStarted));
  }

  private void OnDisable() => CardBackManager.Get()?.UnregisterUpdateCardbacksListener(new CardBackManager.UpdateCardbacksCallback(this.UpdateCardBack));

  private void OnStateStarted(Spell spell, SpellStateType spellStateType, object userData)
  {
    switch (spell.GetActiveState())
    {
      case SpellStateType.NONE:
        this.m_Actor = (Actor) null;
        break;
      case SpellStateType.BIRTH:
        if (!((Object) spell.GetSourceCard() != (Object) null))
          break;
        this.UpdateEchoTexture();
        break;
    }
  }

  public void UpdateEffectWithCardBack(CardBack cardBack) => this.UpdateEchoTexture(cardBack);

  private void UpdateEchoTexture(CardBack cardBackOverride = null)
  {
    if ((Object) this.m_Actor == (Object) null)
    {
      this.m_Actor = GameObjectUtils.FindComponentInParents<Actor>(this.gameObject);
      if ((Object) this.m_Actor == (Object) null)
        Debug.LogError((object) "CardBackSummonIn failed to get Actor!");
    }
    Renderer component = this.GetComponent<Renderer>();
    Texture texture = component.GetMaterial().mainTexture;
    if ((Object) cardBackOverride != (Object) null)
    {
      texture = (Texture) cardBackOverride.m_HiddenCardEchoTexture;
    }
    else
    {
      if (this.m_CardBackManager == null)
      {
        this.m_CardBackManager = CardBackManager.Get();
        if (this.m_CardBackManager == null)
        {
          Debug.LogError((object) "CardBackSummonIn failed to get CardBackManager!");
          this.enabled = false;
          return;
        }
      }
      if (this.m_CardBackManager.IsActorFriendly(this.m_Actor))
      {
        CardBack friendlyCardBack = this.m_CardBackManager.GetFriendlyCardBack();
        if ((Object) friendlyCardBack != (Object) null)
          texture = (Texture) friendlyCardBack.m_HiddenCardEchoTexture;
      }
      else
      {
        CardBack opponentCardBack = this.m_CardBackManager.GetOpponentCardBack();
        if ((Object) opponentCardBack != (Object) null)
          texture = (Texture) opponentCardBack.m_HiddenCardEchoTexture;
      }
    }
    if (!((Object) texture != (Object) null))
      return;
    component.GetMaterial().mainTexture = texture;
  }

  private void UpdateCardBack() => this.UpdateEchoTexture();
}
