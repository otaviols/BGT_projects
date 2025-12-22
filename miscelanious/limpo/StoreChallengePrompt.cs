using Hearthstone;
using Hearthstone.Http;
using MiniJSON;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StoreChallengePrompt : UIBPopup
{
  public UIBButton m_submitButton;
  public UIBButton m_cancelButton;
  public UberText m_messageText;
  public UberText m_inputText;
  public GameObject m_infoButtonFrame;
  public UIBButton m_infoButton;
  private const int TASSADAR_CHALLENGE_TIMEOUT_SECONDS = 15;
  private string m_input = string.Empty;
  private string m_challengeID;
  private string m_challengeUrl;
  private JsonNode m_challengeJson;
  private JsonNode m_challengeInput;
  private string m_challengeType;

  public event StoreChallengePrompt.CancelListener OnCancel;

  public event StoreChallengePrompt.CompleteListener OnChallengeComplete;

  protected override void Awake()
  {
    base.Awake();
    this.m_inputText.RichText = false;
    this.m_submitButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSubmitPressed));
    this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
    this.m_infoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInfoPressed));
  }

  public IEnumerator Show(string challengeUrl)
  {
    StoreChallengePrompt storeChallengePrompt = this;
    storeChallengePrompt.m_challengeJson = (JsonNode) null;
    storeChallengePrompt.m_challengeUrl = challengeUrl;
    if (!storeChallengePrompt.IsShown())
    {
      storeChallengePrompt.m_shown = true;
      Dictionary<string, string> headers = new Dictionary<string, string>();
      headers["Accept"] = "application/json;charset=UTF-8";
      headers["Accept-Language"] = Localization.GetBnetLocaleName();
      IHttpRequest challenge = HttpRequestFactory.Get().CreateGetRequest(storeChallengePrompt.m_challengeUrl);
      challenge.SetRequestHeaders((IEnumerable<KeyValuePair<string, string>>) headers);
      challenge.TimeoutSeconds = 15;
      yield return (object) challenge.SendRequest();
      string internalErrorInfo = (string) null;
      if (challenge.IsNetworkError || challenge.IsHttpError)
        internalErrorInfo = challenge.ErrorString;
      else if (string.IsNullOrEmpty(challenge.ResponseAsString))
      {
        internalErrorInfo = "Empty Response";
      }
      else
      {
        if (HearthstoneApplication.IsInternal())
          Log.BattleNet.PrintInfo("Challenge json received: {0}", (object) challenge.ResponseAsString);
        try
        {
          storeChallengePrompt.m_challengeJson = (JsonNode) Json.Deserialize(challenge.ResponseAsString);
        }
        catch (Exception ex)
        {
          Debug.LogException(ex);
          internalErrorInfo = string.Format("{0}: {1}", (object) ex.GetType().Name, (object) ex.Message);
        }
      }
      if (!string.IsNullOrEmpty(internalErrorInfo))
      {
        Log.BattleNet.PrintError("Tassadar Challenge Retrieval Failed: " + internalErrorInfo);
        storeChallengePrompt.Hide(false);
        string header = GameStrings.Get("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE");
        string message = GameStrings.Get("GLUE_STORE_FAIL_CHALLENGE_TIMEOUT");
        CancelPurchase.CancelReason? reason = new CancelPurchase.CancelReason?();
        if (challenge.DidTimeout)
          reason = new CancelPurchase.CancelReason?(CancelPurchase.CancelReason.CHALLENGE_TIMEOUT);
        storeChallengePrompt.DisplayError(header, message, false, reason, internalErrorInfo);
      }
      else
      {
        JsonNode jsonNode = (JsonNode) storeChallengePrompt.m_challengeJson["challenge"];
        storeChallengePrompt.m_challengeID = (string) storeChallengePrompt.m_challengeJson["challenge_id"];
        string str1 = (string) jsonNode["prompt"];
        storeChallengePrompt.m_challengeType = (string) jsonNode["type"];
        storeChallengePrompt.m_challengeInput = (JsonNode) ((List<object>) jsonNode["inputs"])[0];
        JsonList source = jsonNode.ContainsKey("errors") ? jsonNode["errors"] as JsonList : (JsonList) null;
        if (source != null && source.Count > 0)
        {
          string str2 = string.Join("\n", source.Select<object, string>((Func<object, string>) (n => (string) n)).ToArray<string>());
          storeChallengePrompt.DisplayError((string) storeChallengePrompt.m_challengeInput["label"], str2, false, new CancelPurchase.CancelReason?(CancelPurchase.CancelReason.CHALLENGE_OTHER_ERROR), str2);
        }
        else
        {
          bool flag = false;
          if (storeChallengePrompt.m_challengeType == "cvv")
            flag = true;
          storeChallengePrompt.m_messageText.Text = str1;
          if (string.IsNullOrEmpty(storeChallengePrompt.m_messageText.Text))
            Log.BattleNet.PrintError("Challenge has no prompt text, json received: {0}", (object) challenge.ResponseAsString);
          storeChallengePrompt.m_infoButtonFrame.SetActive(flag);
          storeChallengePrompt.m_input = string.Empty;
          storeChallengePrompt.UpdateInputText();
          storeChallengePrompt.DoShowAnimation(new UIBPopup.OnAnimationComplete(storeChallengePrompt.OnShown));
        }
      }
    }
  }

  public string HideChallenge()
  {
    string challengeId = this.m_challengeID;
    this.Hide(false);
    return challengeId;
  }

  private void OnShown()
  {
    if (!this.IsShown())
      return;
    this.ShowInput();
  }

  protected override void Hide(bool animate)
  {
    if (!this.IsShown())
      return;
    this.m_shown = false;
    this.HideInput();
    this.DoHideAnimation(!animate, new UIBPopup.OnAnimationComplete(((UIBPopup) this).OnHidden));
  }

  protected override void OnHidden() => this.m_challengeID = (string) null;

  private void Cancel()
  {
    string challengeId = this.m_challengeID;
    this.Hide(true);
    if (this.OnCancel == null)
      return;
    this.OnCancel(challengeId);
  }

  private void OnSubmitPressed(UIEvent e) => this.StartCoroutine(this.SubmitChallenge());

  private IEnumerator SubmitChallenge()
  {
    StoreChallengePrompt storeChallengePrompt = this;
    storeChallengePrompt.HideInput();
    Dictionary<string, string> headers = new Dictionary<string, string>();
    headers["Accept"] = "application/json;charset=UTF-8";
    headers["Accept-Language"] = Localization.GetBnetLocaleName();
    headers["Content-Type"] = "application/json;charset=UTF-8";
    string str1 = (storeChallengePrompt.m_challengeInput == null ? (string) null : (string) storeChallengePrompt.m_challengeInput["input_id"]) ?? "";
    string str2 = storeChallengePrompt.m_input == null ? "" : storeChallengePrompt.m_input;
    JsonNode jsonNode1 = new JsonNode();
    JsonList jsonList = new JsonList();
    JsonNode jsonNode2 = new JsonNode();
    jsonNode2.Add("input_id", (object) str1);
    jsonNode2.Add("value", (object) str2);
    jsonList.Add((object) jsonNode2);
    jsonNode1.Add("inputs", (object) jsonList);
    string s = Json.Serialize((object) jsonNode1);
    IHttpRequest challengeResponse = HttpRequestFactory.Get().CreatePostRequest(storeChallengePrompt.m_challengeUrl, Encoding.UTF8.GetBytes(s));
    challengeResponse.SetRequestHeaders((IEnumerable<KeyValuePair<string, string>>) headers);
    challengeResponse.TimeoutSeconds = 15;
    yield return (object) challengeResponse.SendRequest();
    JsonNode jsonNode3 = (JsonNode) null;
    string internalErrorInfo1 = (string) null;
    if (challengeResponse.IsNetworkError || challengeResponse.IsHttpError)
      internalErrorInfo1 = challengeResponse.ErrorString;
    else if (string.IsNullOrEmpty(challengeResponse.ResponseAsString))
    {
      internalErrorInfo1 = "Empty Response";
    }
    else
    {
      if (HearthstoneApplication.IsInternal())
        Log.BattleNet.PrintInfo("Submit challenge response json received: {0}", (object) challengeResponse.ResponseAsString);
      try
      {
        jsonNode3 = (JsonNode) Json.Deserialize(challengeResponse.ResponseAsString);
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
        internalErrorInfo1 = string.Format("{0}: {1}", (object) ex.GetType().Name, (object) ex.Message);
      }
    }
    if (!string.IsNullOrEmpty(internalErrorInfo1))
    {
      Log.BattleNet.PrintError("Tassadar Challenge Submission Failed: " + internalErrorInfo1);
      storeChallengePrompt.Hide(false);
      string header = GameStrings.Get("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE");
      string message = GameStrings.Get("GLUE_STORE_FAIL_CHALLENGE_TIMEOUT");
      CancelPurchase.CancelReason? reason = new CancelPurchase.CancelReason?();
      if (challengeResponse.DidTimeout)
        reason = new CancelPurchase.CancelReason?(CancelPurchase.CancelReason.CHALLENGE_TIMEOUT);
      storeChallengePrompt.DisplayError(header, message, false, reason, internalErrorInfo1);
    }
    else
    {
      int num = (bool) jsonNode3["done"] ? 1 : 0;
      string challengeId = storeChallengePrompt.m_challengeID;
      if (num == 0)
      {
        JsonNode jsonNode4 = jsonNode3["challenge"] as JsonNode;
        string message = string.Join("\n", (jsonNode4.ContainsKey("errors") ? (IEnumerable<object>) (jsonNode4["errors"] as JsonList) : (IEnumerable<object>) new JsonList()).Select<object, string>((Func<object, string>) (n => (string) n)).ToArray<string>());
        storeChallengePrompt.DisplayError((string) storeChallengePrompt.m_challengeInput["label"], message, true, new CancelPurchase.CancelReason?(), (string) null);
      }
      else
      {
        bool isSuccess = true;
        string internalErrorInfo2 = jsonNode3.ContainsKey("error_code") ? jsonNode3["error_code"] as string : (string) null;
        if (!string.IsNullOrEmpty(internalErrorInfo2))
          isSuccess = false;
        if (isSuccess)
        {
          storeChallengePrompt.Hide(true);
          storeChallengePrompt.FireComplete(challengeId, isSuccess, new CancelPurchase.CancelReason?(), internalErrorInfo2);
        }
        else
        {
          string header = GameStrings.Get("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE");
          string message = GameStrings.Get("GLUE_STORE_FAIL_THROTTLED");
          CancelPurchase.CancelReason cancelReason = CancelPurchase.CancelReason.CHALLENGE_OTHER_ERROR;
          if (internalErrorInfo2 == "DENIED")
          {
            cancelReason = CancelPurchase.CancelReason.CHALLENGE_DENIED;
            internalErrorInfo2 = (string) null;
          }
          storeChallengePrompt.DisplayError(header, message, false, new CancelPurchase.CancelReason?(cancelReason), internalErrorInfo2);
        }
      }
    }
  }

  private void DisplayError(
    string header,
    string message,
    bool allowInputAgain,
    CancelPurchase.CancelReason? reason,
    string internalErrorInfo)
  {
    this.ClearInput();
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_showAlertIcon = false;
    info.m_headerText = header;
    info.m_text = message;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
    info.m_alertTextAlignment = UberText.AlignmentOptions.Center;
    if (allowInputAgain)
      info.m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => this.ShowInput());
    else
      this.FireComplete(this.HideChallenge(), false, reason, internalErrorInfo);
    DialogManager.Get().ShowPopup(info);
  }

  private void FireComplete(
    string challengeID,
    bool isSuccess,
    CancelPurchase.CancelReason? reason,
    string internalErrorInfo)
  {
    if (this.OnChallengeComplete == null)
      return;
    this.OnChallengeComplete(challengeID, isSuccess, reason, internalErrorInfo);
  }

  private void OnCancelPressed(UIEvent e) => this.Cancel();

  private void OnInfoPressed(UIEvent e) => Application.OpenURL(ExternalUrlService.Get().GetCVVLink());

  private void ShowInput()
  {
    this.m_inputText.gameObject.SetActive(false);
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.gameObject.layer);
    Bounds bounds = this.m_inputText.GetBounds();
    Vector3 min = bounds.min;
    Vector3 max = bounds.max;
    Rect guiViewportRect = CameraUtils.CreateGUIViewportRect(firstByLayer, min, max);
    UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams()
    {
      m_owner = this.gameObject,
      m_password = true,
      m_rect = guiViewportRect,
      m_updatedCallback = new UniversalInputManager.TextInputUpdatedCallback(this.OnInputUpdated),
      m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(this.OnInputComplete),
      m_canceledCallback = new UniversalInputManager.TextInputCanceledCallback(this.OnInputCanceled),
      m_font = this.m_inputText.TrueTypeFont,
      m_alignment = new TextAnchor?(TextAnchor.MiddleCenter),
      m_maxCharacters = this.m_challengeInput != null ? (int) (long) this.m_challengeInput["max_length"] : 0
    };
    UniversalInputManager.Get().UseTextInput(parms);
    this.m_submitButton.SetEnabled(true);
  }

  private void HideInput()
  {
    UniversalInputManager.Get().CancelTextInput(this.gameObject);
    this.m_inputText.gameObject.SetActive(true);
    this.m_submitButton.SetEnabled(false);
  }

  private void ClearInput() => UniversalInputManager.Get().SetInputText("");

  private void OnInputUpdated(string input)
  {
    this.m_input = input;
    this.UpdateInputText();
  }

  private void OnInputComplete(string input)
  {
    this.m_input = input;
    this.UpdateInputText();
    this.StartCoroutine(this.SubmitChallenge());
  }

  private void OnInputCanceled(bool userRequested, GameObject requester)
  {
    this.m_input = string.Empty;
    this.UpdateInputText();
    this.Cancel();
  }

  private void UpdateInputText()
  {
    StringBuilder stringBuilder = new StringBuilder(this.m_input.Length);
    for (int index = 0; index < this.m_input.Length; ++index)
      stringBuilder.Append('*');
    this.m_inputText.Text = stringBuilder.ToString();
  }

  public delegate void CancelListener(string challengeID);

  public delegate void CompleteListener(
    string challengeID,
    bool isSuccess,
    CancelPurchase.CancelReason? reason,
    string internalErrorInfo);
}
