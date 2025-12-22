using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Hearthstone;
using Hearthstone.Core;
using System;
using System.Collections.Generic;
using System.Threading;

public class FatalErrorMgr
{
  private const string UPDATE_GETSTATUS_JOB_NAME = "FatalErrorMgr.";
  private static FatalErrorMgr s_instance;
  private List<FatalErrorMessage> m_messages = new List<FatalErrorMessage>();
  private List<FatalErrorMgr.ErrorListener> m_errorListeners = new List<FatalErrorMgr.ErrorListener>();
  private string m_generatedErrorCode;
  private object m_lock = new object();

  public bool IsUnrecoverable { get; private set; }

  private int PosToFireMessage { get; set; } = -1;

  public static FatalErrorMgr Get()
  {
    if (FatalErrorMgr.s_instance == null)
      FatalErrorMgr.s_instance = new FatalErrorMgr();
    return FatalErrorMgr.s_instance;
  }

  public static bool IsInitialized() => FatalErrorMgr.s_instance != null;

  public void RunProcessJob() => Processor.QueueJobIfNotExist("FatalErrorMgr.", this.Job_ProcessMessages());

  public void Add(FatalErrorMessage message)
  {
    this.RunProcessJob();
    lock (this.m_lock)
    {
      this.m_messages.Add(message);
      if (HearthstoneApplication.IsMainThread)
      {
        this.FireErrorListeners(message);
      }
      else
      {
        if (this.PosToFireMessage != -1)
          return;
        this.PosToFireMessage = this.m_messages.Count - 1;
      }
    }
  }

  public void SetErrorCode(
    string prefixSource,
    string errorSubset1,
    string errorSubset2 = null,
    string errorSubset3 = null)
  {
    this.m_generatedErrorCode = prefixSource + ":" + errorSubset1;
    if (errorSubset2 != null)
      this.m_generatedErrorCode = this.m_generatedErrorCode + ":" + errorSubset2;
    if (errorSubset3 == null)
      return;
    this.m_generatedErrorCode = this.m_generatedErrorCode + ":" + errorSubset3;
  }

  public void ClearAllErrors()
  {
    while (this.PosToFireMessage != -1)
      Thread.Sleep(100);
    lock (this.m_lock)
      this.m_messages.Clear();
    this.m_generatedErrorCode = (string) null;
  }

  public bool AddErrorListener(FatalErrorMgr.ErrorCallback callback) => this.AddErrorListener(callback, (object) null);

  public bool AddErrorListener(FatalErrorMgr.ErrorCallback callback, object userData)
  {
    FatalErrorMgr.ErrorListener errorListener = new FatalErrorMgr.ErrorListener();
    errorListener.SetCallback(callback);
    errorListener.SetUserData(userData);
    if (this.m_errorListeners.Contains(errorListener))
      return false;
    this.m_errorListeners.Add(errorListener);
    return true;
  }

  public bool RemoveErrorListener(FatalErrorMgr.ErrorCallback callback) => this.RemoveErrorListener(callback, (object) null);

  public bool RemoveErrorListener(FatalErrorMgr.ErrorCallback callback, object userData)
  {
    FatalErrorMgr.ErrorListener errorListener = new FatalErrorMgr.ErrorListener();
    errorListener.SetCallback(callback);
    errorListener.SetUserData(userData);
    return this.m_errorListeners.Remove(errorListener);
  }

  public FatalErrorMessage[] GetMessages()
  {
    lock (this.m_lock)
      return this.m_messages.ToArray();
  }

  public string GetFormattedErrorCode() => this.m_generatedErrorCode;

  public bool HasError()
  {
    lock (this.m_lock)
      return this.m_messages.Count > 0;
  }

  public void NotifyExitPressed()
  {
    this.SendAcknowledgements();
    HearthstoneApplication.Get().Exit();
  }

  public static bool IsReconnectAllowedBasedOnFatalErrorReason(FatalErrorReason reason)
  {
    switch (reason)
    {
      case FatalErrorReason.LOGIN_FROM_ANOTHER_DEVICE:
      case FatalErrorReason.ADMIN_KICK_OR_BAN:
      case FatalErrorReason.ACCOUNT_SETUP_ERROR:
      case FatalErrorReason.MOBILE_GAME_SERVER_RPC_ERROR:
        return false;
      case FatalErrorReason.BREAKING_NEWS:
        if (SceneMgr.Get().GetMode() == SceneMgr.Mode.STARTUP)
          return false;
        break;
    }
    return true;
  }

  public void SetUnrecoverable(bool isUnrecoverable) => this.IsUnrecoverable = isUnrecoverable;

  private void SendAcknowledgements()
  {
    foreach (FatalErrorMessage message in this.GetMessages())
    {
      if (message.m_ackCallback != null)
        message.m_ackCallback(message.m_ackUserData);
    }
  }

  private IEnumerator<IAsyncJobResult> Job_ProcessMessages()
  {
    while (!HearthstoneApplication.Get().IsExiting())
    {
      if (this.PosToFireMessage == -1)
        yield return (IAsyncJobResult) new WaitForDurationForWorker(500.0);
      lock (this.m_lock)
      {
        if (this.PosToFireMessage != -1)
        {
          FatalErrorMessage[] messages = this.GetMessages();
          for (int posToFireMessage = this.PosToFireMessage; posToFireMessage < messages.Length; ++posToFireMessage)
            this.FireErrorListeners(messages[posToFireMessage]);
          this.PosToFireMessage = -1;
        }
      }
    }
  }

  protected void FireErrorListeners(FatalErrorMessage message)
  {
    foreach (FatalErrorMgr.ErrorListener errorListener in this.m_errorListeners.ToArray())
      errorListener.Fire(message);
  }

  public delegate void ErrorCallback(FatalErrorMessage message, object userData);

  protected class ErrorListener : EventListener<FatalErrorMgr.ErrorCallback>
  {
    public void Fire(FatalErrorMessage message)
    {
      if (!GeneralUtils.IsCallbackValid((Delegate) this.m_callback))
        return;
      this.m_callback(message, this.m_userData);
    }
  }
}
