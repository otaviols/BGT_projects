using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using System;
using UnityEngine;

public class PlayerLeaderboardRecentCombatEntry : MonoBehaviour
{
  public Actor m_opponentTileActor;
  public GameObject m_iconOwnerSwords;
  public GameObject m_iconOpponentSwords;
  public GameObject m_iconOwnerSplat;
  public GameObject m_iconOpponentSplat;
  public GameObject m_background;
  public GameObject m_opponentMeshRoot;
  private PlayerLeaderboardRecentCombatEntry.RecentActionType m_recentActionType;
  private PlayerLeaderboardRecentCombatEntry.RecentActionTarget m_recentActionTarget;
  private int m_ownerId;
  private int m_opponentId;
  private int m_splatAmount;
  private PlayerLeaderboardCard m_source;
  private const float TILE_PORTRAIT_MESH_Y_OFFSET = 0.01f;
  private const float TILE_Y_OFFSET = -0.5f;

  private PlayerLeaderboardTile m_opponentLeaderboardTile => this.m_opponentTileActor.GetComponent<PlayerLeaderboardTile>();

  public void Awake()
  {
    this.m_opponentMeshRoot.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 180f, 0.0f));
    this.m_opponentMeshRoot.transform.localPosition = new Vector3(this.m_opponentMeshRoot.transform.localPosition.x, 0.01f, this.m_opponentMeshRoot.transform.localPosition.z);
    this.m_opponentTileActor.transform.localPosition = new Vector3(this.m_opponentTileActor.transform.localPosition.x, -0.5f, this.m_opponentTileActor.transform.localPosition.z);
  }

  private void SetActionTarget(
    PlayerLeaderboardRecentCombatEntry.RecentActionTarget target)
  {
    this.m_recentActionTarget = target;
  }

  private void SetActionType(
    PlayerLeaderboardRecentCombatEntry.RecentActionType type)
  {
    this.m_recentActionType = type;
  }

  private void SetSplatAmount(int splatAmount) => this.m_splatAmount = -splatAmount;

  public void Load(
    PlayerLeaderboardCard source,
    PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo recentCombatInfo)
  {
    this.m_source = source;
    this.m_ownerId = recentCombatInfo.ownerId;
    this.m_opponentId = recentCombatInfo.opponentId;
    this.SetActionTarget(recentCombatInfo.damageTarget == this.m_ownerId ? PlayerLeaderboardRecentCombatEntry.RecentActionTarget.OWNER : (recentCombatInfo.damageTarget == this.m_opponentId ? PlayerLeaderboardRecentCombatEntry.RecentActionTarget.OPPONENT : PlayerLeaderboardRecentCombatEntry.RecentActionTarget.TIE));
    this.SetActionType(recentCombatInfo.isDefeated ? PlayerLeaderboardRecentCombatEntry.RecentActionType.DEATH : PlayerLeaderboardRecentCombatEntry.RecentActionType.DAMAGE);
    this.SetSplatAmount(recentCombatInfo.damage);
    this.LoadTileForPlayer(this.m_ownerId);
    this.LoadTileForPlayer(this.m_opponentId);
    this.UpdateDisplay();
    LayerUtils.SetLayer(this.gameObject, GameLayer.Tooltip);
  }

  private void UpdateDisplay()
  {
    this.m_iconOwnerSwords.SetActive(this.m_recentActionTarget == PlayerLeaderboardRecentCombatEntry.RecentActionTarget.OPPONENT || this.m_recentActionTarget == PlayerLeaderboardRecentCombatEntry.RecentActionTarget.TIE);
    this.m_iconOpponentSwords.SetActive(this.m_recentActionTarget == PlayerLeaderboardRecentCombatEntry.RecentActionTarget.OWNER);
    this.m_iconOwnerSplat.SetActive(this.m_recentActionTarget == PlayerLeaderboardRecentCombatEntry.RecentActionTarget.OPPONENT || this.m_recentActionTarget == PlayerLeaderboardRecentCombatEntry.RecentActionTarget.TIE);
    this.m_iconOpponentSplat.SetActive(this.m_recentActionTarget == PlayerLeaderboardRecentCombatEntry.RecentActionTarget.OWNER);
    this.m_opponentLeaderboardTile.SetSkullIconActive(this.m_recentActionTarget == PlayerLeaderboardRecentCombatEntry.RecentActionTarget.OPPONENT && this.m_recentActionType == PlayerLeaderboardRecentCombatEntry.RecentActionType.DEATH);
    this.m_opponentLeaderboardTile.SetHealthBarActive(false);
    this.UpdateSplatSpell(this.m_recentActionTarget == PlayerLeaderboardRecentCombatEntry.RecentActionTarget.OPPONENT || this.m_recentActionTarget == PlayerLeaderboardRecentCombatEntry.RecentActionTarget.TIE ? this.m_iconOwnerSplat : this.m_iconOpponentSplat);
  }

  private void UpdateSplatSpell(GameObject splatIcon)
  {
    DamageSplatSpell component = splatIcon.GetComponent<DamageSplatSpell>();
    component.SetDamage(-this.m_splatAmount);
    component.ChangeState(SpellStateType.IDLE);
    component.Show();
  }

  private void LoadTileForPlayer(int playerId)
  {
    Actor opponentTileActor = this.m_opponentTileActor;
    Entity entity = playerId != 0 ? GameState.Get().GetPlayerInfoMap()[playerId].GetPlayerHero() : PlayerLeaderboardManager.Get().GetOddManOutOpponentHero();
    if (entity == null)
    {
      Debug.LogWarningFormat("PlayerLeaderboardRecentCombatEntry.LoadTileForPlayer() - FAILED to load playerHeroEntity for playerId \"{0}\"", (object) playerId);
    }
    else
    {
      DefLoader.DisposableCardDef disposableCardDef = entity.ShareDisposableCardDef();
      if (disposableCardDef == null)
      {
        Debug.LogWarningFormat("PlayerLeaderboardRecentCombatEntry.LoadTileForPlayer() - FAILED to load cardDef for playerId \"{0}\"", (object) playerId);
      }
      else
      {
        Material[] materials = new Material[2];
        ServiceManager.Get<DisposablesCleaner>()?.Attach(opponentTileActor.gameObject, (IDisposable) disposableCardDef);
        materials[0] = opponentTileActor.GetMeshRenderer().GetMaterial();
        TAG_PREMIUM premiumType = entity.GetPremiumType();
        if ((UnityEngine.Object) disposableCardDef.CardDef.GetLeaderboardTileFullPortrait() != (UnityEngine.Object) null)
        {
          materials[1] = disposableCardDef.CardDef.GetLeaderboardTileFullPortrait();
          opponentTileActor.GetMeshRenderer().SetMaterials(materials);
        }
        else if (disposableCardDef.CardDef.TryGetHistoryTileFullPortrait(premiumType, out materials[1]))
          opponentTileActor.GetMeshRenderer().SetMaterials(materials);
        else
          opponentTileActor.GetMeshRenderer().GetMaterial(1).mainTexture = disposableCardDef.CardDef.GetPortraitTexture(premiumType);
        foreach (Renderer componentsInChild in opponentTileActor.GetMeshRenderer().GetComponentsInChildren<Renderer>())
        {
          if (!(componentsInChild.tag == "FakeShadow"))
            componentsInChild.GetMaterial().color = Board.Get().m_HistoryTileColor;
        }
        opponentTileActor.GetMeshRenderer().GetMaterial(1).color = Board.Get().m_HistoryTileColor;
        Color color = GameState.Get().GetFriendlyPlayerId() == playerId ? this.m_source.m_selfBorderColor : this.m_source.m_enemyBorderColor;
        this.SetBorderColor(this.PlayerIsDead(playerId) ? this.m_source.m_deadColor : color, opponentTileActor);
      }
    }
  }

  private void SetBorderColor(Color color, Actor targetTile) => targetTile.GetMeshRenderer().GetMaterial().color = color;

  private bool PlayerIsDead(int playerId)
  {
    if (this.m_recentActionType != PlayerLeaderboardRecentCombatEntry.RecentActionType.DEATH)
      return false;
    if (this.m_recentActionTarget == PlayerLeaderboardRecentCombatEntry.RecentActionTarget.OPPONENT && playerId == this.m_opponentId)
      return true;
    return this.m_recentActionTarget == PlayerLeaderboardRecentCombatEntry.RecentActionTarget.OWNER && playerId == this.m_ownerId;
  }

  public enum RecentActionType
  {
    DAMAGE,
    DEATH,
  }

  public enum RecentActionTarget
  {
    OWNER,
    OPPONENT,
    TIE,
  }
}
