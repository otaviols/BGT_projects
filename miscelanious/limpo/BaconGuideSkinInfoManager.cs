using Blizzard.T5.AssetManager;
using Hearthstone;
using Hearthstone.UI;
using System;
using System.Text;
using UnityEngine;

[CustomEditClass]
public class BaconGuideSkinInfoManager : BaconBaseSkinInfoManager
{
  private static BaconGuideSkinInfoManager s_instance;
  private static bool s_isReadyingInstance;

  public static BaconGuideSkinInfoManager Get() => BaconGuideSkinInfoManager.s_instance;

  public static void EnterPreviewWhenReady(CollectionCardVisual cardVisual)
  {
    BaconGuideSkinInfoManager guideSkinInfoManager = BaconGuideSkinInfoManager.Get();
    if ((UnityEngine.Object) guideSkinInfoManager != (UnityEngine.Object) null)
      guideSkinInfoManager.EnterPreview(cardVisual);
    else if (BaconGuideSkinInfoManager.s_isReadyingInstance)
    {
      Debug.LogWarning((object) "BaconGuideSkinInfoManager:EnterPreviewWhenReady called while the info manager instance was being readied");
    }
    else
    {
      Widget widget = (Widget) WidgetInstance.Create("BaconGuideSkinInfoManager.prefab:2201365483a4bd748ab41038e3b56d91");
      if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "BaconGuideSkinInfoManager:EnterPreviewWhenReady failed to create widget instance");
      }
      else
      {
        BaconGuideSkinInfoManager.s_isReadyingInstance = true;
        widget.RegisterReadyListener((Action<object>) (_ =>
        {
          BaconGuideSkinInfoManager.s_instance = widget.GetComponentInChildren<BaconGuideSkinInfoManager>();
          BaconGuideSkinInfoManager.s_isReadyingInstance = false;
          if ((UnityEngine.Object) BaconGuideSkinInfoManager.s_instance == (UnityEngine.Object) null)
            Debug.LogError((object) "BaconGuideSkinInfoManager:EnterPreviewWhenReady created widget instance but failed to get BaconGuideSkinInfoManager component");
          else
            BaconGuideSkinInfoManager.s_instance.EnterPreview(cardVisual);
        }), (object) null, true);
      }
    }
  }

  public static bool IsLoadedAndShowingPreview() => (bool) (UnityEngine.Object) BaconGuideSkinInfoManager.s_instance && BaconGuideSkinInfoManager.s_instance.IsShowingPreview;

  private void OnDestroy()
  {
    this.m_currentHeroCardDef?.Dispose();
    this.m_currentHeroCardDef = (DefLoader.DisposableCardDef) null;
    AssetHandle.SafeDispose<UberShaderAnimation>(ref this.m_currentHeroGoldenAnimation);
    this.CancelPreview();
    BaconGuideSkinInfoManager.s_instance = (BaconGuideSkinInfoManager) null;
  }

  protected override void PushNavigateBack() => Navigation.PushUnique(new Navigation.NavigateBackHandler(BaconGuideSkinInfoManager.OnNavigateBack));

  protected override void RemoveNavigateBack() => Navigation.RemoveHandler(new Navigation.NavigateBackHandler(BaconGuideSkinInfoManager.OnNavigateBack));

  private static bool OnNavigateBack()
  {
    BaconGuideSkinInfoManager guideSkinInfoManager = BaconGuideSkinInfoManager.Get();
    if ((UnityEngine.Object) guideSkinInfoManager != (UnityEngine.Object) null)
      guideSkinInfoManager.CancelPreview();
    return true;
  }

  protected override void SetFavoriteHero()
  {
    if (!CollectionManager.Get().IsBattlegroundsGuideCardId(this.m_currentEntityDef.GetCardId()))
      return;
    int dbId = GameUtils.TranslateCardIdToDbId(this.m_currentEntityDef.GetCardId());
    BattlegroundsGuideSkinId skinId;
    if (CollectionManager.Get().GetBattlegroundsGuideSkinIdForCardId(dbId, out skinId))
    {
      if (!CollectionManager.Get().OwnsBattlegroundsGuideSkin(dbId))
        return;
      Network.Get().SetBattlegroundsFavoriteGuideSkin(skinId);
    }
    else
      Network.Get().ClearBattlegroundsFavoriteGuideSkin();
  }

  protected override bool CanToggleFavorite() => BaconHeroSkinUtils.CanFavoriteBattlegroundsGuideSkin(this.m_currentEntityDef);

  protected override void AppendDebugTextForCurrentCard(StringBuilder builder)
  {
    base.AppendDebugTextForCurrentCard(builder);
    int dbId = GameUtils.TranslateCardIdToDbId(this.m_currentEntityDef.GetCardId());
    BattlegroundsGuideSkinId skinId;
    if (CollectionManager.Get().GetBattlegroundsGuideSkinIdForCardId(dbId, out skinId))
    {
      builder.Append("Guide Skin Id: ");
      builder.Append(skinId.ToValue());
      builder.AppendLine();
    }
    else
      builder.AppendLine("No Guide Skin Id");
  }
}
