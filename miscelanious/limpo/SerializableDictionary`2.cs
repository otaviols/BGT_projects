using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : 
  Dictionary<TKey, TValue>,
  ISerializationCallbackReceiver
{
  [SerializeField]
  private List<TKey> keys = new List<TKey>();
  [SerializeField]
  private List<TValue> values = new List<TValue>();

  public void OnBeforeSerialize()
  {
    this.keys.Clear();
    this.values.Clear();
    foreach (KeyValuePair<TKey, TValue> keyValuePair in (Dictionary<TKey, TValue>) this)
    {
      this.keys.Add(keyValuePair.Key);
      this.values.Add(keyValuePair.Value);
    }
  }

  public void OnAfterDeserialize()
  {
    this.Clear();
    if (this.keys.Count != this.values.Count)
      throw new Exception(string.Format("There are {0} keys and {1} values after deserialization. Make sure that both kay and value types are serializable", (object) this.keys.Count, (object) this.values.Count));
    for (int index = 0; index < this.keys.Count; ++index)
      this.Add(this.keys[index], this.values[index]);
  }
}
