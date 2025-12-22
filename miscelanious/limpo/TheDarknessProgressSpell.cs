using System.Collections.Generic;

public class TheDarknessProgressSpell : Spell
{
  public UberText m_ProgressText;

  public override bool AddPowerTargets()
  {
    if (!base.AddPowerTargets())
      return false;
    int currnt = 0;
    if (!this.GetCurrentProgress(ref currnt))
      return false;
    int tag = this.GetSourceCard().GetEntity().GetTag(GAME_TAG.SCORE_VALUE_1);
    this.m_ProgressText.Text = string.Format("{0}/{1}", (object) currnt, (object) tag);
    return true;
  }

  private bool GetCurrentProgress(ref int currnt)
  {
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      Network.PowerHistory power = taskList[index].GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = (Network.HistTagChange) power;
        if (histTagChange.Entity == this.GetSourceCard().GetEntity().GetEntityId() && histTagChange.Tag == 453)
        {
          currnt = histTagChange.Value;
          return true;
        }
      }
    }
    return false;
  }
}
