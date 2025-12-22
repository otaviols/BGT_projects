using SwrveUnity;
using SwrveUnity.Messaging;
using SwrveUnity.ResourceManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwrveEmpty : SwrveSDK
{
  public override void Init(MonoBehaviour container, int appId, string apiKey, SwrveConfig config = null)
  {
    this.Container = container;
    this.ResourceManager = new SwrveResourceManager();
    this.prefabName = container.name;
    this.appId = appId;
    this.apiKey = apiKey;
    this.config = config;
    this.Language = config.Language;
    this.Initialised = true;
  }

  public override bool SendQueuedEvents() => true;

  public override void FlushToDisk(bool saveEventsBeingSent = false)
  {
  }

  public override bool IsMessageDisplaying() => false;

  public override SwrveBaseMessage GetBaseMessageForEvent(
    string eventName,
    IDictionary<string, string> payload)
  {
    return (SwrveBaseMessage) null;
  }

  public override SwrveConversation GetConversationForEvent(
    string eventName,
    IDictionary<string, string> payload = null)
  {
    return (SwrveConversation) null;
  }

  public override IEnumerator ShowMessageForEvent(
    string eventName,
    IDictionary<string, string> payload,
    SwrveBaseMessage message,
    ISwrveInstallButtonListener installButtonListener = null,
    ISwrveCustomButtonListener customButtonListener = null,
    ISwrveMessageListener messageListener = null,
    ISwrveClipboardButtonListener clipboardButtonListener = null,
    ISwrveEmbeddedMessageListener embeddedMessageListener = null)
  {
    yield return (object) null;
  }

  public override IEnumerator ShowConversationForEvent(
    string eventName,
    SwrveConversation conversation)
  {
    yield return (object) null;
  }

  public override void DismissMessage()
  {
  }

  public override void RefreshUserResourcesAndCampaigns()
  {
  }

  public override void SessionStart()
  {
  }

  public override void UserUpdate(Dictionary<string, string> attributes)
  {
  }

  public override void LoadFromDisk()
  {
  }

  public override Dictionary<string, string> GetDeviceInfo() => new Dictionary<string, string>();

  public override void OnSwrvePause()
  {
  }

  public override void OnSwrveResume()
  {
  }

  public override void OnSwrveDestroy()
  {
  }

  public override void ButtonWasPressedByUser(SwrveButton button)
  {
  }

  public override void MessageWasShownToUser(SwrveMessageFormat messageFormat)
  {
  }
}
