using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class TGTGrandStand : MonoBehaviour
{
  private const string ANIMATION_IDLE = "Idle";
  private const string ANIMATION_CLICKED = "Clicked";
  private readonly string[] ANIMATION_CHEER = new string[3]
  {
    "Cheer01",
    "Cheer02",
    "Cheer03"
  };
  private readonly string[] ANIMATION_OHNO = new string[2]
  {
    "OhNo01",
    "OhNo02"
  };
  private const string ANIMATION_SCORE_CARD = "ScoreCard";
  private const float MIN_RANDOM_TIME_FACTOR = 0.05f;
  private const float MAX_RANDOM_TIME_FACTOR = 0.2f;
  private const float CHEER_ANIMATION_PLAY_TIME = 4f;
  private const float OHNO_ANIMATION_PLAY_TIME = 3.5f;
  private const float FRIENDLY_HERO_DAMAGE_WEIGHT_TRGGER = 7f;
  private const float OPPONENT_HERO_DAMAGE_WEIGHT_TRGGER = 10f;
  private const float FRIENDLY_LEGENDARY_SPAWN_MIN_COST_TRGGER = 6f;
  private const float OPPONENT_LEGENDARY_SPAWN_MIN_COST_TRGGER = 9f;
  private const float FRIENDLY_LEGENDARY_DEATH_MIN_COST_TRGGER = 6f;
  private const float OPPONENT_LEGENDARY_DEATH_MIN_COST_TRGGER = 9f;
  private const float FRIENDLY_MINION_DAMAGE_WEIGHT = 15f;
  private const float OPPONENT_MINION_DAMAGE_WEIGHT = 15f;
  private const float FRIENDLY_MINION_DEATH_WEIGHT = 15f;
  private const float OPPONENT_MINION_DEATH_WEIGHT = 15f;
  private const float FRIENDLY_MINION_SPAWN_WEIGHT = 10f;
  private const float OPPONENT_MINION_SPAWN_WEIGHT = 10f;
  private const float OPPONENT_HERO_DAMAGE_SCORE_CARD_TRIGGER = 15f;
  private const float OPPONENT_HERO_DAMAGE_SCORE_CARD_10S_TRIGGER = 20f;
  public GameObject m_HumanRoot;
  public GameObject m_OrcRoot;
  public GameObject m_KnightRoot;
  public Animator m_HumanAnimator;
  public Animator m_OrcAnimator;
  public Animator m_KnightAnimator;
  public GameObject m_HumanScoreCard;
  public GameObject m_OrcScoreCard;
  public GameObject m_KnightScoreCard;
  public UberText m_HumanScoreUberText;
  public UberText m_OrcScoreUberText;
  public UberText m_KnightScoreUberText;
  [CustomEditField(Sections = "Human Sounds", T = EditType.SOUND_PREFAB)]
  public string m_ClickHumanSound;
  [CustomEditField(Sections = "Human Sounds", T = EditType.SOUND_PREFAB)]
  public List<string> m_CheerHumanSounds;
  [CustomEditField(Sections = "Human Sounds", T = EditType.SOUND_PREFAB)]
  public List<string> m_OhNoHumanSounds;
  [CustomEditField(Sections = "Orc Sounds", T = EditType.SOUND_PREFAB)]
  public string m_ClickOrcSound;
  [CustomEditField(Sections = "Orc Sounds", T = EditType.SOUND_PREFAB)]
  public List<string> m_CheerOrcSounds;
  [CustomEditField(Sections = "Orc Sounds", T = EditType.SOUND_PREFAB)]
  public List<string> m_OhNoOrcSounds;
  [CustomEditField(Sections = "Knight Sounds", T = EditType.SOUND_PREFAB)]
  public string m_ClickKnightSound;
  [CustomEditField(Sections = "Knight Sounds", T = EditType.SOUND_PREFAB)]
  public List<string> m_CheerKnightSounds;
  [CustomEditField(Sections = "Knight Sounds", T = EditType.SOUND_PREFAB)]
  public List<string> m_OhNoKnightSounds;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_ScoreCardSound;
  private BoardEvents m_boardEvents;
  private bool m_isAnimating;
  private static TGTGrandStand s_instance;

  private void Awake() => TGTGrandStand.s_instance = this;

  private void Start() => this.StartCoroutine(this.RegisterBoardEvents());

  private void Update() => this.HandleClicks();

  private void OnDestroy() => TGTGrandStand.s_instance = (TGTGrandStand) null;

  public static TGTGrandStand Get() => TGTGrandStand.s_instance;

  private void HandleClicks()
  {
    if (!InputCollection.GetMouseButtonDown(0))
      return;
    if (this.IsOver(this.m_HumanRoot))
      this.HumanClick();
    if (this.IsOver(this.m_OrcRoot))
      this.OrcClick();
    if (!this.IsOver(this.m_KnightRoot))
      return;
    this.KnightClick();
  }

  private void HumanClick()
  {
    this.PlayClickedAnimation(this.m_HumanAnimator, "Clicked");
    this.PlaySound(this.m_HumanRoot, this.m_ClickHumanSound);
  }

  private void OrcClick()
  {
    this.PlayClickedAnimation(this.m_OrcAnimator, "Clicked");
    this.PlaySound(this.m_OrcRoot, this.m_ClickOrcSound);
  }

  private void KnightClick()
  {
    this.PlayClickedAnimation(this.m_KnightAnimator, "Clicked");
    this.PlaySound(this.m_KnightRoot, this.m_ClickKnightSound);
  }

  private IEnumerator TestAnimations()
  {
    yield return (object) new WaitForSeconds(4f);
    this.PlayCheerAnimation();
    yield return (object) new WaitForSeconds(8f);
    this.PlayCheerAnimation();
    yield return (object) new WaitForSeconds(9f);
    this.PlayCheerAnimation();
    yield return (object) new WaitForSeconds(8f);
    this.PlayOhNoAnimation();
    yield return (object) new WaitForSeconds(8f);
    this.PlayOhNoAnimation();
  }

  public void PlayCheerAnimation()
  {
    int index1 = Random.Range(0, this.ANIMATION_CHEER.Length);
    this.PlayAnimation(this.m_HumanAnimator, this.ANIMATION_CHEER[index1], 4f);
    this.PlaySoundFromList(this.m_CheerHumanSounds, index1);
    int index2 = Random.Range(0, this.ANIMATION_CHEER.Length);
    this.PlayAnimation(this.m_OrcAnimator, this.ANIMATION_CHEER[index2], 4f);
    this.PlaySoundFromList(this.m_CheerOrcSounds, index2);
    int index3 = Random.Range(0, this.ANIMATION_CHEER.Length);
    this.PlayAnimation(this.m_KnightAnimator, this.ANIMATION_CHEER[index3], 4f);
    this.PlaySoundFromList(this.m_CheerKnightSounds, index3);
  }

  public void PlayOhNoAnimation()
  {
    int index1 = Random.Range(0, this.ANIMATION_OHNO.Length);
    this.PlayAnimation(this.m_HumanAnimator, this.ANIMATION_OHNO[index1], 3.5f);
    this.PlaySoundFromList(this.m_OhNoHumanSounds, index1);
    int index2 = Random.Range(0, this.ANIMATION_OHNO.Length);
    this.PlayAnimation(this.m_OrcAnimator, this.ANIMATION_OHNO[index2], 3.5f);
    this.PlaySoundFromList(this.m_OhNoOrcSounds, index2);
    int index3 = Random.Range(0, this.ANIMATION_OHNO.Length);
    this.PlayAnimation(this.m_KnightAnimator, this.ANIMATION_OHNO[index3], 3.5f);
    this.PlaySoundFromList(this.m_OhNoKnightSounds, index3);
  }

  public void PlayScoreCard(string humanScore, string orcScore, string knightScore)
  {
    this.m_HumanScoreUberText.Text = humanScore;
    this.m_OrcScoreUberText.Text = orcScore;
    this.m_KnightScoreUberText.Text = knightScore;
    this.m_HumanAnimator.SetTrigger("ScoreCard");
    this.m_OrcAnimator.SetTrigger("ScoreCard");
    this.m_KnightAnimator.SetTrigger("ScoreCard");
    this.PlaySound(this.m_OrcRoot, this.m_ScoreCardSound);
  }

  private void PlaySoundFromList(List<string> soundList, int index)
  {
    if (soundList == null || soundList.Count == 0)
      return;
    if (index > soundList.Count)
      index = 0;
    this.PlaySound(this.m_OrcRoot, soundList[index]);
  }

  private void PlaySound(GameObject rootObject, string soundPath)
  {
    if (string.IsNullOrEmpty(soundPath))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) soundPath, rootObject);
  }

  private void PlayClickedAnimation(Animator animator, string animName)
  {
    this.m_isAnimating = true;
    animator.SetTrigger(animName);
    this.StartCoroutine(this.ReturnToIdleAnimation(animator, 0.0f));
  }

  private void PlayAnimation(Animator animator, string animName, float time)
  {
    this.m_isAnimating = true;
    this.m_HumanScoreCard.SetActive(false);
    this.m_OrcScoreCard.SetActive(false);
    this.m_KnightScoreCard.SetActive(false);
    this.StartCoroutine(this.PlayAnimationRandomStart(animator, animName, time));
  }

  private IEnumerator PlayAnimationRandomStart(
    Animator animator,
    string animName,
    float time)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TGTGrandStand tgtGrandStand = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      animator.SetTrigger(animName);
      tgtGrandStand.StartCoroutine(tgtGrandStand.ReturnToIdleAnimation(animator, time));
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(Random.Range(0.05f, 0.2f));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private IEnumerator ReturnToIdleAnimation(Animator animator, float time)
  {
    yield return (object) new WaitForSeconds(time);
    this.m_isAnimating = false;
    animator.SetTrigger("Idle");
  }

  private void Shake()
  {
    if (this.m_isAnimating)
      return;
    this.StartCoroutine(this.PlayAnimationRandomStart(this.m_HumanAnimator, "Clicked", 0.0f));
    this.StartCoroutine(this.PlayAnimationRandomStart(this.m_OrcAnimator, "Clicked", 0.0f));
    this.StartCoroutine(this.PlayAnimationRandomStart(this.m_KnightAnimator, "Clicked", 0.0f));
  }

  private bool IsOver(GameObject go) => (bool) (Object) go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);

  private IEnumerator RegisterBoardEvents()
  {
    TGTGrandStand tgtGrandStand = this;
    while ((Object) BoardEvents.Get() == (Object) null)
      yield return (object) null;
    tgtGrandStand.m_boardEvents = BoardEvents.Get();
    tgtGrandStand.m_boardEvents.RegisterFriendlyHeroDamageEvent(new BoardEvents.EventDelegate(tgtGrandStand.FriendlyHeroDamage), 7f);
    tgtGrandStand.m_boardEvents.RegisterOpponentHeroDamageEvent(new BoardEvents.EventDelegate(tgtGrandStand.OpponentHeroDamage), 10f);
    tgtGrandStand.m_boardEvents.RegisterFriendlyLegendaryMinionSpawnEvent(new BoardEvents.EventDelegate(tgtGrandStand.FriendlyLegendarySpawn), 6f);
    tgtGrandStand.m_boardEvents.RegisterOppenentLegendaryMinionSpawnEvent(new BoardEvents.EventDelegate(tgtGrandStand.OpponentLegendarySpawn), 9f);
    tgtGrandStand.m_boardEvents.RegisterFriendlyLegendaryMinionDeathEvent(new BoardEvents.EventDelegate(tgtGrandStand.FriendlyLegendaryDeath), 6f);
    tgtGrandStand.m_boardEvents.RegisterOppenentLegendaryMinionDeathEvent(new BoardEvents.EventDelegate(tgtGrandStand.OpponentLegendaryDeath), 9f);
    tgtGrandStand.m_boardEvents.RegisterFriendlyMinionDamageEvent(new BoardEvents.EventDelegate(tgtGrandStand.FriendlyMinionDamage), 15f);
    tgtGrandStand.m_boardEvents.RegisterOpponentMinionDamageEvent(new BoardEvents.EventDelegate(tgtGrandStand.OpponentMinionDamage), 15f);
    tgtGrandStand.m_boardEvents.RegisterFriendlyMinionDeathEvent(new BoardEvents.EventDelegate(tgtGrandStand.FriendlyMinionDeath), 15f);
    tgtGrandStand.m_boardEvents.RegisterOppenentMinionDeathEvent(new BoardEvents.EventDelegate(tgtGrandStand.OpponentMinionDeath), 15f);
    tgtGrandStand.m_boardEvents.RegisterFriendlyMinionSpawnEvent(new BoardEvents.EventDelegate(tgtGrandStand.FriendlyMinionSpawn), 10f);
    tgtGrandStand.m_boardEvents.RegisterOppenentMinionSpawnEvent(new BoardEvents.EventDelegate(tgtGrandStand.OpponentMinionSpawn), 10f);
    tgtGrandStand.m_boardEvents.RegisterLargeShakeEvent(new BoardEvents.LargeShakeEventDelegate(tgtGrandStand.Shake));
  }

  private void FriendlyHeroDamage(float weight) => this.PlayOhNoAnimation();

  private void OpponentHeroDamage(float weight)
  {
    if ((double) weight > 15.0)
    {
      if ((double) weight > 20.0)
        this.PlayScoreCard("10", "10", "10");
      else
        this.PlayScoreCard("10", Random.Range(7, 9).ToString(), Random.Range(8, 10).ToString());
    }
    else
      this.PlayCheerAnimation();
  }

  private void FriendlyLegendarySpawn(float weight) => this.PlayCheerAnimation();

  private void OpponentLegendarySpawn(float weight) => this.PlayOhNoAnimation();

  private void FriendlyLegendaryDeath(float weight) => this.PlayOhNoAnimation();

  private void OpponentLegendaryDeath(float weight) => this.PlayCheerAnimation();

  private void FriendlyMinionDamage(float weight) => this.PlayOhNoAnimation();

  private void OpponentMinionDamage(float weight) => this.PlayCheerAnimation();

  private void FriendlyMinionDeath(float weight) => this.PlayOhNoAnimation();

  private void OpponentMinionDeath(float weight) => this.PlayCheerAnimation();

  private void FriendlyMinionSpawn(float weight) => this.PlayCheerAnimation();

  private void OpponentMinionSpawn(float weight) => this.PlayOhNoAnimation();
}
