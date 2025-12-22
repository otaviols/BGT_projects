using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate), typeof (WidgetTransform))]
public class MobileChatLogDeckcodeMessageFrame : MobileChatLogMessageFrame
{
  private const float DeckClassIconOffset = 50f;
  [SerializeField]
  private GameObject m_iconGameObject;
  [SerializeField]
  private GameObject m_defaultIcon;
  [SerializeField]
  private GameObject m_mercenariesIcon;
  [SerializeField]
  private string m_hintColor;
  [SerializeField]
  private string m_hintSize;
  private WidgetTemplate m_widget;
  private WidgetTransform m_widgetTransform;

  public string DeckName { get; set; }

  public string DeckcodeString { get; set; }

  public override float Width
  {
    get => this.LocalBounds.size.x;
    set
    {
      float num = value / 2f;
      Vector3 localPosition = this.transform.localPosition;
      this.m_widgetTransform.Left = localPosition.x - num;
      this.m_widgetTransform.Right = localPosition.x + num;
    }
  }

  private void Awake()
  {
    this.m_widget = this.GetComponent<WidgetTemplate>();
    this.m_widgetTransform = this.GetComponent<WidgetTransform>();
    this.m_widget.RegisterReadyListener(new Action<object>(this.OnDeckcodeMessageFrameReady), (object) null, true);
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnClickEventListener));
  }

  private void Start() => this.UpdateLocalBounds();

  private void OnDeckcodeMessageFrameReady(object _)
  {
    this.m_widget.SetLayerOverride(GameLayer.BattleNetChat);
    this.UpdateLocalBounds();
  }

  private void UpdateIconPosition()
  {
    Vector3 position = this.transform.position;
    Bounds widgetTransforms = WidgetTransform.GetWorldBoundsOfWidgetTransforms(this.transform);
    position.x = (float) ((double) this.transform.position.x - (double) widgetTransforms.extents.x + 6.25);
    this.m_iconGameObject.transform.position = position;
  }

  public void BindClassData(ShareableDeck shareableDeck)
  {
    this.DeckName = shareableDeck.DeckName;
    this.UpdateIconPosition();
    string str1 = "<color=" + this.m_hintColor + "><size=" + this.m_hintSize + ">" + (UniversalInputManager.Get().IsTouchMode() ? GameStrings.Get("GLOBAL_CHAT_DECK_CODE_HINT_TOUCH") : GameStrings.Get("GLOBAL_CHAT_DECK_CODE_HINT")) + "</size></color>";
    if (shareableDeck is ShareableMercenariesTeam)
    {
      this.m_mercenariesIcon.SetActive(true);
      string str2;
      if (string.IsNullOrWhiteSpace(this.DeckName))
        str2 = GameStrings.Format("GLOBAL_CHAT_MERCENARIES_PARTY_CODE_MESSAGE", (object) str1);
      else
        str2 = GameStrings.Format("GLOBAL_CHAT_MERCENARIES_PARTY_CODE_WITH_NAME_MESSAGE", (object) this.DeckName, (object) str1);
      this.Message = str2;
    }
    else
    {
      TAG_CLASS classFromDeck = ShareableDeck.ExtractClassFromDeck(shareableDeck);
      if (classFromDeck == TAG_CLASS.INVALID)
        this.m_defaultIcon.SetActive(true);
      this.m_widget.BindDataModel((IDataModel) new PrototypeDataModel()
      {
        String1 = classFromDeck.ToString()
      }, false);
      string str3 = classFromDeck == TAG_CLASS.INVALID ? string.Empty : GameStrings.GetClassName(classFromDeck);
      string str4;
      if (string.IsNullOrWhiteSpace(this.DeckName))
        str4 = GameStrings.Format("GLOBAL_CHAT_DECK_CODE_MESSAGE", (object) str3, (object) str1);
      else
        str4 = GameStrings.Format("GLOBAL_CHAT_DECK_CODE_WITH_NAME_MESSAGE", (object) str3, (object) this.DeckName, (object) str1);
      this.Message = str4;
    }
  }

  private void OnClickEventListener(string eventName)
  {
    if (!(eventName == "BUTTON_CLICKED"))
      return;
    ClipboardUtils.CopyToClipboard(ShareableDeck.GenerateDeckCodeMessage(this.DeckcodeString, this.DeckName));
    UIStatus.Get().AddInfo(GameStrings.Get("GLUE_COLLECTION_DECK_COPIED_TOAST"));
  }

  public override void RebuildUberText()
  {
    Bounds bounds = new Bounds();
    bounds.extents = new Vector3((float) (((double) this.m_widgetTransform.Right - (double) this.m_widgetTransform.Left) / 2.0 - 50.0), (float) (((double) this.m_widgetTransform.Top - (double) this.m_widgetTransform.Bottom) / 2.0), this.LocalBounds.extents.z);
    bounds.center = new Vector3(this.text.transform.localPosition.x + 25f, this.text.transform.localPosition.y, this.text.transform.localPosition.z);
    WidgetTransform component;
    if (this.text.TryGetComponent<WidgetTransform>(out component))
    {
      component.Left = bounds.min.x;
      component.Right = bounds.max.x;
      component.Bottom = bounds.min.y;
      component.Top = bounds.max.y;
    }
    this.text.UpdateNow(true);
  }

  public override void OnPositionUpdate() => this.UpdateLocalBounds();

  public override void UpdateLocalBounds()
  {
    float x = (float) (((double) this.m_widgetTransform.Right - (double) this.m_widgetTransform.Left) / 2.0);
    float y = (float) (((double) this.m_widgetTransform.Top - (double) this.m_widgetTransform.Bottom) / 2.0);
    Bounds bounds = new Bounds();
    bounds.center = Vector3.zero;
    bounds.extents = new Vector3(x, y, 0.0f);
    this.m_widgetTransform.Right = x;
    this.m_widgetTransform.Left = -x;
    this.m_widgetTransform.Top = y;
    this.m_widgetTransform.Bottom = -y;
    BoxCollider component;
    if (this.m_widget.TryGetComponent<BoxCollider>(out component))
      component.center = new Vector3(0.0f, 0.0f, -0.15f);
    this.RebuildUberText();
    this.LocalBounds = bounds;
    this.UpdateIconPosition();
  }
}
