using UnityEngine;

public class PuzzleIntroSpell : Spell
{
  [SerializeField]
  private Transform m_ConfirmButton;

  public Transform GetConfirmButton() => this.m_ConfirmButton;
}
