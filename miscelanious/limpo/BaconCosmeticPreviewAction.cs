using System;

[CustomEditClass]
[Serializable]
public class BaconCosmeticPreviewAction
{
  [CustomEditField(SortPopupByName = true)]
  public BaconCosmeticPreviewActionType actionType;
  public float delay;
  public float duration;
  public bool waitUntilFinished;
  [CustomEditField(HidePredicate = "ShouldHideBoardState")]
  public TAG_BOARD_VISUAL_STATE boardState;
  [CustomEditField(HidePredicate = "ShouldHideFsmParameter")]
  public string fsmParameter;
  [CustomEditField(HidePredicate = "ShouldHideFinisherParams", SortPopupByName = true)]
  public KeyboardFinisherSettings.DamageLevel strikeDamageLevel;
  [CustomEditField(HidePredicate = "ShouldHideFinisherParams", SortPopupByName = true)]
  public KeyboardFinisherSettings.LethalLevel strikeLethalLevel;
  [CustomEditField(HidePredicate = "ShouldHideFinisherParams")]
  public int strikeImpactDamage;
  public string displayText;
}
