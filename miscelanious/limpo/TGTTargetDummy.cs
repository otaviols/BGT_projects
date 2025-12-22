using System.Collections;
using UnityEngine;

[CustomEditClass]
public class TGTTargetDummy : MonoBehaviour
{
  private const int SPIN_PERCENT = 5;
  public GameObject m_Body;
  public GameObject m_Shield;
  public GameObject m_Sword;
  public Animation m_Animation;
  public GameObject m_BodyMesh;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_HitBodySoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_HitShieldSoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_HitSwordSoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_HitSpinSoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_SqueakSoundPrefab;
  private static TGTTargetDummy s_instance;
  private float m_squeakSoundVelocity;
  private float m_lastSqueakSoundVol;
  private Quaternion m_lastFrameSqueakAngle;
  private AudioSource m_squeakSound;

  private void Awake() => TGTTargetDummy.s_instance = this;

  private void Start()
  {
    this.StartCoroutine(this.RegisterBoardEventLargeShake());
    GameObject gameObject = SoundLoader.LoadSound((AssetReference) this.m_SqueakSoundPrefab);
    if (!((Object) gameObject != (Object) null))
      return;
    gameObject.transform.position = this.m_Body.transform.position;
    this.m_squeakSound = gameObject.GetComponent<AudioSource>();
  }

  private void Update() => this.HandleHits();

  public static TGTTargetDummy Get() => TGTTargetDummy.s_instance;

  public void ArrowHit() => this.m_Animation.CrossFade("TGT_GrandStand_Dummy_ArrowHit", 0.1f);

  private void BodyHit()
  {
    this.PlaySqueakSound();
    if (!string.IsNullOrEmpty(this.m_HitBodySoundPrefab))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_HitBodySoundPrefab, this.m_Body);
    this.m_Animation.CrossFade("TGT_GrandStand_Dummy_Hit", 0.1f);
  }

  private void ShieldHit()
  {
    this.PlaySqueakSound();
    if (Random.Range(0, 100) < 5)
    {
      this.Spin(false);
    }
    else
    {
      if (!string.IsNullOrEmpty(this.m_HitShieldSoundPrefab))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_HitShieldSoundPrefab, this.m_Body);
      this.m_Animation.CrossFade("TGT_GrandStand_Dummy_ShieldHit", 0.1f);
    }
  }

  private void SwordHit()
  {
    this.PlaySqueakSound();
    if (Random.Range(0, 100) < 5)
    {
      this.Spin(true);
    }
    else
    {
      if (!string.IsNullOrEmpty(this.m_HitSwordSoundPrefab))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_HitSwordSoundPrefab, this.m_Body);
      this.m_Animation.CrossFade("TGT_GrandStand_Dummy_SwordHit", 0.1f);
    }
  }

  private IEnumerator RegisterBoardEventLargeShake()
  {
    TGTTargetDummy tgtTargetDummy = this;
    while ((Object) BoardEvents.Get() == (Object) null)
      yield return (object) null;
    yield return (object) new WaitForSeconds(2f);
    BoardEvents.Get().RegisterLargeShakeEvent(new BoardEvents.LargeShakeEventDelegate(tgtTargetDummy.BodyHit));
  }

  private void HandleHits()
  {
    if (!InputCollection.GetMouseButtonDown(0))
      return;
    if (this.IsOver(this.m_Body))
      this.BodyHit();
    if (this.IsOver(this.m_Shield))
      this.ShieldHit();
    if (!this.IsOver(this.m_Sword))
      return;
    this.SwordHit();
  }

  private void Spin(bool reverse)
  {
    float num = 1080f;
    if (reverse)
      num = -1080f;
    if (!string.IsNullOrEmpty(this.m_HitSpinSoundPrefab))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_HitSpinSoundPrefab, this.m_Body);
    this.m_BodyMesh.transform.localEulerAngles = Vector3.zero;
    iTween.RotateTo(this.m_BodyMesh, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, this.m_BodyMesh.transform.localEulerAngles.y + num, 0.0f), (object) "isLocal", (object) true, (object) "time", (object) 3f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
  }

  private void PlaySqueakSound()
  {
    this.StopCoroutine(this.SqueakSound());
    this.m_lastSqueakSoundVol = 0.0f;
    this.StartCoroutine(this.SqueakSound());
  }

  private IEnumerator SqueakSound()
  {
    if (!((Object) this.m_squeakSound == (Object) null))
    {
      if ((Object) this.m_squeakSound != (Object) null && this.m_squeakSound.isPlaying)
        SoundManager.Get().Stop(this.m_squeakSound);
      SoundManager.Get().PlayPreloaded(this.m_squeakSound, 0.0f);
      while ((Object) this.m_squeakSound != (Object) null && this.m_squeakSound.isPlaying)
      {
        float current = Mathf.Clamp01(Quaternion.Angle(this.m_Body.transform.rotation, this.m_lastFrameSqueakAngle) * 0.1f);
        this.m_lastFrameSqueakAngle = this.m_Body.transform.rotation;
        float num = Mathf.SmoothDamp(current, this.m_lastSqueakSoundVol, ref this.m_squeakSoundVelocity, 0.5f);
        this.m_lastSqueakSoundVol = num;
        SoundManager.Get().SetVolume(this.m_squeakSound, Mathf.Clamp01(num));
        yield return (object) null;
      }
    }
  }

  private bool IsOver(GameObject go) => (bool) (Object) go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);
}
