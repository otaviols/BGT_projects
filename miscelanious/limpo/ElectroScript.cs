using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using UnityEngine;

public class ElectroScript : MonoBehaviour
{
  public ElectroScript.Prefabs prefabs;
  public ElectroScript.Timers timers;
  public ElectroScript.Dynamics dynamics;
  public ElectroScript.LineSettings lines;
  public ElectroScript.TextureSettings tex;
  private int numVertices;
  private Vector3 deltaV1;
  private Vector3 deltaV2;
  private float srcTrgDist;
  private float srcDstDist;
  private float lastUpdate;
  private Hashtable branches;

  private void Start()
  {
    this.srcTrgDist = 0.0f;
    this.srcDstDist = 0.0f;
    this.numVertices = 0;
    this.deltaV1 = this.prefabs.destination.position - this.prefabs.source.position;
    this.lastUpdate = 0.0f;
    this.branches = new Hashtable();
  }

  private void Update()
  {
    this.srcTrgDist = Vector3.Distance(this.prefabs.source.position, this.prefabs.target.position);
    this.srcDstDist = Vector3.Distance(this.prefabs.source.position, this.prefabs.destination.position);
    if (this.branches.Count > 0)
    {
      foreach (int key in (IEnumerable) ((Hashtable) this.branches.Clone()).Keys)
      {
        LineRenderer branch = (LineRenderer) this.branches[(object) key];
        if ((double) branch.GetComponent<BranchScript>().timeSpawned + (double) this.timers.branchLife < (double) Time.time)
        {
          this.branches.Remove((object) key);
          UnityEngine.Object.Destroy((UnityEngine.Object) branch.gameObject);
        }
      }
    }
    if (this.prefabs.target.localPosition != this.prefabs.destination.localPosition)
    {
      if ((double) Vector3.Distance(Vector3.zero, this.deltaV1) * (double) Time.deltaTime * (1.0 / (double) this.timers.timeToPowerUp) > (double) Vector3.Distance(this.prefabs.target.position, this.prefabs.destination.position))
        this.prefabs.target.position = this.prefabs.destination.position;
      else
        this.prefabs.target.Translate(this.deltaV1 * Time.deltaTime * (1f / this.timers.timeToPowerUp));
    }
    if ((double) Time.time - (double) this.lastUpdate < (double) this.timers.timeToUpdate)
      return;
    this.lastUpdate = Time.time;
    this.AnimateArc();
    this.DrawArc();
    this.RayCast();
  }

  private void DrawArc()
  {
    this.numVertices = Mathf.RoundToInt(this.srcTrgDist / this.lines.keyVertexDist) * (1 + this.lines.numInterpoles) + 1;
    this.deltaV2 = (this.prefabs.target.localPosition - this.prefabs.source.localPosition) / (float) this.numVertices;
    Vector3 localPosition1 = this.prefabs.source.localPosition;
    Vector3[] vector3Array1 = new Vector3[this.numVertices];
    this.prefabs.lightning.positionCount = this.numVertices;
    for (int index1 = 0; index1 < this.numVertices; index1 = index1 + this.lines.numInterpoles + 1)
    {
      Vector3 position1 = localPosition1;
      position1.y += (float) ((double) UnityEngine.Random.value * 2.0 - 1.0) * this.lines.keyVertexRange;
      position1.z += (float) ((double) UnityEngine.Random.value * 2.0 - 1.0) * this.lines.keyVertexRange;
      this.prefabs.lightning.SetPosition(index1, position1);
      vector3Array1[index1] = position1;
      if (!this.branches.ContainsKey((object) index1))
      {
        if ((double) UnityEngine.Random.value < (double) this.dynamics.chanceToArc)
        {
          LineRenderer lineRenderer = UnityEngine.Object.Instantiate<LineRenderer>(this.prefabs.branch, Vector3.zero, Quaternion.identity);
          lineRenderer.GetComponent<BranchScript>().timeSpawned = Time.time;
          lineRenderer.transform.parent = this.prefabs.lightning.transform;
          this.branches.Add((object) index1, (object) lineRenderer);
          lineRenderer.transform.position = this.prefabs.lightning.transform.TransformPoint(position1);
          position1.x = UnityEngine.Random.value - 0.5f;
          position1.y = (float) (((double) UnityEngine.Random.value - 0.5) * 2.0);
          position1.z = (float) (((double) UnityEngine.Random.value - 0.5) * 2.0);
          lineRenderer.transform.LookAt(lineRenderer.transform.TransformPoint(position1));
          lineRenderer.transform.Find("stop").localPosition = lineRenderer.transform.Find("start").localPosition + new Vector3(0.0f, 0.0f, UnityEngine.Random.Range(this.lines.minBranchLength, this.lines.maxBranchLength));
          int length = Mathf.RoundToInt(Vector3.Distance(lineRenderer.transform.Find("start").position, lineRenderer.transform.Find("stop").position) / this.lines.keyVertexDist) * (1 + this.lines.numInterpoles) + 1;
          Vector3 vector3 = (lineRenderer.transform.Find("stop").localPosition - lineRenderer.transform.Find("start").localPosition) / (float) length;
          Vector3 localPosition2 = lineRenderer.transform.Find("start").localPosition;
          Vector3[] vector3Array2 = new Vector3[length];
          lineRenderer.positionCount = length;
          for (int index2 = 0; index2 < length; index2 = index2 + this.lines.numInterpoles + 1)
          {
            Vector3 position2 = localPosition2;
            position2.x += (float) ((double) UnityEngine.Random.value * 2.0 - 1.0) * this.lines.keyVertexRange;
            position2.y += (float) ((double) UnityEngine.Random.value * 2.0 - 1.0) * this.lines.keyVertexRange;
            lineRenderer.SetPosition(index2, position2);
            vector3Array2[index2] = position2;
            localPosition2 += vector3 * (float) (this.lines.numInterpoles + 1);
          }
          lineRenderer.SetPosition(0, lineRenderer.transform.Find("start").localPosition);
          lineRenderer.SetPosition(length - 1, lineRenderer.transform.Find("stop").localPosition);
          for (int index3 = 0; index3 < length; ++index3)
          {
            if (index3 % (this.lines.numInterpoles + 1) != 0)
            {
              Vector3 a = vector3Array2[index3 - 1];
              Vector3 b = vector3Array2[index3 + this.lines.numInterpoles];
              float num = (float) ((double) Vector3.Distance(a, b) / (double) (this.lines.numInterpoles + 1) / (double) Vector3.Distance(a, b) * 3.14159274101257);
              for (int index4 = 0; index4 < this.lines.numInterpoles; ++index4)
              {
                Vector3 position3;
                position3.x = a.x + vector3.x * (float) (1 + index4);
                position3.y = a.y + (float) (((double) Mathf.Sin(num - 1.570796f) / 2.0 + 0.5) * ((double) b.y - (double) a.y));
                position3.z = a.z + (float) (((double) Mathf.Sin(num - 1.570796f) / 2.0 + 0.5) * ((double) b.z - (double) a.z));
                lineRenderer.SetPosition(index3 + index4, position3);
                num += num;
              }
              index3 += this.lines.numInterpoles;
            }
          }
        }
      }
      else
      {
        LineRenderer branch = (LineRenderer) this.branches[(object) index1];
        int length = Mathf.RoundToInt(Vector3.Distance(branch.transform.Find("start").position, branch.transform.Find("stop").position) / this.lines.keyVertexDist) * (1 + this.lines.numInterpoles) + 1;
        Vector3 vector3 = (branch.transform.Find("stop").localPosition - branch.transform.Find("start").localPosition) / (float) length;
        Vector3 localPosition3 = branch.transform.Find("start").localPosition;
        Vector3[] vector3Array3 = new Vector3[length];
        branch.positionCount = length;
        branch.SetPosition(0, branch.transform.Find("start").localPosition);
        for (int index5 = 0; index5 < length; index5 = index5 + this.lines.numInterpoles + 1)
        {
          Vector3 position4 = localPosition3;
          position4.x += (float) ((double) UnityEngine.Random.value * 2.0 - 1.0) * this.lines.keyVertexRange;
          position4.y += (float) ((double) UnityEngine.Random.value * 2.0 - 1.0) * this.lines.keyVertexRange;
          branch.SetPosition(index5, position4);
          vector3Array3[index5] = position4;
          localPosition3 += vector3 * (float) (this.lines.numInterpoles + 1);
        }
        branch.SetPosition(0, branch.transform.Find("start").localPosition);
        branch.SetPosition(length - 1, branch.transform.Find("stop").localPosition);
        for (int index6 = 0; index6 < length; ++index6)
        {
          if (index6 % (this.lines.numInterpoles + 1) != 0)
          {
            Vector3 a = vector3Array3[index6 - 1];
            Vector3 b = vector3Array3[index6 + this.lines.numInterpoles];
            float num = (float) ((double) Vector3.Distance(a, b) / (double) (this.lines.numInterpoles + 1) / (double) Vector3.Distance(a, b) * 3.14159274101257);
            for (int index7 = 0; index7 < this.lines.numInterpoles; ++index7)
            {
              Vector3 position5;
              position5.x = a.x + vector3.x * (float) (1 + index7);
              position5.y = a.y + (float) (((double) Mathf.Sin(num - 1.570796f) / 2.0 + 0.5) * ((double) b.y - (double) a.y));
              position5.z = a.z + (float) (((double) Mathf.Sin(num - 1.570796f) / 2.0 + 0.5) * ((double) b.z - (double) a.z));
              branch.SetPosition(index6 + index7, position5);
              num += num;
            }
            index6 += this.lines.numInterpoles;
          }
        }
      }
      localPosition1 += this.deltaV2 * (float) (this.lines.numInterpoles + 1);
    }
    this.prefabs.lightning.SetPosition(0, this.prefabs.source.localPosition);
    this.prefabs.lightning.SetPosition(this.numVertices - 1, this.prefabs.target.localPosition);
    for (int index8 = 0; index8 < this.numVertices; ++index8)
    {
      if (index8 % (this.lines.numInterpoles + 1) != 0)
      {
        Vector3 a = vector3Array1[index8 - 1];
        Vector3 b = vector3Array1[index8 + this.lines.numInterpoles];
        float num = (float) ((double) Vector3.Distance(a, b) / (double) (this.lines.numInterpoles + 1) / (double) Vector3.Distance(a, b) * 3.14159274101257);
        for (int index9 = 0; index9 < this.lines.numInterpoles; ++index9)
        {
          Vector3 position;
          position.x = a.x + this.deltaV2.x * (float) (1 + index9);
          position.y = a.y + (float) (((double) Mathf.Sin(num - 1.570796f) / 2.0 + 0.5) * ((double) b.y - (double) a.y));
          position.z = a.z + (float) (((double) Mathf.Sin(num - 1.570796f) / 2.0 + 0.5) * ((double) b.z - (double) a.z));
          this.prefabs.lightning.SetPosition(index8 + index9, position);
          num += num;
        }
        index8 += this.lines.numInterpoles;
      }
    }
  }

  private void AnimateArc()
  {
    Material material = this.prefabs.lightning.GetComponent<Renderer>().GetMaterial();
    Vector2 mainTextureOffset = material.mainTextureOffset;
    Vector2 mainTextureScale = material.mainTextureScale;
    mainTextureOffset.x += Time.deltaTime * this.tex.animateSpeed;
    mainTextureOffset.y = this.tex.offsetY;
    mainTextureScale.x = this.srcTrgDist / this.srcDstDist * this.tex.scaleX;
    mainTextureScale.y = this.tex.scaleY;
    material.mainTextureOffset = mainTextureOffset;
    material.mainTextureScale = mainTextureScale;
  }

  private void RayCast()
  {
    foreach (RaycastHit raycastHit in Physics.RaycastAll(this.prefabs.source.position, this.prefabs.target.position - this.prefabs.source.position, Vector3.Distance(this.prefabs.source.position, this.prefabs.target.position)))
      UnityEngine.Object.Instantiate<Transform>(this.prefabs.sparks, raycastHit.point, Quaternion.identity);
    if (this.branches.Count <= 0)
      return;
    foreach (int key in (IEnumerable) ((Hashtable) this.branches.Clone()).Keys)
    {
      LineRenderer branch = (LineRenderer) this.branches[(object) key];
      foreach (RaycastHit raycastHit in Physics.RaycastAll(branch.transform.Find("start").position, branch.transform.Find("stop").position - branch.transform.Find("start").position, Vector3.Distance(branch.transform.Find("start").position, branch.transform.Find("stop").position)))
        UnityEngine.Object.Instantiate<Transform>(this.prefabs.sparks, raycastHit.point, Quaternion.identity);
    }
  }

  [Serializable]
  public class Prefabs
  {
    public LineRenderer lightning;
    public LineRenderer branch;
    public Transform sparks;
    public Transform source;
    public Transform destination;
    public Transform target;
  }

  [Serializable]
  public class Timers
  {
    public float timeToUpdate = 0.05f;
    public float timeToPowerUp = 0.5f;
    public float branchLife = 0.1f;
  }

  [Serializable]
  public class Dynamics
  {
    public float chanceToArc = 0.2f;
  }

  [Serializable]
  public class LineSettings
  {
    public float keyVertexDist = 3f;
    public float keyVertexRange = 4f;
    public int numInterpoles = 5;
    public float minBranchLength = 11f;
    public float maxBranchLength = 16f;
  }

  [Serializable]
  public class TextureSettings
  {
    public float scaleX;
    public float scaleY;
    public float animateSpeed;
    public float offsetY;
  }
}
