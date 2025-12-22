using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.Core.Streaming;
using Hearthstone.Progression;
using Hearthstone.Streaming;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : Entity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = GameEntity.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = GameEntity.InitStringOptions();
  private Map<string, AudioSource> m_preloadedSounds = new Map<string, AudioSource>();
  private int m_preloadsNeeded;
  private int m_realTimeTurn;
  private int m_realTimeStep;
  private Spell m_endOfGameSpell;
  private const string DefaultEmoteHandlerReference = "EmoteHandler.prefab:5d44be0e8bb7fd14d9fbdbda6a74ab91";
  private const string BattlegroundsEmoteHandlerReference = "BattlegroundsEmoteHandler.prefab:212598c2e67d4b74c85d4913af706d9b";
  private const string EnemyEmoteHandlerReference = "EnemyEmoteHandler.prefab:6ace3edd8826cad4aaa0d0e0eb085012";
  private Coroutine m_destroyHeroTrackingCoroutine;
  private readonly WaitForSeconds MAX_DESTROY_HERO_TIME = new WaitForSeconds(10f);
  private static MonoBehaviour s_coroutines;
  protected GameEntityOptions m_gameOptions = new GameEntityOptions(GameEntity.s_booleanOptions, GameEntity.s_stringOptions);
  private int m_inputBlockerCount;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.ALWAYS_SHOW_MULLIGAN_TIMER,
      false
    },
    {
      GameEntityOption.MULLIGAN_IS_CHOOSE_ONE,
      false
    },
    {
      GameEntityOption.MULLIGAN_TIMER_HAS_ALTERNATE_POSITION,
      false
    },
    {
      GameEntityOption.CARDS_IN_TOOLTIP_SHIFTED_DURING_MULLIGAN,
      false
    },
    {
      GameEntityOption.MULLIGAN_REQUIRES_CONFIRMATION,
      true
    },
    {
      GameEntityOption.MULLIGAN_HAS_HERO_LOBBY,
      false
    },
    {
      GameEntityOption.DIM_OPPOSING_HERO_DURING_MULLIGAN,
      false
    },
    {
      GameEntityOption.HANDLE_COIN,
      true
    },
    {
      GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS,
      false
    },
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      true
    },
    {
      GameEntityOption.SUPPRESS_CLASS_NAMES,
      false
    },
    {
      GameEntityOption.USE_SECRET_CLASS_NAMES,
      true
    },
    {
      GameEntityOption.ALLOW_NAME_BANNER_MODE_ICONS,
      true
    },
    {
      GameEntityOption.CAN_ADJUST_BIG_CARD_HORIZONTALLY,
      false
    },
    {
      GameEntityOption.USE_BONES_FOR_BIG_CARD_PLACEMENT,
      false
    },
    {
      GameEntityOption.USE_BONES_FOR_TOOLTIP_PLACEMENT,
      false
    },
    {
      GameEntityOption.USE_COMPACT_ENCHANTMENT_BANNERS,
      false
    },
    {
      GameEntityOption.ALLOW_FATIGUE,
      true
    },
    {
      GameEntityOption.MOUSEOVER_DELAY_OVERRIDDEN,
      false
    },
    {
      GameEntityOption.ALLOW_ENCHANTMENT_SPARKLES,
      true
    },
    {
      GameEntityOption.ALLOW_SLEEP_FX,
      true
    },
    {
      GameEntityOption.HAS_ALTERNATE_ENEMY_EMOTE_ACTOR,
      false
    },
    {
      GameEntityOption.USES_PREMIUM_EMOTES,
      false
    },
    {
      GameEntityOption.CAN_SQUELCH_OPPONENT,
      true
    },
    {
      GameEntityOption.KEYWORD_HELP_DELAY_OVERRIDDEN,
      false
    },
    {
      GameEntityOption.SHOW_CRAZY_KEYWORD_TOOLTIP,
      false
    },
    {
      GameEntityOption.SHOW_HERO_TOOLTIPS,
      false
    },
    {
      GameEntityOption.USES_BIG_CARDS,
      true
    },
    {
      GameEntityOption.DISABLE_TOOLTIPS,
      false
    },
    {
      GameEntityOption.DELAY_CARD_SOUND_SPELLS,
      false
    },
    {
      GameEntityOption.DISPLAY_MULLIGAN_DETAIL_LABEL,
      false
    },
    {
      GameEntityOption.WAIT_FOR_RATING_INFO,
      true
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>()
  {
    {
      GameEntityOption.ALTERNATE_MULLIGAN_ACTOR_NAME,
      (string) null
    },
    {
      GameEntityOption.ALTERNATE_MULLIGAN_LOBBY_ACTOR_NAME,
      (string) null
    },
    {
      GameEntityOption.VICTORY_SCREEN_PREFAB_PATH,
      "VictoryTwoScoop.prefab:b31e3c6c1e80ced4183c3e231c567669"
    },
    {
      GameEntityOption.DEFEAT_SCREEN_PREFAB_PATH,
      "DefeatTwoScoop.prefab:6535dd92d63fce1478220e9bc50e926b"
    },
    {
      GameEntityOption.RULEBOOK_POPUP_PREFAB_PATH,
      (string) null
    },
    {
      GameEntityOption.VICTORY_AUDIO_PATH,
      "victory_jingle.prefab:23f19dd07c7a5114abe5f525099cbac4"
    },
    {
      GameEntityOption.DEFEAT_AUDIO_PATH,
      "defeat_jingle.prefab:0744a10f38e92f1438a02349c29a7b76"
    }
  };

  public string Uuid { get; set; }

  public List<Network.HistCreateGame.ActionInfo> OnLoadActions { get; } = new List<Network.HistCreateGame.ActionInfo>();

  protected static MonoBehaviour Coroutines
  {
    get
    {
      if ((UnityEngine.Object) GameEntity.s_coroutines == (UnityEngine.Object) null)
        GameEntity.s_coroutines = (MonoBehaviour) new GameObject().AddComponent<EmptyScript>();
      return GameEntity.s_coroutines;
    }
  }

  public void AddInputBlocker() => ++this.m_inputBlockerCount;

  public void RemoveInputBlocker() => --this.m_inputBlockerCount;

  public bool IsInputEnabled() => this.m_inputBlockerCount <= 0;

  private void OnGameplaySceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode != SceneMgr.Mode.GAMEPLAY)
      return;
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
    RemoteActionHandler remoteActionHandler = RemoteActionHandler.Get();
    foreach (Network.HistCreateGame.ActionInfo onLoadAction in this.OnLoadActions)
    {
      Network.UserUI newData = new Network.UserUI()
      {
        playerId = new int?(onLoadAction.PlayerID),
        selectionInfo = new Network.UserUI.SelectionInfo()
      };
      newData.selectionInfo.SelectedEntityID = onLoadAction.SelectedEntityID;
      remoteActionHandler.HandleAction(newData);
    }
    this.OnLoadActions.Clear();
  }

  public GameEntity()
  {
    this.PreloadAssets();
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
  }

  public virtual void OnCreate()
  {
  }

  public virtual void OnCreateGame()
  {
  }

  public virtual void OnDecommissionGame()
  {
    if (SceneMgr.Get() == null)
      return;
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
  }

  public void FadeOutHeroActor(Actor actorToFade)
  {
    this.ToggleSpotLight(actorToFade.GetHeroSpotlight(), false);
    Renderer component = actorToFade.m_portraitMesh.GetComponent<Renderer>();
    Material heroMat = RendererExtension.GetMaterial(component, actorToFade.m_portraitMatIdx);
    Material heroFrameMat = RendererExtension.GetMaterial(component, actorToFade.m_portraitFrameMatIdx);
    Hashtable args = iTween.Hash((object) "time", (object) 0.25f, (object) "from", (object) heroMat.GetFloat("_LightingBlend"), (object) "to", (object) 1f, (object) "onupdate", (object) (Action<object>) (amount =>
    {
      if (!(bool) (UnityEngine.Object) heroMat || !(bool) (UnityEngine.Object) heroFrameMat)
      {
        Log.Graphics.PrintWarning("Actor's portrait HeroMat or HeroFrameMat materials are null");
      }
      else
      {
        heroMat.SetFloat("_LightingBlend", (float) amount);
        heroFrameMat.SetFloat("_LightingBlend", (float) amount);
      }
    }), (object) "onupdatetarget", (object) actorToFade.gameObject);
    iTween.ValueTo(actorToFade.gameObject, args);
  }

  public void FadeOutActor(Actor actorToFade)
  {
    Renderer component = actorToFade.m_portraitMesh.GetComponent<Renderer>();
    Material mat = RendererExtension.GetMaterial(component, actorToFade.m_portraitMatIdx);
    Material frameMat = RendererExtension.GetMaterial(component, actorToFade.m_portraitFrameMatIdx);
    Hashtable args = iTween.Hash((object) "time", (object) 0.25f, (object) "from", (object) mat.GetFloat("_LightingBlend"), (object) "to", (object) 1f, (object) "onupdate", (object) (Action<object>) (amount =>
    {
      mat.SetFloat("_LightingBlend", (float) amount);
      frameMat.SetFloat("_LightingBlend", (float) amount);
    }), (object) "onupdatetarget", (object) actorToFade.gameObject);
    iTween.ValueTo(actorToFade.gameObject, args);
  }

  private void ToggleSpotLight(Light light, bool bOn)
  {
    float num1 = 0.1f;
    float num2 = 1.3f;
    Action<object> action1 = (Action<object>) (amount => light.intensity = (float) amount);
    Action<object> action2 = (Action<object>) (args => light.enabled = false);
    if (bOn)
    {
      light.enabled = true;
      light.intensity = 0.0f;
      Hashtable args = iTween.Hash((object) "time", (object) num1, (object) "from", (object) 0.0f, (object) "to", (object) num2, (object) "onupdate", (object) action1, (object) "onupdatetarget", (object) light.gameObject);
      iTween.ValueTo(light.gameObject, args);
    }
    else
    {
      Hashtable args = iTween.Hash((object) "time", (object) num1, (object) "from", (object) light.intensity, (object) "to", (object) 0.0f, (object) "onupdate", (object) action1, (object) "onupdatetarget", (object) light.gameObject, (object) "oncomplete", (object) action2);
      iTween.ValueTo(light.gameObject, args);
    }
  }

  public void FadeInHeroActor(Actor actorToFade) => this.FadeInHeroActor(actorToFade, 0.0f);

  public void FadeInHeroActor(Actor actorToFade, float lightBlendAmount)
  {
    if (!(bool) (UnityEngine.Object) actorToFade)
    {
      Log.Graphics.PrintWarning("Actor to fade is null!");
    }
    else
    {
      this.ToggleSpotLight(actorToFade.GetHeroSpotlight(), true);
      if (!(bool) (UnityEngine.Object) actorToFade.m_portraitMesh)
      {
        Log.Graphics.PrintWarning("Actor's portrait mesh is null!");
      }
      else
      {
        Renderer component = actorToFade.m_portraitMesh.GetComponent<Renderer>();
        if (!(bool) (UnityEngine.Object) component)
        {
          Log.Graphics.PrintWarning("Actor's portrait mesh component render is null!");
        }
        else
        {
          Material heroMat = RendererExtension.GetMaterial(component, actorToFade.m_portraitMatIdx);
          Material heroFrameMat = RendererExtension.GetMaterial(component, actorToFade.m_portraitFrameMatIdx);
          if (!(bool) (UnityEngine.Object) heroMat || !(bool) (UnityEngine.Object) heroFrameMat)
          {
            Log.Graphics.PrintWarning("Actor's portrait HeroMat or HeroFrameMat materials are null");
          }
          else
          {
            float num = heroMat.GetFloat("_LightingBlend");
            Action<object> action = (Action<object>) (amount =>
            {
              if (!(bool) (UnityEngine.Object) heroMat || !(bool) (UnityEngine.Object) heroFrameMat)
              {
                Log.Graphics.PrintWarning("Actor's portrait HeroMat or HeroFrameMat materials are null");
              }
              else
              {
                heroMat.SetFloat("_LightingBlend", (float) amount);
                heroFrameMat.SetFloat("_LightingBlend", (float) amount);
              }
            });
            Hashtable args = iTween.Hash((object) "time", (object) 0.25f, (object) "from", (object) num, (object) "to", (object) lightBlendAmount, (object) "onupdate", (object) action, (object) "onupdatetarget", (object) actorToFade.gameObject);
            iTween.ValueTo(actorToFade.gameObject, args);
          }
        }
      }
    }
  }

  public void FadeInActor(Actor actorToFade) => this.FadeInActor(actorToFade, 0.0f);

  public void FadeInActor(Actor actorToFade, float lightBlendAmount)
  {
    Renderer component = actorToFade.m_portraitMesh.GetComponent<Renderer>();
    Material mat = RendererExtension.GetMaterial(component, actorToFade.m_portraitMatIdx);
    Material frameMat = RendererExtension.GetMaterial(component, actorToFade.m_portraitFrameMatIdx);
    float num = mat.GetFloat("_LightingBlend");
    Action<object> action = (Action<object>) (amount =>
    {
      mat.SetFloat("_LightingBlend", (float) amount);
      frameMat.SetFloat("_LightingBlend", (float) amount);
    });
    Hashtable args = iTween.Hash((object) "time", (object) 0.25f, (object) "from", (object) num, (object) "to", (object) lightBlendAmount, (object) "onupdate", (object) action, (object) "onupdatetarget", (object) actorToFade.gameObject);
    iTween.ValueTo(actorToFade.gameObject, args);
  }

  public void PreloadSound(string soundPath)
  {
    ++this.m_preloadsNeeded;
    SoundLoader.LoadSound((AssetReference) soundPath, new PrefabCallback<GameObject>(this.OnSoundLoaded), fallback: SoundManager.Get().GetPlaceholderSound());
  }

  protected void PreloadPrefab(
    AssetReference assetRef,
    PrefabCallback<GameObject> callback,
    object callbackData = null,
    AssetLoadingOptions options = AssetLoadingOptions.None)
  {
    ++this.m_preloadsNeeded;
    AssetLoader.Get().InstantiatePrefab(assetRef, (PrefabCallback<GameObject>) ((loadedAssetRef, loadedGameObject, loadedCallbackData) =>
    {
      --this.m_preloadsNeeded;
      callback(loadedAssetRef, loadedGameObject, loadedCallbackData);
    }), callbackData, options);
  }

  private void OnSoundLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    --this.m_preloadsNeeded;
    if (assetRef == null)
      Debug.LogWarning((object) string.Format("GameEntity.OnSoundLoaded() - ERROR missing Asset Ref for sound!", (object) assetRef));
    else if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("GameEntity.OnSoundLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    }
    else
    {
      AudioSource component = go.GetComponent<AudioSource>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        Debug.LogWarning((object) string.Format("GameEntity.OnSoundLoaded() - ERROR \"{0}\" has no Spell component", (object) assetRef));
      else
        this.m_preloadedSounds.Add(assetRef.ToString(), component);
    }
  }

  public void RemovePreloadedSound(string soundPath) => this.m_preloadedSounds.Remove(soundPath);

  public bool CheckPreloadedSound(string soundPath) => this.m_preloadedSounds.TryGetValue(soundPath, out AudioSource _);

  public AudioSource GetPreloadedSound(string soundPath)
  {
    AudioSource preloadedSound;
    if (this.m_preloadedSounds.TryGetValue(soundPath, out preloadedSound))
      return preloadedSound;
    Debug.LogError((object) string.Format("GameEntity.GetPreloadedSound() - \"{0}\" was not preloaded", (object) soundPath));
    return (AudioSource) null;
  }

  public bool IsPreloadingAssets() => this.m_preloadsNeeded > 0;

  public GameEntityOptions GetGameOptions() => this.m_gameOptions;

  public override bool HasValidDisplayName() => false;

  public override string GetName() => nameof (GameEntity);

  public override string GetDebugName() => nameof (GameEntity);

  public override void OnTagsChanged(TagDeltaList changeList, bool fromShowEntity)
  {
    for (int index = 0; index < changeList.Count; ++index)
      this.OnTagChanged(changeList[index]);
  }

  public override void InitRealTimeValues(List<Network.Entity.Tag> tags)
  {
    base.InitRealTimeValues(tags);
    foreach (Network.Entity.Tag tag in tags)
    {
      switch ((GAME_TAG) tag.Name)
      {
        case GAME_TAG.STEP:
          this.SetRealTimeStep(tag.Value);
          continue;
        case GAME_TAG.TURN:
          this.SetRealTimeTurn(tag.Value);
          GameState.Get().TriggerTurnTimerUpdateForTurn(tag.Value);
          continue;
        case GAME_TAG.COIN_MANA_GEM:
          if (tag.Value != 0)
          {
            ManaCrystalMgr.Get().SetManaCrystalType(ManaCrystalType.COIN);
            continue;
          }
          continue;
        case GAME_TAG.BOARD_VISUAL_STATE:
          if (tag.Value > 0)
          {
            Board.Get().ChangeBoardVisualState((TAG_BOARD_VISUAL_STATE) tag.Value);
            continue;
          }
          continue;
        default:
          continue;
      }
    }
  }

  public override void OnRealTimeTagChanged(Network.HistTagChange change)
  {
    switch ((GAME_TAG) change.Tag)
    {
      case GAME_TAG.MISSION_EVENT:
        this.HandleRealTimeMissionEvent(change.Value);
        break;
      case GAME_TAG.STEP:
        this.SetRealTimeStep(change.Value);
        break;
      case GAME_TAG.TURN:
        this.SetRealTimeTurn(change.Value);
        EndTurnButton.Get().OnTurnChanged();
        GameState.Get().UpdateOptionHighlights();
        break;
      case GAME_TAG.COIN_MANA_GEM:
        if (change.Value == 0)
          break;
        ManaCrystalMgr.Get().SetManaCrystalType(ManaCrystalType.COIN);
        break;
    }
  }

  public override void OnTagChanged(TagDelta change)
  {
    base.OnTagChanged(change);
    switch ((GAME_TAG) change.tag)
    {
      case GAME_TAG.TURN:
        EndTurnButton.Get().OnTurnChanged();
        GameState.Get().UpdateOptionHighlights();
        break;
      case GAME_TAG.END_TURN_BUTTON_ALTERNATIVE_APPEARANCE:
        EndTurnButton.Get().ApplyAlternativeAppearance();
        break;
      case GAME_TAG.TURN_INDICATOR_ALTERNATIVE_APPEARANCE:
        TurnStartManager.Get().ApplyAlternativeAppearance();
        break;
      case GAME_TAG.BOARD_VISUAL_STATE:
        Board.Get().ChangeBoardVisualState((TAG_BOARD_VISUAL_STATE) change.newValue);
        break;
      case GAME_TAG.BACON_CHOSEN_BOARD_SKIN_ID:
        BaconBoard.Get()?.OnBoardSkinChosen(change.newValue);
        break;
    }
  }

  private void SetRealTimeTurn(int turn) => this.m_realTimeTurn = turn;

  private void SetRealTimeStep(int step) => this.m_realTimeStep = step;

  public bool IsCurrentTurnRealTime() => this.m_realTimeTurn == this.GetTag(GAME_TAG.TURN);

  public bool IsMulliganActiveRealTime() => this.m_realTimeStep <= 4;

  public virtual void PreloadAssets()
  {
  }

  public virtual void NotifyOfStartOfTurnEventsFinished()
  {
  }

  public virtual bool NotifyOfEndTurnButtonPushed() => true;

  public virtual bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode) => true;

  public virtual void NotifyOfCardMousedOver(Entity mousedOverEntity)
  {
  }

  public virtual void NotifyOfCardMousedOff(Entity mousedOffEntity)
  {
  }

  public virtual bool NotifyOfCardTooltipDisplayShow(Card card) => true;

  public virtual void NotifyOfCardTooltipDisplayHide(Card card)
  {
  }

  public virtual void NotifyOfCardTooltipBigCardActorShow()
  {
  }

  public virtual void NotifyOfCoinFlipResult()
  {
  }

  public virtual bool NotifyOfPlayError(
    PlayErrors.ErrorType error,
    int? errorParam,
    Entity errorSource)
  {
    return false;
  }

  public virtual string[] NotifyOfKeywordHelpPanelDisplay(Entity entity) => (string[]) null;

  public virtual List<TooltipPanelManager.TooltipPanelData> GetOverwriteKeywordHelpPanelDisplay(
    Entity entity)
  {
    return (List<TooltipPanelManager.TooltipPanelData>) null;
  }

  public virtual bool GetEntityBaseForKeywordTooltips(
    Entity source,
    bool isHistoryTile,
    out EntityBase entityBaseForTooltips,
    out List<EntityBase> additionalEntityBaseForTooltips)
  {
    entityBaseForTooltips = (EntityBase) null;
    additionalEntityBaseForTooltips = (List<EntityBase>) null;
    return false;
  }

  public virtual bool SuppressMousedOverCardTooltip(out bool resetTimer)
  {
    resetTimer = false;
    return false;
  }

  public virtual void NotifyOfCardGrabbed(Entity entity)
  {
  }

  public virtual void NotifyOfCardDropped(Entity entity)
  {
  }

  public virtual void NotifyOfTargetModeCancelled()
  {
  }

  public virtual void NotifyOfHelpPanelDisplay(int numPanels)
  {
  }

  public virtual void NotifyOfDebugCommand(int command)
  {
  }

  public virtual void NotifyOfManaCrystalSpawned()
  {
  }

  public virtual void NotifyOfEnemyManaCrystalSpawned()
  {
  }

  public virtual void NotifyOfTooltipZoneMouseOver(TooltipZone tooltip)
  {
  }

  public virtual void NotifyOfHistoryTokenMousedOver(GameObject mousedOverTile)
  {
  }

  public virtual void NotifyOfHistoryTokenMousedOut()
  {
  }

  public virtual void NotifyOfCustomIntroFinished()
  {
  }

  public virtual void NotifyOfGameOver(TAG_PLAYSTATE playState)
  {
    PegCursor.Get().SetMode(PegCursor.Mode.STOPWAITING);
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_EndGameScreen);
    Card heroCard1 = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
    Card heroCard2 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
    Gameplay.Get().SaveOriginalTimeScale();
    AchievementManager.Get()?.PauseToastNotifications();
    Spell enemyBlowUpSpell = (Spell) null;
    Spell friendlyBlowUpSpell = (Spell) null;
    if (this.ShouldPlayHeroBlowUpSpells(playState))
    {
      switch (playState)
      {
        case TAG_PLAYSTATE.WON:
          string stringOption1 = this.GetGameOptions().GetStringOption(GameEntityOption.VICTORY_AUDIO_PATH);
          if (!string.IsNullOrEmpty(stringOption1))
            SoundManager.Get().LoadAndPlay((AssetReference) stringOption1);
          enemyBlowUpSpell = this.BlowUpHero(heroCard1, SpellType.ENDGAME_WIN);
          break;
        case TAG_PLAYSTATE.LOST:
          string stringOption2 = this.GetGameOptions().GetStringOption(GameEntityOption.DEFEAT_AUDIO_PATH);
          if (!string.IsNullOrEmpty(stringOption2))
            SoundManager.Get().LoadAndPlay((AssetReference) stringOption2);
          friendlyBlowUpSpell = this.BlowUpHero(heroCard2, SpellType.ENDGAME_LOSE);
          break;
        case TAG_PLAYSTATE.TIED:
          string stringOption3 = this.GetGameOptions().GetStringOption(GameEntityOption.DEFEAT_AUDIO_PATH);
          if (!string.IsNullOrEmpty(stringOption3))
            SoundManager.Get().LoadAndPlay((AssetReference) stringOption3);
          enemyBlowUpSpell = this.BlowUpHero(heroCard1, SpellType.ENDGAME_DRAW);
          friendlyBlowUpSpell = this.BlowUpHero(heroCard2, SpellType.ENDGAME_LOSE);
          break;
      }
    }
    this.ShowEndGameScreen(playState, enemyBlowUpSpell, friendlyBlowUpSpell);
  }

  public virtual void NotifyOfRealTimeTagChange(Entity entity, Network.HistTagChange tagChange)
  {
  }

  public virtual void ToggleAlternateMulliganActorHighlight(Card card, bool highlighted)
  {
  }

  public virtual bool ToggleAlternateMulliganActorHighlight(Actor actor, bool? highlighted = null) => false;

  public virtual bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => true;

  public virtual string GetVictoryScreenBannerText() => GameStrings.Get("GAMEPLAY_END_OF_GAME_VICTORY");

  public virtual string GetDefeatScreenBannerText() => GameStrings.Get("GAMEPLAY_END_OF_GAME_DEFEAT");

  public virtual string GetTieScreenBannerText() => GameStrings.Get("GAMEPLAY_END_OF_GAME_TIE");

  public virtual void NotifyOfHeroesFinishedAnimatingInMulligan()
  {
  }

  public virtual bool NotifyOfTooltipDisplay(TooltipZone tooltip) => false;

  public virtual void NotifyOfMulliganInitialized()
  {
    if (GameMgr.Get().IsTraditionalTutorial())
      return;
    if (GameMgr.Get().IsBattlegroundsMatchOrTutorial())
      AssetLoader.Get().InstantiatePrefab((AssetReference) "BattlegroundsEmoteHandler.prefab:212598c2e67d4b74c85d4913af706d9b", new PrefabCallback<GameObject>(this.EmoteHandlerDoneLoadingCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    else
      AssetLoader.Get().InstantiatePrefab((AssetReference) "EmoteHandler.prefab:5d44be0e8bb7fd14d9fbdbda6a74ab91", new PrefabCallback<GameObject>(this.EmoteHandlerDoneLoadingCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    if (GameMgr.Get().IsAI() && !GameUtils.IsMatchmadeGameType(GameMgr.Get().GetGameType()) || !this.GetGameOptions().GetBooleanOption(GameEntityOption.CAN_SQUELCH_OPPONENT))
      return;
    AssetLoader.Get().InstantiatePrefab((AssetReference) "EnemyEmoteHandler.prefab:6ace3edd8826cad4aaa0d0e0eb085012", new PrefabCallback<GameObject>(this.EnemyEmoteHandlerDoneLoadingCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  public virtual AudioSource GetAnnouncerLine(Card heroCard, Card.AnnouncerLineType type) => heroCard.GetAnnouncerLine(type);

  public virtual void NotifyOfMulliganEnded()
  {
  }

  private void EmoteHandlerDoneLoadingCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    go.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.FRIENDLY).transform.position;
  }

  private void EnemyEmoteHandlerDoneLoadingCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    go.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.OPPOSING).transform.position;
  }

  public virtual void NotifyOfGamePackOpened()
  {
  }

  public virtual void NotifyOfDefeatCoinAnimation()
  {
  }

  public virtual void SendCustomEvent(int eventID)
  {
  }

  public virtual string GetTurnStartReminderText() => "";

  public virtual bool IsHeroMulliganLobbyFinished() => true;

  public virtual ActorStateType GetMulliganChoiceHighlightState() => ActorStateType.CARD_IDLE;

  public virtual bool ShouldDelayShowingCardInTooltip() => true;

  public virtual Vector3 NameBannerPosition(Player.Side side) => side == Player.Side.FRIENDLY ? new Vector3(0.0f, 5f, 22f) : new Vector3(0.0f, 5f, -10f);

  public virtual void PlayAlternateEnemyEmote(
    int playerId,
    EmoteType emoteType,
    int battlegroundsEmoteId = 0)
  {
  }

  public virtual Vector3 GetMulliganTimerAlternatePosition() => Vector3.zero;

  private bool ShouldSkipMulligan() => this.HasTag(GAME_TAG.SKIP_MULLIGAN);

  public virtual bool ShouldDoAlternateMulliganIntro() => this.ShouldSkipMulligan();

  public virtual bool DoAlternateMulliganIntro()
  {
    if (!this.ShouldSkipMulligan())
      return false;
    GameEntity.Coroutines.StartCoroutine(this.SkipStandardMulliganWithTiming());
    return true;
  }

  protected IEnumerator SkipStandardMulliganWithTiming()
  {
    GameState.Get().SetMulliganBusy(true);
    SceneMgr.Get().NotifySceneLoaded();
    while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
      yield return (object) null;
    GameMgr.Get().UpdatePresence();
    MulliganManager.Get().SkipMulligan();
  }

  public virtual string GetMulliganDetailText() => (string) null;

  public virtual void OnMulliganCardsDealt(List<Card> startingCards)
  {
  }

  public virtual void OnMulliganBeginDealNewCards()
  {
  }

  public virtual float GetAdditionalTimeToWaitForSpells() => 0.0f;

  public virtual bool ShouldShowBigCard() => true;

  public virtual string GetBestNameForPlayer(int playerId)
  {
    string str = !GameState.Get().GetPlayerMap().ContainsKey(playerId) || GameState.Get().GetPlayerMap()[playerId] == null ? (string) null : GameState.Get().GetPlayerMap()[playerId].GetName();
    int num = !GameState.Get().GetPlayerMap().ContainsKey(playerId) ? 0 : (GameState.Get().GetPlayerMap()[playerId].IsFriendlySide() ? 1 : 0);
    bool flag = Options.Get().GetBool(Option.STREAMER_MODE);
    return num != 0 ? (flag || str == null ? GameStrings.Get("GAMEPLAY_HIDDEN_PLAYER_NAME") : str) : (flag || str == null ? GameStrings.Get("GAMEPLAY_MISSING_OPPONENT_NAME") : str);
  }

  public virtual List<RewardData> GetCustomRewards() => (List<RewardData>) null;

  public virtual void HandleRealTimeMissionEvent(int missionEvent)
  {
  }

  public virtual void OnPlayThinkEmote()
  {
    if (GameMgr.Get().IsAI())
      return;
    EmoteType emoteType = EmoteType.THINK1;
    switch (UnityEngine.Random.Range(1, 4))
    {
      case 1:
        emoteType = EmoteType.THINK1;
        break;
      case 2:
        emoteType = EmoteType.THINK2;
        break;
      case 3:
        emoteType = EmoteType.THINK3;
        break;
    }
    GameState.Get().GetCurrentPlayer().GetHeroCard()?.PlayEmote(emoteType);
  }

  public virtual IEnumerator OnPlayThinkEmoteWithTiming()
  {
    if (!GameMgr.Get().IsAI())
    {
      EmoteType emoteType = EmoteType.THINK1;
      switch (UnityEngine.Random.Range(1, 4))
      {
        case 1:
          emoteType = EmoteType.THINK1;
          break;
        case 2:
          emoteType = EmoteType.THINK2;
          break;
        case 3:
          emoteType = EmoteType.THINK3;
          break;
      }
      yield return (object) new WaitForSeconds(GameState.Get().GetCurrentPlayer().GetHeroCard().PlayEmote(emoteType).GetActiveAudioSource().clip.length);
    }
  }

  public virtual void OnEmotePlayed(Card card, EmoteType emoteType, CardSoundSpell emoteSpell)
  {
  }

  public virtual void NotifyOfOpponentWillPlayCard(string cardId, Entity playedEntity)
  {
  }

  public virtual void NotifyOfOpponentPlayedCard(Entity entity)
  {
  }

  public virtual void NotifyOfFriendlyPlayedCard(Entity entity)
  {
  }

  public virtual void NotifyOfResetGameStarted()
  {
  }

  public virtual void NotifyOfResetGameFinished(Entity source, Entity oldGameEntity)
  {
  }

  public virtual void NotifyOfEntityAttacked(Entity attacker, Entity defender)
  {
  }

  public virtual void NotifyOfMinionPlayed(Entity minion)
  {
  }

  public virtual void NotifyOfHeroChanged(Entity newHero)
  {
  }

  public virtual void NotifyOfWeaponEquipped(Entity weapon)
  {
  }

  public virtual void NotifyOfSpellPlayed(Entity spell, Entity target)
  {
  }

  public virtual void NotifyOfHeroPowerUsed(Entity heroPower, Entity target)
  {
  }

  public virtual void NotifyOfMinionDied(Entity minion)
  {
  }

  public virtual void NotifyOfHeroDied(Entity hero)
  {
  }

  public virtual void NotifyOfWeaponDestroyed(Entity weapon)
  {
  }

  public virtual string UpdateCardText(Card card, Actor bigCardActor, string text) => text;

  public virtual void ApplyMulliganActorStateChanges(Actor baseActor)
  {
  }

  public virtual void ApplyMulliganActorLobbyStateChanges(Actor baseActor)
  {
  }

  public virtual void ClearMulliganActorStateChanges(Actor baseActor)
  {
  }

  public virtual string GetMulliganBannerText() => GameStrings.Get("GAMEPLAY_MULLIGAN_STARTING_HAND");

  public virtual string GetMulliganBannerSubtitleText() => GameStrings.Get("GAMEPLAY_MULLIGAN_SUBTITLE");

  public virtual string GetMulliganWaitingText() => GameStrings.Get("GAMEPLAY_MULLIGAN_STARTING_HAND");

  public virtual string GetMulliganWaitingSubtitleText() => (string) null;

  public virtual Vector3 GetAlternateMulliganActorScale() => new Vector3(1f, 1f, 1f);

  public virtual int GetNumberOfFakeMulliganCardsToShowOnLeft(int numOriginalCards) => 0;

  public virtual int GetNumberOfFakeMulliganCardsToShowOnRight(int numOriginalCards) => 0;

  public virtual void ConfigureFakeMulliganCardActor(Actor actor, bool shown)
  {
  }

  public virtual Entity GetExtraMouseOverBigCardEntity(Entity source) => (Entity) null;

  public virtual bool ShowMouseOverBigCardImmediately(Entity mouseOverEntity) => false;

  public virtual bool ShouldSuppressCardMouseOver(Entity mouseOverEntity) => false;

  public virtual bool ShouldSuppressHistoryMouseOver() => false;

  public virtual bool ShouldSuppressOptionHighlight(Entity entity) => false;

  public virtual bool IsGameSpeedupConditionInEffect() => false;

  public virtual void StartMulliganSoundtracks(bool soft)
  {
    if (soft)
      MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_MulliganSoft);
    else
      MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_Mulligan);
  }

  public virtual void StartGameplaySoundtracks()
  {
    Board board = Board.Get();
    MusicPlaylistType type = MusicPlaylistType.InGame_Default;
    if (!((UnityEngine.Object) board == (UnityEngine.Object) null | !GameDownloadManagerProvider.Get().IsReadyAssetsInTags(new string[2]
    {
      DownloadTags.GetTagString(DownloadTags.Quality.MusicExpansion),
      DownloadTags.GetTagString(DownloadTags.Content.Base)
    })))
      type = board.m_BoardMusic;
    MusicManager.Get().StartPlaylist(type);
  }

  public virtual string GetAlternatePlayerName() => "";

  public virtual void QueueEntityForRemoval(Entity entity)
  {
  }

  public virtual IEnumerator PlayMissionIntroLineAndWait()
  {
    yield break;
  }

  public virtual IEnumerator DoActionsAfterIntroBeforeMulligan()
  {
    yield break;
  }

  public virtual IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    yield break;
  }

  public virtual IEnumerator DoActionsAfterDealingBaseMulliganCards()
  {
    yield break;
  }

  public virtual IEnumerator DoActionsBeforeCoinFlip()
  {
    yield break;
  }

  public virtual IEnumerator DoActionsAfterCoinFlip()
  {
    yield break;
  }

  public virtual IEnumerator DoActionsAfterDealingBonusCard()
  {
    yield break;
  }

  public virtual IEnumerator DoActionsBeforeSpreadingMulliganCards()
  {
    yield break;
  }

  public virtual IEnumerator DoActionsAfterSpreadingMulliganCards()
  {
    yield break;
  }

  public virtual IEnumerator DoGameSpecificPostIntroActions()
  {
    yield break;
  }

  public virtual IEnumerator DoCustomIntro(
    Card friendlyHero,
    Card enemyHero,
    HeroLabel friendlyHeroLabel,
    HeroLabel enemyHeroLabel,
    GameStartVsLetters versusText)
  {
    yield break;
  }

  public virtual void OnCustomIntroCancelled(
    Card friendlyHero,
    Card enemyHero,
    HeroLabel friendlyHeroLabel,
    HeroLabel enemyHeroLabel,
    GameStartVsLetters versusText)
  {
  }

  public virtual bool ShouldAllowCardGrab(Entity entity) => true;

  public virtual string CustomChoiceBannerText() => (string) null;

  public virtual InputManager.ZoneTooltipSettings GetZoneTooltipSettings() => new InputManager.ZoneTooltipSettings();

  protected virtual Spell BlowUpHero(Card card, SpellType spellType)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      return (Spell) null;
    Actor actor = card.GetActor();
    if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
      actor.ActivateAllSpellsDeathStates();
    Spell spell = this.ActivateSpellForDestroyedHero(card, spellType);
    Gameplay.Get().StartCoroutine(this.HideOtherElements(card));
    return spell;
  }

  protected virtual Spell ActivateSpellForDestroyedHero(Card card, SpellType spellType)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      return (Spell) null;
    if (spellType != SpellType.ENDGAME_LOSE)
      return card.ActivateActorSpell(spellType);
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    DeathSpellType tag = (DeathSpellType) friendlySidePlayer.GetTag(GAME_TAG.DEATH_SPELL_OVERRIDE);
    Spell spell = DeathSpellTable.Get().GetSpell(tag);
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return card.ActivateActorSpell(spellType);
    GameObject gameObject = friendlySidePlayer.GetHero().GetCard().GetActor().gameObject;
    spell.SetSource(gameObject);
    spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnFriendlyHeroDestroyed));
    this.m_destroyHeroTrackingCoroutine = spell.StartCoroutine(this.EnsureHeroDestroyedCompletes(spell));
    spell.Activate();
    return spell;
  }

  private void OnFriendlyHeroDestroyed(Spell spell, object _)
  {
    if (this.m_destroyHeroTrackingCoroutine == null)
      return;
    spell.StopCoroutine(this.m_destroyHeroTrackingCoroutine);
    this.m_destroyHeroTrackingCoroutine = (Coroutine) null;
  }

  private IEnumerator EnsureHeroDestroyedCompletes(Spell spell)
  {
    yield return (object) this.MAX_DESTROY_HERO_TIME;
    this.m_destroyHeroTrackingCoroutine = (Coroutine) null;
    Log.Spells.PrintError("Destroy hero spell " + spell.gameObject.name + " did not terminate and was killed to prevent game hang. Run the finisher in the authoring scene to diagnose potential problems.");
    spell.ReleaseSpell();
  }

  protected IEnumerator HideOtherElements(Card card)
  {
    yield return (object) new WaitForSeconds(0.5f);
    Player controller = card.GetEntity().GetController();
    if ((UnityEngine.Object) controller.GetHeroPowerCard() != (UnityEngine.Object) null)
    {
      controller.GetHeroPowerCard().HideCard();
      controller.GetHeroPowerCard().GetActor().ToggleForceIdle(true);
      controller.GetHeroPowerCard().GetActor().SetActorState(ActorStateType.CARD_IDLE);
      controller.GetHeroPowerCard().GetActor().DoCardDeathVisuals();
      controller.GetHeroPowerCard().DeactivateCustomKeywordEffect();
    }
    if ((UnityEngine.Object) controller.GetWeaponCard() != (UnityEngine.Object) null)
    {
      controller.GetWeaponCard().HideCard();
      controller.GetWeaponCard().GetActor().ToggleForceIdle(true);
      controller.GetWeaponCard().GetActor().SetActorState(ActorStateType.CARD_IDLE);
      controller.GetWeaponCard().GetActor().DoCardDeathVisuals();
    }
    card.GetActor().HideArmorSpell();
    GemObject healthObject = card.GetActor().GetHealthObject();
    if ((UnityEngine.Object) healthObject != (UnityEngine.Object) null)
      healthObject.Hide();
    GemObject attackObject = card.GetActor().GetAttackObject();
    if ((UnityEngine.Object) attackObject != (UnityEngine.Object) null)
      attackObject.Hide();
    card.GetActor().ToggleForceIdle(true);
    card.GetActor().SetActorState(ActorStateType.CARD_IDLE);
  }

  protected void ShowEndGameScreen(
    TAG_PLAYSTATE playState,
    Spell enemyBlowUpSpell,
    Spell friendlyBlowUpSpell)
  {
    string assetRef = (string) null;
    switch (playState)
    {
      case TAG_PLAYSTATE.WON:
        assetRef = this.GetGameOptions().GetStringOption(GameEntityOption.VICTORY_SCREEN_PREFAB_PATH);
        break;
      case TAG_PLAYSTATE.LOST:
      case TAG_PLAYSTATE.TIED:
        assetRef = this.GetGameOptions().GetStringOption(GameEntityOption.DEFEAT_SCREEN_PREFAB_PATH);
        break;
    }
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, AssetLoadingOptions.IgnorePrefabPosition);
    if (!(bool) (UnityEngine.Object) gameObject)
    {
      Debug.LogErrorFormat("GameEntity.ShowEndGameScreen() - FAILED to load \"{0}\"", (object) assetRef);
    }
    else
    {
      EndGameScreen component = gameObject.GetComponent<EndGameScreen>();
      if (!(bool) (UnityEngine.Object) component)
      {
        Debug.LogErrorFormat("GameEntity.ShowEndGameScreen() - \"{0}\" does not have an EndGameScreen component", (object) assetRef);
      }
      else
      {
        GameEntity.EndGameScreenContext gameScreenContext = new GameEntity.EndGameScreenContext();
        gameScreenContext.m_screen = component;
        gameScreenContext.m_enemyBlowUpSpell = enemyBlowUpSpell;
        gameScreenContext.m_friendlyBlowUpSpell = friendlyBlowUpSpell;
        gameScreenContext.m_endOfGameSpell = this.ActivateEndOfGameSpell();
        if ((bool) (UnityEngine.Object) enemyBlowUpSpell && !enemyBlowUpSpell.IsFinished())
          enemyBlowUpSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnBlowUpSpellFinished), (object) gameScreenContext);
        if ((bool) (UnityEngine.Object) friendlyBlowUpSpell && !friendlyBlowUpSpell.IsFinished())
          friendlyBlowUpSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnBlowUpSpellFinished), (object) gameScreenContext);
        if ((UnityEngine.Object) gameScreenContext.m_endOfGameSpell != (UnityEngine.Object) null && !gameScreenContext.m_endOfGameSpell.IsFinished())
          gameScreenContext.m_endOfGameSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnBlowUpSpellFinished), (object) gameScreenContext);
        this.ShowEndGameScreenAfterEffects(gameScreenContext);
      }
    }
  }

  public virtual bool ShouldShowHeroClassDuringMulligan(Player.Side playerSide) => true;

  public virtual bool ShouldUseAlternateNameForPlayer(Player.Side side) => false;

  public virtual string GetNameBannerOverride(Player.Side side) => (string) null;

  public virtual string GetNameBannerSubtextOverride(Player.Side playerSide) => (string) null;

  public virtual string GetTurnTimerCountdownText(float timeRemainingInTurn) => (string) null;

  public virtual string GetAttackSpellControllerOverride(Entity attacker) => (string) null;

  public virtual ZonePlay.PlayZoneSizeOverride GetPlayZoneSizeOverride() => (ZonePlay.PlayZoneSizeOverride) null;

  private void OnBlowUpSpellFinished(Spell spell, object userData) => this.ShowEndGameScreenAfterEffects((GameEntity.EndGameScreenContext) userData);

  private void ShowEndGameScreenAfterEffects(GameEntity.EndGameScreenContext context)
  {
    if (!this.AreBlowUpSpellsFinished(context))
      return;
    Gameplay.Get().RestoreOriginalTimeScale();
    AchievementManager.Get()?.UnpauseToastNotifications();
    context.m_screen.Show();
  }

  private bool AreBlowUpSpellsFinished(GameEntity.EndGameScreenContext context) => (!((UnityEngine.Object) context.m_enemyBlowUpSpell != (UnityEngine.Object) null) || context.m_enemyBlowUpSpell.IsFinished()) && (!((UnityEngine.Object) context.m_friendlyBlowUpSpell != (UnityEngine.Object) null) || context.m_friendlyBlowUpSpell.IsFinished()) && (!((UnityEngine.Object) context.m_endOfGameSpell != (UnityEngine.Object) null) || context.m_endOfGameSpell.IsFinished());

  public virtual float? GetThinkEmoteDelayOverride() => new float?();

  public virtual Notification.SpeechBubbleDirection GetEmoteDirectionOverride(
    EmoteType emoteType)
  {
    return Notification.SpeechBubbleDirection.None;
  }

  public virtual string[] GetOverrideBoardClickSounds() => (string[]) null;

  public virtual void OnTurnStartManagerFinished()
  {
  }

  public virtual void OnTurnTimerEnded(bool isFriendlyPlayerTurnTimer)
  {
  }

  public virtual bool GetAlternativeEndTurnButtonText(out string myTurnText, out string waitingText)
  {
    myTurnText = string.Empty;
    waitingText = string.Empty;
    return false;
  }

  public virtual bool ShouldOverwriteEndTurnButtonNoMorePlaysState(out bool hasNoMorePlay)
  {
    hasNoMorePlay = false;
    return false;
  }

  public virtual bool ShouldAutoCorrectZone(Zone zone) => true;

  public virtual bool OverwriteZoneDeckToAcceptEntity(
    ZoneDeck deckZone,
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    return false;
  }

  public virtual bool OverwriteEndTurnReminder(Entity entity, out bool showReminder)
  {
    showReminder = false;
    return false;
  }

  private Spell ActivateEndOfGameSpell()
  {
    string stringOption = this.GetGameOptions().GetStringOption(GameEntityOption.END_OF_GAME_SPELL_PREFAB_PATH);
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) stringOption, AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      return (Spell) null;
    this.m_endOfGameSpell = gameObject.GetComponent<Spell>();
    if ((UnityEngine.Object) this.m_endOfGameSpell == (UnityEngine.Object) null)
      return (Spell) null;
    this.m_endOfGameSpell.Activate();
    return this.m_endOfGameSpell;
  }

  public void ActivateEndOfGameSpellState(SpellStateType stateType)
  {
    if (!((UnityEngine.Object) this.m_endOfGameSpell != (UnityEngine.Object) null))
      return;
    this.m_endOfGameSpell.ActivateState(stateType);
  }

  public virtual bool OverwriteCurrentPlayer(Player player, out bool isCurrentPlayer)
  {
    isCurrentPlayer = false;
    return false;
  }

  public virtual bool Overwrite_IsInZone_ForInputManager(
    Entity entity,
    TAG_ZONE zoneTag,
    TAG_ZONE finalZoneTag,
    out bool isInZone)
  {
    isInZone = false;
    return false;
  }

  protected class EndGameScreenContext
  {
    public EndGameScreen m_screen;
    public Spell m_enemyBlowUpSpell;
    public Spell m_friendlyBlowUpSpell;
    public Spell m_endOfGameSpell;
  }
}
