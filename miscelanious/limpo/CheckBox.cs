using UnityEngine;

[CustomEditClass]
public class CheckBox : PegUIElement
{
  public GameObject m_check;
  public TextMesh m_text;
  public UberText m_uberText;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_checkOnSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_checkOffSound;
  private bool m_checked;
  private int m_buttonID;

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    if (!this.gameObject.activeInHierarchy)
      return;
    this.SetState(PegUIElement.InteractionState.Over);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    if (!this.gameObject.activeInHierarchy)
      return;
    this.SetState(PegUIElement.InteractionState.Up);
  }

  protected override void OnPress()
  {
    if (!this.gameObject.activeInHierarchy)
      return;
    this.SetState(PegUIElement.InteractionState.Down);
  }

  protected override void OnRelease()
  {
    if (!this.gameObject.activeInHierarchy)
      return;
    this.ToggleChecked();
    if (this.m_checked && !string.IsNullOrEmpty(this.m_checkOnSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_checkOnSound);
    else if (!this.m_checked && !string.IsNullOrEmpty(this.m_checkOffSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_checkOffSound);
    this.SetState(PegUIElement.InteractionState.Over);
  }

  public void SetButtonText(string s)
  {
    if ((Object) this.m_text != (Object) null)
      this.m_text.text = s;
    if (!((Object) this.m_uberText != (Object) null))
      return;
    this.m_uberText.Text = s;
  }

  public void SetButtonID(int id) => this.m_buttonID = id;

  public int GetButtonID() => this.m_buttonID;

  public void SetState(PegUIElement.InteractionState state)
  {
    this.SetEnabled(true);
    switch (state)
    {
    }
  }

  public virtual void SetChecked(bool isChecked)
  {
    this.m_checked = isChecked;
    if (!((Object) this.m_check != (Object) null))
      return;
    this.m_check.SetActive(this.m_checked);
  }

  public bool IsChecked() => this.m_checked;

  private bool ToggleChecked()
  {
    this.SetChecked(!this.m_checked);
    return this.m_checked;
  }
}
