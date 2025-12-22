using UnityEngine;

public class RewardPackage : PegUIElement
{
  public int m_RewardIndex = -1;

  protected override void Awake() => base.Awake();

  public void OnEnable() => this.SetEnabled(true);

  public void OnDisable() => this.SetEnabled(false);

  public void Update()
  {
    if (InputCollection.GetKeyDown(KeyCode.Alpha1) && this.m_RewardIndex == 0)
      this.OpenReward();
    else if (InputCollection.GetKeyDown(KeyCode.Alpha2) && this.m_RewardIndex == 1)
      this.OpenReward();
    else if (InputCollection.GetKeyDown(KeyCode.Alpha3) && this.m_RewardIndex == 2)
      this.OpenReward();
    else if (InputCollection.GetKeyDown(KeyCode.Alpha4) && this.m_RewardIndex == 3)
    {
      this.OpenReward();
    }
    else
    {
      if (!InputCollection.GetKeyDown(KeyCode.Alpha5) || this.m_RewardIndex != 4)
        return;
      this.OpenReward();
    }
  }

  protected override void OnOver(PegUIElement.InteractionState oldState) => this.GetComponent<PlayMakerFSM>().SendEvent("Action");

  protected override void OnPress() => this.OpenReward();

  private void OpenReward()
  {
    this.GetComponent<PlayMakerFSM>().SendEvent("Death");
    RewardBoxesDisplay.Get().OpenReward(this.m_RewardIndex, this.transform.position);
    this.enabled = false;
  }
}
