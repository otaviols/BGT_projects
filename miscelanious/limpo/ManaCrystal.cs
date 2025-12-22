using Blizzard.T5.MaterialService.Extensions;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class ManaCrystal : MonoBehaviour
{
  public GameObject gem;
  public GameObject spawnEffects;
  public GameObject gemDestroy;
  public GameObject tempSpawnEffects;
  public GameObject tempGemDestroy;
  private readonly string ANIM_SPAWN_EFFECTS = "mana_spawn_edit";
  private readonly string ANIM_TEMP_SPAWN_EFFECTS = "mana_spawn_edit_temp";
  private readonly string ANIM_MANA_GEM_BIRTH = "ManaGemBirth";
  private readonly string ANIM_TEMP_MANA_GEM_BIRTH = "ManaGemBirth_Temp";
  private readonly string ANIM_READY_TO_USED = "ManaGemUsed";
  private readonly string ANIM_USED_TO_READY = "ManaGem_Restore";
  private readonly string ANIM_READY_TO_PROPOSED = "ManaGemProposed";
  private readonly string ANIM_TEMP_READY_TO_PROPOSED = "ManaGemProposed_Temp";
  private readonly string ANIM_PROPOSED_TO_READY = "ManaGemProposed_Cancel";
  private readonly string ANIM_TEMP_PROPOSED_TO_READY = "ManaGemProposed_Cancel_Temp";
  private readonly string ANIM_USED_TO_PROPOSED = "ManaGemUsed_Proposed";
  private readonly string ANIM_PROPOSED_TO_USED = "ManaGemProposed_Used";
  private bool m_isInGame = true;
  private bool m_birthAnimationPlayed;
  private bool m_playingAnimation;
  private bool m_isTemp;
  private Spell m_overloadOwedSpell;
  private Spell m_overloadPaidSpell;
  private ManaCrystal.State m_state;
  private ManaCrystal.State m_visibleState;
  private CancellationTokenSource m_tokenSource;

  private void Start() => this.m_tokenSource = new CancellationTokenSource();

  private void Update()
  {
    ManaCrystal.State state = this.state;
    if (state == this.m_visibleState || state == ManaCrystal.State.DESTROYED)
      return;
    this.PlayGemAnimation(this.GetTransitionAnimName(this.m_visibleState, state), state);
  }

  private void OnDestroy()
  {
    this.m_tokenSource?.Cancel();
    this.m_tokenSource?.Dispose();
  }

  public ManaCrystal.State state
  {
    get => this.m_state;
    set
    {
      if (this.m_state == ManaCrystal.State.DESTROYED)
        return;
      if (value == ManaCrystal.State.DESTROYED)
        this.Destroy();
      else
        this.m_state = value;
    }
  }

  public void MarkAsNotInGame() => this.m_isInGame = false;

  public void MarkAsTemp()
  {
    this.m_isTemp = true;
    ManaCrystalMgr manaCrystalMgr = ManaCrystalMgr.Get();
    this.gem.GetComponentInChildren<MeshRenderer>().SetMaterial(manaCrystalMgr.GetTemporaryManaCrystalMaterial());
    this.gem.transform.Find("Proposed_Quad").gameObject.GetComponent<MeshRenderer>().SetMaterial(manaCrystalMgr.GetTemporaryManaCrystalProposedQuadMaterial());
  }

  public void PlayCreateAnimation()
  {
    this.spawnEffects.SetActive(!this.m_isTemp);
    this.tempSpawnEffects.SetActive(this.m_isTemp);
    if (this.m_isTemp)
    {
      this.tempSpawnEffects.GetComponent<Animation>().Play(this.ANIM_TEMP_SPAWN_EFFECTS);
      this.PlayGemAnimation(this.ANIM_TEMP_MANA_GEM_BIRTH, ManaCrystal.State.READY);
    }
    else
    {
      this.spawnEffects.GetComponent<Animation>().Play(this.ANIM_SPAWN_EFFECTS);
      this.PlayGemAnimation(this.ANIM_MANA_GEM_BIRTH, ManaCrystal.State.READY);
    }
  }

  public void Destroy()
  {
    this.m_state = ManaCrystal.State.DESTROYED;
    this.WaitThenDestroy(this.m_tokenSource.Token).Forget();
  }

  public bool IsOverloaded() => (UnityEngine.Object) this.m_overloadPaidSpell != (UnityEngine.Object) null;

  public bool IsOwedForOverload() => (UnityEngine.Object) this.m_overloadOwedSpell != (UnityEngine.Object) null;

  public void MarkAsOwedForOverload() => this.MarkAsOwedForOverload(false);

  public void ReclaimOverload()
  {
    if (!this.IsOwedForOverload())
      return;
    this.m_overloadOwedSpell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadBirthCompletePayOverload));
    this.m_overloadOwedSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadUnlockedAnimComplete));
    this.m_overloadOwedSpell.ActivateState(SpellStateType.DEATH);
    this.m_overloadOwedSpell = (Spell) null;
  }

  public void Hide()
  {
    this.gem.SetActive(false);
    if (this.m_isTemp)
      this.tempSpawnEffects.SetActive(false);
    else
      this.spawnEffects.SetActive(false);
  }

  public void Show()
  {
    this.gem.SetActive(true);
    if (this.m_isTemp)
      this.tempSpawnEffects.SetActive(true);
    else
      this.spawnEffects.SetActive(true);
  }

  public void PayOverload()
  {
    if (!this.IsOwedForOverload())
    {
      this.state = ManaCrystal.State.USED;
      this.MarkAsOwedForOverload(true);
    }
    else
    {
      this.m_overloadPaidSpell = this.m_overloadOwedSpell;
      this.m_overloadOwedSpell = (Spell) null;
      this.m_overloadPaidSpell.ActivateState(SpellStateType.ACTION);
    }
  }

  public void UnlockOverload()
  {
    if (!this.IsOverloaded())
      return;
    this.m_overloadPaidSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadUnlockedAnimComplete));
    this.m_overloadPaidSpell.ActivateState(SpellStateType.DEATH);
    this.m_overloadPaidSpell = (Spell) null;
  }

  private void PlayGemAnimation(string animName, ManaCrystal.State newVisibleState)
  {
    if (this.m_isInGame && !this.m_birthAnimationPlayed)
    {
      if ((animName.Equals(this.ANIM_MANA_GEM_BIRTH) ? 1 : (animName.Equals(this.ANIM_TEMP_MANA_GEM_BIRTH) ? 1 : 0)) == 0)
        return;
      this.m_birthAnimationPlayed = true;
    }
    Animation component = this.gem.GetComponent<Animation>();
    if (!(bool) (TrackedReference) component[animName])
    {
      Debug.LogWarning((object) string.Format("Mana gem animation named '{0}' doesn't exist.", (object) animName));
    }
    else
    {
      if (this.state == ManaCrystal.State.DESTROYED || this.m_playingAnimation)
        return;
      this.m_playingAnimation = true;
      component.cullingType = AnimationCullingType.BasedOnRenderers;
      component[animName].normalizedTime = 1f;
      component[animName].time = 0.0f;
      component[animName].speed = 1f;
      component.Play(animName);
      if (!this.gameObject.activeInHierarchy)
      {
        this.m_playingAnimation = false;
        this.m_visibleState = newVisibleState;
      }
      else
        this.WaitForAnimation(animName, newVisibleState, this.m_tokenSource.Token).Forget();
    }
  }

  private async UniTaskVoid WaitForAnimation(
    string animName,
    ManaCrystal.State newVisibleState,
    CancellationToken token)
  {
    await UniTask.Delay(TimeSpan.FromSeconds((double) this.gem.GetComponent<Animation>()[animName].length), cancellationToken: token);
    this.m_visibleState = newVisibleState;
    this.m_playingAnimation = false;
  }

  private string GetTransitionAnimName(ManaCrystal.State oldState, ManaCrystal.State newState)
  {
    string transitionAnimName = "";
    switch (oldState)
    {
      case ManaCrystal.State.READY:
        switch (newState)
        {
          case ManaCrystal.State.USED:
            transitionAnimName = this.ANIM_READY_TO_USED;
            break;
          case ManaCrystal.State.PROPOSED:
            transitionAnimName = this.m_isTemp ? this.ANIM_TEMP_READY_TO_PROPOSED : this.ANIM_READY_TO_PROPOSED;
            break;
        }
        break;
      case ManaCrystal.State.USED:
        switch (newState)
        {
          case ManaCrystal.State.READY:
            transitionAnimName = this.ANIM_USED_TO_READY;
            break;
          case ManaCrystal.State.PROPOSED:
            transitionAnimName = this.ANIM_USED_TO_PROPOSED;
            break;
        }
        break;
      case ManaCrystal.State.PROPOSED:
        switch (newState)
        {
          case ManaCrystal.State.READY:
            transitionAnimName = this.m_isTemp ? this.ANIM_TEMP_PROPOSED_TO_READY : this.ANIM_PROPOSED_TO_READY;
            break;
          case ManaCrystal.State.USED:
            transitionAnimName = this.ANIM_PROPOSED_TO_USED;
            break;
        }
        break;
      case ManaCrystal.State.DESTROYED:
        Log.Gameplay.Print("Trying to get an anim name for a mana that's been destroyed!!!");
        break;
    }
    return transitionAnimName;
  }

  private async UniTaskVoid WaitThenDestroy(CancellationToken token)
  {
    ManaCrystal manaCrystal = this;
    while (manaCrystal.m_playingAnimation)
      await UniTask.Yield(PlayerLoopTiming.Update, token);
    Spell spell = manaCrystal.m_isTemp ? manaCrystal.tempGemDestroy.GetComponent<Spell>() : manaCrystal.gemDestroy.GetComponent<Spell>();
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(manaCrystal.OnGemDestroyedAnimComplete));
    spell.Activate();
  }

  private void OnGemDestroyedAnimComplete(
    Spell spell,
    SpellStateType spellStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  private void OnOverloadUnlockedAnimComplete(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) spell.transform.parent.gameObject);
  }

  private void OnOverloadBirthCompletePayOverload(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.IDLE)
      return;
    spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadBirthCompletePayOverload));
    this.PayOverload();
  }

  public void MarkAsOwedForOverload(bool immediatelyLockForOverload)
  {
    if (this.IsOwedForOverload())
    {
      if (!immediatelyLockForOverload)
        return;
      this.PayOverload();
    }
    else
    {
      GameObject gameObject = (GameObject) GameUtils.InstantiateGameObject((string) (MobileOverrideValue<string>) ManaCrystalMgr.Get().manaLockPrefab, this.gameObject);
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        gameObject.transform.localPosition = new Vector3(0.0f, 0.1f, 0.0f);
        float num = 1.1f;
        gameObject.transform.localScale = new Vector3(num, num, num);
      }
      else
      {
        float num = 1f / this.transform.localScale.x;
        gameObject.transform.localScale = new Vector3(num, num, num);
      }
      this.m_overloadOwedSpell = gameObject.transform.Find("Lock_Mana").GetComponent<Spell>();
      this.m_overloadOwedSpell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadUnlockedAnimComplete));
      if (immediatelyLockForOverload)
        this.m_overloadOwedSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadBirthCompletePayOverload));
      this.m_overloadOwedSpell.ActivateState(SpellStateType.BIRTH);
    }
  }

  public enum State
  {
    READY,
    USED,
    PROPOSED,
    DESTROYED,
  }
}
