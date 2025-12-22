using System;
using UnityEngine;

public interface ILegendaryHeroPortrait : IDisposable
{
  Texture PortraitTexture { get; }

  bool IsValidForPath(string assetPath, Player.Side playerSide);

  void AttachToActor(Actor actor);

  void RaiseAnimationEvent(string eventName);

  void RaiseEmoteAnimationEvent(EmoteType emote);

  void ClearDynamicResolutionControllers();

  void ConnectDynamicResolutionController(LegendarySkinDynamicResController controller);
}
