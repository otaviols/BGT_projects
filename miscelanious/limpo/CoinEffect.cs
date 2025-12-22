using UnityEngine;

public class CoinEffect : MonoBehaviour
{
  public GameObject coinSpawnObject;
  private string coinSpawnAnim = "CoinSpawn1_edit";
  public GameObject coin;
  private string coinDropAnim = "MulliganCoinDropGo2Card";
  public GameObject coinGlow;
  private string coinDropAnim2 = "MulliganCoinDrop2_Edit";
  private string animToUse;
  private string coinGlowDropAnim = "MulliganCoinDrop1Glow_Edit";
  private string coinGlowDropAnim2 = "MulliganCoinDrop2Glow_Edit";
  private string GlowanimToUse;

  public void DoAnim(bool localWin)
  {
    if (localWin)
    {
      this.animToUse = this.coinDropAnim2;
      this.GlowanimToUse = this.coinGlowDropAnim2;
    }
    else
    {
      this.animToUse = this.coinDropAnim;
      this.GlowanimToUse = this.coinGlowDropAnim;
    }
    this.coinSpawnObject.SetActive(true);
    this.coin.SetActive(true);
    this.coinGlow.SetActive(true);
    Animation component1 = this.coinSpawnObject.GetComponent<Animation>();
    component1.Stop(this.coinSpawnAnim);
    Animation component2 = this.coin.GetComponent<Animation>();
    component2.Stop(this.animToUse);
    Animation component3 = this.coinGlow.GetComponent<Animation>();
    component3.Stop(this.GlowanimToUse);
    component1.Play(this.coinSpawnAnim);
    component2.Play(this.animToUse);
    component3.Play(this.GlowanimToUse);
  }
}
