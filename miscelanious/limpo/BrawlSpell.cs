using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrawlSpell : Spell
{
  public float m_MinJumpHeight = 1.5f;
  public float m_MaxJumpHeight = 2.5f;
  public float m_MinJumpInDelay = 0.1f;
  public float m_MaxJumpInDelay = 0.2f;
  public float m_JumpInDuration = 1.5f;
  public iTween.EaseType m_JumpInEaseType = iTween.EaseType.linear;
  public float m_HoldTime = 0.1f;
  public float m_MinJumpOutDelay = 0.1f;
  public float m_MaxJumpOutDelay = 0.2f;
  public float m_JumpOutDuration = 1.5f;
  public iTween.EaseType m_JumpOutEaseType = iTween.EaseType.easeOutBounce;
  public float m_SurvivorHoldDuration = 0.5f;
  public List<GameObject> m_LeftJumpOutBones;
  public List<GameObject> m_RightJumpOutBones;
  public AudioSource m_JumpInSoundPrefab;
  public float m_JumpInSoundDelay;
  public AudioSource m_JumpOutSoundPrefab;
  public float m_JumpOutSoundDelay;
  public AudioSource m_LandSoundPrefab;
  public float m_LandSoundDelay;
  private int m_jumpsPending;
  private Card m_survivorCard;

  protected override void OnAction(SpellStateType prevStateType)
  {
    if (this.m_targets.Count > 0)
    {
      this.m_survivorCard = this.FindSurvivor();
      this.StartJumpIns();
    }
    else
    {
      this.OnSpellFinished();
      this.OnStateFinished();
    }
  }

  private Card FindSurvivor()
  {
    foreach (GameObject target in this.m_targets)
    {
      bool flag = true;
      Card component = target.GetComponent<Card>();
      foreach (PowerTask task in this.m_taskList.GetTaskList())
      {
        Network.PowerHistory power = task.GetPower();
        if (power.Type == Network.PowerType.TAG_CHANGE)
        {
          Network.HistTagChange histTagChange = power as Network.HistTagChange;
          if (histTagChange.Tag == 360 && histTagChange.Value == 1)
          {
            Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
            if (entity == null)
              Debug.LogWarning((object) string.Format("{0}.FindSurvivor() - WARNING trying to get entity with id {1} but there is no entity with that id", (object) this, (object) histTagChange.Entity));
            else if ((Object) component == (Object) entity.GetCard())
            {
              flag = false;
              break;
            }
          }
        }
      }
      if (flag)
        return component;
    }
    return (Card) null;
  }

  private void StartJumpIns()
  {
    this.m_jumpsPending = this.m_targets.Count;
    List<Card> cardList = new List<Card>(this.m_jumpsPending);
    foreach (GameObject target in this.m_targets)
    {
      Card component = target.GetComponent<Card>();
      cardList.Add(component);
    }
    float startSec = 0.0f;
    while (cardList.Count > 0)
    {
      int index = Random.Range(0, cardList.Count);
      Card card = cardList[index];
      cardList.RemoveAt(index);
      this.StartJumpIn(card, ref startSec);
    }
  }

  private void StartJumpIn(Card card, ref float startSec)
  {
    float num = Random.Range(this.m_MinJumpInDelay, this.m_MaxJumpInDelay);
    this.StartCoroutine(this.JumpIn(card, startSec + num));
    startSec += num;
  }

  private IEnumerator JumpIn(Card card, float delaySec)
  {
    BrawlSpell brawlSpell = this;
    yield return (object) new WaitForSeconds(delaySec);
    Vector3[] vector3Array = new Vector3[3];
    vector3Array[0] = card.transform.position;
    vector3Array[2] = brawlSpell.transform.position;
    vector3Array[1] = 0.5f * (vector3Array[0] + vector3Array[2]);
    float num = Random.Range(brawlSpell.m_MinJumpHeight, brawlSpell.m_MaxJumpHeight);
    vector3Array[1].y += num;
    object[] objArray = new object[14]
    {
      (object) "path",
      (object) vector3Array,
      (object) "orienttopath",
      (object) true,
      (object) "time",
      (object) brawlSpell.m_JumpInDuration,
      (object) "easetype",
      (object) brawlSpell.m_JumpInEaseType,
      (object) "oncomplete",
      (object) "OnJumpInComplete",
      (object) "oncompletetarget",
      (object) brawlSpell.gameObject,
      (object) "oncompleteparams",
      (object) card
    };
    iTween.MoveTo(card.gameObject, iTween.Hash(objArray));
    if ((Object) brawlSpell.m_JumpInSoundPrefab != (Object) null)
      brawlSpell.StartCoroutine(brawlSpell.LoadAndPlaySound(brawlSpell.m_JumpInSoundPrefab, brawlSpell.m_JumpInSoundDelay));
  }

  private void OnJumpInComplete(Card targetCard)
  {
    targetCard.HideCard();
    --this.m_jumpsPending;
    if (this.m_jumpsPending > 0)
      return;
    this.StartCoroutine(this.Hold());
  }

  private IEnumerator Hold()
  {
    yield return (object) new WaitForSeconds(this.m_HoldTime);
    this.StartJumpOuts();
  }

  private void StartJumpOuts()
  {
    this.m_jumpsPending = this.m_targets.Count - 1;
    List<int> usedBoneIndexes1 = new List<int>();
    List<int> usedBoneIndexes2 = new List<int>();
    float num1 = 0.0f;
    bool flag = true;
    for (int index = 0; index < this.m_targets.Count; ++index)
    {
      Card component = this.m_targets[index].GetComponent<Card>();
      if (!((Object) component == (Object) this.m_survivorCard))
      {
        GameObject freeBone;
        if (flag)
        {
          freeBone = this.GetFreeBone(this.m_LeftJumpOutBones, usedBoneIndexes1);
          if ((Object) freeBone == (Object) null)
          {
            usedBoneIndexes1.Clear();
            freeBone = this.GetFreeBone(this.m_LeftJumpOutBones, usedBoneIndexes1);
          }
        }
        else
        {
          freeBone = this.GetFreeBone(this.m_RightJumpOutBones, usedBoneIndexes2);
          if ((Object) freeBone == (Object) null)
          {
            usedBoneIndexes2.Clear();
            freeBone = this.GetFreeBone(this.m_RightJumpOutBones, usedBoneIndexes2);
          }
        }
        float num2 = Random.Range(this.m_MinJumpOutDelay, this.m_MaxJumpOutDelay);
        this.StartCoroutine(this.JumpOut(component, num1 + num2, freeBone.transform.position));
        num1 += num2;
        flag = !flag;
      }
    }
  }

  private GameObject GetFreeBone(List<GameObject> boneList, List<int> usedBoneIndexes)
  {
    List<int> intList = new List<int>();
    for (int index = 0; index < boneList.Count; ++index)
    {
      if (!usedBoneIndexes.Contains(index))
        intList.Add(index);
    }
    if (intList.Count == 0)
      return (GameObject) null;
    int index1 = Random.Range(0, intList.Count - 1);
    int index2 = intList[index1];
    usedBoneIndexes.Add(index2);
    return boneList[index2];
  }

  private IEnumerator JumpOut(Card card, float delaySec, Vector3 destPos)
  {
    BrawlSpell brawlSpell = this;
    yield return (object) new WaitForSeconds(delaySec);
    card.transform.rotation = Quaternion.identity;
    card.ShowCard();
    Vector3[] vector3Array = new Vector3[3];
    vector3Array[0] = card.transform.position;
    vector3Array[2] = destPos;
    vector3Array[1] = 0.5f * (vector3Array[0] + vector3Array[2]);
    float num = Random.Range(brawlSpell.m_MinJumpHeight, brawlSpell.m_MaxJumpHeight);
    vector3Array[1].y += num;
    object[] objArray = new object[12]
    {
      (object) "path",
      (object) vector3Array,
      (object) "time",
      (object) brawlSpell.m_JumpOutDuration,
      (object) "easetype",
      (object) brawlSpell.m_JumpOutEaseType,
      (object) "oncomplete",
      (object) "OnJumpOutComplete",
      (object) "oncompletetarget",
      (object) brawlSpell.gameObject,
      (object) "oncompleteparams",
      (object) card
    };
    iTween.MoveTo(card.gameObject, iTween.Hash(objArray));
    if ((Object) brawlSpell.m_JumpOutSoundPrefab != (Object) null)
      brawlSpell.StartCoroutine(brawlSpell.LoadAndPlaySound(brawlSpell.m_JumpOutSoundPrefab, brawlSpell.m_JumpOutSoundDelay));
    if ((Object) brawlSpell.m_LandSoundPrefab != (Object) null)
      brawlSpell.StartCoroutine(brawlSpell.LoadAndPlaySound(brawlSpell.m_LandSoundPrefab, brawlSpell.m_LandSoundDelay));
  }

  private void OnJumpOutComplete(Card targetCard)
  {
    --this.m_jumpsPending;
    if (this.m_jumpsPending > 0)
      return;
    this.ActivateState(SpellStateType.DEATH);
    this.StartCoroutine(this.SurvivorHold());
  }

  private IEnumerator SurvivorHold()
  {
    BrawlSpell brawlSpell = this;
    brawlSpell.m_survivorCard.transform.rotation = Quaternion.identity;
    brawlSpell.m_survivorCard.ShowCard();
    yield return (object) new WaitForSeconds(brawlSpell.m_SurvivorHoldDuration);
    if (brawlSpell.IsSurvivorAlone())
      brawlSpell.m_survivorCard.GetZone().UpdateLayout();
    brawlSpell.OnSpellFinished();
    brawlSpell.OnStateFinished();
  }

  private bool IsSurvivorAlone()
  {
    Zone zone = this.m_survivorCard.GetZone();
    foreach (GameObject target in this.m_targets)
    {
      Card component = target.GetComponent<Card>();
      if (!((Object) component == (Object) this.m_survivorCard) && (Object) component.GetZone() == (Object) zone)
        return false;
    }
    return true;
  }

  private IEnumerator LoadAndPlaySound(AudioSource prefab, float delaySec)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BrawlSpell brawlSpell = this;
    AudioSource source;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      SoundManager.Get().PlayPreloaded(source);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    source = Object.Instantiate<AudioSource>(prefab);
    source.transform.parent = brawlSpell.transform;
    TransformUtil.Identity((Component) source);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(delaySec);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
