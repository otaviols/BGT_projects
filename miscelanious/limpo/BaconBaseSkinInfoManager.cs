using Hearthstone;
using System.Text;
using UnityEngine;

public abstract class BaconBaseSkinInfoManager : BaseHeroSkinInfoManager
{
  public GameObject m_DebugTextWrapper;
  public UberText m_DebugText;

  public override void EnterPreview(CollectionCardVisual cardVisual)
  {
    base.EnterPreview(cardVisual);
    if (!((Object) this.m_DebugTextWrapper != (Object) null))
      return;
    if (HearthstoneApplication.IsInternal() && (Object) this.m_DebugText != (Object) null && Options.Get().GetBool(Option.DEBUG_SHOW_BATTLEGROUND_SKIN_IDS))
    {
      StringBuilder builder = new StringBuilder();
      this.AppendDebugTextForCurrentCard(builder);
      this.m_DebugText.Text = builder.ToString();
      this.m_DebugTextWrapper.SetActive(true);
    }
    else
      this.m_DebugTextWrapper.SetActive(false);
  }

  protected virtual void AppendDebugTextForCurrentCard(StringBuilder builder)
  {
    builder.Append("Card Id: ");
    if (this.m_currentEntityDef != null)
    {
      builder.AppendLine();
      builder.Append(this.m_currentEntityDef.GetCardId());
      builder.AppendLine();
    }
    else
      builder.Append("UNKNOWN");
    builder.AppendLine();
  }
}
