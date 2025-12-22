using UnityEngine;

public class CardTypeBanner : MonoBehaviour
{
  public GameObject m_root;
  public UberText m_text;
  public GameObject m_banner;
  private static CardTypeBanner s_instance;
  private Card m_card;
  private readonly Color MINION_COLOR = new Color(0.1529412f, 0.1254902f, 0.03529412f);
  private readonly Color HERO_COLOR = new Color(0.1529412f, 0.1254902f, 0.03529412f);
  private readonly Color SPELL_COLOR = new Color(0.8745098f, 0.7882353f, 0.5254902f);
  private readonly Color REWARD_COLOR = new Color(0.8745098f, 0.7882353f, 0.5254902f);
  private readonly Color BACON_HEROBUDDY_COLOR = new Color(0.8745098f, 0.7882353f, 0.5254902f);
  private readonly Color WEAPON_COLOR = new Color(0.8745098f, 0.7882353f, 0.5254902f);
  private readonly Color LOCATION_COLOR = new Color(0.8745098f, 0.7882353f, 0.5254902f);

  private void Awake() => CardTypeBanner.s_instance = this;

  private void OnDestroy() => CardTypeBanner.s_instance = (CardTypeBanner) null;

  private void Update()
  {
    if (!((Object) this.m_card != (Object) null))
      return;
    if (this.m_card.GetActor().IsShown())
      this.UpdatePosition();
    else
      this.Hide();
  }

  public static CardTypeBanner Get() => CardTypeBanner.s_instance;

  public bool IsShown() => (bool) (Object) this.m_card;

  public void Show(Card card)
  {
    GameEntityOptions gameOptions = GameState.Get()?.GetGameEntity()?.GetGameOptions();
    if (gameOptions != null && gameOptions.GetBooleanOption(GameEntityOption.DISABLE_CARD_TYPE_BANNER))
      return;
    this.m_card = card;
    this.ShowImpl();
  }

  public void Hide()
  {
    this.m_card = (Card) null;
    this.HideImpl();
  }

  public void Hide(Card card)
  {
    if (!((Object) this.m_card == (Object) card))
      return;
    this.Hide();
  }

  public DefLoader.DisposableCardDef ShareDisposableCardDef() => this.m_card?.ShareDisposableCardDef();

  private void ShowImpl()
  {
    this.m_root.gameObject.SetActive(true);
    TAG_CARDTYPE cardType = this.m_card.GetEntity().GetCardType();
    this.m_text.gameObject.SetActive(true);
    this.m_text.Text = GameStrings.GetCardTypeName(cardType);
    switch (cardType)
    {
      case TAG_CARDTYPE.HERO:
        this.m_text.TextColor = this.HERO_COLOR;
        break;
      case TAG_CARDTYPE.MINION:
        this.m_text.TextColor = this.MINION_COLOR;
        break;
      case TAG_CARDTYPE.SPELL:
        this.m_text.TextColor = this.SPELL_COLOR;
        break;
      case TAG_CARDTYPE.WEAPON:
        this.m_text.TextColor = this.WEAPON_COLOR;
        break;
      case TAG_CARDTYPE.BATTLEGROUND_HERO_BUDDY:
        this.m_text.TextColor = this.BACON_HEROBUDDY_COLOR;
        break;
      case TAG_CARDTYPE.LOCATION:
        this.m_text.TextColor = this.LOCATION_COLOR;
        break;
      case TAG_CARDTYPE.BATTLEGROUND_QUEST_REWARD:
        this.m_text.TextColor = this.REWARD_COLOR;
        break;
    }
    this.m_banner.SetActive(true);
    this.UpdatePosition();
  }

  private void HideImpl() => this.m_root.gameObject.SetActive(false);

  private void UpdatePosition() => this.m_root.transform.position = this.m_card.GetActor().GetCardTypeBannerAnchor().transform.position;

  public bool HasCardDef => (Object) this.m_card != (Object) null && this.m_card.HasCardDef;

  public bool HasSameCardDef(CardDef cardDef) => (Object) this.m_card != (Object) null && this.m_card.HasSameCardDef(cardDef);
}
