using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyDisplay : MonoBehaviour
{
  public AsyncReference m_guestHeroTrayControllerReference;
  public AsyncReference m_guestHeroTrayControllerReference_phone;
  public GameObject m_deckPickerTrayContainer;
  public GameObject m_guestHeroPickerWidgetContainer;
  private static FriendlyDisplay s_instance;
  private AbsDeckPickerTrayDisplay m_deckPickerTray;

  private void Awake()
  {
    FriendlyDisplay.s_instance = this;
    this.InitHeroPicker();
  }

  private void OnDestroy()
  {
    FriendlyDisplay.s_instance = (FriendlyDisplay) null;
    this.m_deckPickerTray.RemovePlayButtonListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButtonPressed));
  }

  public static FriendlyDisplay Get() => FriendlyDisplay.s_instance;

  public void Unload() => this.m_deckPickerTray.RemovePlayButtonListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButtonPressed));

  private void OnTrayControllerReady(VisualController trayController)
  {
    if ((UnityEngine.Object) trayController == (UnityEngine.Object) null)
      Debug.LogError((object) "trayController was null in OnTrayControllerReady!");
    else
      this.OnDeckPickerTrayLoaded(trayController.Owner.transform.parent.gameObject);
  }

  private void OnDeckPickerTrayLoaded(GameObject go)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Unable to load DeckPickerTray.");
    }
    else
    {
      this.m_deckPickerTray = go.GetComponentInChildren<AbsDeckPickerTrayDisplay>();
      if ((UnityEngine.Object) this.m_deckPickerTray == (UnityEngine.Object) null)
        Debug.LogError((object) "AbsDeckPickerTrayDisplay component not found in AbsDeckPickerTray object.");
      else if ((UnityEngine.Object) this.m_deckPickerTrayContainer == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "deckPickerTrayContainer was not set in the prefab.");
      }
      else
      {
        GameUtils.SetParent(go, this.m_deckPickerTrayContainer);
        this.DisableOtherModeStuff();
        NetCache.Get().RegisterScreenFriendly((NetCache.NetCacheCallback) null);
        MusicManager.Get().StartPlaylist(FriendChallengeMgr.Get().IsChallengeTavernBrawl() ? MusicPlaylistType.UI_TavernBrawl : MusicPlaylistType.UI_Friendly);
        this.m_deckPickerTray.SetHeaderText(GameStrings.Get(FriendChallengeMgr.Get().IsChallengeTavernBrawl() ? "GLOBAL_TAVERN_BRAWL" : "GLOBAL_FRIEND_CHALLENGE_TITLE"));
        this.m_deckPickerTray.InitAssets();
        this.m_deckPickerTray.AddPlayButtonListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButtonPressed));
      }
    }
  }

  private void DisableOtherModeStuff()
  {
    if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY)
      return;
    Camera screenEffectsCamera = CameraUtils.FindFullScreenEffectsCamera(true);
    if (!((UnityEngine.Object) screenEffectsCamera != (UnityEngine.Object) null))
      return;
    screenEffectsCamera.GetComponent<FullScreenEffects>().Disable();
  }

  private void OnPlayButtonPressed(UIEvent uiEvent)
  {
    if (!((UnityEngine.Object) DeckPickerTrayDisplay.Get() != (UnityEngine.Object) null))
      return;
    CollectionDeck selectedCollectionDeck = DeckPickerTrayDisplay.Get().GetSelectedCollectionDeck();
    if (selectedCollectionDeck == null)
      return;
    Log.Decks.PrintInfo("Finding Friendly Game With Deck:");
    selectedCollectionDeck.LogDeckStringInformation();
  }

  private void InitHeroPicker()
  {
    int num = (UnityEngine.Object) GuestHeroPickerDisplay.Get() != (UnityEngine.Object) null ? 1 : ((UnityEngine.Object) HeroPickerDisplay.Get() != (UnityEngine.Object) null ? 1 : 0);
    int scenarioId = -1;
    if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
      scenarioId = TavernBrawlManager.Get().CurrentMission().missionId;
    if (num != 0)
    {
      Log.All.PrintWarning("Attempting to load HeroPickerDisplay a second time!");
    }
    else
    {
      List<ScenarioGuestHeroesDbfRecord> guestHeroesDbfRecordList = (List<ScenarioGuestHeroesDbfRecord>) null;
      if (scenarioId > 0)
        guestHeroesDbfRecordList = GameUtils.GetScenarioGuestHeroes(scenarioId);
      if (guestHeroesDbfRecordList != null && guestHeroesDbfRecordList.Count > 0)
      {
        this.m_guestHeroTrayControllerReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnTrayControllerReady));
        this.m_guestHeroTrayControllerReference_phone.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnTrayControllerReady));
        if (!((UnityEngine.Object) this.m_guestHeroPickerWidgetContainer != (UnityEngine.Object) null))
          return;
        this.m_guestHeroPickerWidgetContainer.SetActive(true);
      }
      else
      {
        AssetLoader.Get().InstantiatePrefab((AssetReference) ((bool) UniversalInputManager.UsePhoneUI ? "DeckPickerTray_phone.prefab:a30124f640b5b92459bf820a4e3b1ca7" : "DeckPickerTray.prefab:3e13b59cdca14074bbce2b7d903ed895"), (PrefabCallback<GameObject>) ((name, go, data) => this.OnDeckPickerTrayLoaded(go)), options: AssetLoadingOptions.IgnorePrefabPosition);
        if (!((UnityEngine.Object) this.m_guestHeroPickerWidgetContainer != (UnityEngine.Object) null) || !this.m_guestHeroPickerWidgetContainer.activeInHierarchy)
          return;
        this.m_guestHeroPickerWidgetContainer.SetActive(false);
        Log.All.PrintError("The guest hero picker was activated (loaded) in Friendly.prefab even though we aren't using guest heroes.");
      }
    }
  }
}
