using PegasusGame;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class SelectSpell : Spell
{
  public List<SelectSpellTableEntry> m_Table = new List<SelectSpellTableEntry>();
  private Spell m_selectedSpell;
  private int m_selectionIndex = -1;

  private void LoadSelectedSpell(int selection)
  {
    if (this.m_selectionIndex == selection && (bool) (Object) this.m_selectedSpell)
      return;
    this.m_selectionIndex = selection;
    if ((bool) (Object) this.m_selectedSpell)
    {
      if (this.m_selectedSpell.HasUsableState(SpellStateType.CANCEL))
        this.m_selectedSpell.ActivateState(SpellStateType.CANCEL);
      else
        this.m_selectedSpell.Deactivate();
      this.m_selectedSpell = (Spell) null;
    }
    SelectSpellTableEntry selectSpellTableEntry1 = (SelectSpellTableEntry) null;
    foreach (SelectSpellTableEntry selectSpellTableEntry2 in this.m_Table)
    {
      if (selectSpellTableEntry2.m_Selection == selection)
      {
        selectSpellTableEntry1 = selectSpellTableEntry2;
        break;
      }
    }
    if (selectSpellTableEntry1 == null || !((Object) selectSpellTableEntry1.m_Spell != (Object) null))
      return;
    this.m_selectedSpell = SpellManager.Get().GetSpell(selectSpellTableEntry1.m_Spell);
    if (!((Object) this.m_selectedSpell != (Object) null))
      return;
    TransformUtil.AttachAndPreserveLocalTransform(this.m_selectedSpell.transform, this.gameObject.transform);
  }

  public override bool AttachPowerTaskList(PowerTaskList taskList) => this.SetSelectedSpell(taskList) && this.m_selectedSpell.AttachPowerTaskList(taskList) && base.AttachPowerTaskList(taskList);

  private bool SetSelectedSpell(PowerTaskList taskList)
  {
    foreach (PowerTask task in taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == HistoryMeta.Type.EFFECT_SELECTION)
        {
          this.LoadSelectedSpell(histMetaData.Data);
          return (Object) this.m_selectedSpell != (Object) null;
        }
      }
    }
    return false;
  }

  protected override void OnBirth(SpellStateType prevStateType)
  {
    this.LoadSelectedSpell(0);
    if ((bool) (Object) this.m_selectedSpell)
    {
      this.m_selectedSpell.SetSource(this.GetSource());
      this.m_selectedSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSelectedSpellStateFinished));
      this.m_selectedSpell.ActivateState(SpellStateType.BIRTH);
    }
    base.OnBirth(prevStateType);
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    this.m_selectedSpell.SetSource(this.GetSource());
    this.m_selectedSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSelectedSpellFinished));
    this.m_selectedSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSelectedSpellStateFinished));
    this.m_selectedSpell.ActivateState(SpellStateType.ACTION);
    base.OnAction(prevStateType);
  }

  protected override void OnCancel(SpellStateType prevStateType)
  {
    if ((Object) this.m_selectedSpell != (Object) null && this.m_selectedSpell.GetActiveState() != SpellStateType.NONE && this.m_selectedSpell.GetActiveState() != SpellStateType.CANCEL)
      this.m_selectedSpell.ActivateState(SpellStateType.CANCEL);
    base.OnCancel(prevStateType);
  }

  private void OnSelectedSpellFinished(Spell spell, object userData) => this.OnSpellFinished();

  private void OnSelectedSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE || !((Object) spell == (Object) this.m_selectedSpell))
      return;
    this.m_selectedSpell = (Spell) null;
    this.m_selectionIndex = -1;
    this.Deactivate();
  }
}
