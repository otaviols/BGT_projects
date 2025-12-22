using System.Collections.Generic;
using UnityEngine;

public class TutorialKeywordManager : MonoBehaviour
{
  public TutorialKeywordTooltip m_keywordPanelPrefab;
  private static TutorialKeywordManager s_instance;
  private List<TutorialKeywordTooltip> m_keywordPanels;
  private Actor m_actor;
  private Card m_card;

  private void Awake() => TutorialKeywordManager.s_instance = this;

  private void OnDestroy() => TutorialKeywordManager.s_instance = (TutorialKeywordManager) null;

  public static TutorialKeywordManager Get() => TutorialKeywordManager.s_instance;

  public void UpdateKeywordHelp(Card c, Actor a) => this.UpdateKeywordHelp(c, a, true);

  public void UpdateKeywordHelp(Card card, Actor actor, bool showOnRight, float? overrideScale = null)
  {
    this.m_card = card;
    this.UpdateKeywordHelp(card.GetEntity(), actor, showOnRight, overrideScale);
  }

  public void UpdateKeywordHelp(
    Entity entity,
    Actor actor,
    bool showOnRight,
    float? overrideScale = null)
  {
    float num1 = 1f;
    if (overrideScale.HasValue)
      num1 = overrideScale.Value;
    this.PrepareToUpdateKeywordHelp(actor);
    string[] strArray = GameState.Get().GetGameEntity().NotifyOfKeywordHelpPanelDisplay(entity);
    if (strArray != null)
      this.SetupKeywordPanel(strArray[0], strArray[1]);
    this.SetUpPanels((EntityBase) entity);
    TutorialKeywordTooltip tutorialKeywordTooltip = (TutorialKeywordTooltip) null;
    for (int index = 0; index < this.m_keywordPanels.Count; ++index)
    {
      TutorialKeywordTooltip keywordPanel = this.m_keywordPanels[index];
      float num2 = 1.05f;
      if (entity.IsHero())
        num2 = 1.2f;
      else if (entity.GetZone() == TAG_ZONE.PLAY)
      {
        if ((bool) UniversalInputManager.UsePhoneUI)
          num1 = 1.7f;
        num2 = 1.45f * num1;
      }
      keywordPanel.transform.localScale = new Vector3(num1, num1, num1);
      Bounds bounds = this.m_actor.GetMeshRenderer().bounds;
      float num3 = (float) (-0.200000002980232 * (double) bounds.size.z);
      if ((bool) UniversalInputManager.UsePhoneUI && entity.GetZone() == TAG_ZONE.PLAY)
        num3 += 1.5f;
      if (index == 0)
      {
        if (showOnRight)
        {
          Transform transform = keywordPanel.transform;
          Vector3 position = this.m_actor.transform.position;
          bounds = this.m_actor.GetMeshRenderer().bounds;
          double x = (double) bounds.size.x * (double) num2;
          bounds = this.m_actor.GetMeshRenderer().bounds;
          double z = (double) bounds.extents.z + (double) num3;
          Vector3 vector3_1 = new Vector3((float) x, 0.0f, (float) z);
          Vector3 vector3_2 = position + vector3_1;
          transform.position = vector3_2;
        }
        else
        {
          Transform transform = keywordPanel.transform;
          Vector3 position = this.m_actor.transform.position;
          bounds = this.m_actor.GetMeshRenderer().bounds;
          double x = -(double) bounds.size.x * (double) num2;
          bounds = this.m_actor.GetMeshRenderer().bounds;
          double z = (double) bounds.extents.z + (double) num3;
          Vector3 vector3_3 = new Vector3((float) x, 0.0f, (float) z);
          Vector3 vector3_4 = position + vector3_3;
          transform.position = vector3_4;
        }
      }
      else
        keywordPanel.transform.position = tutorialKeywordTooltip.transform.position - new Vector3(0.0f, 0.0f, (float) ((double) tutorialKeywordTooltip.GetHeight() * 0.349999994039536 + (double) keywordPanel.GetHeight() * 0.349999994039536));
      tutorialKeywordTooltip = keywordPanel;
    }
    GameState.Get().GetGameEntity().NotifyOfHelpPanelDisplay(this.m_keywordPanels.Count);
  }

  private void PrepareToUpdateKeywordHelp(Actor actor)
  {
    this.HideKeywordHelp();
    this.m_actor = actor;
    this.m_keywordPanels = new List<TutorialKeywordTooltip>();
  }

  private void SetUpPanels(EntityBase entityInfo)
  {
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.TAUNT);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.STEALTH);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DIVINE_SHIELD);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLPOWER);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.ENRAGED);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.CHARGE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.BATTLECRY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.FROZEN);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.FREEZE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.WINDFURY);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SECRET);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DEATHRATTLE);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.OVERLOAD);
    this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.COMBO);
  }

  private bool SetupKeywordPanelIfNecessary(EntityBase entityInfo, GAME_TAG tag)
  {
    if (entityInfo.HasTag(tag))
    {
      this.SetupKeywordPanel(tag);
      return true;
    }
    if (!entityInfo.HasReferencedTag(tag))
      return false;
    this.SetupKeywordRefPanel(tag);
    return true;
  }

  public void SetupKeywordPanel(GAME_TAG tag) => this.SetupKeywordPanel(GameStrings.GetKeywordName(tag), GameStrings.GetKeywordText(tag));

  public void SetupKeywordRefPanel(GAME_TAG tag) => this.SetupKeywordPanel(GameStrings.GetKeywordName(tag), GameStrings.GetRefKeywordText(tag));

  public void SetupKeywordPanel(string headline, string description)
  {
    TutorialKeywordTooltip component = Object.Instantiate<GameObject>(this.m_keywordPanelPrefab.gameObject).GetComponent<TutorialKeywordTooltip>();
    if ((Object) component == (Object) null)
      return;
    component.Initialize(headline, description);
    this.m_keywordPanels.Add(component);
  }

  public void HideKeywordHelp()
  {
    if (this.m_keywordPanels == null)
      return;
    foreach (TutorialKeywordTooltip keywordPanel in this.m_keywordPanels)
    {
      if (!((Object) keywordPanel == (Object) null))
        Object.Destroy((Object) keywordPanel.gameObject);
    }
  }

  public Card GetCard() => this.m_card;

  public Vector3 GetPositionOfTopPanel() => this.m_keywordPanels == null || this.m_keywordPanels.Count == 0 ? new Vector3(0.0f, 0.0f, 0.0f) : this.m_keywordPanels[0].transform.position;
}
