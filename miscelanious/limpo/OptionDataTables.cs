using Blizzard.T5.Core;

public class OptionDataTables
{
  public static readonly Map<Option, System.Type> s_typeMap = new Map<Option, System.Type>()
  {
    {
      Option.SOUND,
      typeof (bool)
    },
    {
      Option.MUSIC,
      typeof (bool)
    },
    {
      Option.CURSOR,
      typeof (bool)
    },
    {
      Option.HUD,
      typeof (bool)
    },
    {
      Option.STREAMING,
      typeof (bool)
    },
    {
      Option.SOUND_VOLUME,
      typeof (float)
    },
    {
      Option.MUSIC_VOLUME,
      typeof (float)
    },
    {
      Option.SOUND_MONO_ENABLED,
      typeof (bool)
    },
    {
      Option.GFX_WIDTH,
      typeof (int)
    },
    {
      Option.GFX_HEIGHT,
      typeof (int)
    },
    {
      Option.GFX_FULLSCREEN,
      typeof (bool)
    },
    {
      Option.HAS_SEEN_NEW_CINEMATIC,
      typeof (bool)
    },
    {
      Option.GFX_QUALITY,
      typeof (int)
    },
    {
      Option.FAKE_PACK_OPENING,
      typeof (bool)
    },
    {
      Option.FAKE_PACK_COUNT,
      typeof (int)
    },
    {
      Option.HEALTHY_GAMING_DEBUG,
      typeof (bool)
    },
    {
      Option.LAST_SCENE_MODE,
      typeof (int)
    },
    {
      Option.LOCALE,
      typeof (string)
    },
    {
      Option.IDLE_KICKER,
      typeof (bool)
    },
    {
      Option.IDLE_KICK_TIME,
      typeof (string)
    },
    {
      Option.BACKGROUND_SOUND,
      typeof (bool)
    },
    {
      Option.PREFERRED_REGION,
      typeof (int)
    },
    {
      Option.NEARBY_PLAYERS,
      typeof (bool)
    },
    {
      Option.GFX_WIN_CAMERA_CLEAR,
      typeof (bool)
    },
    {
      Option.GFX_MSAA,
      typeof (int)
    },
    {
      Option.GFX_FXAA,
      typeof (bool)
    },
    {
      Option.GFX_TARGET_FRAME_RATE,
      typeof (int)
    },
    {
      Option.GFX_VSYNC,
      typeof (int)
    },
    {
      Option.CARD_BACK,
      typeof (int)
    },
    {
      Option.CARD_BACK2,
      typeof (int)
    },
    {
      Option.LOCAL_TUTORIAL_PROGRESS,
      typeof (int)
    },
    {
      Option.CONNECT_TO_AURORA,
      typeof (bool)
    },
    {
      Option.NEW_USER_LOGIN,
      typeof (bool)
    },
    {
      Option.RECONNECT,
      typeof (bool)
    },
    {
      Option.RECONNECT_TIMEOUT,
      typeof (float)
    },
    {
      Option.RECONNECT_RETRY_TIME,
      typeof (float)
    },
    {
      Option.CHANGED_CARDS_DATA,
      typeof (string)
    },
    {
      Option.KELTHUZADTAUNTS,
      typeof (int)
    },
    {
      Option.GFX_WIN_POSX,
      typeof (int)
    },
    {
      Option.GFX_WIN_POSY,
      typeof (int)
    },
    {
      Option.PREFERRED_CDN_INDEX,
      typeof (int)
    },
    {
      Option.LAST_FAILED_DOP_VERSION,
      typeof (int)
    },
    {
      Option.TOUCH_MODE,
      typeof (bool)
    },
    {
      Option.SHOWN_GFX_DEVICE_WARNING,
      typeof (bool)
    },
    {
      Option.INTRO,
      typeof (bool)
    },
    {
      Option.DISABLE_LOGIN_POPUPS,
      typeof (bool)
    },
    {
      Option.TUTORIAL_LOST_PROGRESS,
      typeof (int)
    },
    {
      Option.ERROR_SCREEN,
      typeof (bool)
    },
    {
      Option.CLIENT_OPTIONS_VERSION,
      typeof (int)
    },
    {
      Option.IKS_VIEW_ATTEMPTS,
      typeof (int)
    },
    {
      Option.IKS_LAST_DOWNLOAD_TIME,
      typeof (ulong)
    },
    {
      Option.IKS_LAST_DOWNLOAD_RESPONSE,
      typeof (string)
    },
    {
      Option.IKS_LAST_STORED_RESPONSE,
      typeof (string)
    },
    {
      Option.IKS_CACHE_AGE,
      typeof (int)
    },
    {
      Option.SEEN_PACK_PRODUCT_LIST,
      typeof (string)
    },
    {
      Option.CHEAT_HISTORY,
      typeof (string)
    },
    {
      Option.PRELOAD_CARD_ASSETS,
      typeof (bool)
    },
    {
      Option.COLLECTION_PREMIUM_TYPE,
      typeof (string)
    },
    {
      Option.DEV_TIMESCALE,
      typeof (float)
    },
    {
      Option.IKS_LAST_SHOWN_AD,
      typeof (string)
    },
    {
      Option.SHOW_STANDARD_ONLY,
      typeof (bool)
    },
    {
      Option.DISABLE_SET_ROTATION_INTRO,
      typeof (bool)
    },
    {
      Option.FORCE_SHOW_IKS,
      typeof (bool)
    },
    {
      Option.PEGUI_DEBUG,
      typeof (int)
    },
    {
      Option.SKIP_ALL_MULLIGANS,
      typeof (bool)
    },
    {
      Option.IS_TEMPORARY_ACCOUNT_CHEAT,
      typeof (bool)
    },
    {
      Option.TEMPORARY_ACCOUNT_DATA,
      typeof (string)
    },
    {
      Option.DISALLOWED_CLOUD_STORAGE,
      typeof (bool)
    },
    {
      Option.CREATED_ACCOUNT,
      typeof (bool)
    },
    {
      Option.LAST_HEAL_UP_EVENT_DATE,
      typeof (long)
    },
    {
      Option.LATEST_SEEN_TAVERNBRAWL_SESSION_LIMIT,
      typeof (int)
    },
    {
      Option.PUSH_NOTIFICATION_STATUS,
      typeof (int)
    },
    {
      Option.DBF_XML_LOADING,
      typeof (bool)
    },
    {
      Option.HAS_SHOWN_DEVICE_PERFORMANCE_WARNING,
      typeof (bool)
    },
    {
      Option.HAS_SHOWN_MINSPEC_NEXT_VERSION_WARNING,
      typeof (bool)
    },
    {
      Option.SCREENSHOT_DIRECTORY,
      typeof (string)
    },
    {
      Option.SIMULATE_CELLULAR,
      typeof (bool)
    },
    {
      Option.ASSET_DOWNLOAD_ENABLED,
      typeof (bool)
    },
    {
      Option.UPDATE_STATE,
      typeof (int)
    },
    {
      Option.NATIVE_UPDATE_STATE,
      typeof (string)
    },
    {
      Option.ASK_UNKNOWN_APPS,
      typeof (bool)
    },
    {
      Option.LAUNCH_COUNT,
      typeof (int)
    },
    {
      Option.IS_INSTALL_REPORTED,
      typeof (bool)
    },
    {
      Option.FIRST_INSTALL_TIME,
      typeof (ulong)
    },
    {
      Option.UPDATED_CLIENT_VERSION,
      typeof (string)
    },
    {
      Option.UPDATE_STOP_LEVEL,
      typeof (int)
    },
    {
      Option.SIMULATE_NO_INTERNET,
      typeof (bool)
    },
    {
      Option.MAX_DOWNLOAD_SPEED,
      typeof (int)
    },
    {
      Option.STREAMING_SPEED_IN_GAME,
      typeof (int)
    },
    {
      Option.AUTOCONVERT_VIRTUAL_CURRENCY,
      typeof (bool)
    },
    {
      Option.STREAMER_MODE,
      typeof (bool)
    },
    {
      Option.LATEST_SEEN_SHOP_PRODUCT_LIST,
      typeof (string)
    },
    {
      Option.LATEST_DISPLAYED_SHOP_PRODUCT_LIST,
      typeof (string)
    },
    {
      Option.RATING_DEBUG,
      typeof (int)
    },
    {
      Option.DEBUG_CURSOR,
      typeof (bool)
    },
    {
      Option.CRASH_COUNT,
      typeof (int)
    },
    {
      Option.EXCEPTION_COUNT,
      typeof (int)
    },
    {
      Option.LOW_MEMORY_COUNT,
      typeof (int)
    },
    {
      Option.CLOSED_WITHOUT_CRASH,
      typeof (bool)
    },
    {
      Option.EXCEPTION_HASH,
      typeof (string)
    },
    {
      Option.LAST_EXCEPTION_HASH,
      typeof (string)
    },
    {
      Option.CRASH_IN_A_ROW_COUNT,
      typeof (int)
    },
    {
      Option.SAME_EXCEPTION_COUNT,
      typeof (int)
    },
    {
      Option.CELL_PROMPT_THRESHOLD,
      typeof (int)
    },
    {
      Option.DOWNLOAD_ALL_FINISHED,
      typeof (bool)
    },
    {
      Option.DELAYED_REPORTER_STOP,
      typeof (bool)
    },
    {
      Option.SCREEN_SHAKE_ENABLED,
      typeof (bool)
    },
    {
      Option.HUD_CONFIG,
      typeof (string)
    },
    {
      Option.HUD_SCALE,
      typeof (float)
    },
    {
      Option.ENABLED_LOG_LIST,
      typeof (string)
    },
    {
      Option.HAS_SEEN_CLIPBOARD_NOTIFICATION,
      typeof (bool)
    },
    {
      Option.PROG_TILE_DEBUG,
      typeof (bool)
    },
    {
      Option.EARLY_CONCEDE_CONFIRMATION_DISABLED,
      typeof (bool)
    },
    {
      Option.PROG_HIDDEN_ACHIEVEMENTS,
      typeof (bool)
    },
    {
      Option.LAST_LOGIN_TYPE,
      typeof (int)
    },
    {
      Option.TRANSITION_AUTH_TOKEN,
      typeof (string)
    },
    {
      Option.TRANSITION_GUEST_ID,
      typeof (string)
    },
    {
      Option.ANR_THROTTLE,
      typeof (float)
    },
    {
      Option.ANR_WAIT_SECONDS,
      typeof (float)
    },
    {
      Option.HAS_ACCEPTED_PRIVACY_POLICY_AND_EULA,
      typeof (bool)
    },
    {
      Option.APP_RATING_POPUP_COUNT,
      typeof (int)
    },
    {
      Option.NEWEST_REWARDED_DECK_ID,
      typeof (long)
    },
    {
      Option.SHOW_CREATE_SKIP_ACCT,
      typeof (bool)
    },
    {
      Option.DEBUG_SHOW_PRODUCT_IDS,
      typeof (bool)
    },
    {
      Option.APKINSTALL_START,
      typeof (string)
    },
    {
      Option.APKINSTALL_FAILURE_REPORTED,
      typeof (bool)
    },
    {
      Option.INTERNET_UNREACHABLE,
      typeof (string)
    },
    {
      Option.DEBUG_SHOW_BATTLEGROUND_SKIN_IDS,
      typeof (bool)
    },
    {
      Option.DEBUG_START_GADGET_ON_STARTUP,
      typeof (string)
    },
    {
      Option.DEBUG_GADGET_START_PAGE,
      typeof (string)
    },
    {
      Option.DEBUG_GADGET_START_LOGS,
      typeof (string)
    },
    {
      Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_TRADITIONAL,
      typeof (bool)
    },
    {
      Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_BATTLEGROUNDS,
      typeof (bool)
    },
    {
      Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_MERCENARIES,
      typeof (bool)
    },
    {
      Option.INSTALLED_LOCALES,
      typeof (string)
    },
    {
      Option.CURRENT_LOCALE_TIME_STAMP,
      typeof (ulong)
    },
    {
      Option.AF_FIRST_BOX_AFTER_TUTORIAL,
      typeof (bool)
    },
    {
      Option.AF_FIRST_SHOP_VISIT,
      typeof (bool)
    },
    {
      Option.AF_FIRST_PACK_OPENED,
      typeof (bool)
    },
    {
      Option.AF_FIRST_NON_TUTORIAL_GAME_START_TRADITIONAL,
      typeof (bool)
    },
    {
      Option.AF_FIRST_NON_TUTORIAL_GAME_START_BATTLEGROUNDS,
      typeof (bool)
    },
    {
      Option.AF_FIRST_NON_TUTORIAL_GAME_START_MERCENARIES,
      typeof (bool)
    },
    {
      Option.AF_REWARD_TRACK_EVENT,
      typeof (bool)
    },
    {
      Option.PAGE_MOUSE_OVERS,
      typeof (int)
    },
    {
      Option.COVER_MOUSE_OVERS,
      typeof (int)
    },
    {
      Option.LAST_PRECON_HERO_CHOSEN,
      typeof (int)
    },
    {
      Option.AI_MODE,
      typeof (int)
    },
    {
      Option.TIP_PRACTICE_PROGRESS,
      typeof (int)
    },
    {
      Option.TIP_PLAY_PROGRESS,
      typeof (int)
    },
    {
      Option.TIP_FORGE_PROGRESS,
      typeof (int)
    },
    {
      Option.LAST_CUSTOM_DECK_CHOSEN,
      typeof (long)
    },
    {
      Option.SELECTED_ADVENTURE,
      typeof (int)
    },
    {
      Option.SELECTED_ADVENTURE_MODE,
      typeof (int)
    },
    {
      Option.LAST_SELECTED_STORE_BOOSTER_ID,
      typeof (int)
    },
    {
      Option.LAST_SELECTED_STORE_ADVENTURE_ID,
      typeof (int)
    },
    {
      Option.SERVER_OPTIONS_VERSION,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_TAVERNBRAWL_SEASON,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD,
      typeof (int)
    },
    {
      Option.LAST_SELECTED_STORE_HERO_ID,
      typeof (int)
    },
    {
      Option.TIMES_SEEN_TAVERNBRAWL_CRAZY_RULES_QUOTE,
      typeof (int)
    },
    {
      Option.SKIP_DECK_TEMPLATE_PAGE_FOR_CLASS_FLAGS,
      typeof (int)
    },
    {
      Option.SET_ROTATION_INTRO_PROGRESS,
      typeof (int)
    },
    {
      Option.TIMES_MOUSED_OVER_SWITCH_FORMAT_BUTTON,
      typeof (int)
    },
    {
      Option.LAST_TAVERN_JOINED,
      typeof (long)
    },
    {
      Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON_CHALKBOARD,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_SCHEDULED_DOUBLE_GOLD_VO,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_SCHEDULED_ALL_POPUPS_SHOWN_VO,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_SCHEDULED_ENTERED_ARENA_DRAFT,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_SCHEDULED_LOGIN_FLOW_COMPLETE,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_WELCOME_QUEST_DIALOG,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_ARENA_SEASON_STARTING,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_ARENA_SEASON_ENDING,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_CURRENCY_CHANGED_VERSION,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_SCHEDULED_WELCOME_QUEST_SHOWN_VO,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_SCHEDULED_GENERIC_REWARD_SHOWN_VO,
      typeof (int)
    },
    {
      Option.LAST_SELECTED_STORE_PACK_TYPE,
      typeof (int)
    },
    {
      Option.LATEST_SEEN_SCHEDULED_ARENA_REWARD_SHOWN_VO,
      typeof (int)
    },
    {
      Option.WHIZBANG_POPUP_COUNTER,
      typeof (int)
    },
    {
      Option.SET_ROTATION_INTRO_PROGRESS_NEW_PLAYER,
      typeof (int)
    },
    {
      Option.FORMAT_TYPE,
      typeof (int)
    },
    {
      Option.FORMAT_TYPE_LAST_PLAYED,
      typeof (int)
    },
    {
      Option.AADC_LOCAL_SAVE_TIME_STAMP,
      typeof (ulong)
    },
    {
      Option.AADC_SERVER_SAVE_TIME_STAMP,
      typeof (ulong)
    },
    {
      Option.AADC_DISABLE_CHAT,
      typeof (bool)
    },
    {
      Option.AADC_DISABLE_GEOLOCATION,
      typeof (bool)
    },
    {
      Option.AADC_DISABLE_NEARBY_FRIENDS,
      typeof (bool)
    },
    {
      Option.AADC_DISABLE_PERSONALIZED_PRODUCTS,
      typeof (bool)
    },
    {
      Option.AADC_PUSH_NOTIFICATIONS,
      typeof (bool)
    },
    {
      Option.AADC_DISABLE_AB_TESTING,
      typeof (bool)
    }
  };
  public static readonly Map<Option, object> s_defaultsMap = new Map<Option, object>()
  {
    {
      Option.SOUND,
      (object) true
    },
    {
      Option.MUSIC,
      (object) true
    },
    {
      Option.CURSOR,
      (object) true
    },
    {
      Option.HUD,
      (object) true
    },
    {
      Option.STREAMING,
      (object) true
    },
    {
      Option.SOUND_VOLUME,
      (object) 1f
    },
    {
      Option.MUSIC_VOLUME,
      (object) 1f
    },
    {
      Option.SOUND_MONO_ENABLED,
      (object) false
    },
    {
      Option.GFX_FULLSCREEN,
      (object) true
    },
    {
      Option.GFX_QUALITY,
      (object) 1
    },
    {
      Option.IDLE_KICKER,
      (object) true
    },
    {
      Option.IDLE_KICK_TIME,
      (object) "30 min"
    },
    {
      Option.BACKGROUND_SOUND,
      (object) true
    },
    {
      Option.PREFERRED_REGION,
      (object) -1
    },
    {
      Option.NEARBY_PLAYERS,
      (object) true
    },
    {
      Option.LOCAL_TUTORIAL_PROGRESS,
      (object) TutorialProgress.NOTHING_COMPLETE
    },
    {
      Option.CONNECT_TO_AURORA,
      (object) false
    },
    {
      Option.NEW_USER_LOGIN,
      (object) false
    },
    {
      Option.RECONNECT,
      (object) true
    },
    {
      Option.RECONNECT_TIMEOUT,
      (object) 60f
    },
    {
      Option.RECONNECT_RETRY_TIME,
      (object) 5f
    },
    {
      Option.TOUCH_MODE,
      (object) false
    },
    {
      Option.SHOWN_GFX_DEVICE_WARNING,
      (object) false
    },
    {
      Option.INTRO,
      (object) true
    },
    {
      Option.TUTORIAL_LOST_PROGRESS,
      (object) 0
    },
    {
      Option.ERROR_SCREEN,
      (object) true
    },
    {
      Option.IKS_VIEW_ATTEMPTS,
      (object) 0
    },
    {
      Option.IKS_LAST_DOWNLOAD_TIME,
      (object) 0UL
    },
    {
      Option.IKS_LAST_DOWNLOAD_RESPONSE,
      (object) ""
    },
    {
      Option.IKS_LAST_STORED_RESPONSE,
      (object) ""
    },
    {
      Option.IKS_CACHE_AGE,
      (object) 300
    },
    {
      Option.SEEN_PACK_PRODUCT_LIST,
      (object) ""
    },
    {
      Option.CHEAT_HISTORY,
      (object) ""
    },
    {
      Option.PRELOAD_CARD_ASSETS,
      (object) false
    },
    {
      Option.DEV_TIMESCALE,
      (object) 1f
    },
    {
      Option.IKS_LAST_SHOWN_AD,
      (object) ""
    },
    {
      Option.SHOW_STANDARD_ONLY,
      (object) false
    },
    {
      Option.FORCE_SHOW_IKS,
      (object) false
    },
    {
      Option.PEGUI_DEBUG,
      (object) 0
    },
    {
      Option.IS_TEMPORARY_ACCOUNT_CHEAT,
      (object) false
    },
    {
      Option.TEMPORARY_ACCOUNT_DATA,
      (object) ""
    },
    {
      Option.DISALLOWED_CLOUD_STORAGE,
      (object) false
    },
    {
      Option.CREATED_ACCOUNT,
      (object) false
    },
    {
      Option.LAST_HEAL_UP_EVENT_DATE,
      (object) 0L
    },
    {
      Option.PUSH_NOTIFICATION_STATUS,
      (object) 1
    },
    {
      Option.HAS_SHOWN_DEVICE_PERFORMANCE_WARNING,
      (object) false
    },
    {
      Option.HAS_SHOWN_MINSPEC_NEXT_VERSION_WARNING,
      (object) false
    },
    {
      Option.UPDATE_STATE,
      (object) 0
    },
    {
      Option.NATIVE_UPDATE_STATE,
      (object) ""
    },
    {
      Option.ASK_UNKNOWN_APPS,
      (object) true
    },
    {
      Option.MAX_DOWNLOAD_SPEED,
      (object) 0
    },
    {
      Option.STREAMING_SPEED_IN_GAME,
      (object) 512000
    },
    {
      Option.AUTOCONVERT_VIRTUAL_CURRENCY,
      (object) false
    },
    {
      Option.STREAMER_MODE,
      (object) false
    },
    {
      Option.LATEST_SEEN_SHOP_PRODUCT_LIST,
      (object) ""
    },
    {
      Option.LATEST_DISPLAYED_SHOP_PRODUCT_LIST,
      (object) ""
    },
    {
      Option.RATING_DEBUG,
      (object) RatingDebugOption.INVALID
    },
    {
      Option.CRASH_COUNT,
      (object) 0
    },
    {
      Option.EXCEPTION_COUNT,
      (object) 0
    },
    {
      Option.LOW_MEMORY_COUNT,
      (object) 0
    },
    {
      Option.CLOSED_WITHOUT_CRASH,
      (object) true
    },
    {
      Option.EXCEPTION_HASH,
      (object) ""
    },
    {
      Option.LAST_EXCEPTION_HASH,
      (object) ""
    },
    {
      Option.CRASH_IN_A_ROW_COUNT,
      (object) 0
    },
    {
      Option.SAME_EXCEPTION_COUNT,
      (object) 0
    },
    {
      Option.CELL_PROMPT_THRESHOLD,
      (object) 20971520
    },
    {
      Option.DOWNLOAD_ALL_FINISHED,
      (object) false
    },
    {
      Option.DELAYED_REPORTER_STOP,
      (object) false
    },
    {
      Option.HUD_CONFIG,
      (object) ""
    },
    {
      Option.HUD_SCALE,
      (object) 1f
    },
    {
      Option.ENABLED_LOG_LIST,
      (object) ""
    },
    {
      Option.HAS_SEEN_CLIPBOARD_NOTIFICATION,
      (object) false
    },
    {
      Option.PROG_TILE_DEBUG,
      (object) false
    },
    {
      Option.PROG_HIDDEN_ACHIEVEMENTS,
      (object) false
    },
    {
      Option.EARLY_CONCEDE_CONFIRMATION_DISABLED,
      (object) false
    },
    {
      Option.ANR_THROTTLE,
      (object) 0.01f
    },
    {
      Option.ANR_WAIT_SECONDS,
      (object) 10f
    },
    {
      Option.APP_RATING_POPUP_COUNT,
      (object) 0
    },
    {
      Option.NEWEST_REWARDED_DECK_ID,
      (object) 0L
    },
    {
      Option.SHOW_CREATE_SKIP_ACCT,
      (object) false
    },
    {
      Option.DEBUG_SHOW_PRODUCT_IDS,
      (object) false
    },
    {
      Option.APKINSTALL_START,
      (object) ""
    },
    {
      Option.APKINSTALL_FAILURE_REPORTED,
      (object) false
    },
    {
      Option.INTERNET_UNREACHABLE,
      (object) ""
    },
    {
      Option.LAST_SELECTED_STORE_PACK_TYPE,
      (object) 1
    },
    {
      Option.ASSET_DOWNLOAD_ENABLED,
      (object) true
    },
    {
      Option.IN_RANKED_PLAY_MODE,
      (object) true
    },
    {
      Option.SPECTATOR_OPEN_JOIN,
      (object) true
    },
    {
      Option.SCREEN_SHAKE_ENABLED,
      (object) true
    },
    {
      Option.LATEST_SEEN_TAVERNBRAWL_SEASON,
      (object) 0
    },
    {
      Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD,
      (object) 0
    },
    {
      Option.TIMES_SEEN_TAVERNBRAWL_CRAZY_RULES_QUOTE,
      (object) 0
    },
    {
      Option.SKIP_DECK_TEMPLATE_PAGE_FOR_CLASS_FLAGS,
      (object) 0
    },
    {
      Option.HAS_SEEN_WILD_MODE_VO,
      (object) false
    },
    {
      Option.NEEDS_TO_MAKE_STANDARD_DECK,
      (object) true
    },
    {
      Option.HAS_SEEN_INVALID_ROTATED_CARD,
      (object) false
    },
    {
      Option.SHOW_SWITCH_TO_WILD_ON_PLAY_SCREEN,
      (object) false
    },
    {
      Option.SHOW_SWITCH_TO_WILD_ON_CREATE_DECK,
      (object) false
    },
    {
      Option.SHOW_WILD_DISCLAIMER_POPUP_ON_CREATE_DECK,
      (object) true
    },
    {
      Option.HAS_SEEN_BASIC_DECK_WARNING,
      (object) false
    },
    {
      Option.GLOW_COLLECTION_BUTTON_AFTER_SET_ROTATION,
      (object) false
    },
    {
      Option.HAS_SEEN_RAF,
      (object) false
    },
    {
      Option.HAS_SEEN_RAF_RECRUIT_URL,
      (object) false
    },
    {
      Option.SHOULD_AUTO_CHECK_IN_TO_FIRESIDE_GATHERINGS,
      (object) true
    },
    {
      Option.HAS_CLICKED_FIRESIDE_GATHERINGS_BUTTON,
      (object) false
    },
    {
      Option.HAS_INITIATED_FIRESIDE_GATHERING_SCAN,
      (object) false
    },
    {
      Option.LATEST_SEEN_SCHEDULED_DOUBLE_GOLD_VO,
      (object) 0
    },
    {
      Option.LATEST_SEEN_SCHEDULED_ALL_POPUPS_SHOWN_VO,
      (object) 0
    },
    {
      Option.LATEST_SEEN_SCHEDULED_ENTERED_ARENA_DRAFT,
      (object) 0
    },
    {
      Option.LATEST_SEEN_SCHEDULED_LOGIN_FLOW_COMPLETE,
      (object) 0
    },
    {
      Option.LATEST_SEEN_WELCOME_QUEST_DIALOG,
      (object) 0
    },
    {
      Option.LATEST_SEEN_ARENA_SEASON_STARTING,
      (object) 0
    },
    {
      Option.LATEST_SEEN_ARENA_SEASON_ENDING,
      (object) 0
    },
    {
      Option.LATEST_SEEN_CURRENCY_CHANGED_VERSION,
      (object) 0
    },
    {
      Option.WHIZBANG_POPUP_COUNTER,
      (object) 0
    },
    {
      Option.SET_ROTATION_INTRO_PROGRESS_NEW_PLAYER,
      (object) 0
    },
    {
      Option.FORMAT_TYPE,
      (object) 2
    },
    {
      Option.FORMAT_TYPE_LAST_PLAYED,
      (object) 2
    },
    {
      Option.HAS_SEEN_CLASSIC_MODE_VO,
      (object) false
    },
    {
      Option.DEBUG_SHOW_BATTLEGROUND_SKIN_IDS,
      (object) false
    },
    {
      Option.AADC_LOCAL_SAVE_TIME_STAMP,
      (object) 0UL
    },
    {
      Option.AADC_SERVER_SAVE_TIME_STAMP,
      (object) 0UL
    },
    {
      Option.AADC_DISABLE_CHAT,
      (object) true
    },
    {
      Option.AADC_DISABLE_GEOLOCATION,
      (object) true
    },
    {
      Option.AADC_DISABLE_NEARBY_FRIENDS,
      (object) true
    },
    {
      Option.AADC_DISABLE_PERSONALIZED_PRODUCTS,
      (object) true
    },
    {
      Option.AADC_PUSH_NOTIFICATIONS,
      (object) true
    },
    {
      Option.AADC_DISABLE_AB_TESTING,
      (object) true
    },
    {
      Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_TRADITIONAL,
      (object) false
    },
    {
      Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_BATTLEGROUNDS,
      (object) false
    },
    {
      Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_MERCENARIES,
      (object) false
    },
    {
      Option.INSTALLED_LOCALES,
      (object) ""
    },
    {
      Option.CURRENT_LOCALE_TIME_STAMP,
      (object) 0UL
    },
    {
      Option.AF_FIRST_BOX_AFTER_TUTORIAL,
      (object) false
    },
    {
      Option.AF_FIRST_SHOP_VISIT,
      (object) false
    },
    {
      Option.AF_FIRST_PACK_OPENED,
      (object) false
    },
    {
      Option.AF_FIRST_NON_TUTORIAL_GAME_START_TRADITIONAL,
      (object) false
    },
    {
      Option.AF_FIRST_NON_TUTORIAL_GAME_START_BATTLEGROUNDS,
      (object) false
    },
    {
      Option.AF_FIRST_NON_TUTORIAL_GAME_START_MERCENARIES,
      (object) false
    },
    {
      Option.AF_REWARD_TRACK_EVENT,
      (object) false
    }
  };
}
