using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICCPrologueFakeDeath : Spell
{
  public Spell m_ExplodeReformSpell;
  public Spell m_LichKingExitSpell;
  public Spell m_TirionEnterSpell;
  public GameObject m_FakeDefeatScreen;
  public float m_FakeDefeatScreenShowTime = 5f;
  public float m_TirionEnterDelay = 2f;
  private Card m_lichKingCard;
  private Card m_tirionCard;
  private Card m_frostLichJainaCard;
  private int m_tirionEnterTaskIndex;
  private int m_frostLichJainaEnterTaskIndex;
  private Spell m_explodeReformSpellInstance;
  private ICC_01_LICHKING m_missionEntity;
  private ICCPrologueFakeDeath.FakeDeathState m_fakeDeathState;
  private ScreenEffectsHandle m_screenEffectsHandle;

  public override bool AddPowerTargets()
  {
    base.AddPowerTargets();
    if (this.m_missionEntity == null)
    {
      this.m_missionEntity = GameState.Get().GetGameEntity() as ICC_01_LICHKING;
      if (this.m_missionEntity == null)
        Log.Spells.PrintError("ICCPrologueFakeDeath.AddPowerTargets(): GameEntity is not an instance of ICC_01_LICHKING!");
    }
    this.FindHeroCards();
    return true;
  }

  private void FindHeroCards()
  {
    if ((Object) this.m_lichKingCard == (Object) null)
      this.m_lichKingCard = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
    if ((Object) this.m_frostLichJainaCard == (Object) null)
    {
      List<PowerTask> taskList = this.m_taskList.GetTaskList();
      for (int index = 0; index < taskList.Count; ++index)
      {
        if (taskList[index].GetPower() is Network.HistFullEntity power)
        {
          Entity entity = GameState.Get().GetEntity(power.Entity.ID);
          if (entity.GetControllerSide() == Player.Side.FRIENDLY && entity.IsHero())
          {
            this.m_frostLichJainaCard = entity.GetCard();
            this.m_frostLichJainaEnterTaskIndex = index;
            break;
          }
        }
      }
    }
    if (!((Object) this.m_tirionCard == (Object) null))
      return;
    List<PowerTask> taskList1 = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      if (taskList1[index].GetPower() is Network.HistTagChange power)
      {
        Entity entity = GameState.Get().GetEntity(power.Entity);
        if (entity.GetControllerSide() == Player.Side.OPPOSING && entity.IsHero() && power.Tag == 262)
        {
          this.m_tirionCard = entity.GetCard();
          this.m_tirionEnterTaskIndex = index;
          break;
        }
      }
    }
  }

  public override bool CanPurge() => this.m_fakeDeathState == ICCPrologueFakeDeath.FakeDeathState.COMPLETE && base.CanPurge();

  public override bool ShouldReconnectIfStuck() => false;

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DoEffects());
  }

  private IEnumerator DoEffects()
  {
    ICCPrologueFakeDeath prologueFakeDeath = this;
    if (prologueFakeDeath.m_fakeDeathState == ICCPrologueFakeDeath.FakeDeathState.EXPLODING_JAINA)
      yield return (object) prologueFakeDeath.StartCoroutine(prologueFakeDeath.ExplodeJaina());
    if (prologueFakeDeath.m_fakeDeathState == ICCPrologueFakeDeath.FakeDeathState.FROST_LICH_JAINA_ENTER)
      yield return (object) prologueFakeDeath.StartCoroutine(prologueFakeDeath.FrostJainaEnter());
    if (prologueFakeDeath.m_fakeDeathState == ICCPrologueFakeDeath.FakeDeathState.LICH_KING_EXIT)
      yield return (object) prologueFakeDeath.StartCoroutine(prologueFakeDeath.LichKingExit());
    if (prologueFakeDeath.m_fakeDeathState == ICCPrologueFakeDeath.FakeDeathState.TIRION_ENTER)
      yield return (object) prologueFakeDeath.StartCoroutine(prologueFakeDeath.TirionEnter());
    prologueFakeDeath.OnSpellFinished();
    prologueFakeDeath.OnStateFinished();
  }

  private IEnumerator ExplodeJaina()
  {
    ICCPrologueFakeDeath prologueFakeDeath = this;
    EndTurnButton.Get().AddInputBlocker();
    PegCursor.Get().SetMode(PegCursor.Mode.STOPWAITING);
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_EndGameScreen);
    SoundManager.Get().LoadAndPlay((AssetReference) "defeat_jingle.prefab:0744a10f38e92f1438a02349c29a7b76");
    prologueFakeDeath.StartCoroutine(prologueFakeDeath.HideBoardElements());
    Card heroCard = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
    heroCard.ActivateCharacterDeathEffects();
    prologueFakeDeath.m_explodeReformSpellInstance = SpellManager.Get().GetSpell(prologueFakeDeath.m_ExplodeReformSpell);
    SpellUtils.SetCustomSpellParent(prologueFakeDeath.m_explodeReformSpellInstance, (Component) heroCard.GetActor());
    prologueFakeDeath.m_explodeReformSpellInstance.ActivateState(SpellStateType.ACTION);
    while (prologueFakeDeath.m_explodeReformSpellInstance.GetActiveState() != SpellStateType.NONE)
      yield return (object) null;
    prologueFakeDeath.m_fakeDeathState = ICCPrologueFakeDeath.FakeDeathState.FROST_LICH_JAINA_ENTER;
  }

  private IEnumerator FrostJainaEnter()
  {
    ICCPrologueFakeDeath owner = this;
    if (!((Object) owner.m_frostLichJainaCard == (Object) null))
    {
      owner.m_taskList.DoTasks(0, owner.m_frostLichJainaEnterTaskIndex);
      GameObject fakeDefeatScreenInstance = Object.Instantiate<GameObject>(owner.m_FakeDefeatScreen);
      DefeatTwoScoop defeatTwoScoop = fakeDefeatScreenInstance.GetComponentInChildren<DefeatTwoScoop>(true);
      while (!defeatTwoScoop.IsLoaded())
        yield return (object) null;
      owner.m_screenEffectsHandle = new ScreenEffectsHandle((object) owner);
      ScreenEffectParameters desaturatePerspective = ScreenEffectParameters.BlurVignetteDesaturatePerspective;
      owner.m_screenEffectsHandle.StartEffect(desaturatePerspective);
      defeatTwoScoop.Show(false);
      yield return (object) new WaitForSeconds(owner.m_FakeDefeatScreenShowTime);
      owner.m_screenEffectsHandle.StopEffect();
      defeatTwoScoop.Hide();
      owner.m_taskList.DoTasks(0, owner.m_frostLichJainaEnterTaskIndex + 1);
      while ((Object) owner.m_frostLichJainaCard.GetActor() == (Object) null || owner.m_frostLichJainaCard.IsActorLoading())
        yield return (object) null;
      owner.m_frostLichJainaCard.HideCard();
      owner.m_explodeReformSpellInstance.ActivateState(SpellStateType.DEATH);
      if (owner.m_missionEntity != null)
        owner.StartCoroutine(owner.m_missionEntity.PlayLichKingRezLines());
      while (!owner.m_explodeReformSpellInstance.IsFinished())
        yield return (object) null;
      owner.m_frostLichJainaCard.ShowCard();
      owner.m_frostLichJainaCard.GetActor().GetAttackObject().Hide();
      while (owner.m_explodeReformSpellInstance.GetActiveState() != SpellStateType.NONE)
        yield return (object) null;
      while (GameState.Get().IsBusy())
        yield return (object) null;
      Object.Destroy((Object) fakeDefeatScreenInstance);
      owner.m_fakeDeathState = ICCPrologueFakeDeath.FakeDeathState.LICH_KING_EXIT;
    }
  }

  private IEnumerator LichKingExit()
  {
    Spell lichKingExitSpellInstance = SpellManager.Get().GetSpell(this.m_LichKingExitSpell);
    SpellUtils.SetCustomSpellParent(lichKingExitSpellInstance, (Component) this.m_lichKingCard.GetActor());
    lichKingExitSpellInstance.Activate();
    while (lichKingExitSpellInstance.GetActiveState() != SpellStateType.NONE)
      yield return (object) null;
    yield return (object) new WaitForSeconds(this.m_TirionEnterDelay);
    this.m_fakeDeathState = ICCPrologueFakeDeath.FakeDeathState.TIRION_ENTER;
  }

  private IEnumerator TirionEnter()
  {
    ICCPrologueFakeDeath prologueFakeDeath = this;
    if (!((Object) prologueFakeDeath.m_tirionCard == (Object) null))
    {
      prologueFakeDeath.m_taskList.DoTasks(0, prologueFakeDeath.m_tirionEnterTaskIndex + 1);
      prologueFakeDeath.m_tirionCard.SetDoNotSort(true);
      prologueFakeDeath.m_tirionCard.SetDoNotWarpToNewZone(true);
      while ((Object) prologueFakeDeath.m_tirionCard.GetActor() == (Object) null || prologueFakeDeath.m_tirionCard.IsActorLoading())
        yield return (object) null;
      TransformUtil.CopyWorld((Component) prologueFakeDeath.m_tirionCard, (Component) prologueFakeDeath.m_tirionCard.GetZone().transform);
      prologueFakeDeath.m_tirionCard.GetActor().Hide();
      Spell tirionEnterSpellInstance = SpellManager.Get().GetSpell(prologueFakeDeath.m_TirionEnterSpell);
      SpellUtils.SetCustomSpellParent(tirionEnterSpellInstance, (Component) prologueFakeDeath.m_tirionCard.GetActor());
      tirionEnterSpellInstance.Activate();
      while (tirionEnterSpellInstance.GetActiveState() != SpellStateType.NONE)
        yield return (object) null;
      prologueFakeDeath.m_tirionCard.SetDoNotSort(false);
      prologueFakeDeath.m_tirionCard.SetDoNotWarpToNewZone(false);
      NameBanner nameBannerForSide = Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING);
      nameBannerForSide.UpdateHeroNameBanner();
      nameBannerForSide.UpdateSubtext();
      prologueFakeDeath.m_missionEntity.StartGameplaySoundtracks();
      EndTurnButton.Get().RemoveInputBlocker();
      prologueFakeDeath.m_fakeDeathState = ICCPrologueFakeDeath.FakeDeathState.COMPLETE;
    }
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
    EXPLODING_JAINA,
    FROST_LICH_JAINA_ENTER,
    LICH_KING_EXIT,
    TIRION_ENTER,
    COMPLETE,
  }
}
