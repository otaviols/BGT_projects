using PegasusShared;

public class FiresideBrawlChoiceDialog : DialogBase
{
  public UIBButton m_regularBrawlButton;
  public UIBButton m_fsgBrawlButton;
  public PegUIElement m_offClickCatcher;
  public UberText m_regularBrawlText;
  public UberText m_fsgBrawlText;
  private FiresideBrawlChoiceDialog.ResponseCallback m_responseCallback;

  private void Start()
  {
    this.m_regularBrawlButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRegularButtonPress));
    bool flag1 = TavernBrawlManager.Get().IsTavernBrawlActive(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);
    bool flag2 = TavernBrawlManager.Get().HasUnlockedTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);
    bool flag3 = flag1 & flag2;
    this.m_regularBrawlButton.SetEnabled(flag3);
    this.m_regularBrawlButton.Flip(flag3);
    this.m_regularBrawlText.Text = flag2 ? (flag3 ? GameStrings.Get("GLUE_FIRESIDE_GATHERING_PLAY_REGULAR_BRAWL") : TavernBrawlManager.Get().GetStartingTimeText(true)) : GameStrings.Get("GLUE_TOOLTIP_BUTTON_TAVERN_BRAWL_NOT_UNLOCKED");
    bool flag4 = TavernBrawlManager.Get().IsTavernBrawlActive(BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING);
    this.m_fsgBrawlButton.SetEnabled(flag4);
    this.m_fsgBrawlButton.Flip(flag4);
    this.m_fsgBrawlText.Text = !flag4 ? GameStrings.Get("GLUE_FIRESIDE_GATHERING_BRAWL_UNAVAILABLE") : GameStrings.Get("GLUE_FIRESIDE_GATHERING_PLAY_FSG_BRAWL");
    this.m_fsgBrawlButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnFSGButtonPress));
    this.m_offClickCatcher.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.Hide()));
  }

  public override void Show()
  {
    base.Show();
    BnetBar.Get().DisableButtonsByDialog((DialogBase) this);
    this.DoShowAnimation();
    SoundManager.Get().LoadAndPlay((AssetReference) "friendly_challenge.prefab:649e070117bcd0d45bac691a03bf2dec");
    DialogBase.DoBlur();
  }

  public override void Hide()
  {
    base.Hide();
    SoundManager.Get().LoadAndPlay((AssetReference) "banner_shrink.prefab:d9de7386a7f2017429d126e972232123");
    DialogBase.EndBlur();
  }

  public void SetInfo(FiresideBrawlChoiceDialog.Info info) => this.m_responseCallback = info.m_callback;

  private void OnRegularButtonPress(UIEvent e) => this.ChooseTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);

  private void OnFSGButtonPress(UIEvent e) => this.ChooseTavernBrawl(BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING);

  private void ChooseTavernBrawl(BrawlType source)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    if (this.m_responseCallback != null)
      this.m_responseCallback(source);
    this.Hide();
  }

  public delegate void ResponseCallback(BrawlType choice);

  public class Info
  {
    public FiresideBrawlChoiceDialog.ResponseCallback m_callback;
  }
}
