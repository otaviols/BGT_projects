using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UngoroPackOpeningSpell : SuperSpell
{
  private List<Entity> m_newCards;
  private List<int> m_fullEntityTaskIndices;
  private List<Transform> cardDestinations = new List<Transform>();
  private Transform cardSpawningPosition;
  public float m_CardFlyOutTime = 2f;
  public float m_CardHangTime = 3f;
  private int previousLayer;

  public override bool AddPowerTargets()
  {
    if (!this.CanAddPowerTargets())
      return false;
    this.m_newCards = new List<Entity>();
    this.m_fullEntityTaskIndices = new List<int>();
    this.FindNewCardsFullEntityTask();
    return true;
  }

  private void FindNewCardsFullEntityTask()
  {
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      if (taskList[index].GetPower() is Network.HistFullEntity power)
      {
        foreach (Network.Entity.Tag tag in power.Entity.Tags)
        {
          if (tag.Name == 49 && tag.Value == 3)
          {
            this.m_fullEntityTaskIndices.Add(index);
            this.m_newCards.Add(GameState.Get().GetEntity(power.Entity.ID));
            break;
          }
        }
      }
    }
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    UngoroPackOpeningPositioner component = this.m_activeAreaEffectSpell.GetComponent<UngoroPackOpeningPositioner>();
    if ((Object) component == (Object) null)
    {
      Log.Spells.PrintError("UngoroPackOpeningSpell.OnAction(): No UngoroPackOpeningPositioner found on spell {0}.", (object) this.m_activeAreaEffectSpell.gameObject.name);
      this.OnSpellFinished();
      this.OnStateFinished();
    }
    else if (this.m_newCards.Count <= 0)
    {
      this.OnSpellFinished();
      this.OnStateFinished();
    }
    else
    {
      ++this.m_effectsPendingFinish;
      this.m_activeAreaEffectSpell.AddSpellEventCallback(new Spell.SpellEventCallback(this.OnSpellEvent));
      this.cardDestinations = component.GetPositioningBonesForCardCount(this.m_newCards.Count);
      this.cardSpawningPosition = component.m_PackSpawningBone;
      this.StartCoroutine(this.SpawnAndHideReceivedCards());
    }
  }

  private IEnumerator SpawnAndHideReceivedCards()
  {
    UngoroPackOpeningSpell packOpeningSpell = this;
    int startIndex1 = 0;
    Player.Side controllerSide = packOpeningSpell.m_newCards[0].GetControllerSide();
    ZoneMgr.Get().FindZoneOfType<ZoneHand>(controllerSide).AddLayoutBlocker();
    for (int i = 0; i < packOpeningSpell.m_newCards.Count; ++i)
    {
      bool complete = false;
      PowerTaskList.CompleteCallback callback = (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true);
      int count1 = 1 + (packOpeningSpell.m_fullEntityTaskIndices[i] - startIndex1);
      Card newCard = packOpeningSpell.m_newCards[i].GetCard();
      newCard.SetDoNotSort(true);
      newCard.SetDoNotWarpToNewZone(true);
      newCard.SetInputEnabled(false);
      packOpeningSpell.m_taskList.DoTasks(startIndex1, count1, callback);
      while ((Object) newCard.GetActor() == (Object) null || newCard.IsActorLoading())
        yield return (object) null;
      newCard.HideCard();
      while (!complete)
        yield return (object) null;
      startIndex1 = packOpeningSpell.m_fullEntityTaskIndices[i] + 1;
      newCard = (Card) null;
    }
    if (packOpeningSpell.m_newCards.Count > 0)
      packOpeningSpell.previousLayer = packOpeningSpell.m_newCards[0].GetCard().gameObject.layer;
  }

  public void OnSpellEvent(string eventName, object eventData, object userData)
  {
    this.PlayInnkeeperVO();
    this.StartCoroutine(this.SplayOutReceivedCards());
  }

  private void PlayInnkeeperVO()
  {
    TAG_RARITY tagRarity = TAG_RARITY.INVALID;
    TAG_PREMIUM tagPremium = TAG_PREMIUM.NORMAL;
    foreach (Entity newCard in this.m_newCards)
    {
      if (!newCard.IsHidden())
      {
        TAG_RARITY rarity = newCard.GetRarity();
        TAG_PREMIUM premiumType = newCard.GetPremiumType();
        if (rarity > tagRarity)
        {
          tagRarity = rarity;
          tagPremium = premiumType;
        }
        else if (rarity == tagRarity && premiumType == TAG_PREMIUM.GOLDEN)
          tagPremium = premiumType;
      }
    }
    switch (tagRarity)
    {
      case TAG_RARITY.COMMON:
        if (tagPremium != TAG_PREMIUM.GOLDEN)
          break;
        SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_FOIL_C_29.prefab:69820e4999e4afa439761151e057a526");
        break;
      case TAG_RARITY.RARE:
        if (tagPremium == TAG_PREMIUM.GOLDEN)
        {
          SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_FOIL_R_30.prefab:f5bf5bfd8e5f4d247aa8a6da966969cf");
          break;
        }
        SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_RARE_27.prefab:8ff0de7a4fd144b4b983caea4c54da4d");
        break;
      case TAG_RARITY.EPIC:
        if (tagPremium == TAG_PREMIUM.GOLDEN)
        {
          SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_FOIL_E_31.prefab:d419d6eca0e2a72469544bae5f11542f");
          break;
        }
        SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_EPIC_26.prefab:e76d67f55b976104794c3cf73382e82a");
        break;
      case TAG_RARITY.LEGENDARY:
        if (tagPremium == TAG_PREMIUM.GOLDEN)
        {
          SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_FOIL_L_32.prefab:caefd66acfc4e2b4f858035c274b257e");
          break;
        }
        SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_LEGENDARY_25.prefab:e015c982aec12bc4893f36396d426750");
        break;
    }
  }

  private IEnumerator SplayOutReceivedCards()
  {
    UngoroPackOpeningSpell packOpeningSpell = this;
    for (int i = 0; i < packOpeningSpell.m_newCards.Count; ++i)
    {
      Card newCard = packOpeningSpell.m_newCards[i].GetCard();
      while ((Object) newCard.GetActor() == (Object) null || newCard.IsActorLoading())
        yield return (object) null;
      newCard = (Card) null;
    }
    for (int index = 0; index < packOpeningSpell.m_newCards.Count; ++index)
    {
      Card card = packOpeningSpell.m_newCards[index].GetCard();
      TransformUtil.CopyWorld((Component) card, (Component) packOpeningSpell.cardSpawningPosition);
      card.ShowCard();
      LayerUtils.SetLayer(card.gameObject, GameLayer.Tooltip);
      Transform cardDestination = packOpeningSpell.cardDestinations[index];
      card.transform.localScale = new Vector3(card.transform.localScale.x * cardDestination.localScale.x, card.transform.localScale.y * cardDestination.localScale.y, card.transform.localScale.z * cardDestination.localScale.z);
      Vector3 position = packOpeningSpell.cardDestinations[index].position;
      iTween.MoveTo(card.gameObject, position, packOpeningSpell.m_CardFlyOutTime);
    }
    yield return (object) new WaitForSeconds(packOpeningSpell.m_CardHangTime);
    for (int index = 0; index < packOpeningSpell.m_newCards.Count; ++index)
    {
      Card card = packOpeningSpell.m_newCards[index].GetCard();
      card.SetTransitionStyle(ZoneTransitionStyle.VERY_SLOW);
      card.SetDoNotSort(false);
      card.SetDoNotWarpToNewZone(false);
      card.SetInputEnabled(true);
      LayerUtils.SetLayer(card.gameObject, packOpeningSpell.previousLayer);
    }
    Zone zone = packOpeningSpell.m_newCards[0].GetCard().GetZone();
    zone.RemoveLayoutBlocker();
    zone.UpdateLayout();
    --packOpeningSpell.m_effectsPendingFinish;
    packOpeningSpell.OnSpellFinished();
    packOpeningSpell.OnStateFinished();
  }
}
