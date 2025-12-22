using Blizzard.T5.Services;
using UnityEngine;

public class CardBackDragEffect : MonoBehaviour
{
  private const float MIN_VELOCITY = 2f;
  private const float MAX_VELOCITY = 30f;
  public Actor m_Actor;
  public GameObject m_EffectsRoot;
  private CardBackManager m_CardBackManager;
  private Vector3 m_LastPosition;
  private float m_Speed;
  private bool m_Active;
  private float m_Min = 2f;
  private float m_Max = 30f;

  private void Start()
  {
    SceneMgr service;
    if (!ServiceManager.TryGet<SceneMgr>(out service) || service.GetMode() != SceneMgr.Mode.GAMEPLAY)
    {
      this.enabled = false;
    }
    else
    {
      this.m_LastPosition = this.transform.position;
      if (this.m_CardBackManager == null)
      {
        this.m_CardBackManager = CardBackManager.Get();
        if (this.m_CardBackManager == null)
        {
          Debug.LogError((object) "Failed to get CardBackManager!");
          this.enabled = false;
        }
      }
      if (this.m_CardBackManager != null)
        this.m_CardBackManager.RegisterUpdateCardbacksListener(new CardBackManager.UpdateCardbacksCallback(this.SetEffect));
      this.SetEffect();
    }
  }

  private void Update()
  {
    if (!((Object) this.m_EffectsRoot != (Object) null))
      return;
    if (!this.GetComponent<Renderer>().enabled)
    {
      this.ShowParticles(false);
      if (!this.m_EffectsRoot.activeSelf)
        return;
      this.m_EffectsRoot.SetActive(false);
    }
    else
    {
      this.m_Speed = (float) ((double) (this.transform.position - this.m_LastPosition).magnitude / (double) Time.deltaTime * 3.59999990463257);
      this.UpdateDragEffect();
      this.m_LastPosition = this.transform.position;
    }
  }

  private void OnDisable()
  {
    if (!((Object) this.m_EffectsRoot != (Object) null))
      return;
    this.ShowParticles(false);
  }

  private void OnDestroy()
  {
    if (CardBackManager.Get() == null)
      return;
    CardBackManager.Get().UnregisterUpdateCardbacksListener(new CardBackManager.UpdateCardbacksCallback(this.SetEffect));
  }

  public void SetEffect()
  {
    if (this.m_CardBackManager == null)
    {
      this.m_CardBackManager = CardBackManager.Get();
      if (this.m_CardBackManager == null)
      {
        Debug.LogError((object) "Failed to get CardBackManager!");
        this.enabled = false;
        return;
      }
    }
    this.m_CardBackManager.UpdateDragEffect(this.gameObject, this.m_CardBackManager.IsActorFriendly(this.m_Actor) ? CardBackManager.CardBackSlot.FRIENDLY : CardBackManager.CardBackSlot.OPPONENT);
    CardBack cardBackForActor = this.m_CardBackManager.GetCardBackForActor(this.m_Actor);
    if (!((Object) cardBackForActor != (Object) null))
      return;
    this.m_Min = cardBackForActor.m_EffectMinVelocity;
    this.m_Max = cardBackForActor.m_EffectMaxVelocity;
  }

  private void UpdateDragEffect()
  {
    if ((double) this.m_Speed > (double) this.m_Min && (double) this.m_Speed < (double) this.m_Max)
    {
      if (this.m_Active)
        return;
      this.m_Active = true;
    }
    else
    {
      if (!this.m_Active)
        return;
      this.m_Active = false;
    }
    this.ShowParticles(this.m_Active);
  }

  private void ShowParticles(bool show)
  {
    foreach (ParticleSystem componentsInChild in this.GetComponentsInChildren<ParticleSystem>())
    {
      if (!((Object) componentsInChild == (Object) null))
      {
        if (show)
          componentsInChild.Play();
        else
          componentsInChild.Stop();
      }
    }
  }
}
