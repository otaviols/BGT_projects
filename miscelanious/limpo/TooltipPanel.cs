using UnityEngine;

public class TooltipPanel : MonoBehaviour
{
  public UberText m_name;
  public UberText m_body;
  public GameObject m_background;
  private float k_scaleOffset = 1.2f;
  private float k_scaleOffsetPhone = 4.2f;
  private float k_scaleOffsetTablet = 2f;
  private bool m_destroyed;
  protected float m_initialBackgroundHeight;
  protected Vector3 m_initialBackgroundScale = Vector3.zero;
  public const float GAMEPLAY_SCALE_FOR_SHOW_TOOLTIP = 0.75f;
  public static PlatformDependentValue<float> HAND_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 0.65f,
    Phone = 0.8f
  };
  public static PlatformDependentValue<float> GAMEPLAY_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 0.75f,
    Phone = 1.4f
  };
  public static PlatformDependentValue<float> GAMEPLAY_SCALE_LARGE = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 0.9f,
    Phone = 0.625f
  };
  public static PlatformDependentValue<float> HISTORY_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 0.48f,
    Phone = 0.853f
  };
  public static PlatformDependentValue<float> MULLIGAN_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 0.65f,
    Phone = 0.4f
  };
  public const float GAMEPLAY_HERO_POWER_SCALE = 0.6f;
  public static PlatformDependentValue<float> BOX_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 8f,
    Phone = 4.5f
  };
  public const float OPEN_BOX_SCALE_FOR_SHOW_TOOLTIP = 4f;
  public static PlatformDependentValue<float> COLLECTION_MANAGER_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 4f,
    Phone = 4.5f
  };
  public static PlatformDependentValue<float> FORGE_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 4f,
    Phone = 8f
  };
  public static PlatformDependentValue<float> ADVENTURE_SCALE = TooltipPanel.FORGE_SCALE;
  public const float PACK_OPENING_SCALE = 2.75f;
  public const float UNOPENED_PACK_SCALE = 5f;
  public const float DECK_HELPER_SCALE = 3.75f;
  protected float m_scaleToUse;

  public bool Destroyed => this.m_destroyed || !(bool) (Object) this.m_name || !(bool) (Object) this.m_body;

  private void Awake()
  {
    LayerUtils.SetLayer(this.gameObject, GameLayer.Tooltip);
    this.m_scaleToUse = (float) TooltipPanel.GAMEPLAY_SCALE;
  }

  public void Reset()
  {
    this.transform.localScale = Vector3.one;
    this.transform.eulerAngles = Vector3.zero;
  }

  public void SetScale(float newScale)
  {
    this.m_scaleToUse = newScale;
    this.transform.localScale = new Vector3(this.m_scaleToUse, this.m_scaleToUse, this.m_scaleToUse);
  }

  public virtual void Initialize(string keywordName, string keywordText)
  {
    this.SetName(keywordName);
    this.SetBodyText(keywordText);
    this.gameObject.SetActive(true);
    this.m_name.UpdateNow();
    this.m_body.UpdateNow();
  }

  public void SetName(string s) => this.m_name.Text = s;

  public void SetBodyText(string s) => this.m_body.Text = s;

  public virtual float GetHeight() => this.m_background.GetComponent<Renderer>().bounds.size.z;

  public virtual float GetWidth() => this.m_background.GetComponent<Renderer>().bounds.size.x;

  public bool IsTextRendered() => this.m_name.IsDone() && this.m_body.IsDone();

  public void ShiftBodyText()
  {
    if (this.Destroyed || this.m_name.Text.Length != 0)
      return;
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_body.transform.position += new Vector3(0.0f, 0.0f, this.m_name.Height + this.m_name.LineSpacing * this.k_scaleOffsetPhone);
    else if (PlatformSettings.IsMobileRuntimeOS && !(bool) UniversalInputManager.UsePhoneUI)
      this.m_body.transform.position += new Vector3(0.0f, 0.0f, this.m_name.Height + this.m_name.LineSpacing * this.k_scaleOffsetTablet);
    else
      this.m_body.transform.position += new Vector3(0.0f, 0.0f, this.m_name.Height + this.m_name.LineSpacing * this.k_scaleOffset);
  }
}
