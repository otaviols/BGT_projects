using System;
using System.Collections.Generic;

[Serializable]
public class UNNotificationAction
{
  private const string IDENTIFIER_KEY = "identifier";
  private const string TITLE_KEY = "title";
  private const string OPTIONS_KEY = "options";
  public string identifier;
  public string title;
  public List<UNUserNotificationAction> options;
}
