using UnityEngine;

public class EmoteEntry
{
  private EmoteType m_emoteType;
  private Spell m_emoteSpell;
  private CardSoundSpell m_emoteSoundSpell;
  private string m_emoteGameStringKey;
  private string m_emoteSpellPath;
  private string m_emoteSoundSpellPath;
  private Component m_owner;

  public EmoteEntry(
    EmoteType type,
    string spellPath,
    string soundSpellPath,
    string stringKey,
    Card owner)
  {
    this.m_emoteType = type;
    this.m_emoteSpellPath = spellPath;
    this.m_emoteSoundSpellPath = soundSpellPath;
    this.m_emoteGameStringKey = stringKey;
    this.m_owner = (Component) owner;
  }

  public EmoteEntry(
    EmoteType type,
    string spellPath,
    string soundSpellPath,
    string stringKey,
    Actor owner)
  {
    this.m_emoteType = type;
    this.m_emoteSpellPath = spellPath;
    this.m_emoteSoundSpellPath = soundSpellPath;
    this.m_emoteGameStringKey = stringKey;
    this.m_owner = (Component) owner;
  }

  public EmoteType GetEmoteType() => this.m_emoteType;

  public string GetGameStringKey()
  {
    if ((Object) this.m_emoteSoundSpell != (Object) null)
    {
      string gameStringKey = this.m_emoteSoundSpell.DetermineGameStringKey();
      if (!string.IsNullOrEmpty(gameStringKey))
        this.m_emoteGameStringKey = gameStringKey;
    }
    return this.m_emoteGameStringKey;
  }

  private void LoadSoundSpell()
  {
    if (string.IsNullOrEmpty(this.m_emoteSoundSpellPath))
      return;
    this.m_emoteSoundSpell = SpellManager.Get().GetSpell(this.m_emoteSoundSpellPath) as CardSoundSpell;
    if ((Object) this.m_emoteSoundSpell == (Object) null)
    {
      Error.AddDevFatalUnlessWorkarounds("EmoteEntry.LoadSoundSpell() - \"{0}\" does not have a Spell component.", (object) this.m_emoteSoundSpellPath);
    }
    else
    {
      if (!((Object) this.m_owner != (Object) null))
        return;
      SpellUtils.SetupSoundSpell(this.m_emoteSoundSpell, this.m_owner);
    }
  }

  public CardSoundSpell GetSoundSpell(bool loadIfNeeded = true)
  {
    if ((Object) this.m_emoteSoundSpell == (Object) null & loadIfNeeded)
      this.LoadSoundSpell();
    return this.m_emoteSoundSpell;
  }

  public Spell GetSpell(bool loadIfNeeded = true)
  {
    if ((Object) this.m_emoteSpell == (Object) null & loadIfNeeded && !string.IsNullOrEmpty(this.m_emoteSpellPath))
      this.m_emoteSpell = SpellUtils.LoadAndSetupSpell(this.m_emoteSpellPath, this.m_owner);
    return this.m_emoteSpell;
  }

  public void Clear()
  {
    SpellManager spellManager = SpellManager.Get();
    if (spellManager == null)
      return;
    if ((Object) this.m_emoteSoundSpell != (Object) null)
    {
      spellManager.ReleaseSpell((Spell) this.m_emoteSoundSpell);
      this.m_emoteSoundSpell = (CardSoundSpell) null;
    }
    if (!((Object) this.m_emoteSpell != (Object) null))
      return;
    spellManager.ReleaseSpell(this.m_emoteSpell);
    this.m_emoteSpell = (Spell) null;
  }
}
