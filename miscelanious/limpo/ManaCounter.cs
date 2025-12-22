using UnityEngine;

public class ManaCounter : MonoBehaviour
{
  public Player.Side m_Side;
  public GameObject m_phoneGemContainer;
  public UberText m_availableManaPhone;
  public UberText m_permanentManaPhone;
  private Player m_player;
  private UberText m_textMesh;
  private GameObject m_phoneGem;

  private void Awake()
  {
    this.m_textMesh = this.GetComponent<UberText>();
    if (this.m_Side == Player.Side.FRIENDLY)
      return;
    if ((Object) this.m_availableManaPhone != (Object) null)
    {
      string message = "The property m_availableManaPhone is set on ManaCounter for non-friendly mana crystals. This should be null.";
      SceneDebugger.Get().AddErrorMessage(message);
    }
    if (!((Object) this.m_permanentManaPhone != (Object) null))
      return;
    string message1 = "The property m_permanentManaPhone is set on ManaCounter for non-friendly mana crystals. This should be null.";
    SceneDebugger.Get().AddErrorMessage(message1);
  }

  private void Start() => this.m_textMesh.Text = GameStrings.Format("GAMEPLAY_MANA_COUNTER", (object) "0", (object) "0");

  public void InitializeLargeResourceGameObject(string resourcePath)
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    if ((Object) this.m_phoneGem != (Object) null)
      Object.Destroy((Object) this.m_phoneGem);
    this.m_phoneGem = AssetLoader.Get().InstantiatePrefab((AssetReference) resourcePath, AssetLoadingOptions.IgnorePrefabPosition);
    GameUtils.SetParent(this.m_phoneGem, this.m_phoneGemContainer, true);
  }

  public void SetPlayer(Player player) => this.m_player = player;

  public Player GetPlayer() => this.m_player;

  public GameObject GetPhoneGem() => this.m_phoneGem;

  public void UpdateText()
  {
    if (!this.gameObject.activeInHierarchy)
      return;
    int tag = this.m_player.GetTag(GAME_TAG.RESOURCES);
    if (!this.gameObject.activeInHierarchy)
      this.gameObject.SetActive(true);
    int availableResources = this.m_player.GetNumAvailableResources();
    string str;
    if ((bool) UniversalInputManager.UsePhoneUI && tag >= 10)
      str = availableResources.ToString();
    else
      str = GameStrings.Format("GAMEPLAY_MANA_COUNTER", (object) availableResources, (object) tag);
    this.m_textMesh.Text = str;
    if (!(bool) UniversalInputManager.UsePhoneUI || !((Object) this.m_availableManaPhone != (Object) null) || this.m_Side != Player.Side.FRIENDLY)
      return;
    this.m_availableManaPhone.Text = availableResources.ToString();
    this.m_permanentManaPhone.Text = tag.ToString();
  }
}
