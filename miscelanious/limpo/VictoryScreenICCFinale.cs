using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreenICCFinale : VictoryScreen
{
  public Animation m_BurnAwayAnimation;
  public Renderer m_LichPortraitRenderer;
  public string m_PortraitTextureName;
  public AudioSource m_PreLichTransformationAudio;
  public AudioSource m_MidLichTransformationAudio;
  public AudioSource m_HideTwoScoopsAudio;
  public List<AudioSource> m_HeroPostcardShowAudio;
  public AudioSource m_HeroPostcardHideAudio;
  public UberText m_HeroNameText;
  public UberText m_HeroDescriptionText;
  public UIBButton m_ContinueButton;
  public GameObject m_HeroPostcard;
  public MeshRenderer m_HeroPostcardRenderer;
  public Material m_DruidPostcardMaterial;
  public Material m_HunterPostcardMaterial;
  public Material m_MagePostcardMaterial;
  public Material m_PaladinPostcardMaterial;
  public Material m_PriestPostcardMaterial;
  public Material m_RoguePostcardMaterial;
  public Material m_ShamanPostcardMaterial;
  public Material m_WarlockPostcardMaterial;
  public Material m_WarriorPostcardMaterial;
  private Map<TAG_CLASS, string> m_heroDesriptions = new Map<TAG_CLASS, string>()
  {
    {
      TAG_CLASS.DRUID,
      "ICCDruidVictory_01"
    },
    {
      TAG_CLASS.HUNTER,
      "ICCHunterVictory_01"
    },
    {
      TAG_CLASS.MAGE,
      "ICCMageVictory_01"
    },
    {
      TAG_CLASS.PALADIN,
      "ICCPaladinVictory_01"
    },
    {
      TAG_CLASS.PRIEST,
      "ICCPriestVictory_01"
    },
    {
      TAG_CLASS.ROGUE,
      "ICCRogueVictory_01"
    },
    {
      TAG_CLASS.SHAMAN,
      "ICCShamanVictory_01"
    },
    {
      TAG_CLASS.WARLOCK,
      "ICCWarlockVictory_01"
    },
    {
      TAG_CLASS.WARRIOR,
      "ICCWarriorVictory_01"
    }
  };
  private Map<TAG_CLASS, string> m_heroMap = new Map<TAG_CLASS, string>()
  {
    {
      TAG_CLASS.DRUID,
      "HERO_06"
    },
    {
      TAG_CLASS.HUNTER,
      "HERO_05"
    },
    {
      TAG_CLASS.MAGE,
      "HERO_08"
    },
    {
      TAG_CLASS.PALADIN,
      "HERO_04"
    },
    {
      TAG_CLASS.PRIEST,
      "HERO_09"
    },
    {
      TAG_CLASS.ROGUE,
      "HERO_03"
    },
    {
      TAG_CLASS.SHAMAN,
      "HERO_02"
    },
    {
      TAG_CLASS.WARLOCK,
      "HERO_07"
    },
    {
      TAG_CLASS.WARRIOR,
      "HERO_01"
    }
  };
  private Map<TAG_CLASS, string> m_dkHeroMap = new Map<TAG_CLASS, string>()
  {
    {
      TAG_CLASS.DRUID,
      "ICC_832"
    },
    {
      TAG_CLASS.HUNTER,
      "ICC_828"
    },
    {
      TAG_CLASS.MAGE,
      "ICC_833"
    },
    {
      TAG_CLASS.PALADIN,
      "ICC_829"
    },
    {
      TAG_CLASS.PRIEST,
      "ICC_830"
    },
    {
      TAG_CLASS.ROGUE,
      "ICC_827"
    },
    {
      TAG_CLASS.SHAMAN,
      "ICC_481"
    },
    {
      TAG_CLASS.WARLOCK,
      "ICC_831"
    },
    {
      TAG_CLASS.WARRIOR,
      "ICC_834"
    }
  };
  private bool m_dismissedTwoScoops;
  private DefLoader.DisposableCardDef m_heroSkinCardDef;
  private static readonly float TIRION_VO_DELAY = 5f;
  private static readonly float POSTCARD_DELAY = 2f;

  protected override void Awake()
  {
    base.Awake();
    Card card = GameState.Get().GetFriendlySidePlayer().GetStartingHero().GetCard();
    Entity entity = card.GetEntity();
    TAG_CLASS key1 = card.GetEntity().GetClass();
    string cardId1;
    this.m_heroMap.TryGetValue(key1, out cardId1);
    if (!string.IsNullOrEmpty(cardId1))
    {
      if (cardId1 == entity.GetCardId())
      {
        this.m_LichPortraitRenderer.GetMaterial().SetTexture(this.m_PortraitTextureName, card.GetPortraitTexture());
      }
      else
      {
        this.m_heroSkinCardDef?.Dispose();
        this.m_heroSkinCardDef = DefLoader.Get().GetCardDef(cardId1);
        if ((Object) this.m_heroSkinCardDef?.CardDef != (Object) null)
          this.m_LichPortraitRenderer.GetMaterial().SetTexture(this.m_PortraitTextureName, this.m_heroSkinCardDef.CardDef.GetPortraitTexture(TAG_PREMIUM.NORMAL));
      }
    }
    VictoryTwoScoop twoScoop = this.m_twoScoop as VictoryTwoScoop;
    if ((Object) twoScoop == (Object) null)
      Log.Gameplay.PrintError("VictoryScreenICCPrologue.Awake() - m_twoScoop is not an instance of VictoryTwoScoop!");
    string cardId2;
    this.m_dkHeroMap.TryGetValue(key1, out cardId2);
    if (!string.IsNullOrEmpty(cardId2))
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId2);
      if (entityDef != null)
      {
        this.m_HeroNameText.Text = entityDef.GetName();
        twoScoop.SetOverrideHero(entityDef);
      }
    }
    string key2;
    this.m_heroDesriptions.TryGetValue(key1, out key2);
    if (string.IsNullOrEmpty(key2))
      return;
    this.m_HeroDescriptionText.Text = GameStrings.Get(key2);
  }

  protected override void OnDestroy()
  {
    this.m_heroSkinCardDef?.Dispose();
    this.m_heroSkinCardDef = (DefLoader.DisposableCardDef) null;
    base.OnDestroy();
  }

  protected override void ShowStandardFlow() => this.ShowTwoScoop();

  protected override void OnTwoScoopShown()
  {
    base.OnTwoScoopShown();
    this.StartCoroutine(this.PlayAnim());
  }

  private IEnumerator PlayAnim()
  {
    VictoryScreenICCFinale victoryScreenIccFinale = this;
    yield return (object) new WaitForSeconds(VictoryScreenICCFinale.TIRION_VO_DELAY);
    while (NotificationManager.Get().IsQuotePlaying)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor().GetSpell(SpellType.ENDGAME_WIN).ActivateState(SpellStateType.DEATH);
    VictoryTwoScoop twoScoop = victoryScreenIccFinale.m_twoScoop as VictoryTwoScoop;
    if ((Object) twoScoop != (Object) null)
      twoScoop.StopFireworksAudio();
    else
      Log.Gameplay.PrintError("VictoryScreenICCFinale.PlayAnim(): m_twoScoop is not of type VictoryTwoScoop!");
    if (GameState.Get().GetGameEntity() is ICC_08_Finale missionEntity)
    {
      yield return (object) victoryScreenIccFinale.StartCoroutine(missionEntity.PlayTirionVictoryVO());
      Coroutine enumerator = victoryScreenIccFinale.StartCoroutine(missionEntity.PlayFriendlyHeroVictoryVO(victoryScreenIccFinale.m_twoScoop.m_heroActor, victoryScreenIccFinale.m_PreLichTransformationAudio, victoryScreenIccFinale.m_MidLichTransformationAudio));
      yield return (object) new WaitForSeconds(1f);
      victoryScreenIccFinale.m_BurnAwayAnimation["LichHeroBurnAway"].speed = 0.4f;
      victoryScreenIccFinale.m_BurnAwayAnimation.Play("LichHeroBurnAway");
      yield return (object) enumerator;
      enumerator = (Coroutine) null;
    }
    else
      Log.Gameplay.PrintError("VictoryScreenICCEpilogue.PlayAnim(): GameEntity is not an instance of ICC_08_Finale!.");
    if (!(bool) UniversalInputManager.UsePhoneUI)
      victoryScreenIccFinale.m_continueText.gameObject.SetActive(true);
    victoryScreenIccFinale.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(victoryScreenIccFinale.ContinueButtonPress_HideTwoScoop));
    while (!victoryScreenIccFinale.m_dismissedTwoScoops)
      yield return (object) null;
    victoryScreenIccFinale.m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(victoryScreenIccFinale.ContinueButtonPress_HideTwoScoop));
    victoryScreenIccFinale.m_hitbox.gameObject.SetActive(false);
    if (!(bool) UniversalInputManager.UsePhoneUI)
      victoryScreenIccFinale.m_continueText.gameObject.SetActive(false);
    yield return (object) new WaitForSeconds(VictoryScreenICCFinale.POSTCARD_DELAY);
    if ((Object) victoryScreenIccFinale.m_HeroPostcard != (Object) null)
    {
      if ((Object) victoryScreenIccFinale.m_HeroPostcardRenderer != (Object) null)
      {
        Material materialForClass = victoryScreenIccFinale.GetPostcardMaterialForClass(GameState.Get().GetFriendlySidePlayer().GetStartingHero().GetClass());
        victoryScreenIccFinale.m_HeroPostcardRenderer.SetMaterial(materialForClass);
      }
      victoryScreenIccFinale.m_HeroPostcard.SetActive(true);
      foreach (AudioSource source in victoryScreenIccFinale.m_HeroPostcardShowAudio)
      {
        if ((Object) source != (Object) null)
          SoundManager.Get().Play(source);
      }
      Hashtable args = iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutBounce);
      iTween.ScaleFrom(victoryScreenIccFinale.m_HeroPostcard, args);
    }
    else
      Log.Gameplay.PrintError("VictoryScreenICCFinale.PlayAnim(): m_HeroPostcard is null!");
    victoryScreenIccFinale.m_ContinueButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(victoryScreenIccFinale.ContinueButtonPress_DismissPostcard));
  }

  private void ContinueButtonPress_HideTwoScoop(UIEvent e)
  {
    if ((Object) this.m_HideTwoScoopsAudio != (Object) null)
      SoundManager.Get().Play(this.m_HideTwoScoopsAudio);
    this.m_twoScoop.Hide();
    this.m_dismissedTwoScoops = true;
  }

  private void ContinueButtonPress_DismissPostcard(UIEvent e)
  {
    this.m_ContinueButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ContinueButtonPress_DismissPostcard));
    this.StartCoroutine(this.DismissPostcard());
  }

  private IEnumerator DismissPostcard()
  {
    VictoryScreenICCFinale victoryScreenIccFinale = this;
    if ((Object) victoryScreenIccFinale.m_HeroPostcardHideAudio != (Object) null)
      SoundManager.Get().Play(victoryScreenIccFinale.m_HeroPostcardHideAudio);
    Hashtable args = iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) 0.25f, (object) "easetype", (object) iTween.EaseType.linear);
    iTween.ScaleTo(victoryScreenIccFinale.m_HeroPostcard, args);
    while (iTween.HasTween(victoryScreenIccFinale.m_HeroPostcard))
      yield return (object) null;
    victoryScreenIccFinale.m_HeroPostcard.SetActive(false);
    victoryScreenIccFinale.ContinueEvents();
  }

  private Material GetPostcardMaterialForClass(TAG_CLASS classType)
  {
    switch (classType)
    {
      case TAG_CLASS.DRUID:
        return this.m_DruidPostcardMaterial;
      case TAG_CLASS.HUNTER:
        return this.m_HunterPostcardMaterial;
      case TAG_CLASS.MAGE:
        return this.m_MagePostcardMaterial;
      case TAG_CLASS.PALADIN:
        return this.m_PaladinPostcardMaterial;
      case TAG_CLASS.PRIEST:
        return this.m_PriestPostcardMaterial;
      case TAG_CLASS.ROGUE:
        return this.m_RoguePostcardMaterial;
      case TAG_CLASS.SHAMAN:
        return this.m_ShamanPostcardMaterial;
      case TAG_CLASS.WARLOCK:
        return this.m_WarlockPostcardMaterial;
      case TAG_CLASS.WARRIOR:
        return this.m_WarriorPostcardMaterial;
      default:
        return this.m_MagePostcardMaterial;
    }
  }
}
