using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassDisenchant : MonoBehaviour
{
  public GameObject m_root;
  public GameObject m_disenchantContainer;
  public MassDisenchantFX m_FX;
  public MassDisenchantSound m_sound;
  public UberText m_headlineText;
  public UberText m_detailsHeadlineText;
  public UberText m_detailsText;
  public UberText m_totalAmountText;
  public NormalButton m_disenchantButton;
  public UberText m_singleSubHeadlineText;
  public UberText m_doubleSubHeadlineText;
  public GameObject m_singleRoot;
  public GameObject m_doubleRoot;
  public List<DisenchantBar> m_singleDisenchantBars;
  public List<DisenchantBar> m_doubleDisenchantBars;
  public UIBButton m_infoButton;
  public Material m_rarityBarNormalMaterial;
  public Material m_rarityBarGoldMaterial;
  public Mesh m_rarityBarNormalMesh;
  public Mesh m_rarityBarGoldMesh;
  private bool m_useSingle = true;
  private int m_totalAmount;
  private int m_totalCardsToDisenchant;
  private Vector3 m_origTotalScale;
  private Vector3 m_origDustScale;
  private int m_highestGlowBalls;
  private List<GameObject> m_cleanupObjects = new List<GameObject>();
  private long m_preMassDisenchantDustValue;
  private IGraphicsManager m_graphicsManager;
  private static MassDisenchant s_Instance;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    MassDisenchant.s_Instance = this;
    this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    this.m_headlineText.Text = GameStrings.Get("GLUE_MASS_DISENCHANT_HEADLINE");
    this.m_detailsHeadlineText.Text = GameStrings.Get("GLUE_MASS_DISENCHANT_DETAILS_HEADLINE");
    this.m_disenchantButton.SetText(GameStrings.Get("GLUE_MASS_DISENCHANT_BUTTON_TEXT"));
    if ((UnityEngine.Object) this.m_detailsText != (UnityEngine.Object) null)
      this.m_detailsText.Text = GameStrings.Get("GLUE_MASS_DISENCHANT_DETAILS");
    if ((UnityEngine.Object) this.m_singleSubHeadlineText != (UnityEngine.Object) null)
      this.m_singleSubHeadlineText.Text = GameStrings.Get("GLUE_MASS_DISENCHANT_SUB_HEADLINE_TEXT");
    if ((UnityEngine.Object) this.m_doubleSubHeadlineText != (UnityEngine.Object) null)
      this.m_doubleSubHeadlineText.Text = GameStrings.Get("GLUE_MASS_DISENCHANT_SUB_HEADLINE_TEXT");
    this.m_disenchantButton.SetUserOverYOffset(-0.04409015f);
    foreach (DisenchantBar singleDisenchantBar in this.m_singleDisenchantBars)
      singleDisenchantBar.Init();
    foreach (DisenchantBar doubleDisenchantBar in this.m_doubleDisenchantBars)
      doubleDisenchantBar.Init();
    CollectionManager.Get().RegisterMassDisenchantListener(new CollectionManager.OnMassDisenchant(this.OnMassDisenchant));
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void Start()
  {
    this.m_disenchantButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDisenchantButtonPressed));
    this.m_disenchantButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnDisenchantButtonOver));
    this.m_disenchantButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnDisenchantButtonOut));
    if (!((UnityEngine.Object) this.m_infoButton != (UnityEngine.Object) null))
      return;
    this.m_infoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInfoButtonPressed));
  }

  public static MassDisenchant Get() => MassDisenchant.s_Instance;

  public void Show() => this.m_root.SetActive(true);

  public void Hide()
  {
    this.m_root.SetActive(false);
    this.BlockCurrencyFrame(false);
  }

  public bool IsShown() => this.m_root.activeSelf;

  private void OnDestroy()
  {
    foreach (GameObject cleanupObject in this.m_cleanupObjects)
    {
      if ((UnityEngine.Object) cleanupObject != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) cleanupObject);
    }
    CollectionManager.Get().RemoveMassDisenchantListener(new CollectionManager.OnMassDisenchant(this.OnMassDisenchant));
    this.BlockCurrencyFrame(false);
  }

  public int GetTotalAmount() => this.m_totalAmount;

  public void UpdateContents(List<CollectibleCard> disenchantCards)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_useSingle = true;
    }
    else
    {
      this.m_useSingle = true;
      foreach (CollectibleCard disenchantCard in disenchantCards)
      {
        if (disenchantCard.PremiumType == TAG_PREMIUM.GOLDEN)
        {
          this.m_useSingle = false;
          break;
        }
      }
    }
    List<DisenchantBar> disenchantBars = this.m_useSingle ? this.m_singleDisenchantBars : this.m_doubleDisenchantBars;
    foreach (DisenchantBar disenchantBar in disenchantBars)
      disenchantBar.Reset();
    CraftingManager craftingManager = CraftingManager.Get();
    DefLoader defLoader = DefLoader.Get();
    this.m_totalAmount = 0;
    this.m_totalCardsToDisenchant = 0;
    foreach (CollectibleCard disenchantCard in disenchantCards)
    {
      string cardId = disenchantCard.CardId;
      TAG_PREMIUM premiumType = disenchantCard.PremiumType;
      NetCache.CardValue cardValue = craftingManager.GetCardValue(cardId, premiumType);
      if (cardValue != null)
      {
        EntityDef entityDef = defLoader.GetEntityDef(cardId);
        int craftableDisenchantCount = disenchantCard.IsCraftableDisenchantCount;
        int sellAmount = cardValue.GetSellValue() * craftableDisenchantCount;
        DisenchantBar disenchantBar = this.FindDisenchantBar(disenchantBars, premiumType, entityDef.GetRarity());
        if (disenchantBar == null)
        {
          Debug.LogWarning((object) string.Format("MassDisenchant.UpdateContents(): Could not find {0} bar to modify for card {1} (premium {2}, disenchant count {3})", this.m_useSingle ? (object) "single" : (object) "double", (object) entityDef, (object) premiumType, (object) craftableDisenchantCount));
        }
        else
        {
          disenchantBar.AddCards(craftableDisenchantCount, sellAmount, premiumType);
          this.m_totalCardsToDisenchant += craftableDisenchantCount;
          this.m_totalAmount += sellAmount;
        }
      }
    }
    if (this.m_totalAmount > 0)
    {
      this.m_singleRoot.SetActive(this.m_useSingle);
      if ((UnityEngine.Object) this.m_doubleRoot != (UnityEngine.Object) null)
        this.m_doubleRoot.SetActive(!this.m_useSingle);
      this.m_disenchantButton.SetEnabled(true);
    }
    foreach (DisenchantBar disenchantBar in disenchantBars)
      disenchantBar.UpdateVisuals(this.m_totalCardsToDisenchant);
    this.m_totalAmountText.Text = GameStrings.Format("GLUE_MASS_DISENCHANT_TOTAL_AMOUNT", (object) this.m_totalAmount);
  }

  private DisenchantBar FindDisenchantBar(
    List<DisenchantBar> disenchantBars,
    TAG_PREMIUM premiumType,
    TAG_RARITY rarity)
  {
    int index = 0;
    for (int count = disenchantBars.Count; index < count; ++index)
    {
      DisenchantBar disenchantBar = disenchantBars[index];
      if ((disenchantBar.m_premiumType == premiumType || (bool) UniversalInputManager.UsePhoneUI) && disenchantBar.m_rarity == rarity)
        return disenchantBar;
    }
    return (DisenchantBar) null;
  }

  public IEnumerator StartHighlight()
  {
    yield return (object) null;
    this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
  }

  public void OnMassDisenchant(int amount)
  {
    int maxGlowBalls;
    switch (this.m_graphicsManager.RenderQualityLevel)
    {
      case GraphicsQuality.Low:
        maxGlowBalls = 3;
        break;
      case GraphicsQuality.Medium:
        maxGlowBalls = 6;
        break;
      default:
        maxGlowBalls = 10;
        break;
    }
    this.BlockUI();
    this.StartCoroutine(this.DoDisenchantAnims(maxGlowBalls, amount));
  }

  private void BlockCurrencyFrame(bool block)
  {
    BnetBar bnetBar = BnetBar.Get();
    if ((UnityEngine.Object) bnetBar == (UnityEngine.Object) null)
      return;
    bnetBar.SetBlockCurrencyFrames(block);
  }

  private void BlockUI(bool block = true)
  {
    this.BlockCurrencyFrame(block);
    this.m_FX.m_blockInteraction.SetActive(block);
  }

  private void OnDisenchantButtonOver(UIEvent e)
  {
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.MASS_DISENCHANT)
    {
      this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
      SoundManager.Get().LoadAndPlay((AssetReference) "Hub_Mouseover.prefab:40130da7b734190479c527d6bca1a4a8");
    }
    else
      this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
  }

  private void OnDisenchantButtonOut(UIEvent e)
  {
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.MASS_DISENCHANT)
      this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    else
      this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
  }

  private void OnDisenchantButtonPressed(UIEvent e)
  {
    Options.Get().SetBool(Option.HAS_DISENCHANTED, true);
    this.m_disenchantButton.SetEnabled(false);
    this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
    this.BlockCurrencyFrame(true);
    this.m_preMassDisenchantDustValue = NetCache.Get().GetArcaneDustBalance();
    Network.Get().MassDisenchant();
  }

  private void OnInfoButtonPressed(UIEvent e) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLUE_MASS_DISENCHANT_BUTTON_TEXT"),
    m_text = string.Format("{0}\n\n{1}", (object) GameStrings.Get("GLUE_MASS_DISENCHANT_DETAILS_HEADLINE"), (object) GameStrings.Get("GLUE_MASS_DISENCHANT_DETAILS")),
    m_showAlertIcon = false,
    m_responseDisplay = AlertPopup.ResponseDisplay.OK
  });

  private void Unbloomify(List<GameObject> glows, float newVal)
  {
    foreach (GameObject glow in glows)
      glow.GetComponent<RenderToTexture>().m_BloomIntensity = newVal;
  }

  private void UncolorTotal(float newVal) => this.m_totalAmountText.TextColor = Color.Lerp(Color.white, new Color(0.7f, 0.85f, 1f, 1f), newVal);

  private void SetGemSaturation(
    List<DisenchantBar> disenchantBars,
    float saturation,
    bool onlyActive = false,
    bool onlyInactive = false)
  {
    foreach (DisenchantBar disenchantBar in disenchantBars)
    {
      int numCards = disenchantBar.GetNumCards();
      if (onlyActive && numCards != 0 || onlyInactive && numCards == 0 || !onlyInactive && !onlyActive)
        RendererExtension.GetMaterial(disenchantBar.m_rarityGem.GetComponent<Renderer>()).SetColor("_Fade", new Color(saturation, saturation, saturation, 1f));
    }
  }

  private IEnumerator DoDisenchantAnims(int maxGlowBalls, int disenchantTotal)
  {
    MassDisenchant massDisenchant = this;
    if (disenchantTotal == 0)
      yield return (object) null;
    massDisenchant.m_origTotalScale = massDisenchant.m_totalAmountText.transform.localScale;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      massDisenchant.m_origDustScale = ArcaneDustAmount.Get().m_dustJar.transform.localScale;
    }
    else
    {
      CurrencyFrame currencyFrame;
      if (BnetBar.Get().TryGetRelevantCurrencyFrame(CurrencyType.DUST, out currencyFrame))
        massDisenchant.m_origDustScale = currencyFrame.CurrencyIconContainer.transform.localScale;
    }
    List<DisenchantBar> disenchantBars = massDisenchant.m_useSingle ? massDisenchant.m_singleDisenchantBars : massDisenchant.m_doubleDisenchantBars;
    float vigTime = 0.2f;
    ScreenEffectParameters parameters = new ScreenEffectParameters(ScreenEffectType.VIGNETTE, time: vigTime, vignette: new VignetteParameters?(new VignetteParameters(0.8f)));
    massDisenchant.m_screenEffectsHandle.StartEffect(parameters);
    iTween.ValueTo(massDisenchant.gameObject, iTween.Hash((object) "from", (object) 1f, (object) "to", (object) 0.3f, (object) "time", (object) vigTime, (object) "onupdate", (object) (Action<object>) (newVal => this.SetGemSaturation(disenchantBars, (float) newVal))));
    if (massDisenchant.m_sound.m_intro != string.Empty)
      SoundManager.Get().LoadAndPlay((AssetReference) massDisenchant.m_sound.m_intro);
    yield return (object) new WaitForSeconds(vigTime);
    float duration = 0.5f;
    float rate = duration / 20f;
    iTween.ValueTo(massDisenchant.gameObject, iTween.Hash((object) "from", (object) 0.3f, (object) "to", (object) 1.75f, (object) "time", (object) (float) (1.5 * (double) duration), (object) "easeInType", (object) iTween.EaseType.easeInCubic, (object) "onupdate", (object) (Action<object>) (newVal => this.SetGemSaturation(disenchantBars, (float) newVal, true))));
    List<GameObject> glows = new List<GameObject>();
    if ((UnityEngine.Object) massDisenchant.m_FX.m_glowTotal != (UnityEngine.Object) null)
      glows.Add(massDisenchant.m_FX.m_glowTotal);
    massDisenchant.m_totalAmountText.transform.localScale = massDisenchant.m_origTotalScale * 2.54f;
    iTween.ScaleTo(massDisenchant.m_totalAmountText.gameObject, iTween.Hash((object) "scale", (object) massDisenchant.m_origTotalScale, (object) "time", (object) 3.0));
    if ((UnityEngine.Object) massDisenchant.m_FX.m_glowTotal != (UnityEngine.Object) null)
      massDisenchant.m_FX.m_glowTotal.SetActive(true);
    massDisenchant.m_highestGlowBalls = 0;
    Color glowColor = new Color(0.7f, 0.85f, 1f, 1f);
    float origYSpeed = 0.0f;
    float origXSpeed = 0.0f;
    float origInten = 0.0f;
    foreach (DisenchantBar disenchantBar in disenchantBars)
    {
      int numCards = disenchantBar.GetNumCards();
      if (numCards > massDisenchant.m_highestGlowBalls)
        massDisenchant.m_highestGlowBalls = numCards;
    }
    massDisenchant.m_highestGlowBalls = massDisenchant.m_highestGlowBalls > maxGlowBalls ? maxGlowBalls : massDisenchant.m_highestGlowBalls;
    foreach (DisenchantBar bar in disenchantBars)
    {
      int numCards = bar.GetNumCards();
      if (numCards > 0)
      {
        RarityFX rarityFx = massDisenchant.GetRarityFX(bar);
        int totalGlowBalls = numCards > maxGlowBalls ? maxGlowBalls : numCards;
        for (int glowBallNum = 0; glowBallNum < totalGlowBalls; ++glowBallNum)
          massDisenchant.StartCoroutine(massDisenchant.LaunchGlowball(bar, rarityFx, glowBallNum, totalGlowBalls, massDisenchant.m_highestGlowBalls));
      }
    }
    for (int i = 0; (double) i < (double) duration / (double) rate; ++i)
    {
      float num = 0.0f;
      foreach (DisenchantBar bar in disenchantBars)
      {
        RaritySound raritySound = massDisenchant.GetRaritySound(bar);
        int numCards = bar.GetNumCards();
        if (i == 0 && numCards != 0)
        {
          if (raritySound.m_drainSound != string.Empty)
            SoundManager.Get().LoadAndPlay((AssetReference) raritySound.m_drainSound);
          if ((UnityEngine.Object) bar.m_numGoldText != (UnityEngine.Object) null && bar.m_numGoldText.gameObject.activeSelf)
          {
            bar.m_numGoldText.gameObject.SetActive(false);
            TransformUtil.SetLocalPosX((Component) bar.m_numCardsText, 2.902672f);
          }
          Vector3 localScale = bar.m_numCardsText.gameObject.transform.localScale;
          iTween.ScaleFrom(bar.m_numCardsText.gameObject, iTween.Hash((object) "x", (object) (float) ((double) localScale.x * 2.27999997138977), (object) "y", (object) (float) ((double) localScale.y * 2.27999997138977), (object) "z", (object) (float) ((double) localScale.z * 2.27999997138977), (object) "time", (object) 3.0));
          bar.m_numCardsText.TextColor = glowColor;
          iTween.ColorTo(bar.m_numCardsText.gameObject, iTween.Hash((object) "r", (object) 1f, (object) "g", (object) 1f, (object) "b", (object) 1f, (object) "time", (object) 3.0));
          if (massDisenchant.m_graphicsManager.RenderQualityLevel == GraphicsQuality.High && (UnityEngine.Object) bar.m_glow != (UnityEngine.Object) null)
          {
            glows.Add(bar.m_glow);
            bar.m_glow.GetComponent<RenderToTexture>().m_BloomIntensity = 0.01f;
            bar.m_glow.SetActive(true);
          }
          Material material = RendererExtension.GetMaterial(bar.m_rarityGem.GetComponent<Renderer>());
          origYSpeed = material.GetFloat("_YSpeed");
          origXSpeed = material.GetFloat("_XSpeed");
          origInten = RendererExtension.GetMaterial(bar.m_amountBar.GetComponent<Renderer>()).GetFloat("_Intensity");
          material.SetFloat("_YSpeed", -10f);
          material.SetFloat("_XSpeed", 20f);
        }
      }
      if (i == 0)
      {
        if (massDisenchant.m_graphicsManager.RenderQualityLevel == GraphicsQuality.High)
          iTween.ValueTo(massDisenchant.gameObject, iTween.Hash((object) "from", (object) 1f, (object) "to", (object) 0.1f, (object) "time", (object) (float) ((double) duration * 3.0), (object) "onupdate", (object) (Action<object>) (newVal => this.Unbloomify(glows, (float) newVal))));
        iTween.ValueTo(massDisenchant.gameObject, iTween.Hash((object) "from", (object) 1f, (object) "to", (object) 0.1f, (object) "time", (object) (float) ((double) duration * 3.0), (object) "onupdate", (object) (Action<object>) (newVal => this.UncolorTotal((float) newVal))));
        float disenchantDustValue = (float) massDisenchant.m_preMassDisenchantDustValue;
        iTween.ValueTo(massDisenchant.gameObject, iTween.Hash((object) "from", (object) disenchantDustValue, (object) "to", (object) (float) ((double) disenchantDustValue + (double) disenchantTotal), (object) "time", (object) (float) (3.0 * (double) duration), (object) "onupdate", (object) (Action<object>) (newVal => this.SetDustBalance((float) newVal)), (object) "oncomplete", (object) (Action<object>) (newVal =>
        {
          (CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as CollectionPageManager).UpdateMassDisenchant();
          this.m_screenEffectsHandle.StopEffect(vigTime);
          this.BlockUI(false);
        })));
      }
      foreach (DisenchantBar bar in disenchantBars)
      {
        if (bar.GetNumCards() != 0)
        {
          RendererExtension.GetMaterial(bar.m_amountBar.GetComponent<Renderer>()).SetFloat("_Intensity", 2f);
          num += massDisenchant.DrainBarAndDust(bar, i, duration, rate);
        }
      }
      massDisenchant.m_totalAmountText.Text = Convert.ToInt32(num).ToString();
      yield return (object) new WaitForSeconds(rate / duration);
    }
    if ((UnityEngine.Object) massDisenchant.m_FX.m_glowTotal != (UnityEngine.Object) null)
      massDisenchant.m_FX.m_glowTotal.SetActive(false);
    massDisenchant.m_totalAmountText.Text = "0";
    massDisenchant.m_totalAmountText.TextColor = Color.white;
    iTween.ValueTo(massDisenchant.gameObject, iTween.Hash((object) "from", (object) 0.3f, (object) "to", (object) 1f, (object) "time", (object) duration, (object) "delay", (object) vigTime, (object) "onupdate", (object) (Action<object>) (newVal => this.SetGemSaturation(disenchantBars, (float) newVal, onlyInactive: true))));
    iTween.ValueTo(massDisenchant.gameObject, iTween.Hash((object) "from", (object) 1.75f, (object) "to", (object) 1f, (object) "time", (object) duration, (object) "delay", (object) vigTime, (object) "onupdate", (object) (Action<object>) (newVal => this.SetGemSaturation(disenchantBars, (float) newVal, true))));
    foreach (DisenchantBar disenchantBar in disenchantBars)
    {
      if ((UnityEngine.Object) disenchantBar.m_glow != (UnityEngine.Object) null)
        disenchantBar.m_glow.SetActive(false);
      disenchantBar.m_numCardsText.TextColor = Color.white;
      Material material = RendererExtension.GetMaterial(disenchantBar.m_rarityGem.GetComponent<Renderer>());
      material.SetFloat("_YSpeed", origYSpeed);
      material.SetFloat("_XSpeed", origXSpeed);
      RendererExtension.GetMaterial(disenchantBar.m_amountBar.GetComponent<Renderer>()).SetFloat("_Intensity", origInten);
    }
  }

  private void SetDustBalance(float bal)
  {
    int balance = (int) bal;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      ArcaneDustAmount.Get().m_dustCount.Text = balance.ToString();
    }
    else
    {
      if (!((UnityEngine.Object) Shop.Get() != (UnityEngine.Object) null))
        return;
      Shop.Get().DisplayCurrencyBalance(CurrencyType.DUST, (long) balance);
    }
  }

  private float DrainBarAndDust(DisenchantBar bar, int drainRun, float duration, float rate)
  {
    float numCards = (float) bar.GetNumCards();
    float num1 = numCards - (float) ((double) (drainRun + 1) * (double) numCards / ((double) duration / (double) rate));
    if ((double) num1 < 0.0)
      num1 = 0.0f;
    float amountDust = (float) bar.GetAmountDust();
    float num2 = amountDust - (float) ((double) (drainRun + 1) * (double) amountDust / ((double) duration / (double) rate));
    if ((double) num2 < 0.0)
      num2 = 0.0f;
    UberText numCardsText = bar.m_numCardsText;
    int int32 = Convert.ToInt32(num1);
    string str1 = int32.ToString();
    numCardsText.Text = str1;
    UberText amountText = bar.m_amountText;
    int32 = Convert.ToInt32(num2);
    string str2 = int32.ToString();
    amountText.Text = str2;
    float percent = 0.0f;
    if (this.m_totalCardsToDisenchant > 0)
      percent = num1 / (float) this.m_totalCardsToDisenchant;
    bar.SetPercent(percent);
    return num2;
  }

  private Vector3 GetRanBoxPt(GameObject box)
  {
    Vector3 localScale = box.transform.localScale;
    return box.transform.position + new Vector3(UnityEngine.Random.Range((float) (-(double) localScale.x / 2.0), localScale.x / 2f), UnityEngine.Random.Range((float) (-(double) localScale.y / 2.0), localScale.y / 2f), UnityEngine.Random.Range((float) (-(double) localScale.z / 2.0), localScale.z / 2f));
  }

  private IEnumerator LaunchGlowball(
    DisenchantBar bar,
    RarityFX rareFX,
    int glowBallNum,
    int totalGlowBalls,
    int m_highestGlowBalls)
  {
    float num1 = 0.02f;
    float num2 = (float) glowBallNum;
    float num3 = (float) (1.0 - (double) num1 * (double) m_highestGlowBalls) / (float) totalGlowBalls;
    float min = (float) ((double) num2 * (double) num3 + (double) num2 * (double) num1);
    float max = (float) (((double) num2 + 1.0) * (double) num3 + ((double) num2 + 1.0) * (double) num1);
    GameObject glowBall = UnityEngine.Object.Instantiate<GameObject>(this.m_FX.m_glowBall);
    this.m_cleanupObjects.Add(glowBall);
    RendererExtension.SetSharedMaterial(glowBall.GetComponent<Renderer>(), rareFX.glowBallMat);
    RendererExtension.SetMaterial((Renderer) glowBall.GetComponent<TrailRenderer>(), rareFX.glowTrailMat);
    glowBall.transform.position = bar.m_rarityGem.transform.position;
    glowBall.transform.position = new Vector3(glowBall.transform.position.x, glowBall.transform.position.y + 0.5f, glowBall.transform.position.z);
    List<Vector3> curvePoints = new List<Vector3>();
    curvePoints.Add(glowBall.transform.position);
    if ((double) UnityEngine.Random.Range(0.0f, 1f) < 0.5)
    {
      curvePoints.Add(this.GetRanBoxPt(this.m_FX.m_gemBoxLeft1));
      curvePoints.Add(this.GetRanBoxPt(this.m_FX.m_gemBoxLeft2));
    }
    else
    {
      curvePoints.Add(this.GetRanBoxPt(this.m_FX.m_gemBoxRight1));
      curvePoints.Add(this.GetRanBoxPt(this.m_FX.m_gemBoxRight2));
    }
    GameObject dustJar = (GameObject) null;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      dustJar = ArcaneDustAmount.Get().m_dustJar;
      curvePoints.Add(dustJar.transform.position);
    }
    else
    {
      CurrencyFrame currencyFrame;
      if (BnetBar.Get().TryGetRelevantCurrencyFrame(CurrencyType.DUST, out currencyFrame))
      {
        dustJar = currencyFrame.CurrencyIconContainer;
        curvePoints.Add(Camera.main.ViewportToWorldPoint(BaseUI.Get().m_BnetCamera.WorldToViewportPoint(dustJar.transform.position)));
      }
    }
    yield return (object) new WaitForSeconds(UnityEngine.Random.Range(min, max));
    RaritySound rareSound = this.GetRaritySound(bar);
    if (rareSound.m_missileSound != string.Empty)
      SoundManager.Get().LoadAndPlay((AssetReference) rareSound.m_missileSound);
    if (glowBallNum == 0)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(rareFX.burstFX);
      this.m_cleanupObjects.Add(gameObject);
      gameObject.transform.position = bar.m_rarityGem.transform.position;
      gameObject.GetComponent<ParticleSystem>().Play();
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject, 3f);
    }
    float num4 = 0.4f;
    glowBall.SetActive(true);
    iTween.MoveTo(glowBall, iTween.Hash((object) "path", (object) curvePoints.ToArray(), (object) "time", (object) num4, (object) "easetype", (object) iTween.EaseType.linear));
    UnityEngine.Object.Destroy((UnityEngine.Object) glowBall, num4);
    yield return (object) new WaitForSeconds(num4);
    if (rareSound.m_jarSound != string.Empty)
      SoundManager.Get().LoadAndPlay((AssetReference) rareSound.m_jarSound);
    GameObject original = (GameObject) null;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      original = ArcaneDustAmount.Get().m_dustFX;
    }
    else
    {
      CurrencyFrame currencyFrame;
      if (BnetBar.Get().TryGetRelevantCurrencyFrame(CurrencyType.DUST, out currencyFrame))
        original = currencyFrame.m_dustFX;
    }
    if ((UnityEngine.Object) original != (UnityEngine.Object) null && (double) UnityEngine.Random.Range(0.0f, 1f) > 0.699999988079071)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original);
      this.m_cleanupObjects.Add(gameObject);
      gameObject.transform.parent = original.transform.parent;
      gameObject.transform.localPosition = original.transform.localPosition;
      gameObject.transform.localScale = original.transform.localScale;
      gameObject.transform.localRotation = original.transform.localRotation;
      gameObject.SetActive(true);
      gameObject.GetComponent<ParticleSystem>().Play();
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject, 4.9f);
    }
    if ((UnityEngine.Object) rareFX.explodeFX != (UnityEngine.Object) null)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(rareFX.explodeFX);
      this.m_cleanupObjects.Add(gameObject);
      gameObject.transform.parent = rareFX.explodeFX.transform.parent;
      gameObject.transform.localPosition = rareFX.explodeFX.transform.localPosition;
      gameObject.transform.localScale = rareFX.explodeFX.transform.localScale;
      gameObject.transform.localRotation = rareFX.explodeFX.transform.localRotation;
      gameObject.SetActive(true);
      gameObject.GetComponent<ParticleSystem>().Play();
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject, 3f);
    }
    if ((UnityEngine.Object) dustJar != (UnityEngine.Object) null)
    {
      Vector3 vector3 = Vector3.Min(dustJar.transform.localScale * 1.2f, this.m_origDustScale * 3f);
      iTween.ScaleTo(dustJar, iTween.Hash((object) "scale", (object) vector3, (object) "time", (object) 0.15f));
      iTween.ScaleTo(dustJar, iTween.Hash((object) "scale", (object) this.m_origDustScale, (object) "delay", (object) 0.1, (object) "time", (object) 1f));
    }
    yield return (object) null;
  }

  private RarityFX GetRarityFX(DisenchantBar bar)
  {
    RarityFX rarityFx = new RarityFX();
    switch (bar.m_rarity)
    {
      case TAG_RARITY.RARE:
        rarityFx.burstFX = this.m_FX.m_burstFX_Rare;
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          rarityFx.explodeFX = ArcaneDustAmount.Get().m_explodeFX_Rare;
        }
        else
        {
          CurrencyFrame currencyFrame;
          if (BnetBar.Get().TryGetRelevantCurrencyFrame(CurrencyType.DUST, out currencyFrame))
            rarityFx.explodeFX = currencyFrame.m_explodeFX_Rare;
        }
        rarityFx.glowBallMat = this.m_FX.m_glowBallMat_Rare;
        rarityFx.glowTrailMat = this.m_FX.m_glowTrailMat_Rare;
        break;
      case TAG_RARITY.EPIC:
        rarityFx.burstFX = this.m_FX.m_burstFX_Epic;
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          rarityFx.explodeFX = ArcaneDustAmount.Get().m_explodeFX_Epic;
        }
        else
        {
          CurrencyFrame currencyFrame;
          if (BnetBar.Get().TryGetRelevantCurrencyFrame(CurrencyType.DUST, out currencyFrame))
            rarityFx.explodeFX = currencyFrame.m_explodeFX_Epic;
        }
        rarityFx.glowBallMat = this.m_FX.m_glowBallMat_Epic;
        rarityFx.glowTrailMat = this.m_FX.m_glowTrailMat_Epic;
        break;
      case TAG_RARITY.LEGENDARY:
        rarityFx.burstFX = this.m_FX.m_burstFX_Legendary;
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          rarityFx.explodeFX = ArcaneDustAmount.Get().m_explodeFX_Legendary;
        }
        else
        {
          CurrencyFrame currencyFrame;
          if (BnetBar.Get().TryGetRelevantCurrencyFrame(CurrencyType.DUST, out currencyFrame))
            rarityFx.explodeFX = currencyFrame.m_explodeFX_Legendary;
        }
        rarityFx.glowBallMat = this.m_FX.m_glowBallMat_Legendary;
        rarityFx.glowTrailMat = this.m_FX.m_glowTrailMat_Legendary;
        break;
      default:
        rarityFx.burstFX = this.m_FX.m_burstFX_Common;
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          rarityFx.explodeFX = ArcaneDustAmount.Get().m_explodeFX_Legendary;
        }
        else
        {
          CurrencyFrame currencyFrame;
          if (BnetBar.Get().TryGetRelevantCurrencyFrame(CurrencyType.DUST, out currencyFrame))
            rarityFx.explodeFX = currencyFrame.m_explodeFX_Legendary;
        }
        rarityFx.glowBallMat = this.m_FX.m_glowBallMat_Common;
        rarityFx.glowTrailMat = this.m_FX.m_glowTrailMat_Common;
        break;
    }
    return rarityFx;
  }

  private RaritySound GetRaritySound(DisenchantBar bar)
  {
    RaritySound raritySound = new RaritySound();
    switch (bar.m_rarity)
    {
      case TAG_RARITY.RARE:
        raritySound.m_drainSound = this.m_sound.m_rare.m_drainSound;
        raritySound.m_jarSound = this.m_sound.m_rare.m_jarSound;
        raritySound.m_missileSound = this.m_sound.m_rare.m_missileSound;
        break;
      case TAG_RARITY.EPIC:
        raritySound.m_drainSound = this.m_sound.m_epic.m_drainSound;
        raritySound.m_jarSound = this.m_sound.m_epic.m_jarSound;
        raritySound.m_missileSound = this.m_sound.m_epic.m_missileSound;
        break;
      case TAG_RARITY.LEGENDARY:
        raritySound.m_drainSound = this.m_sound.m_legendary.m_drainSound;
        raritySound.m_jarSound = this.m_sound.m_legendary.m_jarSound;
        raritySound.m_missileSound = this.m_sound.m_legendary.m_missileSound;
        break;
      default:
        raritySound.m_drainSound = this.m_sound.m_common.m_drainSound;
        raritySound.m_jarSound = this.m_sound.m_common.m_jarSound;
        raritySound.m_missileSound = this.m_sound.m_common.m_missileSound;
        break;
    }
    return raritySound;
  }
}
