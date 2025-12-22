using UnityEngine;

public class PagingArrow : MonoBehaviour
{
  public GameObject m_pagingArrowHighlight;

  public void ShowHighlight() => this.m_pagingArrowHighlight.SetActive(true);

  public void HideHighlight() => this.m_pagingArrowHighlight.SetActive(false);
}
