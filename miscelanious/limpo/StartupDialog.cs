using System;
using UnityEngine;
using UnityEngine.UI;

public class StartupDialog : MonoBehaviour
{
  private const string kStartupDialogResourcePath = "StartupDialog";
  [SerializeField]
  private GameObject m_singleButtonRoot;
  [SerializeField]
  private GameObject m_doubleButtonRoot;
  [SerializeField]
  private Text m_headerText;
  [SerializeField]
  private Text m_bodyText;
  [SerializeField]
  private UGUIButton m_singleButton;
  [SerializeField]
  private UGUIButton m_doubleButton1;
  [SerializeField]
  private UGUIButton m_doubleButton2;
  private static StartupDialog s_instance;

  public static void ShowStartupDialog(
    string header,
    string body,
    string buttonText,
    Action buttonDelegate)
  {
    if (!StartupDialog.EnsureInstance())
      return;
    StartupDialog.s_instance.SetupSingleButtonDialog(header, body, buttonText, buttonDelegate);
  }

  public static void ShowStartupDialog(
    string header,
    string body,
    string buttonText,
    Action buttonDelegate,
    bool closeAtClick)
  {
    if (!StartupDialog.EnsureInstance())
      return;
    StartupDialog.s_instance.SetupSingleButtonDialog(header, body, buttonText, buttonDelegate, closeAtClick);
  }

  public static void ShowStartupDialog(
    string header,
    string body,
    string buttonText1,
    Action buttonDelegate1,
    string buttonText2,
    Action buttonDelegate2)
  {
    if (!StartupDialog.EnsureInstance())
      return;
    StartupDialog.s_instance.SetupDoubleButtonDialog(header, body, buttonText1, buttonDelegate1, buttonText2, buttonDelegate2);
  }

  public static void ShowStartupDialog(
    string header,
    string body,
    string buttonText1,
    Action buttonDelegate1,
    bool closeAtClick1,
    string buttonText2,
    Action buttonDelegate2,
    bool closeAtClick2)
  {
    if (!StartupDialog.EnsureInstance())
      return;
    StartupDialog.s_instance.SetupDoubleButtonDialog(header, body, buttonText1, buttonDelegate1, closeAtClick1, buttonText2, buttonDelegate2, closeAtClick2);
  }

  public static void Destroy()
  {
    if (!((UnityEngine.Object) StartupDialog.s_instance != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) StartupDialog.s_instance.gameObject);
    StartupDialog.s_instance = (StartupDialog) null;
  }

  private static bool EnsureInstance()
  {
    if ((UnityEngine.Object) StartupDialog.s_instance == (UnityEngine.Object) null)
    {
      GameObject original = Resources.Load<GameObject>(nameof (StartupDialog));
      if ((UnityEngine.Object) original == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("Couldn't load prefab at ({0}).", (object) nameof (StartupDialog));
        return false;
      }
      GameObject target = UnityEngine.Object.Instantiate<GameObject>(original);
      StartupDialog.s_instance = target.GetComponent<StartupDialog>();
      if ((UnityEngine.Object) StartupDialog.s_instance == (UnityEngine.Object) null)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) target);
        Debug.LogErrorFormat("Couldn't find StartupDialog component on prefab at ({0}).", (object) nameof (StartupDialog));
        return false;
      }
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) target);
    }
    return true;
  }

  private void SetupSingleButtonDialog(
    string header,
    string body,
    string buttonText,
    Action buttonDelegate)
  {
    this.m_singleButtonRoot.SetActive(true);
    this.m_doubleButtonRoot.SetActive(false);
    this.m_headerText.text = header;
    this.m_bodyText.text = body;
    this.m_singleButton.SetupButton(buttonText, buttonDelegate, new Action(StartupDialog.Destroy));
  }

  private void SetupSingleButtonDialog(
    string header,
    string body,
    string buttonText,
    Action buttonDelegate,
    bool closeAtClick)
  {
    this.m_singleButtonRoot.SetActive(true);
    this.m_doubleButtonRoot.SetActive(false);
    this.m_headerText.text = header;
    this.m_bodyText.text = body;
    if (closeAtClick)
      this.m_singleButton.SetupButton(buttonText, buttonDelegate, new Action(StartupDialog.Destroy));
    else
      this.m_singleButton.SetupButton(buttonText, buttonDelegate, (Action) null);
  }

  private void SetupDoubleButtonDialog(
    string header,
    string body,
    string buttonText1,
    Action buttonDelegate1,
    string buttonText2,
    Action buttonDelegate2)
  {
    this.m_singleButtonRoot.SetActive(false);
    this.m_doubleButtonRoot.SetActive(true);
    this.m_headerText.text = header;
    this.m_bodyText.text = body;
    this.m_doubleButton1.SetupButton(buttonText1, buttonDelegate1, new Action(StartupDialog.Destroy));
    this.m_doubleButton2.SetupButton(buttonText2, buttonDelegate2, new Action(StartupDialog.Destroy));
  }

  private void SetupDoubleButtonDialog(
    string header,
    string body,
    string buttonText1,
    Action buttonDelegate1,
    bool closeAtClick1,
    string buttonText2,
    Action buttonDelegate2,
    bool closeAtClick2)
  {
    this.m_singleButtonRoot.SetActive(false);
    this.m_doubleButtonRoot.SetActive(true);
    this.m_headerText.text = header;
    this.m_bodyText.text = body;
    if (closeAtClick1)
      this.m_doubleButton1.SetupButton(buttonText1, buttonDelegate1, new Action(StartupDialog.Destroy));
    else
      this.m_doubleButton1.SetupButton(buttonText1, buttonDelegate1, (Action) null);
    if (closeAtClick2)
      this.m_doubleButton2.SetupButton(buttonText2, buttonDelegate2, new Action(StartupDialog.Destroy));
    else
      this.m_doubleButton2.SetupButton(buttonText2, buttonDelegate2, (Action) null);
  }
}
