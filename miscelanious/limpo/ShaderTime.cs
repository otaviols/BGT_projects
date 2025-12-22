using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTime : IService, IHasUpdate
{
  private float m_maxTime = (float) ushort.MaxValue;
  private float m_time;
  private bool m_enabled = true;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    yield break;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (IGraphicsManager)
  };

  public void Shutdown() => Shader.SetGlobalFloat("_ShaderTime", 0.0f);

  public void Update()
  {
    this.UpdateShaderAnimationTime();
    this.UpdateGyro();
  }

  private void UpdateShaderAnimationTime()
  {
    if (!this.m_enabled)
    {
      this.m_time = 1f;
    }
    else
    {
      this.m_time += Time.deltaTime / 20f;
      if ((double) this.m_time > (double) this.m_maxTime)
      {
        this.m_time -= this.m_maxTime;
        if ((double) this.m_time <= 0.0)
          this.m_time = 0.0001f;
      }
    }
    Shader.SetGlobalFloat("_ShaderTime", this.m_time);
  }

  private void UpdateGyro() => Shader.SetGlobalVector("_Gyroscope", (Vector4) Input.gyro.gravity);
}
