using PegasusGame;

public class DarkmoonWheelSpell : SuperSpell
{
  private int m_metadataChoice = -1;

  public override bool ShouldReconnectIfStuck() => false;

  public override bool AttachPowerTaskList(PowerTaskList taskList)
  {
    bool flag = base.AttachPowerTaskList(taskList);
    this.m_metadataChoice = this.GetSpinResultMetadata();
    return this.m_metadataChoice != -1 && flag;
  }

  private int GetSpinResultMetadata()
  {
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = power as Network.HistMetaData;
        if (histMetaData.MetaType == HistoryMeta.Type.EFFECT_SELECTION)
          return histMetaData.Data;
      }
    }
    return -1;
  }

  protected override void DoActionNow()
  {
    this.m_startSpell.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt("YoggWheelOutcome").Value = this.m_metadataChoice;
    base.DoActionNow();
  }
}
