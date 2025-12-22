using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class HistoryCard : HistoryItem
{
  public UberText m_createdByText;
  public static readonly Color OPPONENT_COLOR = new Color(0.7137f, 0.2f, 0.1333f, 1f);
  public static readonly Color FRIENDLY_COLOR = new Color(0.6509f, 0.6705f, 0.9843f, 1f);
  private const float ABILITY_CARD_ANIMATE_TO_BIG_CARD_AREA_TIME = 1f;
  private const float LETTUCE_ABILITY_ANIMATE_TO_BIG_CARD_AREA_TIME = 0.5f;
  private const float BIG_CARD_SCALE = 1.03f;
  private const float MOUSE_OVER_Z_OFFSET_TOP = -1.404475f;
  private const float MOUSE_OVER_Z_OFFSET_BOTTOM = 0.1681719f;
  private const float MOUSE_OVER_Z_OFFSET_PHONE = -4.75f;
  private const float MOUSE_OVER_Z_OFFSET_SECRET_PHONE = -4.3f;
  private const float MOUSE_OVER_Z_OFFSET_WITH_CREATOR_PHONE = -4.3f;
  private const float MOUSE_OVER_HEIGHT_OFFSET = 7.524521f;
  private PlatformDependentValue<float> MOUSE_OVER_X_OFFSET = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 4.326718f,
    Tablet = 4.7f,
    Phone = 5.4f
  };
  private PlatformDependentValue<float> MOUSE_OVER_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 1f,
    Tablet = 1f,
    Phone = 1f
  };
  private PlatformDependentValue<float> X_SIZE_OF_MOUSE_OVER_CHILD = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 2.5f,
    Tablet = 2.5f,
    Phone = 2.5f
  };
  private const float MAX_WIDTH_OF_CHILDREN = 5f;
  private const string CREATED_BY_BONE_NAME = "HistoryCreatedByBone";
  private Material m_fullTileMaterial;
  private Material m_halfTileMaterial;
  private bool m_mousedOver;
  private bool m_halfSize;
  private bool m_hasBeenShown;
  private Actor m_separator;
  private bool m_haveDisplayedCreator;
  private bool m_gameEntityMousedOver;
  private List<HistoryInfo> m_childInfos;
  private List<HistoryChildCard> m_historyChildren = new List<HistoryChildCard>();
  private HistoryInfo m_ownerInfo;
  private HistoryChildCard m_owner;
  private bool m_bigCardFinishedCallbackHasRun;
  private HistoryManager.BigCardFinishedCallback m_bigCardFinishedCallback;
  private bool m_bigCardCountered;
  private bool m_bigCardWaitingForSecret;
  private bool m_bigCardFromMetaData;
  private Entity m_bigCardPostTransformedEntity;
  private float m_tileSize;
  private int m_displayTimeMS;
  private HistoryInfoType m_historyInfoType;

  public void LoadMainCardActor()
  {
    string assetRef = !this.m_fatigue ? (!this.m_burned ? ActorNames.GetHistoryActor(this.m_entity, this.m_historyInfoType) : "Card_Hand_BurnAway.prefab:869912636c30bc244bace332571afc94") : "Card_Hand_Fatigue.prefab:ae394ca0bb29a964eb4c7eeb555f2fae";
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) gameObject == (Object) null)
    {
      Debug.LogWarningFormat("HistoryCard.LoadMainCardActor() - FAILED to load actor \"{0}\"", (object) assetRef);
    }
    else
    {
      Actor component = gameObject.GetComponent<Actor>();
      if ((Object) component == (Object) null)
      {
        Debug.LogWarningFormat("HistoryCard.LoadMainCardActor() - ERROR actor \"{0}\" has no Actor component", (object) assetRef);
      }
      else
      {
        this.m_mainCardActor = component;
        if (this.m_fatigue)
          this.m_mainCardActor.GetPowersText().Text = GameStrings.Get("GAMEPLAY_FATIGUE_HISTORY_TEXT");
        else if (this.m_burned)
        {
          this.m_mainCardActor.GetPowersText().Text = GameStrings.Get("GAMEPLAY_BURNED_CARDS_HISTORY_TEXT");
        }
        else
        {
          this.m_mainCardActor.SetCardDefFromEntity(this.m_entity);
          this.m_mainCardActor.SetPremium(this.m_entity.GetPremiumType());
          this.m_mainCardActor.SetWatermarkCardSetOverride(this.m_entity.GetWatermarkCardSetOverride());
        }
        this.m_mainCardActor.SetHistoryItem((HistoryItem) this);
        this.m_mainCardActor.UpdateAllComponents();
        if (this.m_mainCardActor.UseCoinManaGem())
          this.m_mainCardActor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
        this.InitDisplayedCreator();
      }
    }
  }

  private void InitDisplayedCreator()
  {
    if (this.m_entity == null)
      return;
    string displayedCreatorName = this.m_entity.GetDisplayedCreatorName();
    if (string.IsNullOrEmpty(displayedCreatorName))
      return;
    GameObject bone = this.m_mainCardActor.FindBone("HistoryCreatedByBone");
    if (!(bool) (Object) bone)
    {
      Error.AddDevWarning("Missing Bone", "Missing {0} on {1}", (object) "HistoryCreatedByBone", (object) this.m_mainCardActor);
    }
    else
    {
      this.m_createdByText.Text = GameStrings.Format("GAMEPLAY_HISTORY_CREATED_BY", (object) displayedCreatorName);
      this.m_createdByText.transform.parent = this.m_mainCardActor.GetRootObject().transform;
      this.m_createdByText.gameObject.SetActive(true);
      TransformUtil.SetPoint((Component) this.m_createdByText, new Vector3(0.5f, 0.0f, 1f), bone, new Vector3(0.5f, 0.0f, 0.0f));
      this.m_createdByText.gameObject.SetActive(false);
      this.m_haveDisplayedCreator = true;
    }
  }

  private void ShowDisplayedCreator() => this.m_createdByText.gameObject.SetActive(this.m_haveDisplayedCreator);

  public bool HasBeenShown() => this.m_hasBeenShown;

  public void MarkAsShown()
  {
    if (this.m_hasBeenShown)
      return;
    this.m_hasBeenShown = true;
  }

  public bool IsHalfSize() => this.m_halfSize;

  public float GetTileSize() => this.m_tileSize;

  public void LoadTile(HistoryTileInitInfo info)
  {
    this.m_childInfos = info.m_childInfos;
    this.m_ownerInfo = info.m_ownerInfo;
    if ((Object) info.m_fatigueTexture != (Object) null)
    {
      this.m_portraitTexture = info.m_fatigueTexture;
      this.m_fatigue = true;
    }
    else if ((Object) info.m_burnedCardsTexture != (Object) null)
    {
      this.m_portraitTexture = info.m_burnedCardsTexture;
      this.m_burned = true;
    }
    else
    {
      this.m_entity = info.m_entity;
      this.m_portraitTexture = info.m_portraitTexture;
      this.m_portraitGoldenMaterial = info.m_portraitGoldenMaterial;
      this.SetCardDef(info.m_cardDef);
      this.m_fullTileMaterial = info.m_fullTileMaterial;
      this.m_halfTileMaterial = info.m_halfTileMaterial;
      this.m_splatAmount = info.m_splatAmount;
      this.m_isPoisonous = info.m_isPoisonous;
      this.m_isCriticalHit = info.m_isCriticalHit;
      this.m_dead = info.m_dead;
    }
    this.m_historyInfoType = info.m_type;
    switch (info.m_type)
    {
      case HistoryInfoType.NONE:
      case HistoryInfoType.WEAPON_PLAYED:
      case HistoryInfoType.CARD_PLAYED:
      case HistoryInfoType.FATIGUE:
      case HistoryInfoType.BURNED_CARDS:
        this.LoadPlayTile();
        break;
      case HistoryInfoType.ATTACK:
        this.LoadAttackTile();
        break;
      case HistoryInfoType.TRIGGER:
        this.LoadTriggerTile();
        break;
      case HistoryInfoType.WEAPON_BREAK:
        this.LoadWeaponBreak();
        break;
    }
  }

  public void NotifyMousedOver()
  {
    if (this.m_mousedOver || (Object) this == (Object) HistoryManager.Get().GetCurrentBigCard())
      return;
    this.LoadChildCardsFromInfos();
    this.LoadOwnerFromInfo();
    this.m_mousedOver = true;
    SoundManager.Get().LoadAndPlay((AssetReference) "history_event_mouseover.prefab:0bc4f1638257a264a9b02e811c0a61b5", this.m_tileActor.gameObject);
    if (!(bool) (Object) this.m_mainCardActor)
    {
      this.LoadMainCardActor();
      LayerUtils.SetLayer((Component) this.m_mainCardActor, GameLayer.Tooltip);
    }
    this.ShowTile();
  }

  public void NotifyMousedOut()
  {
    if (!this.m_mousedOver)
      return;
    this.m_mousedOver = false;
    if (this.m_gameEntityMousedOver)
    {
      GameState.Get().GetGameEntity().NotifyOfHistoryTokenMousedOut();
      this.m_gameEntityMousedOver = false;
    }
    TooltipPanelManager.Get().HideKeywordHelp();
    if ((Object) this.m_owner != (Object) null && (Object) this.m_owner.m_mainCardActor != (Object) null)
    {
      this.m_owner.m_mainCardActor.ActivateAllSpellsDeathStates();
      this.m_owner.m_mainCardActor.Hide();
    }
    if ((bool) (Object) this.m_mainCardActor)
    {
      this.m_mainCardActor.ActivateAllSpellsDeathStates();
      this.m_mainCardActor.Hide();
    }
    for (int index = 0; index < this.m_historyChildren.Count; ++index)
    {
      if (!((Object) this.m_historyChildren[index].m_mainCardActor == (Object) null))
      {
        this.m_historyChildren[index].m_mainCardActor.ActivateAllSpellsDeathStates();
        this.m_historyChildren[index].m_mainCardActor.Hide();
      }
    }
    if ((bool) (Object) this.m_separator)
      this.m_separator.Hide();
    HistoryManager.Get().UpdateLayout();
  }

  private void LoadPlayTile()
  {
    this.m_halfSize = false;
    this.LoadTileImpl("HistoryTile_Card.prefab:df3002d4532e4dd40b37101e83202db4");
    this.LoadArrowSeparator();
  }

  private void LoadAttackTile()
  {
    this.m_halfSize = true;
    this.LoadTileImpl("HistoryTile_Attack.prefab:816bc6c1f4d8f0c439e981d30bf5b57b");
    this.LoadSwordsSeparator();
  }

  private void LoadWeaponBreak()
  {
    this.m_halfSize = true;
    this.LoadTileImpl("HistoryTile_Attack.prefab:816bc6c1f4d8f0c439e981d30bf5b57b");
  }

  private void LoadTriggerTile()
  {
    this.m_halfSize = true;
    this.LoadTileImpl("HistoryTile_Trigger.prefab:14cb236519ac3744b8c7c1274a379c94");
    this.LoadArrowSeparator();
  }

  private void LoadTileImpl(string actorPath)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) actorPath, AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) gameObject == (Object) null)
    {
      Debug.LogWarningFormat("HistoryCard.LoadTileImpl() - FAILED to load actor \"{0}\"", (object) actorPath);
    }
    else
    {
      Actor component = gameObject.GetComponent<Actor>();
      if ((Object) component == (Object) null)
      {
        Debug.LogWarningFormat("HistoryCard.LoadTileImpl() - ERROR actor \"{0}\" has no Actor component", (object) actorPath);
      }
      else
      {
        this.m_tileActor = component;
        this.m_tileActor.transform.parent = this.transform;
        TransformUtil.Identity((Component) this.m_tileActor.transform);
        this.m_tileActor.transform.localScale = HistoryManager.Get().transform.localScale;
        Material[] materialArray = new Material[2]
        {
          RendererExtension.GetMaterial((Renderer) this.m_tileActor.GetMeshRenderer()),
          null
        };
        if (this.m_halfSize)
        {
          if ((Object) this.m_halfTileMaterial != (Object) null)
          {
            materialArray[1] = this.m_halfTileMaterial;
            RendererExtension.SetMaterials((Renderer) this.m_tileActor.GetMeshRenderer(), materialArray);
          }
          else
            RendererExtension.GetMaterial((Renderer) this.m_tileActor.GetMeshRenderer(), 1).mainTexture = this.m_portraitTexture;
        }
        else if ((Object) this.m_fullTileMaterial != (Object) null)
        {
          materialArray[1] = this.m_fullTileMaterial;
          RendererExtension.SetMaterials((Renderer) this.m_tileActor.GetMeshRenderer(), materialArray);
        }
        else
          RendererExtension.GetMaterial((Renderer) this.m_tileActor.GetMeshRenderer(), 1).mainTexture = this.m_portraitTexture;
        Color color1 = Color.white;
        if ((Object) Board.Get() != (Object) null)
          color1 = Board.Get().m_HistoryTileColor;
        Color color2 = this.m_fatigue || this.m_burned ? (!this.AffectsFriendlySidePlayer() ? color1 * HistoryCard.OPPONENT_COLOR : color1 * HistoryCard.FRIENDLY_COLOR) : (!this.m_entity.IsControlledByFriendlySidePlayer() ? color1 * HistoryCard.OPPONENT_COLOR : color1 * HistoryCard.FRIENDLY_COLOR);
        foreach (Renderer componentsInChild in this.m_tileActor.GetMeshRenderer().GetComponentsInChildren<Renderer>())
        {
          if (!componentsInChild.CompareTag(HistoryItem.RENDERER_TAG))
            RendererExtension.GetMaterial(componentsInChild).color = Board.Get().m_HistoryTileColor;
        }
        List<Material> materials = RendererExtension.GetMaterials((Renderer) this.m_tileActor.GetMeshRenderer());
        materials[0].color = color2;
        materials[1].color = Board.Get().m_HistoryTileColor;
        if (!((Object) this.GetTileCollider() != (Object) null))
          return;
        this.m_tileSize = this.GetTileCollider().bounds.size.z;
      }
    }
  }

  private bool AffectsFriendlySidePlayer() => this.m_childInfos != null && this.m_childInfos.Count != 0 && this.m_childInfos[0] != null && this.m_childInfos[0].GetDuplicatedEntity() != null && this.m_childInfos[0].GetDuplicatedEntity().IsControlledByFriendlySidePlayer();

  private void LoadSwordsSeparator() => this.LoadSeparator("History_Swords.prefab:361feac100313e443b68055167e5088c");

  private void LoadArrowSeparator()
  {
    if (this.m_childInfos == null || this.m_childInfos.Count == 0)
      return;
    this.LoadSeparator("History_Arrow.prefab:a9ef1ff267ab0a24c9cdef7f3678b5a4");
  }

  private void LoadSeparator(string actorPath)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) actorPath, AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) gameObject == (Object) null)
    {
      Debug.LogWarning((object) string.Format("HistoryCard.LoadSeparator() - FAILED to load actor \"{0}\"", (object) actorPath));
    }
    else
    {
      Actor component1 = gameObject.GetComponent<Actor>();
      if ((Object) component1 == (Object) null)
      {
        Debug.LogWarning((object) string.Format("HistoryCard.LoadSeparator() - ERROR actor \"{0}\" has no Actor component", (object) actorPath));
      }
      else
      {
        this.m_separator = component1;
        MeshRenderer component2 = this.m_separator.GetRootObject().transform.Find("Blue").gameObject.GetComponent<MeshRenderer>();
        MeshRenderer component3 = this.m_separator.GetRootObject().transform.Find("Red").gameObject.GetComponent<MeshRenderer>();
        if (this.m_fatigue || this.m_burned)
        {
          component3.enabled = true;
          component2.enabled = false;
        }
        else
        {
          bool flag = this.m_entity.IsControlledByFriendlySidePlayer();
          component2.enabled = flag;
          component3.enabled = !flag;
        }
        this.m_separator.transform.parent = this.transform;
        TransformUtil.Identity((Component) this.m_separator.transform);
        if ((Object) this.m_separator.GetRootObject() != (Object) null)
          TransformUtil.Identity((Component) this.m_separator.GetRootObject().transform);
        this.m_separator.Hide();
      }
    }
  }

  private void LoadOwnerFromInfo()
  {
    if (this.m_ownerInfo == null)
      return;
    this.m_owner = this.LoadHistoryChildCard(this.m_ownerInfo);
    this.m_ownerInfo = (HistoryInfo) null;
  }

  private void LoadChildCardsFromInfos()
  {
    if (this.m_childInfos == null)
      return;
    foreach (HistoryInfo childInfo in this.m_childInfos)
    {
      HistoryChildCard historyChildCard = this.LoadHistoryChildCard(childInfo);
      if ((Object) historyChildCard != (Object) null)
        this.m_historyChildren.Add(historyChildCard);
    }
    this.m_childInfos = (List<HistoryInfo>) null;
  }

  private HistoryChildCard LoadHistoryChildCard(HistoryInfo info)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "HistoryChildCard.prefab:f85dbd296f9764f4e9c6a2c638a024d3", AssetLoadingOptions.IgnorePrefabPosition);
    HistoryChildCard component = gameObject.GetComponent<HistoryChildCard>();
    Entity duplicatedEntity = info.GetDuplicatedEntity();
    if (duplicatedEntity == null)
    {
      Log.Gameplay.PrintError(string.Format("{0}.{1}: {2} has a null duplicated entity!", (object) nameof (HistoryCard), (object) nameof (LoadHistoryChildCard), (object) info));
      return (HistoryChildCard) null;
    }
    using (DefLoader.DisposableCardDef cardDef = duplicatedEntity.ShareDisposableCardDef())
    {
      if ((Object) cardDef?.CardDef == (Object) null)
        return (HistoryChildCard) null;
      component.SetCardInfo(duplicatedEntity, cardDef, info.GetSplatAmount(), info.HasDied(), info.m_isBurnedCard, info.m_isPoisonous, info.m_isCriticalHit);
      component.transform.parent = this.transform;
      component.LoadMainCardActor();
      Actor componentInChildren = gameObject.GetComponentInChildren<Actor>();
      if ((Object) componentInChildren == (Object) null)
        return (HistoryChildCard) null;
      componentInChildren.SetEntity(duplicatedEntity);
      componentInChildren.SetCardDef(cardDef);
      componentInChildren.UpdateAllComponents();
    }
    return component;
  }

  private void ShowTile()
  {
    if (!this.m_mousedOver)
    {
      this.m_mainCardActor.Hide();
    }
    else
    {
      this.m_mainCardActor.Show();
      this.ShowDisplayedCreator();
      this.InitializeMainCardActor();
      this.DisplaySpells();
      float x = this.transform.position.x + (float) this.MOUSE_OVER_X_OFFSET;
      float y = this.transform.position.y + 7.524521f;
      float z1 = (bool) UniversalInputManager.UsePhoneUI ? this.GetZOffsetForThisTilesMouseOverCard() : this.transform.position.z + this.GetZOffsetForThisTilesMouseOverCard();
      if ((Object) this.m_owner != (Object) null)
      {
        this.m_owner.m_mainCardActor.Show();
        this.m_owner.InitializeMainCardActor();
        this.m_owner.DisplaySpells();
        this.m_owner.m_mainCardActor.UpdateAllComponents();
        this.m_owner.m_mainCardActor.transform.position = new Vector3(x, y, z1);
        this.m_owner.m_mainCardActor.transform.localScale = new Vector3((float) this.MOUSE_OVER_SCALE, 1f, (float) this.MOUSE_OVER_SCALE);
        x += (float) this.X_SIZE_OF_MOUSE_OVER_CHILD;
      }
      this.m_mainCardActor.transform.position = new Vector3(x, y, z1);
      this.m_mainCardActor.transform.localScale = new Vector3((float) this.MOUSE_OVER_SCALE, 1f, (float) this.MOUSE_OVER_SCALE);
      if ((bool) UniversalInputManager.UsePhoneUI && (this.m_fatigue || this.m_burned))
        this.m_mainCardActor.transform.localScale = new Vector3(1f, 1f, 1f);
      if (!this.m_gameEntityMousedOver)
      {
        this.m_gameEntityMousedOver = true;
        GameState.Get().GetGameEntity().NotifyOfHistoryTokenMousedOver(this.gameObject);
      }
      if (!this.m_fatigue && !this.m_burned)
        TooltipPanelManager.Get().UpdateKeywordHelpForHistoryCard(this.m_entity, this.m_mainCardActor, this.m_createdByText);
      if (this.m_historyChildren.Count <= 0)
        return;
      int num1 = 4;
      int num2 = 8;
      if ((Object) this.m_owner != (Object) null)
      {
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          num1 = 1;
          num2 = 4;
        }
        else
        {
          num1 = 3;
          num2 = 8;
        }
      }
      int num3 = 3;
      if (this.m_historyChildren.Count <= num1)
        num3 = 1;
      else if (this.m_historyChildren.Count <= num2)
        num3 = 2;
      float max = 1f;
      switch (num3)
      {
        case 2:
          max = 0.5f;
          break;
        case 3:
          max = 0.3f;
          break;
      }
      int num4 = Mathf.CeilToInt((float) this.m_historyChildren.Count / (float) num3);
      float num5 = Mathf.Clamp(5f / ((float) num4 * (float) this.X_SIZE_OF_MOUSE_OVER_CHILD), 0.1f, max);
      int num6 = 0;
      int num7 = 1;
      for (int index = 0; index < this.m_historyChildren.Count; ++index)
      {
        this.m_historyChildren[index].m_mainCardActor.Show();
        this.m_historyChildren[index].InitializeMainCardActor();
        this.m_historyChildren[index].DisplaySpells();
        this.m_historyChildren[index].m_mainCardActor.UpdateAllComponents();
        float z2 = this.m_mainCardActor.transform.position.z;
        switch (num3)
        {
          case 2:
            if (num7 == 1)
            {
              z2 += 0.78f;
              break;
            }
            z2 -= 0.78f;
            break;
          case 3:
            switch (num7)
            {
              case 1:
                z2 += 0.98f;
                break;
              case 3:
                z2 -= 0.93f;
                break;
            }
            break;
        }
        float num8 = this.m_mainCardActor.transform.position.x + (float) ((double) (float) this.X_SIZE_OF_MOUSE_OVER_CHILD * (1.0 + (double) num5) / 2.0);
        this.m_historyChildren[index].m_mainCardActor.transform.position = new Vector3(num8 + (float) this.X_SIZE_OF_MOUSE_OVER_CHILD * (float) num6 * num5, this.m_mainCardActor.transform.position.y, z2);
        this.m_historyChildren[index].m_mainCardActor.transform.localScale = new Vector3(num5, num5, num5);
        ++num6;
        if (num6 >= num4)
        {
          num6 = 0;
          ++num7;
        }
      }
      if (!((Object) this.m_separator != (Object) null))
        return;
      float num9 = 0.4f;
      float num10 = (float) this.X_SIZE_OF_MOUSE_OVER_CHILD / 2f;
      this.m_separator.Show();
      this.m_separator.transform.position = new Vector3(this.m_mainCardActor.transform.position.x + num10, this.m_mainCardActor.transform.position.y + num9, this.m_mainCardActor.transform.position.z);
    }
  }

  private float GetZOffsetForThisTilesMouseOverCard()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return this.m_entity != null && this.m_entity.IsSecret() && this.m_entity.IsHidden() || this.m_haveDisplayedCreator ? -4.3f : -4.75f;
    double num = (double) Mathf.Abs(-1.572647f);
    HistoryManager historyManager = HistoryManager.Get();
    double numHistoryTiles = (double) historyManager.GetNumHistoryTiles();
    return (float) (num / numHistoryTiles * (double) (historyManager.GetNumHistoryTiles() - historyManager.GetIndexForTile(this) - 1) - 1.40447497367859);
  }

  public void LoadBigCard(HistoryBigCardInitInfo info)
  {
    this.m_entity = info.m_entity;
    this.m_historyInfoType = info.m_historyInfoType;
    this.m_portraitTexture = info.m_portraitTexture;
    this.SetCardDef(info.m_cardDef);
    this.m_portraitGoldenMaterial = info.m_portraitGoldenMaterial;
    this.m_bigCardFinishedCallback = info.m_finishedCallback;
    this.m_bigCardCountered = info.m_countered;
    this.m_bigCardWaitingForSecret = info.m_waitForSecretSpell;
    this.m_bigCardFromMetaData = info.m_fromMetaData;
    this.m_bigCardPostTransformedEntity = info.m_postTransformedEntity;
    this.m_displayTimeMS = info.m_displayTimeMS;
    this.LoadMainCardActor();
  }

  public void LoadBigCardPostTransformedEntity()
  {
    if (this.m_bigCardPostTransformedEntity == null)
      return;
    this.m_entity = this.m_bigCardPostTransformedEntity;
    Card card = this.m_entity.GetCard();
    this.m_portraitTexture = card.GetPortraitTexture(this.m_entity.GetPremiumType());
    this.m_portraitGoldenMaterial = card.GetGoldenMaterial();
    using (DefLoader.DisposableCardDef cardDef = card.ShareDisposableCardDef())
      this.SetCardDef(cardDef);
    this.LoadMainCardActor();
  }

  public HistoryManager.BigCardFinishedCallback GetBigCardFinishedCallback() => this.m_bigCardFinishedCallback;

  public void RunBigCardFinishedCallback()
  {
    if (this.m_bigCardFinishedCallbackHasRun)
      return;
    this.m_bigCardFinishedCallbackHasRun = true;
    if (this.m_bigCardFinishedCallback == null)
      return;
    this.m_bigCardFinishedCallback();
  }

  public bool WasBigCardCountered() => this.m_bigCardCountered;

  public int GetDisplayTimeMS() => this.m_displayTimeMS;

  public bool IsCastedByLettuceCharacter() => this.m_entity.GetLettuceAbilityOwner() != null;

  public bool IsBigCardWaitingForSecret() => this.m_bigCardWaitingForSecret;

  public bool IsBigCardFromMetaData() => this.m_bigCardFromMetaData;

  public Entity GetBigCardPostTransformedEntity() => this.m_bigCardPostTransformedEntity;

  public bool HasBigCardPostTransformedEntity() => this.m_bigCardPostTransformedEntity != null;

  public void ShowBigCard(Vector3[] pathToFollow)
  {
    this.m_mainCardActor.transform.localScale = new Vector3(1.03f, 1.03f, 1.03f);
    Entity entity = this.m_entity;
    if (this.HasBigCardPostTransformedEntity())
      entity = this.m_bigCardPostTransformedEntity;
    if (entity == null)
      return;
    float num = 1f;
    if (entity.IsLettuceAbility())
      num = 0.5f;
    if (this.m_displayTimeMS > 0)
    {
      float b = (float) this.m_displayTimeMS / 1000f;
      num = Mathf.Min(num, b);
    }
    if (entity.IsSpell() || entity.IsHeroPower() || entity.IsLettuceAbility() || this.m_bigCardFromMetaData)
    {
      pathToFollow[0] = this.m_mainCardActor.transform.position;
      iTween.MoveTo(this.m_mainCardActor.gameObject, iTween.Hash((object) "path", (object) pathToFollow, (object) "time", (object) num, (object) "oncomplete", (object) "OnBigCardPathComplete", (object) "oncompletetarget", (object) this.gameObject));
      iTween.ScaleTo(this.gameObject, new Vector3(1f, 1f, 1f), num);
      SoundManager.Get().LoadAndPlay((AssetReference) "play_card_from_hand_1.prefab:ac4be75e319a97947a68308a08e54e88");
    }
    else
      this.ShowDisplayedCreator();
  }

  private void OnBigCardPathComplete() => this.ShowDisplayedCreator();
}
