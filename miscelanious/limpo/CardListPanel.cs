using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class CardListPanel : MonoBehaviour
{
  [CustomEditField(Sections = "Object Links")]
  public NestedPrefab m_leftArrowNested;
  [CustomEditField(Sections = "Object Links")]
  public NestedPrefab m_rightArrowNested;
  [SerializeField]
  private float m_CardSpacing = 2.3f;
  private UIBButton m_leftArrow;
  private UIBButton m_rightArrow;
  private const int MAX_CARDS_PER_PAGE = 3;
  private int m_numPages = 1;
  private int m_pageNum;
  private List<int> m_cards = new List<int>();
  private List<Actor> m_cardActors = new List<Actor>();

  [CustomEditField(Sections = "Variables")]
  public float CardSpacing
  {
    get => this.m_CardSpacing;
    set
    {
      this.m_CardSpacing = value;
      this.UpdateCardPositions();
    }
  }

  private void Awake()
  {
    this.m_leftArrowNested.gameObject.SetActive(false);
    this.m_rightArrowNested.gameObject.SetActive(false);
  }

  public void Show(List<int> cards)
  {
    if (cards != null)
      this.m_cards = cards;
    this.SetupPagingArrows();
    this.m_numPages = (this.m_cards.Count + 3 - 1) / 3;
    this.ShowPage(0);
  }

  private void ShowPage(int pageNum)
  {
    if (pageNum < 0 || pageNum >= this.m_numPages)
    {
      Log.All.PrintWarning("CardListPanel.ShowPage: attempting to show invalid pageNum=" + (object) pageNum + " numPages=" + (object) this.m_numPages);
    }
    else
    {
      this.m_pageNum = pageNum;
      this.StopCoroutine("TransitionPage");
      this.StartCoroutine("TransitionPage");
    }
  }

  private IEnumerator TransitionPage()
  {
    CardListPanel cardListPanel = this;
    if ((Object) cardListPanel.m_leftArrow != (Object) null)
      cardListPanel.m_leftArrow.gameObject.SetActive(false);
    if ((Object) cardListPanel.m_rightArrow != (Object) null)
      cardListPanel.m_rightArrow.gameObject.SetActive(false);
    List<Spell> spellList = new List<Spell>();
    foreach (Component cardActor in cardListPanel.m_cardActors)
      Object.Destroy((Object) cardActor.gameObject);
    cardListPanel.m_cardActors.Clear();
    spellList.Clear();
    int num1 = cardListPanel.m_pageNum * 3;
    int num2 = Mathf.Min(3, cardListPanel.m_cards.Count - num1);
    for (int index = 0; index < num2; ++index)
    {
      int card = cardListPanel.m_cards[num1 + index];
      using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(GameUtils.TranslateDbIdToCardId(card)))
      {
        Actor component1 = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(fullDef.EntityDef, TAG_PREMIUM.NORMAL), AssetLoadingOptions.IgnorePrefabPosition).GetComponent<Actor>();
        component1.SetCardDef(fullDef.DisposableCardDef);
        component1.SetEntityDef(fullDef.EntityDef);
        GameUtils.SetParent((Component) component1, cardListPanel.gameObject);
        LayerUtils.SetLayer((Component) component1, cardListPanel.gameObject.layer);
        List<CardChangeDbfRecord> cardChangeRecords = GameDbf.GetIndex().GetCardChangeRecords(card);
        GameObject gameObject = (GameObject) null;
        switch (fullDef.EntityDef.GetCardType())
        {
          case TAG_CARDTYPE.HERO:
            gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hand_Hero_NerfGlows.prefab:6f101676067a4514f8641429c0592adc");
            break;
          case TAG_CARDTYPE.MINION:
            gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hand_Ally_NerfGlows.prefab:a693fa02720fcb644b3223d7d75d26eb");
            break;
          case TAG_CARDTYPE.SPELL:
            gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hand_Ability_NerfGlows.prefab:adb8690f5caa2a84eb9431b8f09664db");
            break;
          case TAG_CARDTYPE.WEAPON:
            gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hand_Weapon_NerfGlows.prefab:645b0cbf4d3be464a8e4fe447f6a0dee");
            break;
          case TAG_CARDTYPE.LOCATION:
            gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hand_Location_NerfGlows.prefab:32299cf99a8ea8541b06329fb1961c71");
            break;
        }
        if ((Object) gameObject != (Object) null)
        {
          CardNerfGlows component2 = gameObject.GetComponent<CardNerfGlows>();
          if ((Object) component2 != (Object) null)
          {
            TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, component1.transform);
            LayerUtils.SetLayer((Component) component2, component1.gameObject.layer);
            component2.SetGlowsForCard(cardChangeRecords);
          }
          else
            Debug.LogError((object) ("CardListPanel.cs: Nerf Glows GameObject " + (object) gameObject + " does not have a CardNerfGlows script attached."));
        }
        cardListPanel.m_cardActors.Add(component1);
      }
    }
    cardListPanel.UpdateCardPositions();
    foreach (Actor cardActor in cardListPanel.m_cardActors)
    {
      spellList.Add(cardActor.ActivateSpellBirthState(SpellType.DEATHREVERSE));
      cardActor.ContactShadow(true);
    }
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_move_invalid_or_click.prefab:777caa6f44f027747a03f3d85bcc897c");
    yield return (object) new WaitForSeconds(0.2f);
    if ((Object) cardListPanel.m_leftArrow != (Object) null)
      cardListPanel.m_leftArrow.gameObject.SetActive(cardListPanel.m_pageNum != 0);
    if ((Object) cardListPanel.m_rightArrow != (Object) null)
      cardListPanel.m_rightArrow.gameObject.SetActive(cardListPanel.m_pageNum < cardListPanel.m_numPages - 1);
  }

  private void UpdateCardPositions()
  {
    int count = this.m_cardActors.Count;
    for (int index = 0; index < count; ++index)
    {
      Actor cardActor = this.m_cardActors[index];
      Vector3 zero = Vector3.zero;
      float num = ((float) index - (float) (count - 1) / 2f) * this.m_CardSpacing;
      zero.x += num;
      cardActor.transform.localPosition = zero;
    }
  }

  private void SetupPagingArrows()
  {
    if (this.m_cards.Count > 3)
    {
      this.m_leftArrowNested.gameObject.SetActive(true);
      this.m_rightArrowNested.gameObject.SetActive(true);
      GameObject go1 = this.m_leftArrowNested.PrefabGameObject();
      LayerUtils.SetLayer(go1, this.m_leftArrowNested.gameObject.layer);
      this.m_leftArrow = go1.GetComponent<UIBButton>();
      this.m_leftArrow.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.TurnPage(false)));
      GameObject go2 = this.m_rightArrowNested.PrefabGameObject();
      LayerUtils.SetLayer(go2, this.m_rightArrowNested.gameObject.layer);
      this.m_rightArrow = go2.GetComponent<UIBButton>();
      this.m_rightArrow.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.TurnPage(true)));
      HighlightState componentInChildren = this.m_rightArrow.GetComponentInChildren<HighlightState>();
      if (!(bool) (Object) componentInChildren)
        return;
      componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    }
    else
    {
      this.m_leftArrowNested.gameObject.SetActive(false);
      this.m_rightArrowNested.gameObject.SetActive(false);
    }
  }

  private void TurnPage(bool right)
  {
    HighlightState componentInChildren = this.m_rightArrow.GetComponentInChildren<HighlightState>();
    if ((bool) (Object) componentInChildren)
      componentInChildren.ChangeState(ActorStateType.NONE);
    this.ShowPage(this.m_pageNum + (right ? 1 : -1));
  }
}
