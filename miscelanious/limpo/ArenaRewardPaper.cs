using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaRewardPaper : MonoBehaviour
{
  public GameObject m_XmarksRoot;
  public List<GameObject> m_XmarkBox;
  public GameObject m_Xmark1;
  public GameObject m_Xmark2;
  public GameObject m_Xmark3;
  public UberText m_WinsUberText;
  public UberText m_LossesUberText;
  public UberText m_EventEndsText;
  private static readonly AssetReference DEFAULT_REWARD_PAPER = new AssetReference("ArenaPaper.prefab:0c4143d801e717543a456f444d689a16");
  private static readonly AssetReference DEFAULT_REWARD_PAPER_PHONE = new AssetReference("ArenaPaper_phone.prefab:644a36f346814cc41bf925997db07f5e");

  public static AssetReference GetDefaultRewardPaper() => !(bool) UniversalInputManager.UsePhoneUI ? ArenaRewardPaper.DEFAULT_REWARD_PAPER : ArenaRewardPaper.DEFAULT_REWARD_PAPER_PHONE;

  public IEnumerator PlayRewardBurnAway(PlayMakerFSM rewardFSM)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    ArenaRewardPaper arenaRewardPaper = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      rewardFSM.SendEvent("FINISHED");
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Animation component = arenaRewardPaper.GetComponent<Animation>();
    component.Play();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(component.clip.length);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public void PlayEmberWipeFX()
  {
    foreach (ParticleSystem componentsInChild in this.GetComponentsInChildren<ParticleSystem>())
      componentsInChild.Play();
  }
}
