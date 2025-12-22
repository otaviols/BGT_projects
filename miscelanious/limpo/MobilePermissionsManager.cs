using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;
using UnityEngine;

public class MobilePermissionsManager : IService
{
  private Map<MobilePermission, List<string>> m_androidPermissionMap = new Map<MobilePermission, List<string>>();
  private Map<MobilePermission, List<MobilePermissionsManager.PermissionResultCallback>> m_pendingRequests = new Map<MobilePermission, List<MobilePermissionsManager.PermissionResultCallback>>();

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    this.InitAndroidPermissionStrings();
    int num = Application.isEditor ? 1 : 0;
    yield break;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (MobileCallbackManager)
  };

  public void Shutdown()
  {
  }

  public static MobilePermissionsManager Get() => ServiceManager.Get<MobilePermissionsManager>();

  public void RequestPermission(
    MobilePermission permission,
    MobilePermissionsManager.PermissionResultCallback callback)
  {
    callback(permission, false);
  }

  public bool CheckPermission(MobilePermission permission) => this.CheckPermissionWindows(permission);

  public bool WifiRequiresLocationPermission() => false;

  public bool CheckPermissionWindows(MobilePermission permission) => true;

  private void InitAndroidPermissionStrings()
  {
    this.m_androidPermissionMap[MobilePermission.FINE_LOCATION] = new List<string>()
    {
      "android.permission.ACCESS_FINE_LOCATION"
    };
    this.m_androidPermissionMap[MobilePermission.COARSE_LOCATION] = new List<string>()
    {
      "android.permission.ACCESS_COARSE_LOCATION"
    };
    this.m_androidPermissionMap[MobilePermission.BEACON] = new List<string>()
    {
      "android.permission.ACCESS_COARSE_LOCATION"
    };
    this.m_androidPermissionMap[MobilePermission.WIFI] = new List<string>()
    {
      "android.permission.ACCESS_NETWORK_STATE",
      "android.permission.ACCESS_WIFI_STATE"
    };
    this.m_androidPermissionMap[MobilePermission.BLUETOOTH] = new List<string>()
    {
      "android.permission.BLUETOOTH",
      "android.permission.BLUETOOTH_ADMIN"
    };
    this.m_androidPermissionMap[MobilePermission.CAMERA] = new List<string>()
    {
      "android.permission.CAMERA"
    };
    this.m_androidPermissionMap[MobilePermission.MICROPHONE] = new List<string>()
    {
      "android.permission.RECORD_AUDIO"
    };
    this.m_androidPermissionMap[MobilePermission.GOOGLE_PUSH_NOTIFICATIONS] = new List<string>()
    {
      "com.google.android.c2dm.permission.RECEIVE",
      "com.blizzard.wtcg.hearthstone.permission.C2D_MESSAGE"
    };
    this.m_androidPermissionMap[MobilePermission.AMAZON_PUSH_NOTIFICATIONS] = new List<string>()
    {
      "com.blizzard.wtcg.hearthstone.permission.RECEIVE_ADM_MESSAGE",
      "com.amazon.device.messaging.permission.RECEIVE"
    };
  }

  public delegate void PermissionResultCallback(MobilePermission permission, bool granted);
}
