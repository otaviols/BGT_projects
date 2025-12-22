using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SpellSystemTest : MonoBehaviour
{
  private string[] m_testManifest = new string[12]
  {
    "FX_Ally_Attack_Impact_Battlegrounds_Large_PREFAB.prefab:dea2b75d99e630e47ab4b0d0bba38a41",
    "FX_Ally_Attack_Impact_Battlegrounds_Mega_PREFAB.prefab:43a5c5d7f0793154ba5aa84181da34ed",
    "FX_Ally_Attack_Impact_Battlegrounds_PREFAB.prefab:c5edc82f4cab6e94e8f222cb0644687a",
    "FX_Ally_Attack_Impact_Battlegrounds_Small_PREFAB.prefab:d46308838cab6d44da610c9e92bcf24e",
    "FX_Ally_Attack_Impact_Mercenaries_Critical_Hit.prefab:a5d9ceeaabd114247acb6c4cbe231a7f",
    "FX_Ally_Attack_Impact_Mercenaries_Critical_Hit_Large.prefab:bce2cc5bb87c78c4cb75ed1e8f0fda15",
    "FX_Ally_Attack_Impact_Mercenaries_Critical_Hit_Mega.prefab:14decebd6a519d143bfebb51e17333bc",
    "FX_Ally_Attack_Impact_Mercenaries_Critical_Hit_Small.prefab:84550a8ea683c5444acdd9a49664a335",
    "ReuseFX_Ally_Attack_Impact_Small_PREFAB.prefab:d507b4e4c8918fa4fb96a889f5181c00",
    "FX_Ally_Attack_Impact_PREFAB.prefab:6c71198e5d017044d904c9095071972f",
    "FX_Ally_Attack_Impact_Large_PREFAB.prefab:9de87064a0d9231479ec94c43c218741",
    "FX_Ally_Attack_Impact_Mega_PREFAB.prefab:17d6b20302c03fb4f810aefb2df1b5ef"
  };
  [SerializeField]
  private Actor m_mockActor;
  [SerializeField]
  private int m_timesToAcquireSameSpell = 3;
  [SerializeField]
  private TextMesh m_spellNameText;
  [SerializeField]
  private TextMesh m_stateText;
  [SerializeField]
  private TextMesh m_timerText;
  [SerializeField]
  private TextMesh m_testText;
  [SerializeField]
  private float m_idleWaitTime = 3f;
  [SerializeField]
  private float m_timeBetweenSpellAcquisitions = 1f;
  private Queue<string> m_spellQueue = new Queue<string>();
  private SpellManager m_spellManager;
  private Spell m_currentSpell;
  private int m_acquireCount;
  private List<SpellStateType> m_statesToTest = new List<SpellStateType>()
  {
    SpellStateType.BIRTH,
    SpellStateType.ACTION,
    SpellStateType.IDLE,
    SpellStateType.DEATH,
    SpellStateType.NONE
  };
  private List<SpellStateType> m_usableStates = new List<SpellStateType>();
  private CancellationTokenSource m_nextStateToken;
  private CancellationTokenSource m_releaseSpellToken;
  private float m_currentTime;
  private Vector3 m_defaultActorPosition;

  public void BeginTest()
  {
    this.m_spellManager = SpellManager.Get();
    this.m_testText.text = "Pooled Spell";
    this.m_defaultActorPosition = this.m_mockActor.transform.position;
    this.m_spellManager.BuildManifestPoolingSet(this.m_testManifest);
    foreach (string str in this.m_testManifest)
      this.m_spellQueue.Enqueue(str);
    this.AcquireNextSpell();
  }

  private void AcquireNextSpell()
  {
    if (this.m_spellQueue.Count == 0)
    {
      Debug.Log((object) "Test Finished");
      this.m_spellNameText.text = "Test Finished";
    }
    else
    {
      string spellAssetRef = this.m_acquireCount < this.m_timesToAcquireSameSpell ? this.m_spellQueue.Peek() : this.m_spellQueue.Dequeue();
      int length = spellAssetRef.IndexOf(':');
      string message = spellAssetRef.Substring(0, length);
      Debug.Log((object) message);
      this.m_spellNameText.text = message;
      this.m_currentSpell?.RemoveFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished));
      this.m_currentSpell?.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished));
      this.m_currentSpell = this.m_spellManager.GetSpell(spellAssetRef);
      this.m_currentTime = 0.0f;
      this.m_usableStates.Clear();
      foreach (SpellStateType stateType in this.m_statesToTest)
      {
        if (this.m_currentSpell.HasUsableState(stateType))
          this.m_usableStates.Add(stateType);
      }
      Transform transform1 = this.m_currentSpell.gameObject.transform;
      Transform transform2 = this.m_mockActor.gameObject.transform;
      TransformUtil.AttachAndPreserveLocalTransform(transform1, transform2);
      transform1.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      transform2.position = this.m_defaultActorPosition;
      this.m_currentSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished));
      this.HandleNextUsableState();
      this.m_acquireCount = this.m_acquireCount < this.m_timesToAcquireSameSpell ? this.m_acquireCount + 1 : 0;
    }
  }

  private void HandleNextUsableState()
  {
    if (this.m_usableStates.Count == 0)
    {
      this.m_releaseSpellToken = new CancellationTokenSource();
      this.WaitThenReleaseSpell(this.m_releaseSpellToken.Token).Forget();
    }
    else
    {
      SpellStateType usableState = this.m_usableStates[0];
      this.m_usableStates.RemoveAt(0);
      this.m_currentSpell.ActivateState(usableState);
      this.m_stateText.text = this.m_currentSpell.GetActiveState().ToString();
      this.m_currentTime = 0.0f;
      this.m_nextStateToken = new CancellationTokenSource();
      this.WaitThenHandleNextSpellState(this.m_nextStateToken.Token).Forget();
    }
  }

  private void OnSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
  {
    this.m_nextStateToken?.Cancel();
    this.m_releaseSpellToken?.Cancel();
    SpellStateType activeState = spell.GetActiveState();
    int index = this.m_usableStates.IndexOf(activeState);
    if (index != -1)
      this.m_usableStates.RemoveAt(index);
    this.m_stateText.text = activeState.ToString();
    this.m_currentTime = 0.0f;
    this.m_nextStateToken = new CancellationTokenSource();
    this.WaitThenHandleNextSpellState(this.m_nextStateToken.Token).Forget();
  }

  private void OnSpellFinished(Spell spell, object userData)
  {
    this.m_nextStateToken?.Cancel();
    this.m_releaseSpellToken?.Cancel();
    this.m_spellManager.ReleaseSpell(spell);
    this.WaitThenAcquireNextSpell().Forget();
  }

  private async UniTaskVoid WaitThenAcquireNextSpell()
  {
    this.m_stateText.text = "Acquiring Spell";
    this.m_currentTime = 0.0f;
    await UniTask.Delay(TimeSpan.FromSeconds((double) this.m_timeBetweenSpellAcquisitions));
    this.AcquireNextSpell();
  }

  private async UniTaskVoid WaitThenReleaseSpell(CancellationToken token)
  {
    this.m_stateText.text = "Releasing Spell";
    this.m_currentTime = 0.0f;
    await UniTask.Delay(TimeSpan.FromSeconds((double) this.m_timeBetweenSpellAcquisitions), cancellationToken: token);
    if (token.IsCancellationRequested)
      return;
    this.OnSpellFinished(this.m_currentSpell, (object) null);
  }

  private async UniTaskVoid WaitThenHandleNextSpellState(CancellationToken token)
  {
    await UniTask.Delay(TimeSpan.FromSeconds((double) this.m_idleWaitTime), cancellationToken: token);
    if (token.IsCancellationRequested)
      return;
    this.HandleNextUsableState();
  }

  private void Update()
  {
    this.m_currentTime += Time.deltaTime;
    this.m_timerText.text = this.m_currentTime.ToString("0.00");
  }
}
