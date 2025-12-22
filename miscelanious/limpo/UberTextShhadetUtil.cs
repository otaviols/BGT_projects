using UnityEngine;

public class UberTextShhadetUtil : IUberTextShaderUtil
{
  public Shader FindShader(string name) => ShaderUtils.FindShader(name);
}
