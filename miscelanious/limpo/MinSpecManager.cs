using MiniJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class MinSpecManager
{
  private MinSpecManager.RequirementSpec[] m_requirements;
  private static MinSpecManager s_instance;

  public static MinSpecManager Get()
  {
    if (MinSpecManager.s_instance == null)
    {
      MinSpecManager.s_instance = new MinSpecManager();
      MinSpecManager.s_instance.Initialize();
    }
    return MinSpecManager.s_instance;
  }

  public bool LoadJsonData(string json, string keyOS = "other")
  {
    if (string.IsNullOrEmpty(json))
      return false;
    bool flag = false;
    try
    {
      if (Json.Deserialize(json) is JsonNode jsonNode)
      {
        if (jsonNode.ContainsKey(keyOS))
        {
          JsonNode jsonNode1 = jsonNode[keyOS] as JsonNode;
          foreach (MinSpecManager.RequirementSpec requirement in this.m_requirements)
          {
            if (jsonNode1.ContainsKey(requirement.m_keyName))
            {
              JsonNode jsonNode2 = jsonNode1[requirement.m_keyName] as JsonNode;
              foreach (string key in jsonNode2.Keys)
              {
                float result;
                if (float.TryParse(key, out result))
                  requirement.m_requirement[result] = (float) Convert.ChangeType(jsonNode2[key], typeof (float));
              }
              flag = true;
            }
          }
        }
      }
    }
    catch (Exception ex)
    {
      Log.MinSpecManager.PrintError("Failed to parse the minspec info: {0}\n'{1}'", (object) ex.Message, (object) json);
    }
    return flag;
  }

  public List<MinSpecManager.MinSpecKind> GetNotEnoughSpecs(
    bool isChangedVersion,
    string LiveVersion)
  {
    float version = 25f;
    if (isChangedVersion)
    {
      int[] versionInt;
      if (UpdateUtils.GetSplitVersion(LiveVersion, out versionInt))
        version = (float) versionInt[0] + (float) versionInt[1] * 0.1f;
      else
        Log.MinSpecManager.PrintWarning("The live version string is wrong, using the binary version info instead: " + LiveVersion);
    }
    List<MinSpecManager.MinSpecKind> notEnoughSpecs = new List<MinSpecManager.MinSpecKind>();
    foreach (MinSpecManager.RequirementSpec requirement in this.m_requirements)
    {
      if ((double) requirement.m_systemValue == 0.0)
        Log.MinSpecManager.PrintInfo("Skipped to check because there is no system value of {0}", (object) requirement.m_kind.ToString());
      else if (requirement.m_requirement.Count > 0)
      {
        float requrement = this.GetRequrement(requirement.m_requirement, version);
        if ((double) requrement > (double) requirement.m_systemValue)
        {
          Log.MinSpecManager.PrintInfo("Detected a Minspec warning - {0}: {1} > {2}", (object) requirement.m_kind.ToString(), (object) requrement, (object) requirement.m_systemValue);
          notEnoughSpecs.Add(requirement.m_kind);
        }
      }
    }
    return notEnoughSpecs;
  }

  protected void Initialize(
    float systemRam = 0.0f,
    float systemCPUFreq = 0.0f,
    float systemOSSpec = 0.0f,
    float OpenGLSpec = 0.0f)
  {
    this.m_requirements = new MinSpecManager.RequirementSpec[4]
    {
      new MinSpecManager.RequirementSpec("required_ram", MinSpecManager.MinSpecKind.RAM_SIZE, systemRam),
      new MinSpecManager.RequirementSpec("cpu_freq", MinSpecManager.MinSpecKind.CPU_FREQ, systemCPUFreq),
      new MinSpecManager.RequirementSpec("required_osspecs", MinSpecManager.MinSpecKind.OS_SPEC, systemOSSpec),
      new MinSpecManager.RequirementSpec("required_opengl", MinSpecManager.MinSpecKind.OPENGL_SPEC, OpenGLSpec)
    };
  }

  private float GetRequrement(SortedDictionary<float, float> data, float version)
  {
    float requrement = data.First<KeyValuePair<float, float>>().Value;
    foreach (KeyValuePair<float, float> keyValuePair in data)
    {
      if ((double) keyValuePair.Key <= (double) version)
        requrement = keyValuePair.Value;
    }
    return requrement;
  }

  private static float GetOpenGLVersion()
  {
    Match match = Regex.Match(SystemInfo.graphicsDeviceVersion, "OpenGL\\D*([\\d|\\.]*).*");
    float result;
    return match.Success && float.TryParse(match.Groups[1].Value, out result) ? result : 0.0f;
  }

  private static float GetSystemValue(MinSpecManager.MinSpecKind kind)
  {
    float systemValue = 0.0f;
    try
    {
      switch (kind)
      {
        case MinSpecManager.MinSpecKind.RAM_SIZE:
          systemValue = MobileCallbackManager.GetSystemTotalMemoryMB();
          break;
        case MinSpecManager.MinSpecKind.CPU_FREQ:
          systemValue = (float) SystemInfo.processorFrequency;
          break;
        case MinSpecManager.MinSpecKind.OS_SPEC:
          systemValue = MobileCallbackManager.GetSystemOSSpec();
          break;
        case MinSpecManager.MinSpecKind.OPENGL_SPEC:
          systemValue = MinSpecManager.GetOpenGLVersion();
          break;
      }
    }
    catch (Exception ex)
    {
      Log.MinSpecManager.PrintError("Failed to set the system value of '{0}': {1}", (object) kind.ToString(), (object) ex.Message);
    }
    return systemValue;
  }

  public enum MinSpecKind
  {
    RAM_SIZE,
    CPU_FREQ,
    OS_SPEC,
    OPENGL_SPEC,
    MAX_KIND_SIZE,
  }

  private class RequirementSpec
  {
    public string m_keyName;
    public MinSpecManager.MinSpecKind m_kind;
    public float m_systemValue;
    public SortedDictionary<float, float> m_requirement;

    public RequirementSpec(string key, MinSpecManager.MinSpecKind kind, float defaultVal)
    {
      this.m_keyName = key;
      this.m_kind = kind;
      this.m_requirement = new SortedDictionary<float, float>();
      this.m_systemValue = (double) defaultVal > 0.0 ? defaultVal : MinSpecManager.GetSystemValue(kind);
    }
  }
}
