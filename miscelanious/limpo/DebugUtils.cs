using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugUtils
{
  public static string GetHierarchyPath(Object obj, char separator = '.')
  {
    StringBuilder b = new StringBuilder();
    DebugUtils.GetHierarchyPath_Internal(b, obj, separator);
    return b.ToString();
  }

  public static string GetHierarchyPathAndType(Object obj, char separator = '.')
  {
    StringBuilder b = new StringBuilder();
    b.Append("[Type]=").Append(((object) obj).GetType().FullName).Append(" [Path]=");
    DebugUtils.GetHierarchyPath_Internal(b, obj, separator);
    return b.ToString();
  }

  private static bool GetHierarchyPath_Internal(StringBuilder b, Object obj, char separator)
  {
    if (obj == (Object) null)
      return false;
    Transform transform1;
    switch (obj)
    {
      case GameObject _:
        transform1 = ((GameObject) obj).transform;
        break;
      case Component _:
        transform1 = ((Component) obj).transform;
        break;
      default:
        transform1 = (Transform) null;
        break;
    }
    Transform transform2 = transform1;
    List<string> stringList = new List<string>();
    for (; (Object) transform2 != (Object) null; transform2 = transform2.parent)
      stringList.Insert(0, transform2.gameObject.name);
    if (stringList.Count > 0 && separator == '/')
      b.Append(separator);
    for (int index = 0; index < stringList.Count; ++index)
    {
      b.Append(stringList[index]);
      if (index < stringList.Count - 1)
        b.Append(separator);
    }
    return true;
  }
}
