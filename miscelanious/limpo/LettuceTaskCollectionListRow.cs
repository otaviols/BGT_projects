using Hearthstone.UI;
using UnityEngine;

public class LettuceTaskCollectionListRow : MonoBehaviour
{
  [SerializeField]
  private Listable m_listable;

  private void Awake() => this.m_listable.SetLayerOverride(GameLayer.CameraMask);
}
