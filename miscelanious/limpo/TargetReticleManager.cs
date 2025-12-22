using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetReticleManager : MonoBehaviour
{
  private const int MAX_TARGET_ARROW_LINKS = 15;
  private const float LINK_Y_LENGTH = 1f;
  private const float LENGTH_BETWEEN_LINKS = 1.2f;
  private const float LINK_PARABOLA_HEIGHT_NORMAL = 1.5f;
  public const float LINK_PARABOLA_HEIGHT_MERCENARIES = 0.4f;
  private const float LINK_ANIMATION_SPEED = 0.5f;
  private const float STARTING_X_ROTATION_FOR_DEFAULT_ARROW = 300f;
  private static readonly PlatformDependentValue<bool> SHOW_DAMAGE_INDICATOR_ON_ENTITY = new PlatformDependentValue<bool>(PlatformCategory.Input)
  {
    Mouse = false,
    Touch = true
  };
  private static readonly PlatformDependentValue<float> DAMAGE_INDICATOR_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 2.5f,
    Tablet = 3.75f
  };
  private static readonly PlatformDependentValue<float> DAMAGE_INDICATOR_Z_OFFSET = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 0.75f,
    Tablet = -1.2f
  };
  private const float FRIENDLY_HERO_ORIGIN_Z_OFFSET = 1f;
  private const float LINK_FADE_OFFSET = -1.2f;
  private static TargetReticleManager s_instance;
  private bool m_isActive;
  private bool m_showArrow = true;
  private bool m_bullseyeAlwaysOn;
  private int m_originLocationEntityID = -1;
  private int m_sourceEntityID = -1;
  private int m_numActiveLinks;
  private float m_linkAnimationZOffset;
  private float m_parabolaHeight = 1.5f;
  private Vector3 m_targetArrowOrigin;
  private Vector3 m_remoteArrowPosition;
  private GameObject m_arrow;
  private TargetDamageIndicator m_damageIndicator;
  private GameObject m_hunterReticle;
  private GameObject m_questionMark;
  private List<GameObject> m_targetArrowLinks;
  private TARGET_RETICLE_TYPE m_reticleType;
  private TARGET_ARROW_TYPE m_targetArrowType;
  private bool m_useHandAsOrigin;

  private void Awake() => TargetReticleManager.s_instance = this;

  private void OnDestroy() => TargetReticleManager.s_instance = (TargetReticleManager) null;

  public static TargetReticleManager Get() => TargetReticleManager.s_instance;

  public bool IsActive() => (Object) this.GetAppropriateReticle() != (Object) null && this.m_isActive;

  public bool IsLocalArrow() => this.m_targetArrowType != TARGET_ARROW_TYPE.Enemy;

  public bool IsEnemyArrow() => this.m_targetArrowType == TARGET_ARROW_TYPE.Enemy;

  public bool IsStaticArrow() => this.m_targetArrowType == TARGET_ARROW_TYPE.Static;

  public bool IsLocalArrowActive() => this.m_targetArrowType != TARGET_ARROW_TYPE.Enemy && this.IsActive();

  public bool IsEnemyArrowActive() => this.m_targetArrowType == TARGET_ARROW_TYPE.Enemy && this.IsActive();

  public bool ShouldPreventMouseOverBigCard() => this.IsActive() && !this.IsStaticArrow();

  public int ArrowSourceEntityID => this.m_originLocationEntityID;

  public void ShowBullseye(bool show)
  {
    if (this.m_bullseyeAlwaysOn && !show)
      return;
    if (this.m_reticleType == TARGET_RETICLE_TYPE.DefaultArrow)
    {
      if (!this.IsActive() || !this.m_showArrow)
        return;
      Transform transform = this.m_arrow.transform.Find("TargetArrow_TargetMesh");
      if (!(bool) (Object) transform)
        return;
      RenderUtils.EnableRenderers(transform.gameObject, show);
    }
    else if (this.m_reticleType == TARGET_RETICLE_TYPE.HunterReticle)
    {
      if ((Object) this.m_hunterReticle == (Object) null)
        return;
      BlitToTexture component = this.m_hunterReticle.GetComponent<BlitToTexture>();
      if ((Object) component == (Object) null)
        return;
      Material material = RendererExtension.GetMaterial(component.DrawAfterBlit.GetComponent<Renderer>());
      if ((Object) material == (Object) null)
        return;
      if (show)
        material.color = Color.red;
      else
        material.color = Color.white;
    }
    else
    {
      if (this.m_reticleType != TARGET_RETICLE_TYPE.QuestionMark || !this.IsActive() || !this.m_showArrow)
        return;
      Transform transform = this.m_questionMark.transform.Find("TargetQuestionMark_TargetMesh");
      if (!(bool) (Object) transform)
        return;
      RenderUtils.EnableRenderers(transform.gameObject, show);
    }
  }

  public void CreateFriendlyTargetArrow(
    Entity sourceEntity,
    bool showDamageIndicatorText,
    bool showArrow = true,
    string overrideText = null,
    bool useHandAsOrigin = false,
    bool isAttackArrow = false)
  {
    if ((GameMgr.Get() == null ? 0 : (GameMgr.Get().IsSpectator() ? 1 : 0)) == 0)
      this.DisableCollidersForUntargetableCards(sourceEntity.GetCard());
    if (GameState.Get().GetGameEntity().HasTag(GAME_TAG.ALL_TARGETS_RANDOM))
      this.m_reticleType = TARGET_RETICLE_TYPE.QuestionMark;
    else if (sourceEntity.HasTag(GAME_TAG.TARGETING_ARROW_TYPE) && !isAttackArrow)
    {
      this.m_reticleType = (TARGET_RETICLE_TYPE) sourceEntity.GetTag(GAME_TAG.TARGETING_ARROW_TYPE);
    }
    else
    {
      Spell playSpell = sourceEntity.GetCard().GetPlaySpell(0);
      this.m_reticleType = !((Object) playSpell != (Object) null) ? TARGET_RETICLE_TYPE.DefaultArrow : playSpell.m_TargetReticle;
    }
    this.SetParabolaHeight(1.5f);
    string damageIndicatorText = (string) null;
    if (overrideText != null)
      damageIndicatorText = overrideText;
    else if (showDamageIndicatorText)
      damageIndicatorText = sourceEntity.GetTargetingArrowText();
    Entity entity = sourceEntity;
    if (sourceEntity.IsSpell())
      entity = sourceEntity.GetHero();
    else if (sourceEntity.IsLettuceAbility())
    {
      Entity lettuceAbilityOwner = sourceEntity.GetLettuceAbilityOwner();
      if (lettuceAbilityOwner != null)
        entity = lettuceAbilityOwner;
    }
    this.CreateTargetArrow(TARGET_ARROW_TYPE.Friendly, entity != null ? entity.GetEntityId() : 0, sourceEntity.GetEntityId(), damageIndicatorText, showArrow, useHandAsOrigin);
    this.AttachLinksToAppropriateReticle();
    this.SetTargetArrowLinkLayer(GameLayer.Tooltip);
  }

  public void RefreshTargetingArrowText(Entity sourceEntity)
  {
    string targetingArrowText = sourceEntity.GetTargetingArrowText();
    if (this.IsEnemyArrow())
      return;
    this.StartCoroutine(this.SetDamageText(targetingArrowText));
  }

  private void AttachLinksToAppropriateReticle()
  {
    GameObject appropriateReticle = this.GetAppropriateReticle();
    foreach (GameObject targetArrowLink in this.m_targetArrowLinks)
      targetArrowLink.transform.parent = appropriateReticle.transform;
  }

  public void CreateEnemyTargetArrow(Entity originEntity)
  {
    this.m_reticleType = !GameState.Get().GetGameEntity().HasTag(GAME_TAG.ALL_TARGETS_RANDOM) ? TARGET_RETICLE_TYPE.DefaultArrow : TARGET_RETICLE_TYPE.QuestionMark;
    this.SetParabolaHeight(1.5f);
    this.CreateTargetArrow(TARGET_ARROW_TYPE.Enemy, originEntity.GetEntityId(), originEntity.GetEntityId(), (string) null, true);
    this.AttachLinksToAppropriateReticle();
    this.SetTargetArrowLinkLayer(GameLayer.Tooltip);
  }

  public void CreateStaticTargetArrow(Entity originEntity, Entity targetEntity)
  {
    if (originEntity == null || targetEntity == null)
    {
      Log.Gameplay.PrintError("Unable to create static target arrow. Null entities provided.");
    }
    else
    {
      this.m_reticleType = TARGET_RETICLE_TYPE.DefaultArrow;
      this.m_targetArrowOrigin = targetEntity.GetCard().transform.position;
      this.m_remoteArrowPosition = this.m_targetArrowOrigin;
      this.m_arrow.transform.position = this.m_targetArrowOrigin;
      this.SetParabolaHeight(1.5f);
      this.CreateTargetArrow(TARGET_ARROW_TYPE.Static, originEntity.GetEntityId(), originEntity.GetEntityId(), (string) null, true);
      this.m_bullseyeAlwaysOn = true;
      this.ShowBullseye(true);
      this.AttachLinksToAppropriateReticle();
      this.SetTargetArrowLinkLayer(GameLayer.Tooltip);
    }
  }

  public void DestroyEnemyTargetArrow() => this.DestroyTargetArrow(TARGET_ARROW_TYPE.Enemy, false);

  public void DestroyStaticTargetArrow() => this.DestroyTargetArrow(TARGET_ARROW_TYPE.Static, false);

  public void DestroyFriendlyTargetArrow(bool isLocallyCanceled)
  {
    this.EnableCollidersThatWereDisabled();
    this.DestroyTargetArrow(TARGET_ARROW_TYPE.Friendly, isLocallyCanceled);
  }

  public void UpdateArrowPosition()
  {
    if (!this.IsActive())
      return;
    if (!this.m_showArrow)
    {
      this.UpdateArrowOriginPosition();
      this.UpdateDamageIndicator();
    }
    else
    {
      bool flag = GameMgr.Get() != null && GameMgr.Get().IsSpectator();
      Vector3 point;
      if (this.IsEnemyArrow() | flag || this.IsStaticArrow())
      {
        Vector3 zero = Vector3.zero;
        Vector3 position = this.GetAppropriateReticle().transform.position;
        point.x = Mathf.Lerp(position.x, this.m_remoteArrowPosition.x, 0.1f);
        point.y = Mathf.Lerp(position.y, this.m_remoteArrowPosition.y, 0.1f);
        point.z = Mathf.Lerp(position.z, this.m_remoteArrowPosition.z, 0.1f);
        Card card = this.IsEnemyArrow() ? RemoteActionHandler.Get().GetOpponentHeldCard() : RemoteActionHandler.Get().GetFriendlyHeldCard();
        if ((Object) card != (Object) null)
        {
          if (card.GetEntity().GetZone() != TAG_ZONE.DECK)
            this.m_targetArrowOrigin = card.transform.position;
          if ((Object) card.GetActor() == (Object) null)
          {
            Card heroCard1 = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
            Card heroCard2 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
            if (this.m_targetArrowType == TARGET_ARROW_TYPE.Enemy && (Object) heroCard1 != (Object) null)
              this.m_targetArrowOrigin = heroCard1.transform.position;
            else if (this.m_targetArrowType == TARGET_ARROW_TYPE.Friendly && (Object) heroCard2 != (Object) null)
              this.m_targetArrowOrigin = heroCard2.transform.position;
          }
        }
      }
      else
      {
        RaycastHit hitInfo;
        if (!UniversalInputManager.Get().GetInputHitInfo(Camera.main, GameLayer.DragPlane, out hitInfo))
          return;
        point = hitInfo.point;
        this.UpdateArrowOriginPosition();
      }
      float y = 57.29578f * Mathf.Atan2(point.x - this.m_targetArrowOrigin.x, point.z - this.m_targetArrowOrigin.z);
      if (this.m_reticleType == TARGET_RETICLE_TYPE.DefaultArrow || this.m_reticleType == TARGET_RETICLE_TYPE.QuestionMark)
      {
        GameObject appropriateReticle = this.GetAppropriateReticle();
        appropriateReticle.transform.localEulerAngles = new Vector3(0.0f, y, 0.0f);
        appropriateReticle.transform.position = point;
        this.UpdateTargetArrowLinks(Mathf.Sqrt(Mathf.Pow(this.m_targetArrowOrigin.x - point.x, 2f) + Mathf.Pow(this.m_targetArrowOrigin.z - point.z, 2f)));
      }
      else if (this.m_reticleType == TARGET_RETICLE_TYPE.HunterReticle)
        this.m_hunterReticle.transform.position = point;
      else
        Debug.LogError((object) "Unknown Target Reticle Type!");
      this.UpdateDamageIndicator();
    }
  }

  public void SetRemotePlayerArrowPosition(Vector3 newPosition) => this.m_remoteArrowPosition = newPosition;

  private void DestroyCurrentArrow(bool isLocallyCanceled)
  {
    if (this.IsEnemyArrow())
      this.DestroyEnemyTargetArrow();
    else
      this.DestroyFriendlyTargetArrow(isLocallyCanceled);
  }

  private void DisableCollidersForUntargetableCards(Card sourceCard)
  {
    List<Card> cards = new List<Card>();
    foreach (Player player in GameState.Get().GetPlayerMap().Values)
    {
      this.AddUntargetableCard(sourceCard, cards, player.GetHeroPowerCard());
      this.AddUntargetableCard(sourceCard, cards, player.GetWeaponCard());
      foreach (Card card in player.GetSecretZone().GetCards())
        this.AddUntargetableCard(sourceCard, cards, card);
    }
    foreach (Card card in cards)
    {
      if (!((Object) card == (Object) null))
      {
        Actor actor = card.GetActor();
        if (!((Object) actor == (Object) null))
          actor.TurnOffCollider();
      }
    }
  }

  private void AddUntargetableCard(Card sourceCard, List<Card> cards, Card card)
  {
    if ((Object) sourceCard == (Object) card)
      return;
    cards.Add(card);
  }

  private void EnableCollidersThatWereDisabled()
  {
    List<Card> cardList = new List<Card>();
    foreach (Player player in GameState.Get().GetPlayerMap().Values)
    {
      cardList.Add(player.GetHeroPowerCard());
      cardList.Add(player.GetWeaponCard());
      foreach (Card card in player.GetSecretZone().GetCards())
        cardList.Add(card);
    }
    foreach (Card card in cardList)
    {
      if (!((Object) card == (Object) null) && !((Object) card.GetActor() == (Object) null))
        card.GetActor().TurnOnCollider();
    }
  }

  private void CreateTargetArrow(
    TARGET_ARROW_TYPE targetArrowType,
    int originLocationEntityID,
    int sourceEntityID,
    string damageIndicatorText,
    bool showArrow,
    bool useHandAsOrigin = false)
  {
    if (this.IsActive())
    {
      Log.Gameplay.Print("Uh-oh... creating a targeting arrow but one is already active...");
      this.DestroyCurrentArrow(false);
    }
    this.m_targetArrowType = targetArrowType;
    this.m_sourceEntityID = sourceEntityID;
    this.m_originLocationEntityID = originLocationEntityID;
    this.m_showArrow = showArrow;
    this.m_useHandAsOrigin = useHandAsOrigin;
    this.UpdateArrowOriginPosition();
    bool flag = GameMgr.Get() != null && GameMgr.Get().IsSpectator();
    if (this.IsEnemyArrow() | flag && !this.IsStaticArrow())
    {
      this.m_remoteArrowPosition = this.m_targetArrowOrigin;
      this.m_arrow.transform.position = this.m_targetArrowOrigin;
    }
    this.ActivateArrow(true);
    this.ShowBullseye(false);
    this.ShowDamageIndicator(!this.IsEnemyArrow());
    this.UpdateArrowPosition();
    if (this.IsEnemyArrow())
      return;
    this.StartCoroutine(this.SetDamageText(damageIndicatorText));
    if (flag || this.IsStaticArrow())
      return;
    PegCursor.Get().Hide();
  }

  public void PreloadTargetArrows(PrefabInstanceLoadTracker.Context context)
  {
    this.m_targetArrowLinks = new List<GameObject>();
    PrefabInstanceLoadTracker.Get().InstantiatePrefab(context, (AssetReference) "Target_Arrow_Bullseye.prefab:7afe007e5f455b04b9407307d8df1983", new PrefabCallback<GameObject>(this.LoadArrowCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    PrefabInstanceLoadTracker.Get().InstantiatePrefab(context, (AssetReference) "TargetDamageIndicator.prefab:91b47a1196e64e946a974becc0fb29f1", new PrefabCallback<GameObject>(this.LoadDamageIndicatorCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    PrefabInstanceLoadTracker.Get().InstantiatePrefab(context, (AssetReference) "Target_Arrow_Link.prefab:eb929158148ae954881c5684d27a1aa2", new PrefabCallback<GameObject>(this.LoadLinkCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    PrefabInstanceLoadTracker.Get().InstantiatePrefab(context, (AssetReference) "HunterReticle.prefab:83c7a1ebe50ef476f891c1b39dd5fd88", new PrefabCallback<GameObject>(this.LoadHunterReticleCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    PrefabInstanceLoadTracker.Get().InstantiatePrefab(context, (AssetReference) "Target_Question_Mark.prefab:adc81f6922c3de840b0e071ac55c7d62", new PrefabCallback<GameObject>(this.LoadQuestionCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void DestroyTargetArrow(TARGET_ARROW_TYPE arrowType, bool isLocallyCanceled)
  {
    if (!this.IsActive())
      return;
    if (arrowType != this.m_targetArrowType)
    {
      Log.Gameplay.Print(string.Format("trying to destroy {0} arrow but the active arrow is {1}", (object) arrowType.ToString(), (object) this.m_targetArrowType.ToString()));
    }
    else
    {
      if (isLocallyCanceled)
        GameState.Get().GetEntity(this.m_sourceEntityID)?.GetCard().NotifyTargetingCanceled();
      this.m_originLocationEntityID = -1;
      this.m_sourceEntityID = -1;
      if (!this.IsEnemyArrow())
      {
        if (!this.IsStaticArrow())
          RemoteActionHandler.Get().NotifyOpponentOfTargetEnd();
        PegCursor.Get().Show();
      }
      this.ActivateArrow(false);
      this.ShowDamageIndicator(false);
    }
  }

  private void LoadArrowCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_arrow = go;
    this.ShowBullseye(false);
  }

  private void LoadQuestionCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_questionMark = go;
    this.ShowBullseye(false);
  }

  private void LoadLinkCallback(AssetReference assetRef, GameObject go, object callbackData) => this.StartCoroutine(this.OnLinkLoaded(go));

  private void LoadDamageIndicatorCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_damageIndicator = go.GetComponent<TargetDamageIndicator>();
    if ((Object) this.m_damageIndicator == (Object) null)
    {
      Log.Gameplay.PrintError("LoadDamageIndicatorCallback - No TargetDamageIndicator script attached to '{0}'!", (object) go.name);
    }
    else
    {
      this.m_damageIndicator.transform.eulerAngles = new Vector3(90f, 0.0f, 0.0f);
      this.m_damageIndicator.transform.localScale = new Vector3((float) TargetReticleManager.DAMAGE_INDICATOR_SCALE, (float) TargetReticleManager.DAMAGE_INDICATOR_SCALE, (float) TargetReticleManager.DAMAGE_INDICATOR_SCALE);
    }
  }

  private void LoadHunterReticleCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_hunterReticle = go;
    this.m_hunterReticle.transform.parent = this.transform;
    this.m_hunterReticle.SetActive(false);
  }

  private IEnumerator OnLinkLoaded(GameObject linkActorObject)
  {
    while ((Object) this.m_arrow == (Object) null)
      yield return (object) null;
    for (int index = 0; index < 14; ++index)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(linkActorObject);
      gameObject.transform.parent = this.m_arrow.transform;
      this.m_targetArrowLinks.Add(gameObject);
    }
    linkActorObject.transform.parent = this.m_arrow.transform;
    this.m_targetArrowLinks.Add(linkActorObject);
  }

  private int NumberOfRequiredLinks(float lengthOfArrow)
  {
    int num = (int) Mathf.Floor(lengthOfArrow / 1.2f) + 1;
    if (num == 1)
      num = 0;
    return num;
  }

  private GameObject GetAppropriateReticle()
  {
    switch (this.m_reticleType)
    {
      case TARGET_RETICLE_TYPE.DefaultArrow:
        return this.m_arrow;
      case TARGET_RETICLE_TYPE.HunterReticle:
        return this.m_hunterReticle;
      case TARGET_RETICLE_TYPE.QuestionMark:
        return this.m_questionMark;
      default:
        Log.All.PrintError("Unknown Target Reticle Type!");
        return (GameObject) null;
    }
  }

  private Transform GetAppropriateArrowMeshTransform()
  {
    switch (this.m_reticleType)
    {
      case TARGET_RETICLE_TYPE.DefaultArrow:
      case TARGET_RETICLE_TYPE.HunterReticle:
        return this.m_arrow.transform.Find("TargetArrow_ArrowMesh");
      case TARGET_RETICLE_TYPE.QuestionMark:
        return this.m_questionMark.transform.Find("TargetQuestionMark_QuestionMarkMesh");
      default:
        Log.All.PrintError("Unknown Target Reticle Type!");
        return (Transform) null;
    }
  }

  private float GetStartingXRotationForArrowMesh()
  {
    switch (this.m_reticleType)
    {
      case TARGET_RETICLE_TYPE.DefaultArrow:
      case TARGET_RETICLE_TYPE.HunterReticle:
        return 300f;
      case TARGET_RETICLE_TYPE.QuestionMark:
        return 0.0f;
      default:
        Log.All.PrintError("Unknown Target Reticle Type!");
        return 0.0f;
    }
  }

  private void UpdateTargetArrowLinks(float lengthOfArrow)
  {
    this.m_numActiveLinks = this.NumberOfRequiredLinks(lengthOfArrow);
    int count = this.m_targetArrowLinks.Count;
    Transform arrowMeshTransform = this.GetAppropriateArrowMeshTransform();
    if (this.m_numActiveLinks == 0)
    {
      arrowMeshTransform.localEulerAngles = new Vector3(this.GetStartingXRotationForArrowMesh(), 180f, 0.0f);
      for (int index = 0; index < count; ++index)
        RenderUtils.EnableRenderers(this.m_targetArrowLinks[index].gameObject, false);
    }
    else
    {
      float num1 = (float) (-(double) lengthOfArrow / 2.0);
      float num2 = (float) (-(double) this.m_parabolaHeight / ((double) num1 * (double) num1));
      for (int index = 0; index < count; ++index)
      {
        if (!((Object) this.m_targetArrowLinks[index] == (Object) null))
        {
          if (index >= this.m_numActiveLinks)
          {
            RenderUtils.EnableRenderers(this.m_targetArrowLinks[index].gameObject, false);
          }
          else
          {
            float z = (float) -(1.20000004768372 * (double) (index + 1)) + this.m_linkAnimationZOffset;
            float y = num2 * Mathf.Pow(z - num1, 2f) + this.m_parabolaHeight;
            float x = (float) (180.0 - (double) Mathf.Atan((float) (2.0 * (double) num2 * ((double) z - (double) num1))) * 57.2957801818848);
            RenderUtils.EnableRenderers(this.m_targetArrowLinks[index].gameObject, true);
            this.m_targetArrowLinks[index].transform.localPosition = new Vector3(0.0f, y, z);
            this.m_targetArrowLinks[index].transform.eulerAngles = new Vector3(x, this.GetAppropriateReticle().transform.localEulerAngles.y, 0.0f);
            float alpha = 1f;
            if (index == 0)
            {
              if ((double) z > -1.20000004768372)
                alpha = Mathf.Pow(z / -1.2f, 6f);
            }
            else if (index == this.m_numActiveLinks - 1)
            {
              float num3 = this.m_linkAnimationZOffset / 1.2f;
              alpha = num3 * num3;
            }
            this.SetLinkAlpha(this.m_targetArrowLinks[index], alpha);
          }
        }
      }
      float y1 = num2 * Mathf.Pow(arrowMeshTransform.localPosition.z - num1, 2f) + this.m_parabolaHeight;
      float x1 = 0.0f;
      if (this.m_reticleType != TARGET_RETICLE_TYPE.QuestionMark)
      {
        x1 = Mathf.Atan((float) (2.0 * (double) num2 * ((double) arrowMeshTransform.localPosition.z - (double) num1))) * 57.29578f;
        if ((double) x1 < 0.0)
          x1 += 360f;
      }
      arrowMeshTransform.localPosition = new Vector3(0.0f, y1, arrowMeshTransform.localPosition.z);
      arrowMeshTransform.localEulerAngles = new Vector3(x1, 180f, 0.0f);
      this.m_linkAnimationZOffset += Time.deltaTime * 0.5f;
      if ((double) this.m_linkAnimationZOffset <= 1.20000004768372)
        return;
      this.m_linkAnimationZOffset -= 1.2f;
    }
  }

  private void SetLinkAlpha(GameObject linkGameObject, float alpha)
  {
    alpha = Mathf.Clamp(alpha, 0.0f, 1f);
    foreach (Renderer component in linkGameObject.GetComponents<Renderer>())
    {
      Material material = RendererExtension.GetMaterial(component);
      material.color = material.color with { a = alpha };
    }
  }

  private void UpdateDamageIndicator()
  {
    if ((Object) this.m_damageIndicator == (Object) null)
      return;
    Vector3 zero = Vector3.zero;
    Vector3 vector3;
    if ((bool) TargetReticleManager.SHOW_DAMAGE_INDICATOR_ON_ENTITY)
    {
      vector3 = this.m_targetArrowOrigin;
      vector3.z += (float) TargetReticleManager.DAMAGE_INDICATOR_Z_OFFSET;
    }
    else
    {
      vector3 = this.GetAppropriateReticle().transform.position;
      vector3.z += (float) TargetReticleManager.DAMAGE_INDICATOR_Z_OFFSET;
    }
    this.m_damageIndicator.transform.position = vector3;
  }

  private void ShowDamageIndicator(bool show)
  {
    if (!(bool) (Object) this.m_damageIndicator || !this.m_damageIndicator.gameObject.activeInHierarchy)
      return;
    this.m_damageIndicator.Show(show);
  }

  private IEnumerator SetDamageText(string damageText)
  {
    while ((Object) this.m_damageIndicator == (Object) null)
      yield return (object) null;
    this.m_damageIndicator.SetText(damageText);
    if (string.IsNullOrEmpty(damageText))
      this.m_damageIndicator.Show(false);
  }

  private void UpdateArrowOriginPosition()
  {
    Entity entity = GameState.Get().GetEntity(this.m_originLocationEntityID);
    if (entity == null && !this.m_useHandAsOrigin)
    {
      Log.Gameplay.Print(string.Format("Can't update arrow origin position because nothing was specified! (m_originLocationEntityID = {0}, m_useHandAsOrigin = {1})", (object) this.m_originLocationEntityID, (object) this.m_useHandAsOrigin));
      this.DestroyCurrentArrow(false);
    }
    else
    {
      if (entity != null)
        this.m_targetArrowOrigin = entity.GetCard().transform.position;
      if (this.m_useHandAsOrigin || entity != null && entity.GetZone() == TAG_ZONE.DECK)
        this.m_targetArrowOrigin = !this.IsEnemyArrow() ? InputManager.Get().GetFriendlyHand().transform.position : InputManager.Get().GetEnemyHand().transform.position;
      if (entity == null || !entity.IsHero() || this.IsEnemyArrow())
        return;
      ++this.m_targetArrowOrigin.z;
    }
  }

  private void ActivateArrow(bool active)
  {
    this.m_isActive = active;
    RenderUtils.EnableRenderers(this.m_arrow.gameObject, false);
    this.m_hunterReticle.SetActive(false);
    RenderUtils.EnableRenderers(this.m_questionMark.gameObject, false);
    if (!active)
      return;
    if (this.m_reticleType == TARGET_RETICLE_TYPE.DefaultArrow)
      RenderUtils.EnableRenderers(this.m_arrow.gameObject, active && this.m_showArrow);
    else if (this.m_reticleType == TARGET_RETICLE_TYPE.HunterReticle)
      this.m_hunterReticle.SetActive(active && this.m_showArrow);
    else if (this.m_reticleType == TARGET_RETICLE_TYPE.QuestionMark)
      RenderUtils.EnableRenderers(this.m_questionMark.gameObject, active && this.m_showArrow);
    else
      Debug.LogError((object) "Unknown Target Reticle Type!");
  }

  public void ShowArrow(bool show)
  {
    this.m_showArrow = show;
    RenderUtils.EnableRenderers(this.m_arrow.gameObject, false);
    this.m_hunterReticle.SetActive(false);
    RenderUtils.EnableRenderers(this.m_questionMark.gameObject, false);
    if (!show)
      return;
    if (this.m_reticleType == TARGET_RETICLE_TYPE.DefaultArrow)
      RenderUtils.EnableRenderers(this.m_arrow.gameObject, show);
    else if (this.m_reticleType == TARGET_RETICLE_TYPE.HunterReticle)
      this.m_hunterReticle.SetActive(show);
    else if (this.m_reticleType == TARGET_RETICLE_TYPE.QuestionMark)
      RenderUtils.EnableRenderers(this.m_questionMark.gameObject, show);
    else
      Debug.LogError((object) "Unknown Target Reticle Type!");
  }

  public void SetTargetArrowLinkLayer(GameLayer layer)
  {
    foreach (GameObject targetArrowLink in this.m_targetArrowLinks)
      targetArrowLink.layer = (int) layer;
  }

  public void SetParabolaHeight(float newHeight) => this.m_parabolaHeight = newHeight;
}
