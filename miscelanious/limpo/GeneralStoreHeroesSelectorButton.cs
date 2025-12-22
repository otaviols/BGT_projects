using UnityEngine;

[CustomEditClass]
public class GeneralStoreHeroesSelectorButton : PegUIElement
{
  public Actor m_heroActor;
  public UberText m_heroName;
  public HighlightState m_highlight;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_selectSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_unselectSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_mouseOverSound;
  private string m_heroId;
  private int m_sortOrder;
  private bool m_selected;
  private bool m_purchased;
  private DefLoader.DisposableFullDef m_currentDef;
  private CardHeroDbfRecord m_cardHero;

  protected override void Awake()
  {
    base.Awake();
    if ((bool) UniversalInputManager.UsePhoneUI)
      OverlayUI.Get().AddGameObject(this.gameObject, scaleMode: CanvasScaleMode.WIDTH);
    else
      OverlayUI.Get().AddGameObject(this.gameObject);
  }

  protected override void OnDestroy()
  {
    this.m_currentDef?.Dispose();
    this.m_currentDef = (DefLoader.DisposableFullDef) null;
    base.OnDestroy();
  }

  public int GetHeroDbId() => this.m_cardHero.ID;

  public string GetHeroCardMiniGuid() => GameUtils.TranslateDbIdToCardId(this.m_cardHero.CardId);

  public int GetHeroCardDbId() => GameUtils.TranslateCardIdToDbId(this.GetHeroCardMiniGuid());

  public void SetCardHeroDbfRecord(CardHeroDbfRecord cardHero) => this.m_cardHero = cardHero;

  public CardHeroDbfRecord GetCardHeroDbfRecord() => this.m_cardHero;

  public void SetSortOrder(int sortOrder) => this.m_sortOrder = sortOrder;

  public int GetSortOrder() => this.m_sortOrder;

  public void SetPurchased(bool purchased) => this.m_purchased = purchased;

  public bool GetPurchased() => this.m_purchased;

  public void UpdatePortrait(GeneralStoreHeroesSelectorButton rhs) => this.UpdatePortrait(rhs.m_currentDef);

  public void UpdatePortrait(DefLoader.DisposableFullDef def)
  {
    this.m_heroActor.SetFullDef(def);
    this.m_heroActor.UpdateAllComponents();
    this.m_heroActor.SetUnlit();
    this.m_currentDef?.Dispose();
    this.m_currentDef = def?.Share();
  }

  public void UpdateName(GeneralStoreHeroesSelectorButton rhs) => this.UpdateName(rhs.m_heroName.Text);

  public void UpdateName(string name)
  {
    if (!((Object) this.m_heroName != (Object) null))
      return;
    this.m_heroName.Text = name;
  }

  public void Select()
  {
    if (this.m_selected)
      return;
    this.m_selected = true;
    this.m_highlight.ChangeState(this.GetInteractionState() == PegUIElement.InteractionState.Up ? ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE : ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    if (string.IsNullOrEmpty(this.m_selectSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_selectSound);
  }

  public void Unselect()
  {
    if (!this.m_selected)
      return;
    this.m_selected = false;
    this.m_highlight.ChangeState(ActorStateType.NONE);
    if (string.IsNullOrEmpty(this.m_unselectSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_unselectSound);
  }

  public bool IsAvailable() => true;

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    base.OnOver(oldState);
    if (!((Object) this.m_highlight != (Object) null) || !this.IsAvailable())
      return;
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
    if (string.IsNullOrEmpty(this.m_mouseOverSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_mouseOverSound);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    base.OnOut(oldState);
    if (!((Object) this.m_highlight != (Object) null) || !this.IsAvailable())
      return;
    this.m_highlight.ChangeState(this.m_selected ? ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE : ActorStateType.NONE);
  }

  protected override void OnRelease()
  {
    base.OnRelease();
    if (!((Object) this.m_highlight != (Object) null) || !this.IsAvailable())
      return;
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
  }

  protected override void OnPress()
  {
    base.OnPress();
    if (!((Object) this.m_highlight != (Object) null) || !this.IsAvailable())
      return;
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
  }
}
