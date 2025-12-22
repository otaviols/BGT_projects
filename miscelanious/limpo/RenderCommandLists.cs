using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderCommandLists
{
  public List<RenderCommand> OpaqueRenderCommands;
  public List<RenderCommand> TransparentRenderCommands;
  private int[] foundPasses = new int[3];
  private List<Material> matPool = new List<Material>(5);
  private static ShaderTagId s_lightModeTag = new ShaderTagId("LightMode");
  private static ShaderTagId s_defaultLightModeTag = new ShaderTagId("SRPDefaultUnlit");
  private static ShaderTagId[] s_lightModesToInclude = new ShaderTagId[3]
  {
    new ShaderTagId("UniversalForward"),
    new ShaderTagId("LightweightForward"),
    new ShaderTagId("SRPDefaultUnlit")
  };
  private static ProfilerMarker s_RTTCreateCommands = new ProfilerMarker("RTT_CreateCommands");

  public RenderCommandLists()
  {
    this.OpaqueRenderCommands = new List<RenderCommand>();
    this.TransparentRenderCommands = new List<RenderCommand>();
  }

  public void Clear()
  {
    this.OpaqueRenderCommands.Clear();
    this.TransparentRenderCommands.Clear();
  }

  private int SortRenderCommands(RenderCommand a, RenderCommand b) => a.Renderer.sortingOrder == b.Renderer.sortingOrder ? a.Material.renderQueue - b.Material.renderQueue : a.Renderer.sortingOrder - b.Renderer.sortingOrder;

  public void AppendRenderCommands(
    GameObject objectToDraw,
    bool includeInactiveRenderers = false,
    RenderCommandLists.MatOverrideDictionary overrides = null)
  {
    this.AppendRenderCommands(objectToDraw.GetComponentsInChildren<Renderer>(includeInactiveRenderers), overrides);
  }

  public void AppendRenderCommands(
    Renderer[] toDraw,
    RenderCommandLists.MatOverrideDictionary overrides = null)
  {
    foreach (Renderer renderer in toDraw)
    {
      List<RenderCommandLists.MaterialOveride> materialOverideList = (List<RenderCommandLists.MaterialOveride>) null;
      bool flag1 = false;
      if (overrides != null)
      {
        overrides.TryGetValue(renderer, out materialOverideList);
        if (materialOverideList != null)
        {
          foreach (RenderCommandLists.MaterialOveride materialOveride in materialOverideList)
          {
            if (materialOveride.meshIndex == -1)
            {
              flag1 = true;
              if (materialOverideList.Count > 1)
              {
                Debug.LogError((object) "Multiple overrides passed when a global override is active. This is not supported");
                break;
              }
              break;
            }
          }
        }
      }
      if (flag1)
        this.matPool.Add(materialOverideList[0].materialToUse);
      else if (renderer.HasCustomMaterials())
      {
        renderer.GetMaterialsToExistingList(this.matPool);
      }
      else
      {
        renderer.GetSharedMaterials(this.matPool);
        foreach (UnityEngine.Object @object in this.matPool)
        {
          if (@object == (UnityEngine.Object) null)
          {
            this.matPool.Clear();
            renderer.GetMaterialsToExistingList(this.matPool);
            break;
          }
        }
      }
      List<Material> matPool = this.matPool;
      MeshRenderer meshRenderer = renderer as MeshRenderer;
      int num1 = 1;
      if ((bool) (UnityEngine.Object) meshRenderer)
      {
        bool flag2 = false;
        MeshFilter component1 = (MeshFilter) null;
        TextMesh component2 = (TextMesh) null;
        int num2 = renderer.TryGetComponent<MeshFilter>(out component1) ? 1 : 0;
        if (num2 == 0)
          flag2 = renderer.TryGetComponent<TextMesh>(out component2);
        if (num2 != 0 && (bool) (UnityEngine.Object) component1.sharedMesh)
          num1 = component1.sharedMesh.subMeshCount;
        else if (flag2)
          num1 = matPool.Count;
      }
      else
      {
        SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
        if ((bool) (UnityEngine.Object) skinnedMeshRenderer)
          num1 = skinnedMeshRenderer.sharedMesh.subMeshCount;
      }
      for (int index1 = 0; index1 < num1; ++index1)
      {
        int index2 = index1;
        if (index2 >= matPool.Count)
          index2 = 0;
        Material materialToUse = matPool[index2];
        if (!flag1 && materialOverideList != null)
        {
          foreach (RenderCommandLists.MaterialOveride materialOveride in materialOverideList)
          {
            if (materialOveride.meshIndex == index1)
            {
              materialToUse = materialOveride.materialToUse;
              break;
            }
          }
        }
        for (int index3 = 0; index3 < this.foundPasses.Length; ++index3)
          this.foundPasses[index3] = -1;
        int num3 = 0;
        for (int passIndex = 0; passIndex < materialToUse.passCount; ++passIndex)
        {
          ShaderTagId shaderTagId = materialToUse.shader.FindPassTagValue(passIndex, RenderCommandLists.s_lightModeTag);
          if (shaderTagId == ShaderTagId.none)
            shaderTagId = RenderCommandLists.s_defaultLightModeTag;
          for (int index4 = 0; index4 < RenderCommandLists.s_lightModesToInclude.Length; ++index4)
          {
            if (shaderTagId == RenderCommandLists.s_lightModesToInclude[index4] && this.foundPasses[index4] == -1)
            {
              this.foundPasses[index4] = passIndex;
              ++num3;
              break;
            }
          }
          if (num3 == this.foundPasses.Length)
            break;
        }
        RenderCommand renderCommand1;
        if (materialToUse.renderQueue < 3000)
        {
          foreach (int foundPass in this.foundPasses)
          {
            if (foundPass != -1)
            {
              List<RenderCommand> opaqueRenderCommands = this.OpaqueRenderCommands;
              renderCommand1 = new RenderCommand();
              renderCommand1.Renderer = renderer;
              renderCommand1.Material = materialToUse;
              renderCommand1.MeshIndex = index1;
              renderCommand1.passIndex = foundPass;
              RenderCommand renderCommand2 = renderCommand1;
              opaqueRenderCommands.Add(renderCommand2);
            }
          }
        }
        else
        {
          foreach (int foundPass in this.foundPasses)
          {
            if (foundPass != -1)
            {
              List<RenderCommand> transparentRenderCommands = this.TransparentRenderCommands;
              renderCommand1 = new RenderCommand();
              renderCommand1.Renderer = renderer;
              renderCommand1.Material = materialToUse;
              renderCommand1.MeshIndex = index1;
              renderCommand1.passIndex = foundPass;
              RenderCommand renderCommand3 = renderCommand1;
              transparentRenderCommands.Add(renderCommand3);
            }
          }
        }
      }
      this.matPool.Clear();
    }
    this.OpaqueRenderCommands.Sort(new Comparison<RenderCommand>(this.SortRenderCommands));
    this.TransparentRenderCommands.Sort(new Comparison<RenderCommand>(this.SortRenderCommands));
  }

  public struct MaterialOveride
  {
    public int meshIndex;
    public Material materialToUse;

    public MaterialOveride(Material toUse, int meshIdx = -1)
    {
      this.materialToUse = toUse;
      this.meshIndex = meshIdx;
    }
  }

  public class MatOverrideDictionary : Dictionary<Renderer, List<RenderCommandLists.MaterialOveride>>
  {
    public void Add(Renderer key, RenderCommandLists.MaterialOveride matOverride)
    {
      List<RenderCommandLists.MaterialOveride> materialOverideList;
      if (this.TryGetValue(key, out materialOverideList))
        materialOverideList.Add(matOverride);
      else
        this.Add(key, new List<RenderCommandLists.MaterialOveride>(1)
        {
          matOverride
        });
    }
  }
}
