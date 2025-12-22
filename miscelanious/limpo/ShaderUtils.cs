using UnityEngine;

public class ShaderUtils
{
  public static Shader FindShader(string name) => ShaderPreCompiler.GetShader(name);
}
