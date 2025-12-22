using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class Flipbook : MonoBehaviour
{
  public float m_flipbookRate = 15f;
  public bool m_flipbookRandom;
  public Vector2[] m_flipbookOffsets = new Vector2[4]
  {
    new Vector2(0.0f, 0.5f),
    new Vector2(0.5f, 0.5f),
    new Vector2(0.0f, 0.0f),
    new Vector2(0.5f, 0.0f)
  };
  public bool m_animate = true;
  public bool m_reverse = true;
  public bool m_RandomRateRange;
  public float m_RandomRateMin;
  public float m_RandomRateMax;
  private float m_flipbookFrame;
  private bool m_flipbookReverse;
  private int m_flipbookLastOffset;

  private void Start()
  {
    if (this.m_RandomRateRange)
      this.m_flipbookRate = Random.Range(this.m_RandomRateMin, this.m_RandomRateMax);
    if (!this.m_flipbookRandom)
      return;
    this.m_flipbookLastOffset = Random.Range(0, this.m_flipbookOffsets.Length);
    this.SetIndex(this.m_flipbookLastOffset);
  }

  private void Update()
  {
    float flipbookRate = this.m_flipbookRate;
    if ((double) flipbookRate == 0.0)
      return;
    bool flag = false;
    if ((double) flipbookRate < 0.0)
    {
      flipbookRate *= -1f;
      flag = true;
    }
    if (!this.m_animate)
      return;
    if ((double) this.m_flipbookFrame > (double) flipbookRate)
    {
      int i;
      if (this.m_flipbookRandom)
      {
        int num = 0;
        do
        {
          i = Random.Range(0, this.m_flipbookOffsets.Length);
          ++num;
        }
        while (i == this.m_flipbookLastOffset && num < 100);
        this.m_flipbookLastOffset = i;
      }
      else
      {
        if (flag)
        {
          this.m_flipbookLastOffset -= Mathf.FloorToInt(this.m_flipbookFrame / flipbookRate);
          if (this.m_flipbookLastOffset < 0)
          {
            this.m_flipbookLastOffset = Mathf.FloorToInt((float) (this.m_flipbookOffsets.Length - Mathf.Abs(this.m_flipbookLastOffset)));
            if (this.m_flipbookLastOffset < 0)
              this.m_flipbookLastOffset = this.m_flipbookOffsets.Length - 1;
          }
        }
        else if (!this.m_flipbookReverse)
        {
          if (this.m_reverse)
          {
            if (this.m_flipbookLastOffset >= this.m_flipbookOffsets.Length - 1)
            {
              this.m_flipbookLastOffset = this.m_flipbookOffsets.Length - 1;
              this.m_flipbookReverse = true;
            }
            else
              ++this.m_flipbookLastOffset;
          }
          else
          {
            this.m_flipbookLastOffset += Mathf.FloorToInt(this.m_flipbookFrame / flipbookRate);
            if (this.m_flipbookLastOffset >= this.m_flipbookOffsets.Length)
            {
              this.m_flipbookLastOffset = Mathf.FloorToInt((float) (this.m_flipbookLastOffset - this.m_flipbookOffsets.Length));
              if (this.m_flipbookLastOffset >= this.m_flipbookOffsets.Length)
                this.m_flipbookLastOffset = 0;
            }
          }
        }
        else if (this.m_flipbookLastOffset <= 0)
        {
          this.m_flipbookLastOffset = 1;
          this.m_flipbookReverse = false;
        }
        else
        {
          this.m_flipbookLastOffset -= Mathf.FloorToInt(this.m_flipbookFrame / flipbookRate);
          if (this.m_flipbookLastOffset < 0)
            this.m_flipbookLastOffset = Mathf.FloorToInt((float) (this.m_flipbookOffsets.Length - Mathf.Abs(this.m_flipbookLastOffset)));
          if (this.m_flipbookLastOffset < 0)
            this.m_flipbookLastOffset = this.m_flipbookOffsets.Length - 1;
        }
        i = this.m_flipbookLastOffset;
      }
      this.m_flipbookFrame = 0.0f;
      this.SetIndex(i);
    }
    this.m_flipbookFrame += Time.deltaTime * 60f;
  }

  public void SetIndex(int i)
  {
    if (i < 0 || i >= this.m_flipbookOffsets.Length)
    {
      if (i < 0)
        this.m_flipbookLastOffset = 0;
      else
        this.m_flipbookLastOffset = this.m_flipbookOffsets.Length;
    }
    else
      this.GetComponent<Renderer>().GetMaterial().SetTextureOffset("_MainTex", this.m_flipbookOffsets[i]);
  }
}
