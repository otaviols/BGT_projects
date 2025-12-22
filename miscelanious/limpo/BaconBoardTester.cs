using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaconBoardTester : MonoBehaviour
{
  public KeyboardFSMSettings m_keyboardStateSettings;
  public GameObject m_pcAuthoringRoot;
  public GameObject m_pcCombatSkinsRoot;
  public GameObject m_pcTavernSkinsRoot;
  public GameObject m_phoneAuthoringRoot;
  public GameObject m_phoneCombatSkinsRoot;
  public GameObject m_phoneTavernSkinsRoot;
  public GameObject m_controlsRoot;
  public GameObject m_turnIndicatorRoot;
  public GameObject m_turnIndicatorPhoneRoot;
  public List<PlayMakerFSM> m_boardStateChangingObjects;
  public Dropdown m_platformSelection;
  public Dropdown m_combatSkinSelectionPC;
  public Dropdown m_combatSkinSelectionPhone;
  public Dropdown m_tavernSkinSelectionPC;
  public Dropdown m_tavernSkinSelectionPhone;
  public Dropdown m_fsmStateSelection;
  private BaconBoardTester.BaconBoardTesterPlatform m_selectedPlatform;
  private BaconBoardSkinBehaviour m_selectedPCCombatSkin;
  private BaconBoardSkinBehaviour m_selectedPCTavernSkin;
  private BaconBoardSkinBehaviour m_prevPCCombatSkin;
  private BaconBoardSkinBehaviour m_prevPCTavernSkin;
  private BaconBoardSkinBehaviour m_selectedPhoneCombatSkin;
  private BaconBoardSkinBehaviour m_selectedPhoneTavernSkin;
  private BaconBoardSkinBehaviour m_prevPhoneCombatSkin;
  private BaconBoardSkinBehaviour m_prevPhoneTavernSkin;
  private string m_selectedState = "SHOP";
  private TAG_BOARD_VISUAL_STATE m_prevState;
  private BaconBoardSkinBehaviour[] m_pcCombatSkins;
  private BaconBoardSkinBehaviour[] m_pcTavernSkins;
  private BaconBoardSkinBehaviour[] m_phoneCombatSkins;
  private BaconBoardSkinBehaviour[] m_phoneTavernSkins;

  private void Start() => this.StartCoroutine(this.WaitForSoundManagerThenLoad());

  public IEnumerator WaitForSoundManagerThenLoad()
  {
    while (SoundManager.Get() == null)
      yield return (object) new WaitForSeconds(1f);
    this.InitObjects();
  }

  private void InitObjects()
  {
    this.m_pcCombatSkins = this.m_pcCombatSkinsRoot.GetComponentsInChildren<BaconBoardSkinBehaviour>(true);
    this.m_pcTavernSkins = this.m_pcTavernSkinsRoot.GetComponentsInChildren<BaconBoardSkinBehaviour>(true);
    this.m_phoneCombatSkins = this.m_phoneCombatSkinsRoot.GetComponentsInChildren<BaconBoardSkinBehaviour>(true);
    this.m_phoneTavernSkins = this.m_phoneTavernSkinsRoot.GetComponentsInChildren<BaconBoardSkinBehaviour>(true);
    this.m_selectedPlatform = !this.m_phoneAuthoringRoot.activeSelf || this.m_pcAuthoringRoot.activeSelf ? BaconBoardTester.BaconBoardTesterPlatform.PC : BaconBoardTester.BaconBoardTesterPlatform.PHONE;
    this.m_platformSelection.SetValueWithoutNotify((int) this.m_selectedPlatform);
    this.m_selectedPCCombatSkin = this.GetActiveSkin(this.m_pcCombatSkins);
    this.m_selectedPCTavernSkin = this.GetActiveSkin(this.m_pcTavernSkins);
    this.m_selectedPhoneCombatSkin = this.GetActiveSkin(this.m_phoneCombatSkins);
    this.m_selectedPhoneTavernSkin = this.GetActiveSkin(this.m_phoneTavernSkins);
    this.InitDropdown(this.m_combatSkinSelectionPC, Array.ConvertAll<BaconBoardSkinBehaviour, string>(this.m_pcCombatSkins, (Converter<BaconBoardSkinBehaviour, string>) (skin => skin.gameObject.name)), this.m_selectedPCCombatSkin.gameObject.name);
    this.InitDropdown(this.m_tavernSkinSelectionPC, Array.ConvertAll<BaconBoardSkinBehaviour, string>(this.m_pcTavernSkins, (Converter<BaconBoardSkinBehaviour, string>) (skin => skin.gameObject.name)), this.m_selectedPCTavernSkin.gameObject.name);
    this.InitDropdown(this.m_combatSkinSelectionPhone, Array.ConvertAll<BaconBoardSkinBehaviour, string>(this.m_phoneCombatSkins, (Converter<BaconBoardSkinBehaviour, string>) (skin => skin.gameObject.name)), this.m_selectedPhoneCombatSkin.gameObject.name);
    this.InitDropdown(this.m_tavernSkinSelectionPhone, Array.ConvertAll<BaconBoardSkinBehaviour, string>(this.m_phoneTavernSkins, (Converter<BaconBoardSkinBehaviour, string>) (skin => skin.gameObject.name)), this.m_selectedPhoneTavernSkin.gameObject.name);
    this.InitDropdown(this.m_fsmStateSelection, this.m_keyboardStateSettings.Settings.ConvertAll<string>((Converter<KeyboardFSMSettings.KeyAndAnimationTriggerPair, string>) (setting => setting.PlaymakerState + " ( " + setting.KeyboardKey.ToString() + ")")).ToArray(), "SHOP");
    this.ActivateSelection();
  }

  public void OnCombatBoardChangedPC(int index)
  {
    this.m_selectedPCCombatSkin = this.m_pcCombatSkins[index];
    this.ActivateSelection();
  }

  public void OnCombatBoardChangedPhone(int index)
  {
    this.m_selectedPhoneCombatSkin = this.m_phoneCombatSkins[index];
    this.ActivateSelection();
  }

  public void OnTavernBoardChangedPC(int index)
  {
    this.m_selectedPCTavernSkin = this.m_pcTavernSkins[index];
    this.ActivateSelection();
  }

  public void OnTavernBoardChangedPhone(int index)
  {
    this.m_selectedPhoneTavernSkin = this.m_phoneTavernSkins[index];
    this.ActivateSelection();
  }

  public void OnPlatformChange(int value)
  {
    this.m_selectedPlatform = (BaconBoardTester.BaconBoardTesterPlatform) value;
    this.ActivateSelection();
  }

  public void OnFSMStateChange(int index)
  {
    this.m_selectedState = this.m_keyboardStateSettings[index].PlaymakerState;
    this.ActivateSelection();
  }

  public void OnUseBlurToggled(bool value)
  {
    this.m_turnIndicatorRoot.SetActive(value);
    this.m_turnIndicatorPhoneRoot.SetActive(value);
  }

  private void InitDropdown(Dropdown dropdown, string[] items, string selection)
  {
    dropdown.AddOptions(new List<string>((IEnumerable<string>) items));
    dropdown.SetValueWithoutNotify(dropdown.options.FindIndex((Predicate<Dropdown.OptionData>) (option => option.text.StartsWith(selection))));
  }

  private BaconBoardSkinBehaviour GetActiveSkin(
    BaconBoardSkinBehaviour[] skins)
  {
    foreach (BaconBoardSkinBehaviour skin in skins)
    {
      if (skin.gameObject.activeSelf)
        return skin;
    }
    skins[0].gameObject.SetActive(true);
    return skins[0];
  }

  private void SetStateOnFsms(string stateName)
  {
    foreach (PlayMakerFSM stateChangingObject in this.m_boardStateChangingObjects)
    {
      if (this.FsmContainsState(stateChangingObject, stateName))
        stateChangingObject.SetState(stateName);
    }
  }

  private bool FsmContainsState(PlayMakerFSM fsm, string stateName)
  {
    foreach (FsmState fsmState in fsm.FsmStates)
    {
      if (stateName.Equals(fsmState.Name))
        return true;
    }
    return false;
  }

  public void AddStateChangingPlaymaker(GameObject container)
  {
    PlayMakerFSM componentInChildren = container.GetComponentInChildren<PlayMakerFSM>();
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
      return;
    this.m_boardStateChangingObjects.Add(componentInChildren);
  }

  public void ActivateSelection()
  {
    foreach (BaconBoardSkinBehaviour pcCombatSkin in this.m_pcCombatSkins)
    {
      if ((UnityEngine.Object) pcCombatSkin != (UnityEngine.Object) this.m_selectedPCCombatSkin)
        pcCombatSkin.gameObject.SetActive(false);
    }
    foreach (BaconBoardSkinBehaviour pcTavernSkin in this.m_pcTavernSkins)
    {
      if ((UnityEngine.Object) pcTavernSkin != (UnityEngine.Object) this.m_selectedPCTavernSkin)
        pcTavernSkin.gameObject.SetActive(false);
    }
    foreach (BaconBoardSkinBehaviour phoneCombatSkin in this.m_phoneCombatSkins)
    {
      if ((UnityEngine.Object) phoneCombatSkin != (UnityEngine.Object) this.m_selectedPhoneCombatSkin)
        phoneCombatSkin.gameObject.SetActive(false);
    }
    foreach (BaconBoardSkinBehaviour phoneTavernSkin in this.m_phoneTavernSkins)
    {
      if ((UnityEngine.Object) phoneTavernSkin != (UnityEngine.Object) this.m_selectedPhoneTavernSkin)
        phoneTavernSkin.gameObject.SetActive(false);
    }
    this.m_pcAuthoringRoot.SetActive(this.m_selectedPlatform == BaconBoardTester.BaconBoardTesterPlatform.PC);
    this.m_combatSkinSelectionPC.gameObject.SetActive(this.m_selectedPlatform == BaconBoardTester.BaconBoardTesterPlatform.PC);
    this.m_tavernSkinSelectionPC.gameObject.SetActive(this.m_selectedPlatform == BaconBoardTester.BaconBoardTesterPlatform.PC);
    this.m_phoneAuthoringRoot.SetActive(this.m_selectedPlatform == BaconBoardTester.BaconBoardTesterPlatform.PHONE);
    this.m_combatSkinSelectionPhone.gameObject.SetActive(this.m_selectedPlatform == BaconBoardTester.BaconBoardTesterPlatform.PHONE);
    this.m_tavernSkinSelectionPhone.gameObject.SetActive(this.m_selectedPlatform == BaconBoardTester.BaconBoardTesterPlatform.PHONE);
    this.m_selectedPCCombatSkin.gameObject.SetActive(this.m_selectedPlatform == BaconBoardTester.BaconBoardTesterPlatform.PC);
    this.m_selectedPCTavernSkin.gameObject.SetActive(this.m_selectedPlatform == BaconBoardTester.BaconBoardTesterPlatform.PC);
    this.m_selectedPhoneCombatSkin.gameObject.SetActive(this.m_selectedPlatform == BaconBoardTester.BaconBoardTesterPlatform.PHONE);
    this.m_selectedPhoneTavernSkin.gameObject.SetActive(this.m_selectedPlatform == BaconBoardTester.BaconBoardTesterPlatform.PHONE);
    TAG_BOARD_VISUAL_STATE newBoardState = this.m_selectedState == "SHOP" ? TAG_BOARD_VISUAL_STATE.SHOP : TAG_BOARD_VISUAL_STATE.COMBAT;
    if (this.m_selectedPlatform == BaconBoardTester.BaconBoardTesterPlatform.PC)
    {
      if (newBoardState == TAG_BOARD_VISUAL_STATE.COMBAT)
        this.m_selectedPCCombatSkin.CopyCornersFromSkin(this.m_selectedPCTavernSkin);
      if (this.m_prevState != newBoardState || (UnityEngine.Object) this.m_selectedPCCombatSkin != (UnityEngine.Object) this.m_prevPCCombatSkin || this.m_selectedState == "COMBAT")
        this.m_selectedPCCombatSkin.SetBoardState(newBoardState);
      if (this.m_prevState != newBoardState || (UnityEngine.Object) this.m_selectedPCTavernSkin != (UnityEngine.Object) this.m_prevPCTavernSkin)
        this.m_selectedPCTavernSkin.SetBoardState(newBoardState);
      this.m_prevPCCombatSkin = this.m_selectedPCCombatSkin;
      this.m_prevPCTavernSkin = this.m_selectedPCTavernSkin;
      this.m_prevState = newBoardState;
      if (this.m_selectedState != "SHOP" && this.m_selectedState != "COMBAT")
        this.m_selectedPCCombatSkin.DebugTriggerFSMState(this.m_selectedState);
    }
    else
    {
      if (newBoardState == TAG_BOARD_VISUAL_STATE.COMBAT)
        this.m_selectedPhoneCombatSkin.CopyCornersFromSkin(this.m_selectedPhoneTavernSkin);
      if (this.m_prevState != newBoardState || (UnityEngine.Object) this.m_selectedPhoneCombatSkin != (UnityEngine.Object) this.m_prevPhoneCombatSkin || this.m_selectedState == "COMBAT")
        this.m_selectedPhoneCombatSkin.SetBoardState(newBoardState);
      if (this.m_prevState != newBoardState || (UnityEngine.Object) this.m_selectedPhoneTavernSkin != (UnityEngine.Object) this.m_prevPhoneTavernSkin)
        this.m_selectedPhoneTavernSkin.SetBoardState(newBoardState);
      this.m_prevPhoneCombatSkin = this.m_selectedPhoneCombatSkin;
      this.m_prevPhoneTavernSkin = this.m_selectedPhoneTavernSkin;
      this.m_prevState = newBoardState;
      if (this.m_selectedState != "SHOP" && this.m_selectedState != "COMBAT")
        this.m_selectedPhoneCombatSkin.DebugTriggerFSMState(this.m_selectedState);
    }
    this.SetStateOnFsms(newBoardState.ToString());
  }

  public enum BaconBoardTesterPlatform
  {
    PC,
    PHONE,
  }
}
