using Hearthstone;
using UnityEngine;

public static class Error
{
  public static readonly PlatformDependentValue<bool> HAS_APP_STORE = new PlatformDependentValue<bool>(PlatformCategory.OS)
  {
    PC = false,
    Mac = false,
    iOS = true,
    Android = true
  };
  private static bool s_hasShownNonRepeatingDevWarning = false;

  public static void AddWarning(string header, string message, params object[] messageArgs) => Error.AddWarning(new ErrorParams()
  {
    m_header = header,
    m_message = string.Format(message, messageArgs)
  });

  public static void AddWarningLoc(
    string headerKey,
    string messageKey,
    params object[] messageArgs)
  {
    Error.AddWarning(new ErrorParams()
    {
      m_header = GameStrings.Get(headerKey),
      m_message = GameStrings.Format(messageKey, messageArgs)
    });
  }

  public static void AddWarning(ErrorParams parms)
  {
    if (!(bool) (Object) DialogManager.Get())
    {
      parms.m_reason = FatalErrorReason.UNAVAILAVLE_DIALOGMANAGER_FOR_WARNING;
      Error.AddFatal(parms);
    }
    else
    {
      Debug.LogWarning((object) string.Format("Error.AddWarning() - header={0} message={1}", (object) parms.m_header, (object) parms.m_message));
      if (UniversalInputManager.Get() != null)
        UniversalInputManager.Get().CancelTextInput((GameObject) null, true);
      Error.ShowWarningDialog(parms);
    }
  }

  public static void AddDevWarning(string header, string message, params object[] messageArgs)
  {
    string str = string.Format(message, messageArgs);
    if (!Debug.isDebugBuild)
      Debug.LogWarning((object) string.Format("Error.AddDevWarning() - header={0} message={1}", (object) header, (object) str));
    else
      Error.AddWarning(new ErrorParams()
      {
        m_header = header,
        m_message = str
      });
  }

  public static void AddDevWarningNonRepeating(
    string header,
    string message,
    params object[] messageArgs)
  {
    if (!Error.s_hasShownNonRepeatingDevWarning)
    {
      Error.s_hasShownNonRepeatingDevWarning = true;
      Error.AddDevWarning(header, message, messageArgs);
    }
    else
    {
      string str = string.Format(message, messageArgs);
      Debug.LogWarning((object) string.Format("Error.AddDevWarningNonRepeating() - header={0} message={1}", (object) header, (object) str));
    }
  }

  public static void AddFatal(
    FatalErrorReason reason,
    string messageKey,
    params object[] messageArgs)
  {
    Error.AddFatal(new ErrorParams()
    {
      m_message = GameStrings.Format(messageKey, messageArgs),
      m_reason = reason
    });
  }

  public static void AddFatal(ErrorParams parms)
  {
    Debug.LogError((object) string.Format("Error.AddFatal() - message={0}", (object) parms.m_message));
    TelemetryManager.Client().SendFatalError(parms.m_reason.ToString());
    if (UniversalInputManager.Get() != null)
      UniversalInputManager.Get().CancelTextInput((GameObject) null, true);
    if (Error.ShouldUseWarningDialogForFatalError())
    {
      if (string.IsNullOrEmpty(parms.m_header))
        parms.m_header = "Fatal Error as Warning";
      Error.ShowWarningDialog(parms);
    }
    else
    {
      parms.m_type = ErrorType.FATAL;
      FatalErrorMgr.Get().Add(new FatalErrorMessage()
      {
        m_id = (parms.m_header ?? string.Empty) + parms.m_message,
        m_text = parms.m_message,
        m_ackCallback = parms.m_ackCallback,
        m_ackUserData = parms.m_ackUserData,
        m_allowClick = parms.m_allowClick,
        m_redirectToStore = parms.m_redirectToStore,
        m_delayBeforeNextReset = parms.m_delayBeforeNextReset,
        m_reason = parms.m_reason
      });
    }
  }

  public static void AddDevFatal(string message, params object[] messageArgs)
  {
    string message1 = string.Format(message, messageArgs);
    if (!HearthstoneApplication.IsInternal())
    {
      Debug.LogError((object) string.Format("Error.AddDevFatal() - message={0}", (object) message1));
    }
    else
    {
      Debug.LogError((object) message1);
      if (SceneDebugger.Get() == null)
        return;
      SceneDebugger.Get().AddErrorMessage(message1);
    }
  }

  public static void AddDevFatalUnlessWorkarounds(string message, params object[] messageArgs)
  {
    string message1 = string.Format(message, messageArgs);
    if (HearthstoneApplication.UseDevWorkarounds())
      Debug.LogError((object) message1);
    else
      Error.AddDevFatal(message1);
  }

  private static bool ShouldUseWarningDialogForFatalError() => !HearthstoneApplication.IsPublic() && (bool) (Object) DialogManager.Get() && !Options.Get().GetBool(Option.ERROR_SCREEN);

  private static void ShowWarningDialog(ErrorParams parms)
  {
    parms.m_type = ErrorType.WARNING;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_id = parms.m_header + parms.m_message,
      m_headerText = parms.m_header,
      m_text = parms.m_message,
      m_responseCallback = new AlertPopup.ResponseCallback(Error.OnWarningPopupResponse),
      m_responseUserData = (object) parms,
      m_showAlertIcon = true
    });
  }

  private static void OnWarningPopupResponse(AlertPopup.Response response, object userData)
  {
    ErrorParams errorParams = (ErrorParams) userData;
    if (errorParams.m_ackCallback == null)
      return;
    errorParams.m_ackCallback(errorParams.m_ackUserData);
  }
}
