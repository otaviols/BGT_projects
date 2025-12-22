using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
  public const string KT_PREFAB_PATH = "KT_Quote.prefab:7ad118a1a10e9ab409ade82268a378f5";
  public const string TIRION_PREFAB_PATH = "Tirion_Quote.prefab:2f88f08e8896841429c972fc5c4c7088";
  public const string NORMAL_NEFARIAN_PREFAB_PATH = "NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913";
  public const string ZOMBIE_NEFARIAN_PREFAB_PATH = "NefarianDragon_Quote.prefab:179fec888df7e4c02b8de3b7ad109a23";
  public const string RAGNAROS_PREFAB_PATH = "Ragnaros_Quote.prefab:c9e0154894cd1a946b90ebefeb481a51";
  public const string MAJORDOMO_PREFAB_PATH = "Majordomo_Quote.prefab:72286f87e5b724c21aa1d92d04426614";
  public const string RENO_PREFAB_PATH = "Reno_Quote.prefab:0a2e34fa6782a0747b4f5d5574d1331a";
  public const string RENO_BIG_PREFAB_PATH = "Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921";
  public const string CARTOGRAPHER_PREFAB_PATH = "Cartographer_Quote.prefab:c6056bfb8c0025a458553adabc8ed537";
  public const string ELISE_BIG_PREFAB_PATH = "Elise_BigQuote.prefab:932bc9e74bb49e047ae8dd480492db26";
  public const string FINLEY_BIG_PREFAB_PATH = "Finley_BigQuote.prefab:1c1c332cf5009194cb7dd7316c465aee";
  public const string BRANN_BIG_PREFAB_PATH = "Brann_BigQuote.prefab:a03dd286404083c439e371ba84d7a82b";
  public const string RAFAAM_WRAP_PREFAB_PATH = "Rafaam_wrap_Quote.prefab:d7100015bf618604ea93bad6b9f54f8b";
  public const string RAFAAM_WRAP_BIG_PREFAB_PATH = "Rafaam_wrap_BigQuote.prefab:ee7dbbb027adc1947b64b05f31d4c124";
  public const string RAFAAM_BIG_PREFAB_PATH = "Rafaam_BigQuote.prefab:ff1fd65bf3d8ba748b144b805fca871f";
  public const string RAFAAM_PREFAB_PATH = "Rafaam_Quote.prefab:d27a824bbfd6bd94185fe10e594f0014";
  public const string BRANN_PREFAB_PATH = "Brann_Quote.prefab:2c11651ab7740924189734944b8d7089";
  public const string BLAGGH_PREFAB_PATH = "Blaggh_Quote.prefab:f5d1e7053e6368e4a930ca3906cff53a";
  public const string MEDIVH_PREFAB_PATH = "Medivh_Quote.prefab:423c4a6b7e7a7f643bf0b2992ad3d31b";
  public const string MEDIVH_BIG_PREFAB_PATH = "Medivh_BigQuote.prefab:78e18a627031f6c48aef27a0fa1123c1";
  public const string MEDIVAS_BIG_PREFAB_PATH = "Medivas_BigQuote.prefab:ad677b060790a304fa6caed25f19bf88";
  public const string MOROES_PREFAB_PATH = "Moroes_Quote.prefab:ea3a21837aab2b0448ce4090103724cf";
  public const string MOROES_BIG_PREFAB_PATH = "Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d";
  public const string CURATOR_PREFAB_PATH = "Curator_Quote.prefab:ab58be80382875e4cbaa766fda73cd39";
  public const string CURATOR_BIG_PREFAB_PATH = "Curator_BigQuote.prefab:f01875528133988418925bd870aa7b81";
  public const string BARNES_PREFAB_PATH = "Barnes_Quote.prefab:2e7e9f28b5bc37149a12b2e5feaa244a";
  public const string BARNES_BIG_PREFAB_PATH = "Barnes_BigQuote.prefab:15c396b2577ab09449f3721d23da3dba";
  public const string AYA_BIG_PREFAB_PATH = "Aya_BigQuote.prefab:26a19c2632327c14dbf648b96f8751d1";
  public const string HANCHO_BIG_PREFAB_PATH = "HanCho_BigQuote.prefab:0b24275caed054c45b2ebcb91fd9112d";
  public const string KAZAKUS_BIG_PREFAB_PATH = "Kazakus_BigQuote.prefab:b0007ae4277fc5a40a8c6f8c774ab823";
  public const string LICHKING_PREFAB_PATH = "LichKing_Quote.prefab:59d5b461e0b2bbe479b7db63e0962d30";
  public const string TIRION_BIG_PREFAB_PATH = "Tirion_BigQuote.prefab:878fcebc1cddaf24f828c44edb07f7f8";
  public const string AHUNE_BIG_PREFAB_PATH = "Ahune_BigQuote.prefab:00dd8f83adda33345ac291cc76241482";
  public const string RAGNAROS_BIG_PREFAB_PATH = "Ragnaros_BigQuote.prefab:843c4fab946192943a909b026f755505";
  public const string DEMON_HUNTER_ILLIDAN_PREFAB_PATH = "DemonHunter_Illidan_Popup_Banner.prefab:c2b08a2b89af02e4bb9e80b08526df7a";
  public static readonly float DEPTH = -15f;
  public static readonly Vector3 LEFT_OF_FRIENDLY_HERO = new Vector3(-1f, 0.0f, 1f);
  public static readonly Vector3 RIGHT_OF_FRIENDLY_HERO = new Vector3(-6f, 0.0f, 1f);
  public static readonly Vector3 LEFT_OF_ENEMY_HERO = new Vector3(-1f, 0.0f, -3.5f);
  public static readonly Vector3 RIGHT_OF_ENEMY_HERO = new Vector3(-6f, 0.0f, -3f);
  public static readonly Vector3 DEFAULT_CHARACTER_POS = new Vector3(100f, NotificationManager.DEPTH, 24.7f);
  public static readonly Vector3 DEFAULT_BANNER_POS = new Vector3(0.0f, NotificationManager.DEPTH, 0.0f);
  public static readonly Vector3 CHARACTER_POS_ABOVE_QUEST_TOAST = new Vector3(100f, 50f, 24.7f);
  public static readonly Vector3 ALT_ADVENTURE_SCREEN_POS = new Vector3(104.8f, NotificationManager.DEPTH, 131.1f);
  public static readonly Vector3 PHONE_CHARACTER_POS = new Vector3(124.1f, NotificationManager.DEPTH, 24.7f);
  public static readonly float PHONE_OVERLAY_UI_CHARACTER_X_OFFSET = -0.5f;
  public static readonly float DEFAULT_BANNER_OFFSET_Z = 24.7f;
  public static readonly float DEFAULT_BANNER_OFFSET_X = 50f;
  public GameObject speechBubblePrefab;
  public GameObject speechIndicatorPrefab;
  public GameObject bounceArrowPrefab;
  public GameObject fadeArrowPrefab;
  public GameObject popupTextPrefab;
  public GameObject fancyPopupTextPrefab;
  public GameObject dialogBoxPrefab;
  public GameObject innkeeperQuotePrefab;
  [SerializeField]
  private GameObject m_battlegroundsEmoteNotificationPrefab;
  [SerializeField]
  private GameObject m_storeNotificationPrefab;
  private static NotificationManager s_instance;
  private Map<int, List<Notification>> notificationsToDestroyUponNewNotifier;
  private Map<int, List<Notification>> speechBubbleNotToDestoryUponNewNotifier;
  private List<Notification> arrows;
  private List<Notification> popUpTexts;
  private Notification popUpDialog;
  private Notification m_quote;
  private List<string> m_quotesThisSession;
  private const float DEFAULT_QUOTE_DURATION = 8f;
  private Vector3 NOTIFICATION_SCALE = 0.163f * Vector3.one;
  private Vector3 NOTIFICATION_SCALE_PHONE = 0.326f * Vector3.one;

  public static Vector3 GetDefaultDialogueBannerPos(CanvasAnchor anchor)
  {
    Vector3 defaultBannerPos = NotificationManager.DEFAULT_BANNER_POS;
    switch (anchor - 3)
    {
      case CanvasAnchor.CENTER:
      case CanvasAnchor.RIGHT:
      case CanvasAnchor.BOTTOM:
        defaultBannerPos += Vector3.forward * NotificationManager.DEFAULT_BANNER_OFFSET_Z;
        break;
      case CanvasAnchor.LEFT:
      case CanvasAnchor.TOP:
      case CanvasAnchor.BOTTOM_LEFT:
        defaultBannerPos -= Vector3.forward * NotificationManager.DEFAULT_BANNER_OFFSET_Z;
        break;
    }
    switch (anchor - 1)
    {
      case CanvasAnchor.CENTER:
      case CanvasAnchor.TOP:
      case CanvasAnchor.BOTTOM_RIGHT:
        defaultBannerPos += Vector3.right * NotificationManager.DEFAULT_BANNER_OFFSET_X;
        break;
      case CanvasAnchor.LEFT:
      case CanvasAnchor.BOTTOM_LEFT:
      case CanvasAnchor.TOP_LEFT:
        defaultBannerPos -= Vector3.right * NotificationManager.DEFAULT_BANNER_OFFSET_X;
        break;
    }
    return defaultBannerPos;
  }

  public static Vector3 NOTIFICATITON_WORLD_SCALE => !(bool) UniversalInputManager.UsePhoneUI ? 18f * Vector3.one : 25f * Vector3.one;

  private void Awake()
  {
    NotificationManager.s_instance = this;
    this.m_quotesThisSession = new List<string>();
  }

  private void OnDestroy() => NotificationManager.s_instance = (NotificationManager) null;

  private void Start()
  {
    this.notificationsToDestroyUponNewNotifier = new Map<int, List<Notification>>();
    this.speechBubbleNotToDestoryUponNewNotifier = new Map<int, List<Notification>>();
    this.arrows = new List<Notification>();
    this.popUpTexts = new List<Notification>();
  }

  public static NotificationManager Get() => NotificationManager.s_instance;

  public Notification CreatePopupDialog(
    string headlineText,
    string bodyText,
    string yesOrOKButtonText,
    string noButtonText)
  {
    return this.CreatePopupDialog(headlineText, bodyText, yesOrOKButtonText, noButtonText, new Vector3(0.0f, 0.0f, 0.0f));
  }

  public Notification CreatePopupDialog(
    string headlineText,
    string bodyText,
    string yesOrOKButtonText,
    string noButtonText,
    Vector3 offset)
  {
    if ((UnityEngine.Object) this.popUpDialog != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.popUpDialog.gameObject);
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.dialogBoxPrefab);
    Vector3 position = Camera.main.transform.position;
    gameObject.transform.position = position + new Vector3(-0.07040818f, -16.10709f, 1.79612f) + offset;
    this.popUpDialog = gameObject.GetComponent<Notification>();
    this.popUpDialog.ChangeDialogText(headlineText, bodyText, yesOrOKButtonText, noButtonText);
    this.popUpDialog.PlayBirth();
    UniversalInputManager.Get().SetGameDialogActive(true);
    return this.popUpDialog;
  }

  public Notification CreateSpeechBubble(string speechText, Actor actor) => this.CreateSpeechBubble(speechText, Notification.SpeechBubbleDirection.BottomLeft, actor, false);

  public Notification CreateSpeechBubble(
    string speechText,
    Actor actor,
    bool bDestroyWhenNewCreated)
  {
    return this.CreateSpeechBubble(speechText, Notification.SpeechBubbleDirection.BottomLeft, actor, bDestroyWhenNewCreated);
  }

  public Notification CreateSpeechBubble(
    string speechText,
    Notification.SpeechBubbleDirection direction,
    Actor actor)
  {
    return this.CreateSpeechBubble(speechText, direction, actor, false);
  }

  public Notification CreateSpeechBubble(
    string speechText,
    Notification.SpeechBubbleDirection direction,
    Actor actor,
    bool bDestroyWhenNewCreated,
    bool parentToActor = true,
    float bubbleScale = 0.0f)
  {
    return this.CreateSpeechBubble(new NotificationManager.SpeechBubbleOptions().WithSpeechText(speechText).WithSpeechBubbleDirection(direction).WithActor(actor).WithDestroyWhenNewCreated(bDestroyWhenNewCreated).WithParentToActor(parentToActor).WithBubbleScale(bubbleScale));
  }

  public bool HasNonDestroyableSpeechBubbleExisting(
    Notification.SpeechBubbleDirection direction,
    int speechBubbleGroup)
  {
    if (this.speechBubbleNotToDestoryUponNewNotifier.Count == 0 || !this.speechBubbleNotToDestoryUponNewNotifier.ContainsKey(speechBubbleGroup) || this.speechBubbleNotToDestoryUponNewNotifier[speechBubbleGroup] == null)
      return false;
    for (int index = 0; index < this.speechBubbleNotToDestoryUponNewNotifier[speechBubbleGroup].Count; ++index)
    {
      if (!((UnityEngine.Object) this.speechBubbleNotToDestoryUponNewNotifier[speechBubbleGroup][index] == (UnityEngine.Object) null) && this.speechBubbleNotToDestoryUponNewNotifier[speechBubbleGroup][index].GetSpeechBubbleDirection() == direction)
        return true;
    }
    return false;
  }

  public Notification CreateSpeechBubble(NotificationManager.SpeechBubbleOptions options)
  {
    if (options.destroyWhenNewCreated && this.HasNonDestroyableSpeechBubbleExisting(options.direction, options.speechBubbleGroup))
      return (Notification) null;
    this.DestroyOtherNotifications(options.direction, options.speechBubbleGroup);
    Notification component;
    if (options.speechText == "" && options.visualEmoteType == NotificationManager.VisualEmoteType.NONE)
    {
      component = UnityEngine.Object.Instantiate<GameObject>(this.speechIndicatorPrefab).GetComponent<Notification>();
      component.PlaySmallBirthForFakeBubble();
      component.SetPositionForSmallBubble(options.actor);
      if (!Cheats.Get().IsSpeechBubbleEnabled())
        component.SetPosition(Cheats.Get().SPEECH_BUBBLE_HIDDEN_POSITION);
    }
    else if (options.visualEmoteType == NotificationManager.VisualEmoteType.COLLECTIBLE_BATTLEGROUNDS_EMOTE)
    {
      component = UnityEngine.Object.Instantiate<GameObject>(this.m_battlegroundsEmoteNotificationPrefab).GetComponent<Notification>();
      if (component is BattlegroundsEmoteNotification emoteNotification)
        emoteNotification.BindEmoteDataModel(options.battlegroundsEmoteId);
      else
        Debug.LogError((object) "NotificationManager: Could not find BattlegroundsEmoteNotification component on emote notification prefab.");
      component.SetPosition(options.actor, options.direction);
      component.PlayBirth();
    }
    else if (options.visualEmoteType == NotificationManager.VisualEmoteType.STORE)
    {
      component = UnityEngine.Object.Instantiate<GameObject>(this.m_storeNotificationPrefab).GetComponent<Notification>();
      component.ChangeText(options.speechText);
      component.FaceDirection(options.direction);
      component.SetPosition(options.actor, options.direction);
      component.PlayBirth();
    }
    else
    {
      component = UnityEngine.Object.Instantiate<GameObject>(this.speechBubblePrefab).GetComponent<Notification>();
      if (options.visualEmoteType == NotificationManager.VisualEmoteType.NONE)
      {
        component.ChangeText(options.speechText);
        component.ChangeEmote(NotificationManager.VisualEmoteType.NONE);
      }
      else
      {
        component.ChangeText("");
        component.ChangeEmote(options.visualEmoteType);
      }
      component.FaceDirection(options.direction);
      component.PlayBirth();
      component.SetPosition(options.actor, options.direction);
      if (!Cheats.Get().IsSpeechBubbleEnabled() && options.visualEmoteType == NotificationManager.VisualEmoteType.NONE)
        component.SetPosition(Cheats.Get().SPEECH_BUBBLE_HIDDEN_POSITION);
      if (!Mathf.Approximately(options.bubbleScale, 0.0f))
      {
        GameObject gameObject = new GameObject();
        gameObject.transform.SetParent(options.actor.transform);
        TransformUtil.Identity(gameObject);
        component.SetParentOffsetObject(gameObject);
        gameObject.transform.localScale = new Vector3(options.bubbleScale, options.bubbleScale, options.bubbleScale);
      }
    }
    if (options.destroyWhenNewCreated)
    {
      if (!this.notificationsToDestroyUponNewNotifier.ContainsKey(options.speechBubbleGroup))
        this.notificationsToDestroyUponNewNotifier.Add(options.speechBubbleGroup, new List<Notification>());
      this.notificationsToDestroyUponNewNotifier[options.speechBubbleGroup].Add(component);
    }
    else
    {
      if (!this.speechBubbleNotToDestoryUponNewNotifier.ContainsKey(options.speechBubbleGroup))
        this.speechBubbleNotToDestoryUponNewNotifier.Add(options.speechBubbleGroup, new List<Notification>());
      this.speechBubbleNotToDestoryUponNewNotifier[options.speechBubbleGroup].Add(component);
    }
    if (options.parentToActor)
      component.transform.parent = options.actor.transform;
    if (options.finishCallback != null)
      component.OnFinishDeathState += options.finishCallback;
    if ((double) options.emoteDuration > 0.0)
      this.DestroyNotification(component, options.emoteDuration);
    component.notificationGroup = options.speechBubbleGroup;
    return component;
  }

  public Notification CreateBouncingArrow(UserAttentionBlocker blocker, bool addToList)
  {
    if (!SceneMgr.Get().IsInGame() && !UserAttentionManager.CanShowAttentionGrabber(blocker, "NotificationManger.CreateBouncingArrow"))
      return (Notification) null;
    Notification component = UnityEngine.Object.Instantiate<GameObject>(this.bounceArrowPrefab).GetComponent<Notification>();
    component.PlayBirth();
    if (addToList)
      this.arrows.Add(component);
    return component;
  }

  public Notification CreateBouncingArrow(
    UserAttentionBlocker blocker,
    Vector3 position,
    Vector3 rotation)
  {
    return this.CreateBouncingArrow(blocker, position, rotation, true);
  }

  public Notification CreateBouncingArrow(
    UserAttentionBlocker blocker,
    Vector3 position,
    Vector3 rotation,
    bool addToList,
    float scaleFactor = 1f)
  {
    if (!SceneMgr.Get().IsInGame() && !UserAttentionManager.CanShowAttentionGrabber(blocker, "NotificationManger.CreateBouncingArrow"))
      return (Notification) null;
    Notification bouncingArrow = this.CreateBouncingArrow(blocker, addToList);
    bouncingArrow.transform.position = position;
    bouncingArrow.transform.localEulerAngles = rotation;
    bouncingArrow.transform.localScale = Vector3.one * scaleFactor;
    return bouncingArrow;
  }

  public Notification CreateFadeArrow(bool addToList)
  {
    Notification component = UnityEngine.Object.Instantiate<GameObject>(this.fadeArrowPrefab).GetComponent<Notification>();
    component.PlayBirth();
    if (addToList)
      this.arrows.Add(component);
    return component;
  }

  public Notification CreateFadeArrow(Vector3 position, Vector3 rotation) => this.CreateFadeArrow(position, rotation, true);

  public Notification CreateFadeArrow(
    Vector3 position,
    Vector3 rotation,
    bool addToList)
  {
    Notification fadeArrow = this.CreateFadeArrow(addToList);
    fadeArrow.transform.position = position;
    fadeArrow.transform.localEulerAngles = rotation;
    return fadeArrow;
  }

  public Notification CreatePopupText(
    UserAttentionBlocker blocker,
    Transform bone,
    string text,
    bool convertLegacyPosition = true,
    NotificationManager.PopupTextType popupTextType = NotificationManager.PopupTextType.BASIC)
  {
    return convertLegacyPosition ? this.CreatePopupText(blocker, bone.position, bone.localScale, text, convertLegacyPosition, popupTextType) : this.CreatePopupText(blocker, bone.localPosition, bone.localScale, text, convertLegacyPosition, popupTextType);
  }

  public Notification CreatePopupText(
    UserAttentionBlocker blocker,
    Vector3 position,
    Vector3 scale,
    string text,
    bool convertLegacyPosition = true,
    NotificationManager.PopupTextType popupTextType = NotificationManager.PopupTextType.BASIC)
  {
    if (!SceneMgr.Get().IsInGame() && !UserAttentionManager.CanShowAttentionGrabber(blocker, "NotificationManager.CreatePopupText"))
      return (Notification) null;
    Vector3 vector3 = position;
    if (convertLegacyPosition)
    {
      Camera camera = SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY ? Box.Get().GetBoxCamera().GetComponent<Camera>() : BoardCameras.Get().GetComponentInChildren<Camera>();
      vector3 = OverlayUI.Get().GetRelativePosition(position, camera, OverlayUI.Get().m_heightScale.m_Center);
    }
    GameObject go = UnityEngine.Object.Instantiate<GameObject>(popupTextType == NotificationManager.PopupTextType.BASIC ? this.popupTextPrefab : this.fancyPopupTextPrefab);
    LayerUtils.SetLayer(go, GameLayer.UI);
    go.transform.localPosition = vector3;
    go.transform.localScale = scale;
    OverlayUI.Get().AddGameObject(go);
    Notification component = go.GetComponent<Notification>();
    component.ChangeText(text);
    component.PlayBirth();
    component.OnDestroyCallback += new Action<Notification>(this.OnPopupTextDestroy);
    this.popUpTexts.Add(component);
    return component;
  }

  public bool IsQuotePlaying => (UnityEngine.Object) this.m_quote != (UnityEngine.Object) null;

  public Notification CreateInnkeeperQuote(
    UserAttentionBlocker blocker,
    string text,
    string soundPath,
    float durationSeconds = 0.0f,
    Action<int> finishCallback = null,
    bool clickToDismiss = false)
  {
    return this.CreateInnkeeperQuote(blocker, NotificationManager.DEFAULT_CHARACTER_POS, text, soundPath, durationSeconds, finishCallback, clickToDismiss);
  }

  public Notification CreateInnkeeperQuote(
    UserAttentionBlocker blocker,
    string text,
    string soundPath,
    Action<int> finishCallback,
    bool clickToDismiss = false)
  {
    return this.CreateInnkeeperQuote(blocker, NotificationManager.DEFAULT_CHARACTER_POS, text, soundPath, finishCallback: finishCallback, clickToDismiss: clickToDismiss);
  }

  public Notification CreateInnkeeperQuote(
    UserAttentionBlocker blocker,
    Vector3 position,
    string text,
    string soundPath,
    float durationSeconds = 0.0f,
    Action<int> finishCallback = null,
    bool clickToDismiss = false)
  {
    if (!SceneMgr.Get().IsInGame() && !UserAttentionManager.CanShowAttentionGrabber(blocker, "NotificationManager.CreateInnkeeperQuote"))
    {
      if (finishCallback != null)
        finishCallback(0);
      return (Notification) null;
    }
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.innkeeperQuotePrefab);
    gameObject.GetComponentInChildren<BoxCollider>().enabled = clickToDismiss;
    Notification component = gameObject.GetComponent<Notification>();
    component.ignoreAudioOnDestroy = clickToDismiss;
    if (finishCallback != null)
      component.OnFinishDeathState += finishCallback;
    this.PlayCharacterQuote(component, position, text, soundPath, durationSeconds);
    return component;
  }

  public Notification CreateKTQuote(
    string stringTag,
    string soundPath,
    bool allowRepeatDuringSession = true)
  {
    return this.CreateKTQuote(NotificationManager.DEFAULT_CHARACTER_POS, stringTag, soundPath, allowRepeatDuringSession);
  }

  public Notification CreateKTQuote(
    Vector3 position,
    string stringTag,
    string soundPath,
    bool allowRepeatDuringSession = true)
  {
    return this.CreateCharacterQuote("KT_Quote.prefab:7ad118a1a10e9ab409ade82268a378f5", position, GameStrings.Get(stringTag), soundPath, allowRepeatDuringSession);
  }

  public Notification CreateZombieNefarianQuote(
    Vector3 position,
    string stringTag,
    string soundPath,
    bool allowRepeatDuringSession)
  {
    return this.CreateCharacterQuote("NefarianDragon_Quote.prefab:179fec888df7e4c02b8de3b7ad109a23", position, GameStrings.Get(stringTag), soundPath, allowRepeatDuringSession);
  }

  public void PlayBundleInnkeeperLineForClass(TAG_CLASS cardClass)
  {
    bool usePhoneUi = (bool) UniversalInputManager.UsePhoneUI;
    string empty = string.Empty;
    string soundPath = string.Empty;
    switch (cardClass)
    {
      case TAG_CLASS.DRUID:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_DRUID");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryDruid_01.prefab:2c4672cdfe2a96a45a7ac4f29c17d5b7";
        break;
      case TAG_CLASS.HUNTER:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_HUNTER");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryHunter_01.prefab:77302a32e0268f845a97992117241577";
        break;
      case TAG_CLASS.MAGE:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_MAGE");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryMage_01.prefab:2059ede4ae6efab489ecb4240a08d5bb";
        break;
      case TAG_CLASS.PALADIN:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_PALADIN");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryPaladin_01.prefab:21b7870188f66714b9707961d833b26a";
        break;
      case TAG_CLASS.PRIEST:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_PRIEST");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryPriest_01.prefab:fe9cd14401fd7f14f80950fb99864ce7";
        break;
      case TAG_CLASS.ROGUE:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_ROGUE");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryRogue_01.prefab:aa4c71ab99a240a4885e4a8d034adb1b";
        break;
      case TAG_CLASS.SHAMAN:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_SHAMAN");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryShaman_01.prefab:1101d9f890551164791f277babaa25d9";
        break;
      case TAG_CLASS.WARLOCK:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_WARLOCK");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryWarlock_01.prefab:5eaf5c883b0310e4d91bcfd3debc6eff";
        break;
      case TAG_CLASS.WARRIOR:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_WARRIOR");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryWarrior_01.prefab:41b4581beb2dae945843ed164a6ec710";
        break;
    }
    if (string.IsNullOrEmpty(empty))
      return;
    NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, empty, soundPath, (Action<int>) null, usePhoneUi);
  }

  public Notification CreateCharacterQuote(
    string prefabPath,
    string text,
    string soundPath,
    bool allowRepeatDuringSession = true,
    float durationSeconds = 0.0f,
    CanvasAnchor anchorPoint = CanvasAnchor.BOTTOM_LEFT,
    bool blockAllOtherInput = false)
  {
    return this.CreateCharacterQuote(prefabPath, NotificationManager.DEFAULT_CHARACTER_POS, text, soundPath, allowRepeatDuringSession, durationSeconds, anchorPoint: anchorPoint, blockAllOtherInput: blockAllOtherInput);
  }

  public Notification CreateCharacterQuote(
    string prefabPath,
    Vector3 position,
    string text,
    string soundPath,
    bool allowRepeatDuringSession = true,
    float durationSeconds = 0.0f,
    Action<int> finishCallback = null,
    CanvasAnchor anchorPoint = CanvasAnchor.BOTTOM_LEFT,
    bool blockAllOtherInput = false)
  {
    if (!allowRepeatDuringSession && this.m_quotesThisSession.Contains(soundPath))
      return (Notification) null;
    this.m_quotesThisSession.Add(soundPath);
    Notification quote = GameUtils.LoadGameObjectWithComponent<Notification>(prefabPath);
    if ((UnityEngine.Object) quote == (UnityEngine.Object) null)
      return (Notification) null;
    quote.ShowWithExistingPopups = true;
    quote.PrefabPath = prefabPath;
    quote.SetClickBlockerActive(blockAllOtherInput);
    if (finishCallback != null)
      quote.OnFinishDeathState += finishCallback;
    this.PlayCharacterQuote(quote, position, text, soundPath, durationSeconds, anchorPoint);
    return quote;
  }

  public Notification CreateBigCharacterQuoteWithGameString(
    string prefabPath,
    Vector3 position,
    string soundPath,
    string bubbleGameStringID,
    bool allowRepeatDuringSession = true,
    float durationSeconds = 0.0f,
    Action<int> finishCallback = null,
    bool useOverlayUI = false,
    Notification.SpeechBubbleDirection bubbleDir = Notification.SpeechBubbleDirection.None,
    bool persistCharacter = false,
    bool altPosition = false)
  {
    if (!allowRepeatDuringSession && this.m_quotesThisSession.Contains(bubbleGameStringID))
      return (Notification) null;
    this.m_quotesThisSession.Add(bubbleGameStringID);
    return this.CreateBigCharacterQuoteWithText(prefabPath, position, soundPath, GameStrings.Get(bubbleGameStringID), durationSeconds, finishCallback, useOverlayUI, bubbleDir, persistCharacter, altPosition);
  }

  public Notification CreateBigCharacterQuoteWithText(
    string prefabPath,
    Vector3 position,
    string soundPath,
    string bubbleText,
    float durationSeconds = 0.0f,
    Action<int> finishCallback = null,
    bool useOverlayUI = false,
    Notification.SpeechBubbleDirection bubbleDir = Notification.SpeechBubbleDirection.None,
    bool persistCharacter = false,
    bool altPosition = false)
  {
    bool animateSpeechBubble = false;
    Notification quote;
    if (prefabPath != null && (UnityEngine.Object) this.m_quote != (UnityEngine.Object) null && this.m_quote.PersistCharacter && prefabPath.Equals(this.m_quote.PrefabPath))
    {
      quote = this.m_quote;
      animateSpeechBubble = true;
    }
    else
      quote = GameUtils.LoadGameObjectWithComponent<Notification>(prefabPath);
    if ((UnityEngine.Object) quote == (UnityEngine.Object) null)
      return (Notification) null;
    quote.PrefabPath = prefabPath;
    quote.PersistCharacter = persistCharacter;
    quote.ShowWithExistingPopups = true;
    if (bubbleDir != Notification.SpeechBubbleDirection.None)
      quote.RepositionSpeechBubbleAroundBigQuote(bubbleDir, animateSpeechBubble);
    if (finishCallback != null)
      quote.OnFinishDeathState += finishCallback;
    this.PlayBigCharacterQuote(quote, bubbleText, soundPath, durationSeconds, position, useOverlayUI, persistCharacter, altPosition);
    return quote;
  }

  public void ForceAddSoundToPlayedList(string soundPath) => this.m_quotesThisSession.Add(soundPath);

  public void ForceRemoveSoundFromPlayedList(string soundPath) => this.m_quotesThisSession.Remove(soundPath);

  public bool HasSoundPlayedThisSession(string soundPath) => this.m_quotesThisSession.Contains(soundPath);

  public void ResetSoundsPlayedThisSession() => this.m_quotesThisSession.Clear();

  private void PlayBigCharacterQuote(
    Notification quote,
    string text,
    string soundPath,
    float durationSeconds,
    Vector3 position,
    bool useOverlayUI = false,
    bool persistCharacter = false,
    bool altPosition = false)
  {
    bool flag = true;
    if ((bool) (UnityEngine.Object) this.m_quote)
    {
      if ((UnityEngine.Object) this.m_quote == (UnityEngine.Object) quote)
        flag = false;
      else
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_quote.gameObject);
    }
    this.m_quote = quote;
    this.m_quote.ChangeText(text);
    if (useOverlayUI)
    {
      string name = altPosition ? "OffScreenSpeaker2" : "OffScreenSpeaker1";
      TransformUtil.AttachAndPreserveLocalTransform(this.m_quote.transform, OverlayUI.Get().FindBone(name));
    }
    else
      TransformUtil.AttachAndPreserveLocalTransform(this.m_quote.transform, Board.Get().FindBone("OffScreenSpeaker1"));
    Vector3 vector3 = Vector3.zero;
    if (position != NotificationManager.DEFAULT_CHARACTER_POS)
      vector3 = position;
    if (useOverlayUI && (bool) UniversalInputManager.UsePhoneUI)
      vector3.x += NotificationManager.PHONE_OVERLAY_UI_CHARACTER_X_OFFSET;
    this.m_quote.transform.localPosition = vector3;
    this.m_quote.transform.localEulerAngles = Vector3.zero;
    if (!useOverlayUI && this.m_quote.rotate180InGameplay)
      this.m_quote.transform.localEulerAngles = new Vector3(0.0f, 180f, 0.0f);
    if (flag)
      this.m_quote.transform.localScale = Vector3.one * 0.01f;
    if (!string.IsNullOrEmpty(soundPath) && AssetLoader.Get().IsAssetAvailable((AssetReference) soundPath))
    {
      SoundLoader.LoadSound((AssetReference) soundPath, new PrefabCallback<GameObject>(this.OnBigQuoteSoundLoaded), (object) new NotificationManager.QuoteSoundCallbackData()
      {
        m_quote = this.m_quote,
        m_durationSeconds = durationSeconds,
        m_persistCharacter = persistCharacter
      }, SoundManager.Get().GetPlaceholderSound());
    }
    else
    {
      this.m_quote.PlayBirthWithForcedScale(Vector3.one);
      if ((double) durationSeconds <= 0.0)
        return;
      if (persistCharacter)
        this.DestroySpeechBubble(this.m_quote, durationSeconds);
      else
        this.DestroyNotification(this.m_quote, durationSeconds);
    }
  }

  private void PlayCharacterQuote(
    Notification quote,
    Vector3 position,
    string text,
    string soundPath,
    float durationSeconds,
    CanvasAnchor anchorPoint = CanvasAnchor.BOTTOM_LEFT)
  {
    if ((bool) (UnityEngine.Object) this.m_quote)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_quote.gameObject);
    this.m_quote = quote;
    this.m_quote.ChangeText(text);
    this.m_quote.transform.position = position;
    this.m_quote.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    OverlayUI.Get().AddGameObject(this.m_quote.gameObject, anchorPoint);
    if (!string.IsNullOrEmpty(soundPath) && AssetLoader.Get().IsAssetAvailable((AssetReference) soundPath))
      SoundLoader.LoadSound((AssetReference) soundPath, new PrefabCallback<GameObject>(this.OnQuoteSoundLoaded), (object) new NotificationManager.QuoteSoundCallbackData()
      {
        m_quote = this.m_quote,
        m_durationSeconds = durationSeconds
      }, SoundManager.Get().GetPlaceholderSound());
    else
      this.PlayQuoteWithoutSound(durationSeconds, text);
  }

  private void PlayQuoteWithoutSound(float durationSeconds, string text = null)
  {
    this.m_quote.PlayBirthWithForcedScale((bool) UniversalInputManager.UsePhoneUI ? this.NOTIFICATION_SCALE_PHONE : this.NOTIFICATION_SCALE);
    if ((double) durationSeconds <= 0.0 && text != null)
      durationSeconds = ClipLengthEstimator.StringToReadTime(text);
    this.DestroyNotification(this.m_quote, durationSeconds);
  }

  private void OnQuoteSoundLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    NotificationManager.QuoteSoundCallbackData soundCallbackData = (NotificationManager.QuoteSoundCallbackData) callbackData;
    if (!(bool) (UnityEngine.Object) soundCallbackData.m_quote)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) go);
    }
    else
    {
      AudioSource source = (AudioSource) null;
      if ((bool) (UnityEngine.Object) go)
      {
        source = go.GetComponent<AudioSource>();
        if ((bool) (UnityEngine.Object) source && !(bool) (UnityEngine.Object) source.clip)
          source = (AudioSource) null;
      }
      if (!(bool) (UnityEngine.Object) source)
      {
        Log.Asset.PrintInfo("Quote Sound failed to load!");
        this.PlayQuoteWithoutSound((double) soundCallbackData.m_durationSeconds > 0.0 ? soundCallbackData.m_durationSeconds : 8f);
      }
      else
      {
        this.m_quote.AssignAudio(source);
        SoundManager.Get().PlayPreloaded(source);
        this.m_quote.PlayBirthWithForcedScale((bool) UniversalInputManager.UsePhoneUI ? this.NOTIFICATION_SCALE_PHONE : this.NOTIFICATION_SCALE);
        this.DestroyNotification(this.m_quote, Mathf.Max(soundCallbackData.m_durationSeconds, source.clip.length));
        if (!((UnityEngine.Object) this.m_quote.clickOff != (UnityEngine.Object) null))
          return;
        this.m_quote.clickOff.SetData((object) this.m_quote);
        this.m_quote.clickOff.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ClickNotification));
      }
    }
  }

  private void OnBigQuoteSoundLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    NotificationManager.QuoteSoundCallbackData soundCallbackData = (NotificationManager.QuoteSoundCallbackData) callbackData;
    if (!(bool) (UnityEngine.Object) soundCallbackData.m_quote)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) go);
    }
    else
    {
      AudioSource source = (AudioSource) null;
      if ((bool) (UnityEngine.Object) go)
      {
        source = go.GetComponent<AudioSource>();
        if ((bool) (UnityEngine.Object) source && !(bool) (UnityEngine.Object) source.clip)
          source = (AudioSource) null;
      }
      if (!(bool) (UnityEngine.Object) source)
      {
        Log.Asset.PrintInfo("Quote Sound failed to load!");
        this.PlayQuoteWithoutSound((double) soundCallbackData.m_durationSeconds > 0.0 ? soundCallbackData.m_durationSeconds : 8f);
      }
      else
      {
        this.m_quote.AssignAudio(source);
        SoundManager.Get().PlayPreloaded(source);
        this.m_quote.PlayBirthWithForcedScale(Vector3.one);
        float delaySeconds = Mathf.Max(soundCallbackData.m_durationSeconds, source.clip.length);
        Log.Notifications.Print("Destroying notification or speech bubble after {0} seconds. durationSeconds: {1} source.clip.length: {2} persistCharacter? {3}", (object) delaySeconds, (object) soundCallbackData.m_durationSeconds, (object) source.clip.length, (object) soundCallbackData.m_persistCharacter);
        if (soundCallbackData.m_persistCharacter)
          this.DestroySpeechBubble(this.m_quote, delaySeconds);
        else
          this.DestroyNotification(this.m_quote, delaySeconds);
        if (!((UnityEngine.Object) this.m_quote.clickOff != (UnityEngine.Object) null))
          return;
        this.m_quote.clickOff.SetData((object) this.m_quote);
        this.m_quote.clickOff.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ClickNotification));
      }
    }
  }

  public void DestroyAllArrows()
  {
    if (this.arrows.Count == 0)
      return;
    for (int index = 0; index < this.arrows.Count; ++index)
    {
      if ((UnityEngine.Object) this.arrows[index] != (UnityEngine.Object) null)
        this.NukeNotificationWithoutPlayingAnim(this.arrows[index]);
    }
  }

  public void DestroyAllPopUps()
  {
    if (this.popUpTexts.Count == 0)
      return;
    for (int index = 0; index < this.popUpTexts.Count; ++index)
    {
      if (!((UnityEngine.Object) this.popUpTexts[index] == (UnityEngine.Object) null))
        this.NukeNotification(this.popUpTexts[index]);
    }
    this.popUpTexts.Clear();
  }

  private void DestroyOtherNotifications(
    Notification.SpeechBubbleDirection direction,
    int speechBubbleGroup)
  {
    if (this.notificationsToDestroyUponNewNotifier.Count == 0 || !this.notificationsToDestroyUponNewNotifier.ContainsKey(speechBubbleGroup) || this.notificationsToDestroyUponNewNotifier[speechBubbleGroup] == null)
      return;
    for (int index = 0; index < this.notificationsToDestroyUponNewNotifier[speechBubbleGroup].Count; ++index)
    {
      if (!((UnityEngine.Object) this.notificationsToDestroyUponNewNotifier[speechBubbleGroup][index] == (UnityEngine.Object) null) && this.notificationsToDestroyUponNewNotifier[speechBubbleGroup][index].GetSpeechBubbleDirection() == direction)
        this.NukeNotificationWithoutPlayingAnim(this.notificationsToDestroyUponNewNotifier[speechBubbleGroup][index]);
    }
  }

  public void DestroyNotification(Notification notification, float delaySeconds)
  {
    if ((UnityEngine.Object) notification == (UnityEngine.Object) null)
      return;
    if ((double) delaySeconds == 0.0)
      this.NukeNotification(notification);
    else
      this.StartCoroutine(this.WaitAndThenDestroyNotification(notification, delaySeconds));
  }

  public void DestroySpeechBubble(Notification notification, float delaySeconds)
  {
    if ((UnityEngine.Object) notification == (UnityEngine.Object) null)
      return;
    if ((double) delaySeconds == 0.0)
      this.NukeSpeechBubble(notification);
    else
      this.StartCoroutine(this.WaitAndThenDestroySpeechBubble(notification, delaySeconds));
  }

  private void OnPopupTextDestroy(Notification notification) => this.popUpTexts.Remove(notification);

  public void DestroyNotificationWithText(string text, float delaySeconds = 0.0f)
  {
    Notification notification = (Notification) null;
    for (int index = 0; index < this.popUpTexts.Count; ++index)
    {
      if (!((UnityEngine.Object) this.popUpTexts[index] == (UnityEngine.Object) null) && this.popUpTexts[index].speechUberText.Text == text)
        notification = this.popUpTexts[index];
    }
    this.DestroyNotification(notification, delaySeconds);
  }

  private void ClickNotification(UIEvent e)
  {
    Notification data = (Notification) e.GetElement().GetData();
    this.NukeNotification(data);
    data.clickOff.RemoveEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ClickNotification));
  }

  public void DestroyAllNotificationsNowWithNoAnim()
  {
    if ((bool) (UnityEngine.Object) this.popUpDialog)
      this.NukeNotificationWithoutPlayingAnim(this.popUpDialog);
    if ((bool) (UnityEngine.Object) this.m_quote)
      this.NukeNotificationWithoutPlayingAnim(this.m_quote);
    foreach (List<Notification> notificationList in this.notificationsToDestroyUponNewNotifier.Values)
    {
      for (int index = 0; index < notificationList.Count; ++index)
      {
        Notification notification = notificationList[index];
        if (!((UnityEngine.Object) notification == (UnityEngine.Object) null))
          this.NukeNotificationWithoutPlayingAnim(notification);
      }
    }
    this.DestroyAllArrows();
    this.DestroyAllPopUps();
  }

  public void DestroyActiveQuote(float delaySeconds, bool ignoreAudio = false)
  {
    if ((UnityEngine.Object) this.m_quote == (UnityEngine.Object) null)
      return;
    if (ignoreAudio)
      this.m_quote.ignoreAudioOnDestroy = true;
    if ((double) delaySeconds == 0.0)
      this.NukeNotification(this.m_quote);
    else
      this.StartCoroutine(this.WaitAndThenDestroyNotification(this.m_quote, delaySeconds));
  }

  public void DestroyNotificationNowWithNoAnim(Notification notification)
  {
    if ((UnityEngine.Object) notification == (UnityEngine.Object) null)
      return;
    this.NukeNotificationWithoutPlayingAnim(notification);
  }

  private IEnumerator WaitAndThenDestroyNotification(
    Notification notification,
    float amountSeconds)
  {
    yield return (object) new WaitForSeconds(amountSeconds);
    if ((UnityEngine.Object) notification != (UnityEngine.Object) null)
      this.NukeNotification(notification);
  }

  private void NukeNotification(Notification notification)
  {
    if ((UnityEngine.Object) notification == (UnityEngine.Object) null)
    {
      Log.All.PrintWarning("Attempting to Nuke a Notification that does not exist!");
    }
    else
    {
      foreach (List<Notification> notificationList in this.notificationsToDestroyUponNewNotifier.Values)
      {
        if (notificationList.Contains(notification))
          notificationList.Remove(notification);
      }
      foreach (List<Notification> notificationList in this.speechBubbleNotToDestoryUponNewNotifier.Values)
      {
        if (notificationList.Contains(notification))
          notificationList.Remove(notification);
      }
      if (notification.IsDying())
        return;
      notification.PlayDeath();
      UniversalInputManager.Get().SetGameDialogActive(false);
    }
  }

  private void NukeNotificationWithoutPlayingAnim(Notification notification)
  {
    foreach (List<Notification> notificationList in this.notificationsToDestroyUponNewNotifier.Values)
    {
      if (notificationList.Contains(notification))
        notificationList.Remove(notification);
    }
    foreach (List<Notification> notificationList in this.speechBubbleNotToDestoryUponNewNotifier.Values)
    {
      if (notificationList.Contains(notification))
        notificationList.Remove(notification);
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) notification.gameObject);
    UniversalInputManager.Get().SetGameDialogActive(false);
  }

  private IEnumerator WaitAndThenDestroySpeechBubble(
    Notification notification,
    float amountSeconds)
  {
    yield return (object) new WaitForSeconds(amountSeconds);
    if ((UnityEngine.Object) notification != (UnityEngine.Object) null)
      this.NukeSpeechBubble(notification);
  }

  private void NukeSpeechBubble(Notification notification)
  {
    if ((UnityEngine.Object) notification == (UnityEngine.Object) null)
    {
      Log.All.PrintWarning("Attempting to Nuke a Speech Bubble for a Notification that does not exist!");
    }
    else
    {
      if (notification.IsDying())
        return;
      notification.PlaySpeechBubbleDeath();
    }
  }

  public TutorialNotification CreateTutorialDialog(
    string headlineGameString,
    string bodyTextGameString,
    string buttonGameString,
    UIEvent.Handler buttonHandler,
    Vector2 materialOffset,
    bool swapMaterial = false)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "TutorialIntroDialog.prefab:2d189389d0be2f2428bf37ace33e85b1");
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Unable to load tutorial dialog TutorialIntroDialog prefab.");
      return (TutorialNotification) null;
    }
    TutorialNotification notification = gameObject.GetComponent<TutorialNotification>();
    if ((UnityEngine.Object) notification == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "TutorialNotification component does not exist on TutorialIntroDialog prefab.");
      return (TutorialNotification) null;
    }
    TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, OverlayUI.Get().m_heightScale.m_Center);
    if ((bool) UniversalInputManager.UsePhoneUI)
      gameObject.transform.localScale = 1.5f * gameObject.transform.localScale;
    this.popUpDialog = (Notification) notification;
    notification.headlineUberText.Text = GameStrings.Get(headlineGameString);
    notification.speechUberText.Text = GameStrings.Get(bodyTextGameString);
    notification.m_ButtonStart.SetText(GameStrings.Get(buttonGameString));
    if (swapMaterial)
      RendererExtension.SetMaterial((Renderer) notification.artOverlay, notification.swapMaterial);
    RendererExtension.GetMaterial((Renderer) notification.artOverlay).mainTextureOffset = materialOffset;
    notification.m_ButtonStart.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
    {
      if (buttonHandler != null)
        buttonHandler(e);
      notification.m_ButtonStart.ClearEventListeners();
      this.DestroyNotification((Notification) notification, 0.0f);
    }));
    this.popUpDialog.PlayBirth();
    UniversalInputManager.Get().SetGameDialogActive(true);
    return notification;
  }

  public enum PopupTextType
  {
    BASIC,
    FANCY,
  }

  public enum VisualEmoteType
  {
    NONE,
    HOT_STREAK,
    TRIPLE,
    TECH_UP_01,
    TECH_UP_02,
    TECH_UP_03,
    TECH_UP_04,
    TECH_UP_05,
    TECH_UP_06,
    BATTLEGROUNDS_01,
    BATTLEGROUNDS_02,
    BATTLEGROUNDS_03,
    BATTLEGROUNDS_04,
    BATTLEGROUNDS_05,
    BATTLEGROUNDS_06,
    BANANA,
    HERO_BUDDY,
    DOUBLE_HERO_BUDDY,
    COLLECTIBLE_BATTLEGROUNDS_EMOTE,
    QUEST_COMPLETE,
    STORE,
  }

  public class SpeechBubbleOptions
  {
    public string speechText = "";
    public Notification.SpeechBubbleDirection direction = Notification.SpeechBubbleDirection.BottomLeft;
    public Actor actor;
    public bool destroyWhenNewCreated = true;
    public bool parentToActor = true;
    public float bubbleScale;
    public NotificationManager.VisualEmoteType visualEmoteType;
    public int speechBubbleGroup;
    public Action<int> finishCallback;
    public float emoteDuration;
    public int battlegroundsEmoteId;

    public NotificationManager.SpeechBubbleOptions WithSpeechText(
      string speechText)
    {
      this.speechText = speechText;
      return this;
    }

    public NotificationManager.SpeechBubbleOptions WithSpeechBubbleDirection(
      Notification.SpeechBubbleDirection direction)
    {
      this.direction = direction;
      return this;
    }

    public NotificationManager.SpeechBubbleOptions WithActor(Actor actor)
    {
      this.actor = actor;
      return this;
    }

    public NotificationManager.SpeechBubbleOptions WithParentToActor(
      bool parentToActor)
    {
      this.parentToActor = parentToActor;
      return this;
    }

    public NotificationManager.SpeechBubbleOptions WithDestroyWhenNewCreated(
      bool destroyWhenNewCreated)
    {
      this.destroyWhenNewCreated = destroyWhenNewCreated;
      return this;
    }

    public NotificationManager.SpeechBubbleOptions WithBubbleScale(
      float bubbleScale)
    {
      this.bubbleScale = bubbleScale;
      return this;
    }

    public NotificationManager.SpeechBubbleOptions WithVisualEmoteType(
      NotificationManager.VisualEmoteType visualEmoteType)
    {
      this.visualEmoteType = visualEmoteType;
      return this;
    }

    public NotificationManager.SpeechBubbleOptions WithSpeechBubbleGroup(
      int speechBubbleGroup)
    {
      this.speechBubbleGroup = speechBubbleGroup;
      return this;
    }

    public NotificationManager.SpeechBubbleOptions WithFinishCallback(
      Action<int> finishCallback)
    {
      this.finishCallback = finishCallback;
      return this;
    }

    public NotificationManager.SpeechBubbleOptions WithEmoteDuration(
      float emoteDuration)
    {
      this.emoteDuration = emoteDuration;
      return this;
    }

    public NotificationManager.SpeechBubbleOptions WithBattlegroundsEmoteId(
      int id)
    {
      this.battlegroundsEmoteId = id;
      return this;
    }
  }

  private class QuoteSoundCallbackData
  {
    public Notification m_quote;
    public float m_durationSeconds;
    public bool m_persistCharacter;
  }
}
