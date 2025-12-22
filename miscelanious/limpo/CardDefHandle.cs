using UnityEngine;

public class CardDefHandle
{
  private string m_cardId;
  private DefLoader.DisposableCardDef m_cardDef;

  public void SetCardId(string cardId) => this.m_cardId = cardId;

  public void Set(CardDefHandle other)
  {
    this.m_cardId = other?.m_cardId;
    this.SetCardDef(other?.m_cardDef);
  }

  public bool SetCardDef(DefLoader.DisposableCardDef def)
  {
    if (!((Object) def?.CardDef != (Object) this.m_cardDef?.CardDef))
      return false;
    this.ReleaseCardDef();
    this.m_cardDef = def?.Share();
    return true;
  }

  public DefLoader.DisposableCardDef Share()
  {
    if (this.m_cardDef == null)
      this.m_cardDef = DefLoader.Get()?.GetCardDef(this.m_cardId);
    return this.m_cardDef?.Share();
  }

  public CardDef Get(TAG_PREMIUM premiumType)
  {
    if (this.m_cardDef == null)
      this.m_cardDef = DefLoader.Get()?.GetCardDef(this.m_cardId, premiumType);
    return this.m_cardDef?.CardDef;
  }

  public void ReleaseCardDef()
  {
    this.m_cardDef?.Dispose();
    this.m_cardDef = (DefLoader.DisposableCardDef) null;
  }
}
