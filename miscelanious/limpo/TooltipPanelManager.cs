using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TooltipPanelManager : MonoBehaviour
{
  public TooltipPanel m_tooltipPanelPrefab;
  private static TooltipPanelManager s_instance;
  private Pool<TooltipPanel> m_tooltipPanelPool = new Pool<TooltipPanel>();
  private List<TooltipPanel> m_tooltipPanels = new List<TooltipPanel>();
  private Actor m_actor;
  private Card m_card;
  private float scaleToUse;
  private const float FADE_IN_TIME = 0.125f;
  private const float DELAY_BEFORE_FADE_IN = 0.4f;
  private const int ADJUST_PANEL_AFTER_COUNT = 3;
  public Vector3 m_TooltipOffsetFromCard = new Vector3(1.2f, -2f, 0.0f);
  public Vector3 m_TooltipOffsetFromSignatureCard = new Vector3(1.2f, -2f, 1f);
  private CancellationTokenSource m_panelTokenSource;
  private static readonly GAME_TAG[] spellpowerTags = new GAME_TAG[9]
  {
    GAME_TAG.SPELLPOWER,
    GAME_TAG.SPELLPOWER_ARCANE,
    GAME_TAG.SPELLPOWER_FIRE,
    GAME_TAG.SPELLPOWER_FROST,
    GAME_TAG.SPELLPOWER_NATURE,
    GAME_TAG.SPELLPOWER_HOLY,
    GAME_TAG.SPELLPOWER_SHADOW,
    GAME_TAG.SPELLPOWER_FEL,
    GAME_TAG.SPELLPOWER_PHYSICAL
  };

  private void Awake()
  {
    TooltipPanelManager.s_instance = this;
    this.scaleToUse = (float) TooltipPanel.GAMEPLAY_SCALE;
    this.m_tooltipPanelPool.SetCreateItemCallback(new Pool<TooltipPanel>.CreateItemCallback(this.CreateKeywordPanel));
    this.m_tooltipPanelPool.SetDestroyItemCallback(new Pool<TooltipPanel>.DestroyItemCallback(this.DestroyKeywordPanel));
    this.m_tooltipPanelPool.SetExtensionCount(1);
    if (SceneMgr.Get() != null)
      SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
    if (this.m_panelTokenSource != null)
      return;
    this.m_panelTokenSource = new CancellationTokenSource();
  }

  private void OnDestroy()
  {
    this.m_tooltipPanelPool.ReleaseAll();
    this.m_tooltipPanelPool.Clear();
    TooltipPanelManager.s_instance = (TooltipPanelManager) null;
    this.m_panelTokenSource?.Cancel();
    this.m_panelTokenSource?.Dispose();
  }

  public static TooltipPanelManager Get() => TooltipPanelManager.s_instance;

  public void UpdateKeywordPanelsPosition(Card card, bool showOnRight)
  {
    Actor actor = card.GetActor();
    if ((Object) actor == (Object) null || (Object) actor.GetMeshRenderer() == (Object) null)
      return;
    bool flag1 = card.GetZone() is ZoneHand;
    bool flag2 = card.GetEntity() != null && card.GetEntity().IsHeroPower();
    bool flag3 = card.GetEntity().IsLettuceAbility();
    bool flag4 = card.GetEntity().HasTag(GAME_TAG.LETTUCE_MERCENARY);
    TooltipPanelManager.TooltipPanelCreationArgs args = new TooltipPanelManager.TooltipPanelCreationArgs()
    {
      actorMeshRoot = actor.GetMeshRenderer().gameObject,
      actorRoot = actor.gameObject,
      card = card,
      showOnRight = showOnRight,
      inHand = flag1,
      isHeroPower = flag2,
      isLettuceAbility = flag3,
      isMercenary = flag4
    };
    if (this.ShouldUseBonesForPlacingTooltips(args))
      this.PositionPanelsForGameWithBones(args, this.m_panelTokenSource.Token);
    else
      this.PositionPanelsForGame(args, this.m_panelTokenSource.Token).Forget();
  }

  public void UpdateKeywordHelp(
    Card card,
    Actor actor,
    bool showOnRight = true,
    float? overrideScale = null,
    Vector3? overrideOffset = null)
  {
    this.m_card = card;
    this.UpdateKeywordHelp(card.GetEntity(), actor, showOnRight, overrideScale, overrideOffset);
  }

  private void GetDesiredEntityBaseForEntity(
    Entity entity,
    bool isHistoryTile,
    out EntityBase mainEntityBaseForKeyword,
    out List<EntityBase> additionalEntityBasesForKeyword)
  {
    mainEntityBaseForKeyword = (EntityBase) null;
    additionalEntityBasesForKeyword = (List<EntityBase>) null;
    if (entity == null || GameState.Get().GetGameEntity().GetEntityBaseForKeywordTooltips(entity, isHistoryTile, out mainEntityBaseForKeyword, out additionalEntityBasesForKeyword))
      return;
    int tag = entity.GetTag(GAME_TAG.ALTERNATE_MOUSE_OVER_CARD);
    if (tag != 0)
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(tag);
      if (entityDef == null)
      {
        Log.Gameplay.PrintError("TooltipPanelManager.GetDesiredEntityBaseForEntity(): Unable to load EntityDef for card ID {0}.", (object) tag);
      }
      else
      {
        mainEntityBaseForKeyword = (EntityBase) entityDef;
        return;
      }
    }
    mainEntityBaseForKeyword = (EntityBase) entity;
  }

  public void UpdateKeywordHelp(
    Entity entity,
    Actor actor,
    bool showOnRight,
    float? overrideScale = null,
    Vector3? overrideOffset = null)
  {
    this.m_card = entity.GetCard();
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.SHOW_CRAZY_KEYWORD_TOOLTIP))
    {
      if (!((Object) TutorialKeywordManager.Get() != (Object) null))
        return;
      TutorialKeywordManager.Get().UpdateKeywordHelp(entity, actor, showOnRight, overrideScale);
    }
    else
    {
      bool flag1 = this.m_card.GetZone() is ZoneHand;
      bool flag2 = entity.IsHeroPower();
      bool flag3 = entity.IsLettuceAbility();
      this.scaleToUse = !overrideScale.HasValue ? (!flag1 ? (float) TooltipPanel.GAMEPLAY_SCALE : (float) TooltipPanel.HAND_SCALE) : overrideScale.Value;
      this.PrepareToUpdateKeywordHelp(actor);
      List<TooltipPanelManager.TooltipPanelData> helpPanelDisplay = GameState.Get().GetGameEntity().GetOverwriteKeywordHelpPanelDisplay(entity);
      if (helpPanelDisplay == null)
      {
        string[] strArray = GameState.Get().GetGameEntity().NotifyOfKeywordHelpPanelDisplay(entity);
        if (strArray != null)
          this.SetupTooltipPanel(strArray[0], strArray[1]);
        EntityBase mainEntityBaseForKeyword;
        List<EntityBase> additionalEntityBasesForKeyword;
        this.GetDesiredEntityBaseForEntity(entity, false, out mainEntityBaseForKeyword, out additionalEntityBasesForKeyword);
        this.SetUpPanels(mainEntityBaseForKeyword, additionalEntityBasesForKeyword);
      }
      else
      {
        foreach (TooltipPanelManager.TooltipPanelData tooltipPanelData in helpPanelDisplay)
          this.SetupTooltipPanel(tooltipPanelData.m_title, tooltipPanelData.m_description);
      }
      if (flag3)
      {
        MercenariesAbilityTray abilityTray = ZoneMgr.Get().GetLettuceZoneController().GetAbilityTray();
        if ((Object) abilityTray != (Object) null)
          showOnRight = abilityTray.GetTrayPositionOfAbility(entity.GetCard()) < 2;
      }
      TooltipPanelManager.TooltipPanelCreationArgs args = new TooltipPanelManager.TooltipPanelCreationArgs()
      {
        actorMeshRoot = actor.GetMeshRenderer().gameObject,
        actorRoot = actor.gameObject,
        card = this.m_card,
        showOnRight = showOnRight,
        inHand = flag1,
        isHeroPower = flag2,
        isLettuceAbility = flag3,
        isMercenary = entity.HasTag(GAME_TAG.LETTUCE_MERCENARY),
        overrideOffset = overrideOffset
      };
      if (this.ShouldUseBonesForPlacingTooltips(args))
        this.PositionPanelsForGameWithBones(args, this.m_panelTokenSource.Token);
      else
        this.PositionPanelsForGame(args, this.m_panelTokenSource.Token).Forget();
      GameState.Get().GetGameEntity().NotifyOfHelpPanelDisplay(this.m_tooltipPanels.Count);
    }
  }

  private bool ShouldUseBonesForPlacingTooltips(TooltipPanelManager.TooltipPanelCreationArgs args)
  {
    if (args == null || (Object) args.actorMeshRoot == (Object) null || (Object) args.actorRoot == (Object) null || (Object) args.card == (Object) null || args.card.GetEntity() == null || !args.isLettuceAbility && !args.card.GetEntity().IsMinion())
      return false;
    GameEntity gameEntity = GameState.Get()?.GetGameEntity();
    if (gameEntity == null || !gameEntity.GetGameOptions().GetBooleanOption(GameEntityOption.USE_BONES_FOR_TOOLTIP_PLACEMENT))
      return false;
    BigCardTooltipDisplayBones componentInChildren = args.actorRoot.gameObject.GetComponentInChildren<BigCardTooltipDisplayBones>();
    if ((Object) componentInChildren == (Object) null)
      return false;
    BigCardTooltipDisplayBones.BoneVerification bonesToCheck = args.isLettuceAbility ? BigCardTooltipDisplayBones.BoneVerification.ALL_BONES : BigCardTooltipDisplayBones.BoneVerification.PRIMARY_ONLY;
    return componentInChildren.HasBonesForCurrentPlatform(bonesToCheck) && GameState.Get().MercenariesAllowBigCardBones();
  }

  private int TooltipBones_ComputePanelsPerColumn_LettuceAbility() => !PlatformSettings.IsMobile() ? 3 : 2;

  private async UniTaskVoid PositionPanelsForGameWithBones_LettuceAbility(
    TooltipPanelManager.TooltipPanelCreationArgs args,
    CancellationToken token)
  {
    if (args == null || (Object) args.actorRoot == (Object) null || this.m_tooltipPanels.Count == 0)
      return;
    BigCardTooltipDisplayBones componentInChildren = args.actorRoot.GetComponentInChildren<BigCardTooltipDisplayBones>();
    if ((Object) componentInChildren == (Object) null || !componentInChildren.HasBonesForCurrentPlatform(BigCardTooltipDisplayBones.BoneVerification.ALL_BONES))
      return;
    TooltipBoneLayout boneLayout = componentInChildren.GetRigForCurrentPlatform();
    if ((Object) boneLayout == (Object) null || !boneLayout.HasAllBones())
      return;
    GameObject topRowBone;
    GameObject bottomRowBone;
    if (args.showOnRight)
    {
      topRowBone = boneLayout.m_topRightTooltipBone;
      bottomRowBone = boneLayout.m_bottomRightTooltipBone;
    }
    else
    {
      topRowBone = boneLayout.m_topLeftTooltipBone;
      bottomRowBone = boneLayout.m_bottomLeftTooltipBone;
    }
    TooltipPanel lastTopRowPanel = (TooltipPanel) null;
    TooltipPanel lastBottomRowPanel = (TooltipPanel) null;
    TooltipPanel prevPanel = (TooltipPanel) null;
    TooltipPanel curPanel = this.m_tooltipPanels[0];
    UniTask uniTask;
    while ((Object) curPanel != (Object) null && !curPanel.Destroyed && !curPanel.IsTextRendered())
    {
      uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
      await uniTask;
    }
    if ((Object) curPanel == (Object) null || (Object) curPanel.gameObject == (Object) null || (Object) args.actorMeshRoot == (Object) null || curPanel.Destroyed)
      return;
    Vector3 selfUnitAnchor1 = new Vector3(args.showOnRight ? 0.0f : 1f, 0.0f, 0.0f);
    if (args.overrideOffset.HasValue)
      TransformUtil.SetPoint((Component) curPanel, selfUnitAnchor1, topRowBone, Vector3.zero, args.overrideOffset.Value);
    else
      TransformUtil.SetPoint((Component) curPanel, selfUnitAnchor1, topRowBone, Vector3.zero, Vector3.zero);
    lastTopRowPanel = curPanel;
    prevPanel = curPanel;
    curPanel = (TooltipPanel) null;
    if (this.m_tooltipPanels.Count < 2)
      return;
    curPanel = this.m_tooltipPanels[1];
    while ((Object) curPanel != (Object) null && !curPanel.Destroyed && !curPanel.IsTextRendered())
    {
      uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
      await uniTask;
    }
    if ((Object) curPanel == (Object) null || (Object) curPanel.gameObject == (Object) null || (Object) args.actorMeshRoot == (Object) null || curPanel.Destroyed)
      return;
    Vector3 selfUnitAnchor2 = new Vector3(args.showOnRight ? 0.0f : 1f, 0.0f, 1f);
    if (args.overrideOffset.HasValue)
      TransformUtil.SetPoint((Component) curPanel, selfUnitAnchor2, bottomRowBone, Vector3.zero, args.overrideOffset.Value);
    else
      TransformUtil.SetPoint((Component) curPanel, selfUnitAnchor2, bottomRowBone, Vector3.zero, Vector3.zero);
    lastBottomRowPanel = curPanel;
    prevPanel = curPanel;
    curPanel = (TooltipPanel) null;
    int panelsPerColumn = this.TooltipBones_ComputePanelsPerColumn_LettuceAbility();
    if (panelsPerColumn <= 0)
      panelsPerColumn = 2;
    for (int i = 2; i < this.m_tooltipPanels.Count; ++i)
    {
      curPanel = this.m_tooltipPanels[i];
      while ((Object) curPanel != (Object) null && !curPanel.Destroyed && !curPanel.IsTextRendered())
      {
        uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
        await uniTask;
      }
      if ((Object) curPanel == (Object) null || (Object) curPanel.gameObject == (Object) null || (Object) args.actorMeshRoot == (Object) null || curPanel.Destroyed)
        break;
      switch (i % panelsPerColumn)
      {
        case 0:
          float x1 = prevPanel.gameObject.transform.localScale.x;
          Vector3 zero1 = Vector3.zero;
          Vector3 selfUnitAnchor3;
          Vector3 relativeUnitAnchor1;
          if (args.showOnRight)
          {
            selfUnitAnchor3 = new Vector3(0.0f, 0.0f, 0.0f);
            relativeUnitAnchor1 = new Vector3(1f, 0.0f, 0.0f);
            zero1.x += boneLayout.m_manualHorizontalAdjustment * x1;
          }
          else
          {
            selfUnitAnchor3 = new Vector3(1f, 0.0f, 0.0f);
            relativeUnitAnchor1 = new Vector3(0.0f, 0.0f, 0.0f);
            zero1.x -= boneLayout.m_manualHorizontalAdjustment * x1;
          }
          TransformUtil.SetPoint((Component) curPanel, selfUnitAnchor3, (Component) lastTopRowPanel, relativeUnitAnchor1, zero1);
          lastTopRowPanel = curPanel;
          break;
        case 1:
          float x2 = prevPanel.gameObject.transform.localScale.x;
          Vector3 zero2 = Vector3.zero;
          Vector3 selfUnitAnchor4;
          Vector3 relativeUnitAnchor2;
          if (args.showOnRight)
          {
            selfUnitAnchor4 = new Vector3(0.0f, 0.0f, 1f);
            relativeUnitAnchor2 = new Vector3(1f, 0.0f, 1f);
            zero2.x += boneLayout.m_manualHorizontalAdjustment * x2;
          }
          else
          {
            selfUnitAnchor4 = new Vector3(1f, 0.0f, 1f);
            relativeUnitAnchor2 = new Vector3(0.0f, 0.0f, 1f);
            zero2.x -= boneLayout.m_manualHorizontalAdjustment * x2;
          }
          TransformUtil.SetPoint((Component) curPanel, selfUnitAnchor4, (Component) lastBottomRowPanel, relativeUnitAnchor2, zero2);
          lastBottomRowPanel = curPanel;
          break;
        default:
          TransformUtil.SetPoint((Component) curPanel, new Vector3(0.0f, 0.0f, 1f), (Component) prevPanel, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -(boneLayout.m_manualVerticalAdjustment * prevPanel.gameObject.transform.localScale.z)));
          break;
      }
      prevPanel = curPanel;
      curPanel = (TooltipPanel) null;
    }
  }

  private int TooltipBones_ComputePanelsPerColumn_NonLettuceAbility(Card card) => (Object) card == (Object) null ? 3 : (!(card.GetZone() is ZonePlay) ? 3 : (!PlatformSettings.IsMobile() ? 4 : 3));

  private async UniTaskVoid PositionPanelsForGameWithBones_NonLettuceAbility(
    TooltipPanelManager.TooltipPanelCreationArgs args,
    CancellationToken token)
  {
    if (args == null || (Object) args.actorRoot == (Object) null || this.m_tooltipPanels.Count == 0)
      return;
    BigCardTooltipDisplayBones componentInChildren = args.actorRoot.GetComponentInChildren<BigCardTooltipDisplayBones>();
    if ((Object) componentInChildren == (Object) null || !componentInChildren.HasBonesForCurrentPlatform(BigCardTooltipDisplayBones.BoneVerification.PRIMARY_ONLY))
      return;
    TooltipBoneLayout boneLayout = componentInChildren.GetRigForCurrentPlatform();
    if ((Object) boneLayout == (Object) null || !boneLayout.HasPrimaryBones())
      return;
    GameObject bone;
    Vector3 currPanelCorner;
    if (args.showOnRight)
    {
      bone = boneLayout.m_topRightTooltipBone;
      currPanelCorner = new Vector3(0.0f, 0.0f, 1f);
    }
    else
    {
      bone = boneLayout.m_topLeftTooltipBone;
      currPanelCorner = new Vector3(1f, 0.0f, 1f);
    }
    TooltipPanel curPanel = this.m_tooltipPanels[0];
    UniTask uniTask;
    while ((Object) curPanel != (Object) null && !curPanel.Destroyed && !curPanel.IsTextRendered())
    {
      uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
      await uniTask;
    }
    if ((Object) curPanel == (Object) null || (Object) curPanel.gameObject == (Object) null || (Object) args.actorMeshRoot == (Object) null || curPanel.Destroyed)
      return;
    if (args.overrideOffset.HasValue)
      TransformUtil.SetPoint(curPanel.gameObject, currPanelCorner, bone, Vector3.zero, args.overrideOffset.Value);
    else
      TransformUtil.SetPoint(curPanel.gameObject, currPanelCorner, bone, Vector3.zero, Vector3.zero);
    TooltipPanel topColumnPanel = curPanel;
    TooltipPanel prevPanel = curPanel;
    int panelsPerColumn = this.TooltipBones_ComputePanelsPerColumn_NonLettuceAbility(args.card);
    if (panelsPerColumn <= 0)
      panelsPerColumn = 3;
    for (int i = 1; i < this.m_tooltipPanels.Count; ++i)
    {
      curPanel = this.m_tooltipPanels[i];
      while ((Object) curPanel != (Object) null && !curPanel.Destroyed && !curPanel.IsTextRendered())
      {
        uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
        await uniTask;
      }
      if ((Object) curPanel == (Object) null || (Object) curPanel.gameObject == (Object) null || (Object) args.actorMeshRoot == (Object) null || curPanel.Destroyed)
        break;
      Vector3 zero = Vector3.zero;
      if (i % panelsPerColumn == 0)
      {
        float x = prevPanel.gameObject.transform.localScale.x;
        Vector3 relativeUnitAnchor;
        if (args.showOnRight)
        {
          currPanelCorner = new Vector3(0.0f, 0.0f, 1f);
          relativeUnitAnchor = new Vector3(1f, 0.0f, 1f);
          zero.x += boneLayout.m_manualHorizontalAdjustment * x;
        }
        else
        {
          currPanelCorner = new Vector3(1f, 0.0f, 1f);
          relativeUnitAnchor = new Vector3(0.0f, 0.0f, 1f);
          zero.x -= boneLayout.m_manualHorizontalAdjustment * x;
        }
        TransformUtil.SetPoint(curPanel.gameObject, currPanelCorner, topColumnPanel.gameObject, relativeUnitAnchor, zero);
        topColumnPanel = curPanel;
      }
      else
      {
        float z = prevPanel.gameObject.transform.localScale.z;
        zero.z -= boneLayout.m_manualVerticalAdjustment * z;
        TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), prevPanel.gameObject, Vector3.zero, zero);
      }
      prevPanel = curPanel;
    }
  }

  private void PositionPanelsForGameWithBones(
    TooltipPanelManager.TooltipPanelCreationArgs args,
    CancellationToken token)
  {
    if (args == null)
      return;
    if (args.isLettuceAbility)
      this.PositionPanelsForGameWithBones_LettuceAbility(args, token).Forget();
    else
      this.PositionPanelsForGameWithBones_NonLettuceAbility(args, token).Forget();
  }

  private async UniTaskVoid PositionPanelsForGame(
    TooltipPanelManager.TooltipPanelCreationArgs args,
    CancellationToken token)
  {
    if (args == null)
      return;
    Vector3 inHandOffset = new Vector3();
    if (args.inHand && args.isMercenary && this.m_tooltipPanels.Count > 3)
    {
      Bounds setPointBounds = TransformUtil.ComputeSetPointBounds(args.actorMeshRoot);
      Vector3 size1 = setPointBounds.size;
      Vector3 vector3_1 = new Vector3();
      for (int index = 0; index < this.m_tooltipPanels.Count; ++index)
      {
        TooltipPanel tooltipPanel = this.m_tooltipPanels[index];
        if ((Object) tooltipPanel != (Object) null)
        {
          Vector3 vector3_2 = vector3_1;
          setPointBounds = TransformUtil.ComputeSetPointBounds((Component) tooltipPanel);
          Vector3 size2 = setPointBounds.size;
          vector3_1 = vector3_2 + size2;
        }
      }
      inHandOffset = new Vector3(0.0f, 0.0f, (float) (((double) vector3_1.z - (double) size1.z) / 2.0));
      if (!(bool) UniversalInputManager.UsePhoneUI && !UniversalInputManager.Get().IsTouchMode())
        inHandOffset.z += 0.1f;
    }
    TooltipPanel prevPanel = (TooltipPanel) null;
    for (int i = 0; i < this.m_tooltipPanels.Count; ++i)
    {
      TooltipPanel curPanel = this.m_tooltipPanels[i];
      while ((Object) curPanel != (Object) null && !curPanel.Destroyed && !curPanel.IsTextRendered())
        await UniTask.Yield(PlayerLoopTiming.Update, token);
      if ((Object) curPanel == (Object) null || (Object) curPanel.gameObject == (Object) null || (Object) args.actorMeshRoot == (Object) null || curPanel.Destroyed)
        break;
      if (i == 0)
      {
        if (args.overrideOffset.HasValue)
        {
          if (args.showOnRight)
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), args.actorMeshRoot, new Vector3(1f, 0.0f, 1f), args.overrideOffset.Value);
          else
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0.0f, 1f), args.actorMeshRoot, new Vector3(0.0f, 0.0f, 1f), args.overrideOffset.Value);
        }
        else if (args.inHand)
        {
          if (args.showOnRight)
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), args.actorMeshRoot, new Vector3(1f, 0.0f, 1f), inHandOffset);
          else
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0.0f, 1f), args.actorMeshRoot, new Vector3(0.0f, 0.0f, 1f), new Vector3(-0.15f, 0.0f, 0.0f) + inHandOffset);
        }
        else if (args.isHeroPower)
        {
          if (args.showOnRight)
          {
            if ((bool) UniversalInputManager.UsePhoneUI)
              TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), args.actorMeshRoot, new Vector3(1f, 0.0f, 1f), new Vector3(0.6f, 0.0f, 0.2f));
            else
              TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), args.actorMeshRoot, new Vector3(1f, 0.0f, 1f), new Vector3(0.38f, 0.0f, -0.05f));
          }
          else if ((bool) UniversalInputManager.UsePhoneUI)
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0.0f, 1f), args.actorMeshRoot, new Vector3(0.0f, 0.0f, 1f), new Vector3(-0.2f, 0.0f, 0.2f));
          else
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0.0f, 1f), args.actorMeshRoot, new Vector3(0.0f, 0.0f, 1f), new Vector3(-0.42f, 0.0f, -0.05f));
        }
        else if (args.isLettuceAbility)
        {
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            if (args.showOnRight)
              TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), args.actorMeshRoot, new Vector3(1f, 0.0f, 1f), new Vector3((float) (0.5 * (double) this.scaleToUse + 0.5), 0.0f, 1.45f));
            else
              TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0.0f, 1f), args.actorMeshRoot, new Vector3(0.0f, 0.0f, 1f), new Vector3((float) (-0.779999971389771 * (double) this.scaleToUse - 0.5), 0.0f, 1.45f));
          }
          else if (args.showOnRight)
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), args.actorMeshRoot, new Vector3(1f, 0.0f, 1f), new Vector3((float) (0.5 * (double) this.scaleToUse + 0.5), 0.0f, 1.35f));
          else
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0.0f, 1f), args.actorMeshRoot, new Vector3(0.0f, 0.0f, 1f), new Vector3((float) (-0.779999971389771 * (double) this.scaleToUse - 0.5), 0.0f, 1.35f));
        }
        else if ((bool) UniversalInputManager.UsePhoneUI)
        {
          if (args.showOnRight)
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), args.actorMeshRoot, new Vector3(1f, 0.0f, 1f), new Vector3(1.5f, 0.0f, 2f));
          else
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0.0f, 1f), args.actorMeshRoot, new Vector3(0.0f, 0.0f, 1f), new Vector3(-1.8f, 0.0f, 2f));
        }
        else if (args.showOnRight)
          TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), args.actorMeshRoot, new Vector3(1f, 0.0f, 1f), new Vector3((float) (0.5 * (double) this.scaleToUse + 0.150000005960464), 0.0f, 0.8f));
        else
          TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0.0f, 1f), args.actorMeshRoot, new Vector3(0.0f, 0.0f, 1f), new Vector3((float) (-0.779999971389771 * (double) this.scaleToUse - 0.150000005960464), 0.0f, 0.8f));
      }
      else if (args.isLettuceAbility && i == 1)
        TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), prevPanel.gameObject, Vector3.zero, new Vector3(0.0f, 0.0f, 1.35f) + new Vector3(0.0f, 0.0f, -3f));
      else
        TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), prevPanel.gameObject, Vector3.zero, new Vector3(0.0f, 0.0f, 0.17f));
      prevPanel = curPanel;
      curPanel = (TooltipPanel) null;
    }
  }

  public List<TooltipPanel> GetCurrentTooltipPanels() => this.m_tooltipPanels;

  public void UpdateKeywordHelpForHistoryCard(Entity entity, Actor actor, UberText createdByText)
  {
    this.m_card = entity.GetCard();
    this.scaleToUse = (float) TooltipPanel.HISTORY_SCALE;
    this.PrepareToUpdateKeywordHelp(actor);
    string[] strArray = GameState.Get().GetGameEntity().NotifyOfKeywordHelpPanelDisplay(entity);
    if (strArray != null)
      this.SetupTooltipPanel(strArray[0], strArray[1]);
    EntityBase mainEntityBaseForKeyword;
    this.GetDesiredEntityBaseForEntity(entity, true, out mainEntityBaseForKeyword, out List<EntityBase> _);
    this.SetUpPanels(mainEntityBaseForKeyword);
    this.PositionPanelsForHistory(actor, createdByText, this.m_panelTokenSource.Token).Forget();
  }

  private async UniTaskVoid PositionPanelsForHistory(
    Actor actor,
    UberText createdByText,
    CancellationToken token)
  {
    GameObject firstRelativeAnchor;
    UniTask uniTask;
    if (createdByText.gameObject.activeSelf)
    {
      firstRelativeAnchor = createdByText.gameObject;
    }
    else
    {
      GameObject historyKeywordBone = actor.FindBone("HistoryKeywordBone");
      if ((Object) historyKeywordBone == (Object) null)
      {
        Error.AddDevWarning("Missing Bone", "Missing HistoryKeywordBone on {0}", (object) actor);
        uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
        await uniTask;
      }
      firstRelativeAnchor = historyKeywordBone;
      historyKeywordBone = (GameObject) null;
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_tooltipPanels.Clear();
    TooltipPanel prevPanel = (TooltipPanel) null;
    bool showHorizontally = false;
    for (int i = 0; i < this.m_tooltipPanels.Count; ++i)
    {
      TooltipPanel curPanel = this.m_tooltipPanels[i];
      while ((Object) curPanel != (Object) null && !curPanel.Destroyed && !curPanel.IsTextRendered())
      {
        uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
        await uniTask;
      }
      if (!((Object) curPanel == (Object) null) && !curPanel.Destroyed)
      {
        if (i == 0)
        {
          TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.5f, 0.0f, 1f), firstRelativeAnchor, new Vector3(0.5f, 0.0f, 0.0f));
        }
        else
        {
          if ((double) prevPanel.transform.position.z - ((double) prevPanel.GetHeight() * 0.349999994039536 + (double) curPanel.GetHeight() * 0.349999994039536) < -8.30000019073486)
            showHorizontally = true;
          if (showHorizontally)
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), prevPanel.gameObject, new Vector3(1f, 0.0f, 1f), Vector3.zero);
          else
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.5f, 0.0f, 1f), prevPanel.gameObject, new Vector3(0.5f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.06094122f));
        }
        prevPanel = curPanel;
        curPanel = (TooltipPanel) null;
      }
    }
  }

  public void UpdateKeywordHelpForCollectionManager(
    EntityDef entityDef,
    Actor actor,
    TooltipPanelManager.Orientation orientation)
  {
    this.scaleToUse = (float) TooltipPanel.COLLECTION_MANAGER_SCALE;
    this.PrepareToUpdateKeywordHelp(actor);
    this.SetUpPanels((EntityBase) entityDef);
    this.PositionPanelsForCM(actor, orientation, this.m_panelTokenSource.Token).Forget();
  }

  private async UniTaskVoid PositionPanelsForCM(
    Actor actor,
    TooltipPanelManager.Orientation orientation = TooltipPanelManager.Orientation.RightTop,
    CancellationToken token = default (CancellationToken))
  {
    GameObject actorObject = actor.GetMeshRenderer().gameObject;
    TooltipPanel prevPanel = (TooltipPanel) null;
    int maxPanelCount = this.m_tooltipPanels.Count;
    if ((bool) UniversalInputManager.UsePhoneUI)
      maxPanelCount = Mathf.Min(this.m_tooltipPanels.Count, 3);
    Vector3 actorStartAnchor;
    Vector3 panelStartAnchor;
    Vector3 panelEndAnchor;
    switch (orientation)
    {
      case TooltipPanelManager.Orientation.RightTop:
        actorStartAnchor = new Vector3(1f, 0.0f, 1f);
        panelStartAnchor = new Vector3(0.0f, 0.0f, 1f);
        panelEndAnchor = Vector3.zero;
        break;
      case TooltipPanelManager.Orientation.RightBottom:
        actorStartAnchor = new Vector3(1f, 0.0f, 0.0f);
        panelStartAnchor = Vector3.zero;
        panelEndAnchor = new Vector3(0.0f, 0.0f, 1f);
        break;
      case TooltipPanelManager.Orientation.LeftMiddle:
        actorStartAnchor = new Vector3(-1f, 0.0f, 0.5f);
        panelStartAnchor = new Vector3(1f, 0.0f, 0.4f);
        panelEndAnchor = new Vector3(0.0f, 0.0f, 0.0f);
        break;
      default:
        Log.All.PrintError("TooltipPanelManager.PositionPanelsForCM received a bad orientation value: " + (object) orientation);
        actorStartAnchor = Vector3.zero;
        panelStartAnchor = Vector3.zero;
        panelEndAnchor = Vector3.zero;
        break;
    }
    for (int i = 0; i < this.m_tooltipPanels.Count; ++i)
    {
      TooltipPanel panel = this.m_tooltipPanels[i];
      if (i >= maxPanelCount)
      {
        panel.gameObject.SetActive(false);
      }
      else
      {
        while ((Object) panel != (Object) null && !panel.Destroyed && !panel.IsTextRendered())
          await UniTask.Yield(PlayerLoopTiming.Update, token);
        if (!((Object) panel == (Object) null) && !panel.Destroyed)
        {
          if (actor.IsSpellActive(SpellType.GHOSTCARD))
          {
            Spell spell = actor.GetSpell(SpellType.GHOSTCARD);
            if ((Object) spell != (Object) null)
            {
              RenderToTexture componentInChildren = spell.gameObject.GetComponentInChildren<RenderToTexture>();
              if ((Object) componentInChildren != (Object) null)
                actorObject = componentInChildren.GetRenderToObject();
            }
          }
          if (i == 0)
          {
            TransformUtil.SetPoint(panel.gameObject, panelStartAnchor, actorObject, actorStartAnchor, Vector3.up);
            if (actor.isMissingCard())
            {
              RenderToTexture component = actor.m_missingCardEffect.GetComponent<RenderToTexture>();
              if ((Object) component != (Object) null)
                panel.gameObject.transform.position -= component.GetOffscreenPositionOffset();
            }
            else if (actor.isGhostCard())
            {
              RenderToTexture component = actor.m_ghostCardGameObject.GetComponent<RenderToTexture>();
              if ((Object) component != (Object) null)
                panel.gameObject.transform.position -= component.GetOffscreenPositionOffset();
            }
          }
          else
            TransformUtil.SetPoint(panel.gameObject, panelStartAnchor, prevPanel.gameObject, panelEndAnchor, Vector3.zero);
          prevPanel = panel;
          panel = (TooltipPanel) null;
        }
      }
    }
  }

  public void UpdateGhostCardHelpForCollectionManager(
    Actor actor,
    GhostCard.Type ghostType,
    TooltipPanelManager.Orientation orientation)
  {
    this.scaleToUse = (float) TooltipPanel.COLLECTION_MANAGER_SCALE;
    this.PrepareToUpdateGhostCardHelp(actor);
    string str = UniversalInputManager.Get().IsTouchMode() ? "_TOUCH" : "";
    string headline;
    string description;
    switch (ghostType)
    {
      case GhostCard.Type.MISSING_UNCRAFTABLE:
      case GhostCard.Type.MISSING:
        headline = GameStrings.Get("GLUE_GHOST_CARD_MISSING_TITLE");
        description = GameStrings.Get("GLUE_GHOST_CARD_MISSING_DESCRIPTION" + str);
        break;
      case GhostCard.Type.NOT_VALID:
        headline = GameStrings.Get("GLUE_GHOST_CARD_NOT_VALID_TITLE");
        description = GameStrings.Get("GLUE_GHOST_CARD_NOT_VALID_DESCRIPTION" + str);
        break;
      default:
        return;
    }
    this.SetupTooltipPanel(headline, description);
    this.PositionPanelsForCM(actor, orientation, this.m_panelTokenSource.Token).Forget();
  }

  public void UpdateKeywordHelpForDeckHelper(EntityDef entityDef, Actor actor)
  {
    this.scaleToUse = 3.75f;
    this.PrepareToUpdateKeywordHelp(actor);
    this.SetUpPanels((EntityBase) entityDef);
    this.PositionPanelsForForge(actor.GetMeshRenderer().gameObject, token: this.m_panelTokenSource.Token).Forget();
  }

  public void UpdateKeywordHelpForAdventure(EntityDef entityDef, Actor actor)
  {
    this.scaleToUse = (float) TooltipPanel.ADVENTURE_SCALE;
    this.PrepareToUpdateKeywordHelp(actor);
    this.SetUpPanels((EntityBase) entityDef);
    this.PositionPanelsForForge(actor.GetMeshRenderer().gameObject, token: this.m_panelTokenSource.Token).Forget();
  }

  public void UpdateKeywordHelpForForge(EntityDef entityDef, Actor actor, int cardChoice = 0)
  {
    this.scaleToUse = (float) TooltipPanel.FORGE_SCALE;
    this.PrepareToUpdateKeywordHelp(actor);
    this.SetUpPanels((EntityBase) entityDef);
    this.PositionPanelsForForge(actor.GetMeshRenderer().gameObject, token: this.m_panelTokenSource.Token).Forget();
  }

  private async UniTaskVoid PositionPanelsForForge(
    GameObject actorObject,
    int cardChoice = 0,
    CancellationToken token = default (CancellationToken))
  {
    TooltipPanel prevPanel = (TooltipPanel) null;
    for (int i = 0; i < this.m_tooltipPanels.Count; ++i)
    {
      TooltipPanel panel = this.m_tooltipPanels[i];
      while ((Object) panel != (Object) null && !panel.Destroyed && !panel.IsTextRendered())
        await UniTask.Yield(PlayerLoopTiming.Update, token);
      if (!((Object) panel == (Object) null) && !panel.Destroyed)
      {
        if (i == 0)
        {
          if ((bool) UniversalInputManager.UsePhoneUI)
            TransformUtil.SetPoint(panel.gameObject, new Vector3(0.0f, 0.0f, 1f), actorObject, cardChoice == 3 ? new Vector3(0.0f, 0.0f, 1f) : new Vector3(1f, 0.0f, 1f), cardChoice == 3 ? new Vector3(-31f, 0.0f, 0.0f) : Vector3.zero);
          else
            TransformUtil.SetPoint(panel.gameObject, new Vector3(0.0f, 0.0f, 1f), actorObject, new Vector3(1f, 0.0f, 1f), Vector3.zero);
        }
        else
          TransformUtil.SetPoint(panel.gameObject, new Vector3(0.0f, 0.0f, 1f), prevPanel.gameObject, new Vector3(0.0f, 0.0f, 0.0f), Vector3.zero);
        prevPanel = panel;
        panel = (TooltipPanel) null;
      }
    }
  }

  public void UpdateKeywordHelpForPackOpening(EntityDef entityDef, Actor actor)
  {
    this.scaleToUse = 2.75f;
    this.PrepareToUpdateKeywordHelp(actor);
    this.SetUpPanels((EntityBase) entityDef);
    this.PositionPanelsForPackOpening(actor, this.m_panelTokenSource.Token).Forget();
  }

  private async UniTaskVoid PositionPanelsForPackOpening(
    Actor actor,
    CancellationToken token)
  {
    TooltipPanel prevPanel = (TooltipPanel) null;
    for (int i = 0; i < this.m_tooltipPanels.Count; ++i)
    {
      TooltipPanel panel = this.m_tooltipPanels[i];
      while ((Object) panel != (Object) null && !panel.Destroyed && !panel.IsTextRendered())
        await UniTask.Yield(PlayerLoopTiming.Update, token);
      if (!((Object) panel == (Object) null) && !panel.Destroyed)
      {
        if (i == 0)
        {
          TransformUtil.SetPoint(panel.gameObject, new Vector3(1f, 0.0f, 1f), actor.GetMeshRenderer().gameObject, new Vector3(0.0f, 0.0f, 1f), Vector3.zero);
          panel.transform.position -= actor.GetPremium() == TAG_PREMIUM.SIGNATURE ? this.m_TooltipOffsetFromSignatureCard : this.m_TooltipOffsetFromCard;
        }
        else
          TransformUtil.SetPoint(panel.gameObject, new Vector3(0.0f, 0.0f, 1f), prevPanel.gameObject, new Vector3(0.0f, 0.0f, 0.0f), Vector3.zero);
        prevPanel = panel;
        panel = (TooltipPanel) null;
      }
    }
  }

  public void UpdateKeywordHelpForMulliganCard(Entity entity, Actor actor)
  {
    this.m_card = entity.GetCard();
    this.scaleToUse = (float) TooltipPanel.MULLIGAN_SCALE;
    this.PrepareToUpdateKeywordHelp(actor);
    string[] strArray = GameState.Get().GetGameEntity().NotifyOfKeywordHelpPanelDisplay(entity);
    if (strArray != null)
      this.SetupTooltipPanel(strArray[0], strArray[1]);
    EntityBase mainEntityBaseForKeyword;
    this.GetDesiredEntityBaseForEntity(entity, false, out mainEntityBaseForKeyword, out List<EntityBase> _);
    this.SetUpPanels(mainEntityBaseForKeyword);
    this.PositionPanelsForMulligan(actor.GetMeshRenderer().gameObject, this.m_panelTokenSource.Token).Forget();
  }

  private async UniTaskVoid PositionPanelsForMulligan(
    GameObject actorObject,
    CancellationToken token)
  {
    TooltipPanel prevPanel = (TooltipPanel) null;
    bool showHorizontally = false;
    for (int i = 0; i < this.m_tooltipPanels.Count; ++i)
    {
      TooltipPanel curPanel = this.m_tooltipPanels[i];
      while ((Object) curPanel != (Object) null && !curPanel.Destroyed && !curPanel.IsTextRendered())
        await UniTask.Yield(PlayerLoopTiming.Update, token);
      if (!((Object) curPanel == (Object) null) && !curPanel.Destroyed)
      {
        if (i == 0)
        {
          TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.5f, 0.0f, 1f), actorObject, new Vector3(0.5f, 0.0f, 0.0f), new Vector3(-0.112071f, 0.0f, -0.1244259f));
        }
        else
        {
          if ((double) prevPanel.transform.position.z - ((double) prevPanel.GetHeight() * 0.349999994039536 + (double) curPanel.GetHeight() * 0.349999994039536) < -8.30000019073486)
            showHorizontally = true;
          if (showHorizontally)
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.0f, 0.0f, 1f), prevPanel.gameObject, new Vector3(1f, 0.0f, 1f), Vector3.zero);
          else
            TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.5f, 0.0f, 1f), prevPanel.gameObject, new Vector3(0.5f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.1588802f));
        }
        prevPanel = curPanel;
        curPanel = (TooltipPanel) null;
      }
    }
  }

  private void PrepareToUpdateKeywordHelp(Actor actor)
  {
    this.HideKeywordHelp();
    this.m_actor = actor;
    this.m_tooltipPanels.Clear();
  }

  private void PrepareToUpdateGhostCardHelp(Actor actor)
  {
    this.HideTooltipPanels();
    this.m_actor = actor;
    this.m_tooltipPanels.Clear();
  }

  private void ShowIncompatibleRunesPanelIfNecessary(EntityBase entityBase)
  {
    if (!entityBase.HasRuneCost || !CollectionManager.Get().IsInEditMode())
      return;
    RunePattern runesToAdd = new RunePattern(entityBase);
    if (CollectionManager.Get().GetEditedDeck().CanAddRunes(runesToAdd, DeckRule_DeathKnightRuneLimit.MaxRuneSlots))
      return;
    this.SetupTooltipPanel(GameStrings.Get("GLUE_COLLECTION_INCOMPATIBLE_RUNES_HEADER"), GameStrings.Get("GLUE_COLLECTION_INCOMPATIBLE_RUNES_DESCRIPTION"));
  }

  private void SetUpPanels(
    EntityBase mainEntityBaseForKeyword,
    List<EntityBase> additionalEntityBasesForKeyword = null)
  {
    TooltipPanelManager.KeywordPanelEntityInfo entityInfo = new TooltipPanelManager.KeywordPanelEntityInfo()
    {
      MainEntityBase = mainEntityBaseForKeyword,
      AdditionalEntityBases = additionalEntityBasesForKeyword
    };
    this.ShowIncompatibleRunesPanelIfNecessary(mainEntityBaseForKeyword);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SHIFTING);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SHIFTING_MINION);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SHIFTING_WEAPON);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SHIFTING_SPELL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.FLOOPY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.BOSS);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.WILD);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.HALL_OF_FAME);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.EMPOWER);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.TAUNT);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.STEALTH);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DIVINE_SHIELD);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLPOWER);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLPOWER_ARCANE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLPOWER_FIRE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLPOWER_FROST);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLPOWER_NATURE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLPOWER_HOLY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLPOWER_SHADOW);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLPOWER_FEL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLPOWER_PHYSICAL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLRESISTANCE_ARCANE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLRESISTANCE_FIRE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLRESISTANCE_FROST);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLRESISTANCE_NATURE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLRESISTANCE_HOLY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLRESISTANCE_SHADOW);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLRESISTANCE_FEL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLWEAKNESS_ARCANE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLWEAKNESS_FIRE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLWEAKNESS_FROST);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLWEAKNESS_NATURE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLWEAKNESS_HOLY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLWEAKNESS_SHADOW);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLWEAKNESS_FEL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.ENRAGED_TOOLTIP);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.CHARGE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.BATTLECRY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.FROZEN);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.FREEZE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.WINDFURY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.MEGA_WINDFURY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.ECHO);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.RUSH);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.MODULAR);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.OVERKILL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.PROPHECY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.ETHEREAL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.MARK_OF_EVIL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.WAND);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.TWINSPELL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.REBORN);
    if (mainEntityBaseForKeyword.GetZone() != TAG_ZONE.SECRET || !mainEntityBaseForKeyword.IsSecret())
      this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SECRET);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DEATHRATTLE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.OVERLOAD);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.COMBO);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SILENCE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_SILENCE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.COUNTER);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.IMMUNE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPARE_PART);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.INSPIRE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DISCOVER);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.CTHUN);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.AUTOATTACK);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.MINION_TYPE_REFERENCE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.JADE_GOLEM);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.GRIMY_GOONS);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.JADE_LOTUS);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.KABAL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.QUEST);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SIDEQUEST);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.POISONOUS);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.ADAPT);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LIFESTEAL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.RECRUIT);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DUNGEON_PASSIVE_BUFF);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.START_OF_GAME);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.CASTSWHENDRAWN);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SHRINE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.FATIGUEREFERENCE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.OUTCAST);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.STUDY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLBURST);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.CORRUPT);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DORMANT);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.CORRUPTEDCARD);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.FRENZY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.BLOOD_GEM);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.REFRESH);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.AVENGE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLCRAFT);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.TOOL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.QUESTLINE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.TRADEABLE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_ATTACK);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_SPELLCOMBO);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_SPELLDAMAGEARCANE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_SPELLDAMAGEFEL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_SPELLDAMAGEFIRE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_SPELLDAMAGEFROST);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_SPELLDAMAGEHOLY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_SPELLDAMAGENATURE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_SPELLDAMAGESHADOW);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DEATHBLOW);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.BLEED);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.CRITICALDAMAGE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.ROOT);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_ALLIANCE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_HORDE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_REFRESH);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_ELVES);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.HONORABLEKILL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.REVIVE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.ALLIED);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_HEALINGPOWER);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.BACON_FREEZE_TOOLTIP);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.BACON_STEALTH_TOOLTIP);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.BACON_QUEST_TOOLTIP);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.MERCS_SPELLRESISTANCE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.MERCS_SPELLWEAKNESS);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.COLOSSAL);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DREDGE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.MERCS_BENCH);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.INFUSE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.INFUSED);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.CORPSE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.MANATHIRST);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.MERCS_EXPLORER);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.LETTUCE_CHARGE);
    if (!mainEntityBaseForKeyword.IsHeroPower())
      return;
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.AI_MUST_PLAY);
  }

  private void GetEntityTagValuesForKeywordPanel(
    EntityBase entityInfo,
    GAME_TAG tag,
    out int tagValue,
    out int referenceTagValue)
  {
    tagValue = 0;
    if (entityInfo.HasTag(tag))
      tagValue = entityInfo.GetTag(tag);
    else if (entityInfo.HasCachedTagForDormant(tag))
      tagValue = entityInfo.GetCachedTagForDormant(tag);
    referenceTagValue = 0;
    if (!entityInfo.HasReferencedTag(tag))
      return;
    referenceTagValue = entityInfo.GetReferencedTag(tag);
  }

  private bool SetupKeywordPanelIfNecessary(
    TooltipPanelManager.KeywordPanelEntityInfo entityInfo,
    GAME_TAG tag)
  {
    EntityBase entityInfo1 = entityInfo.MainEntityBase;
    int tagValue1;
    int referenceTagValue1;
    this.GetEntityTagValuesForKeywordPanel(entityInfo1, tag, out tagValue1, out referenceTagValue1);
    if (tagValue1 == 0 && referenceTagValue1 == 0 && entityInfo.AdditionalEntityBases != null)
    {
      foreach (EntityBase additionalEntityBase in entityInfo.AdditionalEntityBases)
      {
        int tagValue2;
        int referenceTagValue2;
        this.GetEntityTagValuesForKeywordPanel(additionalEntityBase, tag, out tagValue2, out referenceTagValue2);
        if (tagValue2 != 0 || referenceTagValue2 != 0)
        {
          tagValue1 = tagValue2;
          referenceTagValue1 = referenceTagValue2;
          entityInfo1 = additionalEntityBase;
          break;
        }
      }
    }
    if (tagValue1 == 0 && referenceTagValue1 == 0)
      return false;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER && GameStrings.HasCollectionKeywordText(tag))
    {
      if (GAME_TAG.EMPOWER == tag)
      {
        if (entityInfo1.GetClass() != TAG_CLASS.NEUTRAL)
          tag = this.GetEmpowerTagByClass(entityInfo1.GetClass());
        if (CollectionManager.Get().IsInEditMode())
        {
          CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
          string galakrondCardIdByClass = GameUtils.GetGalakrondCardIdByClass(editedDeck.GetClass());
          if (editedDeck.GetCardIdCount(galakrondCardIdByClass) > 0)
            tag = this.GetEmpowerTagByClass(editedDeck.GetClass());
        }
      }
      this.SetupCollectionKeywordPanel(tag);
      return true;
    }
    if (tagValue1 != 0 && GameStrings.HasKeywordText(tag))
    {
      foreach (GAME_TAG spellpowerTag in TooltipPanelManager.spellpowerTags)
      {
        if (tag == spellpowerTag)
        {
          int tag1 = entityInfo1.GetTag(tag);
          string empty = string.Empty;
          string description;
          if (tag1 > 0)
            description = GameStrings.Format(GameStrings.GetKeywordTextKey(tag), (object) tag1);
          else
            description = GameStrings.Get(GameStrings.GetRefKeywordTextKey(tag));
          this.SetupTooltipPanel(GameStrings.GetKeywordName(tag), description);
          return true;
        }
      }
      if (tag == GAME_TAG.WINDFURY && tagValue1 > 1)
      {
        if (tagValue1 != 3)
          return false;
        this.SetupKeywordPanel(GAME_TAG.MEGA_WINDFURY);
        return true;
      }
      if (GameMgr.Get() != null && GameMgr.Get().IsBattlegrounds())
      {
        switch (tag)
        {
          case GAME_TAG.STEALTH:
            this.SetupKeywordPanel(GAME_TAG.BACON_STEALTH_TOOLTIP);
            return true;
          case GAME_TAG.FROZEN:
            this.SetupKeywordPanel(GAME_TAG.BACON_FREEZE_TOOLTIP);
            return true;
          case GAME_TAG.QUEST:
            this.SetupKeywordPanel(GAME_TAG.BACON_QUEST_TOOLTIP);
            return true;
        }
      }
      if (tag == GAME_TAG.SHIFTING_MINION || tag == GAME_TAG.SHIFTING_WEAPON || tag == GAME_TAG.SHIFTING_SPELL || tag == GAME_TAG.SHIFTING)
      {
        int tag2 = entityInfo1.GetTag(GAME_TAG.TRANSFORMED_FROM_CARD);
        if (tag2 == 0)
          return false;
        EntityDef entityDef = DefLoader.Get().GetEntityDef(tag2);
        string description = GameStrings.Get(GameStrings.GetKeywordTextKey(tag));
        this.SetupTooltipPanel(entityDef.GetName(), description);
        return true;
      }
      if (tag == GAME_TAG.AI_MUST_PLAY && SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
      {
        int controllerId = entityInfo1.GetControllerId();
        Player player = GameState.Get().GetPlayer(controllerId);
        if (player != null && !player.IsAI())
          return false;
      }
      if (tag == GAME_TAG.EMPOWER && SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
      {
        int controllerId = entityInfo1.GetControllerId();
        Player player = GameState.Get().GetPlayer(controllerId);
        if (player != null && player.HasTag(GAME_TAG.PROXY_GALAKROND))
          tag = this.GetEmpowerTagByClass(GameState.Get().GetEntity(player.GetTag(GAME_TAG.PROXY_GALAKROND)).GetClass());
      }
      this.SetupKeywordPanel(tag);
      return true;
    }
    if (referenceTagValue1 == 0 || !GameStrings.HasRefKeywordText(tag))
      return false;
    this.SetupKeywordRefPanel(tag);
    return true;
  }

  private Vector3 GetPanelPosition(TooltipPanel panel)
  {
    Vector3 panelPosition = new Vector3(0.0f, 0.0f, 0.0f);
    TooltipPanel tooltipPanel1 = (TooltipPanel) null;
    for (int index = 0; index < this.m_tooltipPanels.Count; ++index)
    {
      TooltipPanel tooltipPanel2 = this.m_tooltipPanels[index];
      float num1 = !this.m_card.GetEntity().IsHero() ? (this.m_card.GetEntity().GetZone() != TAG_ZONE.PLAY ? 0.85f : 1.05f) : 1.2f;
      if ((Object) this.m_actor.GetMeshRenderer() == (Object) null)
        return panelPosition;
      Bounds bounds = this.m_actor.GetMeshRenderer().bounds;
      float num2 = (float) (-0.200000002980232 * (double) bounds.size.z);
      if ((Object) tooltipPanel2 == (Object) panel)
      {
        if (index == 0)
        {
          Vector3 position = this.m_actor.transform.position;
          bounds = this.m_actor.GetMeshRenderer().bounds;
          double x = (double) bounds.size.x * (double) num1;
          bounds = this.m_actor.GetMeshRenderer().bounds;
          double z = (double) bounds.extents.z + (double) num2;
          Vector3 vector3 = new Vector3((float) x, 0.0f, (float) z);
          panelPosition = position + vector3;
        }
        else
          panelPosition = tooltipPanel1.transform.position - new Vector3(0.0f, 0.0f, (float) ((double) tooltipPanel1.GetHeight() * 0.349999994039536 + (double) tooltipPanel2.GetHeight() * 0.349999994039536));
      }
      tooltipPanel1 = tooltipPanel2;
    }
    return panelPosition;
  }

  private void SetupCollectionKeywordPanel(GAME_TAG tag) => this.SetupTooltipPanel(GameStrings.GetKeywordName(tag), GameStrings.Get(GameStrings.GetCollectionKeywordTextKey(tag)));

  private void SetupKeywordPanel(GAME_TAG tag) => this.SetupTooltipPanel(GameStrings.GetKeywordName(tag), GameStrings.Get(GameStrings.GetKeywordTextKey(tag)));

  private void SetupKeywordRefPanel(GAME_TAG tag) => this.SetupTooltipPanel(GameStrings.GetKeywordName(tag), GameStrings.Get(GameStrings.GetRefKeywordTextKey(tag)));

  private void SetupTooltipPanel(string headline, string description)
  {
    TooltipPanel helpPanel = this.m_tooltipPanelPool.Acquire();
    if ((Object) helpPanel == (Object) null)
      return;
    helpPanel.Reset();
    helpPanel.Initialize(headline, description);
    helpPanel.SetScale(this.scaleToUse);
    this.m_tooltipPanels.Add(helpPanel);
    this.FadeInPanel(helpPanel);
  }

  private void FadeInPanel(TooltipPanel helpPanel)
  {
    this.CleanTweensOnPanel(helpPanel);
    float num = 0.4f;
    if (GameState.Get() != null && GameState.Get().GetBooleanGameOption(GameEntityOption.KEYWORD_HELP_DELAY_OVERRIDDEN))
      num = 0.0f;
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "onupdatetarget", (object) this.gameObject, (object) "onupdate", (object) "OnUberTextFadeUpdate", (object) "time", (object) 0.125f, (object) "delay", (object) num, (object) "to", (object) 1f, (object) "from", (object) 0.0f));
  }

  private void OnUberTextFadeUpdate(float newValue)
  {
    foreach (Component tooltipPanel in this.m_tooltipPanels)
      RenderUtils.SetAlpha(tooltipPanel.gameObject, newValue, true);
  }

  private void CleanTweensOnPanel(TooltipPanel helpPanel)
  {
    iTween.Stop(this.gameObject);
    RenderUtils.SetAlpha(helpPanel.gameObject, 0.0f, true);
  }

  public void ShowKeywordHelp()
  {
    foreach (Component tooltipPanel in this.m_tooltipPanels)
      tooltipPanel.gameObject.SetActive(true);
  }

  public void HideKeywordHelp()
  {
    GameState gameState = GameState.Get();
    if (gameState != null && gameState.GetBooleanGameOption(GameEntityOption.SHOW_CRAZY_KEYWORD_TOOLTIP) && (Object) TutorialKeywordManager.Get() != (Object) null)
      TutorialKeywordManager.Get().HideKeywordHelp();
    this.HideTooltipPanels();
  }

  public void HideTooltipPanels()
  {
    foreach (TooltipPanel tooltipPanel in this.m_tooltipPanels)
    {
      if (!((Object) tooltipPanel == (Object) null))
      {
        this.CleanTweensOnPanel(tooltipPanel);
        tooltipPanel.gameObject.SetActive(false);
        this.m_tooltipPanelPool.Release(tooltipPanel);
      }
    }
  }

  public Card GetCard() => this.m_card;

  public Vector3 GetPositionOfTopPanel() => this.m_tooltipPanels.Count == 0 ? new Vector3(0.0f, 0.0f, 0.0f) : this.m_tooltipPanels[0].transform.position;

  public TooltipPanel CreateKeywordPanel(int i) => Object.Instantiate<TooltipPanel>(this.m_tooltipPanelPrefab);

  private void DestroyKeywordPanel(TooltipPanel panel)
  {
    if (!((Object) panel != (Object) null))
      return;
    Object.Destroy((Object) panel.gameObject);
  }

  private void OnSceneUnloaded(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    foreach (Component tooltipPanel in this.m_tooltipPanels)
      Object.Destroy((Object) tooltipPanel.gameObject);
    this.m_tooltipPanels.Clear();
    this.m_tooltipPanelPool.Clear();
    Object.Destroy((Object) this.m_actor);
    this.m_actor = (Actor) null;
    Object.Destroy((Object) this.m_card);
    this.m_card = (Card) null;
  }

  private GAME_TAG GetEmpowerTagByClass(TAG_CLASS tagClass)
  {
    GAME_TAG empowerTagByClass = GAME_TAG.EMPOWER;
    switch (tagClass)
    {
      case TAG_CLASS.PRIEST:
        empowerTagByClass = GAME_TAG.EMPOWER_PRIEST;
        break;
      case TAG_CLASS.ROGUE:
        empowerTagByClass = GAME_TAG.EMPOWER_ROGUE;
        break;
      case TAG_CLASS.SHAMAN:
        empowerTagByClass = GAME_TAG.EMPOWER_SHAMAN;
        break;
      case TAG_CLASS.WARLOCK:
        empowerTagByClass = GAME_TAG.EMPOWER_WARLOCK;
        break;
      case TAG_CLASS.WARRIOR:
        empowerTagByClass = GAME_TAG.EMPOWER_WARRIOR;
        break;
    }
    return empowerTagByClass;
  }

  public struct TooltipPanelData
  {
    public string m_title;
    public string m_description;
  }

  public enum Orientation
  {
    RightTop,
    RightBottom,
    LeftMiddle,
  }

  private class TooltipPanelCreationArgs
  {
    public GameObject actorMeshRoot;
    public GameObject actorRoot;
    public Card card;
    public bool showOnRight;
    public bool inHand;
    public bool isHeroPower;
    public bool isLettuceAbility;
    public bool isMercenary;
    public Vector3? overrideOffset;
  }

  private struct KeywordPanelEntityInfo
  {
    public EntityBase MainEntityBase { get; set; }

    public List<EntityBase> AdditionalEntityBases { get; set; }
  }
}
