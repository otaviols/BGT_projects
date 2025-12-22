using UnityEngine;

public class HistoryChildCard : HistoryItem
{
  public void SetCardInfo(
    Entity entity,
    DefLoader.DisposableCardDef cardDef,
    int splatAmount,
    bool isDead,
    bool isBurnedCard,
    bool isPoisonous,
    bool isCriticalHit)
  {
    this.m_entity = entity;
    this.m_portraitTexture = cardDef.CardDef.GetPortraitTexture(this.m_entity.GetPremiumType());
    this.m_portraitGoldenMaterial = cardDef.CardDef.GetPremiumPortraitMaterial();
    this.SetCardDef(cardDef);
    this.m_splatAmount = splatAmount;
    this.m_dead = isDead;
    this.m_burned = isBurnedCard;
    this.m_isPoisonous = isPoisonous;
    this.m_isCriticalHit = isCriticalHit;
  }

  public void LoadMainCardActor()
  {
    string historyActor = ActorNames.GetHistoryActor(this.m_entity, HistoryInfoType.NONE);
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) historyActor, AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) gameObject == (Object) null)
    {
      Debug.LogWarningFormat("HistoryChildCard.LoadActorCallback() - FAILED to load actor \"{0}\"", (object) historyActor);
    }
    else
    {
      Actor component = gameObject.GetComponent<Actor>();
      if ((Object) component == (Object) null)
      {
        Debug.LogWarningFormat("HistoryChildCard.LoadActorCallback() - ERROR actor \"{0}\" has no Actor component", (object) historyActor);
      }
      else
      {
        this.m_mainCardActor = component;
        this.m_mainCardActor.SetPremium(this.m_entity.GetPremiumType());
        this.m_mainCardActor.SetWatermarkCardSetOverride(this.m_entity.GetWatermarkCardSetOverride());
        this.m_mainCardActor.SetHistoryItem((HistoryItem) this);
        this.m_mainCardActor.UpdateAllComponents();
        LayerUtils.SetLayer(this.m_mainCardActor.gameObject, GameLayer.Tooltip);
        this.m_mainCardActor.Hide();
      }
    }
  }
}
