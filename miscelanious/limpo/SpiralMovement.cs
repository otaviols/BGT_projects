using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpiralMovement : MonoBehaviour
{
  public GameObject[] parts;
  public float partFlytimeMul = 1f;
  public AnimationCurve partFlytime;
  public float partSpawnPointCount = 3f;
  public float partSpawnPointOffset = 2f;
  public float partSpawnLifetime = 0.5f;
  public float partsHideTime = 2f;
  public float partSpawnPointRotSpeed = 40f;
  public float spawnY = 1f;
  public AnimationCurve magnetTimeRemap;
  public AnimationCurve partRotTimeRemap;
  public AnimationCurve partScaleAnimation;
  [SerializeField]
  public SpiralMovement.MaterialParam[] materialParams;
  private float time;
  private List<SpiralMovement.Part> partsP;
  private float partsCount;
  private Material partMaterial;
  private SpiralMovement.States state;

  public void ResetAnim()
  {
    this.partsCount = (float) this.parts.Length;
    if ((double) this.partsCount == 0.0)
      return;
    this.partMaterial = this.parts[0].GetComponent<MeshRenderer>().GetSharedMaterial();
    if (this.partsP != null)
      this.partsP.Clear();
    else
      this.partsP = new List<SpiralMovement.Part>();
    for (int index = 0; (double) index < (double) this.partsCount; ++index)
    {
      float _delay = (float) index / this.partsCount * this.partSpawnLifetime;
      float _spawnPointRot = (float) ((double) Mathf.Floor(UnityEngine.Random.Range(0.0f, this.partSpawnPointCount - float.Epsilon)) * 3.14159274101257 * 2.0) / this.partSpawnPointCount;
      this.partsP.Add(new SpiralMovement.Part(this.parts[index], this.partFlytime.Evaluate((float) index / this.partsCount) * this.partFlytimeMul, this.partSpawnPointRotSpeed, _spawnPointRot, this.partSpawnPointOffset, this.spawnY, this.partMaterial, _delay, this.magnetTimeRemap, this.partRotTimeRemap, this.partScaleAnimation));
    }
    this.time = 0.0f;
    this.state = SpiralMovement.States.UPDATE;
  }

  private void Update()
  {
    if (this.state == SpiralMovement.States.HIDE || this.partsP == null)
      return;
    foreach (SpiralMovement.Part part in this.partsP)
      part.Update(Time.deltaTime);
    if (this.state == SpiralMovement.States.UPDATE)
    {
      if ((double) this.time < (double) this.partsHideTime)
      {
        foreach (SpiralMovement.MaterialParam materialParam in this.materialParams)
          this.partMaterial.SetFloat(materialParam.paramName, materialParam.curveAnimation.Evaluate(this.time * materialParam.timeMul) * materialParam.valueMul);
      }
      else
      {
        this.state = SpiralMovement.States.HIDE;
        using (List<SpiralMovement.Part>.Enumerator enumerator = this.partsP.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.Hide();
          return;
        }
      }
    }
    this.time += Time.deltaTime;
  }

  private enum States
  {
    START,
    UPDATE,
    HIDE,
  }

  [Serializable]
  public class MaterialParam
  {
    [SerializeField]
    public string paramName;
    [SerializeField]
    public float valueMul = 1f;
    [SerializeField]
    public float timeMul = 1f;
    [SerializeField]
    public AnimationCurve curveAnimation;
  }

  private class Part
  {
    private float lifetime;
    private float time;
    private Vector3 endPos;
    private Quaternion endRot = Quaternion.identity;
    private Vector3 endScale;
    private Vector3 pos;
    private Quaternion rot;
    private Vector3 scale;
    private float spawnPointRotSpeed;
    private GameObject go;
    private Transform goTransform;
    private MeshRenderer goMeshRenderer;
    private SpiralMovement.Part.PartStates partState;
    private float delay;
    private float spawnPointIniRot;
    private float partSpawnPointOffset;
    private float currentAngle;
    private float spawnY;
    private AnimationCurve scaleAnimCurve;
    private AnimationCurve magnetTimeRemap;
    private AnimationCurve rotTimeRemap;

    public Part(
      GameObject _go,
      float _lifetime,
      float _spawnPointRotSpeed,
      float _spawnPointRot,
      float _partSpawnPointOffset,
      float _spawnY,
      Material _sharedMaterial,
      float _delay,
      AnimationCurve _magnetTimeRemap,
      AnimationCurve _rotTimeRemap,
      AnimationCurve _scaleAnimCurve)
    {
      this.lifetime = _lifetime;
      this.go = _go;
      this.goTransform = this.go.transform;
      this.goMeshRenderer = _go.GetComponent<MeshRenderer>();
      this.endPos = this.goTransform.localPosition;
      this.spawnPointIniRot = _spawnPointRot;
      this.partSpawnPointOffset = _partSpawnPointOffset;
      this.spawnPointRotSpeed = _spawnPointRotSpeed;
      this.goMeshRenderer.SetSharedMaterial(_sharedMaterial);
      this.delay = _delay;
      this.goMeshRenderer.enabled = false;
      this.currentAngle = 0.0f;
      this.spawnY = _spawnY;
      this.endScale = this.goTransform.localScale;
      this.scaleAnimCurve = _scaleAnimCurve;
      this.magnetTimeRemap = _magnetTimeRemap;
      this.rotTimeRemap = _rotTimeRemap;
    }

    public void Update(float _deltatime)
    {
      if (this.partState == SpiralMovement.Part.PartStates.END)
        return;
      if (this.partState == SpiralMovement.Part.PartStates.DELAY && (double) this.time > (double) this.delay)
      {
        this.time -= this.delay;
        this.goMeshRenderer.enabled = true;
        this.partState = SpiralMovement.Part.PartStates.UPDATE;
      }
      if (this.partState == SpiralMovement.Part.PartStates.UPDATE)
      {
        if ((double) this.time < (double) this.lifetime)
        {
          float time = this.time / this.lifetime;
          float num = this.magnetTimeRemap.Evaluate(time);
          this.currentAngle = this.spawnPointRotSpeed * this.rotTimeRemap.Evaluate(time) + this.spawnPointIniRot;
          this.goTransform.localPosition = Vector3.Lerp(new Vector3(Mathf.Cos(this.currentAngle) * this.partSpawnPointOffset, this.spawnY, Mathf.Sin(this.currentAngle) * this.partSpawnPointOffset), this.endPos, num);
          this.goTransform.localScale = this.endScale * this.scaleAnimCurve.Evaluate(num);
        }
        else
        {
          this.partState = SpiralMovement.Part.PartStates.END;
          this.goTransform.localRotation = this.endRot;
          this.goTransform.localPosition = this.endPos;
          this.goTransform.localScale = this.endScale;
          return;
        }
      }
      this.time += _deltatime;
    }

    public void Hide()
    {
      this.goMeshRenderer.enabled = false;
      this.partState = SpiralMovement.Part.PartStates.END;
    }

    private enum PartStates
    {
      DELAY,
      UPDATE,
      END,
    }
  }
}
