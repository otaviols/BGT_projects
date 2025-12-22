using Blizzard.T5.AssetManager;
using UnityEngine;

[CreateAssetMenu(menuName = "Prototyping/Finisher Gameplay Settings")]
public class FinisherGameplaySettings : ScriptableObject
{
  [HideInInspector]
  public string SmallPrefab;
  [HideInInspector]
  public string SmallOpponentPrefab;
  [HideInInspector]
  public string LargePrefab;
  [HideInInspector]
  public string LargeOpponentPrefab;
  [HideInInspector]
  public string LethalPrefab;
  [HideInInspector]
  public string LethalOpponentPrefab;
  [HideInInspector]
  public string FirstPlaceVictoryPrefab;
  [HideInInspector]
  public string FirstPlaceVictoryOpponentPrefab;
  [HideInInspector]
  public string DestroyPlayerPrefab;
  [HideInInspector]
  public string DestroyOpponentPrefab;
  [HideInInspector]
  public string FirstPlaceVictoryDestroyPlayerPrefab;
  [HideInInspector]
  public string FirstPlaceVictoryDestroyOpponentPrefab;
  public bool ShowImpactEffects;
  public bool FullyImplemented;

  public static FinisherGameplaySettings GetFinisherGameplaySettings(
    Entity hero)
  {
    int id = hero.GetTag(GAME_TAG.BATTLEGROUNDS_FAVORITE_FINISHER);
    if (id <= 0)
    {
      Log.Spells.PrintError(hero.GetDebugName() + " has no tag BATTLEGROUNDS_FAVORITE_FINISHER. Using Default Finisher.");
      id = 1;
    }
    BattlegroundsFinisherDbfRecord record = GameDbf.BattlegroundsFinisher.GetRecord(id);
    if (record == null)
    {
      Log.Spells.PrintError(string.Format("No Finisher was found for Finisher ID {0}. Using default finisher.", (object) id));
      record = GameDbf.BattlegroundsFinisher.GetRecord(1);
    }
    AssetReference fromAssetString1 = AssetReference.CreateFromAssetString(record.GameplaySettings);
    AssetHandle<FinisherGameplaySettings> assetHandle = fromAssetString1 != null ? AssetLoader.Get().LoadAsset<FinisherGameplaySettings>(fromAssetString1) : (AssetHandle<FinisherGameplaySettings>) null;
    FinisherGameplaySettings gameplaySettings = (bool) assetHandle ? assetHandle.Asset : (FinisherGameplaySettings) null;
    if ((Object) gameplaySettings == (Object) null)
    {
      Log.Spells.PrintError(string.Format("Finisher ID {0} is missing its finisher settings entirely in HE2. Using default finisher.", (object) id));
      AssetReference fromAssetString2 = AssetReference.CreateFromAssetString(GameDbf.BattlegroundsFinisher.GetRecord(1).GameplaySettings);
      gameplaySettings = AssetLoader.Get().LoadAsset<FinisherGameplaySettings>(fromAssetString2).Asset;
    }
    return gameplaySettings;
  }
}
