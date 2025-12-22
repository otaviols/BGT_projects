using PegasusUtil;
using System;
using UnityEngine;

public class ChallengePromptView : ShopView.IComponent
{
  private StoreChallengePrompt m_challengePrompt;

  public bool IsLoaded => (UnityEngine.Object) this.m_challengePrompt != (UnityEngine.Object) null;

  public bool IsShown => this.IsLoaded && this.m_challengePrompt.IsShown();

  public event Action OnComponentReady = () => { };

  public event Action<string, bool, CancelPurchase.CancelReason?, string> OnComplete = (challengeId, isSuccess, reason, error) => { };

  public event Action<string> OnCancel = challengeId => { };

  public void Load(IAssetLoader assetLoader)
  {
    if (this.IsLoaded)
      return;
    assetLoader.InstantiatePrefab((AssetReference) (string) ShopPrefabs.ShopChallengePromptPrefab, new PrefabCallback<GameObject>(this.OnLoaded));
  }

  public void Unload()
  {
    if (!this.IsLoaded)
      return;
    this.m_challengePrompt.OnChallengeComplete -= new StoreChallengePrompt.CompleteListener(this.CompleteListener);
    this.m_challengePrompt.OnCancel -= new StoreChallengePrompt.CancelListener(this.CancelListener);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_challengePrompt.gameObject);
    this.m_challengePrompt = (StoreChallengePrompt) null;
  }

  public void StartChallenge(string challengeId)
  {
    if (!this.IsLoaded)
      return;
    this.m_challengePrompt.StartCoroutine(this.m_challengePrompt.Show(challengeId));
  }

  public void Hide()
  {
    if (!this.IsShown)
      return;
    this.m_challengePrompt.Hide();
  }

  public bool Cancel(Action<string> onCancel)
  {
    string str = this.m_challengePrompt.HideChallenge();
    if (string.IsNullOrEmpty(str))
      return false;
    onCancel(str);
    return true;
  }

  private void OnLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "ChallengePromptView.OnLoaded(): go is null!");
    }
    else
    {
      this.m_challengePrompt = go.GetComponent<StoreChallengePrompt>();
      if ((UnityEngine.Object) this.m_challengePrompt == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "ChallengePromptView.OnLoaded(): go has no StoreChallengePrompt component");
      }
      else
      {
        this.m_challengePrompt.Hide();
        this.m_challengePrompt.OnChallengeComplete += new StoreChallengePrompt.CompleteListener(this.CompleteListener);
        this.m_challengePrompt.OnCancel += new StoreChallengePrompt.CancelListener(this.CancelListener);
        this.OnComponentReady();
      }
    }
  }

  private void CompleteListener(
    string challengeId,
    bool isSuccess,
    CancelPurchase.CancelReason? reason,
    string error)
  {
    this.OnComplete(challengeId, isSuccess, reason, error);
  }

  private void CancelListener(string challengeId) => this.OnCancel(challengeId);
}
