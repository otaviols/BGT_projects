using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularBundleNodeLayout : MonoBehaviour
{
  public List<ModularBundleNode> Nodes;
  private GeneralStorePacksContentDisplay m_parentDisplay;

  public int LayoutID { get; set; }

  public bool IsAnimating { get; private set; }

  public void Initialize(
    GeneralStorePacksContentDisplay display,
    int layoutId,
    List<ModularBundleLayoutNodeDbfRecord> nodeData)
  {
    this.m_parentDisplay = display;
    this.LayoutID = layoutId;
    if (this.Nodes.Count > nodeData.Count)
      Debug.LogWarningFormat("Node layout {0} has more nodes than there are Node Records in the MODULAR_BUNDLE_LAYOUT dbi.", (object) this.name);
    else if (this.Nodes.Count < nodeData.Count)
      Debug.LogWarningFormat("Node layout {0} has fewer nodes than there are Node Records in the MODULAR_BUNDLE_LAYOUT dbi.", (object) this.name);
    List<int> intList = new List<int>();
    for (int index1 = 0; index1 < nodeData.Count && index1 < this.Nodes.Count; ++index1)
    {
      int index2 = Mathf.Clamp(nodeData[index1].NodeIndex - 1, 0, this.Nodes.Count - 1);
      if ((Object) this.Nodes[index2] == (Object) null)
        Debug.LogErrorFormat("Node layout {0} has unassigned Nodes elements, at index={1}", (object) this.name, (object) index2);
      else if (intList.Contains(index2))
      {
        Debug.LogErrorFormat("Duplicate node index found for layout {0}", (object) this.name);
      }
      else
      {
        intList.Add(index2);
        this.Nodes[index2].Initialize(this, nodeData[index1]);
      }
    }
  }

  public GeneralStorePacksContentDisplay GetDisplay() => this.m_parentDisplay;

  public void PlayEntranceAnimationsInSequence(
    bool forceImmediate,
    ModularBundleNodeLayout.OnModularBundleAnimationsFinished callback,
    object callbackData)
  {
    if (!this.gameObject.activeInHierarchy)
      return;
    this.StartCoroutine(this.PlayEntranceAnimationsInSequenceCoroutine(forceImmediate, callback, callbackData));
  }

  private IEnumerator PlayEntranceAnimationsInSequenceCoroutine(
    bool forceImmediate,
    ModularBundleNodeLayout.OnModularBundleAnimationsFinished callback,
    object callbackData)
  {
    this.IsAnimating = true;
    foreach (ModularBundleNode node in this.Nodes)
    {
      if (!node.IsReady())
        yield return (object) null;
    }
    foreach (ModularBundleNode node1 in this.Nodes)
    {
      ModularBundleNode node = node1;
      if (forceImmediate)
      {
        node.EnterImmediately();
      }
      else
      {
        yield return (object) new WaitForSeconds(node.DelayBeforeEntryAnimation);
        node.PlayEntryAnimation();
        node = (ModularBundleNode) null;
      }
    }
    this.IsAnimating = false;
    callback(callbackData);
  }

  public void PlayExitAnimationsInSequence(
    bool forceImmediate,
    ModularBundleNodeLayout.OnModularBundleAnimationsFinished callback,
    object callbackData)
  {
    if (!this.gameObject.activeInHierarchy)
      return;
    this.StartCoroutine(this.PlayExitAnimationsInSequenceCoroutine(forceImmediate, callback, callbackData));
  }

  private IEnumerator PlayExitAnimationsInSequenceCoroutine(
    bool forceImmediate,
    ModularBundleNodeLayout.OnModularBundleAnimationsFinished callback,
    object callbackData)
  {
    this.IsAnimating = true;
    foreach (ModularBundleNode node in this.Nodes)
    {
      if (!node.IsReady())
        yield return (object) null;
    }
    for (int i = this.Nodes.Count - 1; i >= 0; --i)
    {
      if (forceImmediate)
      {
        this.Nodes[i].ExitImmediately();
      }
      else
      {
        yield return (object) new WaitForSeconds(this.Nodes[i].DelayBeforeEntryAnimation);
        this.Nodes[i].PlayExitAnimation();
      }
    }
    this.IsAnimating = false;
    callback(callbackData);
  }

  public delegate void OnModularBundleAnimationsFinished(object callbackData);

  public struct NodeCallbackData
  {
    public int layoutId;
    public List<ModularBundleLayoutNodeDbfRecord> layoutNodes;
    public string prefab;
    public bool forceImmediate;

    public NodeCallbackData(
      int layoutId,
      List<ModularBundleLayoutNodeDbfRecord> layoutNodes,
      string prefab,
      bool forceImmediate)
    {
      this.layoutId = layoutId;
      this.layoutNodes = layoutNodes;
      this.prefab = prefab;
      this.forceImmediate = forceImmediate;
    }
  }
}
