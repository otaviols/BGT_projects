using System;
using System.Collections.Generic;
using UnityEngine;

public class EditTimeSceneConfiguration : MonoBehaviour
{
  [HideInInspector]
  public string LastSelectedConfiguration = "";
  public List<EditTimeSceneConfiguration.SceneConfiguration> Configurations = new List<EditTimeSceneConfiguration.SceneConfiguration>();
  [Header("Quick Links")]
  [SerializeField]
  private KeyboardFinisherSettings _authoringSettings;
  [SerializeField]
  public KeyboardFinisherSettings _recordingSettings;
  [SerializeField]
  public FinisherAuthoringList _allFinishers;

  public void ApplyState(int stateIndex, HashSet<int> runStates)
  {
    if (stateIndex < 0 || stateIndex >= this.Configurations.Count)
    {
      Log.BattlegroundsAuthoring.PrintError("EditTimeSceneConfiguration: Attempted to apply state " + (object) stateIndex + " which is undefined.");
    }
    else
    {
      if (runStates == null)
        runStates = new HashSet<int>();
      if (runStates.Contains(stateIndex))
      {
        Log.BattlegroundsAuthoring.PrintError("EditTimeSceneConfiguration: Infinite recursion detected in state " + (object) stateIndex + ".");
      }
      else
      {
        runStates.Add(stateIndex);
        if (this.Configurations[stateIndex].FirstRunState >= 0)
        {
          if (this.Configurations[stateIndex].FirstRunState >= this.Configurations.Count)
          {
            Log.BattlegroundsAuthoring.PrintError("EditTimeSceneConfiguration: Attempted during FirstRunState step to apply state " + (object) stateIndex + " which is undefined.");
            return;
          }
          this.ApplyState(this.Configurations[stateIndex].FirstRunState, runStates);
        }
        for (int index = 0; index < this.Configurations[stateIndex].ObjectsToDeactivate.Count; ++index)
        {
          if ((UnityEngine.Object) this.Configurations[stateIndex].ObjectsToDeactivate[index] == (UnityEngine.Object) null)
            Log.BattlegroundsAuthoring.PrintWarning(string.Format("Attempting to Deactivate Configuration {0} Object {1} but it is null", (object) stateIndex, (object) index));
          else
            this.Configurations[stateIndex].ObjectsToDeactivate[index].SetActive(false);
        }
        for (int index = 0; index < this.Configurations[stateIndex].ObjectsToActivate.Count; ++index)
        {
          if ((UnityEngine.Object) this.Configurations[stateIndex].ObjectsToActivate[index] == (UnityEngine.Object) null)
            Log.BattlegroundsAuthoring.PrintWarning(string.Format("Attempting to Activate Configuration {0} Object {1} but it is null", (object) stateIndex, (object) index));
          else
            this.Configurations[stateIndex].ObjectsToActivate[index].SetActive(true);
        }
        for (int index = 0; index < this.Configurations[stateIndex].ComponentsToDeactivate.Count; ++index)
        {
          if ((UnityEngine.Object) this.Configurations[stateIndex].ComponentsToDeactivate[index] == (UnityEngine.Object) null)
            Log.BattlegroundsAuthoring.PrintWarning(string.Format("Attempting to Deactivate Configuration {0} Component {1} but it is null", (object) stateIndex, (object) index));
          else
            this.Configurations[stateIndex].ComponentsToDeactivate[index].enabled = false;
        }
        for (int index = 0; index < this.Configurations[stateIndex].ComponentsToActivate.Count; ++index)
        {
          if ((UnityEngine.Object) this.Configurations[stateIndex].ComponentsToActivate[index] == (UnityEngine.Object) null)
            Log.BattlegroundsAuthoring.PrintWarning(string.Format("Attempting to Activate Configuration {0} Component {1} but it is null", (object) stateIndex, (object) index));
          else
            this.Configurations[stateIndex].ComponentsToActivate[index].enabled = true;
        }
      }
    }
  }

  [Serializable]
  public class SceneConfiguration
  {
    [Tooltip("A readable name for this state for the dropdown")]
    public string ConfigurationName;
    [Tooltip("The index of a state to run (zero-indexed) before running this one. -1 = don't run a state before this. Used to group common operations. This property is recursive, i.e. if that state has a FirstRunState value, that state's FirstRunState will be run first.")]
    public int FirstRunState = -1;
    [Tooltip("List of references to game objects whose active-self property will be set false.")]
    public List<GameObject> ObjectsToActivate = new List<GameObject>();
    [Tooltip("List of references to game objects whose active-self property will be set true.")]
    public List<GameObject> ObjectsToDeactivate = new List<GameObject>();
    [Tooltip("List of references to components whose enabled property will be set false.")]
    public List<Behaviour> ComponentsToActivate = new List<Behaviour>();
    [Tooltip("List of references to components whose enabled property will be set true.")]
    public List<Behaviour> ComponentsToDeactivate = new List<Behaviour>();
    [Tooltip("Check this to suppress it from appearing in the dropdown")]
    public bool Hidden;
  }
}
