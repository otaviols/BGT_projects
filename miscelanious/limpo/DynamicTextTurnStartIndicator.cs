using System;
using System.Collections.Generic;

public class DynamicTextTurnStartIndicator : TurnStartIndicator
{
  public List<DynamicTextTurnStartIndicator.StringMapping> m_stringMappings;

  public override void Show()
  {
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    if (friendlySidePlayer == null)
    {
      Log.Gameplay.PrintError("DynamicTextTurnStartIndicator.Show(): playerEntity is somehow null, text will not be displayed! Contact a gameplay engineer.", (object) this);
    }
    else
    {
      foreach (DynamicTextTurnStartIndicator.StringMapping stringMapping in this.m_stringMappings)
      {
        if ((UnityEngine.Object) stringMapping.m_DynamicText == (UnityEngine.Object) null)
        {
          Log.Gameplay.PrintError("DynamicTextTurnStartIndicator.Show(): m_DynamicText on {0} is null, please assign an UberText!", (object) this);
          return;
        }
        if (stringMapping.m_TagToPullStringIDFrom == 0)
        {
          Log.Gameplay.PrintError("DynamicTextTurnStartIndicator.Show(): m_DynamicText on {0} is null, please assign an UberText!", (object) this);
          return;
        }
        stringMapping.m_DynamicText.Text = GameDbf.GetIndex().GetClientString(friendlySidePlayer.GetTag(stringMapping.m_TagToPullStringIDFrom));
      }
      base.Show();
    }
  }

  [Serializable]
  public class StringMapping
  {
    public UberText m_DynamicText;
    public int m_TagToPullStringIDFrom;
  }
}
