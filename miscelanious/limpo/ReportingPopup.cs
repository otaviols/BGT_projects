using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone.UI;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class ReportingPopup : MonoBehaviour
{
  public const string ReportHeaderText = "GLOBAL_FRIENDLIST_REPORT_SELECT_HEADER";
  public const string ReportHeaderFriendText = "GLOBAL_FRIENDLIST_REPORT_SELECT_HEADER_FRIEND";
  public const string ReportHeaderNonfriendText = "GLOBAL_FRIENDLIST_REPORT_SELECT_HEADER_NONFRIEND";
  public const string ReportReasonDropdownDefaultText = "GLOBAL_FRIENDLIST_REPORT_SELECT_REASON";
  public const string ShowReportReasonEvent = "SHOW_REPORT_REASON";
  public const string ShowReportReasonCheckboxesEvent = "SHOW_REPORT_REASON_CHECKBOXES";
  public const string ShowReportCompleteEvent = "SHOW_REPORT_COMPLETE";
  public const string DismissPopupEvent = "DISMISS_POPUP";
  public const string ReportReasonNextReleasedEvent = "SELECT_REASON_NEXT_BUTTON_CLICKED";
  public const string ReportDetailsSubmitReleasedEvent = "SELECT_DETAILS_SUBMIT_BUTTON_CLICKED";
  private static Dictionary<ReportType.ComplaintType, List<ReportType.SubcomplaintType>> ReportReasons = new Dictionary<ReportType.ComplaintType, List<ReportType.SubcomplaintType>>()
  {
    {
      ReportType.ComplaintType.INAPPROPRIATE_NAME,
      new List<ReportType.SubcomplaintType>()
      {
        ReportType.SubcomplaintType.BATTLETAG
      }
    },
    {
      ReportType.ComplaintType.INAPPROPRIATE_COMMUNICATION,
      new List<ReportType.SubcomplaintType>()
      {
        ReportType.SubcomplaintType.TEXT_CHAT,
        ReportType.SubcomplaintType.SPAM,
        ReportType.SubcomplaintType.CHAT_ADVERTISEMENT
      }
    },
    {
      ReportType.ComplaintType.CHEATING,
      new List<ReportType.SubcomplaintType>()
      {
        ReportType.SubcomplaintType.HACKING,
        ReportType.SubcomplaintType.BOTTING,
        ReportType.SubcomplaintType.BOOSTING_DERANKING
      }
    }
  };
  private static Dictionary<ReportType.ComplaintType, string> ComplaintTypeLabels = new Dictionary<ReportType.ComplaintType, string>()
  {
    {
      ReportType.ComplaintType.INAPPROPRIATE_NAME,
      "GLOBAL_REPORT_REASON_INAPPROPRIATE_NAME"
    },
    {
      ReportType.ComplaintType.INAPPROPRIATE_COMMUNICATION,
      "GLOBAL_REPORT_REASON_INAPPROPRIATE_CHAT"
    },
    {
      ReportType.ComplaintType.CHEATING,
      "GLOBAL_REPORT_REASON_CHEATING"
    }
  };
  private static Dictionary<ReportType.SubcomplaintType, string> SubcomplaintTypeLabels = new Dictionary<ReportType.SubcomplaintType, string>()
  {
    {
      ReportType.SubcomplaintType.BATTLETAG,
      "GLOBAL_REPORT_DETAIL_BATTLETAG"
    },
    {
      ReportType.SubcomplaintType.TEXT_CHAT,
      "GLOBAL_REPORT_DETAIL_HARASSMENT"
    },
    {
      ReportType.SubcomplaintType.SPAM,
      "GLOBAL_REPORT_DETAIL_SPAM"
    },
    {
      ReportType.SubcomplaintType.CHAT_ADVERTISEMENT,
      "GLOBAL_REPORT_DETAIL_ADVERTISEMENT"
    },
    {
      ReportType.SubcomplaintType.HACKING,
      "GLOBAL_REPORT_DETAIL_HACKING"
    },
    {
      ReportType.SubcomplaintType.BOTTING,
      "GLOBAL_REPORT_DETAIL_BOTTING"
    },
    {
      ReportType.SubcomplaintType.BOOSTING_DERANKING,
      "GLOBAL_REPORT_DETAIL_INTENTIONALLY_LOSING_DERANKING"
    }
  };
  [SerializeField]
  private AsyncReference m_reportReasonHeaderReference;
  [SerializeField]
  private AsyncReference m_reportDetailsHeaderReference;
  [SerializeField]
  private AsyncReference m_reportReasonDropdownReference;
  [SerializeField]
  private AsyncReference m_reportReasonDetailReference;
  [SerializeField]
  private AsyncReference[] m_reportReasonDetailCheckboxReferences;
  [SerializeField]
  private AsyncReference m_reportReasonNextButtonReference;
  [SerializeField]
  private AsyncReference m_reportDetailsSubmitButtonReference;
  private Widget m_widget;
  private BnetPlayer m_player;
  private UberText m_reportReasonHeader;
  private UberText m_reportDetailHeader;
  private DropdownControl m_reportReasonDropdown;
  private List<CheckBox> m_reportDetailCheckboxes;
  private UIBButton m_reportReasonNextButton;
  private UIBButton m_reportDetailsSubmitButton;
  private ReportType.ComplaintType? m_selectedIssueType;
  private HashSet<ReportType.SubcomplaintType> m_selectedReportDetails = new HashSet<ReportType.SubcomplaintType>();

  private void Awake()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_widget.SetLayerOverride(GameLayer.HighPriorityUI);
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.ReportingPopupEventListener));
    this.m_widget.RegisterReadyListener((System.Action<object>) (_ => this.m_widget.TriggerEvent("SHOW_REPORT_REASON")), (object) null, true);
    this.m_reportReasonHeaderReference.RegisterReadyListener<UberText>((System.Action<UberText>) (reportHeader => this.m_reportReasonHeader = reportHeader));
    this.m_reportDetailsHeaderReference.RegisterReadyListener<UberText>((System.Action<UberText>) (reportHeader => this.m_reportDetailHeader = reportHeader));
    this.m_reportReasonDropdownReference.RegisterReadyListener<DropdownControl>((System.Action<DropdownControl>) (dropdownControl =>
    {
      this.m_reportReasonDropdown = dropdownControl;
      this.SetReportReasons();
    }));
    this.m_reportDetailCheckboxes = new List<CheckBox>();
    foreach (AsyncReference checkboxReference in this.m_reportReasonDetailCheckboxReferences)
      checkboxReference.RegisterReadyListener<CheckBox>((System.Action<CheckBox>) (checkbox =>
      {
        this.m_reportDetailCheckboxes.Add(checkbox);
        checkbox.SetChecked(false);
      }));
    this.m_reportReasonNextButtonReference.RegisterReadyListener<UIBButton>((System.Action<UIBButton>) (button =>
    {
      this.m_reportReasonNextButton = button;
      this.SetButton(this.m_reportReasonNextButton, false, true);
    }));
    this.m_reportDetailsSubmitButtonReference.RegisterReadyListener<UIBButton>((System.Action<UIBButton>) (button =>
    {
      this.m_reportDetailsSubmitButton = button;
      this.SetButton(this.m_reportDetailsSubmitButton, false, true);
    }));
  }

  private void ReportingPopupEventListener(string eventName)
  {
    if (!(eventName == "SELECT_REASON_NEXT_BUTTON_CLICKED"))
    {
      if (!(eventName == "SELECT_DETAILS_SUBMIT_BUTTON_CLICKED"))
        return;
      this.OnReportDetailsSubmitReleased();
    }
    else
      this.OnReportReasonNextReleased();
  }

  public void Init(BnetPlayer player)
  {
    this.m_player = player;
    if (BnetFriendMgr.Get().IsFriend(this.m_player))
    {
      this.m_reportReasonHeader.Text = GameStrings.Format("GLOBAL_FRIENDLIST_REPORT_SELECT_HEADER_FRIEND", (object) player.GetBattleTag());
      this.m_reportDetailHeader.Text = GameStrings.Format("GLOBAL_FRIENDLIST_REPORT_SELECT_HEADER_FRIEND", (object) player.GetBattleTag());
    }
    else
    {
      this.m_reportReasonHeader.Text = GameStrings.Format("GLOBAL_FRIENDLIST_REPORT_SELECT_HEADER_NONFRIEND", (object) player.GetBattleTag());
      this.m_reportDetailHeader.Text = GameStrings.Format("GLOBAL_FRIENDLIST_REPORT_SELECT_HEADER_NONFRIEND", (object) player.GetBattleTag());
    }
    this.m_selectedIssueType = new ReportType.ComplaintType?();
    this.m_selectedReportDetails.Clear();
    this.m_reportReasonDropdown.setSelection((object) null);
    this.SetButton(this.m_reportReasonNextButton, false, true);
    this.SetButton(this.m_reportDetailsSubmitButton, false, true);
    this.m_widget.TriggerEvent("SHOW_REPORT_REASON");
  }

  private void SetReportReasons()
  {
    this.m_reportReasonDropdown.clearItems();
    this.m_reportReasonDropdown.setItemChosenCallback(new DropdownControl.itemChosenCallback(this.OnReportReasonSelect));
    this.m_reportReasonDropdown.setItemTextCallback(new DropdownControl.itemTextCallback(this.GetReportReasonString));
    this.m_reportReasonDropdown.setUnselectedItemText(GameStrings.Get("GLOBAL_FRIENDLIST_REPORT_SELECT_REASON"));
    foreach (KeyValuePair<ReportType.ComplaintType, List<ReportType.SubcomplaintType>> reportReason in ReportingPopup.ReportReasons)
      this.m_reportReasonDropdown.addItem((object) reportReason.Key);
    LayerUtils.SetLayer((Component) this.m_reportReasonDropdown, GameLayer.HighPriorityUI);
    this.m_reportReasonDropdown.gameObject.SetActive(true);
  }

  private void SetDetailsForReason(ReportType.ComplaintType issue)
  {
    List<ReportType.SubcomplaintType> subcomplaintTypeList;
    if (!ReportingPopup.ReportReasons.TryGetValue(issue, out subcomplaintTypeList) || subcomplaintTypeList == null)
      return;
    for (int index = 0; index < subcomplaintTypeList.Count; ++index)
    {
      if (index < this.m_reportDetailCheckboxes.Count)
      {
        CheckBox checkBox = this.m_reportDetailCheckboxes[index];
        checkBox.SetChecked(false);
        ReportType.SubcomplaintType subcomplaintType = subcomplaintTypeList[index];
        checkBox.SetButtonText(ReportingPopup.SubcomplaintTypeLabels[subcomplaintType]);
        checkBox.ClearEventListeners();
        checkBox.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (_ => this.OnReportDetailSelectionChanged(checkBox, subcomplaintType)));
        checkBox.gameObject.SetActive(true);
      }
      else
        break;
    }
    for (int index = this.m_reportDetailCheckboxes.Count - 1; index >= subcomplaintTypeList.Count; --index)
    {
      CheckBox reportDetailCheckbox = this.m_reportDetailCheckboxes[index];
      reportDetailCheckbox.SetButtonText(string.Empty);
      reportDetailCheckbox.ClearEventListeners();
      reportDetailCheckbox.gameObject.SetActive(false);
    }
  }

  private string GetReportReasonString(object val)
  {
    string str1 = "";
    string str2;
    return val is ReportType.ComplaintType key && ReportingPopup.ComplaintTypeLabels.TryGetValue(key, out str2) ? str2 : str1;
  }

  private void OnReportReasonSelect(object selection, object prevSelection)
  {
    if (selection is ReportType.ComplaintType complaintType)
    {
      this.m_selectedIssueType = new ReportType.ComplaintType?(complaintType);
      this.SetButton(this.m_reportReasonNextButton, true);
    }
    else
      this.SetButton(this.m_reportReasonNextButton, false);
  }

  private void OnReportDetailSelectionChanged(
    CheckBox checkbox,
    ReportType.SubcomplaintType subcomplaintType)
  {
    if (checkbox.IsChecked())
    {
      if (this.m_selectedReportDetails.Count == 0)
        this.SetButton(this.m_reportDetailsSubmitButton, true);
      this.m_selectedReportDetails.Add(subcomplaintType);
    }
    else
    {
      if (this.m_selectedReportDetails.Count == 1)
        this.SetButton(this.m_reportDetailsSubmitButton, false);
      this.m_selectedReportDetails.Remove(subcomplaintType);
    }
  }

  private void OnReportReasonNextReleased()
  {
    if (!this.m_selectedIssueType.HasValue)
      return;
    this.m_widget.TriggerEvent("SHOW_REPORT_REASON_CHECKBOXES");
    this.SetDetailsForReason(this.m_selectedIssueType.Value);
  }

  private void OnReportDetailsSubmitReleased()
  {
    this.m_widget.TriggerEvent("SHOW_REPORT_COMPLETE");
    if (!this.m_selectedIssueType.HasValue)
      return;
    BattleNet.Get().SubmitReport(this.m_player.GetAccountId(), this.m_selectedIssueType.Value, new List<ReportType.SubcomplaintType>((IEnumerable<ReportType.SubcomplaintType>) this.m_selectedReportDetails));
  }

  private void SetButton(UIBButton button, bool enableState, bool forceImmediate = false)
  {
    button.Flip(enableState, forceImmediate);
    button.SetEnabled(enableState);
  }
}
