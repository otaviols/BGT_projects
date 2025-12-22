using Blizzard.T5.MaterialService.Extensions;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class CraftingModeButton : UIBButton
{
  public GameObject m_dustBottle;
  public GameObject m_activeGlow;
  public ParticleSystem m_dustShower;
  public Vector3 m_jarJiggleRotation = new Vector3(0.0f, 30f, 0.0f);
  public GameObject m_textObject;
  public MeshRenderer m_mainMesh;
  public Material m_enabledMaterial;
  public Material m_disabledMaterial;
  public bool m_shouldHideWholeButton;
  private bool m_isGlowEnabled;
  private bool m_showDustBottle;
  private bool m_hasStartedJiggleAnimation;
  private bool m_isDestroyed;
  private CancellationTokenSource m_jiggleTokenSource;

  public void ShowActiveGlow(bool show)
  {
    this.m_isGlowEnabled = show;
    this.m_activeGlow.SetActive(show);
  }

  public void ShowDustBottle(bool show, bool forceMobileActive)
  {
    this.m_showDustBottle = show;
    this.m_dustBottle.SetActive(show || forceMobileActive && (bool) UniversalInputManager.UsePhoneUI);
    if (!show)
      return;
    this.StartBottleJiggle();
  }

  private void StartBottleJiggle()
  {
    if (this.m_hasStartedJiggleAnimation)
      return;
    this.BottleJiggle();
  }

  private void BottleJiggle()
  {
    if (this.m_isDestroyed)
    {
      TelemetryManager.Client().SendLiveIssue("Collections_CraftingModeButton_BottleJiggle", "BottleJiggle called on CraftingModeButton after object was destroyed");
    }
    else
    {
      if (this.m_jiggleTokenSource == null)
        this.m_jiggleTokenSource = new CancellationTokenSource();
      try
      {
        this.Jiggle(this.m_jiggleTokenSource.Token).Forget();
      }
      catch (Exception ex)
      {
        TelemetryManager.Client().SendLiveIssue("Collections_CraftingModeButton_BottleJiggle", "Caught exception '" + ex.Message + "'. Has the token somehow been disposed without destroying the gameobject?");
      }
    }
  }

  private async UniTaskVoid Jiggle(CancellationToken token)
  {
    CraftingModeButton craftingModeButton = this;
    craftingModeButton.m_hasStartedJiggleAnimation = true;
    await UniTask.Delay(TimeSpan.FromSeconds(1.0), cancellationToken: token);
    craftingModeButton.m_dustShower.Play();
    Hashtable args = iTween.Hash((object) "amount", (object) craftingModeButton.m_jarJiggleRotation, (object) "time", (object) 0.5f, (object) "oncomplete", (object) "BottleJiggle", (object) "oncompletetarget", (object) craftingModeButton.gameObject);
    iTween.PunchRotation(craftingModeButton.m_dustBottle.gameObject, args);
  }

  public void Enable(bool enabled)
  {
    if (this.IsEnabled() == enabled)
      return;
    this.SetEnabled(enabled);
    if (this.m_shouldHideWholeButton)
    {
      this.Flip(enabled);
    }
    else
    {
      this.m_activeGlow.SetActive(enabled && this.m_isGlowEnabled);
      this.m_dustShower.gameObject.SetActive(enabled);
      this.m_dustBottle.SetActive(enabled && (this.m_showDustBottle || (bool) UniversalInputManager.UsePhoneUI));
      if ((UnityEngine.Object) this.m_textObject != (UnityEngine.Object) null)
        this.m_textObject.SetActive(enabled);
      if (!((UnityEngine.Object) this.m_mainMesh != (UnityEngine.Object) null))
        return;
      this.m_mainMesh.SetSharedMaterial(enabled ? this.m_enabledMaterial : this.m_disabledMaterial);
    }
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    this.m_hasStartedJiggleAnimation = false;
    this.m_jiggleTokenSource?.Cancel();
    this.m_jiggleTokenSource?.Dispose();
    this.m_jiggleTokenSource = (CancellationTokenSource) null;
    this.m_isDestroyed = true;
  }
}
