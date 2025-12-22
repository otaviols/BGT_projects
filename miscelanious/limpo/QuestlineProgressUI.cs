using Blizzard.T5.MaterialService.Extensions;
using System;
using UnityEngine;

public class QuestlineProgressUI : MonoBehaviour
{
  public Transform m_QuestCardBone;
  public Transform m_QuestRewardBone;
  public UberText m_ProgressText;
  public UberText m_QuestDetailTextLeft;
  public UberText m_QuestDetailTextRight;
  public UberText m_QuestRequirementText1;
  public UberText m_QuestRequirementText2;
  public UberText m_QuestRequirementText3;
  public GameObject m_RequirementCheckmark1;
  public GameObject m_RequirementCheckmark2;
  public GameObject m_RequirementCheckmark3;
  public GameObject m_QuestlinePart1FXReference;
  public GameObject m_QuestlinePart2FXReference;
  public GameObject m_QuestlineFinalFXReference;
  [Header("Reward Overlay Reference Settings")]
  public MeshRenderer m_RewardOverlayRenderer;
  public Texture m_MinionRewardOverlayTexture;
  public Texture m_LegendaryMinionRewardOverlayTexture;
  public Texture m_SpellRewardOverlayTexture;
  public Texture m_GoldenSpellRewardOverlayTexture;
  public Texture m_WeaponRewardOverlayTexture;
  public Texture m_LegendaryWeaponRewardOverlayTexture;
  public Texture m_HeroPowerRewardOverlayTexture;
  [Header("Reward Background Glow Reference Settings")]
  public MeshRenderer m_RewardPartBackGlowRenderer;
  public Material m_DefaultRewardPartBackGlowMaterial;
  public Material m_HeroPowerRewardPartBackGlowMaterial;
  public MeshRenderer m_RewardFinalBackGlowRenderer;
  public Material m_DefaultRewardFinalBackGlowMaterial;
  public Material m_HeroPowerRewardFinalBackGlowMaterial;
  private Actor m_originalQuestActor;
  private Actor m_questCardActor;
  private Actor m_questRewardActor;
  private bool m_isShown;
  private bool m_isResaturating;
  private ScreenEffectsHandle m_screenEffectsHandle;
  public const string SEEK_GUIDANCE = "SW_433";
  public const string DISCOVER_THE_VOID_SHARD = "SW_433t";
  public const string ILLUMINATE_THE_VOID = "SW_433t2";
  public const string SORCERERS_GAMBIT = "SW_450";
  public const string STALL_FOR_TIME = "SW_450t";
  public const string REACH_THE_PORTAL_ROOM = "SW_450t2";

  private void Awake() => this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);

  private void OnDestroy()
  {
    if (!this.m_isResaturating)
      return;
    this.m_screenEffectsHandle.ClearCallbacks();
  }

  public void SetOriginalQuestActor(Actor actor) => this.m_originalQuestActor = actor;

  public void Show()
  {
    this.m_isShown = true;
    this.gameObject.SetActive(true);
    this.UpdateActors();
    this.DesaturateBoard();
  }

  public void Hide()
  {
    this.m_isShown = false;
    this.gameObject.SetActive(false);
    this.StopDesaturate();
  }

  public void UpdateText(int currentQuestProgress, int questProgressTotal)
  {
    this.UpdateProgressText(currentQuestProgress, questProgressTotal);
    this.UpdateQuestDetailText();
    this.UpdateQuestRequirementText();
  }

  private void UpdateProgressText(int currentQuestProgress, int questProgressTotal) => this.m_ProgressText.Text = string.Format("{0}/{1}", (object) currentQuestProgress, (object) questProgressTotal);

  private void UpdateQuestDetailText()
  {
    Entity entity = this.m_originalQuestActor.GetEntity();
    if (entity.HasTag(GAME_TAG.QUESTLINE))
    {
      int tag = entity.GetTag(GAME_TAG.QUESTLINE_PART);
      switch (tag)
      {
        case 1:
        case 2:
        case 3:
          this.m_QuestDetailTextLeft.Text = GameStrings.Get("GAMEPLAY_QUESTLINE_PART_" + (object) tag);
          this.m_QuestDetailTextRight.Text = GameStrings.Get("GAMEPLAY_QUESTLINE_PART_" + (object) (tag + 1));
          this.m_QuestDetailTextLeft.gameObject.SetActive(true);
          this.m_QuestDetailTextRight.gameObject.SetActive(true);
          this.m_QuestlinePart1FXReference.SetActive(tag == 1);
          this.m_QuestlinePart2FXReference.SetActive(tag == 2);
          this.m_QuestlineFinalFXReference.SetActive(tag == 3);
          return;
      }
    }
    this.m_QuestDetailTextLeft.gameObject.SetActive(false);
    this.m_QuestDetailTextRight.gameObject.SetActive(false);
  }

  private void UpdateQuestRequirementText()
  {
    Entity entity = this.m_originalQuestActor.GetEntity();
    string cardId = entity.GetCardId();
    if (cardId == "SW_450" || cardId == "SW_450t" || cardId == "SW_450t2")
    {
      this.m_QuestRequirementText1.Text = GameStrings.Get("GLOBAL_SPELL_SCHOOL_FIRE");
      this.m_QuestRequirementText2.Text = GameStrings.Get("GLOBAL_SPELL_SCHOOL_FROST");
      this.m_QuestRequirementText3.Text = GameStrings.Get("GLOBAL_SPELL_SCHOOL_ARCANE");
    }
    else if (cardId == "SW_433")
    {
      this.m_QuestRequirementText1.Text = GameStrings.Get("GAMEPLAY_COST_2");
      this.m_QuestRequirementText2.Text = GameStrings.Get("GAMEPLAY_COST_3");
      this.m_QuestRequirementText3.Text = GameStrings.Get("GAMEPLAY_COST_4");
    }
    else if (cardId == "SW_433t")
    {
      this.m_QuestRequirementText1.Text = GameStrings.Get("GAMEPLAY_COST_5");
      this.m_QuestRequirementText2.Text = GameStrings.Get("GAMEPLAY_COST_6");
      this.m_QuestRequirementText3.Text = "";
    }
    else if (cardId == "SW_433t2")
    {
      this.m_QuestRequirementText1.Text = GameStrings.Get("GAMEPLAY_COST_7");
      this.m_QuestRequirementText2.Text = GameStrings.Get("GAMEPLAY_COST_8");
      this.m_QuestRequirementText3.Text = "";
    }
    else
    {
      this.m_ProgressText.gameObject.SetActive(true);
      this.m_QuestRequirementText1.gameObject.SetActive(false);
      this.m_QuestRequirementText2.gameObject.SetActive(false);
      this.m_QuestRequirementText3.gameObject.SetActive(false);
      this.m_RequirementCheckmark1.SetActive(false);
      this.m_RequirementCheckmark2.SetActive(false);
      this.m_RequirementCheckmark3.SetActive(false);
      return;
    }
    this.m_ProgressText.gameObject.SetActive(false);
    this.m_QuestRequirementText1.gameObject.SetActive(true);
    this.m_QuestRequirementText2.gameObject.SetActive(true);
    this.m_QuestRequirementText3.gameObject.SetActive(true);
    this.m_RequirementCheckmark1.SetActive(entity.HasTag(GAME_TAG.QUESTLINE_REQUIREMENT_MET_1));
    this.m_RequirementCheckmark2.SetActive(entity.HasTag(GAME_TAG.QUESTLINE_REQUIREMENT_MET_2));
    this.m_RequirementCheckmark3.SetActive(entity.HasTag(GAME_TAG.QUESTLINE_REQUIREMENT_MET_3));
  }

  private void Update()
  {
    if (!this.m_isShown || this.m_originalQuestActor.GetEntity().GetControllerSide() != Player.Side.FRIENDLY)
      return;
    foreach (Card card in GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards())
    {
      if (card.GetEntity().HasTag(GAME_TAG.QUEST_CONTRIBUTOR))
        LayerUtils.SetLayer(card.gameObject, GameLayer.IgnoreFullScreenEffects);
    }
  }

  private void DesaturateBoard() => this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.DesaturatePerspective);

  private void StopDesaturate()
  {
    this.m_isResaturating = true;
    this.m_screenEffectsHandle.StopEffect(new Action(this.OnStopDesaturateFinished));
  }

  private void OnStopDesaturateFinished()
  {
    if (this.m_originalQuestActor.GetEntity().GetControllerSide() == Player.Side.FRIENDLY)
    {
      foreach (Card card in GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards())
      {
        if (!card.IsMousedOver())
          LayerUtils.SetLayer(card.gameObject, GameLayer.Default);
      }
    }
    this.m_isResaturating = false;
  }

  private void UpdateActors()
  {
    this.UpdateQuestActor();
    this.UpdateRewardActor();
  }

  private void UpdateQuestActor()
  {
    if (!((UnityEngine.Object) this.m_questCardActor == (UnityEngine.Object) null) && this.m_questCardActor.GetEntityDef() == this.m_originalQuestActor.GetEntityDef())
      return;
    if ((UnityEngine.Object) this.m_questCardActor != (UnityEngine.Object) null)
      this.m_questCardActor.Destroy();
    GameObject go = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(this.m_originalQuestActor.GetEntity()), AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("QuestlineProgressUI.UpdateQuestCard(): Unable to load hand actor for entity {0}.", (object) this.m_originalQuestActor);
    }
    else
    {
      LayerUtils.SetLayer(go, this.m_QuestCardBone.gameObject.layer);
      go.transform.parent = this.m_QuestCardBone;
      TransformUtil.Identity(go);
      this.m_questCardActor = go.GetComponentInChildren<Actor>();
      this.m_questCardActor.SetEntity(this.m_originalQuestActor.GetEntity());
      this.m_questCardActor.SetCardDefFromActor(this.m_originalQuestActor);
      this.m_questCardActor.SetPremium(this.m_originalQuestActor.GetEntity().GetPremiumType());
      this.m_questCardActor.SetWatermarkCardSetOverride(this.m_originalQuestActor.GetEntity().GetWatermarkCardSetOverride());
      this.m_questCardActor.UpdateAllComponents();
    }
  }

  private void UpdateRewardActor()
  {
    Entity entity = this.m_originalQuestActor.GetEntity();
    string idFromQuestCardId = QuestlineController.GetRewardCardIDFromQuestCardID(entity);
    if (string.IsNullOrEmpty(idFromQuestCardId))
    {
      Log.Gameplay.PrintError("QuestlineProgressUI.UpdateRewardCard(): No reward card ID found for quest card ID {0}.", (object) entity.GetCardId());
    }
    else
    {
      if ((UnityEngine.Object) this.m_questRewardActor == (UnityEngine.Object) null || this.m_questRewardActor.GetEntityDef().GetCardId() != idFromQuestCardId)
      {
        if ((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null)
          this.m_questRewardActor.Destroy();
        using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(idFromQuestCardId))
        {
          if (cardDef == null)
          {
            Log.Gameplay.PrintError("QuestlineProgressUI.UpdateRewardCard(): Unable to load CardDef for card ID {0}.", (object) idFromQuestCardId);
            return;
          }
          EntityDef entityDef = DefLoader.Get().GetEntityDef(idFromQuestCardId);
          if (entityDef == null)
          {
            Log.Gameplay.PrintError("QuestlineProgressUI.UpdateRewardCard(): Unable to load EntityDef for card ID {0}.", (object) idFromQuestCardId);
            return;
          }
          GameObject go = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(entityDef, entity.GetPremiumType()), AssetLoadingOptions.IgnorePrefabPosition);
          if ((UnityEngine.Object) go == (UnityEngine.Object) null)
          {
            Log.Gameplay.PrintError("QuestlineProgressUI.UpdateRewardCard(): Unable to load Hand Actor for entity def {0}.", (object) entityDef);
            return;
          }
          LayerUtils.SetLayer(go, this.m_QuestRewardBone.gameObject.layer);
          go.transform.parent = this.m_QuestRewardBone;
          TransformUtil.Identity(go);
          this.m_questRewardActor = go.GetComponentInChildren<Actor>();
          this.m_questRewardActor.SetEntityDef(entityDef);
          this.m_questRewardActor.SetCardDef(cardDef);
          this.m_questRewardActor.SetPremium(this.m_originalQuestActor.GetEntity().GetPremiumType());
          this.m_questRewardActor.SetWatermarkCardSetOverride(this.m_originalQuestActor.GetEntity().GetWatermarkCardSetOverride());
          this.m_questRewardActor.UpdateAllComponents();
          this.UpdateRewardOverlayTexture(entityDef);
          this.UpdateRewardBackgroundGlowTexture(entityDef);
        }
      }
      if (!entity.HasTag(GAME_TAG.QUESTLINE) || entity.GetTag(GAME_TAG.QUESTLINE_PART) >= 3)
        return;
      this.m_questRewardActor.ActivateSpellBirthState(SpellType.GHOSTMODE);
    }
  }

  private void UpdateRewardOverlayTexture(EntityDef questRewardEntityDef)
  {
    if ((UnityEngine.Object) this.m_RewardOverlayRenderer == (UnityEngine.Object) null)
      return;
    Texture texture = (Texture) null;
    if (questRewardEntityDef.IsMinion())
      texture = questRewardEntityDef.IsElite() ? this.m_LegendaryMinionRewardOverlayTexture : this.m_MinionRewardOverlayTexture;
    else if (questRewardEntityDef.IsSpell())
      texture = this.m_questRewardActor.GetPremium() == TAG_PREMIUM.NORMAL ? this.m_SpellRewardOverlayTexture : this.m_GoldenSpellRewardOverlayTexture;
    else if (questRewardEntityDef.IsWeapon())
      texture = questRewardEntityDef.IsElite() ? this.m_LegendaryWeaponRewardOverlayTexture : this.m_WeaponRewardOverlayTexture;
    else if (questRewardEntityDef.IsHeroPower())
      texture = this.m_HeroPowerRewardOverlayTexture;
    if ((UnityEngine.Object) texture == (UnityEngine.Object) null)
      return;
    Material material = RendererExtension.GetMaterial((Renderer) this.m_RewardOverlayRenderer);
    material.SetTexture("_MainTex", texture);
    material.SetTexture("_AddTex", texture);
  }

  private void UpdateRewardBackgroundGlowTexture(EntityDef questRewardEntityDef)
  {
    if ((UnityEngine.Object) this.m_RewardPartBackGlowRenderer != (UnityEngine.Object) null)
    {
      Material backGlowMaterial = this.m_DefaultRewardPartBackGlowMaterial;
      if (questRewardEntityDef.IsHeroPower())
        backGlowMaterial = this.m_HeroPowerRewardPartBackGlowMaterial;
      RendererExtension.SetMaterial((Renderer) this.m_RewardPartBackGlowRenderer, backGlowMaterial);
    }
    if (!((UnityEngine.Object) this.m_RewardFinalBackGlowRenderer != (UnityEngine.Object) null))
      return;
    Material backGlowMaterial1 = this.m_DefaultRewardFinalBackGlowMaterial;
    if (questRewardEntityDef.IsHeroPower())
      backGlowMaterial1 = this.m_HeroPowerRewardFinalBackGlowMaterial;
    RendererExtension.SetMaterial((Renderer) this.m_RewardFinalBackGlowRenderer, backGlowMaterial1);
  }
}
