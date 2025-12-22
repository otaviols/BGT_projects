using Blizzard.T5.MaterialService.Extensions;
using System;
using UnityEngine;

public class QuestProgressUI : MonoBehaviour
{
  public Transform m_QuestCardBone;
  public Transform m_QuestRewardBone;
  public UberText m_ProgressText;
  public UberText m_QuestDetailText;
  public GameObject m_Standard_Arrow;
  public GameObject m_Battlegrounds_Arrow;
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
  public MeshRenderer m_RewardBackGlowRenderer;
  public Material m_DefaultRewardBackGlowMaterial;
  public Material m_HeroPowerRewardBackGlowMaterial;
  private Actor m_originalQuestActor;
  private Actor m_questCardActor;
  private Actor m_questRewardActor;
  private bool m_isShown;
  private bool m_isResaturating;
  private ScreenEffectsHandle m_screenEffectsHandle;

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
  }

  private void UpdateProgressText(int currentQuestProgress, int questProgressTotal) => this.m_ProgressText.Text = string.Format("{0}/{1}", (object) currentQuestProgress, (object) questProgressTotal);

  private void UpdateQuestDetailText()
  {
    Entity entity = this.m_originalQuestActor.GetEntity();
    if (entity.HasTag(GAME_TAG.QUEST_CONTRIBUTOR))
    {
      int tag = entity.GetTag(GAME_TAG.QUEST_CONTRIBUTOR);
      EntityDef entityDef = DefLoader.Get().GetEntityDef(tag);
      if (entityDef != null)
      {
        this.m_QuestDetailText.Text = entityDef.GetName();
        this.m_QuestDetailText.gameObject.SetActive(true);
        return;
      }
    }
    this.m_QuestDetailText.gameObject.SetActive(false);
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
    this.UpdateArrow();
  }

  private void UpdateArrow()
  {
    if ((UnityEngine.Object) this.m_Battlegrounds_Arrow != (UnityEngine.Object) null && GameMgr.Get() != null && GameMgr.Get().IsBattlegrounds())
    {
      this.m_Battlegrounds_Arrow.SetActive(true);
      this.m_Standard_Arrow?.SetActive(false);
    }
    else
    {
      this.m_Standard_Arrow?.SetActive(true);
      this.m_Battlegrounds_Arrow?.SetActive(false);
    }
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
      Log.Gameplay.PrintError("QuestProgressUI.UpdateQuestCard(): Unable to load hand actor for entity {0}.", (object) this.m_originalQuestActor);
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
    string idFromQuestCardId = QuestController.GetRewardCardIDFromQuestCardID(entity);
    if (string.IsNullOrEmpty(idFromQuestCardId))
    {
      Log.Gameplay.PrintError("QuestProgressUI.UpdateRewardCard(): No reward card ID found for quest card ID {0}.", (object) entity.GetCardId());
    }
    else
    {
      if (!((UnityEngine.Object) this.m_questRewardActor == (UnityEngine.Object) null) && !(this.m_questRewardActor.GetEntityDef().GetCardId() != idFromQuestCardId))
        return;
      if ((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null)
        this.m_questRewardActor.Destroy();
      using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(idFromQuestCardId))
      {
        if (cardDef == null)
        {
          Log.Gameplay.PrintError("QuestProgressUI.UpdateRewardCard(): Unable to load CardDef for card ID {0}.", (object) idFromQuestCardId);
        }
        else
        {
          EntityDef entityDef = DefLoader.Get().GetEntityDef(idFromQuestCardId);
          if (entityDef == null)
          {
            Log.Gameplay.PrintError("QuestProgressUI.UpdateRewardCard(): Unable to load EntityDef for card ID {0}.", (object) idFromQuestCardId);
          }
          else
          {
            GameObject go = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(entityDef, entity.GetPremiumType()), AssetLoadingOptions.IgnorePrefabPosition);
            if ((UnityEngine.Object) go == (UnityEngine.Object) null)
            {
              Log.Gameplay.PrintError("QuestProgressUI.UpdateRewardCard(): Unable to load Hand Actor for entity def {0}.", (object) entityDef);
            }
            else
            {
              LayerUtils.SetLayer(go, this.m_QuestRewardBone.gameObject.layer);
              go.transform.parent = this.m_QuestRewardBone;
              TransformUtil.Identity(go);
              this.m_questRewardActor = go.GetComponentInChildren<Actor>();
              this.m_questRewardActor.SetEntityDef(entityDef);
              this.m_questRewardActor.SetCardDef(cardDef);
              this.m_questRewardActor.SetPremium(this.m_originalQuestActor.GetEntity().GetPremiumType());
              this.m_questRewardActor.SetWatermarkCardSetOverride(this.m_originalQuestActor.GetEntity().GetWatermarkCardSetOverride());
              this.m_questRewardActor.UpdateDynamicTextFromQuestEntity(this.m_originalQuestActor.GetEntity());
              if (this.m_questRewardActor.UseCoinManaGem())
                this.m_questRewardActor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
              this.m_questRewardActor.UpdateAllComponents();
              this.UpdateRewardOverlayTexture(entityDef);
              this.UpdateRewardBackgroundGlowTexture(entityDef);
            }
          }
        }
      }
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
    if ((UnityEngine.Object) this.m_RewardBackGlowRenderer == (UnityEngine.Object) null)
      return;
    Material backGlowMaterial = this.m_DefaultRewardBackGlowMaterial;
    if (questRewardEntityDef.IsHeroPower())
      backGlowMaterial = this.m_HeroPowerRewardBackGlowMaterial;
    RendererExtension.SetMaterial((Renderer) this.m_RewardBackGlowRenderer, backGlowMaterial);
  }
}
