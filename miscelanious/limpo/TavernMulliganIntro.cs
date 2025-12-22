using System.Collections;
using UnityEngine;

public class TavernMulliganIntro : InfoPopupMulliganIntro
{
  private static bool s_hasSeenTutorialPopup;

  public void Show(MonoBehaviour monoBehaviour) => monoBehaviour.StartCoroutine(this.ShowPopupIfNotAlreadySeen(monoBehaviour));

  private IEnumerator ShowPopupIfNotAlreadySeen(MonoBehaviour monoBehaviour)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TavernMulliganIntro tavernMulliganIntro = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      TavernMulliganIntro.s_hasSeenTutorialPopup = true;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) tavernMulliganIntro.ShowPopup("AdventureTutorialPopup_DAL.prefab:58e01991c604aad43bc7ae12db9023f6", "FriendlyChoice", TavernMulliganIntro.s_hasSeenTutorialPopup);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
