using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class TGTArcheryTarget : MonoBehaviour
{
  public int m_BullseyePercent = 5;
  public int m_TargetDummyPercent = 1;
  public float m_MaxRandomOffset = 0.3f;
  public int m_Levelup = 50;
  public GameObject m_Collider01;
  public Animation m_Animation;
  public GameObject m_TargetRoot;
  public GameObject m_Arrow;
  public GameObject m_SplitArrow;
  public int m_MaxArrows;
  public List<TGTArrow> m_TargetDummyArrows;
  public GameObject m_ArrowBone01;
  public GameObject m_ArrowBone02;
  public BoxCollider m_BoxCollider01;
  public BoxCollider m_BoxCollider02;
  public BoxCollider m_BoxColliderBullseye;
  public Transform m_CenterBone;
  public Transform m_OuterRadiusBone;
  public Transform m_BullseyeCenterBone;
  public Transform m_BullseyeRadiusBone;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_HitTargetSoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_HitBullseyeSoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_HitTargetDummySoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_SplitArrowSoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_RemoveArrowSoundPrefab;
  private GameObject[] m_arrows;
  private int m_lastArrow = 1;
  private float m_targetRadius;
  private float m_bullseyeRadius;
  private int m_ArrowCount;
  private List<int> m_AvailableTargetDummyArrows;
  private GameObject m_lastBullseyeArrow;
  private bool m_lastArrowWasBullseye;
  private bool m_clearingArrows;
  private float m_lastClickTime;

  private void Start()
  {
    this.m_arrows = new GameObject[this.m_MaxArrows];
    this.m_arrows[0] = Object.Instantiate<GameObject>(this.m_Arrow, this.m_ArrowBone01.transform.position, this.m_ArrowBone01.transform.rotation, this.m_TargetRoot.transform);
    this.m_arrows[1] = Object.Instantiate<GameObject>(this.m_Arrow, this.m_ArrowBone02.transform.position, this.m_ArrowBone02.transform.rotation, this.m_TargetRoot.transform);
    this.m_lastArrow = 2;
    for (int lastArrow = this.m_lastArrow; lastArrow < this.m_MaxArrows; ++lastArrow)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.m_Arrow, new Vector3(-15f, -15f, -15f), Quaternion.identity, this.m_TargetRoot.transform);
      gameObject.SetActive(false);
      this.m_arrows[lastArrow] = gameObject;
    }
    this.m_targetRadius = Vector3.Distance(this.m_CenterBone.position, this.m_OuterRadiusBone.position);
    this.m_bullseyeRadius = Vector3.Distance(this.m_BullseyeCenterBone.position, this.m_BullseyeRadiusBone.position);
    this.m_AvailableTargetDummyArrows = new List<int>();
    for (int index = 0; index < this.m_TargetDummyArrows.Count; ++index)
      this.m_AvailableTargetDummyArrows.Add(index);
    this.m_SplitArrow.SetActive(false);
  }

  private void Update() => this.HandleHits();

  private void HandleHits()
  {
    if (!InputCollection.GetMouseButtonDown(0) || !this.IsOver(this.m_Collider01))
      return;
    this.HandleFireArrow();
  }

  private void HandleFireArrow()
  {
    if (this.m_clearingArrows)
      return;
    ++this.m_ArrowCount;
    if (this.m_ArrowCount > this.m_Levelup)
    {
      this.m_ArrowCount = 0;
      this.m_MaxRandomOffset *= 0.95f;
      this.m_BullseyePercent += 4;
    }
    if (Random.Range(0, 100) < this.m_TargetDummyPercent && this.m_AvailableTargetDummyArrows.Count > 0)
    {
      this.HitTargetDummy();
    }
    else
    {
      Ray ray = Camera.main.ScreenPointToRay(InputCollection.GetMousePosition());
      bool bullseye = false;
      RaycastHit hitInfo;
      if (this.m_BoxColliderBullseye.Raycast(ray, out hitInfo, 100f))
        bullseye = true;
      if (!this.m_BoxCollider02.Raycast(ray, out hitInfo, 100f))
        return;
      ++this.m_lastArrow;
      if (this.m_lastArrow >= this.m_MaxArrows)
      {
        this.m_lastArrow = 0;
        this.StartCoroutine(this.ClearArrows());
      }
      else
      {
        GameObject arrow = this.m_arrows[this.m_lastArrow];
        this.FireArrow(arrow.GetComponent<TGTArrow>(), hitInfo.point, bullseye);
        arrow.transform.eulerAngles = hitInfo.normal;
        this.ImpactTarget();
      }
    }
  }

  private IEnumerator ClearArrows()
  {
    this.m_clearingArrows = true;
    GameObject[] gameObjectArray = this.m_arrows;
    for (int index = 0; index < gameObjectArray.Length; ++index)
    {
      GameObject gameObject = gameObjectArray[index];
      if (gameObject.activeSelf)
      {
        gameObject.SetActive(false);
        this.m_Animation.Stop();
        this.m_Animation.Play("TGT_GrandStand_ArcheryTarget_Remove");
        this.PlaySound(this.m_RemoveArrowSoundPrefab);
        yield return (object) new WaitForSeconds(0.2f);
      }
    }
    gameObjectArray = (GameObject[]) null;
    yield return (object) new WaitForSeconds(0.2f);
    if (this.m_SplitArrow.activeSelf)
    {
      this.m_SplitArrow.SetActive(false);
      this.m_Animation.Stop();
      this.m_Animation.Play("TGT_GrandStand_ArcheryTarget_Remove");
      this.PlaySound(this.m_RemoveArrowSoundPrefab);
    }
    this.m_lastArrowWasBullseye = false;
    this.m_lastBullseyeArrow = (GameObject) null;
    this.m_clearingArrows = false;
  }

  private void FireArrow(TGTArrow arrow, Vector3 hitPosition, bool bullseye)
  {
    arrow.transform.position = hitPosition;
    bool flag = false;
    if ((double) Time.timeSinceLevelLoad > (double) this.m_lastClickTime + 0.800000011920929)
      flag = true;
    this.m_lastClickTime = Time.timeSinceLevelLoad;
    int num1 = this.m_BullseyePercent;
    if (flag)
      num1 *= 2;
    if (num1 > 80)
      num1 = 80;
    if (bullseye && Random.Range(0, 100) < num1)
    {
      int num2 = 2;
      if (flag)
        num2 = 8;
      if (((!this.m_lastArrowWasBullseye ? 0 : (!this.m_SplitArrow.activeSelf ? 1 : 0)) & (bullseye ? 1 : 0)) != 0 && Random.Range(0, 100) < num2)
      {
        this.m_SplitArrow.transform.position = this.m_lastBullseyeArrow.transform.position;
        this.m_SplitArrow.transform.rotation = this.m_lastBullseyeArrow.transform.rotation;
        TGTArrow component1 = this.m_SplitArrow.GetComponent<TGTArrow>();
        TGTArrow component2 = this.m_lastBullseyeArrow.GetComponent<TGTArrow>();
        this.m_SplitArrow.SetActive(true);
        component1.FireArrow(false);
        component1.Bullseye();
        this.PlaySound(this.m_SplitArrowSoundPrefab);
        component1.m_ArrowRoot.transform.position = component2.m_ArrowRoot.transform.position;
        component1.m_ArrowRoot.transform.rotation = component2.m_ArrowRoot.transform.rotation;
        this.m_lastBullseyeArrow.SetActive(false);
        this.m_lastArrowWasBullseye = false;
        this.m_lastBullseyeArrow = (GameObject) null;
      }
      else
      {
        arrow.gameObject.SetActive(true);
        arrow.Bullseye();
        this.PlaySound(this.m_HitBullseyeSoundPrefab);
        arrow.m_ArrowRoot.transform.localPosition = Vector3.zero;
        this.m_lastBullseyeArrow = arrow.gameObject;
        this.m_lastArrowWasBullseye = true;
      }
    }
    else
    {
      this.m_lastArrowWasBullseye = false;
      this.m_lastBullseyeArrow = (GameObject) null;
      arrow.gameObject.SetActive(true);
      if (bullseye)
      {
        Vector2 vector2 = Random.insideUnitCircle.normalized * this.m_bullseyeRadius * 2f;
        arrow.m_ArrowRoot.transform.localPosition = new Vector3(vector2.x, vector2.y, 0.0f);
        arrow.FireArrow(true);
        this.PlaySound(this.m_HitTargetSoundPrefab);
      }
      else
      {
        Vector2 vector2 = Random.insideUnitCircle * Random.Range(0.0f, this.m_MaxRandomOffset);
        Transform transform = arrow.m_ArrowRoot.transform;
        transform.localPosition = new Vector3(vector2.x, vector2.y, 0.0f);
        if ((double) Vector3.Distance(transform.position, this.m_CenterBone.position) > (double) this.m_targetRadius)
          transform.localPosition = Vector3.zero;
        if ((double) Vector3.Distance(transform.position, this.m_BullseyeCenterBone.position) < (double) this.m_bullseyeRadius)
          transform.localPosition = Vector3.zero;
        arrow.FireArrow(true);
        this.PlaySound(this.m_HitTargetSoundPrefab);
      }
    }
  }

  private void HitTargetDummy()
  {
    int index = 0;
    if (this.m_AvailableTargetDummyArrows.Count > 1)
      index = Random.Range(0, this.m_AvailableTargetDummyArrows.Count);
    TGTArrow targetDummyArrow = this.m_TargetDummyArrows[this.m_AvailableTargetDummyArrows[index]];
    targetDummyArrow.gameObject.SetActive(true);
    targetDummyArrow.FireArrow(false);
    TGTTargetDummy.Get().ArrowHit();
    this.PlaySound(this.m_HitTargetDummySoundPrefab);
    if (this.m_AvailableTargetDummyArrows.Count > 1)
      this.m_AvailableTargetDummyArrows.RemoveAt(index);
    else
      this.m_AvailableTargetDummyArrows.Clear();
  }

  private void ImpactTarget()
  {
    this.m_Animation.Stop();
    this.m_Animation.Play("TGT_GrandStand_ArcheryTarget_Hit");
  }

  private void PlaySound(string soundPrefab)
  {
    if (string.IsNullOrEmpty(soundPrefab))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) soundPrefab, this.gameObject);
  }

  private bool IsOver(GameObject go) => (bool) (Object) go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);
}
