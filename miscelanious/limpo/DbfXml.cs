using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class DbfXml
{
  public static bool Load<T>(string xmlFile, Dbf<T> dbf) where T : DbfRecord, new()
  {
    if (!File.Exists(xmlFile))
      return false;
    using (XmlReader xmlReader = XmlReader.Create(xmlFile))
    {
      while (xmlReader.Read())
      {
        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Record")
          DbfXml.LoadRecord<T>(xmlReader.ReadSubtree(), dbf);
      }
      return true;
    }
  }

  public static IEnumerator<IAsyncJobResult> Job_LoadAsync<T>(
    string xmlFile,
    Dbf<T> dbf)
    where T : DbfRecord, new()
  {
    string name = dbf.GetName();
    Dbf<T> threadSafeDBF = new Dbf<T>(name);
    yield return (IAsyncJobResult) new JobDefinition(string.Format("DbfXml.LoadAsyncFromDisk[{0}]", (object) name), DbfXml.Job_LoadAsyncFromDisk<T>(xmlFile, threadSafeDBF), JobFlags.StartImmediately | JobFlags.UseWorkerThread, Array.Empty<IJobDependency>());
    lock (threadSafeDBF)
      dbf.CopyRecords(threadSafeDBF);
  }

  public static IEnumerator<IAsyncJobResult> Job_LoadAsyncFromDisk<T>(
    string xmlFile,
    Dbf<T> dbf)
    where T : DbfRecord, new()
  {
    lock (dbf)
    {
      DbfXml.Load<T>(xmlFile, dbf);
      yield break;
    }
  }

  public static void LoadRecord<T>(XmlReader reader, Dbf<T> dbf, bool hideDbfLocDebugInfo = false) where T : DbfRecord, new()
  {
    DbfRecord newRecord = dbf.CreateNewRecord();
    while (reader.Read())
    {
      if (reader.NodeType == XmlNodeType.Element && !(reader.Name != "Field") && !reader.IsEmptyElement)
      {
        string varName = reader["column"];
        System.Type varType = newRecord.GetVarType(varName);
        if (varType != (System.Type) null)
        {
          try
          {
            if (varType == typeof (DbfLocValue))
              newRecord.SetVar(varName, (object) DbfXml.LoadLocalizedString(reader["loc_ID"], reader.ReadSubtree(), hideDbfLocDebugInfo));
            else if (varType == typeof (bool))
            {
              string strVal = reader.ReadElementContentAsString();
              newRecord.SetVar(varName, (object) GeneralUtils.ForceBool(strVal));
            }
            else if (varType.IsEnum)
            {
              string s = reader.ReadElementContentAs(typeof (string), (IXmlNamespaceResolver) null) as string;
              System.Type underlyingType = Enum.GetUnderlyingType(varType);
              if (underlyingType == typeof (int))
              {
                int result;
                if (int.TryParse(s, out result))
                {
                  newRecord.SetVar(varName, (object) result);
                  continue;
                }
              }
              else if (underlyingType == typeof (uint))
              {
                uint result;
                if (uint.TryParse(s, out result))
                {
                  newRecord.SetVar(varName, (object) result);
                  continue;
                }
              }
              else if (underlyingType == typeof (long))
              {
                long result;
                if (long.TryParse(s, out result))
                {
                  newRecord.SetVar(varName, (object) result);
                  continue;
                }
              }
              else
              {
                ulong result;
                if (underlyingType == typeof (ulong) && ulong.TryParse(s, out result))
                {
                  newRecord.SetVar(varName, (object) result);
                  continue;
                }
              }
              newRecord.SetVar(varName, (object) s);
            }
            else if (varType == typeof (ulong))
              newRecord.SetVar(varName, (object) ulong.Parse(reader.ReadElementContentAsString()));
            else
              newRecord.SetVar(varName, reader.ReadElementContentAs(varType, (IXmlNamespaceResolver) null));
          }
          catch (Exception ex)
          {
            Debug.LogErrorFormat("Failed to read record id={0} column={1} with varType={2} exception={3}", (object) newRecord.ID, (object) varName, (object) varType, (object) ex.ToString());
            throw;
          }
        }
        else
          Debug.LogErrorFormat("Type is not defined for column {0}, dbf={1}. Try \"Build->Generate DBFs and Code\"", (object) varName, (object) newRecord.GetType().Name);
      }
    }
    dbf.AddRecord(newRecord);
  }

  public static DbfLocValue LoadLocalizedString(
    string locIdStr,
    XmlReader reader,
    bool hideDebugInfo = false)
  {
    reader.Read();
    DbfLocValue dbfLocValue = new DbfLocValue(hideDebugInfo);
    if (!string.IsNullOrEmpty(locIdStr))
    {
      int result = 0;
      if (int.TryParse(locIdStr, out result))
        dbfLocValue.SetLocId(result);
    }
    while (reader.Read())
    {
      if (reader.NodeType == XmlNodeType.Element)
      {
        string name = reader.Name;
        string text = reader.ReadElementContentAsString();
        Locale loc;
        try
        {
          loc = EnumUtils.GetEnum<Locale>(name);
        }
        catch (ArgumentException ex)
        {
          continue;
        }
        dbfLocValue.SetString(loc, TextUtils.DecodeWhitespaces(text));
      }
    }
    return dbfLocValue;
  }
}
