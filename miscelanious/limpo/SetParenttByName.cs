using UnityEngine;

[CustomEditClass]
public class SetParenttByName : MonoBehaviour
{
  [CustomEditField(T = EditType.SCENE_OBJECT)]
  public string m_ParentName;

  private void Start()
  {
    if (string.IsNullOrEmpty(this.m_ParentName))
      return;
    GameObject gameObject = this.FindGameObject(this.m_ParentName);
    if ((Object) gameObject == (Object) null)
      Log.Graphics.Print("SetParenttByName failed to locate parent object: {0}", (object) this.m_ParentName);
    else
      this.transform.parent = gameObject.transform;
  }

  private GameObject FindGameObject(string gameObjName)
  {
    if (gameObjName[0] != '/')
      return GameObject.Find(gameObjName);
    string[] strArray = gameObjName.Split('/');
    return GameObject.Find(strArray[strArray.Length - 1]);
  }
}
