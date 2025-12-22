using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BoosterStack : MonoBehaviour
{
  [SerializeField]
  protected Vector3 m_incrementalDisplacement;
  [SerializeField]
  protected int m_stackSize;
  [SerializeField]
  protected GameObject m_boosterContainer;
  [SerializeField]
  protected float m_stackingDelay;
  [SerializeField]
  protected float m_stackingBaseDuration = 0.1f;
  [SerializeField]
  protected float m_stackingIncrementalDuration = 0.02f;
  [SerializeField]
  protected string m_boosterIntroEvent;
  [SerializeField]
  protected string m_boosterOutroEvent;
  [SerializeField]
  protected string m_boosterInstantIntroEvent;
  [SerializeField]
  protected string m_boosterInstantOutroEvent;
  private float m_playTime;
  private float m_startTime;
  private float m_endTime;
  private int m_startingStackSize;
  private int m_currentStackSize;
  private int m_targetStackSize;
  private List<GameObject> m_boosters = new List<GameObject>();
  private bool m_instantIntro;

  private void Start()
  {
    foreach (Transform transform in this.m_boosterContainer.transform)
    {
      int idx = this.m_boosters.Count;
      this.m_boosters.Add(transform.gameObject);
      Widget component = transform.GetComponent<Widget>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null && !component.IsReady)
        component.RegisterReadyListener((Action<object>) (w => this.OnBoosterReady(idx)), (object) null, true);
      else
        this.OnBoosterReady(idx);
    }
  }

  private void Update()
  {
    if (this.IsSettled())
      return;
    this.m_playTime += Time.deltaTime;
    int currentStackSize = this.m_currentStackSize;
    if ((double) this.m_playTime < (double) this.m_startTime)
      return;
    int endIdx;
    if ((double) this.m_playTime < (double) this.m_endTime && !Mathf.Approximately(this.m_endTime, this.m_startTime))
    {
      int num1 = this.m_startingStackSize + Math.Sign(this.m_targetStackSize - this.m_startingStackSize);
      float num2 = (float) (((double) this.m_playTime - (double) this.m_startTime) / ((double) this.m_endTime - (double) this.m_startTime));
      endIdx = num1 + (int) ((double) num2 * (double) (this.m_targetStackSize - num1));
    }
    else
    {
      endIdx = this.m_targetStackSize;
      this.m_playTime = this.m_endTime = this.m_startTime = 0.0f;
    }
    if (endIdx > this.m_currentStackSize)
      this.PlayEventAcrossRange(BoosterStack.BoosterEvent.INTRO, this.m_currentStackSize, endIdx - 1);
    else if (endIdx < this.m_currentStackSize)
      this.PlayEventAcrossRange(BoosterStack.BoosterEvent.OUTRO, this.m_currentStackSize - 1, endIdx);
    this.m_currentStackSize = endIdx;
  }

  private void Awake() => this.SetStacks(this.m_targetStackSize, this.m_instantIntro);

  [Overridable]
  public float StackingDelay
  {
    get => this.m_stackingDelay;
    set => this.m_stackingDelay = value;
  }

  [Overridable]
  public float StackingBaseDuration
  {
    get => this.m_stackingBaseDuration;
    set => this.m_stackingBaseDuration = value;
  }

  [Overridable]
  public float StackingIncrementalDuration
  {
    get => this.m_stackingIncrementalDuration;
    set => this.m_stackingIncrementalDuration = value;
  }

  public int CurrentStackSize => this.m_currentStackSize;

  public bool IsSettled() => this.m_currentStackSize == this.m_targetStackSize;

  public void SetStacks(int stackSize, bool instantaneous = true)
  {
    if (!this.gameObject.activeInHierarchy)
    {
      this.m_targetStackSize = stackSize;
      this.m_instantIntro = instantaneous;
    }
    else if (instantaneous)
    {
      this.m_targetStackSize = stackSize;
      if (stackSize > this.m_currentStackSize)
        this.PlayEventAcrossRange(BoosterStack.BoosterEvent.INSTANT_INTRO, this.m_currentStackSize, stackSize - 1);
      else if (stackSize < this.m_currentStackSize)
        this.PlayEventAcrossRange(BoosterStack.BoosterEvent.INSTANT_OUTRO, this.m_currentStackSize - 1, stackSize);
      this.m_currentStackSize = this.m_targetStackSize;
    }
    else
      this.AddStacks(stackSize - this.m_targetStackSize);
  }

  public void AddStacks(int deltaStacks)
  {
    deltaStacks = Math.Max(deltaStacks, -this.m_targetStackSize);
    float num = (float) Math.Abs(deltaStacks) * this.StackingIncrementalDuration;
    if (this.IsSettled())
    {
      this.m_playTime = 0.0f;
      this.m_startTime = this.StackingDelay;
      this.m_endTime = this.m_startTime + this.StackingBaseDuration + num;
      this.m_startingStackSize = this.m_currentStackSize;
    }
    else
    {
      if (deltaStacks > 0 != this.m_targetStackSize > this.m_currentStackSize)
      {
        this.m_endTime = this.m_startTime = this.m_playTime = 0.0f;
        this.m_startingStackSize = this.m_currentStackSize;
        num = (float) Math.Abs(deltaStacks + this.m_targetStackSize - this.m_currentStackSize) * this.StackingIncrementalDuration;
      }
      this.m_endTime += num;
    }
    this.m_targetStackSize += deltaStacks;
    if (this.m_currentStackSize != this.m_targetStackSize)
      return;
    this.m_endTime = this.m_startTime = 0.0f;
  }

  protected void PlayEventAcrossRange(BoosterStack.BoosterEvent ev, int startIdx, int endIdx)
  {
    int atIndex = Math.Min(startIdx, endIdx);
    for (int index = Math.Max(startIdx, endIdx); atIndex <= index; ++atIndex)
      this.PlayEvent(ev, atIndex);
  }

  protected void PlayEvent(BoosterStack.BoosterEvent ev, int atIndex)
  {
    if (atIndex >= this.m_boosters.Count)
    {
      Log.Store.PrintError("BoosterStack::PlayEvent index {0} out of range (max: {1})", (object) atIndex, (object) (this.m_boosters.Count - 1));
    }
    else
    {
      GameObject booster = this.m_boosters[atIndex];
      booster.transform.localPosition = this.m_incrementalDisplacement * (float) (atIndex - 1);
      Widget component = booster.GetComponent<Widget>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null && !component.IsReady)
      {
        booster.SetActive(true);
      }
      else
      {
        PlayMakerFSM componentInChildren = booster.GetComponentInChildren<PlayMakerFSM>();
        if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
        {
          Log.Store.PrintError("No PlayMakerFSM found on booster {0} in BoosterStack {1}!", (object) booster, (object) this);
        }
        else
        {
          switch (ev)
          {
            case BoosterStack.BoosterEvent.INTRO:
              componentInChildren.SendEvent(this.m_boosterIntroEvent);
              break;
            case BoosterStack.BoosterEvent.OUTRO:
              componentInChildren.SendEvent(this.m_boosterOutroEvent);
              break;
            case BoosterStack.BoosterEvent.INSTANT_INTRO:
              componentInChildren.SendEvent(this.m_boosterInstantIntroEvent);
              break;
            case BoosterStack.BoosterEvent.INSTANT_OUTRO:
              componentInChildren.SendEvent(this.m_boosterInstantOutroEvent);
              break;
          }
        }
      }
    }
  }

  protected void OnBoosterReady(int idx)
  {
    if (idx >= this.m_currentStackSize)
      this.PlayEvent(BoosterStack.BoosterEvent.INSTANT_OUTRO, idx);
    else if (idx == this.m_currentStackSize - 1)
      this.PlayEvent(BoosterStack.BoosterEvent.INTRO, idx);
    else
      this.PlayEvent(BoosterStack.BoosterEvent.INSTANT_INTRO, idx);
  }

  protected enum BoosterEvent
  {
    INTRO,
    OUTRO,
    INSTANT_INTRO,
    INSTANT_OUTRO,
  }
}
