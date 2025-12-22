using System.Collections.Generic;
using UnityEngine;

public class CardbackSubstituteSpell : Spell
{
  public List<Transform> m_FriendlyBones;
  public List<Transform> m_OpponentBones;
  private List<Actor> m_fakeActors;

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.LoadFakeActors();
    PlayMakerFSM component = this.GetComponent<PlayMakerFSM>();
    for (int index = 0; index < this.m_fakeActors.Count; ++index)
      component.FsmVariables.GetFsmGameObject("Card" + (object) (index + 1)).Value = this.m_fakeActors[index].gameObject;
  }

  private void LoadFakeActors()
  {
    this.m_fakeActors = this.SetupActor(this.m_FriendlyBones, Player.Side.FRIENDLY);
    this.m_fakeActors.AddRange((IEnumerable<Actor>) this.SetupActor(this.m_OpponentBones, Player.Side.OPPOSING));
  }

  private List<Actor> SetupActor(List<Transform> bones, Player.Side side)
  {
    List<Actor> actorList = new List<Actor>();
    for (int index = 0; index < bones.Count; ++index)
    {
      Actor component = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", AssetLoadingOptions.IgnorePrefabPosition).GetComponent<Actor>();
      component.SetCardBackSideOverride(new Player.Side?(side));
      component.UpdateAllComponents();
      actorList.Add(component);
      component.transform.parent = bones[index];
      GameUtils.ResetTransform((Component) component);
    }
    return actorList;
  }

  public override void OnSpellFinished()
  {
    foreach (Actor fakeActor in this.m_fakeActors)
      fakeActor.Destroy();
    base.OnSpellFinished();
  }
}
