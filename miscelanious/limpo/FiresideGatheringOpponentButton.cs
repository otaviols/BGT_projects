using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class FiresideGatheringOpponentButton : PegUIElement
{
  public UberText m_name;
  public GameObject m_highlight;
  public GameObject m_rootObject;
  public Transform m_upBone;
  public Transform m_downBone;
  public Color m_friendNameColor;
  public Color m_patronNameColor;
  public MeshRenderer m_mainButtonMesh;
  public Material m_friendlyDuelsMaterial;
  public Material m_firesideBrawlMaterial;
  private BnetPlayer m_associatedBnetPlayer;

  public BnetPlayer AssociatedBnetPlayer
  {
    get => this.m_associatedBnetPlayer;
    set => this.m_associatedBnetPlayer = value;
  }

  public void SetName(string name) => this.m_name.Text = name;

  public void Select()
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "select_AI_opponent.prefab:a48887f01f79fa743a0c5de53a959b60", this.gameObject);
    this.m_highlight.SetActive(true);
    this.SetEnabled(false);
    this.Depress();
  }

  public void Deselect()
  {
    this.m_highlight.SetActive(false);
    this.Raise();
    this.SetEnabled(true);
  }

  public void Raise() => this.Raise(0.1f);

  public void SetIsFriend(bool isFriend) => this.m_name.TextColor = isFriend ? this.m_friendNameColor : this.m_patronNameColor;

  public void SetIsFiresideBrawl(bool isFiresideBrawl) => RendererExtension.SetMaterial((Renderer) this.m_mainButtonMesh, isFiresideBrawl ? this.m_firesideBrawlMaterial : this.m_friendlyDuelsMaterial);

  private void Raise(float time) => iTween.MoveTo(this.m_rootObject, iTween.Hash((object) "position", (object) this.m_upBone.localPosition, (object) nameof (time), (object) time, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true));

  private void Depress() => iTween.MoveTo(this.m_rootObject, iTween.Hash((object) "position", (object) this.m_downBone.localPosition, (object) "time", (object) 0.1f, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true));

  protected override void OnOver(PegUIElement.InteractionState oldState) => SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_hero_mouse_over.prefab:653cc8000b988cd468d2210a209adce6", this.gameObject);
}
