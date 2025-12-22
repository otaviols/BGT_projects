using UnityEngine;

public class DOPAsset : ScriptableObject
{
  public int DataVersion;

  public static DOPAsset GenerateDOPAsset()
  {
    DOPAsset instance = ScriptableObject.CreateInstance<DOPAsset>();
    instance.DataVersion = 25000;
    return instance;
  }
}
