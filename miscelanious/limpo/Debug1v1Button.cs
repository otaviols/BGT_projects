using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using UnityEngine;

public class Debug1v1Button : PegUIElement
{
  public int m_missionId;
  public GameObject m_heroImage;
  public UberText m_name;
  private GameObject m_heroPowerObject;

  public static bool HasUsedDebugMenu { get; set; }

  private void Start()
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.m_missionId);
    if (record == null)
      return;
    string shortName = (string) record.ShortName;
    if (!((Object) this.m_name != (Object) null) || string.IsNullOrEmpty(shortName))
      return;
    this.m_name.Text = shortName;
  }

  private void OnCardDefLoaded(string cardID, CardDef cardDef, object userData) => this.m_heroImage.GetComponent<Renderer>().GetMaterial().mainTexture = cardDef.GetPortraitTexture(TAG_PREMIUM.NORMAL);

  protected override void OnRelease()
  {
    base.OnRelease();
    long selectedDeckId = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
    Debug1v1Button.HasUsedDebugMenu = true;
    GameMgr.Get().FindGame(GameType.GT_TAVERNBRAWL, FormatType.FT_WILD, this.m_missionId, deckId: selectedDeckId);
    Object.Destroy((Object) this.transform.parent.gameObject);
  }
}
