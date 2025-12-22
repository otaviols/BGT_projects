using Blizzard.T5.Fonts;
using Blizzard.T5.Jobs;
using Hearthstone.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardTextTool : MonoBehaviour
{
  private const string PREFS_LOCALE = "CARD_TEXT_LOCALE";
  private const string PREFS_NAME = "CARD_TEXT_NAME";
  private const string PREFS_DESCRIPTION = "CARD_TEXT_DESCRIPTION";
  public GameObject m_CardsRoot;
  public Actor m_AbilityActor;
  public Actor m_AllyActor;
  public Actor m_WeaponActor;
  public Actor m_HeroActor;
  public Actor m_HeroPowerActor;
  public Actor m_BossCardActor;
  public Actor m_MercenariesAbilityActor;
  public Actor m_MercenariesEquipmentActor;
  public Actor m_MercenaryActor;
  public Actor m_LocationActor;
  public Texture2D m_AbilityPortraitTexture;
  public Texture2D m_AllyPortraitTexture;
  public Texture2D m_WeaponPortraitTexture;
  public Texture2D m_HeroPortraitTexture;
  public Texture2D m_HeroPowerPortraitTexture;
  public Texture2D m_BossPortraitTexture;
  public Texture2D m_MercenariesAbilityPortraitTexture;
  public Texture2D m_MercenariesEquipmentPortraitTexture;
  public Texture2D m_MercenaryPortraitTexture;
  public Texture2D m_LocationPortraitTexture;
  public UberText m_AbilityCardDescription;
  public UberText m_AllyCardDescription;
  public UberText m_WeaponCardDescription;
  public UberText m_HeroCardDescription;
  public UberText m_HeroPowerCardDescription;
  public UberText m_BossCardDescription;
  public UberText m_MercenariesAbilityCardDescription;
  public UberText m_MercenariesEquipmentCardDescription;
  public UberText m_MercenaryCardDescription;
  public UberText m_LocationCardDescription;
  public InputField m_DescriptionInputField;
  public UberText m_AbilityCardName;
  public UberText m_AllyCardName;
  public UberText m_WeaponCardName;
  public UberText m_HeroCardName;
  public UberText m_HeroPowerName;
  public UberText m_BossName;
  public UberText m_MercenariesAbilityCardName;
  public UberText m_MercenariesEquipmentCardName;
  public UberText m_MercenaryCardName;
  public UberText m_LocationCardName;
  public InputField m_NameInputField;
  public Button m_LocaleDropDownMainButton;
  public Button m_LocaleDropDownSelectionButton;
  public List<CardTextTool.LocalizedFont> m_LocalizedFontCollection;
  private string m_nameText;
  private string m_descriptionText;
  private Locale m_locale;

  private void Start() => Processor.QueueJob("CardTextTool.Initialize", this.Job_Initialize());

  private void OnApplicationQuit()
  {
    Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetString("CARD_TEXT_NAME", this.m_nameText);
    Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetString("CARD_TEXT_DESCRIPTION", this.m_descriptionText);
    Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.Save();
  }

  public void UpdateDescriptionText()
  {
    string text = Regex.Replace(this.m_DescriptionInputField.text, "(\\$|#)", "");
    this.m_descriptionText = text;
    string str = this.FixedNewline(text);
    this.m_AbilityCardDescription.Text = str;
    this.m_AllyCardDescription.Text = str;
    this.m_WeaponCardDescription.Text = str;
    this.m_HeroCardDescription.Text = str;
    this.m_HeroPowerCardDescription.Text = str;
    this.m_BossCardDescription.Text = str;
    this.m_MercenariesAbilityCardDescription.Text = str;
    this.m_MercenariesEquipmentCardDescription.Text = str;
    this.m_MercenaryCardDescription.Text = str;
    this.m_LocationCardDescription.Text = str;
  }

  public void UpdateNameText()
  {
    string text = this.m_NameInputField.text;
    this.m_nameText = text;
    this.m_AbilityCardName.Text = text;
    this.m_AllyCardName.Text = text;
    this.m_WeaponCardName.Text = text;
    this.m_HeroCardName.Text = text;
    this.m_HeroPowerName.Text = text;
    this.m_BossName.Text = text;
    this.m_MercenariesAbilityCardName.Text = text;
    this.m_MercenariesEquipmentCardName.Text = text;
    this.m_MercenaryCardName.Text = text;
    this.m_LocationCardName.Text = text;
  }

  public void PasteClipboard()
  {
    this.m_descriptionText = (string) typeof (GUIUtility).GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic).GetValue((object) null, (object[]) null);
    this.m_DescriptionInputField.text = this.m_descriptionText;
    this.UpdateDescriptionText();
  }

  public void CopyToClipboard() => typeof (GUIUtility).GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic).SetValue((object) null, (object) this.m_descriptionText, (object[]) null);

  private IEnumerator<IAsyncJobResult> Job_Initialize()
  {
    if (Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.HasKey("CARD_TEXT_LOCALE"))
      this.m_locale = (Locale) Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.GetInt("CARD_TEXT_LOCALE");
    Localization.SetLocale(this.m_locale);
    HearthstoneLocalization.Initialize();
    this.SetupLocaleDropDown();
    this.SetLocale();
    this.m_AbilityActor.SetPortraitTexture((Texture) this.m_AbilityPortraitTexture);
    this.m_AllyActor.SetPortraitTexture((Texture) this.m_AllyPortraitTexture);
    this.m_WeaponActor.SetPortraitTexture((Texture) this.m_WeaponPortraitTexture);
    this.m_HeroActor.SetPortraitTexture((Texture) this.m_HeroPortraitTexture);
    this.m_HeroPowerActor.SetPortraitTexture((Texture) this.m_HeroPowerPortraitTexture);
    this.m_BossCardActor.SetPortraitTexture((Texture) this.m_BossPortraitTexture);
    this.m_MercenariesAbilityActor.SetPortraitTexture((Texture) this.m_MercenariesAbilityPortraitTexture);
    this.m_MercenariesEquipmentActor.SetPortraitTexture((Texture) this.m_MercenariesEquipmentPortraitTexture);
    this.m_MercenaryActor.SetPortraitTexture((Texture) this.m_MercenaryPortraitTexture);
    this.m_LocationActor.SetPortraitTexture((Texture) this.m_LocationPortraitTexture);
    if (Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.HasKey("CARD_TEXT_NAME"))
      this.m_NameInputField.text = Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.GetString("CARD_TEXT_NAME");
    if (Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.HasKey("CARD_TEXT_DESCRIPTION"))
      this.m_DescriptionInputField.text = Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.GetString("CARD_TEXT_DESCRIPTION");
    foreach (UberText componentsInChild in this.m_CardsRoot.GetComponentsInChildren<UberText>())
      componentsInChild.Cache = false;
    this.UpdateDescriptionText();
    this.UpdateNameText();
    yield break;
  }

  private string FixedNewline(string text)
  {
    if (text.Length < 2)
      return text;
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < text.Length; ++index)
    {
      if (index + 1 < text.Length && text[index] == '\\' && text[index + 1] == 'n')
      {
        stringBuilder.Append('\n');
        ++index;
      }
      else
        stringBuilder.Append(text[index]);
    }
    return stringBuilder.ToString();
  }

  private void SetupLocaleDropDown()
  {
    GameObject gameObject1 = this.m_LocaleDropDownSelectionButton.transform.parent.gameObject;
    gameObject1.SetActive(true);
    foreach (Locale locale in Enum.GetValues(typeof (Locale)))
    {
      if (locale != Locale.UNKNOWN)
      {
        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.m_LocaleDropDownSelectionButton.gameObject);
        gameObject2.transform.parent = this.m_LocaleDropDownSelectionButton.transform.parent;
        Button component = gameObject2.GetComponent<Button>();
        component.GetComponentInChildren<UnityEngine.UI.Text>().text = locale.ToString();
        Locale locSet = locale;
        component.onClick.AddListener((UnityAction) (() => this.OnClick_LocaleSetButton(locSet)));
      }
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_LocaleDropDownSelectionButton.gameObject);
    this.SetLocaleButtonText(this.m_locale);
    gameObject1.SetActive(false);
  }

  private void OnClick_LocaleSetButton(Locale locale)
  {
    this.m_LocaleDropDownMainButton.GetComponentInChildren<UnityEngine.UI.Text>().text = locale.ToString();
    this.m_locale = locale;
    this.SaveLocale(this.m_locale);
    this.SetLocale();
  }

  private void SetLocaleButtonText(Locale loc) => this.m_LocaleDropDownMainButton.GetComponentInChildren<UnityEngine.UI.Text>().text = loc.ToString();

  private void SaveLocale(Locale loc)
  {
    Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetInt("CARD_TEXT_LOCALE", (int) this.m_locale);
    Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.Save();
  }

  private void SetLocale() => this.StartCoroutine(this.SetLocaleCoroutine());

  private IEnumerator SetLocaleCoroutine()
  {
    Localization.SetLocale(this.m_locale);
    yield return (object) null;
    this.UpdateCardFonts(Locale.enUS);
    this.UpdateCardFonts(this.m_locale);
  }

  private void UpdateCardFonts(Locale loc)
  {
    foreach (CardTextTool.LocalizedFont localizedFont in this.m_LocalizedFontCollection)
    {
      if (localizedFont.m_Locale == loc)
      {
        if (((UnityEngine.Object) localizedFont.m_FontDef).name == "FranklinGothic")
        {
          this.m_AbilityCardDescription.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_AllyCardDescription.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_WeaponCardDescription.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_HeroCardDescription.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_HeroPowerCardDescription.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_BossCardDescription.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_MercenariesAbilityCardDescription.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_MercenariesEquipmentCardDescription.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_MercenaryCardDescription.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_LocationCardDescription.SetFontWithoutLocalization(localizedFont.m_FontDef);
        }
        if (((UnityEngine.Object) localizedFont.m_FontDef).name == "Belwe_Outline")
        {
          this.m_AbilityCardName.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_AllyCardName.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_WeaponCardName.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_HeroCardName.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_HeroPowerName.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_BossName.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_MercenariesAbilityCardName.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_MercenariesEquipmentCardName.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_MercenaryCardName.SetFontWithoutLocalization(localizedFont.m_FontDef);
          this.m_LocationCardName.SetFontWithoutLocalization(localizedFont.m_FontDef);
        }
      }
    }
  }

  [Serializable]
  public class LocalizedFont
  {
    public Locale m_Locale;
    public FontDefinition m_FontDef;
  }
}
