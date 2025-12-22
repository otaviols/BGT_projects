using UnityEngine;

public class RewardCard : MonoBehaviour
{
  public string m_CardID = string.Empty;
  private bool m_Ready;
  private TAG_PREMIUM m_premium;
  private DefLoader.DisposableFullDef m_fullDef;
  private Actor m_actor;
  private GameLayer m_layer = GameLayer.IgnoreFullScreenEffects;

  private void OnDestroy()
  {
    this.m_Ready = false;
    this.m_fullDef?.Dispose();
    this.m_fullDef = (DefLoader.DisposableFullDef) null;
  }

  public bool IsReady() => this.m_Ready;

  public void LoadCard(CardRewardData cardData, GameLayer layer = GameLayer.IgnoreFullScreenEffects)
  {
    this.m_layer = layer;
    this.m_CardID = cardData.CardID;
    this.m_premium = cardData.Premium;
    DefLoader.Get().LoadFullDef(this.m_CardID, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnFullDefLoaded));
  }

  public void Death() => this.m_actor.ActivateSpellBirthState(SpellType.DEATH);

  private void OnFullDefLoaded(string cardId, DefLoader.DisposableFullDef fullDef, object userData)
  {
    using (fullDef)
    {
      if (fullDef == null)
      {
        Debug.LogWarning((object) string.Format("RewardCard.OnFullDefLoaded() - FAILED to load \"{0}\"", (object) cardId));
      }
      else
      {
        this.m_fullDef?.Dispose();
        this.m_fullDef = fullDef;
        string handActor = ActorNames.GetHandActor(this.m_fullDef?.EntityDef, this.m_premium);
        AssetLoader.Get().InstantiatePrefab((AssetReference) handActor, new PrefabCallback<GameObject>(this.OnActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
      }
    }
  }

  private void OnActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
    {
      Debug.LogWarning((object) string.Format("RewardCard.OnActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      Actor component = go.GetComponent<Actor>();
      if ((Object) component == (Object) null)
      {
        Debug.LogWarning((object) string.Format("RewardCard.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        this.m_actor = component;
        this.m_actor.TurnOffCollider();
        this.m_actor.SetEntityDef(this.m_fullDef?.EntityDef);
        this.m_actor.SetCardDef(this.m_fullDef?.DisposableCardDef);
        this.m_actor.SetPremium(this.m_premium);
        this.m_actor.UpdateAllComponents();
        LayerUtils.SetLayer(component.gameObject, this.m_layer);
        this.m_actor.transform.parent = this.transform;
        this.m_actor.transform.localPosition = Vector3.zero;
        this.m_actor.transform.localEulerAngles = new Vector3(270f, 0.0f, 0.0f);
        this.m_actor.transform.localScale = Vector3.one;
        this.m_actor.Show();
        this.m_Ready = true;
      }
    }
  }
}
