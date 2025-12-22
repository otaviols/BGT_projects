using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnTimed : MonoBehaviour
{
  public float minWaitTime = 5f;
  public float maxWaitTime = 15f;
  public float killX = 10f;
  public float killZ = 10f;
  public GameObject objPrefab;
  private List<GameObject> listOfObjs;

  private void Start()
  {
    this.listOfObjs = new List<GameObject>();
    this.StartCoroutine(this.RespawnLoop());
  }

  private IEnumerator RespawnLoop()
  {
    RandomSpawnTimed randomSpawnTimed = this;
    while (true)
    {
      yield return (object) new WaitForSeconds(Random.Range(randomSpawnTimed.minWaitTime, randomSpawnTimed.maxWaitTime));
      randomSpawnTimed.listOfObjs.Add(Object.Instantiate<GameObject>(randomSpawnTimed.objPrefab, randomSpawnTimed.transform.position, Random.rotation));
    }
  }

  private void Update()
  {
    for (int index = 0; index < this.listOfObjs.Count; ++index)
    {
      if ((double) Mathf.Abs(this.listOfObjs[index].transform.position.x - this.gameObject.transform.position.x) > (double) this.killX || (double) Mathf.Abs(this.listOfObjs[index].transform.position.z - this.gameObject.transform.position.z) > (double) this.killZ)
      {
        GameObject listOfObj = this.listOfObjs[index];
        this.listOfObjs.Remove(this.listOfObjs[index]);
        Object.Destroy((Object) listOfObj);
        --index;
      }
    }
  }
}
