using Shared.Scripts.Util.ValueTypes;
using UnityEngine;

public class StoreRadioButton : FramedRadioButton
{
  public SaleBanner m_saleBanner;
  public UberText m_cost;
  public GameObject m_bonusFrame;
  public UberText m_bonusText;
  public GameObject m_realMoneyTextRoot;
  public GameObject m_goldRoot;
  public UberText m_goldCostText;
  public UberText m_goldButtonText;
  private static readonly Color NO_SALE_TEXT_COLOR = new Color(0.239f, 0.184f, 0.098f);
  private static readonly Color ON_SALE_TEXT_COLOR = new Color(0.702f, 57f / 500f, 0.153f);

  private void Awake() => this.ActivateSale(false);

  public override void Init(
    FramedRadioButton.FrameType frameType,
    string text,
    int buttonID,
    object userData)
  {
    base.Init(frameType, text, buttonID, userData);
    if (!(userData is StoreRadioButton.Data data))
      Debug.LogWarning((object) string.Format("StoreRadioButton.Init(): storeRadioButtonData is null (frameType={0}, text={1}, buttonID={2)", (object) frameType, (object) text, (object) buttonID));
    else if ((Record) data.m_bundle != (Record) null)
      this.InitMoneyOption(data.m_bundle);
    else if (data.m_noGTAPPTransactionData != null)
      this.InitGoldOptionNoGTAPP(data.m_noGTAPPTransactionData);
    else
      Debug.LogWarning((object) string.Format("StoreRadioButton.Init(): storeRadioButtonData has neither gold price nor bundle data! (frameType={0}, text={1}, buttonID={2)", (object) frameType, (object) text, (object) buttonID));
  }

  public void ActivateSale(bool active)
  {
    this.m_saleBanner.m_root.SetActive(active);
    this.m_text.TextColor = active ? StoreRadioButton.ON_SALE_TEXT_COLOR : StoreRadioButton.NO_SALE_TEXT_COLOR;
  }

  private void InitMoneyOption(Network.Bundle bundle)
  {
    this.m_goldRoot.SetActive(false);
    this.m_realMoneyTextRoot.SetActive(true);
    this.m_bonusFrame.SetActive(false);
    this.m_cost.Text = string.Format(GameStrings.Get("GLUE_STORE_PRODUCT_PRICE"), (object) StoreManager.Get().FormatCostBundle(bundle));
  }

  private void InitGoldOptionNoGTAPP(NoGTAPPTransactionData noGTAPPTransactionData)
  {
    string empty = string.Empty;
    long cost;
    if (StoreManager.Get().GetGoldCostNoGTAPP(noGTAPPTransactionData, out cost))
      empty = cost.ToString();
    this.m_goldRoot.SetActive(true);
    this.m_realMoneyTextRoot.SetActive(false);
    this.m_goldButtonText.Text = this.m_text.Text;
    this.m_goldCostText.Text = empty;
  }

  public class Data
  {
    public Network.Bundle m_bundle;
    public NoGTAPPTransactionData m_noGTAPPTransactionData;
  }
}
