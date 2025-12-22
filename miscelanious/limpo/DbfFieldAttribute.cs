using System;

[AttributeUsage(AttributeTargets.Property)]
public class DbfFieldAttribute : Attribute
{
  public string m_varName;

  public DbfFieldAttribute(string varName) => this.m_varName = varName;
}
