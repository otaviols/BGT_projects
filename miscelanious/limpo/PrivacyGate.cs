using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.APIGateway;
using Hearthstone.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyGate : IService
{
  private Map<PrivacyFeatures, bool> featuresData;

  public static bool IsServiceReady { get; private set; }

  public event Action OnPrivacySettingsUpdated;

  public System.Type[] GetDependencies() => new System.Type[3]
  {
    typeof (FiresideGatheringManager),
    typeof (RAFManager),
    typeof (LoginManager)
  };

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    PrivacyGate privacyGate = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    privacyGate.featuresData = privacyGate.InitFeaturesMap();
    Processor.RunCoroutine(privacyGate.GetOptInData(new Action(privacyGate.OnOptInDataReceived)));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (IAsyncJobResult) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public void Shutdown()
  {
  }

  public static PrivacyGate Get() => ServiceManager.Get<PrivacyGate>();

  public bool FeatureEnabled(PrivacyFeatures privacyFeature)
  {
    if (this.featuresData.ContainsKey(privacyFeature))
      return this.featuresData[privacyFeature];
    Log.Privacy.PrintError("Checking Privacy Feature: Privacy feature does not exist in PrivacyGate: " + privacyFeature.ToString());
    return false;
  }

  public void SetFeature(PrivacyFeatures privacyFeature, bool isEnabled)
  {
    if (this.featuresData.ContainsKey(privacyFeature))
    {
      this.featuresData[privacyFeature] = isEnabled;
      this.SetOptInData(privacyFeature, !isEnabled);
    }
    else
      Log.Privacy.PrintError("Set Privacy feature: Privacy feature does not exist in PrivacyGate: " + privacyFeature.ToString() + ".");
  }

  public void RefreshPrivacySettings() => Processor.RunCoroutine(this.GetOptInData(new Action(this.OnOptInDataReceived)));

  private Map<PrivacyFeatures, bool> InitFeaturesMap()
  {
    Map<PrivacyFeatures, bool> map = new Map<PrivacyFeatures, bool>();
    foreach (PrivacyFeatures key in Enum.GetValues(typeof (PrivacyFeatures)))
    {
      if (key != PrivacyFeatures.INVALID)
        map.Add(key, false);
    }
    return map;
  }

  private IEnumerator GetOptInData(Action dataReceivedCallback)
  {
    yield return (object) new WaitUntil((Func<bool>) (() => LoginManager.Get().OptInsReceivedDependency.IsReady()));
    this.featuresData[PrivacyFeatures.CHAT] = !LoginManager.Get().OptInApi.GetAccountOptIn(OptInApi.OptInType.DISABLE_CHAT);
    this.featuresData[PrivacyFeatures.GEOLOCATION] = !LoginManager.Get().OptInApi.GetAccountOptIn(OptInApi.OptInType.DISABLE_GEOLOCATION);
    this.featuresData[PrivacyFeatures.NEARBY_FRIENDS] = !LoginManager.Get().OptInApi.GetAccountOptIn(OptInApi.OptInType.DISABLE_NEARBY_FRIENDS);
    this.featuresData[PrivacyFeatures.PUSH_NOTIFICATIONS] = !LoginManager.Get().OptInApi.GetAccountOptIn(OptInApi.OptInType.DISABLE_PUSH_NOTIFICATIONS);
    this.featuresData[PrivacyFeatures.PERSONALIZED_STORE_ITEMS] = !LoginManager.Get().OptInApi.GetAccountOptIn(OptInApi.OptInType.DISABLE_PERSONALIZED_PRODUCTS);
    Action action = dataReceivedCallback;
    if (action != null)
      action();
  }

  private void SetOptInData(PrivacyFeatures privacyFeature, bool isDisabled)
  {
    OptInApi.OptInType type = OptInApi.OptInType.INVALID;
    switch (privacyFeature)
    {
      case PrivacyFeatures.CHAT:
        type = OptInApi.OptInType.DISABLE_CHAT;
        break;
      case PrivacyFeatures.GEOLOCATION:
        type = OptInApi.OptInType.DISABLE_GEOLOCATION;
        break;
      case PrivacyFeatures.PERSONALIZED_STORE_ITEMS:
        type = OptInApi.OptInType.DISABLE_PERSONALIZED_PRODUCTS;
        break;
      case PrivacyFeatures.PUSH_NOTIFICATIONS:
        type = OptInApi.OptInType.DISABLE_PUSH_NOTIFICATIONS;
        break;
      case PrivacyFeatures.NEARBY_FRIENDS:
        type = OptInApi.OptInType.DISABLE_NEARBY_FRIENDS;
        break;
    }
    LoginManager.Get().OptInApi.SetAccountOptIn(type, isDisabled);
    Processor.RunCoroutine(this.SetFeaturesStatus());
  }

  private void OnOptInDataReceived() => Processor.RunCoroutine(this.SetFeaturesStatus());

  private IEnumerator SetFeaturesStatus()
  {
    FiresideGatheringManager.Get().SetFSGFeatureStatus(this.FeatureEnabled(PrivacyFeatures.GEOLOCATION));
    yield return (object) new WaitUntil((Func<bool>) (() => (UnityEngine.Object) ChatMgr.Get() != (UnityEngine.Object) null));
    ChatMgr.Get().SetChatFeatureStatus(this.FeatureEnabled(PrivacyFeatures.CHAT));
    yield return (object) new WaitUntil((Func<bool>) (() => BnetFriendMgr.Get() != null));
    BnetFriendMgr.Get().SetFriendInviteFeatureStatus(this.FeatureEnabled(PrivacyFeatures.CHAT));
    yield return (object) new WaitUntil((Func<bool>) (() => (UnityEngine.Object) PushNotificationManager.Get() != (UnityEngine.Object) null));
    PushNotificationManager.Get().SetPushNotificationFeatureStatus(this.FeatureEnabled(PrivacyFeatures.PUSH_NOTIFICATIONS));
    BnetNearbyPlayerMgr.Get().SetEnabled(this.FeatureEnabled(PrivacyFeatures.NEARBY_FRIENDS));
    Options.Get().SetBool(Option.NEARBY_PLAYERS, this.FeatureEnabled(PrivacyFeatures.NEARBY_FRIENDS));
    TelemetryManager.SetTelemetryFeatureStatus(true);
    PrivacyGate.IsServiceReady = true;
    Action privacySettingsUpdated = this.OnPrivacySettingsUpdated;
    if (privacySettingsUpdated != null)
      privacySettingsUpdated();
  }
}
