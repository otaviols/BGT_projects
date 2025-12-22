using Blizzard.T5.Services;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
  public BaseUIBones m_Bones;
  public BaseUIPrefabs m_Prefabs;
  public BnetBar m_BnetBar;
  private static BaseUI s_instance;

  public Camera m_BnetCamera { get; private set; }

  public Camera m_BnetDialogCamera { get; private set; }

  private void Awake()
  {
    BaseUI.s_instance = this;
    this.m_BnetCamera = CameraUtils.FindFirstByLayer(GameLayer.BattleNet);
    this.m_BnetDialogCamera = CameraUtils.FindFirstByLayer(GameLayer.BattleNetDialog);
    UnityEngine.Object.Instantiate<ChatMgr>(this.m_Prefabs.m_ChatMgrPrefab, this.transform.position, Quaternion.identity).transform.parent = this.transform;
    this.m_BnetCamera.GetComponent<ScreenResizeDetector>().AddSizeChangedListener(new ScreenResizeDetector.SizeChangedCallback(this.OnScreenSizeChanged));
    this.gameObject.AddComponent<HSDontDestroyOnLoad>();
  }

  private void OnDestroy() => BaseUI.s_instance = (BaseUI) null;

  private void Start()
  {
    this.UpdateLayout();
    InnKeepersSpecial.Init();
  }

  public static BaseUI Get() => BaseUI.s_instance;

  public void OnLoggedIn() => this.m_BnetBar.OnLoggedIn();

  public Camera GetBnetCamera() => this.m_BnetCamera;

  public Camera GetBnetDialogCamera() => this.m_BnetDialogCamera;

  public Transform GetAddFriendBone()
  {
    if (!UniversalInputManager.Get().IsTouchMode())
      return this.m_Bones.m_AddFriend;
    return (bool) UniversalInputManager.UsePhoneUI ? this.m_Bones.m_AddFriendPhoneKeyboard : this.m_Bones.m_AddFriendVirtualKeyboard;
  }

  public Transform GetRecruitAFriendBone() => this.m_Bones.m_RecruitAFriend;

  public Transform GetChatBubbleBone() => this.m_Bones.m_ChatBubble;

  public Transform GetGameMenuBone(bool withRatings = false)
  {
    if (SceneMgr.Get().IsInGame())
      return this.m_Bones.m_InGameMenu;
    return !withRatings ? this.m_Bones.m_BoxMenu : this.m_Bones.m_BoxMenuWithRatings;
  }

  public Transform GetOptionsMenuBone() => this.m_Bones.m_OptionsMenu;

  public Transform GetQuickChatBone()
  {
    ITouchScreenService touchScreenService = ServiceManager.Get<ITouchScreenService>();
    return UniversalInputManager.Get().IsTouchMode() && touchScreenService.IsTouchSupported() || touchScreenService.IsVirtualKeyboardVisible() ? this.m_Bones.m_QuickChatVirtualKeyboard : this.m_Bones.m_QuickChat;
  }

  public Transform GetFriendsListTutorialNotificationBone() => this.m_Bones.m_FriendsListTutorialNotification;

  public bool HandleKeyboardInput()
  {
    if ((UnityEngine.Object) this.m_BnetBar != (UnityEngine.Object) null && this.m_BnetBar.HandleKeyboardInput())
      return true;
    if ((InputCollection.GetKey(KeyCode.LeftControl) || InputCollection.GetKey(KeyCode.RightControl) || InputCollection.GetKey(KeyCode.LeftCommand) || InputCollection.GetKey(KeyCode.RightCommand)) && (InputCollection.GetKey(KeyCode.LeftShift) || InputCollection.GetKey(KeyCode.RightShift)) && InputCollection.GetKeyDown(KeyCode.S) && Options.Get() != null)
    {
      bool flag = Options.Get().GetBool(Option.STREAMER_MODE);
      Options.Get().SetBool(Option.STREAMER_MODE, !flag);
    }
    if (InputCollection.GetKeyUp(KeyCode.Print) || InputCollection.GetKeyUp(KeyCode.SysReq) || InputCollection.GetKeyUp(KeyCode.F13))
      this.StartCoroutine(BaseUI.TakeScreenshot(4f));
    return false;
  }

  public static string SavedScreenshotPath { get; private set; }

  public static string ScreenshotPath
  {
    get
    {
      string path1;
      if (PlatformSettings.IsMobileRuntimeOS)
      {
        path1 = string.Format("{0}/Screenshot.png", (object) Application.persistentDataPath);
        if (File.Exists(path1))
          File.Delete(path1);
      }
      else
      {
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        DateTime now = DateTime.Now;
        string path2 = Options.Get().GetString(Option.SCREENSHOT_DIRECTORY, folderPath);
        if (!Directory.Exists(path2))
          path2 = folderPath;
        path1 = string.Format("{0}/Hearthstone Screenshot {1:MM-dd-yy HH.mm.ss}.png", (object) path2, (object) now);
        int num = 1;
        while (File.Exists(path1))
          path1 = string.Format("{0}/Hearthstone Screenshot {1:MM-dd-yy HH.mm.ss} {2}.png", (object) path2, (object) now, (object) num++);
      }
      return path1;
    }
  }

  public static IEnumerator TakeScreenshot(float maxWaitSeconds)
  {
    BaseUI.SavedScreenshotPath = BaseUI.ScreenshotPath;
    string statusMessage = GameStrings.Get("GLOBAL_SCREENSHOT_COMPLETE");
    if (!PlatformSettings.IsMobileRuntimeOS && !BaseUI.SavedScreenshotPath.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)))
      statusMessage = GameStrings.Format("GLOBAL_SCREENSHOT_COMPLETE_SPECIFIC_DIRECTORY", (object) Path.GetDirectoryName(BaseUI.SavedScreenshotPath));
    UIStatus.Get().HideIfScreenshotMessage();
    ScreenCapture.CaptureScreenshot(PlatformSettings.IsMobileRuntimeOS ? Path.GetFileName(BaseUI.SavedScreenshotPath) : BaseUI.SavedScreenshotPath);
    BaseUI.s_instance.StartCoroutine(BaseUI.NotifyOfScreenshotComplete(statusMessage));
    Log.All.Print(string.Format("screenshot saved to {0}", (object) BaseUI.SavedScreenshotPath));
    yield return (object) BaseUI.WaitUntilFileExists(BaseUI.SavedScreenshotPath, maxWaitSeconds);
  }

  public static string QRCodePath => !PlatformSettings.IsMobile() ? (string) null : string.Format("{0}/QRCode.png", (object) Application.persistentDataPath);

  private void OnScreenSizeChanged(object userData) => this.UpdateLayout();

  private void UpdateLayout()
  {
    this.m_BnetBar.UpdateLayout();
    if (!((UnityEngine.Object) ChatMgr.Get() != (UnityEngine.Object) null))
      return;
    ChatMgr.Get().UpdateLayout();
  }

  private static IEnumerator NotifyOfScreenshotComplete(string statusMessage)
  {
    yield return (object) null;
    UIStatus.Get().AddInfo(statusMessage, UIStatus.StatusType.SCREENSHOT);
  }

  private static IEnumerator WaitUntilFileExists(
    string imageFileName,
    float maxWaitSeconds)
  {
    float seconds = 0.1f;
    float totalCycles = maxWaitSeconds / seconds;
    WaitForSeconds waitForSeconds = new WaitForSeconds(seconds);
    for (int i = 0; (double) i < (double) totalCycles; ++i)
    {
      if (File.Exists(imageFileName))
        yield break;
      else
        yield return (object) waitForSeconds;
    }
    Log.All.PrintWarning(string.Format("screenshot never arrived on fileSystem after {0}s", (object) maxWaitSeconds));
  }
}
