using UnityEngine;
using UnityEngine.Events;

public class DuelsVaultDoor : MonoBehaviour
{
  public const string VAULT_DIAL_ANIM = "vaultpad_dialturn";
  public GameObject m_heroicWinText;
  public GameObject m_prevHeroicWinText;

  private void Start() => GameUtils.OnAnimationExitEvent.AddListener(new UnityAction<string>(this.OnAnimationEnded));

  private void OnAnimationEnded(string AnimationName)
  {
    if (!(AnimationName == "vaultpad_dialturn") || !((Object) this.m_heroicWinText != (Object) null) || !((Object) this.m_prevHeroicWinText != (Object) null))
      return;
    this.m_prevHeroicWinText.SetActive(false);
    this.m_heroicWinText.SetActive(true);
  }

  private void OnDestroy() => GameUtils.OnAnimationExitEvent.RemoveListener(new UnityAction<string>(this.OnAnimationEnded));
}
