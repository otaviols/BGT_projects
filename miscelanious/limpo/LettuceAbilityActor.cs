using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class LettuceAbilityActor : Actor
{
  public bool m_updateVisualsOnCooldown;
  public UberText m_currentCooldownText;
  public UberText m_cooldownConfigText;
  public GameObject m_hourglassObject;
  public GameObject m_checkMarkObject;
  public MeshRenderer m_mercenaryAbilityBannerMesh;
  public GameObject m_speedWing;
  public Color m_cooldownFrameColor;
  public AudioSource m_hoverSound;
  public List<AudioSource> m_clickSounds;

  public override void UpdateMeshComponents()
  {
    base.UpdateMeshComponents();
    this.UpdateMedallionCooldownState();
    this.UpdateSpeedWingState();
  }

  public void PlayMousedOverSound()
  {
    if (!((Object) this.m_hoverSound != (Object) null))
      return;
    SoundManager.Get().Play(this.m_hoverSound);
  }

  public void PlayMouseClickedSound()
  {
    if (this.m_clickSounds == null)
      return;
    foreach (AudioSource clickSound in this.m_clickSounds)
    {
      if ((Object) clickSound != (Object) null)
        SoundManager.Get().Play(clickSound);
    }
  }

  private void UpdateMedallionCooldownState()
  {
    if (this.m_entity == null || !this.m_updateVisualsOnCooldown)
      return;
    Renderer component = this.m_portraitMesh.GetComponent<Renderer>();
    if (this.m_entity.HasTag(GAME_TAG.LETTUCE_CURRENT_COOLDOWN))
    {
      if ((Object) this.m_hourglassObject != (Object) null)
        this.m_hourglassObject.SetActive(true);
      this.m_costTextMesh.gameObject.SetActive(false);
      component.GetMaterial(this.m_portraitFrameMatIdx).color = this.m_cooldownFrameColor;
    }
    else
    {
      if ((Object) this.m_hourglassObject != (Object) null)
        this.m_hourglassObject.SetActive(false);
      this.m_costTextMesh.gameObject.SetActive(true);
      component.GetMaterial(this.m_portraitFrameMatIdx).color = Color.white;
    }
  }

  private void UpdateSpeedWingState()
  {
    if ((Object) this.m_speedWing == (Object) null)
      return;
    bool flag = true;
    if (this.m_updateVisualsOnCooldown && this.m_entity != null && this.m_entity.HasTag(GAME_TAG.LETTUCE_CURRENT_COOLDOWN))
      flag = false;
    if (this.AbilityIsPassiveOrStartOfGame())
      flag = false;
    this.m_speedWing.SetActive(flag);
  }

  private bool AbilityIsPassiveOrStartOfGame() => this.m_entityDef != null && (this.m_entityDef.HasTag(GAME_TAG.LETTUCE_PASSIVE_ABILITY) || this.m_entityDef.HasTag(GAME_TAG.LETTUCE_START_OF_GAME_ABILITY)) || this.m_entity != null && (this.m_entity.HasTag(GAME_TAG.LETTUCE_PASSIVE_ABILITY) || this.m_entity.HasTag(GAME_TAG.LETTUCE_START_OF_GAME_ABILITY));

  public override void UpdateTextComponentsDef(EntityDef entityDef)
  {
    if (entityDef == null)
      return;
    base.UpdateTextComponentsDef(entityDef);
    this.UpdateCurrentCooldownText(entityDef);
    this.UpdateCooldownConfigText(entityDef);
    this.UpdateHourglassObject();
  }

  public override void UpdateTextComponents(Entity entity)
  {
    if (entity == null)
      return;
    base.UpdateTextComponents(entity);
    this.UpdateCurrentCooldownText(entity);
    this.UpdateCooldownConfigText(entity);
    this.UpdateHourglassObject();
    this.UpdateCheckMarkObject();
  }

  protected override void SetMaterialWithTexture(
    TAG_CARDTYPE cardType,
    CardColorSwitcher.CardColorType colorType)
  {
    base.SetMaterialWithTexture(cardType, colorType);
    if (!((Object) this.m_mercenaryAbilityBannerMesh != (Object) null) || this.m_cardColorTex == null)
      return;
    this.m_mercenaryAbilityBannerMesh.GetMaterial(0).mainTexture = (Texture) this.m_cardColorTex;
  }

  private void UpdateCurrentCooldownText(Entity entity)
  {
    if ((Object) this.m_currentCooldownText == (Object) null)
      return;
    int tag = entity.GetTag(GAME_TAG.LETTUCE_CURRENT_COOLDOWN);
    if (tag == 0)
      this.m_currentCooldownText.Text = string.Empty;
    else
      this.m_currentCooldownText.Text = tag.ToString();
  }

  private void UpdateCurrentCooldownText(EntityDef entityDef)
  {
    if ((Object) this.m_currentCooldownText == (Object) null)
      return;
    int tag = entityDef.GetTag(GAME_TAG.LETTUCE_CURRENT_COOLDOWN);
    if (tag == 0)
      this.m_currentCooldownText.Text = string.Empty;
    else
      this.m_currentCooldownText.Text = tag.ToString();
  }

  private void UpdateCooldownConfigText(Entity entity)
  {
    if ((Object) this.m_cooldownConfigText == (Object) null)
      return;
    int tag = entity.GetTag(GAME_TAG.LETTUCE_COOLDOWN_CONFIG);
    if (tag == 0)
      this.m_cooldownConfigText.Text = string.Empty;
    else
      this.m_cooldownConfigText.Text = tag.ToString();
  }

  private void UpdateCooldownConfigText(EntityDef entityDef)
  {
    if ((Object) this.m_cooldownConfigText == (Object) null)
      return;
    int tag = entityDef.GetTag(GAME_TAG.LETTUCE_COOLDOWN_CONFIG);
    if (tag == 0)
      this.m_cooldownConfigText.Text = string.Empty;
    else
      this.m_cooldownConfigText.Text = tag.ToString();
  }

  private void UpdateHourglassObject()
  {
    if ((Object) this.m_hourglassObject == (Object) null)
      return;
    if (string.IsNullOrEmpty(this.m_cooldownConfigText?.Text) && string.IsNullOrEmpty(this.m_currentCooldownText?.Text))
    {
      if (!this.m_hourglassObject.activeSelf)
        return;
      this.m_hourglassObject.SetActive(false);
    }
    else
    {
      if (this.m_hourglassObject.activeSelf)
        return;
      this.m_hourglassObject.SetActive(true);
    }
  }

  public void UpdateCheckMarkObject()
  {
    if ((Object) this.m_checkMarkObject == (Object) null)
      return;
    this.m_checkMarkObject.SetActive(false);
    if (this.m_entity == null)
      return;
    Entity lettuceAbilityOwner = this.m_entity.GetLettuceAbilityOwner();
    if (lettuceAbilityOwner == null || lettuceAbilityOwner.GetSelectedLettuceAbilityID() != this.m_entity.GetEntityId())
      return;
    this.m_checkMarkObject.SetActive(true);
  }
}
