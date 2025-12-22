using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class TGTFood : MonoBehaviour
{
  public bool m_Phone;
  public GameObject m_Triangle;
  public Animator m_TriangleAnimator;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_TriangleSoundPrefab;
  public int m_StartingFoodIndex;
  public float m_NewFoodDelay = 1f;
  public List<TGTFood.FoodItem> m_Food;
  public TGTFood.FoodItem m_Drink;
  private bool m_isAnimating;
  private int m_lastFoodIdx;
  private TGTFood.FoodItem m_CurrentFoodItem;
  private float m_phoneNextCheckTime;

  private void Start()
  {
    this.m_CurrentFoodItem = this.m_Food[this.m_StartingFoodIndex];
    this.m_lastFoodIdx = this.m_StartingFoodIndex;
    this.m_CurrentFoodItem.m_FSM.gameObject.SetActive(true);
    this.m_CurrentFoodItem.m_FSM.SendEvent("Birth");
    this.m_Drink.m_FSM.gameObject.SetActive(true);
    this.m_Drink.m_FSM.SendEvent("Birth");
    if (!this.m_Phone)
      return;
    this.m_Triangle.SetActive(false);
  }

  private void Update()
  {
    this.HandleHits();
    if (!this.m_Phone || this.m_Triangle.activeSelf || (double) Time.timeSinceLevelLoad < (double) this.m_phoneNextCheckTime)
      return;
    this.m_phoneNextCheckTime = Time.timeSinceLevelLoad + 0.25f;
    if (this.m_CurrentFoodItem.m_FSM.FsmVariables.FindFsmBool("isEmpty").Value & this.m_Drink.m_FSM.FsmVariables.FindFsmBool("isEmpty").Value && !this.m_isAnimating)
    {
      this.m_Triangle.SetActive(true);
    }
    else
    {
      if (!this.m_Triangle.activeSelf)
        return;
      this.m_Triangle.SetActive(false);
    }
  }

  private void HandleHits()
  {
    if (!InputCollection.GetMouseButtonUp(0) || !this.IsOver(this.m_Triangle) || this.m_isAnimating)
      return;
    this.StartCoroutine(this.RingTheBell());
  }

  private IEnumerator RingTheBell()
  {
    if (this.m_Phone)
      this.m_Triangle.SetActive(false);
    this.m_isAnimating = true;
    bool foodEmpty = this.m_CurrentFoodItem.m_FSM.FsmVariables.FindFsmBool("isEmpty").Value;
    bool drinkEmpty = this.m_Drink.m_FSM.FsmVariables.FindFsmBool("isEmpty").Value;
    this.BellAnimation();
    if (foodEmpty)
      this.m_CurrentFoodItem.m_FSM.SendEvent("Death");
    if (drinkEmpty)
      this.m_Drink.m_FSM.SendEvent("Death");
    yield return (object) new WaitForSeconds(this.m_NewFoodDelay);
    if (this.m_Phone)
      this.m_Triangle.SetActive(false);
    if (foodEmpty)
    {
      int index = UnityEngine.Random.Range(0, this.m_Food.Count);
      if (index == this.m_lastFoodIdx)
      {
        index = UnityEngine.Random.Range(0, this.m_Food.Count);
        if (index == this.m_lastFoodIdx)
        {
          index = this.m_lastFoodIdx - 1;
          if (index < 0)
            index = 0;
        }
      }
      this.m_lastFoodIdx = index;
      this.m_CurrentFoodItem = this.m_Food[index];
      this.m_CurrentFoodItem.m_FSM.gameObject.SetActive(true);
      this.m_CurrentFoodItem.m_FSM.SendEvent("Birth");
    }
    if (drinkEmpty)
      this.m_Drink.m_FSM.SendEvent("Birth");
    yield return (object) new WaitForSeconds(this.m_NewFoodDelay);
    this.m_isAnimating = false;
  }

  private void BellAnimation()
  {
    if (!this.m_Phone)
      this.m_TriangleAnimator.SetTrigger("Clicked");
    if (string.IsNullOrEmpty(this.m_TriangleSoundPrefab))
      return;
    string triangleSoundPrefab = this.m_TriangleSoundPrefab;
    if (string.IsNullOrEmpty(triangleSoundPrefab))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) triangleSoundPrefab, this.m_Triangle);
  }

  private bool IsOver(GameObject go) => (bool) (UnityEngine.Object) go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);

  [Serializable]
  public class FoodItem
  {
    public PlayMakerFSM m_FSM;
  }
}
