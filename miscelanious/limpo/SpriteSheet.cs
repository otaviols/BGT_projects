using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class SpriteSheet : MonoBehaviour
{
  public int _uvTieX = 1;
  public int _uvTieY = 1;
  public float m_fps = 30f;
  public bool m_Old_Mode;
  private int m_LastIdx = -1;
  private Vector2 m_Size = Vector2.one;
  private float m_NextFrame;
  private int m_X;
  private int m_Y;
  private Renderer m_renderer;

  private void Start()
  {
    this.m_NextFrame = Time.timeSinceLevelLoad + 1f / this.m_fps;
    this.m_renderer = this.GetComponent<Renderer>();
    if ((Object) this.m_renderer == (Object) null)
    {
      Debug.LogError((object) ("SpriteSheet needs a Renderer on: " + this.gameObject.name));
      this.enabled = false;
    }
    this.m_Size = new Vector2(1f / (float) this._uvTieX, 1f / (float) this._uvTieY);
  }

  private void Update()
  {
    if (this.m_Old_Mode)
    {
      int num = (int) ((double) Time.time * (double) this.m_fps % (double) (this._uvTieX * this._uvTieY));
      if (num == this.m_LastIdx)
        return;
      Material material = this.m_renderer.GetMaterial();
      material.mainTextureOffset = new Vector2((float) (num % this._uvTieX) * this.m_Size.x, (float) (1.0 - (double) this.m_Size.y - (double) (num / this._uvTieY) * (double) this.m_Size.y));
      material.mainTextureScale = this.m_Size;
      this.m_LastIdx = num;
    }
    else
    {
      if ((double) Time.timeSinceLevelLoad < (double) this.m_NextFrame)
        return;
      ++this.m_X;
      if (this.m_X > this._uvTieX - 1)
      {
        ++this.m_Y;
        this.m_X = 0;
      }
      if (this.m_Y > this._uvTieY - 1)
        this.m_Y = 0;
      Material material = this.m_renderer.GetMaterial();
      material.mainTextureOffset = new Vector2((float) this.m_X * this.m_Size.x, (float) (1.0 - (double) this.m_Y * (double) this.m_Size.y));
      material.mainTextureScale = this.m_Size;
      this.m_NextFrame = Time.timeSinceLevelLoad + 1f / this.m_fps;
    }
  }
}
