using Blizzard.T5.Fonts;
using Blizzard.T5.Services;
using System;
using UnityEngine;

public class AddFriendFrame : MonoBehaviour
{
  public AddFriendFrameBones m_Bones;
  public UberText m_HeaderText;
  public UberText m_InstructionText;
  public TextField m_InputTextField;
  public Font m_InputFont;
  public RecentOpponent m_RecentOpponent;
  public UberText m_LastPlayedText;
  private PegUIElement m_inputBlocker;
  private string m_inputText = string.Empty;
  private BnetPlayer m_player;
  private bool m_usePlayer;
  private string m_playerDisplayName;
  private Font m_localizedInputFont;
  private float m_initialLastPlayedTextPositionX;
  private IFontTable m_fontTable;

  public event Action Closed;

  private void Awake()
  {
    this.m_fontTable = ServiceManager.Get<IFontTable>();
    this.InitItems();
    this.Layout();
    this.InitInput();
    this.InitInputTextField();
    DialogManager.Get().OnDialogShown += new Action(this.OnDialogShown);
    DialogManager.Get().OnDialogHidden += new Action(this.OnDialogHidden);
    this.m_RecentOpponent.button.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRecentOpponentButtonReleased));
  }

  private void Start()
  {
    this.InitInputBlocker();
    this.m_InputTextField.SetInputFont(this.m_localizedInputFont);
    this.m_InputTextField.Activate();
    this.UpdateRecentOpponent();
    if (DialogManager.Get().ShowingDialog())
      return;
    this.m_InputTextField.Text = this.m_inputText;
    this.UpdateInstructions();
  }

  private void OnDestroy()
  {
    DialogManager.Get().OnDialogShown -= new Action(this.OnDialogShown);
    DialogManager.Get().OnDialogHidden -= new Action(this.OnDialogHidden);
  }

  private void InitInput()
  {
    FontDefinition fontDef = this.m_fontTable.GetFontDef(this.m_InputFont);
    if ((UnityEngine.Object) fontDef == (UnityEngine.Object) null)
      this.m_localizedInputFont = this.m_InputFont;
    else
      this.m_localizedInputFont = fontDef.m_Font;
  }

  public void UpdateLayout() => this.Layout();

  public void Close()
  {
    if ((UnityEngine.Object) this.m_inputBlocker != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_inputBlocker.gameObject);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  public void SetPlayer(BnetPlayer player)
  {
    this.m_player = player;
    if (player == null)
    {
      this.m_usePlayer = false;
      this.m_playerDisplayName = (string) null;
    }
    else
    {
      this.m_usePlayer = true;
      this.m_playerDisplayName = FriendUtils.GetUniqueName(this.m_player);
    }
    if (DialogManager.Get().ShowingDialog())
    {
      this.SaveAndHideText(this.m_playerDisplayName);
    }
    else
    {
      this.m_inputText = this.m_playerDisplayName;
      this.m_InputTextField.Text = this.m_inputText;
      this.UpdateInstructions();
    }
  }

  public void UpdateRecentOpponent()
  {
    NetCache.NetCacheFeatures netObject = NetCache.Get()?.GetNetObject<NetCache.NetCacheFeatures>();
    bool flag = netObject != null && netObject.RecentFriendListDisplayEnabled;
    BnetPlayer recentOpponent = FriendMgr.Get().GetRecentOpponent();
    if (recentOpponent == null | flag)
    {
      this.m_RecentOpponent.button.gameObject.SetActive(false);
    }
    else
    {
      this.m_RecentOpponent.button.gameObject.SetActive(true);
      this.m_RecentOpponent.nameText.Text = FriendUtils.GetUniqueNameWithColor(recentOpponent);
      this.AdjustHeaderTextPositionBasedOnBattletagLength();
    }
  }

  private void OnRecentOpponentButtonReleased(UIEvent e)
  {
    if (string.IsNullOrEmpty(this.m_RecentOpponent.nameText.Text))
      return;
    this.SetPlayer(FriendMgr.Get().GetRecentOpponent());
  }

  private void InitItems()
  {
    this.m_HeaderText.Text = GameStrings.Get("GLOBAL_ADDFRIEND_HEADER");
    this.m_InstructionText.Text = GameStrings.Get("GLOBAL_ADDFRIEND_INSTRUCTION");
    this.m_initialLastPlayedTextPositionX = this.m_LastPlayedText.transform.localPosition.x;
  }

  private void Layout()
  {
    this.transform.parent = BaseUI.Get().transform;
    this.transform.position = BaseUI.Get().GetAddFriendBone().position;
    ITouchScreenService touchScreenService = ServiceManager.Get<ITouchScreenService>();
    if ((!UniversalInputManager.Get().UseWindowsTouch() || !touchScreenService.IsTouchSupported()) && !touchScreenService.IsVirtualKeyboardVisible())
      return;
    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 100f, this.transform.position.z);
  }

  private void UpdateInstructions()
  {
    if (!((UnityEngine.Object) this.m_InstructionText != (UnityEngine.Object) null))
      return;
    this.m_InstructionText.gameObject.SetActive(string.IsNullOrEmpty(this.m_inputText) && string.IsNullOrEmpty(Input.compositionString));
  }

  private void AdjustHeaderTextPositionBasedOnBattletagLength()
  {
    Bounds bounds = this.m_RecentOpponent.nameText.GetBounds();
    double x1 = (double) bounds.size.x;
    bounds = this.m_RecentOpponent.nameText.GetTextBounds();
    double x2 = (double) bounds.size.x;
    float num = (float) (x1 - x2);
    if ((double) this.transform.lossyScale.x != 0.0)
      num /= this.transform.lossyScale.x;
    this.m_LastPlayedText.transform.localPosition = new Vector3(this.m_initialLastPlayedTextPositionX + num, this.m_LastPlayedText.transform.localPosition.y, this.m_LastPlayedText.transform.localPosition.z);
  }

  private void InitInputBlocker()
  {
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.gameObject.layer), "AddFriendInputBlocker", (Component) this);
    inputBlocker.layer = 26;
    this.m_inputBlocker = inputBlocker.AddComponent<PegUIElement>();
    this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerReleased));
  }

  private void OnInputBlockerReleased(UIEvent e) => this.OnClosed();

  private void InitInputTextField()
  {
    this.m_InputTextField.Preprocess += new Action(this.OnInputPreprocess);
    this.m_InputTextField.Changed += new Action<string>(this.OnInputChanged);
    this.m_InputTextField.Submitted += new Action<string>(this.OnInputSubmitted);
    this.m_InputTextField.Canceled += new Action(this.OnInputCanceled);
    this.m_InstructionText.gameObject.SetActive(true);
  }

  private void OnInputPreprocess()
  {
    if (!Input.imeIsSelected)
      return;
    this.UpdateInstructions();
  }

  private void OnInputChanged(string text)
  {
    this.m_inputText = text;
    this.UpdateInstructions();
    this.m_usePlayer = string.Compare(this.m_playerDisplayName, text.Trim(), true) == 0;
  }

  private void OnInputSubmitted(string input)
  {
    string name = this.m_usePlayer ? this.m_player.GetBattleTag().ToString() : input.Trim();
    if (!BnetFriendMgr.Get().SendInvite(name))
    {
      string message = GameStrings.Get("GLOBAL_ADDFRIEND_ERROR_MALFORMED");
      UIStatus.Get().AddError(message);
    }
    this.OnClosed();
  }

  private void OnInputCanceled() => this.OnClosed();

  private void OnClosed()
  {
    if (this.Closed == null)
      return;
    this.Closed();
  }

  private void SaveAndHideText(string text)
  {
    this.m_inputText = text;
    this.m_InputTextField.Text = string.Empty;
  }

  private void ShowSavedText()
  {
    this.m_InputTextField.Text = this.m_inputText;
    this.UpdateInstructions();
  }

  private void OnDialogShown() => this.SaveAndHideText(this.m_inputText);

  private void OnDialogHidden() => this.ShowSavedText();
}
