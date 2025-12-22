using System.Collections;
using UnityEngine;

public class HeroCustomSummonSpell : Spell
{
  public Spell m_NewHeroFX;
  private Card m_oldHeroCard;
  private Card m_newHeroCard;
  private Spell m_swapSpell;

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.SetupHeroesAndPlay());
  }

  protected override void OnCancel(SpellStateType prevStateType)
  {
    if ((Object) this.m_swapSpell != (Object) null && this.m_swapSpell.GetActiveState() != SpellStateType.NONE && this.m_swapSpell.GetActiveState() != SpellStateType.CANCEL)
      this.m_swapSpell.ActivateState(SpellStateType.CANCEL);
    base.OnCancel(prevStateType);
  }

  private IEnumerator SetupHeroesAndPlay()
  {
    this.SetupHeroes();
    HeroCustomSummonSpell.HideStats(this.m_oldHeroCard);
    HeroCustomSummonSpell.HideStats(this.m_newHeroCard);
    this.m_newHeroCard.GetActor().TurnOffCollider();
    TransformUtil.CopyWorld((Component) this.m_newHeroCard, (Component) this.m_newHeroCard.GetZone().GetZoneTransformForCard(this.m_newHeroCard));
    if ((Object) this.m_NewHeroFX == (Object) null)
      this.Finish();
    else
      yield return (object) this.PlaySummonSpell();
  }

  private void SetupHeroes()
  {
    this.m_newHeroCard = this.GetSourceCard();
    if ((Object) this.m_newHeroCard == (Object) null)
      Debug.LogErrorFormat("no card for gameObject: {0}", (object) this.GetSource());
    else
      this.m_oldHeroCard = HeroCustomSummonSpell.GetOldHeroCard(this.m_newHeroCard);
  }

  private IEnumerator PlaySummonSpell()
  {
    HeroCustomSummonSpell customSummonSpell = this;
    Actor actor = customSummonSpell.m_newHeroCard.GetActor();
    customSummonSpell.m_swapSpell = SpellManager.Get().GetSpell(customSummonSpell.m_NewHeroFX);
    SpellUtils.SetCustomSpellParent(customSummonSpell.m_swapSpell, (Component) actor);
    customSummonSpell.m_swapSpell.SetSource(customSummonSpell.m_newHeroCard.gameObject);
    customSummonSpell.m_swapSpell.Activate();
    while (!customSummonSpell.m_swapSpell.IsFinished())
      yield return (object) null;
    customSummonSpell.Finish();
    while (customSummonSpell.m_swapSpell.GetActiveState() != SpellStateType.NONE)
      yield return (object) null;
    SpellManager.Get().ReleaseSpell(customSummonSpell.m_swapSpell);
    customSummonSpell.Deactivate();
  }

  private void Finish()
  {
    this.m_newHeroCard.GetActor().TurnOnCollider();
    this.m_newHeroCard.ShowCard();
    this.m_oldHeroCard.TransitionToZone((Zone) null);
    this.OnSpellFinished();
  }

  public static Card GetOldHeroCard(Card hero)
  {
    ZoneHero zone = hero.GetZone() as ZoneHero;
    if ((Object) zone == (Object) null)
    {
      Debug.LogErrorFormat("not in ZoneHero. card: {0}, zone: {1}", (object) hero, (object) hero.GetZone());
      return (Card) null;
    }
    int cardPos = zone.FindCardPos(hero);
    if (cardPos > 1)
      return zone.GetCardAtSlot(cardPos - 1);
    Debug.LogErrorFormat("invalid position. card: {0}, position: {1}", (object) hero, (object) cardPos);
    return (Card) null;
  }

  public static void HideStats(Card hero)
  {
    hero.GetActor().HideArmorSpell();
    hero.GetActor().DisableArmorSpellForTransition();
    hero.GetActor().GetHealthObject().Hide();
    hero.GetActor().GetAttackObject().Hide();
  }
}
