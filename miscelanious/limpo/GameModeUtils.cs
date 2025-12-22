using PegasusShared;

public static class GameModeUtils
{
  public static bool CanAccessGameModes() => AchieveManager.Get().HasUnlockedDefaultHeroes() || TavernBrawlManager.Get().HasUnlockedTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);

  public static bool ShouldSeeSoloAdventuresMovedPopup()
  {
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.FTUE, GameSaveKeySubkeyId.FTUE_SHOULD_SEE_SOLO_ADVENTURES_MOVED_POPUP, out num);
    return num > 0L;
  }

  public static bool HasSeenMercenariesButtonActivation()
  {
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.FTUE, GameSaveKeySubkeyId.FTUE_HAS_SEEN_MERCENARIES_BUTTON_ACTIVATION, out num);
    return num > 0L;
  }
}
