using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimironsHead : SuperSpell
{
  public GameObject m_root;
  public GameObject m_highPosBone;
  public GameObject m_minionPosBone;
  public GameObject m_background;
  public GameObject m_minionElectricity;
  public GameObject m_minionGlow;
  public GameObject m_mimironNegative;
  public GameObject m_mimironFlare;
  public GameObject m_mimironGlow;
  public GameObject m_mimironElectricity;
  public Spell m_voltSpawnOverrideSpell;
  public string m_perMinionSound;
  public string[] m_startSounds;
  private Card m_volt;
  private Card m_mimiron;
  private List<Card> m_mechMinions = new List<Card>();
  private Transform m_voltParent;
  private Color m_clear = new Color(1f, 1f, 1f, 0.0f);
  private Map<GameObject, List<GameObject>> m_cleanup = new Map<GameObject, List<GameObject>>();
  private bool m_isNegFlash;
  private float m_flashDelay = 0.15f;
  private float m_mimironHighTime = 1.5f;
  private float m_minionHighTime = 2f;
  private float m_sparkDelay = 0.3f;
  private float m_absorbTime = 1f;
  private float m_glowTime = 0.5f;
  private PowerTaskList m_waitForTaskList;

  public override bool AddPowerTargets()
  {
    if (!this.CanAddPowerTargets())
      return false;
    Card card1 = this.m_taskList.GetSourceEntity().GetCard();
    if (this.m_taskList.IsOrigin())
    {
      List<PowerTaskList> powerTaskListList = new List<PowerTaskList>();
      for (PowerTaskList powerTaskList = this.m_taskList; powerTaskList != null; powerTaskList = powerTaskList.GetNext())
        powerTaskListList.Add(powerTaskList);
      foreach (PowerTaskList powerTaskList in powerTaskListList)
      {
        foreach (PowerTask task in powerTaskList.GetTaskList())
        {
          Network.PowerHistory power = task.GetPower();
          if (power.Type == Network.PowerType.TAG_CHANGE)
          {
            Network.HistTagChange histTagChange = power as Network.HistTagChange;
            if (histTagChange.Tag == 360 && histTagChange.Value == 1)
            {
              Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
              if (entity == null)
              {
                Debug.LogWarning((object) string.Format("{0}.AddPowerTargets() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) histTagChange.Entity));
                continue;
              }
              Card card2 = entity.GetCard();
              if ((UnityEngine.Object) card2 != (UnityEngine.Object) card1)
                this.m_mechMinions.Add(card2);
              else
                this.m_mimiron = card2;
              this.m_waitForTaskList = powerTaskList;
            }
          }
          if (power.Type == Network.PowerType.FULL_ENTITY)
          {
            Network.Entity entity1 = (power as Network.HistFullEntity).Entity;
            Entity entity2 = GameState.Get().GetEntity(entity1.ID);
            if (entity2 == null)
              Debug.LogWarning((object) string.Format("{0}.AddPowerTargets() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) entity1.ID));
            else if (!(entity2.GetCardId() != "GVG_111t"))
            {
              this.m_volt = entity2.GetCard();
              this.m_waitForTaskList = powerTaskList;
            }
          }
        }
      }
      if ((UnityEngine.Object) this.m_volt != (UnityEngine.Object) null && (UnityEngine.Object) this.m_mimiron != (UnityEngine.Object) null && this.m_mechMinions.Count > 0)
      {
        this.m_mimiron.IgnoreDeath(true);
        foreach (Card mechMinion in this.m_mechMinions)
          mechMinion.IgnoreDeath(true);
        foreach (Card card3 in card1.GetController().GetBattlefieldZone().GetCards())
          card3.SetDoNotSort(true);
      }
      else
      {
        this.m_volt = (Card) null;
        this.m_mimiron = (Card) null;
        this.m_mechMinions.Clear();
      }
    }
    if ((UnityEngine.Object) this.m_volt == (UnityEngine.Object) null || (UnityEngine.Object) this.m_mimiron == (UnityEngine.Object) null || this.m_mechMinions.Count == 0 || this.m_taskList != this.m_waitForTaskList)
      return false;
    foreach (Card card4 in card1.GetController().GetBattlefieldZone().GetCards())
      card4.SetDoNotSort(true);
    return true;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    if ((bool) (UnityEngine.Object) this.m_voltSpawnOverrideSpell)
      this.m_volt.OverrideCustomSpawnSpell(SpellManager.Get().GetSpell(this.m_voltSpawnOverrideSpell));
    this.StartCoroutine(this.TransformEffect());
  }

  private IEnumerator TransformEffect()
  {
    MimironsHead mimironsHead = this;
    foreach (string startSound in mimironsHead.m_startSounds)
      SoundManager.Get().LoadAndPlay((AssetReference) startSound);
    mimironsHead.m_volt.SetDoNotSort(true);
    mimironsHead.m_taskList.DoAllTasks();
    while (!mimironsHead.m_taskList.IsComplete())
      yield return (object) null;
    mimironsHead.m_volt.GetActor().Hide();
    GameObject gameObject = mimironsHead.m_volt.GetActor().gameObject;
    mimironsHead.m_voltParent = gameObject.transform.parent;
    gameObject.transform.parent = mimironsHead.m_highPosBone.transform;
    gameObject.transform.localPosition = new Vector3(0.0f, -0.1f, 0.0f);
    mimironsHead.m_root.transform.parent = (Transform) null;
    mimironsHead.m_root.transform.localPosition = Vector3.zero;
    iTween.MoveTo(mimironsHead.m_mimiron.gameObject, iTween.Hash((object) "position", (object) mimironsHead.m_highPosBone.transform.localPosition, (object) "easetype", (object) iTween.EaseType.easeOutQuart, (object) "time", (object) mimironsHead.m_mimironHighTime, (object) "delay", (object) 0.5f));
    yield return (object) new WaitForSeconds((float) (0.5 + (double) mimironsHead.m_mimironHighTime / 5.0));
    mimironsHead.TransformMinions();
  }

  private void TransformMinions()
  {
    float num1 = 1f;
    Vector3 vector3_1 = new Vector3(0.0f, 0.0f, 2.3f);
    List<int> intList1 = new List<int>();
    for (int index = 0; index < this.m_mechMinions.Count; ++index)
      intList1.Add(index);
    List<int> intList2 = new List<int>();
    for (int index1 = 0; index1 < this.m_mechMinions.Count; ++index1)
    {
      int index2 = UnityEngine.Random.Range(0, intList1.Count);
      intList2.Add(intList1[index2]);
      intList1.RemoveAt(index2);
    }
    for (int index = 0; index < this.m_mechMinions.Count; ++index)
    {
      this.m_minionPosBone.transform.localPosition = this.m_highPosBone.transform.localPosition + Quaternion.Euler(0.0f, (float) (360 / this.m_mechMinions.Count * intList2[index] + 60), 0.0f) * vector3_1;
      GameObject gameObject = this.m_mechMinions[index].GetActor().gameObject;
      float num2 = num1 / (float) this.m_mechMinions.Count * (float) index;
      this.StartCoroutine(this.MinionPlayFX(gameObject, this.m_minionElectricity, num2 / 2f));
      List<Vector3> vector3List = new List<Vector3>();
      Vector3 vector3_2 = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0.0f, UnityEngine.Random.Range(-2f, 2f));
      vector3List.Add(gameObject.transform.position + (this.m_minionPosBone.transform.localPosition - gameObject.transform.position) / 4f + vector3_2);
      vector3List.Add(this.m_minionPosBone.transform.localPosition);
      if (index < this.m_mechMinions.Count - 1)
        iTween.MoveTo(gameObject, iTween.Hash((object) "path", (object) vector3List.ToArray(), (object) "easetype", (object) iTween.EaseType.easeInOutSine, (object) "delay", (object) num2, (object) "time", (object) (float) ((double) this.m_minionHighTime / (double) this.m_mechMinions.Count)));
      else
        iTween.MoveTo(gameObject, iTween.Hash((object) "path", (object) vector3List.ToArray(), (object) "easetype", (object) iTween.EaseType.easeInOutSine, (object) "delay", (object) num2, (object) "time", (object) (float) ((double) this.m_minionHighTime / (double) this.m_mechMinions.Count), (object) "oncomplete", (object) (Action<object>) (newVal => this.FadeInBackground())));
    }
  }

  private IEnumerator MinionPlayFX(GameObject minion, GameObject FX, float delay)
  {
    GameObject minionFX = UnityEngine.Object.Instantiate<GameObject>(FX);
    minionFX.transform.parent = minion.transform;
    minionFX.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
    if (!this.m_cleanup.ContainsKey(minion))
      this.m_cleanup.Add(minion, new List<GameObject>());
    this.m_cleanup[minion].Add(minionFX);
    yield return (object) new WaitForSeconds(delay);
    minionFX.GetComponent<ParticleSystem>().Play();
  }

  private IEnumerator MimironNegativeFX()
  {
    while (this.m_isNegFlash)
    {
      yield return (object) new WaitForSeconds(this.m_flashDelay);
      this.m_mimironNegative.SetActive(!this.m_mimironNegative.activeSelf);
      if ((double) this.m_flashDelay > 0.0500000007450581)
        this.m_flashDelay -= 0.01f;
    }
    this.m_mimironNegative.SetActive(false);
  }

  private void MinionCleanup(GameObject minion)
  {
    if (!this.m_cleanup.ContainsKey(minion))
      return;
    foreach (GameObject gameObject in this.m_cleanup[minion])
    {
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
    }
  }

  private void FadeInBackground()
  {
    this.m_background.SetActive(true);
    RendererExtension.GetMaterial(this.m_background.GetComponent<Renderer>()).SetColor("_Color", this.m_clear);
    HighlightState componentInChildren = this.m_volt.GetActor().gameObject.GetComponentInChildren<HighlightState>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      componentInChildren.Hide();
    iTween.ColorTo(this.m_background, iTween.Hash((object) "r", (object) 1f, (object) "g", (object) 1f, (object) "b", (object) 1f, (object) "a", (object) 1f, (object) "time", (object) 0.5f, (object) "oncomplete", (object) (Action<object>) (newVal => this.MimironPowerUp())));
  }

  private void SetGlow(Material glowMat, float newVal, string colorVal = "_TintColor") => glowMat.SetColor(colorVal, Color.Lerp(this.m_clear, Color.white, newVal));

  private void MimironPowerUp()
  {
    this.m_mimironElectricity.GetComponent<ParticleSystem>().Play();
    for (int index = 0; index < this.m_mechMinions.Count; ++index)
    {
      GameObject gameObject1 = this.m_mechMinions[index].GetActor().gameObject;
      GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.m_minionGlow);
      if (!this.m_cleanup.ContainsKey(gameObject1))
        this.m_cleanup.Add(gameObject1, new List<GameObject>());
      this.m_cleanup[gameObject1].Add(gameObject2);
      gameObject2.transform.parent = gameObject1.transform;
      gameObject2.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
      float num = this.m_absorbTime / (float) this.m_mechMinions.Count * (float) index;
      Material rendererMaterial = RendererExtension.GetMaterial(gameObject2.GetComponent<Renderer>());
      rendererMaterial.SetColor("_TintColor", this.m_clear);
      RenderUtils.EnableRenderers(gameObject2, true);
      if (index < this.m_mechMinions.Count - 1)
      {
        iTween.ValueTo(gameObject2, iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) this.m_glowTime, (object) "delay", (object) (float) (0.100000001490116 + (double) num + (double) this.m_sparkDelay), (object) "onstart", (object) (Action<object>) (newVal => SoundManager.Get().LoadAndPlay((AssetReference) this.m_perMinionSound)), (object) "onupdate", (object) (Action<object>) (newVal => this.SetGlow(rendererMaterial, (float) newVal))));
        iTween.ValueTo(gameObject2, iTween.Hash((object) "from", (object) 1f, (object) "to", (object) 0.0f, (object) "time", (object) this.m_glowTime, (object) "delay", (object) (float) (0.100000001490116 + (double) num + (double) this.m_sparkDelay + (double) this.m_glowTime), (object) "onupdate", (object) (Action<object>) (newVal => this.SetGlow(rendererMaterial, (float) newVal))));
      }
      else
      {
        iTween.ValueTo(gameObject2, iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) this.m_glowTime, (object) "delay", (object) (float) (0.100000001490116 + (double) num + (double) this.m_sparkDelay), (object) "onstart", (object) (Action<object>) (newVal => SoundManager.Get().LoadAndPlay((AssetReference) this.m_perMinionSound)), (object) "onupdate", (object) (Action<object>) (newVal => this.SetGlow(rendererMaterial, (float) newVal)), (object) "oncomplete", (object) (Action<object>) (newVal => this.AbsorbMinions())));
        iTween.ValueTo(gameObject2, iTween.Hash((object) "from", (object) 1f, (object) "to", (object) 0.0f, (object) "time", (object) this.m_glowTime, (object) "delay", (object) (float) (0.100000001490116 + (double) num + (double) this.m_sparkDelay + (double) this.m_glowTime), (object) "onupdate", (object) (Action<object>) (newVal => this.SetGlow(rendererMaterial, (float) newVal))));
      }
    }
  }

  private void AbsorbMinions()
  {
    Vector3 vector3 = new Vector3(0.0f, -1f, 0.0f);
    for (int index = 0; index < this.m_mechMinions.Count; ++index)
    {
      float num = this.m_absorbTime / (float) this.m_mechMinions.Count * (float) index;
      GameObject minion = this.m_mechMinions[index].GetActor().gameObject;
      if (index < this.m_mechMinions.Count - 1)
        iTween.MoveTo(minion, iTween.Hash((object) "position", (object) (this.m_highPosBone.transform.localPosition + vector3), (object) "easetype", (object) iTween.EaseType.easeInOutSine, (object) "delay", (object) (float) ((double) this.m_glowTime + (double) num + (double) this.m_sparkDelay), (object) "time", (object) 0.5f, (object) "oncomplete", (object) (Action<object>) (newVal => this.MinionCleanup(minion))));
      else
        iTween.MoveTo(minion, iTween.Hash((object) "position", (object) (this.m_highPosBone.transform.localPosition + vector3), (object) "easetype", (object) iTween.EaseType.easeInOutSine, (object) "delay", (object) (float) ((double) this.m_glowTime + (double) num + (double) this.m_sparkDelay), (object) "time", (object) 0.5f, (object) "oncomplete", (object) (Action<object>) (newVal =>
        {
          this.MinionCleanup(minion);
          this.FlareMimiron();
        })));
    }
    this.m_isNegFlash = true;
    this.StartCoroutine(this.MimironNegativeFX());
  }

  private void FlareMimiron()
  {
    Material mimironGlowMaterial = RendererExtension.GetMaterial(this.m_mimironGlow.GetComponent<Renderer>());
    Material mimironFlareMaterial = RendererExtension.GetMaterial(this.m_mimironFlare.GetComponent<Renderer>());
    mimironGlowMaterial.SetColor("_TintColor", this.m_clear);
    mimironFlareMaterial.SetColor("_TintColor", this.m_clear);
    this.m_mimironGlow.SetActive(true);
    this.m_mimironFlare.SetActive(true);
    iTween.ValueTo(this.m_mimironGlow, iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 0.7f, (object) "time", (object) 0.3, (object) "onupdate", (object) (Action<object>) (newVal => this.SetGlow(mimironGlowMaterial, (float) newVal))));
    iTween.ValueTo(this.m_mimironFlare, iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 2.5f, (object) "time", (object) 0.3f, (object) "onupdate", (object) (Action<object>) (newVal => this.SetGlow(mimironFlareMaterial, (float) newVal, "_Intensity")), (object) "oncomplete", (object) (Action<object>) (newVal => this.UnflareMimiron())));
  }

  private void UnflareMimiron()
  {
    this.m_volt.SetDoNotSort(false);
    ZonePlay battlefieldZone = this.m_volt.GetController().GetBattlefieldZone();
    foreach (Card card in battlefieldZone.GetCards())
      card.SetDoNotSort(false);
    battlefieldZone.UpdateLayout();
    this.DestroyMinions();
    this.m_volt.GetActor().Show();
    Material mimironGlowMaterial = RendererExtension.GetMaterial(this.m_mimironGlow.GetComponent<Renderer>());
    Material mimironFlareMaterial = RendererExtension.GetMaterial(this.m_mimironFlare.GetComponent<Renderer>());
    mimironGlowMaterial.SetColor("_TintColor", this.m_clear);
    mimironFlareMaterial.SetColor("_TintColor", this.m_clear);
    this.m_mimironGlow.SetActive(true);
    this.m_mimironFlare.SetActive(true);
    iTween.ValueTo(this.m_mimironGlow, iTween.Hash((object) "from", (object) 0.7f, (object) "to", (object) 0.0f, (object) "time", (object) 0.3, (object) "onupdate", (object) (Action<object>) (newVal => this.SetGlow(mimironGlowMaterial, (float) newVal))));
    iTween.ValueTo(this.m_mimironFlare, iTween.Hash((object) "from", (object) 2.5f, (object) "to", (object) 0.0f, (object) "time", (object) 0.3f, (object) "onupdate", (object) (Action<object>) (newVal => this.SetGlow(mimironFlareMaterial, (float) newVal, "_Intensity")), (object) "oncomplete", (object) (Action<object>) (newVal => this.FadeOutBackground())));
    this.m_isNegFlash = false;
    this.OnSpellFinished();
  }

  private void FadeOutBackground()
  {
    this.m_mimironGlow.SetActive(false);
    this.m_mimironFlare.SetActive(false);
    iTween.ColorTo(this.m_background, iTween.Hash((object) "r", (object) 1f, (object) "g", (object) 1f, (object) "b", (object) 1f, (object) "a", (object) 0.0f, (object) "time", (object) 0.5f, (object) "oncomplete", (object) (Action<object>) (newVal => this.RaiseVolt())));
  }

  private void DestroyMinions()
  {
    foreach (Card mechMinion in this.m_mechMinions)
    {
      mechMinion.IgnoreDeath(false);
      mechMinion.SetDoNotSort(false);
      mechMinion.GetActor().Destroy();
    }
    this.m_mimiron.IgnoreDeath(false);
    this.m_mimiron.SetDoNotSort(false);
    this.m_mimiron.GetActor().Destroy();
  }

  private void RaiseVolt()
  {
    this.m_mimironElectricity.GetComponent<ParticleSystem>().Stop();
    RendererExtension.GetMaterial(this.m_background.GetComponent<Renderer>()).SetColor("_Color", this.m_clear);
    this.m_background.SetActive(false);
    GameObject gameObject = this.m_volt.GetActor().gameObject;
    gameObject.transform.parent = this.m_voltParent;
    iTween.MoveTo(gameObject, iTween.Hash((object) "position", (object) (gameObject.transform.localPosition + new Vector3(0.0f, 3f, 0.0f)), (object) "time", (object) 0.2f, (object) "islocal", (object) true, (object) "oncomplete", (object) (Action<object>) (newVal => this.DropV07tron())));
  }

  private void DropV07tron()
  {
    iTween.MoveTo(this.m_volt.GetActor().gameObject, iTween.Hash((object) "position", (object) Vector3.zero, (object) "time", (object) 0.3f, (object) "islocal", (object) true));
    this.Finish();
  }

  private void Finish()
  {
    this.m_volt = (Card) null;
    this.m_mimiron = (Card) null;
    this.m_mechMinions.Clear();
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }
}
