using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class StoreModeCardButton : UIBButton
{
  public Texture m_dustTexture;

  protected override void Awake()
  {
    base.Awake();
    if ((Object) this.m_dustTexture == (Object) null || !(BattleNet.GetAccountCountry() == "CHN"))
      return;
    Material material = this.m_RootObject.GetComponent<Renderer>().GetMaterial();
    if (!((Object) material != (Object) null))
      return;
    material.SetTexture("_MainTex", this.m_dustTexture);
    this.m_ButtonText.Text = GameStrings.Get("GLUE_STORE_MODE_NAME_DUST");
  }
}
