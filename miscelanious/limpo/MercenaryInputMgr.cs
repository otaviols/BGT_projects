using System;

public class MercenaryInputMgr : InputMgr
{
  private static MercenaryInputMgr s_instance;
  public Func<bool> MouseOverTargetEvaluator;

  protected override bool MouseIsOverDeck
  {
    get => this.MouseOverTargetEvaluator != null ? this.MouseOverTargetEvaluator() : base.MouseIsOverDeck;
    set => base.MouseIsOverDeck = value;
  }

  protected override void Awake()
  {
    base.Awake();
    MercenaryInputMgr.s_instance = this;
  }

  protected override void OnDestroy()
  {
    MercenaryInputMgr.s_instance = (MercenaryInputMgr) null;
    base.OnDestroy();
  }

  public static MercenaryInputMgr Get() => MercenaryInputMgr.s_instance;
}
