using System;
using UnityEngine;

[Serializable]
public class TagVisualActorConditionParameters
{
  [CustomEditField(HidePredicate = "ShouldHideTagValueParameters", HidePredicateInParent = true, Label = "Tag to Compare", SortPopupByName = true)]
  [Tooltip("Required for DOES_TAG_HAVE_VALUE")]
  public GAME_TAG m_Tag;
  [Tooltip("Required for DOES_TAG_HAVE_VALUE")]
  [CustomEditField(HidePredicate = "ShouldHideTagValueParameters", HidePredicateInParent = true, Label = "Tag Comparison Operator")]
  public TagVisualActorConditionComparisonOperator m_ComparisonOperator;
  [Tooltip("Required for DOES_TAG_HAVE_VALUE")]
  [CustomEditField(HidePredicate = "ShouldHideTagValueParameters", HidePredicateInParent = true, Label = "Tag Value to Compare")]
  public int m_Value;
  [CustomEditField(HidePredicate = "ShouldHideTagValueParameters", HidePredicateInParent = true, Label = "Tag Owner Entity")]
  [Tooltip("Required for DOES_TAG_HAVE_VALUE")]
  public TagVisualActorConditionEntity m_TagComparisonEntity;
  [Tooltip("Required for DOES_SPELL_HAVE_STATE")]
  [CustomEditField(HidePredicate = "ShouldHideSpellStateParameters", HidePredicateInParent = true, SortPopupByName = true)]
  public SpellType m_SpellType;
  [Tooltip("Required for DOES_SPELL_HAVE_STATE")]
  [CustomEditField(HidePredicate = "ShouldHideSpellStateParameters", HidePredicateInParent = true)]
  public SpellStateType m_SpellState;
  [Tooltip("Required for AND/OR")]
  [CustomEditField(HidePredicate = "ShouldHideCompoundConditionParameters", HidePredicateInParent = true)]
  public TagVisualActorCondition m_ConditionLHS;
  [Tooltip("Evaluate this condition opposite to the initial result")]
  [CustomEditField(HidePredicate = "ShouldHideCompoundConditionParameters", HidePredicateInParent = true)]
  public bool m_InvertConditionLHS;
  [CustomEditField(HidePredicate = "ShouldHideTagValueParametersLHS", Label = "Tag to Compare LHS", SortPopupByName = true)]
  [Tooltip("Required for AND/OR + DOES_TAG_HAVE_VALUE")]
  public GAME_TAG m_TagLHS;
  [Tooltip("Required for AND/OR + DOES_TAG_HAVE_VALUE")]
  [CustomEditField(HidePredicate = "ShouldHideTagValueParametersLHS", Label = "Tag Comparison Operator LHS")]
  public TagVisualActorConditionComparisonOperator m_ComparisonOperatorLHS;
  [CustomEditField(HidePredicate = "ShouldHideTagValueParametersLHS", Label = "Tag Value to Compare LHS")]
  [Tooltip("Required for AND/OR + DOES_TAG_HAVE_VALUE")]
  public int m_ValueLHS;
  [Tooltip("Required for AND/OR + DOES_TAG_HAVE_VALUE")]
  [CustomEditField(HidePredicate = "ShouldHideTagValueParametersLHS", Label = "Tag Owner Entity LHS")]
  public TagVisualActorConditionEntity m_TagComparisonEntityLHS;
  [Tooltip("Required for AND/OR + DOES_SPELL_HAVE_STATE")]
  [CustomEditField(HidePredicate = "ShouldHideSpellStateParametersLHS", SortPopupByName = true)]
  public SpellType m_SpellTypeLHS;
  [Tooltip("Required for AND/OR + DOES_SPELL_HAVE_STATE")]
  [CustomEditField(HidePredicate = "ShouldHideSpellStateParametersLHS")]
  public SpellStateType m_SpellStateLHS;
  [CustomEditField(HidePredicate = "ShouldHideCompoundConditionParameters", HidePredicateInParent = true)]
  [Tooltip("Required for AND/OR")]
  public TagVisualActorCondition m_ConditionRHS;
  [CustomEditField(HidePredicate = "ShouldHideCompoundConditionParameters", HidePredicateInParent = true)]
  [Tooltip("Evaluate this condition opposite to the initial result")]
  public bool m_InvertConditionRHS;
  [Tooltip("Required for AND/OR + DOES_TAG_HAVE_VALUE")]
  [CustomEditField(HidePredicate = "ShouldHideTagValueParametersRHS", Label = "Tag to Compare RHS", SortPopupByName = true)]
  public GAME_TAG m_TagRHS;
  [Tooltip("Required for AND/OR + DOES_TAG_HAVE_VALUE")]
  [CustomEditField(HidePredicate = "ShouldHideTagValueParametersRHS", Label = "Tag Comparison Operator RHS")]
  public TagVisualActorConditionComparisonOperator m_ComparisonOperatorRHS;
  [CustomEditField(HidePredicate = "ShouldHideTagValueParametersRHS", Label = "Tag Value to Compare RHS")]
  [Tooltip("Required for AND/OR + DOES_TAG_HAVE_VALUE")]
  public int m_ValueRHS;
  [CustomEditField(HidePredicate = "ShouldHideTagValueParametersRHS", Label = "Tag Owner Entity RHS")]
  [Tooltip("Required for AND/OR + DOES_TAG_HAVE_VALUE")]
  public TagVisualActorConditionEntity m_TagComparisonEntityRHS;
  [CustomEditField(HidePredicate = "ShouldHideSpellStateParametersRHS", SortPopupByName = true)]
  [Tooltip("Required for AND/OR + DOES_SPELL_HAVE_STATE")]
  public SpellType m_SpellTypeRHS;
  [Tooltip("Required for AND/OR + DOES_SPELL_HAVE_STATE")]
  [CustomEditField(HidePredicate = "ShouldHideSpellStateParametersRHS")]
  public SpellStateType m_SpellStateRHS;
}
