using Blizzard.T5.Core;
using PegasusLettuce;
using System.Collections;
using UnityEngine;

public class LettucePvPMissionEntity : LettuceMissionEntity
{
  private Spell m_versusSpell;
  private static readonly Map<GameEntityOption, bool> s_booleanOptions = LettucePvPMissionEntity.InitBooleanOptions();
  private static readonly Map<GameEntityOption, string> s_stringOptions = LettucePvPMissionEntity.InitStringOptions();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.WAIT_FOR_RATING_INFO,
      true
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public LettucePvPMissionEntity()
    : base()
  {
    this.m_gameOptions.AddOptions(LettucePvPMissionEntity.s_booleanOptions, LettucePvPMissionEntity.s_stringOptions);
    this.m_enemyAbilityOrderSpeechBubblesEnabled = false;
    Network.Get().RegisterNetHandler((object) MercenariesPvPRatingUpdate.PacketID.ID, new Network.NetHandler(this.OnRatingChange));
  }

  public override void OnDecommissionGame()
  {
    if (Network.Get() != null)
      Network.Get().RemoveNetHandler((object) MercenariesPvPRatingUpdate.PacketID.ID, new Network.NetHandler(this.OnRatingChange));
    base.OnDecommissionGame();
  }

  private void OnRatingChange() => this.RatingChangeData = Network.Get().MercenariesPvPRatingUpdate();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    this.PreloadPrefab((AssetReference) "Lettuce_VersusSpell.prefab:1dec81ab7c8a7704d9f8b316085937a7", (PrefabCallback<GameObject>) ((assetRef, gameObject, callbackData) =>
    {
      this.m_versusSpell = gameObject.GetComponent<Spell>();
      if (!((Object) this.m_versusSpell != (Object) null))
        return;
      this.m_versusSpell.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
      {
        if (spell.GetActiveState() != SpellStateType.NONE)
          return;
        GameEntity.Coroutines.StartCoroutine(this.WaitThenDestroyVersusSpell());
      }));
    }));
  }

  private IEnumerator WaitThenDestroyVersusSpell()
  {
    yield return (object) new WaitForSeconds(10f);
    this.DestroyVersusSpell();
  }

  public override void OnTagChanged(TagDelta change)
  {
    base.OnTagChanged(change);
    if (change.tag != 2228)
      return;
    switch ((SpellStateType) change.newValue)
    {
      case SpellStateType.ACTION:
        this.ActivateVersusSpellState(SpellStateType.ACTION);
        break;
      case SpellStateType.DEATH:
        this.ActivateVersusSpellState(SpellStateType.DEATH);
        break;
    }
  }

  protected override void OnLettuceMissionEntityGameSceneLoaded()
  {
    if (this.GetTag(GAME_TAG.TURN) == 0)
      this.ActivateVersusSpellState(SpellStateType.BIRTH);
    else
      this.DestroyVersusSpell();
  }

  private void DestroyVersusSpell()
  {
    if (!((Object) this.m_versusSpell != (Object) null))
      return;
    Object.Destroy((Object) this.m_versusSpell.gameObject);
    this.m_versusSpell = (Spell) null;
  }

  private void ActivateVersusSpellState(SpellStateType stateType)
  {
    if (!((Object) this.m_versusSpell != (Object) null))
      return;
    this.m_versusSpell.ActivateState(stateType);
  }
}
