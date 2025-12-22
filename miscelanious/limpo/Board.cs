using Blizzard.T5.AssetManager;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class Board : MonoBehaviour
{
  private readonly string[] DEFAULT_BOARD_CLICK_SOUNDS = new string[5]
  {
    "board_common_dirt_poke_1.prefab:db7d81ea320f3bb4b9fa44bcd371d379",
    "board_common_dirt_poke_2.prefab:a078131beb0546444b4ccfc41ec5c547",
    "board_common_dirt_poke_3.prefab:7fbdaca211c05b94382e3142dfdbb306",
    "board_common_dirt_poke_4.prefab:d2713c07dcb56904da5ce08da04b5d26",
    "board_common_dirt_poke_5.prefab:c7234b85b15bca047b7ce32dc96bc851"
  };
  private const string GOLDEN_HERO_TRAY_FRIENDLY = "HeroTray_Golden_Friendly.prefab:53559bff3e3c2414d8ea4c731e363ff7";
  private const string GOLDEN_HERO_TRAY_OPPONENT = "HeroTray_Golden_Opponent.prefab:073fa61999554054e9cc93c518349e15";
  private readonly Color MULLIGAN_AMBIENT_LIGHT_COLOR = new Color(0.1607843f, 0.1921569f, 0.282353f, 1f);
  private const float MULLIGAN_LIGHT_INTENSITY = 0.0f;
  public Color m_AmbientColor = Color.white;
  public Light m_DirectionalLight;
  public float m_DirectionalLightIntensity = 0.275f;
  public GameObject m_FriendlyHeroTray;
  public GameObject m_OpponentHeroTray;
  public GameObject m_FriendlyHeroPhoneTray;
  public GameObject m_OpponentHeroPhoneTray;
  public Transform m_BoneParent;
  public GameObject m_SplitPlaySurface;
  public GameObject m_CombinedPlaySurface;
  public Transform m_ColliderParent;
  public GameObject m_MouseClickDustEffect;
  public Color m_ShadowColor = new Color(0.098f, 0.098f, 0.235f, 0.45f);
  public Color m_DeckColor = Color.white;
  public Color m_EndTurnButtonColor = Color.white;
  public Color m_HistoryTileColor = Color.white;
  public Color m_GoldenHeroTrayColor = Color.white;
  public List<PlayMakerFSM> m_BoardStateChangingObjects;
  public Spell m_leaderboardDamageCapFX;
  public List<Board.BoardSpecialEvents> m_SpecialEvents;
  public MusicPlaylistType m_BoardMusic = MusicPlaylistType.InGame_Default;
  public Texture m_GemManaPhoneTexture;
  private static Board s_instance;
  private bool m_raisedLights;
  private Spell m_FriendlyTraySpellEffect;
  private Spell m_OpponentTraySpellEffect;
  private int m_boardDbId;
  private Color m_TrayTint = Color.white;
  private AssetHandle<Texture> m_friendlyHeroTrayTexture;
  private AssetHandle<Texture> m_friendlyHeroPhoneTrayTexture;
  private AssetHandle<Texture> m_opponentHeroTrayTexture;
  private AssetHandle<Texture> m_opponentHeroPhoneTrayTexture;
  private Pool<GameObject> m_pooledDustEffects;
  private Dictionary<int, ParticleSystem[]> m_cachedParticleSystems;
  private const int MAX_DUST_VFX = 10;
  protected Board.AllAssetsLoadedCallback m_AllAssetsLoadedCallback;

  private void Awake()
  {
    Board.s_instance = this;
    LoadingScreen.Get()?.NotifyMainSceneObjectAwoke(this.gameObject);
    this.ValidateInspectorReferences();
    this.InitDustEffectsCache();
  }

  protected virtual void OnDestroy()
  {
    this.m_pooledDustEffects.ReleaseAll();
    this.m_pooledDustEffects.Clear();
    this.m_cachedParticleSystems.Clear();
    Board.s_instance = (Board) null;
    AssetHandle.SafeDispose<Texture>(ref this.m_friendlyHeroTrayTexture);
    AssetHandle.SafeDispose<Texture>(ref this.m_friendlyHeroPhoneTrayTexture);
    AssetHandle.SafeDispose<Texture>(ref this.m_opponentHeroTrayTexture);
    AssetHandle.SafeDispose<Texture>(ref this.m_opponentHeroPhoneTrayTexture);
  }

  public virtual void Start()
  {
    ProjectedShadow.SetShadowColor(this.m_ShadowColor);
    float[] numArray = new float[32];
    numArray[14] = 0.1f;
    this.m_DirectionalLight.layerShadowCullDistances = numArray;
    Animation component = this.GetComponent<Animation>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
    {
      string name = component.clip.name;
      component[name].normalizedTime = 0.25f;
      component[name].speed = -3f;
      component.Play(name);
    }
    this.StartCoroutine(this.GoldenHeroes());
    if (GameMgr.Get() == null || GameMgr.Get().IsTraditionalTutorial())
      return;
    foreach (Board.BoardSpecialEvents specialEvent in this.m_SpecialEvents)
    {
      if (SpecialEventManager.Get().IsEventActive(specialEvent.EventType, false))
        this.LoadBoardSpecialEvent(specialEvent);
    }
  }

  public static Board Get() => Board.s_instance;

  public void SetBoardDbId(int id) => this.m_boardDbId = id;

  public virtual bool AreAllAssetsLoaded() => true;

  public void RegisterAllAssetsLoadedCallback(Board.AllAssetsLoadedCallback callback) => this.m_AllAssetsLoadedCallback = callback;

  public void ResetAmbientColor() => RenderSettings.ambientLight = this.m_AmbientColor;

  [ContextMenu("RaiseTheLights")]
  public void RaiseTheLights() => this.RaiseTheLights(1f);

  public void RaiseTheLightsQuickly() => this.RaiseTheLights(5f);

  public void RaiseTheLights(float speed)
  {
    if (this.m_raisedLights)
      return;
    float num = 3f / speed;
    Action<object> action1 = (Action<object>) (amount => RenderSettings.ambientLight = (Color) amount);
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) RenderSettings.ambientLight, (object) "to", (object) this.m_AmbientColor, (object) "time", (object) num, (object) "easeType", (object) iTween.EaseType.easeInOutQuad, (object) "onupdate", (object) action1, (object) "onupdatetarget", (object) this.gameObject));
    Action<object> action2 = (Action<object>) (amount => this.m_DirectionalLight.intensity = (float) amount);
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) this.m_DirectionalLight.intensity, (object) "to", (object) this.m_DirectionalLightIntensity, (object) "time", (object) num, (object) "easeType", (object) iTween.EaseType.easeInOutQuad, (object) "onupdate", (object) action2, (object) "onupdatetarget", (object) this.gameObject));
    this.m_raisedLights = true;
  }

  public void SetMulliganLighting()
  {
    RenderSettings.ambientLight = this.MULLIGAN_AMBIENT_LIGHT_COLOR;
    this.m_DirectionalLight.intensity = 0.0f;
  }

  public void DimTheLights() => this.DimTheLights(5f);

  public void DimTheLights(float speed)
  {
    if (!this.m_raisedLights)
      return;
    float num = 3f / speed;
    Action<object> action1 = (Action<object>) (amount => RenderSettings.ambientLight = (Color) amount);
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) RenderSettings.ambientLight, (object) "to", (object) this.MULLIGAN_AMBIENT_LIGHT_COLOR, (object) "time", (object) num, (object) "easeType", (object) iTween.EaseType.easeInOutQuad, (object) "onupdate", (object) action1, (object) "onupdatetarget", (object) this.gameObject));
    Action<object> action2 = (Action<object>) (amount => this.m_DirectionalLight.intensity = (float) amount);
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) this.m_DirectionalLight.intensity, (object) "to", (object) 0.0f, (object) "time", (object) num, (object) "easeType", (object) iTween.EaseType.easeInOutQuad, (object) "onupdate", (object) action2, (object) "onupdatetarget", (object) this.gameObject));
    this.m_raisedLights = false;
  }

  public Transform FindBone(string name)
  {
    if ((UnityEngine.Object) this.m_BoneParent != (UnityEngine.Object) null)
    {
      Transform bone = this.m_BoneParent.Find(name);
      if ((UnityEngine.Object) bone != (UnityEngine.Object) null)
        return bone;
    }
    return Gameplay.Get().GetBoardLayout().FindBone(name);
  }

  public Collider FindCollider(string name)
  {
    if ((UnityEngine.Object) this.m_ColliderParent != (UnityEngine.Object) null)
    {
      Transform transform = this.m_ColliderParent.Find(name);
      if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
        return !((UnityEngine.Object) transform == (UnityEngine.Object) null) ? transform.GetComponent<Collider>() : (Collider) null;
    }
    return Gameplay.Get().GetBoardLayout().FindCollider(name);
  }

  public GameObject GetMouseClickDustEffectPrefab() => this.m_MouseClickDustEffect;

  public void CombinedSurface()
  {
    if (!((UnityEngine.Object) this.m_CombinedPlaySurface != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_SplitPlaySurface != (UnityEngine.Object) null))
      return;
    this.m_CombinedPlaySurface.SetActive(true);
    this.m_SplitPlaySurface.SetActive(false);
  }

  public void SplitSurface()
  {
    if (!((UnityEngine.Object) this.m_CombinedPlaySurface != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_SplitPlaySurface != (UnityEngine.Object) null))
      return;
    this.m_CombinedPlaySurface.SetActive(false);
    this.m_SplitPlaySurface.SetActive(true);
  }

  public Spell GetFriendlyTraySpell() => this.m_FriendlyTraySpellEffect;

  public Spell GetOpponentTraySpell() => this.m_OpponentTraySpellEffect;

  public virtual void ChangeBoardVisualState(TAG_BOARD_VISUAL_STATE boardState)
  {
    if (this.m_BoardStateChangingObjects == null || this.m_BoardStateChangingObjects.Count == 0)
      return;
    foreach (PlayMakerFSM stateChangingObject in this.m_BoardStateChangingObjects)
      stateChangingObject.SetState(EnumUtils.GetString<TAG_BOARD_VISUAL_STATE>(boardState));
  }

  public void ReturnDisabledDustVFX(GameObject dustVFX) => this.m_pooledDustEffects.Release(dustVFX);

  public void BoardClicked(RaycastHit hitInfo)
  {
    if ((UnityEngine.Object) this.m_MouseClickDustEffect == (UnityEngine.Object) null)
      return;
    GameState gameState = GameState.Get();
    if (gameState == null || gameState.IsMulliganManagerActive())
      return;
    GameObject parent = this.m_pooledDustEffects.Acquire();
    if ((UnityEngine.Object) parent == (UnityEngine.Object) null)
      return;
    parent.transform.position = hitInfo.point;
    ParticleSystem[] particleSystemArray;
    if (!this.m_cachedParticleSystems.TryGetValue(parent.GetInstanceID(), out particleSystemArray))
      return;
    Vector3 euler = new Vector3(Input.GetAxis("Mouse Y") * 40f, Input.GetAxis("Mouse X") * 40f, 0.0f);
    int index = 0;
    for (int length = particleSystemArray.Length; index < length; ++index)
    {
      ParticleSystem particleSystem = particleSystemArray[index];
      if (particleSystem.name == "Rocks")
        particleSystem.transform.localRotation = Quaternion.Euler(euler);
      particleSystem.Play();
    }
    string[] strArray = (string[]) null;
    GameEntity gameEntity = gameState.GetGameEntity();
    if (gameEntity != null)
      strArray = gameEntity.GetOverrideBoardClickSounds();
    if (strArray == null || strArray.Length == 0)
      strArray = this.DEFAULT_BOARD_CLICK_SOUNDS;
    string assetRef = strArray[UnityEngine.Random.Range(0, strArray.Length)];
    SoundManager.Get().LoadAndPlay((AssetReference) assetRef, parent);
  }

  protected virtual void ValidateInspectorReferences()
  {
    if ((UnityEngine.Object) this.m_FriendlyHeroTray == (UnityEngine.Object) null)
      Debug.LogError((object) "Friendly Hero Tray is not assigned!");
    if (!((UnityEngine.Object) this.m_OpponentHeroTray == (UnityEngine.Object) null))
      return;
    Debug.LogError((object) "Opponent Hero Tray is not assigned!");
  }

  private void InitDustEffectsCache()
  {
    this.m_pooledDustEffects = new Pool<GameObject>();
    this.m_pooledDustEffects.SetCreateItemCallback(new Pool<GameObject>.CreateItemCallback(this.CreateDustEffect));
    this.m_pooledDustEffects.SetDestroyItemCallback(new Pool<GameObject>.DestroyItemCallback(this.DestroyDustEffect));
    this.m_pooledDustEffects.SetExtensionCount(0);
    this.m_pooledDustEffects.SetMaxReleasedItemCount(10);
    this.m_cachedParticleSystems = new Dictionary<int, ParticleSystem[]>();
    this.m_pooledDustEffects.AddFreeItems(10);
  }

  private GameObject CreateDustEffect(int i)
  {
    GameObject dustEffect = UnityEngine.Object.Instantiate<GameObject>(this.m_MouseClickDustEffect);
    this.m_cachedParticleSystems.Add(dustEffect.GetInstanceID(), dustEffect.GetComponentsInChildren<ParticleSystem>());
    return dustEffect;
  }

  private void DestroyDustEffect(GameObject dustEffect)
  {
    if (!((UnityEngine.Object) dustEffect != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) dustEffect);
  }

  private IEnumerator GoldenHeroes()
  {
    Board board = this;
    bool friendlyHeroIsGolden = false;
    bool opposingHeroIsGolden = false;
    GameState gameState = GameState.Get();
    while (gameState == null)
    {
      gameState = GameState.Get();
      yield return (object) null;
    }
    Player friendlyPlayer = gameState.GetFriendlySidePlayer();
    while (friendlyPlayer == null)
    {
      friendlyPlayer = gameState.GetFriendlySidePlayer();
      yield return (object) null;
    }
    Player opposingPlayer = gameState.GetOpposingSidePlayer();
    Card friendlyHeroCard = friendlyPlayer.GetHeroCard();
    while ((UnityEngine.Object) friendlyHeroCard == (UnityEngine.Object) null)
    {
      friendlyHeroCard = friendlyPlayer.GetHeroCard();
      yield return (object) null;
    }
    Card opposingHeroCard = opposingPlayer.GetHeroCard();
    while ((UnityEngine.Object) opposingHeroCard == (UnityEngine.Object) null)
    {
      opposingHeroCard = opposingPlayer.GetHeroCard();
      yield return (object) null;
    }
    while (friendlyHeroCard.GetEntity() == null)
      yield return (object) null;
    while (opposingHeroCard.GetEntity() == null)
      yield return (object) null;
    if (friendlyHeroCard.GetPremium() == TAG_PREMIUM.GOLDEN)
      friendlyHeroIsGolden = true;
    if (opposingHeroCard.GetPremium() == TAG_PREMIUM.GOLDEN)
      opposingHeroIsGolden = true;
    if (friendlyHeroIsGolden && !friendlyHeroCard.DisablePremiumHeroTray)
      AssetLoader.Get().InstantiatePrefab((AssetReference) "HeroTray_Golden_Friendly.prefab:53559bff3e3c2414d8ea4c731e363ff7", new PrefabCallback<GameObject>(board.ShowFriendlyHeroTray));
    else
      board.StartCoroutine(board.UpdateHeroTray(Player.Side.FRIENDLY, false));
    if (opposingHeroIsGolden && !opposingHeroCard.DisablePremiumHeroTray)
      AssetLoader.Get().InstantiatePrefab((AssetReference) "HeroTray_Golden_Opponent.prefab:073fa61999554054e9cc93c518349e15", new PrefabCallback<GameObject>(board.ShowOpponentHeroTray));
    else
      board.StartCoroutine(board.UpdateHeroTray(Player.Side.OPPOSING, false));
  }

  private void ShowFriendlyHeroTray(AssetReference assetRef, GameObject go, object callbackData)
  {
    go.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.FRIENDLY).OriginalPosition;
    go.SetActive(true);
    foreach (Renderer componentsInChild in go.GetComponentsInChildren<Renderer>())
      RendererExtension.GetMaterial(componentsInChild).color = this.m_GoldenHeroTrayColor;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_FriendlyHeroTray);
    this.m_FriendlyHeroTray = go;
    this.StartCoroutine(this.UpdateHeroTray(Player.Side.FRIENDLY, true));
  }

  private void ShowOpponentHeroTray(AssetReference assetRef, GameObject go, object callbackData)
  {
    go.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.OPPOSING).OriginalPosition;
    go.SetActive(true);
    foreach (Renderer componentsInChild in go.GetComponentsInChildren<Renderer>())
      RendererExtension.GetMaterial(componentsInChild).color = this.m_GoldenHeroTrayColor;
    if ((bool) (UnityEngine.Object) this.m_OpponentHeroTray)
    {
      this.m_OpponentHeroTray.SetActive(false);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_OpponentHeroTray);
    }
    this.m_OpponentHeroTray = go;
    this.StartCoroutine(this.UpdateHeroTray(Player.Side.OPPOSING, true));
  }

  private IEnumerator UpdateHeroTray(Player.Side side, bool isGolden)
  {
    Board board = this;
    while (GameState.Get().GetPlayerMap().Count == 0)
      yield return (object) null;
    Player p = (Player) null;
    while (p == null)
    {
      foreach (Player player in GameState.Get().GetPlayerMap().Values)
      {
        if (player.GetSide() == side)
        {
          p = player;
          break;
        }
      }
      yield return (object) null;
    }
    while (p.GetHero() == null)
      yield return (object) null;
    Entity hero = p.GetHero();
    while (hero.IsLoadingAssets())
      yield return (object) null;
    while ((UnityEngine.Object) hero.GetCard() == (UnityEngine.Object) null)
      yield return (object) null;
    Card heroCard = hero.GetCard();
    while (!heroCard.HasCardDef)
      yield return (object) null;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      while ((UnityEngine.Object) ManaCrystalMgr.Get() == (UnityEngine.Object) null)
        yield return (object) null;
      if (side == Player.Side.FRIENDLY)
      {
        if (!string.IsNullOrEmpty(heroCard.CustomHeroPhoneManaGem))
          AssetLoader.Get().LoadAsset<Texture>((AssetReference) heroCard.CustomHeroPhoneManaGem, new AssetHandleCallback<Texture>(board.OnHeroSkinManaGemTextureLoaded));
        else if ((UnityEngine.Object) board.m_GemManaPhoneTexture != (UnityEngine.Object) null)
          ManaCrystalMgr.Get().SetFriendlyManaGemTexture(new AssetHandle<Texture>(board.m_GemManaPhoneTexture.name, board.m_GemManaPhoneTexture));
      }
    }
    for (int index = 0; index < heroCard.CustomHeroTraySettings.Count; ++index)
    {
      if ((BoardDdId) board.m_boardDbId == heroCard.CustomHeroTraySettings[index].m_Board)
        board.m_TrayTint = heroCard.CustomHeroTraySettings[index].m_Tint;
    }
    if (!string.IsNullOrEmpty(heroCard.CustomHeroTray))
    {
      while ((UnityEngine.Object) heroCard.GetActor() == (UnityEngine.Object) null)
        yield return (object) null;
      if (heroCard.GetActor().GetPremium() == TAG_PREMIUM.GOLDEN && !string.IsNullOrEmpty(heroCard.CustomHeroTrayGolden))
        AssetLoader.Get().LoadAsset<Texture>((AssetReference) heroCard.CustomHeroTrayGolden, new AssetHandleCallback<Texture>(board.OnHeroTrayTextureLoaded), (object) side);
      else
        AssetLoader.Get().LoadAsset<Texture>((AssetReference) heroCard.CustomHeroTray, new AssetHandleCallback<Texture>(board.OnHeroTrayTextureLoaded), (object) side);
    }
    if ((bool) UniversalInputManager.UsePhoneUI && !string.IsNullOrEmpty(heroCard.CustomHeroPhoneTray))
      AssetLoader.Get().LoadAsset<Texture>((AssetReference) heroCard.CustomHeroPhoneTray, new AssetHandleCallback<Texture>(board.OnHeroPhoneTrayTextureLoaded), (object) side);
  }

  private void OnHeroSkinManaGemTextureLoaded(
    AssetReference assetRef,
    AssetHandle<Texture> texture,
    object callbackData)
  {
    using (texture)
    {
      if (!(bool) texture)
      {
        Debug.LogError((object) "OnHeroSkinManaGemTextureLoaded() loaded texture is null!");
      }
      else
      {
        ManaCrystalMgr.Get().SetFriendlyManaGemTexture(texture);
        ManaCrystalMgr.Get().SetFriendlyManaGemTint(this.m_TrayTint);
      }
    }
  }

  private void OnHeroTrayTextureLoaded(
    AssetReference assetRef,
    AssetHandle<Texture> texture,
    object callbackData)
  {
    using (texture)
    {
      if (!(bool) texture)
        Debug.LogError((object) "Board.OnHeroTrayTextureLoaded() loaded texture is null!");
      else if ((Player.Side) callbackData == Player.Side.FRIENDLY)
      {
        AssetHandle.Set<Texture>(ref this.m_friendlyHeroTrayTexture, texture);
        Material material = RendererExtension.GetMaterial((Renderer) this.m_FriendlyHeroTray.GetComponentInChildren<MeshRenderer>());
        material.mainTexture = (Texture) this.m_friendlyHeroTrayTexture;
        material.color = this.m_TrayTint;
      }
      else
      {
        AssetHandle.Set<Texture>(ref this.m_opponentHeroTrayTexture, texture);
        Material material = RendererExtension.GetMaterial((Renderer) this.m_OpponentHeroTray.GetComponentInChildren<MeshRenderer>());
        material.mainTexture = (Texture) this.m_opponentHeroTrayTexture;
        material.color = this.m_TrayTint;
      }
    }
  }

  private void OnHeroPhoneTrayTextureLoaded(
    AssetReference assetRef,
    AssetHandle<Texture> texture,
    object callbackData)
  {
    using (texture)
    {
      if (!(bool) texture)
        Debug.LogError((object) "Board.OnHeroTrayTextureLoaded() loaded texture is null!");
      else if ((Player.Side) callbackData == Player.Side.FRIENDLY)
      {
        if ((UnityEngine.Object) this.m_FriendlyHeroPhoneTray == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) "Friendly Hero Phone Tray Object on Board is null!");
        }
        else
        {
          AssetHandle.Set<Texture>(ref this.m_friendlyHeroPhoneTrayTexture, texture);
          Material material = RendererExtension.GetMaterial((Renderer) this.m_FriendlyHeroPhoneTray.GetComponentInChildren<MeshRenderer>());
          material.mainTexture = (Texture) this.m_friendlyHeroPhoneTrayTexture;
          material.color = this.m_TrayTint;
        }
      }
      else if ((UnityEngine.Object) this.m_OpponentHeroPhoneTray == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "Opponent Hero Phone Tray Object on Board is null!");
      }
      else
      {
        AssetHandle.Set<Texture>(ref this.m_opponentHeroPhoneTrayTexture, texture);
        Material material = RendererExtension.GetMaterial((Renderer) this.m_OpponentHeroPhoneTray.GetComponentInChildren<MeshRenderer>());
        material.mainTexture = (Texture) this.m_opponentHeroPhoneTrayTexture;
        material.color = this.m_TrayTint;
      }
    }
  }

  private void OnHeroTrayEffectLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Board.OnHeroTrayEffectLoaded() Hero tray effect is null!");
    }
    else
    {
      Spell component = go.GetComponent<Spell>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        Debug.LogError((object) "Board.OnHeroTrayEffectLoaded() Hero tray effect: could not find spell component!");
      else if ((Player.Side) callbackData == Player.Side.FRIENDLY)
      {
        go.transform.parent = this.transform;
        go.transform.position = this.FindBone("CustomSocketIn_Friendly").position;
        this.m_FriendlyTraySpellEffect = component;
      }
      else
      {
        go.transform.parent = this.transform;
        go.transform.position = this.FindBone("CustomSocketIn_Opposing").position;
        this.m_OpponentTraySpellEffect = component;
      }
    }
  }

  private void LoadBoardSpecialEvent(Board.BoardSpecialEvents boardSpecialEvent)
  {
    if ((UnityEngine.Object) AssetLoader.Get().InstantiatePrefab((AssetReference) boardSpecialEvent.Prefab) == (UnityEngine.Object) null)
      Debug.LogWarning((object) string.Format("Failed to load special board event: {0}", (object) boardSpecialEvent.Prefab));
    this.m_AmbientColor = boardSpecialEvent.AmbientColorOverride;
  }

  [Serializable]
  public class CustomTraySettings
  {
    public BoardDdId m_Board;
    public Color m_Tint = Color.white;
  }

  [Serializable]
  public class BoardSpecialEvents
  {
    public SpecialEventType EventType;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public string Prefab;
    public Color AmbientColorOverride = Color.white;
  }

  public delegate void AllAssetsLoadedCallback();
}
