using Blizzard.T5.Services;
using System.Collections;
using UnityEngine;

public class ProjectedShadowController : MonoBehaviour
{
  private Actor actor;
  private Card card;
  private Spell customSpawnSpell;
  private Spell customSummonSpell;
  private IGraphicsManager graphicManager;
  private bool isRootObjectProjectedShadowEnabled;
  private float initializationTime;
  public ProjectedShadow rootObjectProjectedShadow;

  private void Awake() => this.graphicManager = ServiceManager.Get<IGraphicsManager>();

  private void Start()
  {
    this.initializationTime = Time.timeSinceLevelLoad;
    this.isRootObjectProjectedShadowEnabled = false;
    if (this.graphicManager == null || this.graphicManager.RenderQualityLevel == GraphicsQuality.High)
      return;
    this.StartCoroutine(this.getSpawnSpell());
  }

  private void LateUpdate()
  {
    if (this.isRootObjectProjectedShadowEnabled || !this.rootObjectProjectedShadow.enabled)
      return;
    this.rootObjectProjectedShadow.enabled = false;
  }

  private IEnumerator getSpawnSpell()
  {
    ProjectedShadowController shadowController = this;
    shadowController.actor = shadowController.GetComponent<Actor>();
    while ((Object) shadowController.card == (Object) null)
    {
      shadowController.card = shadowController.actor.GetCard();
      if ((double) Time.timeSinceLevelLoad - (double) shadowController.initializationTime > 2.0)
        yield break;
      else
        yield return (object) null;
    }
    shadowController.customSummonSpell = shadowController.card.GetCustomSummonSpell();
    if ((Object) shadowController.customSummonSpell != (Object) null)
    {
      shadowController.enableRootShadow();
      shadowController.customSummonSpell.AddFinishedCallback(new Spell.FinishedCallback(shadowController.disableRootShadow));
    }
    shadowController.customSpawnSpell = shadowController.card.GetCustomSpawnSpellOverride();
    if ((Object) shadowController.customSpawnSpell == (Object) null)
      shadowController.customSpawnSpell = shadowController.card.GetCustomSpawnSpell();
    if ((Object) shadowController.customSpawnSpell != (Object) null)
    {
      shadowController.enableRootShadow();
      shadowController.customSpawnSpell.AddFinishedCallback(new Spell.FinishedCallback(shadowController.disableRootShadow));
    }
  }

  private void enableRootShadow()
  {
    this.rootObjectProjectedShadow.enabled = true;
    this.isRootObjectProjectedShadowEnabled = true;
  }

  private void disableRootShadow(Spell spell, object userData)
  {
    this.rootObjectProjectedShadow.enabled = false;
    this.isRootObjectProjectedShadowEnabled = false;
  }
}
