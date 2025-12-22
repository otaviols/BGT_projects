using Hearthstone.UI;
using UnityEngine;

public class EventBoxDressing : MonoBehaviour
{
  [SerializeField]
  private Material m_boxMaterial;
  [SerializeField]
  private Material m_boxMaterialMobile;
  [SerializeField]
  private Material m_tableMaterial;
  [SerializeField]
  private Material m_bottomSpinnerMaterial;
  [SerializeField]
  private Material m_spotLightMaterial;
  [SerializeField]
  private Material m_setRotationButtonMaterial;
  [SerializeField]
  private MusicPlaylistType m_playlistToPlay;
  [SerializeField]
  private WeakAssetReference m_innkeeperGreetings;
  private EventBoxDressing.BoxDressingMaterials m_materials;

  public void Start() => this.m_materials = new EventBoxDressing.BoxDressingMaterials((bool) UniversalInputManager.UsePhoneUI ? this.m_boxMaterialMobile : this.m_boxMaterial, this.m_tableMaterial, this.m_bottomSpinnerMaterial, this.m_spotLightMaterial, this.m_setRotationButtonMaterial);

  public EventBoxDressing.BoxDressingMaterials GetBoxDressingMaterials() => this.m_materials;

  public MusicPlaylistType GetPlaylistType() => this.m_playlistToPlay;

  public WeakAssetReference GetInnkeeperGreetings() => this.m_innkeeperGreetings;

  public enum State
  {
    UNKNOWN = -1, // 0xFFFFFFFF
    DISABLED = 0,
    ENABLED = 1,
  }

  public class BoxDressingMaterials
  {
    private Material m_boxMaterial;
    private Material m_tableMaterial;
    private Material m_bottomSpinnerMaterial;
    private Material m_spotLightMaterial;
    private Material m_setRotationButtonMaterial;

    public BoxDressingMaterials(
      Material box,
      Material table,
      Material bottomSpinner,
      Material spotLight,
      Material setRotationButton)
    {
      this.m_boxMaterial = box;
      this.m_tableMaterial = table;
      this.m_bottomSpinnerMaterial = bottomSpinner;
      this.m_spotLightMaterial = spotLight;
      this.m_setRotationButtonMaterial = setRotationButton;
    }

    public Material BoxMaterial => this.m_boxMaterial;

    public Material TableMaterial => this.m_tableMaterial;

    public Material BottomSpinnerMaterial => this.m_bottomSpinnerMaterial;

    public Material SpotLightMaterial => this.m_spotLightMaterial;

    public Material SetRotationButtonMaterial => this.m_setRotationButtonMaterial;
  }
}
