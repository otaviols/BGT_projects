using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class StageTransistion : MonoBehaviour
{
  public GameObject hlBase;
  public GameObject hlEdge;
  public GameObject entireObj;
  public GameObject inplayObj;
  public GameObject rays;
  public GameObject flash;
  public GameObject fxEmitterA;
  public GameObject fxEmitterB;
  public float FxEmitterAKillTime = 1f;
  private Shader shaderBucket;
  private bool colorchange;
  private bool powerchange;
  private bool amountchange;
  private bool turnon;
  private bool rayschange;
  private bool flashchange;
  public Color endColor;
  public Color flashendColor;
  private int stage;
  public float RayTime = 10f;
  public float fxATime = 1f;
  public float FxEmitterAWaitTime = 1f;
  public float FxEmitterATimer = 2f;
  private bool FxStartAnim;
  private bool FxStartStop;
  private bool fxEmitterAScale;
  private bool raysdone;
  private Renderer m_hlBaseRenderer;
  private Renderer hlEdgeRenderer;

  private void Start()
  {
    this.rays.SetActive(false);
    this.flash.SetActive(false);
    this.entireObj.SetActive(true);
    this.inplayObj.SetActive(false);
    this.m_hlBaseRenderer = this.hlBase.GetComponent<Renderer>();
    this.hlEdgeRenderer = this.hlEdge.GetComponent<Renderer>();
    this.m_hlBaseRenderer.GetMaterial().SetFloat("_Amount", 0.0f);
    this.hlEdgeRenderer.GetMaterial().SetFloat("_Amount", 0.0f);
  }

  private void OnGUI()
  {
    if (!UnityEngine.Event.current.isKey)
      return;
    this.amountchange = true;
  }

  private void OnMouseEnter()
  {
    if (this.FxStartAnim)
      return;
    this.FxStartStop = false;
    this.FxStartAnim = true;
    this.powerchange = true;
    this.fxEmitterAScale = true;
  }

  private void OnMouseExit()
  {
    if (this.FxStartStop)
      return;
    this.FxStartAnim = false;
    this.FxStartStop = true;
  }

  private void OnMouseDown()
  {
    switch (this.stage)
    {
      case 0:
        this.ManaUse();
        break;
      case 1:
        this.RaysOn();
        break;
    }
    ++this.stage;
  }

  private void RaysOn()
  {
    this.rays.SetActive(true);
    this.flash.SetActive(true);
    this.rayschange = true;
  }

  private void ManaUse() => this.colorchange = true;

  private void Update()
  {
    Material material1 = this.hlEdgeRenderer.GetMaterial();
    Material material2 = this.m_hlBaseRenderer.GetMaterial();
    if (this.amountchange)
    {
      double num1 = (double) Time.deltaTime / 0.5;
      float num2 = (float) (num1 * 0.69539999961853);
      float num3 = (float) (num1 * 0.69539999961853);
      Debug.Log((object) ("amount edge " + (object) (material1.GetFloat("_Amount") + num3)));
      material2.SetFloat("_Amount", material2.GetFloat("_Amount") + num2);
      if ((double) material2.GetFloat("_Amount") >= 0.69539999961853)
        this.amountchange = false;
      material1.SetFloat("_Amount", material1.GetFloat("_Amount") + num3);
    }
    if (this.colorchange)
    {
      float t = Time.deltaTime / 0.5f;
      Color color = material2.color;
      material2.color = Color.Lerp(color, this.endColor, t);
    }
    if (this.powerchange)
    {
      double num4 = (double) Time.deltaTime / 0.5;
      float num5 = (float) (num4 * 18.0);
      float num6 = (float) (num4 * 0.69539999961853);
      material2.SetFloat("_power", material2.GetFloat("_power") + num5);
      if ((double) material2.GetFloat("_power") >= 29.0)
        this.powerchange = false;
      material2.SetFloat("_Amount", material2.GetFloat("_Amount") + num6);
      if ((double) material2.GetFloat("_Amount") >= 1.12000000476837)
        this.amountchange = false;
    }
    if (this.rayschange)
    {
      this.rays.transform.localScale += new Vector3(0.0f, Time.deltaTime / 0.5f * this.RayTime, 0.0f);
      if (!this.raysdone && (double) this.rays.transform.localScale.y >= 20.0)
      {
        this.rays.SetActive(false);
        this.GetComponent<Renderer>().enabled = false;
        this.inplayObj.SetActive(true);
        this.inplayObj.GetComponent<Animation>().Play();
        this.fxEmitterA.SetActive(false);
        this.raysdone = true;
      }
    }
    if (this.raysdone)
    {
      Material material3 = this.flash.GetComponent<Renderer>().GetMaterial();
      float num = material3.GetFloat("_InvFade") - Time.deltaTime;
      material3.SetFloat("_InvFade", num);
      Debug.Log((object) ("InvFade " + (object) num));
      if ((double) num <= 0.00999999977648258)
        this.entireObj.SetActive(false);
    }
    if (!this.fxEmitterAScale)
      return;
    float num7 = Time.deltaTime / 0.5f * this.fxATime;
    this.fxEmitterA.transform.localScale += new Vector3(num7, num7, num7);
  }
}
