using System.Collections.Generic;

[CustomEditClass]
public class CardListPopup : DialogBase
{
  [CustomEditField(Sections = "Object Links")]
  public CardListPanel m_CardsContainer_SingleLineDescription;
  [CustomEditField(Sections = "Object Links")]
  public CardListPanel m_CardsContainer_MultiLineDescription;
  [CustomEditField(Sections = "Object Links")]
  public UIBButton m_okayButton;
  [CustomEditField(Sections = "Object Links")]
  public UberText m_descriptionSingleLine;
  [CustomEditField(Sections = "Object Links")]
  public UberText m_descriptionMultiLine;
  private CardListPopup.Info m_info = new CardListPopup.Info();

  protected override void Awake()
  {
    base.Awake();
    this.m_okayButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.Hide()));
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (UniversalInputManager.Get() == null)
      return;
    UniversalInputManager.Get().SetSystemDialogActive(false);
  }

  public void SetInfo(CardListPopup.Info info)
  {
    this.m_info = info;
    if (this.m_info.m_callbackOnHide == null)
      return;
    this.AddHideListener(this.m_info.m_callbackOnHide);
  }

  public override void Show()
  {
    base.Show();
    DialogBase.DoBlur();
    if (this.m_info.m_useMultiLineDescription)
    {
      this.m_CardsContainer_MultiLineDescription.Show(this.m_info.m_cards);
      this.m_descriptionMultiLine.Text = this.m_info.m_description;
    }
    else
    {
      this.m_CardsContainer_SingleLineDescription.Show(this.m_info.m_cards);
      this.m_descriptionSingleLine.Text = this.m_info.m_description;
    }
    UniversalInputManager.Get().SetSystemDialogActive(true);
  }

  public override void Hide()
  {
    base.Hide();
    DialogBase.EndBlur();
  }

  public class Info
  {
    public string m_description;
    public List<int> m_cards;
    public DialogBase.HideCallback m_callbackOnHide;
    public bool m_useMultiLineDescription;
  }
}
