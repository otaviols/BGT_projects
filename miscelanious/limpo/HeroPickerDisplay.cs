using UnityEngine;

public class HeroPickerDisplay : MonoBehaviour
{
  public GameObject m_deckPickerBone;
  private static readonly PlatformDependentValue<Vector3> HERO_PICKER_START_POSITION = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(-57.36467f, 2.4869f, -28.6f),
    Phone = new Vector3(-66.4f, 2.4869f, -28.6f)
  };
  private static readonly Vector3 HERO_PICKER_END_POSITION = new Vector3(40.6f, 2.4869f, -28.6f);
  private static HeroPickerDisplay s_instance;
  private DeckPickerTrayDisplay m_deckPickerTray;

  private void Awake()
  {
    this.transform.localPosition = (Vector3) HeroPickerDisplay.HERO_PICKER_START_POSITION;
    AssetLoader.Get().InstantiatePrefab((AssetReference) ((bool) UniversalInputManager.UsePhoneUI ? "DeckPickerTray_phone.prefab:a30124f640b5b92459bf820a4e3b1ca7" : "DeckPickerTray.prefab:3e13b59cdca14074bbce2b7d903ed895"), new PrefabCallback<GameObject>(this.DeckPickerTrayLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) HeroPickerDisplay.s_instance != (Object) null)
      Debug.LogWarning((object) "HeroPickerDisplay is supposed to be a singleton, but a second instance of it is being created!");
    HeroPickerDisplay.s_instance = this;
    SoundManager.Get().Load(SoundUtils.SquarePanelSlideOnSFX);
    SoundManager.Get().Load(SoundUtils.SquarePanelSlideOffSFX);
  }

  private void OnDestroy() => HeroPickerDisplay.s_instance = (HeroPickerDisplay) null;

  public static HeroPickerDisplay Get() => HeroPickerDisplay.s_instance;

  public bool IsShown() => this.transform.localPosition == HeroPickerDisplay.HERO_PICKER_END_POSITION;

  public bool IsHidden() => this.transform.localPosition == (Vector3) HeroPickerDisplay.HERO_PICKER_START_POSITION;

  public void ShowTray()
  {
    SoundManager.Get().LoadAndPlay(SoundUtils.SquarePanelSlideOnSFX);
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) HeroPickerDisplay.HERO_PICKER_END_POSITION, (object) "time", (object) 0.5f, (object) "isLocal", (object) true, (object) "easeType", (object) iTween.EaseType.easeOutBounce));
  }

  public void CheatLoadHeroButtons(int amount) => this.m_deckPickerTray.CheatLoadHeroButtons(amount);

  private void DeckPickerTrayLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    Options.SetFormatType(CollectionManager.s_HeroPickerFormat);
    this.m_deckPickerTray = go.GetComponent<DeckPickerTrayDisplay>();
    this.m_deckPickerTray.UpdateCreateDeckText();
    this.m_deckPickerTray.SetInHeroPicker();
    this.m_deckPickerTray.transform.parent = this.transform;
    this.m_deckPickerTray.transform.localScale = this.m_deckPickerBone.transform.localScale;
    this.m_deckPickerTray.transform.localPosition = this.m_deckPickerBone.transform.localPosition;
    this.m_deckPickerTray.InitAssets();
    this.ShowTray();
  }

  public void HideTray(float delay = 0.0f)
  {
    SoundManager.Get().LoadAndPlay(SoundUtils.SquarePanelSlideOffSFX);
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (Vector3) HeroPickerDisplay.HERO_PICKER_START_POSITION, (object) "time", (object) 0.5f, (object) "isLocal", (object) true, (object) "oncomplete", (object) "OnTrayHidden", (object) "oncompletetarget", (object) this.gameObject, (object) "easeType", (object) iTween.EaseType.easeInCubic, (object) nameof (delay), (object) delay));
  }

  private void OnTrayHidden()
  {
    this.m_deckPickerTray.Unload();
    Object.DestroyImmediate((Object) this.gameObject);
    if (!((Object) TavernBrawlDisplay.Get() != (Object) null))
      return;
    TavernBrawlDisplay.Get().EnablePlayButton();
    TavernBrawlDisplay.Get().EnableBackButton(true);
  }
}
