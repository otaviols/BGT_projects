using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class DeckReward : Reward
{
  protected int DeckId;
  protected int ClassId;
  protected string DeckNameOverride;
  private DefLoader.DisposableCardDef m_heroCardDef;
  public UberText deckNameWild;
  public UberText deckNameStandard;
  public GameObject deckFrameWild;
  public GameObject deckFrameStandard;
  public MeshRenderer deckMeshWild;
  public MeshRenderer deckMeshStandard;

  protected override void InitData() => this.SetData((RewardData) new DeckRewardData(0, 0, (string) null), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    if (!(this.Data is DeckRewardData))
    {
      Debug.LogWarning((object) string.Format("SimpleReward.ShowReward() - Data {0} is not SimpleRewardData", (object) this.Data));
    }
    else
    {
      Vector3 localScale = this.m_root.transform.localScale;
      this.m_root.SetActive(true);
      this.m_root.transform.localScale = Vector3.zero;
      iTween.ScaleTo(this.m_root, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
    }
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals || !(this.Data is DeckRewardData data))
      return;
    this.DeckId = data.DeckId;
    this.ClassId = data.ClassId;
    this.DeckNameOverride = data.DeckNameOverride;
    if (GameUtils.DeckIncludesRotatedCards(this.DeckId))
    {
      this.deckFrameStandard.SetActive(false);
      this.deckMeshWild.SetMaterial(this.GetClassMaterial());
    }
    else
    {
      this.deckFrameWild.SetActive(false);
      this.deckMeshStandard.SetMaterial(this.GetClassMaterial());
    }
    this.deckNameWild.Text = this.deckNameStandard.Text = this.GetDeckName();
  }

  private Material GetClassMaterial()
  {
    this.ReleaseCardDef();
    string vanillaHero = CollectionManager.GetVanillaHero((TAG_CLASS) this.ClassId);
    this.m_heroCardDef = DefLoader.Get().GetCardDef(vanillaHero);
    return this.m_heroCardDef.CardDef.GetCustomDeckPortrait();
  }

  private string GetDeckName() => !string.IsNullOrEmpty(this.DeckNameOverride) ? this.DeckNameOverride : (string) GameDbf.Deck.GetRecord(this.DeckId).Name;

  protected override void OnDestroy()
  {
    this.ReleaseCardDef();
    base.OnDestroy();
  }

  private void ReleaseCardDef()
  {
    this.m_heroCardDef?.Dispose();
    this.m_heroCardDef = (DefLoader.DisposableCardDef) null;
  }
}
