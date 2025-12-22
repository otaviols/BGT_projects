using UnityEngine;

public class CheatHighlight : MonoBehaviour
{
  [SerializeField]
  public GameObject m_highlight;

  private void OnMouseEnter() => this.m_highlight.SetActive(true);

  private void OnMouseExit() => this.m_highlight.SetActive(false);
}
