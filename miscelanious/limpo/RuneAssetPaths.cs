using PegasusShared;
using System;
using UnityEngine;

[Serializable]
public class RuneAssetPaths
{
  public RuneType m_assetRuneType;
  [CustomEditField(T = EditType.MATERIAL)]
  public Material m_runeMaterial;
  [CustomEditField(T = EditType.MATERIAL)]
  public Material m_runeMaterialHighlighted;
  [CustomEditField(T = EditType.MATERIAL)]
  public Material m_runeMaterialGhosted;
}
