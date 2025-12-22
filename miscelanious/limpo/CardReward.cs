using Assets;
using Blizzard.T5.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardReward : Reward
{
  public GameObject m_nonHeroCardsRoot;
  public GameObject m_heroCardRoot;
  public GameObject m_cardParent;
  public GameObject m_duplicateCardParent;
  public CardRewardCount m_cardCount;
  public bool m_showCardCount = true;
  public bool m_RotateIn = true;
  private static readonly Map<TAG_CARDTYPE, Vector3> CARD_SCALE = new Map<TAG_CARDTYPE, Vector3>()
  {
    {
      TAG_CARDTYPE.SPELL,
      new Vector3(1f, 1f, 1f)
    },
    {
      TAG_CARDTYPE.MINION,
      new Vector3(1f, 1f, 1f)
    },
    {
      TAG_CARDTYPE.WEAPON,
      new Vector3(1f, 0.5f, 1f)
    },
    {
      TAG_CARDTYPE.HERO,
      new Vector3(1f, 1f, 1f)
    },
    {
      TAG_CARDTYPE.LOCATION,
      new Vector3(1f, 1f, 1f)
    }
  };
  private List<Actor> m_actors = new List<Actor>();
  private GameObject m_goToRotate;
  private CardSoundSpell m_emote;

  public void MakeActorsUnlit()
  {
    foreach (Actor actor in this.m_actors)
      actor.SetUnlit();
  }

  protected override void InitData() => this.SetData((RewardData) new CardRewardData(), false);

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals)
      return;
    if (!(this.Data is CardRewardData data))
      Debug.LogWarning((object) string.Format("CardReward.OnDataSet() - data {0} is not CardRewardData", (object) this.Data));
    else if (string.IsNullOrEmpty(data.CardID))
    {
      Debug.LogWarning((object) string.Format("CardReward.OnDataSet() - data {0} has invalid cardID", (object) data));
    }
    else
    {
      this.SetReady(false);
      EntityDef entityDef = DefLoader.Get().GetEntityDef(data.CardID);
      if (entityDef.IsHeroSkin())
      {
        string assetRef;
        if (data.Premium == TAG_PREMIUM.GOLDEN)
        {
          assetRef = "Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d";
          this.SetUpGoldenHeroAchieves();
        }
        else
        {
          CardHero.HeroType? heroType = GameUtils.GetHeroType(data);
          if (heroType.HasValue)
          {
            switch (heroType.GetValueOrDefault())
            {
              case CardHero.HeroType.VANILLA:
                assetRef = "Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d";
                this.SetHeroRewardText();
                goto label_15;
              case CardHero.HeroType.BATTLEGROUNDS_HERO:
                assetRef = "Card_Play_Bacon_Hero.prefab:227eb40f91281fa429c48c8a730c982f";
                this.SetBattlegroundsHeroRewardText();
                goto label_15;
              case CardHero.HeroType.BATTLEGROUNDS_GUIDE:
                assetRef = "Card_Play_Bacon_Guide.prefab:6cf6c56b1ef6f4c4db7210533b95f4ac";
                this.SetBattlegroundsGuideRewardText();
                goto label_15;
            }
          }
          assetRef = "Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d";
        }
label_15:
        AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, new PrefabCallback<GameObject>(this.OnHeroActorLoaded), (object) entityDef, AssetLoadingOptions.IgnorePrefabPosition);
        this.m_goToRotate = this.m_heroCardRoot;
        this.m_cardCount.Hide();
      }
      else
      {
        if ((bool) UniversalInputManager.UsePhoneUI || !this.m_showCardCount)
          this.m_cardCount.Hide();
        if (data.OriginData == 26L)
          this.SetSpecificCardRewardText(entityDef);
        else
          this.SetGenericRandomCardRewardText(entityDef);
        AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(entityDef, data.Premium), new PrefabCallback<GameObject>(this.OnActorLoaded), (object) entityDef, AssetLoadingOptions.IgnorePrefabPosition);
        this.m_goToRotate = this.m_nonHeroCardsRoot;
      }
    }
  }

  protected override void ShowReward(bool updateCacheValues)
  {
    CardRewardData data = this.Data as CardRewardData;
    this.InitRewardText();
    EntityDef entityDef = DefLoader.Get().GetEntityDef(data.CardID);
    if ((data.FixedReward != null && data.FixedReward.UseQuestToast || !GameUtils.IsVanillaHero(data.CardID)) && entityDef.IsHeroSkin() && (Object) this.m_rewardBanner != (Object) null)
      this.m_rewardBanner.gameObject.SetActive(false);
    if (!this.m_showCardCount || entityDef.GetRarity() == TAG_RARITY.LEGENDARY)
      this.m_cardCount.Hide();
    this.m_root.SetActive(true);
    if (this.m_RotateIn)
    {
      this.m_goToRotate.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180f);
      iTween.RotateAdd(this.m_goToRotate.gameObject, iTween.Hash((object) "amount", (object) new Vector3(0.0f, 0.0f, 540f), (object) "time", (object) 1.5f, (object) "easeType", (object) iTween.EaseType.easeOutElastic, (object) "space", (object) Space.Self));
    }
    SoundManager.Get().LoadAndPlay((AssetReference) "game_end_reward.prefab:6c28275a79f151a478d49afc04533e72");
    this.PlayHeroEmote();
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }

  private void OnFullDefLoaded(
    string cardID,
    DefLoader.DisposableFullDef fullDef,
    object callbackData)
  {
    using (fullDef)
    {
      if (fullDef == null)
        Debug.LogWarning((object) string.Format("CardReward.OnFullDefLoaded() - fullDef for CardID {0} is null", (object) cardID));
      else if (fullDef.EntityDef == null)
        Debug.LogWarning((object) string.Format("CardReward.OnFullDefLoaded() - entityDef for CardID {0} is null", (object) cardID));
      else if ((Object) fullDef.CardDef == (Object) null)
      {
        Debug.LogWarning((object) string.Format("CardReward.OnFullDefLoaded() - cardDef for CardID {0} is null", (object) cardID));
      }
      else
      {
        foreach (Actor actor in this.m_actors)
          this.FinishSettingUpActor(actor, fullDef.DisposableCardDef);
        foreach (EmoteEntryDef emoteDef in fullDef.CardDef.m_EmoteDefs)
        {
          if (emoteDef.m_emoteType == EmoteType.START)
            AssetLoader.Get().InstantiatePrefab((AssetReference) emoteDef.m_emoteSoundSpellPath, new PrefabCallback<GameObject>(this.OnStartEmoteLoaded));
        }
        this.SetReady(true);
      }
    }
  }

  private void OnStartEmoteLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
      return;
    CardSoundSpell component = go.GetComponent<CardSoundSpell>();
    if ((Object) component == (Object) null)
      return;
    this.m_emote = component;
  }

  private void PlayHeroEmote()
  {
    if ((Object) this.m_emote == (Object) null)
      return;
    this.m_emote.Reactivate();
  }

  private void OnHeroActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    EntityDef entityDef = (EntityDef) callbackData;
    Actor component = go.GetComponent<Actor>();
    component.SetEntityDef(entityDef);
    component.transform.parent = this.m_heroCardRoot.transform;
    component.transform.localScale = Vector3.one;
    component.transform.localPosition = Vector3.zero;
    component.transform.localRotation = Quaternion.identity;
    component.TurnOffCollider();
    if ((bool) (Object) component.m_healthObject)
      component.m_healthObject.SetActive(false);
    CardRewardData data = this.Data as CardRewardData;
    if (data.FixedReward != null && data.FixedReward.UseQuestToast || !GameUtils.IsVanillaHero(data.CardID))
    {
      PlatformDependentValue<Vector3> platformDependentValue1 = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
      {
        PC = new Vector3(1.35f, 1.35f, 1.35f),
        Phone = new Vector3(1.3f, 1.3f, 1.3f)
      };
      PlatformDependentValue<Vector3> platformDependentValue2 = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
      {
        PC = new Vector3(0.0f, 0.0f, -0.2f),
        Phone = new Vector3(0.0f, 0.0f, -0.3f)
      };
      component.transform.localScale = (Vector3) platformDependentValue1;
      component.transform.localPosition = (Vector3) platformDependentValue2;
    }
    LayerUtils.SetLayer(component.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.m_actors.Add(component);
    DefLoader.Get().LoadFullDef(entityDef.GetCardId(), new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnFullDefLoaded), (object) new CardPortraitQuality(3, TAG_PREMIUM.GOLDEN));
  }

  private void OnActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    EntityDef entityDef = (EntityDef) callbackData;
    if ((Object) go == (Object) null)
    {
      Log.MissingAssets.PrintWarning("CardReward.OnActorLoaded - Failed to load actor {0}", (object) assetRef);
    }
    else
    {
      Actor component = go.GetComponent<Actor>();
      if ((Object) component == (Object) null)
      {
        Log.MissingAssets.PrintWarning("CardReward.OnActorLoaded - No actor found in {0}", (object) assetRef);
      }
      else
      {
        this.StartSettingUpNonHeroActor(component, entityDef, this.m_cardParent.transform);
        CardRewardData data = this.Data as CardRewardData;
        this.m_cardCount.SetCount(data.Count);
        if (data.Count > 1)
          this.StartSettingUpNonHeroActor(Object.Instantiate<Actor>(component), entityDef, this.m_duplicateCardParent.transform);
        DefLoader.Get().LoadFullDef(entityDef.GetCardId(), new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnFullDefLoaded), (object) entityDef, new CardPortraitQuality(3, TAG_PREMIUM.GOLDEN));
      }
    }
  }

  private void StartSettingUpNonHeroActor(
    Actor actor,
    EntityDef entityDef,
    Transform parentTransform)
  {
    actor.SetEntityDef(entityDef);
    actor.transform.parent = parentTransform;
    TAG_CARDTYPE cardType = entityDef.GetCardType();
    if (!CardReward.CARD_SCALE.ContainsKey(cardType))
    {
      Debug.LogWarning((object) ("CardReward - No CARD_SCALE exists for card type " + (object) cardType));
      actor.transform.localScale = CardReward.CARD_SCALE[TAG_CARDTYPE.MINION];
    }
    else
      actor.transform.localScale = CardReward.CARD_SCALE[cardType];
    actor.transform.localPosition = Vector3.zero;
    actor.transform.localRotation = Quaternion.identity;
    actor.TurnOffCollider();
    if (this.Data.Origin != NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT)
      LayerUtils.SetLayer(actor.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.m_actors.Add(actor);
  }

  private void FinishSettingUpActor(Actor actor, DefLoader.DisposableCardDef cardDef)
  {
    CardRewardData data = this.Data as CardRewardData;
    actor.SetCardDef(cardDef);
    actor.SetPremium(data.Premium);
    actor.CreateBannedRibbon();
    actor.UpdateAllComponents();
  }

  private void SetHeroRewardText()
  {
    CardRewardData data = this.Data as CardRewardData;
    string className = GameStrings.GetClassName(DefLoader.Get().GetEntityDef(data.CardID).GetClass());
    NetCache.NetCacheHeroLevels netObject = NetCache.Get().GetNetObject<NetCache.NetCacheHeroLevels>();
    if (netObject == null)
      Debug.LogWarning((object) "Cannot fetch number of classes unlocked, NetCacheHeroLevels accessed before initialization. This can occur when bypassing the Box intro in QA via intro=false in options.txt.");
    int num1 = ((IEnumerable<TAG_CLASS>) GameUtils.ORDERED_HERO_CLASSES).Count<TAG_CLASS>();
    int num2 = netObject == null ? 0 : netObject.Levels.Count;
    this.SetRewardText(GameStrings.Format("GLOBAL_REWARD_HERO_HEADLINE", (object) className), GameStrings.Format("GLOBAL_REWARD_HERO_DETAILS", (object) num2, (object) num1), string.Empty);
  }

  private void SetUpGoldenHeroAchieves() => this.SetRewardText(GameStrings.Get("GLOBAL_REWARD_GOLDEN_HERO_HEADLINE"), string.Empty, string.Empty);

  private void SetBattlegroundsHeroRewardText() => this.SetRewardText(GameStrings.Get("GLOBAL_REWARD_BATTLEGROUNDS_HERO_HEADLINE"), string.Empty, string.Empty);

  private void SetBattlegroundsGuideRewardText() => this.SetRewardText(GameStrings.Get("GLOBAL_REWARD_BATTLEGROUNDS_GUIDE_HEADLINE"), string.Empty, string.Empty);

  private void SetGenericRandomCardRewardText(EntityDef entityDef) => this.SetRewardText(GameStrings.Get("GLUE_GENERIC_RANDOM_CARD_SCROLL_TITLE"), GameStrings.Format("GLUE_GENERIC_RANDOM_CARD_SCROLL_DESC", (object) entityDef.GetName()), string.Empty);

  private void SetSpecificCardRewardText(EntityDef entityDef) => this.SetRewardText(entityDef.GetName(), GameStrings.Get("GLUE_GENERIC_SPECIFIC_CARD_SCROLL_DESC"), string.Empty);

  private void InitRewardText()
  {
    CardRewardData data = this.Data as CardRewardData;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(data.CardID);
    if (entityDef.IsHeroSkin())
      return;
    string headline = GameStrings.Get("GLOBAL_REWARD_CARD_HEADLINE");
    string details = string.Empty;
    string empty = string.Empty;
    int cardSet = (int) entityDef.GetCardSet();
    TAG_CLASS tagClass = entityDef.GetClass();
    string className = GameStrings.GetClassName(tagClass);
    if (GameMgr.Get().IsTraditionalTutorial())
      details = GameUtils.GetCurrentTutorialCardRewardDetails();
    else if (entityDef.IsCoreCard())
    {
      int num = 16;
      if (tagClass == TAG_CLASS.NEUTRAL)
        num = 75;
      int coreCardsIown = CollectionManager.Get().GetCoreCardsIOwn(tagClass);
      if (data.Premium == TAG_PREMIUM.GOLDEN)
      {
        details = string.Empty;
      }
      else
      {
        if (num == coreCardsIown)
          data.InnKeeperLine = CardRewardData.InnKeeperTrigger.CORE_CLASS_SET_COMPLETE;
        else if (coreCardsIown == 4)
          data.InnKeeperLine = CardRewardData.InnKeeperTrigger.SECOND_REWARD_EVER;
        details = GameStrings.Format("GLOBAL_REWARD_CORE_CARD_DETAILS", (object) coreCardsIown, (object) num, (object) className);
      }
    }
    string source;
    if (this.Data.Origin == NetCache.ProfileNotice.NoticeOrigin.LEVEL_UP)
    {
      TAG_CLASS originData = (TAG_CLASS) this.Data.OriginData;
      source = GameStrings.Format("GLOBAL_REWARD_CARD_LEVEL_UP", (object) GameUtils.GetHeroLevel(originData).CurrentLevel.Level.ToString(), (object) GameStrings.GetClassName(originData));
    }
    else
      source = string.Empty;
    this.SetRewardText(headline, details, source);
  }
}
