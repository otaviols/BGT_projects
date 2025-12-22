using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using UnityEngine;

public class PracticeAIButton : PegUIElement
{
  public UberText m_name;
  public UberText m_backsideName;
  public GameObject m_frontCover;
  public GameObject m_backsideCover;
  public HighlightState m_highlight;
  public GameObject m_unlockEffect;
  public GameObject m_questBang;
  public int m_PortraitMaterialIdx = -1;
  public GameObject m_rootObject;
  public Transform m_upBone;
  public Transform m_downBone;
  public Transform m_coveredBone;
  private int m_missionID;
  private long m_deckID;
  private bool m_covered;
  private bool m_locked;
  private bool m_infoSet;
  private bool m_usingBackside;
  private TAG_CLASS m_class;
  private DefLoader.DisposableCardDef m_cardDef;
  private const float FLIPPED_X_ROTATION = 180f;
  private const float NORMAL_X_ROTATION = 0.0f;
  private readonly string FLIP_COROUTINE = "WaitThenFlip";
  private readonly Vector3 GLOW_QUAD_NORMAL_LOCAL_POS = new Vector3(-0.1953466f, 1.336676f, 0.00721521f);
  private readonly Vector3 GLOW_QUAD_FLIPPED_LOCAL_POS = new Vector3(-0.1953466f, -1.336676f, 0.00721521f);

  public int GetMissionID() => this.m_missionID;

  public long GetDeckID() => this.m_deckID;

  public TAG_CLASS GetClass() => this.m_class;

  public void PlayUnlockGlow() => this.m_unlockEffect.GetComponent<Animation>().Play("AITileGlow");

  public void Lock(bool locked)
  {
    this.m_locked = locked;
    float num = this.m_locked ? 1f : 0.0f;
    this.SetEnabled(!this.m_locked);
    this.GetShowingMaterial().SetFloat("_Desaturate", num);
    this.m_rootObject.GetComponent<Renderer>().GetMaterial().SetFloat("_Desaturate", num);
  }

  public void SetInfo(
    string name,
    TAG_CLASS buttonClass,
    DefLoader.DisposableCardDef cardDef,
    int missionID,
    bool flip)
  {
    this.SetInfo(name, buttonClass, cardDef, missionID, 0L, flip);
  }

  public void SetInfo(
    string name,
    TAG_CLASS buttonClass,
    DefLoader.DisposableCardDef cardDef,
    long deckID,
    bool flip)
  {
    this.SetInfo(name, buttonClass, cardDef, 0, deckID, flip);
  }

  public void CoverUp(bool flip)
  {
    this.m_covered = true;
    if (flip)
    {
      this.GetHiddenNameMesh().Text = "";
      this.GetHiddenCover().GetComponent<Renderer>().enabled = true;
      this.Flip();
    }
    else
    {
      this.GetShowingNameMesh().Text = "";
      this.GetShowingCover().GetComponent<Renderer>().enabled = true;
    }
    iTween.MoveTo(this.m_rootObject, iTween.Hash((object) "position", (object) this.m_coveredBone.localPosition, (object) "time", (object) 0.25f, (object) "isLocal", (object) true, (object) "easeType", (object) iTween.EaseType.linear));
    this.SetEnabled(false);
  }

  public void Select()
  {
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("select_AI_opponent.prefab:a48887f01f79fa743a0c5de53a959b60"), this.gameObject);
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    this.SetEnabled(false);
    this.Depress();
  }

  public void Deselect()
  {
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
    if (this.m_covered)
      return;
    this.Raise();
    if (this.m_locked)
      return;
    this.SetEnabled(true);
  }

  public void Raise() => this.Raise(0.1f);

  public void ShowQuestBang(bool shown) => this.m_questBang.SetActive(shown);

  private void Flip()
  {
    this.StopCoroutine(this.FLIP_COROUTINE);
    this.m_usingBackside = !this.m_usingBackside;
    this.StartCoroutine(this.FLIP_COROUTINE, (object) this.m_usingBackside);
  }

  private IEnumerator WaitThenFlip(bool flipToBackside)
  {
    PracticeAIButton practiceAiButton = this;
    iTween.StopByName(practiceAiButton.gameObject, "flip");
    yield return (object) new WaitForEndOfFrame();
    float x1 = flipToBackside ? 0.0f : 180f;
    practiceAiButton.m_rootObject.transform.localEulerAngles = new Vector3(x1, 0.0f, 0.0f);
    Hashtable args = iTween.Hash((object) "amount", (object) new Vector3(180f, 0.0f, 0.0f), (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeOutElastic, (object) "space", (object) Space.Self, (object) "name", (object) "flip");
    iTween.RotateAdd(practiceAiButton.m_rootObject, args);
    float x2 = flipToBackside ? 180f : 0.0f;
    practiceAiButton.m_highlight.transform.localEulerAngles = new Vector3(x2, 0.0f, 0.0f);
    practiceAiButton.m_unlockEffect.transform.localPosition = flipToBackside ? practiceAiButton.GLOW_QUAD_FLIPPED_LOCAL_POS : practiceAiButton.GLOW_QUAD_NORMAL_LOCAL_POS;
  }

  private UberText GetShowingNameMesh() => !this.m_usingBackside ? this.m_name : this.m_backsideName;

  private UberText GetHiddenNameMesh() => !this.m_usingBackside ? this.m_backsideName : this.m_name;

  private Material GetShowingMaterial()
  {
    int materialIndex = this.m_usingBackside ? 2 : 1;
    return this.m_rootObject.GetComponent<Renderer>().GetMaterial(materialIndex);
  }

  private void SetShowingMaterial(Material mat)
  {
    int materialIndex = this.m_usingBackside ? 2 : 1;
    this.m_rootObject.GetComponent<Renderer>().SetMaterial(materialIndex, mat);
  }

  private Material GetHiddenMaterial()
  {
    int materialIndex = this.m_usingBackside ? 1 : 2;
    return this.m_rootObject.GetComponent<Renderer>().GetMaterial(materialIndex);
  }

  private void SetHiddenMaterial(Material mat)
  {
    int materialIndex = this.m_usingBackside ? 1 : 2;
    this.m_rootObject.GetComponent<Renderer>().SetMaterial(materialIndex, mat);
  }

  private GameObject GetShowingCover() => !this.m_usingBackside ? this.m_frontCover : this.m_backsideCover;

  private GameObject GetHiddenCover() => !this.m_usingBackside ? this.m_backsideCover : this.m_frontCover;

  private void SetInfo(
    string name,
    TAG_CLASS buttonClass,
    DefLoader.DisposableCardDef cardDef,
    int missionID,
    long deckID,
    bool flip)
  {
    this.SetMissionID(missionID);
    this.SetDeckID(deckID);
    this.SetButtonClass(buttonClass);
    this.m_cardDef?.Dispose();
    this.m_cardDef = cardDef;
    Material practiceAiPortrait = this.m_cardDef.CardDef.GetPracticeAIPortrait();
    if (flip)
    {
      this.GetHiddenNameMesh().Text = name;
      if ((Object) practiceAiPortrait != (Object) null)
        this.SetHiddenMaterial(practiceAiPortrait);
      this.Flip();
    }
    else
    {
      if (this.m_infoSet)
        Debug.LogWarning((object) "PracticeAIButton.SetInfo() - button is being re-initialized!");
      this.m_infoSet = true;
      if ((Object) practiceAiPortrait != (Object) null)
        this.SetShowingMaterial(practiceAiPortrait);
      this.GetShowingNameMesh().Text = name;
      this.SetOriginalLocalPosition();
    }
    this.m_covered = false;
    this.GetShowingCover().GetComponent<Renderer>().enabled = false;
  }

  private void SetMissionID(int missionID) => this.m_missionID = missionID;

  private void SetDeckID(long deckID) => this.m_deckID = deckID;

  private void SetButtonClass(TAG_CLASS buttonClass) => this.m_class = buttonClass;

  private void Raise(float time) => iTween.MoveTo(this.m_rootObject, iTween.Hash((object) "position", (object) this.m_upBone.localPosition, (object) nameof (time), (object) time, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true));

  private void Depress() => iTween.MoveTo(this.m_rootObject, iTween.Hash((object) "position", (object) this.m_downBone.localPosition, (object) "time", (object) 0.1f, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true));

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("collection_manager_hero_mouse_over.prefab:653cc8000b988cd468d2210a209adce6"), this.gameObject);
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState) => this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);

  protected override void OnDestroy()
  {
    this.m_cardDef?.Dispose();
    this.m_cardDef = (DefLoader.DisposableCardDef) null;
    base.OnDestroy();
  }
}
