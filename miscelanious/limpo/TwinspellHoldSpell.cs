using System.Collections;
using UnityEngine;

public class TwinspellHoldSpell : Spell
{
  private Entity m_originalSpellEntity;
  private Actor m_fakeTwinspellActor;
  private int m_fakeTwinspellHandSlot;
  private bool m_fakeActorLoaded;

  protected override void OnBirth(SpellStateType prevStateType)
  {
    base.OnBirth(prevStateType);
    this.StartCoroutine(this.DoUpdate());
  }

  protected override void OnDeath(SpellStateType prevStateType)
  {
    base.OnDeath(prevStateType);
    this.StopAllCoroutines();
    this.HideFakeTwinspellActor();
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StopAllCoroutines();
  }

  private IEnumerator DoUpdate()
  {
    while (true)
    {
      if (this.m_fakeActorLoaded && (Object) this.m_fakeTwinspellActor != (Object) null)
      {
        if (this.m_fakeTwinspellActor.GetSpell(SpellType.TWINSPELLPENDING).GetActiveState() == SpellStateType.NONE)
          this.ShowFakeTwinspellActor();
        ZoneHand friendlyHand = InputManager.Get().GetFriendlyHand();
        this.m_fakeTwinspellActor.transform.position = friendlyHand.GetCardPosition(this.m_fakeTwinspellHandSlot, -1);
        this.m_fakeTwinspellActor.transform.localEulerAngles = friendlyHand.GetCardRotation(this.m_fakeTwinspellHandSlot, -1);
        this.m_fakeTwinspellActor.transform.localScale = friendlyHand.GetCardScale();
      }
      yield return (object) null;
    }
  }

  public bool Initialize(int heldEntityId, int zonePosition)
  {
    this.m_fakeActorLoaded = false;
    this.m_originalSpellEntity = GameState.Get().GetEntity(heldEntityId);
    if (this.m_originalSpellEntity == null)
    {
      Log.Spells.PrintError("TwinspellHoldSpell.Initialize(): Unable to find Entity for Entity ID {0}.", (object) heldEntityId);
      return false;
    }
    if (!this.m_originalSpellEntity.IsTwinspell())
    {
      Log.Spells.PrintError("TwinspellHoldSpell.Initialize(): TwinspellHoldSpell has been hooked up to a Card that is not a Twinspell!");
      return false;
    }
    if (!this.LoadFakeTwinspellActor())
    {
      Log.Spells.PrintError("TwinspellHoldSpell.Initialize(): Failed to load the fake Twinspell actor", (object) heldEntityId);
      return false;
    }
    this.m_fakeTwinspellHandSlot = zonePosition - 1;
    return true;
  }

  public int GetOriginalSpellEntityId() => this.m_originalSpellEntity == null ? -1 : this.m_originalSpellEntity.GetEntityId();

  public int GetFakeTwinspellZonePosition() => this.m_fakeTwinspellHandSlot + 1;

  private bool LoadFakeTwinspellActor()
  {
    if ((Object) this.m_fakeTwinspellActor != (Object) null)
    {
      this.m_fakeTwinspellActor.DeactivateAllSpells();
      this.m_fakeTwinspellActor.Destroy();
    }
    if (this.m_originalSpellEntity == null)
    {
      Log.Spells.PrintError("TwinspellHoldSpell.LoadFakeTwinspellActor(): m_originalSpellEntity is null. Has TwinspellHoldSpell.Initialize() been called?");
      return false;
    }
    if (!this.m_originalSpellEntity.HasTag(GAME_TAG.TWINSPELL_COPY))
    {
      Log.Spells.PrintError("TwinspellHoldSpell.LoadFakeTwinspellActor(): m_originalSpellEntity does not have the TWINSPELL_COPY tag");
      return false;
    }
    int tag = this.m_originalSpellEntity.GetTag(GAME_TAG.TWINSPELL_COPY);
    using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(tag))
    {
      if (fullDef?.EntityDef == null)
      {
        Log.Spells.PrintError("TwinspellHoldSpell.LoadFakeTwinspellActor(): Unable to load EntityDef for card ID {0}.", (object) tag);
        return false;
      }
      if ((Object) fullDef?.CardDef == (Object) null)
      {
        Log.Spells.PrintError("TwinspellHoldSpell.LoadFakeTwinspellActor(): Unable to load CardDef for card ID {0}.", (object) tag);
        return false;
      }
      string twinspellCardId = GameUtils.TranslateDbIdToCardId(tag);
      AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(fullDef.EntityDef, this.m_originalSpellEntity.GetPremiumType()), (PrefabCallback<GameObject>) ((actorName, actorGameObject, data) => this.OnFakeTwinspellActorLoaded(actorName, actorGameObject, twinspellCardId, this.m_originalSpellEntity.GetPremiumType())), (object) twinspellCardId, AssetLoadingOptions.IgnorePrefabPosition);
      return true;
    }
  }

  private void OnFakeTwinspellActorLoaded(
    AssetReference assetRef,
    GameObject actorGameObject,
    string fakeTwinspellCardId,
    TAG_PREMIUM premium)
  {
    if ((Object) actorGameObject == (Object) null)
    {
      Debug.LogError((object) string.Format("TwinspellHoldSpell.OnFakeTwinspellActorLoaded: Unable to load fake actor for card: {0}", (object) assetRef));
    }
    else
    {
      if ((Object) this.m_fakeTwinspellActor != (Object) null)
      {
        this.m_fakeTwinspellActor.DeactivateAllSpells();
        this.m_fakeTwinspellActor.Destroy();
      }
      this.m_fakeTwinspellActor = actorGameObject.GetComponent<Actor>();
      using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(fakeTwinspellCardId, this.m_fakeTwinspellActor.CardPortraitQuality))
        this.m_fakeTwinspellActor.SetFullDef(fullDef);
      this.m_fakeTwinspellActor.SetPremium(this.m_originalSpellEntity.GetPremiumType());
      this.m_fakeTwinspellActor.SetCardBackSideOverride(new Player.Side?(this.m_originalSpellEntity.GetControllerSide()));
      this.m_fakeTwinspellActor.SetWatermarkCardSetOverride(this.m_originalSpellEntity.GetWatermarkCardSetOverride());
      this.m_fakeTwinspellActor.UpdateAllComponents();
      this.m_fakeTwinspellActor.Hide();
      this.m_fakeActorLoaded = true;
    }
  }

  private void ShowFakeTwinspellActor()
  {
    if ((Object) this.m_fakeTwinspellActor != (Object) null && this.m_fakeTwinspellActor.IsShown())
      return;
    ZoneHand friendlyHand = InputManager.Get().GetFriendlyHand();
    this.m_fakeTwinspellActor.transform.position = friendlyHand.GetCardPosition(this.m_fakeTwinspellHandSlot, -1);
    this.m_fakeTwinspellActor.transform.localEulerAngles = friendlyHand.GetCardRotation(this.m_fakeTwinspellHandSlot, -1);
    this.m_fakeTwinspellActor.transform.localScale = friendlyHand.GetCardScale();
    this.m_fakeTwinspellActor.ActivateSpellBirthState(SpellType.TWINSPELLPENDING);
  }

  private void HideFakeTwinspellActor()
  {
    if ((Object) this.m_fakeTwinspellActor == (Object) null)
      return;
    this.m_fakeTwinspellActor.ActivateSpellDeathState(SpellType.TWINSPELLPENDING);
    this.m_fakeTwinspellActor.Hide();
  }
}
