using Hearthstone.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class AdventureDungeonCrawlTreasureOption : AdventureOptionWidget
{
  [CustomEditField(Sections = "Card Spell Types")]
  public List<AdventureDungeonCrawlTreasureOption.SpellDisplayData> MoteInSpells = new List<AdventureDungeonCrawlTreasureOption.SpellDisplayData>()
  {
    new AdventureDungeonCrawlTreasureOption.SpellDisplayData(SpellType.SUMMON_IN_FORGE, Vector3.zero),
    new AdventureDungeonCrawlTreasureOption.SpellDisplayData(SpellType.BURST_RARE, new Vector3(0.2f, 0.2f, 0.2f))
  };
  [CustomEditField(Sections = "Card Spell Types")]
  public List<AdventureDungeonCrawlTreasureOption.SpellDisplayData> MoteOutSpells = new List<AdventureDungeonCrawlTreasureOption.SpellDisplayData>()
  {
    new AdventureDungeonCrawlTreasureOption.SpellDisplayData(SpellType.BURN, new Vector3(0.2f, 0.2f, 0.0f))
  };
  [CustomEditField(Sections = "Card Spell Types")]
  public List<AdventureDungeonCrawlTreasureOption.SpellDisplayData> MoteOutSpellsForSelected = new List<AdventureDungeonCrawlTreasureOption.SpellDisplayData>()
  {
    new AdventureDungeonCrawlTreasureOption.SpellDisplayData(SpellType.SUMMON_OUT_FORGE, new Vector3(0.2f, 0.2f, 0.2f))
  };
  private Hearthstone.UI.Card m_cardWidget;
  private Actor m_cardActor;
  private List<Spell> m_cachedSpells = new List<Spell>();

  [CustomEditField(Sections = "Properties (Read-Only)")]
  public Actor CardActor => this.m_cardActor;

  [CustomEditField(Sections = "Properties (Read-Only)")]
  public override bool IsReady => base.IsReady && (UnityEngine.Object) this.m_cardWidget != (UnityEngine.Object) null && (UnityEngine.Object) this.m_cardActor != (UnityEngine.Object) null;

  [CustomEditField(Sections = "Properties (Read-Only)")]
  public long CardId => this.m_databaseId;

  protected override void OnWidgetInstanceReady(WidgetInstance widgetInstance)
  {
    base.OnWidgetInstanceReady(widgetInstance);
    if ((UnityEngine.Object) this.m_widgetInstance == (UnityEngine.Object) null)
      return;
    this.m_cardWidget = this.m_widgetInstance.GetComponentInChildren<Hearthstone.UI.Card>();
    if ((UnityEngine.Object) this.m_cardWidget != (UnityEngine.Object) null)
      this.m_cardWidget.RegisterCardLoadedListener(new Hearthstone.UI.Card.OnCardActorLoadedDelegate(this.OnCardActorLoaded));
    this.SetVisible(false);
  }

  protected override void OnClickableReady(Clickable clickable)
  {
    base.OnClickableReady(clickable);
    this.m_clickable.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.Rollout()));
  }

  protected override void OnIntroFinished()
  {
    base.OnIntroFinished();
    foreach (UnityEngine.Object cachedSpell in this.m_cachedSpells)
      UnityEngine.Object.Destroy(cachedSpell);
    this.m_cachedSpells.Clear();
  }

  protected override void OnOutroFinished()
  {
    base.OnOutroFinished();
    foreach (UnityEngine.Object cachedSpell in this.m_cachedSpells)
      UnityEngine.Object.Destroy(cachedSpell);
    this.m_cachedSpells.Clear();
  }

  protected override void Rollover()
  {
    base.Rollover();
    if (!((UnityEngine.Object) this.m_cardActor != (UnityEngine.Object) null))
      return;
    this.m_cardActor.SetActorState(ActorStateType.CARD_MOUSE_OVER);
  }

  protected override void Rollout()
  {
    base.Rollout();
    if (!((UnityEngine.Object) this.m_cardActor != (UnityEngine.Object) null))
      return;
    this.m_cardActor.SetActorState(ActorStateType.CARD_IDLE);
  }

  private void OnCardActorLoaded(Actor cardActor)
  {
    this.m_cardActor = cardActor;
    if (!((UnityEngine.Object) this.m_cardActor != (UnityEngine.Object) null))
      return;
    this.SetVisible(this.m_isVisible);
  }

  private void PlaySpells(
    List<AdventureDungeonCrawlTreasureOption.SpellDisplayData> spellTypes)
  {
    if (spellTypes == null || spellTypes.Count <= 0)
      return;
    if ((UnityEngine.Object) this.m_cardWidget == (UnityEngine.Object) null)
      Debug.LogError((object) "AdventureTreasureOption.PlaySpells - m_cardWidget was null!");
    else if ((UnityEngine.Object) this.m_cardActor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "AdventureTreasureOption.PlaySpells - m_cardActor was null!");
    }
    else
    {
      foreach (AdventureDungeonCrawlTreasureOption.SpellDisplayData spellDisplayData in spellTypes.Where<AdventureDungeonCrawlTreasureOption.SpellDisplayData>((Func<AdventureDungeonCrawlTreasureOption.SpellDisplayData, bool>) (r => r.m_SpellType != 0)).ToList<AdventureDungeonCrawlTreasureOption.SpellDisplayData>())
      {
        Spell spell = this.m_cardActor.GetSpell(spellDisplayData.m_SpellType);
        if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
        {
          Debug.LogErrorFormat("AdventureDungeonCrawlTreasureOption.PlaySpells - {0} spell type was null!", (object) spellDisplayData.m_SpellType);
        }
        else
        {
          spell.SetLocalPosition(spellDisplayData.m_RelativeLocation);
          spell.ActivateState(SpellStateType.BIRTH);
          this.m_cachedSpells.Add(spell);
        }
      }
    }
  }

  public void Init(
    long cardDbId,
    bool locked,
    string lockedText,
    bool upgraded,
    bool completed,
    bool newlyUnlocked,
    AdventureOptionWidget.OptionAcknowledgedCallback acknowledgedCallback)
  {
    this.m_databaseId = cardDbId;
    this.InitWidget((string) null, locked, lockedText, upgraded, completed, newlyUnlocked, acknowledgedCallback);
    this.Rollout();
  }

  public override void Select()
  {
    base.Select();
    if (!(this.m_selectedCallback is AdventureDungeonCrawlTreasureOption.TreasureSelectedOptionCallback selectedCallback))
      Log.Adventures.PrintError("Attempting to execute a callback for the AdventureDungeonCrawlTreasureOption, but no callback was provided!");
    else
      selectedCallback(this.m_databaseId);
  }

  public override void PlayIntro()
  {
    base.PlayIntro();
    this.PlaySpells(this.MoteInSpells);
  }

  public override void PlayOutro()
  {
    base.PlayOutro();
    this.Rollout();
    this.PlaySpells(this.m_dataModel.IsSelectedOption ? this.MoteOutSpellsForSelected : this.MoteOutSpells);
  }

  public delegate void TreasureSelectedOptionCallback(long cardDbId);

  [Serializable]
  public class SpellDisplayData
  {
    public SpellType m_SpellType;
    public Vector3 m_RelativeLocation;

    public SpellDisplayData(SpellType spellType, Vector3 position)
    {
      this.m_SpellType = spellType;
      this.m_RelativeLocation = position;
    }
  }
}
