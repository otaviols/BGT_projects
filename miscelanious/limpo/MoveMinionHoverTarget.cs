using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Actor))]
public class MoveMinionHoverTarget : MonoBehaviour
{
  private Actor m_actor;
  private Card m_lastDroppedCard;
  private readonly List<PlayErrors.ErrorType> PlayErrorsToSuppressWhenDisplaying = new List<PlayErrors.ErrorType>()
  {
    PlayErrors.ErrorType.INVALID,
    PlayErrors.ErrorType.REQ_TARGET_TO_PLAY
  };

  private Entity m_entity => this.m_actor.GetEntity();

  private void Start()
  {
    GameState.Get().RegisterOptionRejectedListener(new GameState.OptionRejectedCallback(this.OnOptionRejected));
    this.m_actor = this.GetComponent<Actor>();
  }

  private void OnDestroy()
  {
    if (GameState.Get() == null)
      return;
    GameState.Get().UnregisterOptionRejectedListener(new GameState.OptionRejectedCallback(this.OnOptionRejected));
  }

  public void DropCardOnHoverTarget(Card heldCard)
  {
    Entity entity = heldCard.GetEntity();
    this.m_lastDroppedCard = heldCard;
    if ((Object) ThinkEmoteManager.Get() != (Object) null)
      ThinkEmoteManager.Get().NotifyOfActivity();
    GameState gameState = GameState.Get();
    PlayErrors.ErrorType mainOptionPlayErrorType;
    int? mainOptionPlayErrorParam;
    PlayErrors.ErrorType targetPlayErrorType;
    int? targetPlayErrorParam;
    if (this.SetSelectedOption(entity, out mainOptionPlayErrorType, out mainOptionPlayErrorParam, out targetPlayErrorType, out targetPlayErrorParam))
    {
      gameState.SetSelectedOptionTarget(entity.GetEntityId());
      gameState.SendOption();
    }
    else
    {
      if (!this.PlayErrorsToSuppressWhenDisplaying.Contains(mainOptionPlayErrorType))
        PlayErrors.DisplayPlayError(mainOptionPlayErrorType, mainOptionPlayErrorParam, this.m_entity);
      else if (!this.PlayErrorsToSuppressWhenDisplaying.Contains(targetPlayErrorType))
        PlayErrors.DisplayPlayError(targetPlayErrorType, targetPlayErrorParam, this.m_entity);
      InputManager.Get().AddHeldCardBackToPlayZone(this.m_lastDroppedCard);
    }
  }

  private bool SetSelectedOption(
    Entity heldEntity,
    out PlayErrors.ErrorType mainOptionPlayErrorType,
    out int? mainOptionPlayErrorParam,
    out PlayErrors.ErrorType targetPlayErrorType,
    out int? targetPlayErrorParam)
  {
    GameState gameState = GameState.Get();
    mainOptionPlayErrorType = PlayErrors.ErrorType.INVALID;
    mainOptionPlayErrorParam = new int?();
    targetPlayErrorType = PlayErrors.ErrorType.INVALID;
    targetPlayErrorParam = new int?();
    Network.Options optionsPacket = gameState.GetOptionsPacket();
    if (optionsPacket == null || optionsPacket.List == null)
      return false;
    for (int index = 0; index < optionsPacket.List.Count; ++index)
    {
      Network.Options.Option option = optionsPacket.List[index];
      if (option.Type == Network.Options.Option.OptionType.POWER && option.Main.ID == this.m_entity.GetEntityId())
      {
        if (!option.Main.IsValidTarget(heldEntity.GetEntityId()))
        {
          targetPlayErrorType = option.Main.GetErrorForTarget(heldEntity.GetEntityId());
          targetPlayErrorParam = option.Main.GetErrorParamForTarget(heldEntity.GetEntityId());
        }
        else if (!option.Main.PlayErrorInfo.IsValid())
        {
          mainOptionPlayErrorType = option.Main.PlayErrorInfo.PlayError;
          mainOptionPlayErrorParam = option.Main.PlayErrorInfo.PlayErrorParam;
        }
        else
        {
          gameState.SetSelectedOption(index);
          return true;
        }
      }
    }
    return false;
  }

  private void OnOptionRejected(Network.Options.Option option, object userData)
  {
    if (option.Main.ID != this.m_entity.GetEntityId())
      return;
    InputManager.Get().AddHeldCardBackToPlayZone(this.m_lastDroppedCard);
  }
}
