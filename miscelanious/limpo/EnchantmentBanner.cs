using Blizzard.T5.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentBanner : MonoBehaviour
{
  public GameObject m_EnchantmentBanner;
  public GameObject m_EnchantmentBannerBottom;
  public UberText m_EnchantmentBannerText;
  public int m_RenderQueueEnchantmentPanel;
  private float m_initialBannerHeight;
  private Vector3 m_initialBannerScale;
  private Vector3 m_initialBannerBottomScale;
  private Vector3 m_initialBannerTextScale;
  private readonly Pool<BigCardEnchantmentPanel> m_enchantmentPool = new Pool<BigCardEnchantmentPanel>();
  private Map<Tuple<string, string>, BigCardEnchantmentPanel> m_uniqueEnchantmentLookup = new Map<Tuple<string, string>, BigCardEnchantmentPanel>();
  private readonly PlatformDependentValue<float> ENCHANTMENT_SCALING_FACTOR = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 1f,
    Tablet = 1f,
    Phone = 1.5f,
    MiniTablet = 1f
  };

  public void Awake()
  {
    this.m_initialBannerHeight = this.m_EnchantmentBanner.GetComponent<Renderer>().bounds.size.z;
    this.m_initialBannerScale = this.m_EnchantmentBanner.transform.localScale;
    this.m_initialBannerBottomScale = this.m_EnchantmentBannerBottom.transform.localScale;
    this.m_initialBannerTextScale = this.m_EnchantmentBannerText.transform.localScale;
    this.m_enchantmentPool.SetCreateItemCallback(new Pool<BigCardEnchantmentPanel>.CreateItemCallback(this.CreateEnchantmentPanel));
    this.m_enchantmentPool.SetDestroyItemCallback(new Pool<BigCardEnchantmentPanel>.DestroyItemCallback(this.DestroyEnchantmentPanel));
    this.m_enchantmentPool.SetExtensionCount(1);
    this.m_enchantmentPool.SetMaxReleasedItemCount(2);
    this.ResetEnchantments();
  }

  private BigCardEnchantmentPanel CreateEnchantmentPanel(int index)
  {
    BigCardEnchantmentPanel component = AssetLoader.Get().InstantiatePrefab((AssetReference) "BigCardEnchantmentPanel.prefab:5af69938cd435a5488e4c9a7b8070e6e").GetComponent<BigCardEnchantmentPanel>();
    component.name = string.Format("{0}{1}", (object) "BigCardEnchantmentPanel", (object) index);
    RenderUtils.SetRenderQueue(component.gameObject, this.m_RenderQueueEnchantmentPanel);
    return component;
  }

  private void DestroyEnchantmentPanel(BigCardEnchantmentPanel panel) => UnityEngine.Object.Destroy((UnityEngine.Object) panel.gameObject);

  public void UpdateEnchantments(Card card, Actor bigCardActor, float enchantmentScalingFactor = 1f)
  {
    this.ResetEnchantments();
    GameObject bone = bigCardActor.FindBone("EnchantmentTooltip");
    if ((UnityEngine.Object) bone == (UnityEngine.Object) null)
      return;
    Entity entity = card.GetEntity();
    bool unique = GameState.Get() != null && GameState.Get().GetGameEntity() != null && GameState.Get().GetBooleanGameOption(GameEntityOption.USE_COMPACT_ENCHANTMENT_BANNERS);
    List<Entity> displayedEnchantments = entity.GetDisplayedEnchantments(unique);
    List<BigCardEnchantmentPanel> activeList = this.m_enchantmentPool.GetActiveList();
    int count1 = displayedEnchantments.Count;
    if (count1 == 0 && !entity.HasTag(GAME_TAG.ENCHANTMENT_BANNER_TEXT) && !entity.IsSideQuest() && !entity.IsObjective())
      return;
    this.m_uniqueEnchantmentLookup.Clear();
    int count2 = activeList.Count;
    int count3 = count1 - count2;
    if (count3 > 0)
      this.m_enchantmentPool.AcquireBatch(count3);
    else if (count3 < 0)
      this.m_enchantmentPool.ReleaseBatch(count1, -count3);
    for (int index = 0; index < activeList.Count; ++index)
    {
      BigCardEnchantmentPanel enchantmentPanel = activeList[index];
      Entity enchantment = displayedEnchantments[index];
      enchantmentPanel.SetEnchantment(enchantment);
      if (unique)
        this.m_uniqueEnchantmentLookup.Add(new Tuple<string, string>(enchantment.GetCardId(), enchantment.GetCardTextInHand()), enchantmentPanel);
    }
    if (unique)
    {
      HashSet<Tuple<string, string>> tupleSet = new HashSet<Tuple<string, string>>();
      foreach (Entity displayedEnchantment in entity.GetDisplayedEnchantments())
      {
        Tuple<string, string> key = new Tuple<string, string>(displayedEnchantment.GetCardId(), displayedEnchantment.GetCardTextInHand());
        if (!tupleSet.Contains(key))
        {
          tupleSet.Add(key);
        }
        else
        {
          uint amount = (uint) Mathf.Max(displayedEnchantment.GetTag(GAME_TAG.SPAWN_TIME_COUNT), 1);
          this.m_uniqueEnchantmentLookup[key].IncrementEnchantmentMultiplier(amount);
        }
      }
    }
    this.LayoutEnchantments(bone, card, bigCardActor, enchantmentScalingFactor);
    LayerUtils.SetLayer(bone, GameLayer.Tooltip);
  }

  public void ResetEnchantments()
  {
    this.m_EnchantmentBanner.SetActive(false);
    this.m_EnchantmentBannerBottom.SetActive(false);
    this.m_EnchantmentBannerText.gameObject.SetActive(false);
    this.m_EnchantmentBanner.transform.parent = this.transform;
    this.m_EnchantmentBannerBottom.transform.parent = this.transform;
    this.m_EnchantmentBannerText.transform.parent = this.transform;
    foreach (BigCardEnchantmentPanel active in this.m_enchantmentPool.GetActiveList())
    {
      active.transform.parent = this.transform;
      active.ResetScale();
      active.Hide();
    }
  }

  private void LayoutEnchantments(
    GameObject bone,
    Card card,
    Actor bigCardActor,
    float enchantmentScalingFactor)
  {
    float adjustedScalingFactor = enchantmentScalingFactor * (float) BigCard.Get().GetPlatformScalingFactor() * (float) this.ENCHANTMENT_SCALING_FACTOR;
    GameObject relative = (UnityEngine.Object) bigCardActor.m_enchantmentBannerAnchorObject == (UnityEngine.Object) null ? bigCardActor.GetMeshRenderer().gameObject : bigCardActor.m_enchantmentBannerAnchorObject;
    float num1 = 0.1f;
    List<BigCardEnchantmentPanel> activeList = this.m_enchantmentPool.GetActiveList();
    BigCardEnchantmentPanel prevPanel = (BigCardEnchantmentPanel) null;
    foreach (BigCardEnchantmentPanel enchantmentPanel in activeList)
    {
      enchantmentPanel.Show();
      enchantmentPanel.transform.localScale *= adjustedScalingFactor;
      if ((UnityEngine.Object) prevPanel == (UnityEngine.Object) null)
        TransformUtil.SetPoint(enchantmentPanel.gameObject, new Vector3(0.5f, 0.0f, 1f), relative, new Vector3(0.5f, 0.0f, 0.0f), new Vector3(0.01f, 0.01f, 0.0f));
      else
        TransformUtil.SetPoint(enchantmentPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), prevPanel.gameObject, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));
      prevPanel = enchantmentPanel;
      enchantmentPanel.transform.parent = bone.transform;
      float height = enchantmentPanel.GetHeight();
      num1 += height;
    }
    Entity entity = card.GetEntity();
    if (entity != null && entity.HasTag(GAME_TAG.ENCHANTMENT_BANNER_TEXT))
    {
      string clientString = GameDbf.GetIndex().GetClientString(entity.GetTag(GAME_TAG.ENCHANTMENT_BANNER_TEXT));
      this.UpdateEnchantmentBannerText(bone, prevPanel, clientString, adjustedScalingFactor);
      num1 += this.m_EnchantmentBannerText.Height;
    }
    else if (entity != null && entity.IsSideQuest())
    {
      string customBannerTextString = GameStrings.Format("GLUE_SIDEQUEST_PROGRESS_BANNER", (object) entity.GetTag(GAME_TAG.QUEST_PROGRESS), (object) entity.GetTag(GAME_TAG.QUEST_PROGRESS_TOTAL));
      this.UpdateEnchantmentBannerText(bone, prevPanel, customBannerTextString, adjustedScalingFactor);
      num1 += this.m_EnchantmentBannerText.Height;
    }
    else if (entity != null && entity.IsObjective())
    {
      int num2 = entity.GetTag(GAME_TAG.QUEST_PROGRESS_TOTAL) - entity.GetTag(GAME_TAG.QUEST_PROGRESS);
      string customBannerTextString;
      if (num2 == 1)
        customBannerTextString = GameStrings.Format("GLUE_OBJECTIVES_BANNER_FINAL_TURN", (object) num2);
      else
        customBannerTextString = GameStrings.Format("GLUE_OBJECTIVES_BANNER", (object) num2);
      this.UpdateEnchantmentBannerText(bone, prevPanel, customBannerTextString, adjustedScalingFactor);
      num1 += this.m_EnchantmentBannerText.Height;
    }
    else
      this.m_EnchantmentBannerText.gameObject.SetActive(false);
    this.m_EnchantmentBanner.SetActive(true);
    this.m_EnchantmentBannerBottom.SetActive(true);
    this.m_EnchantmentBannerBottom.transform.localScale = this.m_initialBannerBottomScale * adjustedScalingFactor;
    this.m_EnchantmentBanner.transform.localEulerAngles = new Vector3(0.0f, 180f, 0.0f);
    this.m_EnchantmentBannerBottom.transform.localEulerAngles = new Vector3(0.0f, 180f, 0.0f);
    TransformUtil.SetPoint(this.m_EnchantmentBanner, new Vector3(0.5f, 0.0f, 1f), relative, new Vector3(0.5f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.2f));
    this.m_EnchantmentBanner.transform.localScale = this.m_initialBannerScale * adjustedScalingFactor;
    TransformUtil.SetLocalScaleZ(this.m_EnchantmentBanner.gameObject, num1 / this.m_initialBannerHeight / this.m_initialBannerScale.z);
    this.m_EnchantmentBanner.transform.parent = bone.transform;
    TransformUtil.SetPoint(this.m_EnchantmentBannerBottom, Anchor.FRONT, this.m_EnchantmentBanner, Anchor.BACK);
    this.m_EnchantmentBannerBottom.transform.parent = bone.transform;
    this.m_EnchantmentBannerBottom.transform.position += new Vector3(0.0f, -0.01f, 0.01f);
  }

  private void UpdateEnchantmentBannerText(
    GameObject bone,
    BigCardEnchantmentPanel prevPanel,
    string customBannerTextString,
    float adjustedScalingFactor)
  {
    this.m_EnchantmentBannerText.transform.localScale = this.m_initialBannerTextScale * adjustedScalingFactor;
    this.m_EnchantmentBannerText.transform.parent = bone.transform;
    if ((UnityEngine.Object) prevPanel == (UnityEngine.Object) null)
      this.m_EnchantmentBannerText.transform.localPosition = new Vector3(0.0f, 0.0f, -0.25f);
    else
      TransformUtil.SetPoint(this.m_EnchantmentBannerText.gameObject, new Vector3(0.5f, 0.0f, 1f), prevPanel.gameObject, new Vector3(0.5f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -0.05f));
    this.m_EnchantmentBannerText.gameObject.SetActive(true);
    this.m_EnchantmentBannerText.Text = customBannerTextString;
  }

  public bool IsBannerVisible() => this.m_EnchantmentBanner.activeInHierarchy;

  public int GetEnchantmentCount() => this.m_enchantmentPool.GetActiveList().Count;

  public Bounds GetLowerMeshBounds() => this.m_EnchantmentBannerBottom.GetComponent<Renderer>().bounds;
}
