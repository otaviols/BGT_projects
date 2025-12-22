using UnityEngine;

public class ForwardEmoteEventsToFSM : MonoBehaviour
{
  public PlayMakerFSM StateMachine;
  public float DamageTimer = 1f;
  private Card m_card;
  private float m_damageCountdown;
  private readonly string c_damageBeginEvent = "OnDamageBegin";
  private readonly string c_damageEndEvent = "OnDamageEnd";

  public void OnAttachedToActor(Actor actor)
  {
    this.m_card = actor.GetComponentInParent<Card>();
    if (!this.enabled)
      return;
    this.OnEnable();
  }

  private void OnEnable()
  {
    if ((Object) this.m_card != (Object) null)
      this.m_card.OnEmotePlayCallback += new Card.EmotePlayCallback(this.EmotePlayCallback);
    this.m_damageCountdown = 0.0f;
  }

  private void OnDisable()
  {
    if (!((Object) this.m_card != (Object) null))
      return;
    this.m_card.OnEmotePlayCallback -= new Card.EmotePlayCallback(this.EmotePlayCallback);
  }

  private void Update()
  {
    if ((double) this.m_damageCountdown <= 0.0)
      return;
    this.m_damageCountdown -= Time.deltaTime;
    if ((double) this.m_damageCountdown > 0.0)
      return;
    this.RaiseFSMEvent(this.c_damageEndEvent);
  }

  public void EmotePlayCallback(EmoteType emoteType) => this.RaiseFSMEvent(string.Format("OnEmote_{0}", (object) emoteType));

  public void RaiseFSMEvent(string eventName)
  {
    if (eventName == this.c_damageBeginEvent)
      this.m_damageCountdown = this.DamageTimer;
    if (!(bool) (Object) this.StateMachine || string.IsNullOrEmpty(eventName))
      return;
    this.StateMachine.SendEvent(eventName);
  }
}
