using System.Collections.Generic;

public class MultiAttributeParser
{
  private Dictionary<string, string> rawDict;

  public bool load(string[] args, out string errMsg)
  {
    errMsg = (string) null;
    this.rawDict = new Dictionary<string, string>();
    if (args.Length == 0)
    {
      errMsg = "There are too few number of arguments.";
      return false;
    }
    for (int index = 0; index < args.Length; ++index)
    {
      string[] strArray = args[index].Split('=');
      if (strArray.Length <= 1)
      {
        errMsg = "Failed to parse into raw dictionary: no value provided.";
        return false;
      }
      this.rawDict.Add(strArray[0], strArray[1]);
    }
    return true;
  }

  public bool getIntAttribute(string key, out int? value, out string errMsg)
  {
    errMsg = (string) null;
    value = new int?();
    if (this.rawDict.ContainsKey(key))
    {
      int result;
      if (!int.TryParse(this.rawDict[key], out result))
      {
        errMsg = string.Format("Failed to parse {0} int attribute value: The value must be a valid number.", (object) key);
        return false;
      }
      value = new int?(result);
    }
    return true;
  }

  public bool getBoolAttribute(string key, out bool? value, out string errMsg)
  {
    errMsg = (string) null;
    value = new bool?();
    if (this.rawDict.ContainsKey(key))
    {
      bool result;
      if (!bool.TryParse(this.rawDict[key], out result))
      {
        errMsg = string.Format("Failed to parse {0} boolean attribute value: The value must be a valid boolean(true/false).", (object) key);
        return false;
      }
      value = new bool?(result);
    }
    return true;
  }

  public bool getStringAttribute(string key, out string value)
  {
    value = (string) null;
    if (this.rawDict.ContainsKey(key))
      value = this.rawDict[key];
    return true;
  }
}
