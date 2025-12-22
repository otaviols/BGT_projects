using System.Collections;
using UnityEngine;

[CustomEditClass]
public class BannerPopup : MonoBehaviour
{
  public GameObject m_root;
  public UberText m_header;
  public UberText m_text;
  public UIBButton m_dismissButton;
  public Spell m_ShowSpell;
  public Spell m_LoopingSpell;
  public Spell m_HideSpell;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_showSound;
  private BannerManager.DelOnCloseBanner m_onCloseBannerPopup;
  private PegUIElement m_inputBlocker;
  private bool m_showSpellComplete = true;
  private bool m_onCloseCallbackCalled;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    this.gameObject.SetActive(false);
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void Start()
  {
    if ((Object) this.m_ShowSpell == (Object) null)
    {
      this.OnShowSpellFinished((Spell) null, (object) null);
    }
    else
    {
      this.m_ShowSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnShowSpellFinished));
      this.m_ShowSpell.Activate();
    }
  }

  private void OnDestroy()
  {
    if (this.m_onCloseCallbackCalled)
      return;
    this.m_onCloseCallbackCalled = true;
    if (this.m_onCloseBannerPopup == null)
      return;
    this.m_onCloseBannerPopup();
  }

  public void Show(
    string headerText,
    string bannerText,
    BannerManager.DelOnCloseBanner onCloseCallback = null)
  {
    OverlayUI.Get().AddGameObject(this.gameObject);
    if ((Object) this.m_header != (Object) null && headerText != null)
      this.m_header.Text = headerText;
    if ((Object) this.m_text != (Object) null && bannerText != null)
      this.m_text.Text = bannerText;
    this.m_onCloseBannerPopup = onCloseCallback;
    this.gameObject.SetActive(true);
    Animation animation = (Object) this.m_root == (Object) null ? (Animation) null : this.m_root.GetComponent<Animation>();
    if ((Object) animation != (Object) null)
      animation.Play();
    if (!string.IsNullOrEmpty(this.m_showSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_showSound);
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.gameObject.layer), "ClosedSignInputBlocker", (Component) this);
    LayerUtils.SetLayer(inputBlocker, this.gameObject.layer);
    this.m_inputBlocker = inputBlocker.AddComponent<PegUIElement>();
    iTween.ScaleFrom(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) 0.25f, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "EnableClickHandler"));
    this.FadeEffectsIn();
    if ((Object) this.m_dismissButton != (Object) null)
      this.m_dismissButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CloseBannerPopup));
    this.m_showSpellComplete = false;
  }

  private void FadeEffectsIn() => this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);

  private void FadeEffectsOut() => this.m_screenEffectsHandle.StopEffect();

  private void CloseBannerPopup(UIEvent e)
  {
    this.m_inputBlocker.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CloseBannerPopup));
    this.Close();
  }

  public void Close()
  {
    this.FadeEffectsOut();
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) Vector3.zero, (object) "time", (object) 0.5f, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "DestroyBannerPopup"));
    SoundManager.Get().LoadAndPlay((AssetReference) "new_quest_click_and_shrink.prefab:601ba6676276eab43947e38f110f7b99");
    ParticleSystem[] componentsInChildren = this.gameObject.GetComponentsInChildren<ParticleSystem>();
    if (componentsInChildren != null)
    {
      foreach (Component component in componentsInChildren)
        component.gameObject.SetActive(false);
    }
    if ((Object) this.m_LoopingSpell != (Object) null)
    {
      this.m_LoopingSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnLoopingSpellFinished));
      this.m_LoopingSpell.ActivateState(SpellStateType.DEATH);
    }
    else
    {
      if (!((Object) this.m_HideSpell != (Object) null))
        return;
      this.m_HideSpell.Activate();
    }
  }

  private void EnableClickHandler()
  {
    if (!((Object) this.m_dismissButton == (Object) null))
      return;
    this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CloseBannerPopup));
  }

  private void DestroyBannerPopup()
  {
    this.m_onCloseCallbackCalled = true;
    if (this.m_onCloseBannerPopup != null)
      this.m_onCloseBannerPopup();
    this.StartCoroutine(this.DestroyPopupObject());
  }

  private IEnumerator DestroyPopupObject()
  {
    BannerPopup bannerPopup = this;
    while (!bannerPopup.m_showSpellComplete)
      yield return (object) null;
    Object.Destroy((Object) bannerPopup.gameObject);
  }

  private void OnShowSpellFinished(Spell spell, object userData)
  {
    this.m_showSpellComplete = true;
    if ((Object) this.m_LoopingSpell == (Object) null)
      this.OnLoopingSpellFinished((Spell) null, (object) null);
    else
      this.m_LoopingSpell.ActivateState(SpellStateType.ACTION);
  }

  private void OnLoopingSpellFinished(Spell spell, object userData)
  {
    if (!((Object) this.m_HideSpell != (Object) null))
      return;
    this.m_HideSpell.Activate();
  }
}
