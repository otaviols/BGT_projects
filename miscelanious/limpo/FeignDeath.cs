using PegasusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeignDeath : SuperSpell
{
  public GameObject m_RootObject;
  public GameObject m_Glow;
  public float m_Height = 1f;

  protected override void Awake()
  {
    base.Awake();
    this.m_RootObject.SetActive(false);
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    if (!this.m_taskList.IsStartOfBlock())
    {
      base.OnAction(prevStateType);
    }
    else
    {
      ++this.m_effectsPendingFinish;
      base.OnAction(prevStateType);
      this.m_targets.Clear();
      for (PowerTaskList powerTaskList = this.m_taskList; powerTaskList != null; powerTaskList = powerTaskList.GetNext())
      {
        foreach (PowerTask task in powerTaskList.GetTaskList())
        {
          if (task.GetPower() is Network.HistMetaData power && power.MetaType == HistoryMeta.Type.TARGET)
          {
            foreach (int id in power.Info)
              this.m_targets.Add(GameState.Get().GetEntity(id).GetCard().gameObject);
          }
        }
      }
      this.StartCoroutine(this.ActionVisual());
    }
  }

  private IEnumerator ActionVisual()
  {
    FeignDeath feignDeath = this;
    List<GameObject> fxObjects = new List<GameObject>();
    foreach (GameObject target in feignDeath.m_targets)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(feignDeath.m_RootObject);
      gameObject.SetActive(true);
      fxObjects.Add(gameObject);
      gameObject.transform.position = target.transform.position;
      gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + feignDeath.m_Height, gameObject.transform.position.z);
      foreach (ParticleSystem componentsInChild in gameObject.GetComponentsInChildren<ParticleSystem>())
        componentsInChild.Play();
    }
    yield return (object) new WaitForSeconds(1f);
    foreach (Object @object in fxObjects)
      Object.Destroy(@object);
    --feignDeath.m_effectsPendingFinish;
    feignDeath.FinishIfPossible();
  }
}
