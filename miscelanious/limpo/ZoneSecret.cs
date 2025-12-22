using System.Collections.Generic;
using UnityEngine;

public class ZoneSecret : Zone
{
  private const float MAX_LAYOUT_PYRAMID_LEVEL = 2f;
  private const float LAYOUT_ANIM_SEC = 1f;

  private void Awake()
  {
    if (GameState.Get() == null)
      return;
    GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
  }

  public override void UpdateLayout()
  {
    ++this.m_updatingLayout;
    if (this.IsBlockingLayout())
      this.UpdateLayoutFinished();
    else if ((bool) UniversalInputManager.UsePhoneUI)
      this.UpdateLayout_Phone();
    else
      this.UpdateLayout_Default();
  }

  public List<Card> GetSecretCards()
  {
    List<Card> secretCards = new List<Card>();
    foreach (Card card in this.m_cards)
    {
      if (card.GetEntity() != null && card.GetEntity().IsSecret())
        secretCards.Add(card);
    }
    return secretCards;
  }

  public List<Card> GetSideQuestCards()
  {
    List<Card> sideQuestCards = new List<Card>();
    foreach (Card card in this.m_cards)
    {
      if (card.GetEntity() != null && card.GetEntity().IsSideQuest())
        sideQuestCards.Add(card);
    }
    return sideQuestCards;
  }

  public List<Card> GetSigilCards()
  {
    List<Card> sigilCards = new List<Card>();
    foreach (Card card in this.m_cards)
    {
      if (card.GetEntity() != null && card.GetEntity().IsSigil())
        sigilCards.Add(card);
    }
    return sigilCards;
  }

  public List<Card> GetObjectiveCards()
  {
    List<Card> objectiveCards = new List<Card>();
    foreach (Card card in this.m_cards)
    {
      if (card.GetEntity() != null && card.GetEntity().IsObjective())
        objectiveCards.Add(card);
    }
    return objectiveCards;
  }

  public Entity GetPuzzleEntity()
  {
    foreach (Card card in this.m_cards)
    {
      if (card.GetEntity() != null && card.GetEntity().IsPuzzle())
        return card.GetEntity();
    }
    return (Entity) null;
  }

  public int GetSecretCount()
  {
    int secretCount = 0;
    foreach (Card card in this.m_cards)
    {
      if (card.GetEntity() != null && card.GetEntity().IsSecret())
        ++secretCount;
    }
    return secretCount;
  }

  public int GetSideQuestCount()
  {
    int sideQuestCount = 0;
    foreach (Card card in this.m_cards)
    {
      if (card.GetEntity() != null && card.GetEntity().IsSideQuest())
        ++sideQuestCount;
    }
    return sideQuestCount;
  }

  public override void OnHealingDoesDamageEntityEnteredPlay()
  {
  }

  public override void OnHealingDoesDamageEntityMousedOut()
  {
  }

  public override void OnHealingDoesDamageEntityMousedOver()
  {
  }

  public override void OnLifestealDoesDamageEntityEnteredPlay()
  {
  }

  public override void OnLifestealDoesDamageEntityMousedOut()
  {
  }

  public override void OnLifestealDoesDamageEntityMousedOver()
  {
  }

  private void UpdateLayout_Default()
  {
    this.SortQuestsToTop();
    Vector2 vector2 = new Vector2(1f, 2f);
    if (this.m_controller != null)
    {
      Card heroCard = this.m_controller.GetHeroCard();
      if ((Object) heroCard != (Object) null && (Object) heroCard.GetActor() != (Object) null)
      {
        Bounds bounds = heroCard.GetActor().GetMeshRenderer().bounds;
        vector2.x = bounds.extents.x;
        vector2.y = bounds.extents.z * 0.9f;
      }
    }
    float num1 = 0.6f * vector2.y;
    int num2 = 0;
    for (int index = 0; index < this.m_cards.Count; ++index)
    {
      Card card = this.m_cards[index];
      if (this.CanAnimateCard(card))
      {
        card.ShowCard();
        Vector3 position = this.transform.position;
        float a = (float) (index + 1 >> 1);
        int num3 = index & 1;
        float num4 = (double) a <= 2.0 ? (!Mathf.Approximately(a, 1f) ? a / 2f : 0.6f) : 1f;
        if (num3 == 0)
          position.x += vector2.x * num4;
        else
          position.x -= vector2.x * num4;
        position.z -= vector2.y * (num4 * num4);
        if ((double) a > 2.0)
          position.z -= num1 * (a - 2f);
        iTween.Stop(card.gameObject);
        int transitionStyle = (int) card.GetTransitionStyle();
        card.SetTransitionStyle(ZoneTransitionStyle.NORMAL);
        if (transitionStyle == 3)
        {
          card.EnableTransitioningZones(false);
          card.transform.position = position;
          card.transform.rotation = this.transform.rotation;
          card.transform.localScale = this.transform.localScale;
        }
        else
        {
          card.EnableTransitioningZones(true);
          ++num2;
          iTween.MoveTo(card.gameObject, position, 1f);
          iTween.RotateTo(card.gameObject, this.transform.localEulerAngles, 1f);
          iTween.ScaleTo(card.gameObject, this.transform.localScale, 1f);
        }
      }
    }
    if (num2 > 0)
      this.StartFinishLayoutTimer(1f);
    else
      this.UpdateLayoutFinished();
  }

  private void UpdateLayout_Phone()
  {
    int num = 0;
    this.SortQuestsToTop();
    bool flag = this.HaveMainQuest();
    int secretPos = 0;
    int sigilPos = 0;
    int sideQuestPos = 0;
    int objectivePos = 0;
    int numCardTypes = 0;
    this.GetZoneInfo(ref numCardTypes, ref secretPos, ref sigilPos, ref sideQuestPos, ref objectivePos);
    for (int index1 = 0; index1 < this.m_cards.Count; ++index1)
    {
      Card card = this.m_cards[index1];
      Entity entity = card.GetEntity();
      if (this.CanAnimateCard(card))
      {
        iTween.Stop(card.gameObject);
        if (entity.IsSecret() && this.GetSecretCards().IndexOf(card) == 0)
        {
          if (!card.IsShown())
          {
            card.ShowExhaustedChange(entity.IsExhausted());
            card.ShowCard();
          }
          Actor actor = card.GetActor();
          if ((Object) actor != (Object) null)
            actor.UpdateAllComponents();
        }
        if (entity.IsSideQuest() && this.GetSideQuestCards().IndexOf(card) == 0)
        {
          if (!card.IsShown())
          {
            card.ShowExhaustedChange(entity.IsExhausted());
            card.ShowCard();
          }
          Actor actor = card.GetActor();
          if ((Object) actor != (Object) null)
            actor.UpdateAllComponents();
        }
        if (entity.IsSigil() && this.GetSigilCards().IndexOf(card) == 0)
        {
          if (!card.IsShown())
          {
            card.ShowExhaustedChange(entity.IsExhausted());
            card.ShowCard();
          }
          Actor actor = card.GetActor();
          if ((Object) actor != (Object) null)
            actor.UpdateAllComponents();
        }
        if (entity.IsObjective() && this.GetObjectiveCards().IndexOf(card) == 0)
        {
          if (!card.IsShown())
          {
            card.ShowExhaustedChange(entity.IsExhausted());
            card.ShowCard();
          }
          Actor actor = card.GetActor();
          if ((Object) actor != (Object) null)
            actor.UpdateAllComponents();
        }
        Vector3 position = this.transform.position;
        if (numCardTypes == 2 && !flag)
        {
          Vector3[] vector3Array = new Vector3[2]
          {
            new Vector3(-0.5f, 0.0f, -0.1f),
            new Vector3(0.5f, 0.0f, -0.1f)
          };
          if (entity.IsSecret())
          {
            if (secretPos >= vector3Array.Length)
            {
              Log.Gameplay.PrintError("UpdateLayout_Phone() - Secret Position overflow, use position 0 instead");
              secretPos = 0;
            }
            position += vector3Array[secretPos];
          }
          else if (entity.IsSigil())
          {
            if (sigilPos >= vector3Array.Length)
            {
              Log.Gameplay.PrintError("UpdateLayout_Phone() - Sigil Position overflow, use position 0 instead");
              sigilPos = 0;
            }
            position += vector3Array[sigilPos];
          }
          else if (entity.IsSideQuest())
          {
            if (sideQuestPos >= vector3Array.Length)
            {
              Log.Gameplay.PrintError("UpdateLayout_Phone() - Sidequest Position overflow, use position 0 instead");
              sideQuestPos = 0;
            }
            position += vector3Array[sideQuestPos];
          }
          else if (entity.IsObjective())
          {
            if (objectivePos >= vector3Array.Length)
            {
              Log.Gameplay.PrintError("UpdateLayout_Phone() - Objective Position overflow, use position 0 instead");
              objectivePos = 0;
            }
            position += vector3Array[objectivePos];
          }
        }
        else if (numCardTypes > 1)
        {
          Vector3[] vector3Array = new Vector3[5]
          {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(-0.7f, 0.0f, -0.2f),
            new Vector3(0.7f, 0.0f, -0.2f),
            new Vector3(-0.9f, 0.0f, -0.85f),
            new Vector3(0.9f, 0.0f, -0.85f)
          };
          if (entity.IsQuest() || entity.IsBobQuest())
          {
            int index2 = index1;
            if (index2 >= vector3Array.Length)
            {
              Log.Gameplay.PrintError("UpdateLayout_Phone() - quest Position overflow, use position 0 instead");
              index2 = 0;
            }
            position += vector3Array[index2];
          }
          else if (entity.IsSecret())
          {
            if (secretPos >= vector3Array.Length)
            {
              Log.Gameplay.PrintError("UpdateLayout_Phone() - Secret Position overflow, use position 0 instead");
              secretPos = 0;
            }
            position += vector3Array[secretPos];
          }
          else if (entity.IsSigil())
          {
            if (sigilPos >= vector3Array.Length)
            {
              Log.Gameplay.PrintError("UpdateLayout_Phone() - Sigil Position overflow, use position 0 instead");
              sigilPos = 0;
            }
            position += vector3Array[sigilPos];
          }
          else if (entity.IsSideQuest())
          {
            if (sideQuestPos >= vector3Array.Length)
            {
              Log.Gameplay.PrintError("UpdateLayout_Phone() - Sidequest Position overflow, use position 0 instead");
              sideQuestPos = 0;
            }
            position += vector3Array[sideQuestPos];
          }
          else if (entity.IsObjective())
          {
            if (objectivePos >= vector3Array.Length)
            {
              Log.Gameplay.PrintError("UpdateLayout_Phone() - Objective Position overflow, use position 0 instead");
              objectivePos = 0;
            }
            position += vector3Array[objectivePos];
          }
        }
        int transitionStyle = (int) card.GetTransitionStyle();
        card.SetTransitionStyle(ZoneTransitionStyle.NORMAL);
        if (transitionStyle == 3)
        {
          card.EnableTransitioningZones(false);
          card.transform.position = position;
        }
        else
        {
          card.EnableTransitioningZones(true);
          ++num;
          iTween.MoveTo(card.gameObject, position, 1f);
        }
        card.transform.rotation = this.transform.rotation;
        card.transform.localScale = this.transform.localScale;
      }
    }
    if (num > 0)
      this.StartFinishLayoutTimer(1f);
    else
      this.UpdateLayoutFinished();
  }

  private void SortQuestsToTop()
  {
    int num = 0;
    for (int index = 0; index < this.m_cards.Count; ++index)
    {
      Card card = this.m_cards[index];
      Entity entity = card.GetEntity();
      if (entity.IsQuest() || entity.IsQuestline() || entity.IsBobQuest())
      {
        if (index > num)
        {
          this.m_cards.RemoveAt(index);
          this.m_cards.Insert(entity.IsBobQuest() ? 0 : num, card);
        }
        ++num;
      }
    }
  }

  private void GetZoneInfo(
    ref int numCardTypes,
    ref int secretPos,
    ref int sigilPos,
    ref int sideQuestPos,
    ref int objectivePos)
  {
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    bool flag4 = false;
    bool flag5 = false;
    numCardTypes = secretPos = sigilPos = sideQuestPos = objectivePos = 0;
    foreach (Card card in this.m_cards)
    {
      if (card.GetEntity().IsQuest() || card.GetEntity().IsQuestline())
      {
        if (!flag1)
          flag1 = true;
        ++numCardTypes;
        ++secretPos;
        ++sigilPos;
        ++sideQuestPos;
        ++objectivePos;
      }
      else if (card.GetEntity().IsSecret() && !flag2)
      {
        flag2 = true;
        ++numCardTypes;
        ++sigilPos;
        ++sideQuestPos;
        ++objectivePos;
      }
      else if (card.GetEntity().IsSigil() && !flag3)
      {
        flag3 = true;
        ++numCardTypes;
        ++sideQuestPos;
        ++objectivePos;
      }
      else if (card.GetEntity().IsSideQuest() && !flag4)
      {
        flag4 = true;
        ++objectivePos;
        ++numCardTypes;
      }
      else if (card.GetEntity().IsObjective() && !flag5)
      {
        flag5 = true;
        ++numCardTypes;
      }
      else
        Debug.LogWarningFormat("GetZoneInfo() - Unknown secret zone card type");
    }
  }

  private bool HaveMainQuest()
  {
    foreach (Card card in this.m_cards)
    {
      if (card.GetEntity().IsQuest() || card.GetEntity().IsQuestline() || card.GetEntity().IsBobQuest())
        return true;
    }
    return false;
  }

  private bool CanAnimateCard(Card card) => !card.IsDoNotSort();

  private void OnGameOver(TAG_PLAYSTATE playState, object userData)
  {
    Player controller = this.GetController();
    if (controller == null || controller.GetTag<TAG_PLAYSTATE>(GAME_TAG.PLAYSTATE) == TAG_PLAYSTATE.WON)
      return;
    for (int index = 0; index < this.m_cards.Count; ++index)
    {
      Card card = this.m_cards[index];
      if (!((Object) card == (Object) null) && this.CanAnimateCard(card))
        card.HideCard();
    }
  }
}
