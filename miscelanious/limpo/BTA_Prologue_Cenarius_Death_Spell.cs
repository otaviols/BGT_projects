using System.Collections;
using UnityEngine;

public class BTA_Prologue_Cenarius_Death_Spell : Spell
{
  public Spell m_ExplodeReformSpell;
  private Card m_CenariusCard;
  private Spell m_explodeReformSpellInstance;
  private BTA_Prologue_Fight_04 m_missionEntity;
  private BTA_Prologue_Cenarius_Death_Spell.FakeDeathState m_fakeDeathState;

  public override bool AddPowerTargets()
  {
    base.AddPowerTargets();
    if (this.m_missionEntity == null)
    {
      this.m_missionEntity = GameState.Get().GetGameEntity() as BTA_Prologue_Fight_04;
      if (this.m_missionEntity == null)
        Log.Spells.PrintError("BTA_Prologue_Cenarius_Death_Spell.AddPowerTargets(): GameEntity is not an instance of BTA_Prologue_Fight_04!");
    }
    this.FindHeroCards();
    return true;
  }

  private void FindHeroCards()
  {
    if (!((Object) this.m_CenariusCard == (Object) null))
      return;
    this.m_CenariusCard = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
  }

  public override bool ShouldReconnectIfStuck() => false;

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DoEffects());
  }

  private IEnumerator DoEffects()
  {
    BTA_Prologue_Cenarius_Death_Spell cenariusDeathSpell = this;
    if (cenariusDeathSpell.m_fakeDeathState == BTA_Prologue_Cenarius_Death_Spell.FakeDeathState.EXPLODING_CENARIUS)
      yield return (object) cenariusDeathSpell.StartCoroutine(cenariusDeathSpell.ExplodeCenarius());
    cenariusDeathSpell.OnSpellFinished();
    cenariusDeathSpell.OnStateFinished();
  }

  private IEnumerator ExplodeCenarius()
  {
    BTA_Prologue_Cenarius_Death_Spell cenariusDeathSpell = this;
    PegCursor.Get().SetMode(PegCursor.Mode.STOPWAITING);
    Card EnemyHeroCard = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
    EnemyHeroCard.ActivateCharacterDeathEffects();
    cenariusDeathSpell.m_explodeReformSpellInstance = SpellManager.Get().GetSpell(cenariusDeathSpell.m_ExplodeReformSpell);
    SpellUtils.SetCustomSpellParent(cenariusDeathSpell.m_explodeReformSpellInstance, (Component) EnemyHeroCard.GetActor());
    cenariusDeathSpell.m_explodeReformSpellInstance.ActivateState(SpellStateType.ACTION);
    while (cenariusDeathSpell.m_explodeReformSpellInstance.GetActiveState() != SpellStateType.NONE)
      yield return (object) null;
    cenariusDeathSpell.StartCoroutine(cenariusDeathSpell.m_missionEntity.PlayVictoryLines());
    yield return (object) new WaitForSeconds(10f);
    cenariusDeathSpell.m_explodeReformSpellInstance.ActivateState(SpellStateType.DEATH);
    while (!cenariusDeathSpell.m_explodeReformSpellInstance.IsFinished())
      yield return (object) null;
    EnemyHeroCard.ShowCard();
  }

  private IEnumerator HideBoardElements()
  {
    yield return (object) new WaitForSeconds(0.5f);
    Player controller = GameState.Get().GetFriendlySidePlayer();
    if ((Object) controller.GetHeroPowerCard() != (Object) null)
    {
      controller.GetHeroPowerCard().HideCard();
      controller.GetHeroPowerCard().GetActor().ToggleForceIdle(true);
      controller.GetHeroPowerCard().GetActor().SetActorState(ActorStateType.CARD_IDLE);
      controller.GetHeroPowerCard().GetActor().DoCardDeathVisuals();
    }
    if ((Object) controller.GetWeaponCard() != (Object) null)
    {
      controller.GetWeaponCard().HideCard();
      controller.GetWeaponCard().GetActor().ToggleForceIdle(true);
      controller.GetWeaponCard().GetActor().SetActorState(ActorStateType.CARD_IDLE);
      controller.GetWeaponCard().GetActor().DoCardDeathVisuals();
    }
    Actor actor = controller.GetHeroCard().GetActor();
    actor.HideArmorSpell();
    actor.GetHealthObject().Hide();
    actor.GetAttackObject().Hide();
    actor.ToggleForceIdle(true);
    actor.SetActorState(ActorStateType.CARD_IDLE);
    yield return (object) new WaitForSeconds(3f);
    Player firstOpponentPlayer = GameState.Get().GetFirstOpponentPlayer(controller);
    if ((Object) firstOpponentPlayer.GetHeroPowerCard() != (Object) null)
    {
      firstOpponentPlayer.GetHeroPowerCard().HideCard();
      firstOpponentPlayer.GetHeroPowerCard().GetActor().ToggleForceIdle(true);
      firstOpponentPlayer.GetHeroPowerCard().GetActor().SetActorState(ActorStateType.CARD_IDLE);
      firstOpponentPlayer.GetHeroPowerCard().GetActor().DoCardDeathVisuals();
    }
    if ((Object) firstOpponentPlayer.GetWeaponCard() != (Object) null)
    {
      firstOpponentPlayer.GetWeaponCard().HideCard();
      firstOpponentPlayer.GetWeaponCard().GetActor().ToggleForceIdle(true);
      firstOpponentPlayer.GetWeaponCard().GetActor().SetActorState(ActorStateType.CARD_IDLE);
      firstOpponentPlayer.GetWeaponCard().GetActor().DoCardDeathVisuals();
    }
  }

  private enum FakeDeathState
  {
    EXPLODING_CENARIUS,
  }
}
