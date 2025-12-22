using System;
using System.Collections;
using UnityEngine;

public class ManaFilterTab : PegUIElement
{
  public const int ALL_TAB_INDEX = -1;
  public const int MIN_MANA_AMOUNT = 0;
  public const int MAX_MANA_AMOUNT = 7;
  public UberText m_costText;
  public UberText m_otherText;
  public ManaCrystal m_crystal;
  private int m_manaID;
  private ManaFilterTab.FilterState m_filterState;
  private AudioSource m_mouseOverSound;

  protected override void Awake()
  {
    this.m_crystal.MarkAsNotInGame();
    base.Awake();
  }

  public void SetFilterState(ManaFilterTab.FilterState state)
  {
    this.m_filterState = state;
    switch (this.m_filterState)
    {
      case ManaFilterTab.FilterState.ON:
        this.m_crystal.state = ManaCrystal.State.PROPOSED;
        break;
      case ManaFilterTab.FilterState.OFF:
        this.m_crystal.state = ManaCrystal.State.READY;
        break;
      case ManaFilterTab.FilterState.DISABLED:
        this.m_crystal.state = ManaCrystal.State.USED;
        break;
    }
  }

  public void NotifyMousedOver()
  {
    if (this.m_filterState == ManaFilterTab.FilterState.ON)
      return;
    this.m_crystal.state = ManaCrystal.State.PROPOSED;
    SoundManager.Get().LoadAndPlay((AssetReference) "mana_crystal_highlight_lp.prefab:279503c4945c5d640b9f7403d764a49b", this.gameObject, 1f, new SoundManager.LoadedCallback(this.ManaCrystalSoundCallback));
  }

  public void NotifyMousedOut()
  {
    Hashtable args = iTween.Hash((object) "from", (object) 1f, (object) "to", (object) 0.0f, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.linear, (object) "onupdate", (object) (Action<object>) (amount => SoundManager.Get().SetVolume(this.m_mouseOverSound, (float) amount)));
    iTween.Stop(this.gameObject);
    iTween.ValueTo(this.gameObject, args);
    if (this.m_filterState == ManaFilterTab.FilterState.ON)
      return;
    this.m_crystal.state = ManaCrystal.State.READY;
  }

  private void ManaCrystalSoundCallback(AudioSource source, object userData)
  {
    if ((UnityEngine.Object) this.m_mouseOverSound != (UnityEngine.Object) null)
      SoundManager.Get().Stop(this.m_mouseOverSound);
    this.m_mouseOverSound = source;
    SoundManager.Get().SetVolume(source, 0.0f);
    if (this.m_crystal.state != ManaCrystal.State.PROPOSED)
      SoundManager.Get().Stop(this.m_mouseOverSound);
    Hashtable args = iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.linear, (object) "onupdate", (object) (Action<object>) (amount => SoundManager.Get().SetVolume(source, (float) amount)));
    iTween.Stop(this.gameObject);
    iTween.ValueTo(this.gameObject, args);
  }

  public void SetManaID(int manaID)
  {
    this.m_manaID = manaID;
    this.UpdateManaText();
  }

  public int GetManaID() => this.m_manaID;

  private void UpdateManaText()
  {
    string str1 = "";
    string str2 = "";
    if (this.m_manaID == -1)
    {
      str2 = GameStrings.Get("GLUE_COLLECTION_ALL");
    }
    else
    {
      str1 = this.m_manaID.ToString();
      if (this.m_manaID == 7)
      {
        if ((bool) UniversalInputManager.UsePhoneUI)
          str1 += GameStrings.Get("GLUE_COLLECTION_PLUS");
        else
          str2 = GameStrings.Get("GLUE_COLLECTION_PLUS");
      }
    }
    if ((UnityEngine.Object) this.m_costText != (UnityEngine.Object) null)
      this.m_costText.Text = str1;
    if (!((UnityEngine.Object) this.m_otherText != (UnityEngine.Object) null))
      return;
    this.m_otherText.Text = str2;
  }

  public enum FilterState
  {
    ON,
    OFF,
    DISABLED,
  }
}
