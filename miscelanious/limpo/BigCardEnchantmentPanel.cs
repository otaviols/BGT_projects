using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class BigCardEnchantmentPanel : MonoBehaviour
{
  public Actor m_Actor;
  public UberText m_HeaderText;
  public UberText m_BodyText;
  public GameObject m_Background;
  public Material m_FallbackEnchantmentPortrait;
  private Entity m_enchantment;
  private DefLoader.DisposableCardDef m_enchantmentCardDef;
  private DefLoader.DisposableCardDef m_creatorCardDef;
  private Vector3 m_initialScale;
  private float m_initialBackgroundHeight;
  private Vector3 m_initialBackgroundScale;
  private bool m_shown;
  private int m_multiplier = 1;
  private string m_header = "";

  private void Awake()
  {
    this.m_initialScale = this.transform.localScale;
    this.m_initialBackgroundHeight = this.m_Background.GetComponentInChildren<MeshRenderer>().bounds.size.z;
    this.m_initialBackgroundScale = this.m_Background.transform.localScale;
  }

  private void OnDestroy()
  {
    this.m_enchantmentCardDef?.Dispose();
    this.m_enchantmentCardDef = (DefLoader.DisposableCardDef) null;
    this.m_creatorCardDef?.Dispose();
    this.m_creatorCardDef = (DefLoader.DisposableCardDef) null;
  }

  public void SetEnchantment(Entity enchantment)
  {
    this.m_enchantment = enchantment;
    string cardId = this.m_enchantment.GetCardId();
    DefLoader.Get().LoadCardDef(cardId, new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnEnchantmentCardDefLoaded), quality: new CardPortraitQuality(1, this.m_enchantment.GetPremiumType()));
  }

  public void Show()
  {
    if (this.m_shown)
      return;
    this.m_shown = true;
    this.gameObject.SetActive(true);
    this.UpdateLayout();
  }

  public void Hide()
  {
    if (!this.m_shown)
      return;
    this.m_shown = false;
    this.gameObject.SetActive(false);
  }

  public void ResetScale()
  {
    this.transform.localScale = this.m_initialScale;
    this.m_Background.transform.localScale = this.m_initialBackgroundScale;
  }

  public bool IsShown() => this.m_shown;

  public float GetHeight() => this.m_Background.GetComponentInChildren<MeshRenderer>().bounds.size.z;

  private void OnEnchantmentCardDefLoaded(
    string cardId,
    DefLoader.DisposableCardDef cardDef,
    object callbackData)
  {
    bool flag = false;
    if (cardDef != null)
    {
      this.m_enchantmentCardDef?.Dispose();
      this.m_enchantmentCardDef = cardDef;
      Material enchantmentPortraitMat;
      if (this.m_enchantmentCardDef.CardDef.TryGetEnchantmentPortrait(out enchantmentPortraitMat))
      {
        this.m_Actor.GetMeshRenderer().SetMaterial(enchantmentPortraitMat);
        flag = true;
      }
      else
      {
        Material fullHistoryTileMat;
        if (this.m_enchantmentCardDef.CardDef.TryGetHistoryTileFullPortrait(this.m_Actor.GetPremium(), out fullHistoryTileMat))
        {
          this.m_Actor.GetMeshRenderer().SetMaterial(fullHistoryTileMat);
          flag = true;
        }
        else
        {
          Texture portraitTexture;
          if (this.m_enchantmentCardDef.CardDef.TryGetPortraitTexture(this.m_Actor.GetPremium(), out portraitTexture))
          {
            this.m_Actor.SetPortraitTextureOverride(portraitTexture);
            flag = true;
          }
        }
      }
    }
    this.m_HeaderText.Text = this.m_enchantment.GetName();
    this.m_header = this.m_enchantment.GetName();
    this.SetMultiplier(Mathf.Max(this.m_enchantment.GetTag(GAME_TAG.SPAWN_TIME_COUNT), 1));
    this.m_BodyText.Text = this.m_enchantment.GetCardTextInHand();
    if (flag)
      return;
    this.LoadCreatorCardDef();
  }

  private void LoadCreatorCardDef()
  {
    if (this.m_enchantment == null)
      return;
    string cardIdForPortrait = this.m_enchantment.GetEnchantmentCreatorCardIDForPortrait();
    if (string.IsNullOrEmpty(cardIdForPortrait))
      this.m_Actor.GetMeshRenderer().SetMaterial(this.m_FallbackEnchantmentPortrait);
    else
      DefLoader.Get().LoadCardDef(cardIdForPortrait, new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnCreatorCardDefLoaded), quality: new CardPortraitQuality(1, this.m_enchantment.GetPremiumType()));
  }

  private void OnCreatorCardDefLoaded(
    string cardId,
    DefLoader.DisposableCardDef cardDef,
    object callbackData)
  {
    if (cardDef == null)
      return;
    this.m_creatorCardDef?.Dispose();
    this.m_creatorCardDef = cardDef;
    Material enchantmentPortraitMat;
    if (this.m_creatorCardDef.CardDef.TryGetEnchantmentPortrait(out enchantmentPortraitMat))
    {
      this.m_Actor.GetMeshRenderer().SetMaterial(enchantmentPortraitMat);
    }
    else
    {
      Material fullHistoryTileMat;
      if (this.m_creatorCardDef.CardDef.TryGetHistoryTileFullPortrait(this.m_Actor.GetPremium(), out fullHistoryTileMat))
      {
        this.m_Actor.GetMeshRenderer().SetMaterial(fullHistoryTileMat);
      }
      else
      {
        if (!((Object) this.m_creatorCardDef.CardDef.GetPortraitTexture(this.m_Actor.GetPremium()) != (Object) null))
          return;
        this.m_Actor.SetPortraitTextureOverride(this.m_creatorCardDef.CardDef.GetPortraitTexture(this.m_Actor.GetPremium()));
      }
    }
  }

  private void UpdateLayout()
  {
    this.m_HeaderText.UpdateNow();
    this.m_BodyText.UpdateNow();
    Bounds bounds = this.m_Actor.GetMeshRenderer().bounds;
    Bounds worldSpaceBounds1 = this.m_HeaderText.GetTextWorldSpaceBounds();
    Bounds worldSpaceBounds2 = this.m_BodyText.GetTextWorldSpaceBounds();
    double z1 = (double) bounds.min.z;
    float z2 = bounds.max.z;
    float z3 = worldSpaceBounds1.min.z;
    float z4 = worldSpaceBounds1.max.z;
    float z5 = worldSpaceBounds2.min.z;
    float z6 = worldSpaceBounds2.max.z;
    double b = (double) z3;
    float num1 = Mathf.Min(Mathf.Min((float) z1, (float) b), z5);
    float num2 = (float) ((double) Mathf.Max(Mathf.Max(z2, z4), z6) - (double) num1 + 0.100000001490116);
    this.transform.localScale = this.m_initialScale;
    this.transform.localEulerAngles = Vector3.zero;
    TransformUtil.SetLocalScaleZ(this.m_Background, this.m_initialBackgroundScale.z * (num2 / this.m_initialBackgroundHeight));
  }

  public string GetEnchantmentId() => this.m_enchantment == null ? (string) null : this.m_enchantment.GetCardId();

  public Entity GetEnchantment() => this.m_enchantment;

  public void IncrementEnchantmentMultiplier(uint amount = 1) => this.SetMultiplier(this.m_multiplier + (int) amount);

  public void SetMultiplier(int multiplier)
  {
    this.m_multiplier = multiplier;
    if (this.m_multiplier > 1)
      this.m_HeaderText.Text = GameStrings.Format("GAMEPLAY_ENCHANTMENT_MULTIPLIER_HEADER", (object) this.m_multiplier, (object) this.m_header);
    else
      this.m_HeaderText.Text = this.m_header;
  }
}
