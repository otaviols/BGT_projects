using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

[CustomEditClass]
public class AdventureRewardsChest : MonoBehaviour
{
  private const string s_EventBlinkChest = "BlinkChest";
  private const string s_EventOpenChest = "OpenChest";
  private const string s_EventSlamInCheckmark = "SlamInCheckmark";
  private const string s_EventBurstCheckmark = "BurstCheckmark";
  private const string s_EventFadeInChest = "FadeChestIn";
  private const string s_EventFadeOutChest = "FadeChestOut";
  [CustomEditField(Sections = "Event Table")]
  public StateEventTable m_EventTable;
  [CustomEditField(Sections = "UI")]
  public PegUIElement m_ChestClickArea;
  [CustomEditField(Sections = "UI")]
  public GameObject m_CheckmarkContainer;
  [CustomEditField(Sections = "UI")]
  public GameObject m_ChestContainer;
  [CustomEditField(Sections = "UI")]
  public GameObject m_GameSaveDataProgressContainer;
  [CustomEditField(Sections = "UI")]
  public MeshRenderer m_ChestQuad;

  public bool m_fadedOut { get; private set; }

  public void AddChestEventListener(UIEventType type, UIEvent.Handler handler) => this.m_ChestClickArea.AddEventListener(type, handler);

  public void RemoveChestEventListener(UIEventType type, UIEvent.Handler handler) => this.m_ChestClickArea.RemoveEventListener(type, handler);

  public void SlamInCheckmark()
  {
    this.ShowCheckmark();
    this.m_EventTable.TriggerState(nameof (SlamInCheckmark));
  }

  public void ShowCheckmark()
  {
    this.m_CheckmarkContainer.SetActive(true);
    this.m_ChestContainer.SetActive(false);
    this.m_GameSaveDataProgressContainer.SetActive(false);
  }

  public void BurstCheckmark()
  {
    this.ShowCheckmark();
    this.m_EventTable.TriggerState(nameof (BurstCheckmark));
  }

  public void BlinkChest()
  {
    if (this.m_fadedOut)
      return;
    this.ShowCheckmark();
    this.m_EventTable.TriggerState(nameof (BlinkChest));
  }

  public void ShowChest()
  {
    this.m_CheckmarkContainer.SetActive(false);
    this.m_ChestContainer.SetActive(true);
    this.m_GameSaveDataProgressContainer.SetActive(false);
  }

  public void ShowGameSaveDataProgress(int progress, int maxProgress)
  {
    this.m_CheckmarkContainer.SetActive(false);
    this.m_ChestContainer.SetActive(false);
    if (progress > 0)
      this.m_GameSaveDataProgressContainer.SetActive(true);
    this.m_GameSaveDataProgressContainer.GetComponentInChildren<UberText>().Text = string.Format("{0}/{1}", (object) progress, (object) maxProgress);
  }

  public void HideAll()
  {
    this.m_CheckmarkContainer.SetActive(false);
    this.m_ChestContainer.SetActive(false);
    this.m_GameSaveDataProgressContainer.SetActive(false);
  }

  public void Enable(bool enable)
  {
    if (!((Object) this.m_ChestClickArea != (Object) null))
      return;
    this.m_ChestClickArea.gameObject.SetActive(enable);
  }

  public void FadeInChest()
  {
    this.m_EventTable.TriggerState("FadeChestIn");
    this.m_fadedOut = false;
  }

  public void FadeOutChest()
  {
    this.m_EventTable.TriggerState("FadeChestOut");
    this.m_fadedOut = true;
  }

  public void FadeOutChestImmediate()
  {
    Color white = Color.white with { a = 0.0f };
    this.m_ChestQuad.GetMaterial().SetColor("_Color", white);
    this.m_fadedOut = true;
  }
}
