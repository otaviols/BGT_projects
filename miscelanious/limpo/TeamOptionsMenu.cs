using UnityEngine;

public class TeamOptionsMenu : MonoBehaviour
{
  public GameObject m_root;
  public PegUIElement m_renameButton;
  public PegUIElement m_deleteButton;
  public PegUIElement m_copyButton;
  public GameObject m_top;
  public GameObject m_bottom;
  public HighlightState m_highlight;
  public Transform m_showBone;
  public Transform m_hideBone;
  public Transform[] m_buttonPositions;
  public Transform[] m_bottomPositions;
  public float[] m_topScales;
  private int m_buttonCount;
  private bool m_shown;
  private LettuceTeam m_team;
  private CollectionTeamInfo m_teamInfo;

  public bool IsShown => this.m_shown;

  public void Awake()
  {
    this.m_root.SetActive(false);
    if ((Object) this.m_renameButton != (Object) null)
      this.m_renameButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRenameButtonReleased));
    if ((Object) this.m_deleteButton != (Object) null)
      this.m_deleteButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeleteButtonReleased));
    if (!((Object) this.m_copyButton != (Object) null))
      return;
    this.m_copyButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCopyButtonReleased));
  }

  public void Show()
  {
    if (this.m_shown)
      return;
    iTween.Stop(this.gameObject);
    this.m_root.SetActive(true);
    this.UpdateLayout();
    if (this.m_buttonCount == 0)
    {
      this.m_root.SetActive(false);
    }
    else
    {
      iTween.MoveTo(this.m_root, iTween.Hash((object) "position", (object) this.m_showBone.transform.position, (object) "time", (object) 0.35f, (object) "easeType", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) "FinishShow", (object) "oncompletetarget", (object) this.gameObject));
      this.m_shown = true;
    }
  }

  public void Hide(bool animate = true)
  {
    if (!this.m_shown)
      return;
    iTween.Stop(this.gameObject);
    if (!animate)
    {
      this.m_root.SetActive(false);
    }
    else
    {
      this.m_root.SetActive(true);
      iTween.MoveTo(this.m_root, iTween.Hash((object) "position", (object) this.m_hideBone.transform.position, (object) "time", (object) 0.35f, (object) "easeType", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) "FinishHide", (object) "oncompletetarget", (object) this.gameObject));
      this.m_shown = false;
    }
  }

  private void FinishHide()
  {
    if (this.m_shown)
      return;
    this.m_root.SetActive(false);
  }

  public void SetTeam(LettuceTeam team) => this.m_team = team;

  public void SetTeamInfo(CollectionTeamInfo teamInfo) => this.m_teamInfo = teamInfo;

  private void OnRenameButtonReleased(UIEvent e)
  {
    this.m_teamInfo.Hide();
    CollectionDeckTray.Get().GetTeamsContent().RenameCurrentlyEditingTeam();
  }

  private void OnDeleteButtonReleased(UIEvent e)
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_headerText = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_HEADER");
    info.m_showAlertIcon = false;
    info.m_text = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_DESC");
    info.m_alertTextAlignment = UberText.AlignmentOptions.Center;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
    info.m_responseCallback = new AlertPopup.ResponseCallback(this.OnDeleteButtonConfirmationResponse);
    this.m_teamInfo.Hide();
    DialogManager.Get().ShowPopup(info);
  }

  private void OnCopyButtonReleased(UIEvent e)
  {
    if (!CollectionDeckTray.Get().IsShowingTeamContents())
      return;
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    if (editingTeam == null || !((Object) UIStatus.Get() != (Object) null))
      return;
    ClipboardUtils.CopyToClipboard(new ShareableMercenariesTeam(editingTeam).Serialize(true));
    UIStatus.Get().AddInfo(GameStrings.Get("GLUE_COLLECTION_DECK_COPIED_TOAST"));
  }

  private void OnDeleteButtonConfirmationResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
      return;
    CollectionDeckTray.Get().GetTeamsContent().DeleteTeam(this.m_team.ID);
  }

  private void UpdateLayout()
  {
    int buttonCount = this.GetButtonCount();
    if (buttonCount != this.m_buttonCount)
    {
      this.m_buttonCount = buttonCount;
      this.UpdateBackground();
    }
    this.UpdateButtons();
  }

  private void UpdateBackground()
  {
    if (this.m_buttonCount == 0)
      return;
    this.m_top.transform.transform.localScale = new Vector3(1f, 1f, this.m_topScales[this.m_buttonCount - 1]);
    this.m_bottom.transform.transform.position = this.m_bottomPositions[this.m_buttonCount - 1].position;
  }

  private void UpdateButtons()
  {
    int index = 0;
    bool flag1 = this.ShowRenameButton();
    bool flag2 = this.ShowDeleteButton();
    bool flag3 = this.ShowCopyButton();
    this.m_renameButton.gameObject.SetActive(flag1);
    if (flag1)
    {
      this.m_renameButton.transform.position = this.m_buttonPositions[index].position;
      ++index;
    }
    this.m_copyButton.gameObject.SetActive(flag3);
    if (flag3)
    {
      this.m_copyButton.transform.position = this.m_buttonPositions[index].position;
      ++index;
    }
    this.m_deleteButton.gameObject.SetActive(flag2);
    if (!flag2)
      return;
    this.m_deleteButton.transform.position = this.m_buttonPositions[index].position;
    int num = index + 1;
  }

  private int GetButtonCount() => 0 + (this.ShowCopyButton() ? 1 : 0) + (this.ShowRenameButton() ? 1 : 0) + (this.ShowDeleteButton() ? 1 : 0);

  private bool ShowRenameButton()
  {
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    return (editingTeam == null || !editingTeam.Locked) && UniversalInputManager.Get().IsTouchMode();
  }

  private bool ShowDeleteButton()
  {
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    return (editingTeam == null || !editingTeam.Locked) && UniversalInputManager.Get().IsTouchMode();
  }

  private bool ShowCopyButton()
  {
    LettuceTeam editingTeam = CollectionManager.Get().GetEditingTeam();
    return editingTeam == null || !editingTeam.Locked;
  }
}
