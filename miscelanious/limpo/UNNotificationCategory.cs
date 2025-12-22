using System;
using System.Collections.Generic;

[Serializable]
public class UNNotificationCategory
{
  private const string IDENTIFIER_KEY = "identifier";
  private const string OPTIONS_KEY = "options";
  private const string ACTIONS_KEY = "actions";
  public string identifier;
  public List<UNNotificationCategoryOptions> options;
  public List<UNNotificationAction> actions;
}
