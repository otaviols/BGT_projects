using UnityEngine;

public class RadioButton : PegUIElement
{
  public GameObject m_hoverGlow;
  public GameObject m_selectedGlow;
  private int m_id;
  private object m_userData;

  protected override void Awake()
  {
    base.Awake();
    this.m_hoverGlow.SetActive(false);
    this.m_selectedGlow.SetActive(false);
    SoundManager.Get().Load((AssetReference) "tiny_button_press_2.prefab:dab8dd96f82865041bbf96a32e47642e");
    SoundManager.Get().Load((AssetReference) "tiny_button_mouseover_2.prefab:ba1a1effe29265246b1cb3d833c8ac78");
  }

  public void SetButtonID(int id) => this.m_id = id;

  public int GetButtonID() => this.m_id;

  public void SetUserData(object userData) => this.m_userData = userData;

  public object GetUserData() => this.m_userData;

  public void SetSelected(bool selected) => this.m_selectedGlow.SetActive(selected);

  public bool IsSelected() => this.m_selectedGlow.activeSelf;

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "tiny_button_mouseover_2.prefab:ba1a1effe29265246b1cb3d833c8ac78");
    this.m_hoverGlow.SetActive(true);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState) => this.m_hoverGlow.SetActive(false);

  protected override void OnRelease()
  {
    base.OnRelease();
    SoundManager.Get().LoadAndPlay((AssetReference) "tiny_button_press_2.prefab:dab8dd96f82865041bbf96a32e47642e");
  }

  protected override void OnDoubleClick()
  {
  }
}
