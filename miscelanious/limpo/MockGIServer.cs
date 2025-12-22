using System;
using System.Collections;
using UnityEngine;

public class MockGIServer
{
  private string[] personalizedmessageIDs;

  public MockGIServer() => this.personalizedmessageIDs = new string[3]
  {
    "12234455",
    "23123332",
    "12320948"
  };

  public IEnumerator GetMessages(Action<string[]> OnDone)
  {
    yield return (object) new WaitForSeconds(0.1f);
    Action<string[]> action = OnDone;
    if (action != null)
      action(this.personalizedmessageIDs);
  }
}
