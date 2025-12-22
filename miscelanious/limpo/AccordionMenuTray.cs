using System.Collections.Generic;
using UnityEngine;

public class AccordionMenuTray : MonoBehaviour
{
  [CustomEditField(Sections = "Description")]
  public UberText m_DescriptionTitleObject;
  [CustomEditField(Sections = "Description")]
  public GameObject m_DescriptionContainer;
  [SerializeField]
  [CustomEditField(Sections = "Choose Frame")]
  public PlayButton m_ChooseButton;
  [CustomEditField(Sections = "Choose Frame")]
  [SerializeField]
  public UIBButton m_BackButton;
  [SerializeField]
  [CustomEditField(Sections = "Choose Frame", T = EditType.GAME_OBJECT)]
  public string m_DefaultChooserButtonPrefab;
  [SerializeField]
  [CustomEditField(Sections = "Choose Frame", T = EditType.GAME_OBJECT)]
  public string m_DefaultChooserSubButtonPrefab;
  [SerializeField]
  [CustomEditField(Sections = "Choose Frame", T = EditType.GAME_OBJECT)]
  public string m_DefaultChooserComingSoonSubButtonPrefab;
  [CustomEditField(Sections = "Choose Frame")]
  public UIBScrollable m_ChooseFrameScroller;
  [CustomEditField(Sections = "Behavior Settings")]
  public bool m_OnlyOneExpands;
  [SerializeField]
  private float m_ButtonOffset = -2.5f;
  protected ChooserSubButton m_SelectedSubButton;
  protected List<ChooserButton> m_ChooserButtons = new List<ChooserButton>();
  protected bool m_isStarted;
  protected bool m_AttemptedLoad;

  [CustomEditField(Sections = "Behavior Settings")]
  public float ButtonOffset
  {
    get => this.m_ButtonOffset;
    set
    {
      this.m_ButtonOffset = value;
      this.OnButtonVisualUpdated();
    }
  }

  protected void OnButtonVisualUpdated()
  {
    float num = 0.0f;
    ChooserButton[] array = this.m_ChooserButtons.ToArray();
    for (int index = 0; index < array.Length; ++index)
    {
      TransformUtil.SetLocalPosZ((Component) array[index].transform, -num);
      num += array[index].GetFullButtonHeight() + this.m_ButtonOffset;
    }
  }

  protected void OnChooserButtonToggled(ChooserButton btn, bool toggled, int index)
  {
    btn.SetSelectSubButtonOnToggle(this.m_OnlyOneExpands);
    if (this.m_OnlyOneExpands)
    {
      if (!toggled)
        return;
      this.ToggleScrollable(false);
      ChooserButton[] array = this.m_ChooserButtons.ToArray();
      for (int index1 = 0; index1 < array.Length; ++index1)
      {
        if (index1 != index)
          array[index1].Toggle = false;
      }
    }
    else
    {
      if (!((Object) this.m_SelectedSubButton != (Object) null))
        return;
      btn = this.m_ChooserButtons[index];
      if (!btn.ContainsSubButton(this.m_SelectedSubButton))
        return;
      this.m_SelectedSubButton.SetHighlight(toggled);
      if (!toggled && (Object) this.m_ChooseButton != (Object) null)
      {
        this.m_ChooseButton.Disable();
      }
      else
      {
        if (this.m_AttemptedLoad || !((Object) this.m_ChooseButton != (Object) null))
          return;
        this.m_ChooseButton.Enable();
      }
    }
  }

  protected void ToggleScrollable(bool enable)
  {
    if (!((Object) this.m_ChooseFrameScroller != (Object) null) || this.m_ChooseFrameScroller.enabled == enable)
      return;
    Log.FiresideGatherings.Print("FiresideGatheringChooserTray.ToggleScrollable: " + enable.ToString());
    this.m_ChooseFrameScroller.enabled = enable;
  }
}
