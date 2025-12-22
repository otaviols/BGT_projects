using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class W8Touch : ITouchScreenService, IService, IHasUpdate
{
  private bool m_initialized;
  public bool m_isWindows8OrGreater;
  private IntPtr m_DLL = IntPtr.Zero;
  private int m_intializationAttemptCount;
  private W8Touch.TouchState[] m_touchState;
  private Vector3 m_touchPosition = new Vector3(-1f, -1f, 0.0f);
  private Vector2 m_touchDelta = new Vector2(0.0f, 0.0f);
  private W8Touch.RECT m_desktopRect;
  private bool m_isVirtualKeyboardVisible;
  private bool m_isVirtualKeyboardShowRequested;
  private bool m_isVirtualKeyboardHideRequested;
  private PowerSource m_lastPowerSourceState = PowerSource.Unintialized;
  private bool m_bWindowFeedbackSettingValue;
  private bool m_bIsWindowFeedbackDisabled;
  private static W8Touch.DelW8ShowKeyboard DLL_W8ShowKeyboard;
  private static W8Touch.DelW8HideKeyboard DLL_W8HideKeyboard;
  private static W8Touch.DelW8ShowOSK DLL_W8ShowOSK;
  private static W8Touch.DelW8Initialize DLL_W8Initialize;
  private static W8Touch.DelW8Shutdown DLL_W8Shutdown;
  private static W8Touch.DelW8GetDeviceId DLL_W8GetDeviceId;
  private static W8Touch.DelW8IsWindows8OrGreater DLL_W8IsWindows8OrGreater;
  private static W8Touch.DelW8IsLastEventFromTouch DLL_W8IsLastEventFromTouch;
  private static W8Touch.DelW8GetBatteryMode DLL_W8GetBatteryMode;
  private static W8Touch.DelW8GetPercentBatteryLife DLL_W8GetPercentBatteryLife;
  private static W8Touch.DelW8GetDesktopRect DLL_W8GetDesktopRect;
  private static W8Touch.DelW8IsVirtualKeyboardVisible DLL_W8IsVirtualKeyboardVisible;
  private static W8Touch.DelW8GetTouchPointCount DLL_W8GetTouchPointCount;
  private static W8Touch.DelW8GetTouchPoint DLL_W8GetTouchPoint;

  private event Action VirtualKeyboardDidShow;

  private event Action VirtualKeyboardDidHide;

  [DllImport("User32.dll")]
  public static extern IntPtr FindWindow(string className, string windowName);

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    W8Touch w8Touch = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if (w8Touch.LoadW8TouchDLL())
      w8Touch.m_isWindows8OrGreater = W8Touch.DLL_W8IsWindows8OrGreater();
    w8Touch.m_touchState = new W8Touch.TouchState[5];
    for (int index = 0; index < 5; ++index)
      w8Touch.m_touchState[index] = W8Touch.TouchState.None;
    if ((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null)
      HearthstoneApplication.Get().OnShutdown += new Action(w8Touch.OnApplicationQuit);
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (UniversalInputManager)
  };

  public void Shutdown() => HearthstoneApplication.Get().OnShutdown -= new Action(this.OnApplicationQuit);

  public void Update()
  {
    if (!this.m_initialized)
      this.InitializeDLL();
    if (!this.IsInitialized())
      return;
    W8Touch.DLL_W8GetDesktopRect(out this.m_desktopRect);
    bool flag1 = W8Touch.DLL_W8IsVirtualKeyboardVisible();
    if (flag1 != this.m_isVirtualKeyboardVisible)
    {
      this.m_isVirtualKeyboardVisible = flag1;
      if (flag1 && this.VirtualKeyboardDidShow != null)
        this.VirtualKeyboardDidShow();
      else if (!flag1 && this.VirtualKeyboardDidHide != null)
        this.VirtualKeyboardDidHide();
    }
    if (this.m_isVirtualKeyboardVisible)
      this.m_isVirtualKeyboardShowRequested = false;
    else
      this.m_isVirtualKeyboardHideRequested = false;
    PowerSource batteryMode = this.GetBatteryMode();
    GraphicsManager service;
    if (batteryMode != this.m_lastPowerSourceState && ServiceManager.TryGet<GraphicsManager>(out service))
    {
      Log.W8Touch.Print("PowerSource Change Detected: {0}", (object) batteryMode);
      this.m_lastPowerSourceState = batteryMode;
      service.RenderQualityLevel = (GraphicsQuality) Options.Get().GetInt(Option.GFX_QUALITY);
    }
    if (!W8Touch.DLL_W8IsLastEventFromTouch() && UniversalInputManager.Get().UseWindowsTouch() || W8Touch.DLL_W8IsLastEventFromTouch() && !UniversalInputManager.Get().UseWindowsTouch())
      this.ToggleTouchMode();
    if (this.m_touchState == null)
      return;
    int num = W8Touch.DLL_W8GetTouchPointCount();
    for (int i = 0; i < 5; ++i)
    {
      W8Touch.tTouchData n = new W8Touch.tTouchData();
      bool flag2 = false;
      if (i < num)
        flag2 = W8Touch.DLL_W8GetTouchPoint(i, n);
      if (flag2 && i == 0)
      {
        Vector2 vector2 = this.TransformTouchPosition(new Vector2((float) n.m_x, (float) n.m_y));
        if ((double) this.m_touchPosition.x != -1.0 && (double) this.m_touchPosition.y != -1.0 && this.m_touchState[i] == W8Touch.TouchState.Down)
        {
          this.m_touchDelta.x = vector2.x - this.m_touchPosition.x;
          this.m_touchDelta.y = vector2.y - this.m_touchPosition.y;
        }
        else
          this.m_touchDelta.x = this.m_touchDelta.y = 0.0f;
        this.m_touchPosition.x = vector2.x;
        this.m_touchPosition.y = vector2.y;
      }
      this.m_touchState[i] = !flag2 || n.m_ID == -1 ? (this.m_touchState[i] == W8Touch.TouchState.Down || this.m_touchState[i] == W8Touch.TouchState.InitialDown ? W8Touch.TouchState.InitialUp : W8Touch.TouchState.None) : (this.m_touchState[i] == W8Touch.TouchState.Down || this.m_touchState[i] == W8Touch.TouchState.InitialDown ? W8Touch.TouchState.Down : W8Touch.TouchState.InitialDown);
    }
  }

  private Vector2 TransformTouchPosition(Vector2 touchInput)
  {
    Vector2 vector2 = new Vector2();
    if (Screen.fullScreen)
    {
      float num1 = (float) Screen.width / (float) Screen.height;
      float num2 = (float) this.m_desktopRect.Right / (float) this.m_desktopRect.Bottom;
      if ((double) Mathf.Abs(num1 - num2) < (double) Mathf.Epsilon)
      {
        float num3 = (float) Screen.width / (float) this.m_desktopRect.Right;
        float num4 = (float) Screen.height / (float) this.m_desktopRect.Bottom;
        vector2.x = touchInput.x * num3;
        vector2.y = ((float) this.m_desktopRect.Bottom - touchInput.y) * num4;
      }
      else if ((double) num1 < (double) num2)
      {
        float bottom = (float) this.m_desktopRect.Bottom;
        float num5 = bottom * num1;
        float num6 = (float) Screen.height / bottom;
        float num7 = (float) Screen.width / num5;
        float num8 = (float) (((double) this.m_desktopRect.Right - (double) num5) / 2.0);
        vector2.x = (touchInput.x - num8) * num7;
        vector2.y = ((float) this.m_desktopRect.Bottom - touchInput.y) * num6;
      }
      else
      {
        float right = (float) this.m_desktopRect.Right;
        float num9 = right / num1;
        float num10 = (float) Screen.height / num9;
        float num11 = (float) Screen.width / right;
        float num12 = (float) (((double) this.m_desktopRect.Bottom - (double) num9) / 2.0);
        vector2.x = touchInput.x * num11;
        vector2.y = ((float) this.m_desktopRect.Bottom - touchInput.y - num12) * num10;
      }
    }
    else
    {
      vector2.x = touchInput.x;
      vector2.y = (float) Screen.height - touchInput.y;
    }
    return vector2;
  }

  private void ToggleTouchMode()
  {
    if (!this.IsInitialized())
      return;
    bool flag = Options.Get().GetBool(Option.TOUCH_MODE);
    Options.Get().SetBool(Option.TOUCH_MODE, !flag);
  }

  public void ShowKeyboard()
  {
    if (!this.IsInitialized() || this.m_isVirtualKeyboardShowRequested || this.m_isVirtualKeyboardVisible && !this.m_isVirtualKeyboardHideRequested)
      return;
    if (this.m_isVirtualKeyboardHideRequested)
      this.m_isVirtualKeyboardHideRequested = false;
    W8Touch.KeyboardFlags keyboardFlags = (W8Touch.KeyboardFlags) W8Touch.DLL_W8ShowKeyboard();
    int num = (int) (keyboardFlags & W8Touch.KeyboardFlags.Shown);
    if ((keyboardFlags & W8Touch.KeyboardFlags.Shown) != W8Touch.KeyboardFlags.Shown || (keyboardFlags & W8Touch.KeyboardFlags.SuccessTabTip) != W8Touch.KeyboardFlags.SuccessTabTip)
      return;
    this.m_isVirtualKeyboardShowRequested = true;
  }

  public void HideKeyboard()
  {
    if (!this.IsInitialized() && !this.m_isVirtualKeyboardVisible)
      return;
    if (this.m_isVirtualKeyboardShowRequested)
      this.m_isVirtualKeyboardShowRequested = false;
    if (W8Touch.DLL_W8HideKeyboard() != 0)
      return;
    this.m_isVirtualKeyboardHideRequested = true;
  }

  public string GetIntelDeviceName() => !this.IsInitialized() ? (string) null : W8Touch.IntelDevice.GetDeviceName(W8Touch.DLL_W8GetDeviceId());

  public PowerSource GetBatteryMode() => !this.IsInitialized() ? PowerSource.Unintialized : (PowerSource) W8Touch.DLL_W8GetBatteryMode();

  public bool IsVirtualKeyboardVisible() => this.IsInitialized() && this.m_isVirtualKeyboardVisible;

  public Vector3 GetTouchPosition() => !this.IsInitialized() || this.m_touchState == null ? new Vector3(0.0f, 0.0f, 0.0f) : new Vector3(this.m_touchPosition.x, this.m_touchPosition.y, this.m_touchPosition.z);

  public Vector3 GetTouchPositionForGUI()
  {
    if (!this.IsInitialized() || this.m_touchState == null)
      return new Vector3(0.0f, 0.0f, 0.0f);
    Vector2 vector2 = this.TransformTouchPosition((Vector2) this.m_touchPosition);
    return new Vector3(vector2.x, vector2.y, this.m_touchPosition.z);
  }

  public bool IsTouchSupported() => this.m_isWindows8OrGreater;

  public void AddOnVirtualKeyboardShowListener(Action listener)
  {
    this.VirtualKeyboardDidShow -= listener;
    this.VirtualKeyboardDidShow += listener;
  }

  public void RemoveOnVirtualKeyboardShowListener(Action listener) => this.VirtualKeyboardDidShow -= listener;

  public void AddOnVirtualKeyboardHideListener(Action listener)
  {
    this.VirtualKeyboardDidHide -= listener;
    this.VirtualKeyboardDidHide += listener;
  }

  public void RemoveOnVirtualKeyboardHideListener(Action listener) => this.VirtualKeyboardDidHide -= listener;

  private IntPtr GetFunction(string name)
  {
    IntPtr procAddress = DLLUtils.GetProcAddress(this.m_DLL, name);
    if (!(procAddress == IntPtr.Zero))
      return procAddress;
    Debug.LogError((object) ("Could not load W8TouchDLL." + name + "()"));
    this.OnApplicationQuit();
    return procAddress;
  }

  private bool LoadW8TouchDLL()
  {
    if (Environment.OSVersion.Version.Major < 6 || Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor < 2)
    {
      Log.W8Touch.Print("Windows Version is Pre-Windows 8");
      return false;
    }
    if (this.m_DLL == IntPtr.Zero)
    {
      this.m_DLL = DLLUtils.LoadPlugin("W8TouchDLL", false);
      if (this.m_DLL == IntPtr.Zero)
      {
        Log.W8Touch.Print("Could not load W8TouchDLL.dll");
        return false;
      }
    }
    W8Touch.DLL_W8ShowKeyboard = (W8Touch.DelW8ShowKeyboard) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_ShowKeyboard"), typeof (W8Touch.DelW8ShowKeyboard));
    W8Touch.DLL_W8HideKeyboard = (W8Touch.DelW8HideKeyboard) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_HideKeyboard"), typeof (W8Touch.DelW8HideKeyboard));
    W8Touch.DLL_W8ShowOSK = (W8Touch.DelW8ShowOSK) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_ShowOSK"), typeof (W8Touch.DelW8ShowOSK));
    W8Touch.DLL_W8Initialize = (W8Touch.DelW8Initialize) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_Initialize"), typeof (W8Touch.DelW8Initialize));
    W8Touch.DLL_W8Shutdown = (W8Touch.DelW8Shutdown) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_Shutdown"), typeof (W8Touch.DelW8Shutdown));
    W8Touch.DLL_W8GetDeviceId = (W8Touch.DelW8GetDeviceId) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_GetDeviceId"), typeof (W8Touch.DelW8GetDeviceId));
    W8Touch.DLL_W8IsWindows8OrGreater = (W8Touch.DelW8IsWindows8OrGreater) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_IsWindows8OrGreater"), typeof (W8Touch.DelW8IsWindows8OrGreater));
    W8Touch.DLL_W8IsLastEventFromTouch = (W8Touch.DelW8IsLastEventFromTouch) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_IsLastEventFromTouch"), typeof (W8Touch.DelW8IsLastEventFromTouch));
    W8Touch.DLL_W8GetBatteryMode = (W8Touch.DelW8GetBatteryMode) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_GetBatteryMode"), typeof (W8Touch.DelW8GetBatteryMode));
    W8Touch.DLL_W8GetPercentBatteryLife = (W8Touch.DelW8GetPercentBatteryLife) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_GetPercentBatteryLife"), typeof (W8Touch.DelW8GetPercentBatteryLife));
    W8Touch.DLL_W8GetDesktopRect = (W8Touch.DelW8GetDesktopRect) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_GetDesktopRect"), typeof (W8Touch.DelW8GetDesktopRect));
    W8Touch.DLL_W8IsVirtualKeyboardVisible = (W8Touch.DelW8IsVirtualKeyboardVisible) Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_IsVirtualKeyboardVisible"), typeof (W8Touch.DelW8IsVirtualKeyboardVisible));
    W8Touch.DLL_W8GetTouchPointCount = (W8Touch.DelW8GetTouchPointCount) Marshal.GetDelegateForFunctionPointer(this.GetFunction("GetTouchPointCount"), typeof (W8Touch.DelW8GetTouchPointCount));
    W8Touch.DLL_W8GetTouchPoint = (W8Touch.DelW8GetTouchPoint) Marshal.GetDelegateForFunctionPointer(this.GetFunction("GetTouchPoint"), typeof (W8Touch.DelW8GetTouchPoint));
    return true;
  }

  private void OnApplicationQuit()
  {
    Log.W8Touch.Print("W8Touch.AppQuit()");
    if (this.m_DLL == IntPtr.Zero)
      return;
    this.ResetWindowFeedbackSetting();
    if (W8Touch.DLL_W8Shutdown != null && this.m_initialized)
    {
      W8Touch.DLL_W8Shutdown();
      this.m_initialized = false;
    }
    if (!DLLUtils.FreeLibrary(this.m_DLL))
      Debug.Log((object) "Error unloading W8TouchDLL.dll");
    this.m_DLL = IntPtr.Zero;
  }

  private bool IsInitialized() => this.m_DLL != IntPtr.Zero && this.m_isWindows8OrGreater && this.m_initialized;

  private void InitializeDLL()
  {
    if (this.m_intializationAttemptCount >= 10)
      return;
    string windowName = GameStrings.Get("GLOBAL_PROGRAMNAME_HEARTHSTONE");
    int num1 = -1;
    if (W8Touch.DLL_W8Initialize != null)
      num1 = W8Touch.DLL_W8Initialize(windowName);
    if (num1 < 0)
    {
      ++this.m_intializationAttemptCount;
    }
    else
    {
      Log.W8Touch.Print("W8Touch Start Success!");
      this.m_initialized = true;
      IntPtr module = DLLUtils.LoadLibrary("User32.DLL");
      if (module == IntPtr.Zero)
      {
        Log.W8Touch.Print("Could not load User32.DLL");
      }
      else
      {
        IntPtr procAddress = DLLUtils.GetProcAddress(module, "SetWindowFeedbackSetting");
        if (procAddress == IntPtr.Zero)
        {
          Log.W8Touch.Print("Could not load User32.SetWindowFeedbackSetting()");
        }
        else
        {
          IntPtr window = W8Touch.FindWindow((string) null, "Hearthstone");
          if (window == IntPtr.Zero)
            window = W8Touch.FindWindow((string) null, GameStrings.Get("GLOBAL_PROGRAMNAME_HEARTHSTONE"));
          if (window == IntPtr.Zero)
          {
            Log.W8Touch.Print("Unable to retrieve Hearthstone window handle!");
          }
          else
          {
            W8Touch.DelSetWindowFeedbackSetting forFunctionPointer = (W8Touch.DelSetWindowFeedbackSetting) Marshal.GetDelegateForFunctionPointer(procAddress, typeof (W8Touch.DelSetWindowFeedbackSetting));
            int cb = Marshal.SizeOf(typeof (int));
            IntPtr num2 = Marshal.AllocHGlobal(cb);
            Marshal.WriteInt32(num2, 0, this.m_bWindowFeedbackSettingValue ? 1 : 0);
            bool flag = true;
            if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_CONTACTVISUALIZATION, 0U, Convert.ToUInt32(cb), num2))
            {
              Log.W8Touch.Print("FEEDBACK_TOUCH_CONTACTVISUALIZATION failed!");
              flag = false;
            }
            if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_TAP, 0U, Convert.ToUInt32(cb), num2))
            {
              Log.W8Touch.Print("FEEDBACK_TOUCH_TAP failed!");
              flag = false;
            }
            if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_PRESSANDHOLD, 0U, Convert.ToUInt32(cb), num2))
            {
              Log.W8Touch.Print("FEEDBACK_TOUCH_PRESSANDHOLD failed!");
              flag = false;
            }
            if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_DOUBLETAP, 0U, Convert.ToUInt32(cb), num2))
            {
              Log.W8Touch.Print("FEEDBACK_TOUCH_DOUBLETAP failed!");
              flag = false;
            }
            if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_RIGHTTAP, 0U, Convert.ToUInt32(cb), num2))
            {
              Log.W8Touch.Print("FEEDBACK_TOUCH_RIGHTTAP failed!");
              flag = false;
            }
            if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_GESTURE_PRESSANDTAP, 0U, Convert.ToUInt32(cb), num2))
            {
              Log.W8Touch.Print("FEEDBACK_GESTURE_PRESSANDTAP failed!");
              flag = false;
            }
            this.m_bIsWindowFeedbackDisabled = flag;
            if (this.m_bIsWindowFeedbackDisabled)
              Log.W8Touch.Print("Windows 8 Feedback Touch Gestures Disabled!");
            Marshal.FreeHGlobal(num2);
          }
        }
        if (DLLUtils.FreeLibrary(module))
          return;
        Log.W8Touch.Print("Error unloading User32.dll");
      }
    }
  }

  private void ResetWindowFeedbackSetting()
  {
    if (!this.m_initialized || !this.m_bIsWindowFeedbackDisabled)
      return;
    IntPtr module = DLLUtils.LoadLibrary("User32.DLL");
    if (module == IntPtr.Zero)
    {
      Log.W8Touch.Print("Could not load User32.DLL");
    }
    else
    {
      IntPtr procAddress = DLLUtils.GetProcAddress(module, "SetWindowFeedbackSetting");
      if (procAddress == IntPtr.Zero)
      {
        Log.W8Touch.Print("Could not load User32.SetWindowFeedbackSetting()");
      }
      else
      {
        IntPtr window = W8Touch.FindWindow((string) null, "Hearthstone");
        if (window == IntPtr.Zero)
          window = W8Touch.FindWindow((string) null, GameStrings.Get("GLOBAL_PROGRAMNAME_HEARTHSTONE"));
        if (window == IntPtr.Zero)
        {
          Log.W8Touch.Print("Unable to retrieve Hearthstone window handle!");
        }
        else
        {
          W8Touch.DelSetWindowFeedbackSetting forFunctionPointer = (W8Touch.DelSetWindowFeedbackSetting) Marshal.GetDelegateForFunctionPointer(procAddress, typeof (W8Touch.DelSetWindowFeedbackSetting));
          IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (int)));
          Marshal.WriteInt32(num, 0, this.m_bWindowFeedbackSettingValue ? 1 : 0);
          bool flag = true;
          if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_CONTACTVISUALIZATION, 0U, 0U, IntPtr.Zero))
          {
            Log.W8Touch.Print("FEEDBACK_TOUCH_CONTACTVISUALIZATION failed!");
            flag = false;
          }
          if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_TAP, 0U, 0U, IntPtr.Zero))
          {
            Log.W8Touch.Print("FEEDBACK_TOUCH_TAP failed!");
            flag = false;
          }
          if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_PRESSANDHOLD, 0U, 0U, IntPtr.Zero))
          {
            Log.W8Touch.Print("FEEDBACK_TOUCH_PRESSANDHOLD failed!");
            flag = false;
          }
          if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_DOUBLETAP, 0U, 0U, IntPtr.Zero))
          {
            Log.W8Touch.Print("FEEDBACK_TOUCH_DOUBLETAP failed!");
            flag = false;
          }
          if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_RIGHTTAP, 0U, 0U, IntPtr.Zero))
          {
            Log.W8Touch.Print("FEEDBACK_TOUCH_RIGHTTAP failed!");
            flag = false;
          }
          if (!forFunctionPointer(window, W8Touch.FEEDBACK_TYPE.FEEDBACK_GESTURE_PRESSANDTAP, 0U, 0U, IntPtr.Zero))
          {
            Log.W8Touch.Print("FEEDBACK_GESTURE_PRESSANDTAP failed!");
            flag = false;
          }
          this.m_bIsWindowFeedbackDisabled = !flag;
          if (!this.m_bIsWindowFeedbackDisabled)
            Log.W8Touch.Print("Windows 8 Feedback Touch Gestures Reset!");
          Marshal.FreeHGlobal(num);
        }
      }
      if (DLLUtils.FreeLibrary(module))
        return;
      Log.W8Touch.Print("Error unloading User32.dll");
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public class tTouchData
  {
    public int m_x;
    public int m_y;
    public int m_ID;
    public int m_Time;
  }

  public struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
  }

  [Flags]
  public enum KeyboardFlags
  {
    Shown = 1,
    NotShown = 2,
    SuccessTabTip = 4,
    SuccessOSK = 8,
    ErrorTabTip = 16, // 0x00000010
    ErrorOSK = 32, // 0x00000020
    NotFoundTabTip = 64, // 0x00000040
    NotFoundOSK = 128, // 0x00000080
  }

  public enum TouchState
  {
    None,
    InitialDown,
    Down,
    InitialUp,
  }

  public class IntelDevice
  {
    private static readonly Map<int, string> DeviceIdMap = new Map<int, string>()
    {
      {
        30720,
        "Auburn"
      },
      {
        28961,
        "Whitney"
      },
      {
        28963,
        "Whitney"
      },
      {
        28965,
        "Whitney"
      },
      {
        4402,
        "Solono"
      },
      {
        9570,
        "Brookdale"
      },
      {
        13698,
        "Montara"
      },
      {
        9586,
        "Springdale"
      },
      {
        9602,
        "Grantsdale"
      },
      {
        10114,
        "Grantsdale"
      },
      {
        9618,
        "Alviso"
      },
      {
        10130,
        "Alviso"
      },
      {
        10098,
        "Lakeport-G"
      },
      {
        10102,
        "Lakeport-G"
      },
      {
        10146,
        "Calistoga"
      },
      {
        10150,
        "Calistoga"
      },
      {
        10626,
        "Broadwater-G"
      },
      {
        10627,
        "Broadwater-G"
      },
      {
        10610,
        "Broadwater-G"
      },
      {
        10611,
        "Broadwater-G"
      },
      {
        10642,
        "Broadwater-G"
      },
      {
        10643,
        "Broadwater-G"
      },
      {
        10658,
        "Broadwater-G"
      },
      {
        10659,
        "Broadwater-G"
      },
      {
        10754,
        "Crestline"
      },
      {
        10755,
        "Crestline"
      },
      {
        10770,
        "Crestline"
      },
      {
        10771,
        "Crestline"
      },
      {
        10674,
        "Bearlake"
      },
      {
        10675,
        "Bearlake"
      },
      {
        10690,
        "Bearlake"
      },
      {
        10691,
        "Bearlake"
      },
      {
        10706,
        "Bearlake"
      },
      {
        10707,
        "Bearlake"
      },
      {
        10818,
        "Cantiga"
      },
      {
        10819,
        "Cantiga"
      },
      {
        11778,
        "Eaglelake"
      },
      {
        11779,
        "Eaglelake"
      },
      {
        11810,
        "Eaglelake"
      },
      {
        11811,
        "Eaglelake"
      },
      {
        11794,
        "Eaglelake"
      },
      {
        11795,
        "Eaglelake"
      },
      {
        11826,
        "Eaglelake"
      },
      {
        11827,
        "Eaglelake"
      },
      {
        11842,
        "Eaglelake"
      },
      {
        11843,
        "Eaglelake"
      },
      {
        11922,
        "Eaglelake"
      },
      {
        11923,
        "Eaglelake"
      },
      {
        70,
        "Arrandale"
      },
      {
        66,
        "Clarkdale"
      },
      {
        262,
        "Mobile_SandyBridge_GT1"
      },
      {
        278,
        "Mobile_SandyBridge_GT2"
      },
      {
        294,
        "Mobile_SandyBridge_GT2+"
      },
      {
        258,
        "DT_SandyBridge_GT2+"
      },
      {
        274,
        "DT_SandyBridge_GT2+"
      },
      {
        290,
        "DT_SandyBridge_GT2+"
      },
      {
        266,
        "SandyBridge_Server"
      },
      {
        270,
        "SandyBridge_Reserved"
      },
      {
        338,
        "Desktop_IvyBridge_GT1"
      },
      {
        342,
        "Mobile_IvyBridge_GT1"
      },
      {
        346,
        "Server_IvyBridge_GT1"
      },
      {
        350,
        "Reserved_IvyBridge_GT1"
      },
      {
        354,
        "Desktop_IvyBridge_GT2"
      },
      {
        358,
        "Mobile_IvyBridge_GT2"
      },
      {
        362,
        "Server_IvyBridge_GT2"
      },
      {
        1026,
        "Desktop_Haswell_GT1_Y6W"
      },
      {
        1030,
        "Mobile_Haswell_GT1_Y6W"
      },
      {
        1034,
        "Server_Haswell_GT1"
      },
      {
        1042,
        "Desktop_Haswell_GT2_U15W"
      },
      {
        1046,
        "Mobile_Haswell_GT2_U15W"
      },
      {
        1051,
        "Workstation_Haswell_GT2"
      },
      {
        1050,
        "Server_Haswell_GT2"
      },
      {
        1054,
        "Reserved_Haswell_DT_GT1.5_U15W"
      },
      {
        2566,
        "Mobile_Haswell_ULT_GT1_Y6W"
      },
      {
        2574,
        "Mobile_Haswell_ULX_GT1_Y6W"
      },
      {
        2582,
        "Mobile_Haswell_ULT_GT2_U15W"
      },
      {
        2590,
        "Mobile_Haswell_ULX_GT2_Y6W"
      },
      {
        2598,
        "Mobile_Haswell_ULT_GT3_U28W"
      },
      {
        2606,
        "Mobile_Haswell_ULT_GT3@28_U28W"
      },
      {
        3346,
        "Desktop_Haswell_GT2F"
      },
      {
        3350,
        "Mobile_Haswell_GT2F"
      },
      {
        3362,
        "Desktop_Crystal-Well_GT3"
      },
      {
        3366,
        "Mobile_Crystal-Well_GT3"
      },
      {
        3370,
        "Server_Crystal-Well_GT3"
      },
      {
        3889,
        "BayTrail"
      },
      {
        33032,
        "Poulsbo"
      },
      {
        33033,
        "Poulsbo"
      },
      {
        2255,
        "CloverTrail"
      },
      {
        40961,
        "CloverTrail"
      },
      {
        40962,
        "CloverTrail"
      },
      {
        40977,
        "CloverTrail"
      },
      {
        40978,
        "CloverTrail"
      }
    };

    public static string GetDeviceName(int deviceId)
    {
      string str;
      return !W8Touch.IntelDevice.DeviceIdMap.TryGetValue(deviceId, out str) ? "" : str;
    }
  }

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate int DelW8ShowKeyboard();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate int DelW8HideKeyboard();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate int DelW8ShowOSK();

  [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto)]
  private delegate int DelW8Initialize(string windowName);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate void DelW8Shutdown();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate int DelW8GetDeviceId();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate bool DelW8IsWindows8OrGreater();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate bool DelW8IsLastEventFromTouch();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate int DelW8GetBatteryMode();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate int DelW8GetPercentBatteryLife();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate void DelW8GetDesktopRect(out W8Touch.RECT desktopRect);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate bool DelW8IsVirtualKeyboardVisible();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate int DelW8GetTouchPointCount();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate bool DelW8GetTouchPoint(int i, W8Touch.tTouchData n);

  public enum FEEDBACK_TYPE
  {
    FEEDBACK_TOUCH_CONTACTVISUALIZATION = 1,
    FEEDBACK_PEN_BARRELVISUALIZATION = 2,
    FEEDBACK_PEN_TAP = 3,
    FEEDBACK_PEN_DOUBLETAP = 4,
    FEEDBACK_PEN_PRESSANDHOLD = 5,
    FEEDBACK_PEN_RIGHTTAP = 6,
    FEEDBACK_TOUCH_TAP = 7,
    FEEDBACK_TOUCH_DOUBLETAP = 8,
    FEEDBACK_TOUCH_PRESSANDHOLD = 9,
    FEEDBACK_TOUCH_RIGHTTAP = 10, // 0x0000000A
    FEEDBACK_GESTURE_PRESSANDTAP = 11, // 0x0000000B
  }

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate bool DelSetWindowFeedbackSetting(
    IntPtr hwnd,
    W8Touch.FEEDBACK_TYPE feedback,
    uint dwFlags,
    uint size,
    IntPtr configuration);
}
